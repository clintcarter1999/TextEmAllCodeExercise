using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using School.Data.Models;

namespace School.API.Profiles
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<StudentTranscript, StudentGPA>();

            CreateMap<StudentGrade, CourseGrade>()
                .ForMember(dest => dest.gradeId, opt => opt.MapFrom(src => src.EnrollmentId))
                .ReverseMap()
                .ForMember(dest => dest.EnrollmentId, opt => opt.MapFrom(src => src.gradeId));

        }
    }
}


