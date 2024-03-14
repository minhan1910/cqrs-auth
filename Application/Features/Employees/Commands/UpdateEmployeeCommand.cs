using Application.Pipelines;
using Application.Services.Employees;
using AutoMapper;
using Common.Requests.Employees;
using Common.Responses.Employees;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Employees.Commands
{
    public sealed class UpdateEmployeeCommand : IRequest<IResponseWrapper>, IValidateMe
    {
        public UpdateEmployeeRequest UpdateEmployeeRequest { get; set; }
    }

    public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, IResponseWrapper>
    {
        private readonly IEmployeeService _employeeService;
        private readonly IMapper _mapper;

        public UpdateEmployeeCommandHandler(IEmployeeService employeeService, IMapper mapper)
        {
            _employeeService = employeeService;
            _mapper = mapper;
        }

        public async Task<IResponseWrapper> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
        {
            var employeeFromDb = await _employeeService.GetEmployeeByIdAsync(request.UpdateEmployeeRequest.Id);
                
            if (employeeFromDb is not null)
            {
                var mappedEmployee = _mapper.Map(request.UpdateEmployeeRequest, employeeFromDb);
                var updatedEmployee = await _employeeService.UpdateEmployeeAsync(mappedEmployee);
                var updatedEmployeeResponse = _mapper.Map<EmployeeResponse>(updatedEmployee);

                return await ResponseWrapper<EmployeeResponse>.SuccessAsync(updatedEmployeeResponse, message: "Updated Employee Successfully!");
            }

            return await ResponseWrapper<EmployeeResponse>.FailAsync("Employee does not exist.");

        }
    }
}