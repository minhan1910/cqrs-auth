using Application.Services.Employees;
using AutoMapper;
using Common.Responses.Employees;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Employees.Queries
{
    public sealed record GetEmployeeByIdQuery(string employeeId)
        : IRequest<IResponseWrapper>
    { }

    public class GetEmployeeByIdHandler : IRequestHandler<GetEmployeeByIdQuery, IResponseWrapper>
    {
        private readonly IEmployeeService _employeeService;
        private readonly IMapper _mapper;

        public GetEmployeeByIdHandler(IEmployeeService employeeService, IMapper mapper)
        {
            _employeeService = employeeService;
            _mapper = mapper;
        }

        public async Task<IResponseWrapper> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
        {
            var id = int.Parse(request.employeeId);
            var employeeFromDb = await _employeeService.GetEmployeeByIdAsync(id);

            if (employeeFromDb is null)
            {
                return await ResponseWrapper.FailAsync("Can not found employee");
            }

            var employeeResponse = _mapper.Map<EmployeeResponse>(employeeFromDb);

            return await ResponseWrapper<EmployeeResponse>.SuccessAsync(employeeResponse, "Get Employee By Id Successfully!");
        }
    }
}