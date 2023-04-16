using Microsoft.AspNetCore.Mvc;
using LoginAPI.Domain;
using Microsoft.AspNetCore.Authorization;
using LoginAPI.Models;

namespace LoginAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService service;

    public UserController(IUserService service)
    {
        this.service = service;
    }

    [HttpPost]
    [Route("Login")]
    public IActionResult Login([FromBody] LoginPostModel loginInfo)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress;
        Console.WriteLine($"Address:{ipAddress}");
        if (string.IsNullOrEmpty(loginInfo.Account) || string.IsNullOrEmpty(loginInfo.Password))
        {
            return BadRequest("Param Error");
        }

        User foundUser = service.GetUser(loginInfo.Account, loginInfo.Password);
        if (foundUser == null)
        {
            return Unauthorized("用戶不存在");
        }
        else
        {
            var jwtToken = service.GenAndSaveToken(foundUser.Id);
            if (string.IsNullOrEmpty(jwtToken))
            {
                return StatusCode(500, "系統錯誤，AccessToken保存失敗");
            }
            else
            {
                return Ok(new { Token = jwtToken });
            }
        }
    }

    [Authorize]
    [HttpGet("Name")]
    public IActionResult GetName()
    {
        var claim = User.Claims.FirstOrDefault(c => c.Type == "userId");
        if (claim != null)
        {
            if (int.TryParse(claim.Value, out int uid))
            {
                var user = service.GetUserById(uid);
                if (user != null)
                {
                    return Ok(new { DisplayName = user.UserName });
                }
            }
        }
        return Unauthorized();
    }

    [Authorize]
    [HttpPost("Logout")]
    public IActionResult Logout()
    {
        var claim = User.Claims.FirstOrDefault(c => c.Type == "userId");
        if (claim != null)
        {
            var uidStr = claim.Value;
            if (int.TryParse(uidStr, out int uid) && service.DeleteAccessToken(uid))
            {
                return Ok();
            }
        }
        return StatusCode(500, "系統錯誤，AccessToken刪除失敗");
    }
}