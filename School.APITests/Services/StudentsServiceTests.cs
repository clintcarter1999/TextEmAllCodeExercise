﻿using System;
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
        public async Task StudentsService_GetAllStudentTranscripts()
        {
            //TODO: Need to mock the DB to test studentServices methods
            await Task.Run(() => Assert.True(true));
        }

        public static ServiceResponse<StudentTranscript> CreateTranscript(int id)
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

        public static ServiceResponse<List<StudentTranscript>> CreateTranscripts()
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