using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

using AutoMapper;
using Xunit;
using Moq;

using School.API.Controllers;
using School.API.Profiles;
using School.API.Services;
using School.Data;
using School.Data.Models;
using School.Data.Repositories;

namespace School.API.Tests
{
    public class StudentsControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUOW;
        private readonly Mock<IStudentsService> _mockStudentsService;
        private readonly Mock<ILogger<StudentsController>> _mockLogger;
        private readonly IMapper _mapper;

        private StudentsController _controller;

        public StudentsControllerTests()
        {
            _mockUOW = new Mock<IUnitOfWork>();
            _mockLogger = new Mock<ILogger<StudentsController>>();
            _mockStudentsService = new Mock<IStudentsService>();

            var myProfile = new AutoMapperProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            IMapper _mapper = new Mapper(configuration);

            if (_mapper == null)
                throw new NullReferenceException("unable to create a mapper");

            _controller = new StudentsController(_mockLogger.Object, _mockStudentsService.Object, _mapper);

        }

        [Fact()]
        public async Task StudentsController_Get_Students_Success()
        {
            // Arrange
            _mockStudentsService.Setup(service => service.GetAllStudentTranscripts()).Returns(Task.Run(() => StudentsServiceTests.CreateTranscripts()));

            // Act
            var result = await _controller.GetStudents() as ActionResult<IEnumerable<StudentGPA>>;
            List<StudentGPA> students = ((Microsoft.AspNetCore.Mvc.ObjectResult)result.Result).Value as List<StudentGPA>;

            // Assert
            Debug.Print("Stop here");
            Assert.IsType<OkObjectResult>(result.Result);
            Assert.True(students != null);
            Assert.True(students.Count > 0);
        }

        [Fact()]
        public async Task StudentsController_Get_Students_NotFound()
        {
            // Arrange

            _mockStudentsService.Setup(service => service.GetAllStudentTranscripts()).Returns(Task.Run(() =>
                                    {
                                        return new ServiceResponse<List<StudentTranscript>>
                                        {
                                            Data = null,
                                            Success = false,
                                            Message = "No students found in the DB"
                                        };
                                    }));

            // Act
            var result = await _controller.GetStudents() as ActionResult<IEnumerable<StudentGPA>>;
            List<StudentGrade> students = result.Value as List<StudentGrade>;

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);

