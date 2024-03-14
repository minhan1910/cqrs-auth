using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    public class MyBaseController<T> : ControllerBase
    {
        private ISender _sender;
        private readonly IHttpContextAccessor _contextAccessor;

        protected ISender MediatorSender => _sender ??= _contextAccessor.HttpContext.RequestServices.GetService<ISender>();

        public MyBaseController(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }
    }
}
