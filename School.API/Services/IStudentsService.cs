using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using School.Data;
using School.Data.Models;

namespace School.API.Services
{
    public interface IStudentsService
    {
        Task<ServiceResponse<List<StudentTranscript>>> GetAllStudentTranscripts();

        Task<ServiceResponse<StudentTranscript>> GetTranscript(int id);

        Task<ServiceResponse<CourseGrade>> PostGrade(CourseGrade courseGrade);

        bool StudentExists(int id);

        bool CourseExists(int id);

        decimal? CalculateGPA(List<StudentTranscriptGrade> studentGrades);
    }
}