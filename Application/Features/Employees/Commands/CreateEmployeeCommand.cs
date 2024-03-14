using Application.Pipelines;
using Application.Services.Employees;
using AutoMapper;
using Common.Requests.Employees;
using Common.Responses.Employees;
using Common.Responses.Wrappers;
using Domain;
using MediatR;

namespace Application.Features.Employees.Commands
{
    public class CreateEmployeeCommand : IRequest<IResponseWrapper>, IValidateMe
    {
        public CreateEmployeeRequest CreateEmployeeRequest { get; set; }
    }

    public class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, IResponseWrapper>
    {

        private readonly IEmployeeService _employeeService;
        private readonly IMapper _mapper;

        public CreateEmployeeCommandHandler(IEmployeeService employeeService, 
                                            IMapper mapper)
        {
            _employeeService = employeeService;
            _mapper = mapper;
        }

        public async Task<IResponseWrapper> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
        {
            var employeeRequest = _mapper.Map<Employee>(request.CreateEmployeeRequest);
            var employee = await _employeeService.CreateEmployeeAsync(employeeRequest);

            if (employee.Id > 0)
            {
                var employeeResponse = _mapper.Map<EmployeeResponse>(employee);
                return await ResponseWrapper<EmployeeResponse>
                    .SuccessAsync(employeeResponse, message: "Employee Created Successfully!");
            }

            return await ResponseWrapper.FailAsync("Create new employee failed.");
        }
    }
}