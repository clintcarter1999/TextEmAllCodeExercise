using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using School.API.Services;
using School.Data.Models;
using School.API.Utility;
using Microsoft.Extensions.Logging;
using School.Data.Context;

namespace School.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly SchoolContext _context;
        private readonly ILogger<StudentsController> _log;
        private readonly IStudentsService _studentsService;
        private readonly IMapper _mapper;

        public StudentsController(SchoolContext context, ILogger<StudentsController> logger, IStudentsService studentsService, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context)); ;
            _log = logger ?? throw new ArgumentNullException(nameof(logger));
            _studentsService = studentsService ?? throw new ArgumentNullException(nameof(studentsService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// GET: /students
        /// 
        /// <summary>
        /// Challenge 2: 
        /// 
        /// Get a list of all students providing a subset of information including:
        /// studentId, firstName, lastName, GPA
        /// </summary>
        /// <returns>IEnumerable of StudentGPA</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentGPA>>> GetStudents()
        {
            _log.LogInformation("Get:/students called");

            IEnumerable<StudentTranscript> transcripts = await _studentsService.GetAllStudentTranscripts();

            if (transcripts == null)
            {
                _log.LogWarning("Get:/students: unable to retrieve student transcripts. Check the Persons table for students");

                return BadRequest(new BadRequestResponse("Unable to retrieve student transcripts"));
            }

            _log.LogInformation("Get:/students: Mapping transcript to StudentGPA model");

            // Map to a DTO object for the consumer
            List<StudentGPA> students = _mapper.Map<List<StudentGPA>>(transcripts);

            _log.LogInformation($"Get:/students: Returning {students?.Count} Students"); 

            return Ok(students);
        }

        /// GET: /students/{id}/transcript
        /// 
        /// <summary>
        /// Challenge 1: 
        /// 
        /// Get a transcript for the given student id
        /// </summary>
        /// <param name="id">id of the student</param>
        /// <returns>StudentTranscript</returns>
        [HttpGet]
        [Route("{id}/transcript")]
        public async Task<ActionResult<StudentTranscript>> GetTranscript(int id)
        {
            _log.LogInformation($"Get:/students/{id}/transcript called");

            bool existingStudent = _studentsService.StudentExists(id);

            if (!existingStudent)
            {
                _log.LogWarning($"Get:/students/{id}/transcript: Student does not exist");

                return NotFound(new NotFoundResponse($"Student Id {id} does not exist"));
            }

            _log.LogInformation($"Get:/students/{id}/transcript: Getting Transcript");

            StudentTranscript transcript = await _studentsService.GetTranscript(id);

            if (transcript == null)
            {
                _log.LogWarning($"Get:/students/{id}/transcript: The Student Exists but we were unable to build a transcript");

                return BadRequest(new BadRequestResponse($"Unable to find a student transcript for Student Id {id}"));
            }

            _log.LogInformation($"Get:/students/{id}/transcript: Returning Transcript");

            return Ok(transcript);
        }

        /// GET: /students/alltranscripts
        /// 
        /// <summary>
        /// Gets the entire transcript of all students.  This was not part of the Challenge.
        /// I will likely turn this into a paging mechanism.
        /// </summary>
        /// <returns>IEnumerable of StudentTranscript</returns>
        [HttpGet]
        [Route("alltranscripts")]
        public async Task<ActionResult<IEnumerable<StudentTranscript>>> GetAllTranscripts()
        {
            _log.LogInformation($"Get:/students/alltranscripts: Getting all student transcripts");

            IEnumerable<StudentTranscript> transcripts = await _studentsService.GetAllStudentTranscripts();

            if (transcripts == null)
            {
                _log.LogWarning($"Get:/students/alltranscripts: We were unable to get any student transcripts!");

                return BadRequest(new BadRequestResponse("Unable to find any student transcripts"));
            }

            _log.LogInformation($"Get:/students/alltranscripts: Returning {transcripts.Count()} student transcripts");

            return Ok(transcripts);
        }

        ///
        /// POST: students/grades
        /// 
        /// <summary>
        /// Creates a new StudentGrade record for a given student and course.
        /// Does not allow posting multiple grades for the same course and student.
        /// </summary>
        /// <param name="courseGrade">CourseGrade</param>
        /// <returns>CourseGrade</returns>
        [HttpPost]
        [Route("grades")]
        public async Task<ActionResult<CourseGrade>> PostGrade(CourseGrade courseGrade)
        {
            Tuple<CourseGrade, string> response = await _studentsService.PostGrade(courseGrade);

            if (response.Item2 != null)
            {
                // We were unable to create a new StudentGrade record.
                return BadRequest(new BadRequestResponse(response.Item2));
            }

            CourseGrade newCourseGrade = response.Item1 as CourseGrade;

            return CreatedAtAction("PostStudentGrade", newCourseGrade);
        }

        private bool PersonExists(int id)
        {
            return _context.Person.Any(e => e.PersonId == id);
        }

        private bool CourseExists(int id)
        {
            return _context.Course.Any(e => e.CourseId == id);
        }
    }
}
