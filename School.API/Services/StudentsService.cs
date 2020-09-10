using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using School.Data;
using School.Data.Context;
using School.Data.Models;
using School.Data.Repositories;

namespace School.API.Services
{
    /// <summary>
    /// Provides service layer abstraction for interacting with the SchoolContext to separate concerns.
    /// Allows controllers to follow Single Responsibility Principle (get a request, let this service handle that, then provide a response)
    /// </summary>
    public class StudentsService : IStudentsService
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<StudentsService> _log;
        private readonly IMapper _mapper;

        public StudentsService(IUnitOfWork uow, ILogger<StudentsService> logger, IMapper mapper)
        {
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            _log = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Returns the transcript for every student in the database.  Careful, this method could
        /// take a long time to return results as the number of students grows.
        /// </summary>
        /// <returns>List of StudentTranscript</returns>
        public async Task<ServiceResponse<List<StudentTranscript>>> GetAllStudentTranscripts()
        {
            _log.LogDebug("StudentsService: Getting all student transcripts");

            //
            // Just making the query a bit more readable than having _uow.Context all over the place
            // by adding these variables representing each Table in the DB
            //
            DbSet<Person> personTable = _uow.Context.Person;
            DbSet<StudentGrade> studentGradeTable = _uow.Context.StudentGrade;
            DbSet<Course> courseTable = _uow.Context.Course;

            List<StudentTranscript> transcripts = await (
                            (
                                from student in personTable
                                join courseGrade in studentGradeTable on student.PersonId equals courseGrade.StudentId
                                select new StudentTranscript
                                {
                                    studentId = student.PersonId,
                                    firstName = student.FirstName,
                                    lastName = student.LastName,
                                    gpa = 0,
                                    grades = (
                                                 from grade in studentGradeTable
                                                 join course in courseTable on grade.CourseId equals course.CourseId
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

                return new ServiceResponse<List<StudentTranscript>>
                {
                    Data = null,
                    Success = false,
                    Message = "StudentService: No student transcripts found"
                };
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

            return new ServiceResponse<List<StudentTranscript>>
            {
                Data = transcripts,
                Success = true,
                Message = "Success"
            };
        }

        /// <summary>
        /// Returns the Transcript for a Student based on the Id value provided.
        /// I suggest calling StudentExists(id) prior to calling this method to verify
        /// that the student exists in this school.  However, this method will return a null
        /// if the student does not exist.
        /// </summary>
        /// <param name="id">Student Id</param>
        /// <returns>StudentTranscript (or null if not found).</returns>
        public async Task<ServiceResponse<StudentTranscript>> GetTranscript(int id)
        {
            _log.LogDebug("StudentService.GetTranscript START");

            if (!StudentExists(id))
            {
                string errorMessage = $"Get:/students/{id}/transcript: Student does not exist";

                return new ServiceResponse<StudentTranscript>
                {
                    Data = null,
                    Success = false,
                    Message = errorMessage
                };
            }

            DbSet<Person> personTable = _uow.Context.Person;
            DbSet<StudentGrade> studentGradeTable = _uow.Context.StudentGrade;
            DbSet<Course> courseTable = _uow.Context.Course;

            var query = await (  
                                from student in personTable
                                join studentGrade in studentGradeTable on student.PersonId equals studentGrade.StudentId
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
                transcript.grades = await (from studentGrade in studentGradeTable
                                     join course in courseTable on studentGrade.CourseId equals course.CourseId
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
                // See massive comment regarding the method for calculating the GPA for Challenge 2.2
                //
                transcript.gpa = CalculateGPA(transcript.grades);

                _log.LogDebug($"StudentsService: Returning transcript for studentId = {id}");

                _log.LogDebug("StudentService.GetTranscript END 1");

                return new ServiceResponse<StudentTranscript>
                {
                    Data = transcript,
                    Success = true
                };
            }

            //TODO: Rethink returning a null like this.
            // Perhaps return
            _log.LogError("StudentService.GetTranscript returning null - Perhaps this should be an exception??");

            return new ServiceResponse<StudentTranscript>
            {
                Data = null,
                Success = false,
                Message = "Unable to gather a transcript for unknown reason"
            }; 
        }

        /// <summary>
        /// PostGrade creates a new Course grade for a given student.
        /// </summary>
        /// <param name="courseGrade"></param>
        /// <returns>A Tuple containing CourseGrade and a error message string</returns>
        public async Task<ServiceResponse<CourseGrade>> PostGrade(CourseGrade courseGrade)
        {
            _log.LogDebug("StudentService.PostGrade called");

            string errorMessage = string.Empty;

            if (!ValidateCourseGrade(courseGrade, ref errorMessage))
            {
                _log.LogError($"StudentService.PostGrade: {errorMessage??"Validation Error"}");

                //
                // Validation failed. There was a validation issue preventing this post.
                //
                return new ServiceResponse<CourseGrade>
                {
                    Success = false,
                    Message = errorMessage,
                    Data = courseGrade // returning the value passed into us
                };
            }

            StudentGrade newGrade = new StudentGrade
            {
                StudentId = courseGrade.studentId,
                CourseId = courseGrade.courseId,
                Grade = courseGrade.grade
            };

            _log.LogDebug("StudentService.PostGrade: Adding new CourseGrade");

            //
            // This is interesting:  GetRepository lazy load/adds a StudentGrade repository here when requested.
            // This is something new I am playing with in this code challenge.
            _uow.GetRepository<StudentGrade>().Add(newGrade);

            await Task.Run(() => _uow.Commit());

            CourseGrade newCourseGrade = _mapper.Map<CourseGrade>(newGrade);

            _log.LogDebug($"StudentService.PostGrade: CourseGrade.gradeId = {newCourseGrade.gradeId} Added Successfully ");

            return new ServiceResponse<CourseGrade>
            {
                Success = true,
                Message = errorMessage,
                Data = newCourseGrade
            };
        }

        private bool ValidateCourseGrade(CourseGrade courseGrade, ref string errorMessage)
        {
            _log.LogDebug("StudentService.ValidateCourseGrade called");

            if (!StudentExists(courseGrade.studentId))
            {
                errorMessage = $"Student Id = {courseGrade.studentId} does not exist";

                _log.LogDebug($"StudentService.Validation: {errorMessage}");

                return false;
            }

            if (!CourseExists(courseGrade.courseId))
            {
                errorMessage += $"Course Id = {courseGrade.courseId} does not exist";

                _log.LogDebug($"StudentService.Validation: {errorMessage}");

                return false;
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

                    _log.LogDebug($"StudentService.Validation: {errorMessage}");

                    return false;
                }
            }

            //
            // We cannot have more than one grade per course per student.  
            //
            var existingGrade = _uow.Context.StudentGrade.Where(e => e.CourseId == courseGrade.courseId && e.StudentId == courseGrade.studentId).FirstOrDefault();

            if (existingGrade != null)
            {
                errorMessage = $"The student already has a grade entered for CourseId = {courseGrade.courseId}.  Did you meant to use PUT/Update?";

                _log.LogDebug($"StudentService.Validation: {errorMessage}");

                return false;
            }

            return true;
        }

        /// <summary>
        /// Looks for a Person that is also a student with the given Person Id.
        /// Note that this method returns false if the Id exists but the Discriminator != "Student".
        /// </summary>
        /// <param name="id">The id of the student</param>
        /// <returns>True if the student exists with the given id, false if no student with the given Id exists</returns>
        public bool StudentExists(int id)
        {
            return _uow.Context.Person.Any(e => e.PersonId == id && e.Discriminator.ToLower() == "student");
        }

        /// <summary>
        /// Might make more sense to have this in a CoursesService service layer rather than students.
        /// </summary>
        /// <param name="id">Course.CourseId</param>
        /// <returns>True if the Course exists with the given id, false if no Course with the given Id exists</returns>
        public bool CourseExists(int id)
        {
            return _uow.Context.Course.Any(e => e.CourseId == id);
        }

        /// <summary>
        /// GPA is a weighted average based on the credit hours awarded for each course.
        /// Null value grades not considered in the Weighted Sum.
        /// </summary>
        /// <param name="studentGrades">List of Grade objects</param>
        /// <returns>Weighted average GPA</returns>
        public decimal? CalculateGPA(List<StudentTranscriptGrade> studentGrades)
        {
            // NOTE: The lack of logging here is because this can get called thousands of times for each query.
            // It would bloat the log.

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
