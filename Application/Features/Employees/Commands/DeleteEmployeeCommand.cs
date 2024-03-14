using Application.Services.Employees;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Employees.Commands
{
    public sealed record DeleteEmployeeCommand(int EmployeeId) : IRequest<IResponseWrapper> { }

    public class DeleteEmployeeCommandHandler : IRequestHandler<DeleteEmployeeCommand, IResponseWrapper>
    {
        private readonly IEmployeeService _employeeService;

        public DeleteEmployeeCommandHandler(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public async Task<IResponseWrapper> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
        {    
            var exsitingEmployee = await _employeeService.GetEmployeeByIdAsync(request.EmployeeId);

            if (exsitingEmployee is null)
            {
                return await ResponseWrapper.FailAsync("Employee is not found for deleting.");
            }

            var statusDeleted = await _employeeService.DeleteEmployeeAsync(exsitingEmployee);

            if (statusDeleted > 0)
            {
                return await ResponseWrapper<int>.SuccessAsync(exsitingEmployee.Id, message: "Delete Employee Successfully!");
            }

            return await ResponseWrapper.FailAsync("Delete Employee Failed.");
        }
    }
}