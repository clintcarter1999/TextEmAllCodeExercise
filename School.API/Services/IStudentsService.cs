using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using School.Data.Models;

namespace School.API.Services
{
    public interface IStudentsService
    {
        Task<List<StudentTranscript>> GetAllStudentTranscripts();

        Task<StudentTranscript> GetTranscript(int id);

        Task<Tuple<CourseGrade, string>> PostGrade(CourseGrade courseGrade);

        bool StudentExists(int id);

        bool CourseExists(int id);

        decimal? CalculateGPA(List<StudentTranscriptGrade> studentGrades);
    }
}