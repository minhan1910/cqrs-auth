using Application.Features.Employees.Commands;
using Application.Features.Employees.Queries;
using Common.Authorisation;
using Common.Requests.Employees;
using Microsoft.AspNetCore.Mvc;
using WebApi.Attributes;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    public class EmployeeController : MyBaseController<EmployeeController>
    {
        public EmployeeController(IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
        }

        [HttpPost]
        [MustHavePermission(AppFeature.Employees, AppAction.Create)]
        public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeRequest createEmployeeRequest)
        {
            var response = await MediatorSender.Send(new CreateEmployeeCommand { CreateEmployeeRequest = createEmployeeRequest });

            return response.IsSuccessfull ? Ok(response) : BadRequest(response);
        }

        [HttpPut]
        [MustHavePermission(AppFeature.Employees, AppAction.Update)]
        public async Task<IActionResult> UpdateEmployee([FromBody] UpdateEmployeeRequest updateEmployeeRequest)
        {
            var response = await MediatorSender.Send(new UpdateEmployeeCommand { UpdateEmployeeRequest = updateEmployeeRequest });

            return response.IsSuccessfull ? Ok(response) : BadRequest(response);
        }

        [HttpDelete]
        [MustHavePermission(AppFeature.Employees, AppAction.Delete)]
        public async Task<IActionResult> DeleteEmployee([FromBody] int employeeId)
        {
            var response = await MediatorSender.Send(new DeleteEmployeeCommand(employeeId));

            return response.IsSuccessfull ? Ok(response) : BadRequest(response);
        }

        [HttpGet]
        [MustHavePermission(AppFeature.Employees, AppAction.Read)]
        public async Task<IActionResult> GetEmployeeList()
        {
            var response = await MediatorSender.Send(new GetEmployeesQuery());

            return response.IsSuccessfull ? Ok(response) : BadRequest(response);
        }

        [HttpGet("{employeeId}")]
        [MustHavePermission(AppFeature.Employees, AppAction.Read)]
        public async Task<IActionResult> GetEmployeeById(string employeeId)
        {
            var response = await MediatorSender.Send(new GetEmployeeByIdQuery(employeeId));

            return response.IsSuccessfull ? Ok(response) : BadRequest(response);
        }
    }
}