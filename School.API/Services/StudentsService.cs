using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using School.Data.Context;
using School.Data.Models;

namespace School.API.Services
{
    /// <summary>
    /// Provides service layer abstraction for interacting with the SchoolContext to separate concerns.
    /// Allows controllers to follow Single Responsibility Principle (get a request, let this service handle that, then provide a response)
    /// </summary>
    public class StudentsService : IStudentsService
    {
        private readonly SchoolContext _context;
        private readonly ILogger<StudentsService> _log;
        private readonly IMapper _mapper;

        public StudentsService(SchoolContext context, ILogger<StudentsService> logger, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _log = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Returns the transcript for every student in the database.  Careful, this method could
        /// take a long time to return results as the number of students grows.
        /// </summary>
        /// <returns>List of StudentTranscript</returns>
        public async Task<List<StudentTranscript>> GetAllStudentTranscripts()
        {
            _log.LogDebug("StudentsService: Getting all student transcripts");

            List<StudentTranscript> transcripts = await (
                            (
                                from student in _context.Person
                                join courseGrade in _context.StudentGrade on student.PersonId equals courseGrade.StudentId
                                select new StudentTranscript
                                {
                                    studentId = student.PersonId,
                                    firstName = student.FirstName,
                                    lastName = student.LastName,
                                    gpa = 0,
                                    grades = (
                                                 from grade in _context.StudentGrade
                                                 join course in _context.Course on grade.CourseId equals course.CourseId
                                                 where grade.StudentId == student.PersonId && grade.Grade != null
                                                 select new StudentTranscriptGrade
                                                 {
                                                     courseId = grade.CourseId,
                                                     title = course.Title,
                                                     credits = course.Credits,
                                                     grade = grade.Grade
                                                 }
                                             ).ToList()
                                }
                            ).ToListAsync());

            if (transcripts == null)
            {
                _log.LogWarning("StudentService: No student transcripts found.  Returning null");
                return null;
            }

            _log.LogDebug($"StudentsService: Calculating GPA for {transcripts.Count} of students");

            // See massive comment in CalculateGPA for Challenge 2.2
            transcripts.Select(s => s.gpa = CalculateGPA(s.grades)).ToList();

            /*
             *   Note 1: grouping in LINQ-to-SQL is very inefficient, because for each group a separate query 
             *           is executed to populate the items in the group. Using the in memory List (LINQ-to-objects) to improve performance.
             * 
             *   Note 2: The *.Select(x => x.FirstOrDefault()) below acts like SELECT DISTINCT
             */

            _log.LogDebug($"StudentsService: Ordering by StudentId");
            transcripts = transcripts.GroupBy(s => s.studentId).Select(x => x.FirstOrDefault()).OrderBy(s => s.studentId).ToList();

            _log.LogDebug($"StudentsService: Returning {transcripts.Count} student transcripts");

            return transcripts;
        }

        /// <summary>
        /// Returns the Transcript for a Student based on the Id value provided.
        /// I suggest calling StudentExists(id) prior to calling this method to verify
        /// that the student exists in this school.  However, this method will return a null
        /// if the student does not exist.
        /// </summary>
        /// <param name="id">Student Id</param>
        /// <returns>StudentTranscript (or null if not found).</returns>
        public async Task<StudentTranscript> GetTranscript(int id)
        {
            _log.LogDebug($"StudentsService: Getting transcript for studentId = {id}");

            var query = await (  
                                from student in _context.Person
                                join studentGrade in _context.StudentGrade on student.PersonId equals studentGrade.StudentId
                                where student.PersonId == id
                                select new StudentTranscript
                                {
                                    studentId = student.PersonId,
                                    firstName = student.FirstName,
                                    lastName = student.LastName,
                                    gpa = null,
                                    grades = null
                                }
                             ).ToListAsync();

            StudentTranscript transcript = query?.FirstOrDefault();

            if (transcript != null)
            {
                _log.LogDebug($"StudentsService: Generating the list of grades for each student...");

                //
                // Generate the Grades List for this student which is needed to calculate the GPA below.
                //
                transcript.grades = await (from studentGrade in _context.StudentGrade
                                     join course in _context.Course on studentGrade.CourseId equals course.CourseId
                                     where studentGrade.StudentId == id && studentGrade.Grade != null
                                     select new StudentTranscriptGrade
                                     {
                                         courseId = course.CourseId,
                                         title = course.Title,
                                         credits = course.Credits,
                                         grade = studentGrade.Grade
                                     }).OrderBy(course => course.courseId).ToListAsync();

                _log.LogDebug($"StudentsService: Calculating GPA for this student");

                //
                // See massive comment in CalculateGPA for Challenge 2.2
                //
                transcript.gpa = CalculateGPA(transcript.grades);

                _log.LogDebug($"StudentsService: Returning transcript for studentId = {id}");

                return transcript;
            }


            return null;
        }

        public async Task<Tuple<CourseGrade, string>> PostGrade(CourseGrade courseGrade)
        {
            string errorMessage = null;

            if (!StudentExists(courseGrade.studentId))
            {
                errorMessage = $"Student Id = {courseGrade.studentId} does not exist\r\n";
                return new Tuple<CourseGrade, string>(null, errorMessage);
            }

            if (!CourseExists(courseGrade.courseId))
            {
                errorMessage += $"Course Id = {courseGrade.courseId} does not exist\r\n";
                return new Tuple<CourseGrade, string>(null, errorMessage);
            }

            //
            // Note: I have a constaint on the grade column which will prevent inserts not meeting the null or range of 0 - 4
            // However, I prefer not to hit the database if are not in a valid state because the SQL constraint error
            // may not provide enough information (other than the name of the constraint violated).
            // I also have Model validation through Data Annotations on the models exposed to the world.
            // This is just bullet proofing in case the user gets around Model validation somehow
            //
            if (courseGrade.grade != null)
            {
                bool validRange = (courseGrade.grade >= 0 && courseGrade.grade <= 4);

                if (!validRange)
                {
                    errorMessage = $"Invalid Grade Value {courseGrade.grade}: A value of null or between 0 and 4 is required.\r\n";
                    return new Tuple<CourseGrade, string>(null, errorMessage);
                }
            }

            //
            // We cannot have more than one grade per course per student.  
            //
            var existingGrade = _context.StudentGrade.Where(e => e.CourseId == courseGrade.courseId && e.StudentId == courseGrade.studentId).FirstOrDefault();

            if (existingGrade != null)
            {
                errorMessage = $"The student already has a grade entered.  You will need to use a PUT action rather than a POST";
                return new Tuple<CourseGrade, string>(null, errorMessage);
            }

            StudentGrade newGrade = new StudentGrade
            {
                StudentId = courseGrade.studentId,
                CourseId = courseGrade.courseId,
                Grade = courseGrade.grade
            };

            _context.StudentGrade.Add(newGrade);

            await _context.SaveChangesAsync();

            CourseGrade newCourseGrade = _mapper.Map<CourseGrade>(newGrade);

            return new Tuple<CourseGrade, string>(newCourseGrade, errorMessage);
        }

        /// <summary>
        /// Looks for a Person that is also a student with the given Person Id.
        /// Note that this method returns false if the Id exists but the Discriminator != "Student".
        /// </summary>
        /// <param name="id">The id of the student</param>
        /// <returns>True if the student exists with the given id, false if no student with the given Id exists</returns>
        public bool StudentExists(int id)
        {
            return _context.Person.Any(e => e.PersonId == id && e.Discriminator.ToLower() == "student");
        }

        /// <summary>
        /// Might make more sense to have this in a CoursesService service layer rather than students.
        /// </summary>
        /// <param name="id">Course.CourseId</param>
        /// <returns>True if the Course exists with the given id, false if no Course with the given Id exists</returns>
        public bool CourseExists(int id)
        {
            return _context.Course.Any(e => e.CourseId == id);
        }

        /// <summary>
        /// GPA is a weighted average based on the credit hours awarded for each course.
        /// Null value grades not considered in the Weighted Sum.
        /// </summary>
        /// <param name="studentGrades">List of Grade objects</param>
        /// <returns>Weighted average GPA</returns>
        public decimal? CalculateGPA(List<StudentTranscriptGrade> studentGrades)
        {
            #region Challenge 2.2 Why I used C# rather than Stored Proc for this

            //
            //
            // Challenge 2.2 There are no restrictions on where the GPA is calculated. 
            //               Include a comment in your code to justify your approach to calculating the GPA.
            // 
            // My Answer: 
            //      Stored Proc: Easier to patch a release with stored proc script vs deploying new Dlls
            //                   Performance is better on the initial query (and then only marginally better/same/worse than EF Query on subsequent queries)
            //                   
            //     
            //      C# Code:
            //                   * The logic of this computed value is resisent to change and easy to test.  
            //                   * The likely hood that a DBA will need to fix the logic in the stored proc for this is negligiable
            //                   * The logic required to compute the weighted sum in a stored procedure is more complicated and prone to issues up front
            //                   * I should code to my strenth (which is not tuning stored procedures) unless the validated requirement is to use a Stored Procedure
            //                   * The logic is portable across multiple DBs such as SQL Server, Oracle, Postgres, etc...  Might have to
            //                      keep multiple versions of the stored procedure if we have to support multiple database types:
            //                   * The need to generate a GPA on every single student is not a high use activity.
            //                   * Much easier to find the code, debug the code, and fix the code in C#
            //                   * If a DBA has to fix this calculation it's most likely due to column/table changes which is going to require us to 
            //                     generate a new API version release anyway.
            //                   * If using migrations in EF/ORM then using a stored proc for computed columns gets a bit tricky causing maintenance costs to rise
            //                   
            //
            #endregion Challenge 2.2 Why I used C# rather than Stored Proc for this

            decimal weightedSum = 0;
            decimal weight = 0;

            // Design Decision - When calculating GPA, do not include NULL grades.
            // These are grades the teacher has not yet recorded for the student and should not
            // weigh against them.

            foreach (StudentTranscriptGrade courseGrade in studentGrades)
            {
                if (courseGrade.grade != null)
                {
                    // Credits should never be <= 0.
                    // This is just defensive coding.  
                    // Probably should Assert this so that we hopefully find it in test before getting to release
                    //
                    // TODO: Add Assert with Logging to hopefuly catch this in test if it happens.
                    // TODO: I suggest adding a constraint on the Credits column to not allow null or <= 0

                    if (courseGrade.credits > 0)
                        weight += courseGrade.credits;
                    else
                        weight += 1;
               
                    weightedSum += (courseGrade.grade?? 1) * courseGrade.credits;
                }
            }

            // Design Decision - We are returning a null if there are no grades to compute.
            if (weight + weightedSum == 0)
                return null;

            if (weight == 0)
            {
                // Todo: Add Error logging here.
                throw new Exception("Unable to calculate the GPA due to course credits = 0");
            }

            return Decimal.Round(weightedSum / weight, 2);
        }
    }
}
