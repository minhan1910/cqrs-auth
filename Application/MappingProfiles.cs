using AutoMapper;
using Common.Requests.Employees;
using Common.Responses.Employees;
using Domain;

namespace Application
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<CreateEmployeeRequest, Employee>();            
            CreateMap<UpdateEmployeeRequest, Employee>();            
            CreateMap<Employee, EmployeeResponse>();            
            CreateMap<Employee, Employee>();            
        }
    }
}