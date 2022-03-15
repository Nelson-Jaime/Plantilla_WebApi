using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Plantilla.WebApi.Busines.Contracts.Domain;
using Plantilla.WebApi.Busines.Contracts.Service;
using Plantilla.WebApi.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plantilla.WebApi.Controllers.v1
{
    [ApiController]
    [ApiVersion("1")]
    [Route("v{version:apiVersion}/[controller]")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [ProducesResponseType(typeof(IEnumerable<UserDto>), 200)]
        [ProducesResponseType(typeof(void), 204)]
        [ProducesResponseType(typeof(void), 500)]
        [ProducesResponseType(typeof(IEnumerable<ErrorObject>), 400)]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var operationResult = await _userService.GetAllUsers();

            if (operationResult.HasSomeException())
            {
                return StatusCode(500);
            }
            if (operationResult.HasErrors())
            {
                return BadRequest(operationResult.Errors);
            }
            if (!operationResult.Result.Any())
            {
                return NoContent();
            }

            return Ok(operationResult.Result);
        }

        [ProducesResponseType(typeof(void), 204)]
        [ProducesResponseType(typeof(void), 500)]
        [ProducesResponseType(typeof(IEnumerable<ErrorObject>), 400)]
        [HttpGet("id")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var operationResult = await _userService.GetUser(id);

            if (operationResult.HasSomeException())
            {
                return StatusCode(500);
            }
            if (operationResult.HasErrors())
            {
                return BadRequest(operationResult.Errors);
            }

            return Ok(operationResult.Result);
        }

        //[ProducesResponseType(200, Type = typeof(UserDto))]
        //[ProducesResponseType(400, Type = typeof(IEnumerable<ErrorObject>))]
        //[HttpPost]
        //public async Task<IActionResult> PostUser([FromBody] UserDto user)
        //{
        //    var operationResult = await _userService.AddUserAsync(user);
        //    if (operationResult.HasErrors())
        //    {
        //        return BadRequest(operationResult.Errors);
        //    }
        //    else
        //    {
        //        return Ok();
        //    }
        //}

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
        [HttpPost("/CreateUser")]
        public async Task<IActionResult> PostUserAndPassword(string username, string password, string roleName)
        {
            var result = await _userService.SaveUserWithRolesAsync(username, password, roleName);
            if (result.HasErrors())
            {
                return StatusCode(404, false);
            }
            if (result.Errors.Where(e => e.Exception != null).Count() > 0)
            {
                return StatusCode(500, false);
            }
            return Ok();
        }


        [ProducesResponseType(400, Type = typeof(IEnumerable<ErrorObject>))]
        [HttpDelete("id")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var operationResult = await _userService.RemoveUser(id);
            if (operationResult.HasErrors())
            {
                return BadRequest(operationResult.Errors);
            }
            return Ok();
        }

    }
}
