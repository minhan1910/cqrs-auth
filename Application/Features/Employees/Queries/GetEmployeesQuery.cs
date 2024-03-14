using Application.Services.Employees;
using AutoMapper;
using Common.Responses.Employees;
using Common.Responses.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Employees.Queries
{
    public sealed record GetEmployeesQuery : IRequest<IResponseWrapper>
    {
    }

    public class GetEmployeesQueryHandler : IRequestHandler<GetEmployeesQuery, IResponseWrapper>
    {
        private readonly IEmployeeService _employeeService;
        private readonly IMapper _mapper;

        public GetEmployeesQueryHandler(IEmployeeService employeeService, IMapper mapper)
        {
            _employeeService = employeeService;
            _mapper = mapper;
        }

        public async Task<IResponseWrapper> Handle(GetEmployeesQuery request, CancellationToken cancellationToken)
        {
            var employees = await _employeeService.GetEmployeeListAsync();

            if (!employees.Any())
            {
                return await ResponseWrapper.FailAsync("No Employees were found.");
            }

            var emploeeResponse = _mapper.Map<List<EmployeeResponse>>(employees);
            
            return await ResponseWrapper<List<EmployeeResponse>>.SuccessAsync(emploeeResponse);
        }
    }
}
