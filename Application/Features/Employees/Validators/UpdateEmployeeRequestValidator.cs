using Application.Services.Employees;
using Common.Requests.Employees;
using Domain;
using FluentValidation;

namespace Application.Features.Employees.Validators
{
    public class UpdateEmployeeRequestValidator : AbstractValidator<UpdateEmployeeRequest>
    {
        public UpdateEmployeeRequestValidator(IEmployeeService employeeService)
        {
            RuleFor(request => request.Id)
                .MustAsync(async (id, ct) => await employeeService.GetEmployeeByIdAsync(id) is Employee employeeInDb
                    && employeeInDb.Id == id)
                .WithMessage("Employee doest not exist.");

            RuleFor(request => request.FirstName)
                .NotEmpty()
                .MaximumLength(60)
                .WithMessage("Employee firstname is required.");

            RuleFor(request => request.LastName)
                .NotEmpty()
                .MaximumLength(60)
                .WithMessage("Employee lastname is required.");

            RuleFor(request => request.Email)
                .NotEmpty()
                .MaximumLength(100)
                .EmailAddress()
                .WithMessage("Employee email is required.");

            RuleFor(request => request.Salary)
                .NotEmpty()
                .WithMessage("Employee must have salary.");
        }
    }
}