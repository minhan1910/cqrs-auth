﻿using Application.Features.Identity.Token.Queries;
using Common.Requests.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.Identity
{
    [Route("api/[controller]")]
    public class TokenController : MyBaseController<TokenController>
    {
        public TokenController(IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
        }

        [HttpPost("get-token")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTokenAsync([FromBody] TokenRequest tokenRequest)
        {
            var response = await MediatorSender.Send(new GetTokenQuery { TokenRequest = tokenRequest });

            if (response.IsSuccessfull)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> GetRefreshTokenAsync([FromBody] RefreshTokenRequest refreshTokenRequest)
        {
            var response = await MediatorSender.Send(new GetRefreshTokenQuery { RefreshTokenRequest = refreshTokenRequest });

            if (response.IsSuccessfull)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }
    }
}
