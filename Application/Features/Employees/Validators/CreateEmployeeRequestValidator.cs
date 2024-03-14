using Common.Requests.Employees;
using FluentValidation;

namespace Application.Features.Employees.Validators
{
    public class CreateEmployeeRequestValidator : AbstractValidator<CreateEmployeeRequest>
    {
        public CreateEmployeeRequestValidator()
        {
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