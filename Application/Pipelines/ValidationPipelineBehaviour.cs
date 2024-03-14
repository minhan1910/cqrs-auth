using Application.Exceptions;
using Common.Responses.Wrappers;
using FluentValidation;
using MediatR;

namespace Application.Pipelines
{
    public class ValidationPipelineBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>, IValidateMe
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationPipelineBehaviour(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, 
                                            RequestHandlerDelegate<TResponse> next, 
                                            CancellationToken cancellationToken)
        {
            if (!_validators.Any())
            {
                return await next();
            }

            var context = new ValidationContext<TRequest>(request);                        

            var validationResults = 
                await Task.WhenAll(_validators
                    .Select(vr => vr.ValidateAsync(context, cancellationToken)));

            bool validationResultsAreValid = validationResults.Any(vr => vr.IsValid);

            if (validationResultsAreValid == false)
            {
                var failures =
                    validationResults
                    .SelectMany(vr => vr.Errors)
                    .Where(f => f != null);

                var errorMessages =
                    failures
                    .Select(failure => failure.ErrorMessage)
                    .ToList();

                return (TResponse) await ResponseWrapper.FailAsync(errorMessages);
                // Throw validation exception
                //throw new CustomValidationException(errorMessages, "One or more validation failure(s) occured.");
            }
            
            return await next(); 
        }
    }
}