            // Validate that it is sending School.API.Utility.NotFoundResponse 
            Assert.IsType<School.API.Utility.BadRequestResponse>(((Microsoft.AspNetCore.Mvc.ObjectResult)result.Result).Value);
        }

        /// <summary>
        /// This is an edge case that probably only happens if there are no students in the DB.
        /// In fact, it may never happen.  However, if it does we will catch this edge case
        /// where the StudentServices.GetAllStudentTranscripts returns with a success status but
        /// delivers no transcripts.  Could be caused by someone messing up the GetAllStudentTranscripts algorithm as well.
        /// </summary>
        /// <returns></returns>
        [Fact()]
        public async Task StudentsController_Get_Students_Success_But_NotFound()
        {
            // Arrange

            _mockStudentsService.Setup(service => service.GetAllStudentTranscripts()).Returns(Task.Run(() =>
            {
                return new ServiceResponse<List<StudentTranscript>>
                {
                    Data = null,
                    Success = true,
                    Message = "Success = true means GetAllStudentTranscripts ran without errors but returned no data somehow"
                };
            }));

            // Act
            var result = await _controller.GetStudents() as ActionResult<IEnumerable<StudentGPA>>;
            List<StudentGrade> students = result.Value as List<StudentGrade>;

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);

            // Validate that it is sending School.API.Utility.NotFoundResponse 
            Assert.IsType<School.API.Utility.BadRequestResponse>(((Microsoft.AspNetCore.Mvc.ObjectResult)result.Result).Value);
        }


        [Fact()]
        public async Task StudentsController_Get_StudentTranscript_Success()
        {
            int id = 2;

            // Arrange
            _mockStudentsService.Setup(service => service.GetTranscript(id)).Returns(Task.Run(() => StudentsServiceTests.CreateTranscript(id)));

            // Act
            var result = await _controller.GetTranscript(id) as ActionResult<StudentTranscript>;
            StudentTranscript transcript = ((Microsoft.AspNetCore.Mvc.ObjectResult)result.Result).Value as StudentTranscript;

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
            Assert.True(transcript != null);
            Assert.True(transcript.studentId == id);
        }

        /// <summary>
        /// This is an edge case that probably only happens if there are no students in the DB.
        /// In fact, it may never happen.  However, if it does we will catch this edge case
        /// where the StudentServices.GetTranscript returns with a success status but
        /// delivers no transcript.  Could be caused by someone messing up the GetTranscript algorithm as well.
        /// </summary>
        /// <returns></returns>
        [Fact()]
        public async Task StudentsController_Get_StudentTranscript_Success_But_NotFound()
        {
            // Arrange
            int id = 2;

            ServiceResponse<StudentTranscript> response = new ServiceResponse<StudentTranscript>
            {
                Data = null,   // <<< We have no data...
                Success = true, //>>> but we are saying we are returning successfully from requesting this students data
                Message = $"Get:/students/{id}/transcript: Unknown error condition. Student found but no data returned"
            };

            _mockStudentsService.Setup(service => service.GetTranscript(id)).Returns(Task.Run(() => { return response; }));

            // Act
            var result = await _controller.GetTranscript(id) as ActionResult<StudentTranscript>;

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);

            // Validate that it is sending School.API.Utility.NotFoundResponse 
            Assert.IsType<School.API.Utility.BadRequestResponse>(((Microsoft.AspNetCore.Mvc.ObjectResult)result.Result).Value);
        }

        [Fact()]
        public async Task StudentsController_Get_StudentTranscript_NotFound()
        {
            int id = 2;

            ServiceResponse<StudentTranscript> response = new ServiceResponse<StudentTranscript>
            {
                Data = null,
                Success = false,
                Message = $"Get:/students/{id}/transcript: Student does not exist"
            };

            _mockStudentsService.Setup(service => service.GetTranscript(id)).Returns(Task.Run(() => { return response; }));


            // Act
            var result = await _controller.GetTranscript(id) as ActionResult<StudentTranscript>;
            StudentTranscript transcript = result.Value as StudentTranscript;

            // Assert
            Assert.True(result != null, "StudentsController.GetStudentTranscript should not return a null response");
            Assert.IsType<NotFoundObjectResult>(result.Result);

            // Validate that it is sending School.API.Utility.NotFoundResponse 
            Assert.IsType<School.API.Utility.NotFoundResponse>(((Microsoft.AspNetCore.Mvc.ObjectResult)result.Result).Value);
        }

        [Fact()]
        public async Task StudentsController_Post_PostGrade_Success()
        {

            ServiceResponse<CourseGrade> response = new ServiceResponse<CourseGrade>
            {
                Data = new CourseGrade { gradeId = 1, courseId = 2032, grade = 4 },
                Success = true,
                Message = $"One of several failures occured inside studentsService.PostGrade"
            };

            CourseGrade grade = new CourseGrade { courseId = 2032, studentId = 2 };

            _mockStudentsService.Setup(service => service.PostGrade(grade)).Returns(Task.Run(() => { return response; }));


            // Act
            var result = await _controller.PostGrade(grade) as ActionResult<CourseGrade>;

            CourseGrade newGrade = ((Microsoft.AspNetCore.Mvc.ObjectResult)result.Result).Value as CourseGrade;

            // Assert
            Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.True(newGrade != null);

        }

        [Fact()]
        public async Task StudentsController_Post_PostGrade_Failure()
        {
            CourseGrade grade = new CourseGrade { gradeId = 1, courseId = 2032, studentId = 2 };

            ServiceResponse<CourseGrade> response = new ServiceResponse<CourseGrade>
            {
                Data = grade,
                Success = false, // we failed to post the new grade for some reason
                Message = $"One of several failures occured inside studentsService.PostGrade"
            };

            _mockStudentsService.Setup(service => service.PostGrade(grade)).Returns(Task.Run(() => { return response; }));

            // Act
            var result = await _controller.PostGrade(grade) as ActionResult<CourseGrade>;

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);

            // Validate that it is sending School.API.Utility.NotFoundResponse 
            Assert.IsType<School.API.Utility.BadRequestResponse>(((Microsoft.AspNetCore.Mvc.ObjectResult)result.Result).Value);

        }
    }
}
