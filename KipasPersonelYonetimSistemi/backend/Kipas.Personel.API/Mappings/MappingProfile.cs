using AutoMapper;
using Kipas.Personel.API.DTOs;
using Kipas.Personel.API.Entities;

namespace Kipas.Personel.API.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Employee, EmployeeDto>()
                .ForMember(
                    destination =>
                        destination.DepartmentName,
                    options => options.MapFrom(
                        source =>
                            source.Department.Name));

            CreateMap<Employee, EmployeeDetailDto>()
                .ForMember(
                    destination =>
                        destination.DepartmentName,
                    options => options.MapFrom(
                        source =>
                            source.Department.Name));

            CreateMap<CreateEmployeeDto, Employee>();

            CreateMap<UpdateEmployeeDto, Employee>();

            CreateMap<Department, DepartmentDto>();
        }
    }
}