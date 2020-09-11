using Xunit;
using School.API.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using School.Data.Repositories;
using Microsoft.Extensions.Logging;
using AutoMapper;
using School.API.Controllers;
using School.Data;
using School.Data.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using School.API.Profiles;

namespace School.API.Services.Tests
{
    public class StudentsServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUOW;
        private readonly Mock<IStudentsService> _mockStudentsService;
        private readonly Mock<ILogger<StudentsController>> _mockLogger;
        private readonly IMapper _mapper;

        private readonly StudentsController _controller;

        public StudentsServiceTests()
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
        public async Task GetStudents_Success_Test()
        {
            // Arrange
            _mockStudentsService.Setup(service => service.GetAllStudentTranscripts()).Returns(Task.Run(() => CreateTranscripts()));

            // Act
            var result = await _controller.GetStudents() as ActionResult<IEnumerable<StudentGPA>>;
            List<StudentGrade> students = result.Value as List<StudentGrade>;

            // Assert
            Debug.Print("Stop here");
            Assert.IsType<OkObjectResult>(result);
            Assert.True(students != null);
            Assert.True(students.Count == 1);
        }

        [Fact()]
        public async Task GetExistingStudentTranscript_Success()
        {
            int id = 2;

            // Arrange
            _mockStudentsService.Setup(service => service.GetTranscript(id)).Returns(Task.Run(() => CreateTranscript(id)));
            _mockStudentsService.Setup(service => service.StudentExists(id)).Returns(true);

            // Act
            var result = await _controller.GetTranscript(id) as ActionResult<StudentTranscript>;
            StudentTranscript student = result.Value as StudentTranscript;

            // Assert
            Debug.Print("Stop here");
            Assert.IsType<OkObjectResult>(result);
            Assert.True(student != null);
            Assert.True(student.studentId == id);
        }

        [Fact()]
        public async Task GetStudentTranscript_StudentDoesNotExist()
        {
            int id = 2;

            // Arrange
            //TODO: This is obviously wrong
            _mockStudentsService.Setup(service => service.StudentExists(id)).Returns(false);
            _mockStudentsService.Setup(service => service.GetTranscript(id)).Returns(Task.Run(() => CreateTranscript(id)));

            // Act
            var result = await _controller.GetTranscript(id) as ActionResult<StudentTranscript>;
            StudentTranscript student = result.Value as StudentTranscript;

            // Assert
            Debug.Print("Stop here");
            Assert.IsType<OkObjectResult>(result);
            Assert.True(student != null);
            Assert.True(student.studentId == id);
        }

        private ServiceResponse<StudentTranscript> CreateTranscript(int id)
        {
            ServiceResponse<StudentTranscript> response =  new ServiceResponse<StudentTranscript>
            {
                Success = true,
                Message = "Testing",
                Data = new StudentTranscript
                {
                    studentId = id,
                    firstName = "Clint",
                    lastName = "Carter",
                    gpa = 4,
                    grades = new List<StudentTranscriptGrade> 
                    {
                        new StudentTranscriptGrade
                        {
                            courseId = 2042,
                            title = "Underwater basket weaving",
                            credits = 3,
                            grade = 4
                        }
                    }
                }
            };

            return response;
        }

        private ServiceResponse<List<StudentTranscript>> CreateTranscripts()
        {
            ServiceResponse<List<StudentTranscript>> response = new ServiceResponse<List<StudentTranscript>>
            {
                Success = true,
                Message = "Testing",
                Data = new List<StudentTranscript>
                {
                    new StudentTranscript
                    {

                        studentId = 1,
                        firstName = "Clint",
                        lastName = "Carter",
                        gpa = 4,
                        grades = new List<StudentTranscriptGrade>
                        {
                            new StudentTranscriptGrade
                            {
                                courseId = 2042,
                                title = "Underwater basket weaving",
                                credits = 3,
                                grade = 4
                            }
                        }
                    },
                    new StudentTranscript
                    {

                        studentId = 2,
                        firstName = "Jeff",
                        lastName = "Ogata",
                        gpa = 4,
                        grades = new List<StudentTranscriptGrade>
                            {
                                new StudentTranscriptGrade
                                {
                                    courseId = 2042,
                                    title = "Underwater basket weaving",
                                    credits = 3,
                                    grade = 4
                                }
                            }
                    }
                }
            

            };

            return response;
        }

        //[Fact()]
        //public void GetAllStudentTranscriptsTest()
        //{
        //    Assert.True(false, "This test needs an implementation");
        //}

        //[Fact()]
        //public void GetTranscriptTest()
        //{
        //    Assert.True(false, "This test needs an implementation");
        //}

        //[Fact()]
        //public void PostGradeTest()
        //{
        //    Assert.True(false, "This test needs an implementation");
        //}

        //[Fact()]
        //public void StudentExistsTest()
        //{
        //    Assert.True(false, "This test needs an implementation");
        //}

        //[Fact()]
        //public void CourseExistsTest()
        //{
        //    Assert.True(false, "This test needs an implementation");
        //}

        //[Fact()]
        //public void CalculateGPATest()
        //{
        //    Assert.True(false, "This test needs an implementation");
        //}
    }
}