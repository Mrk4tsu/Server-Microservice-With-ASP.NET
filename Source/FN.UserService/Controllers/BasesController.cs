﻿using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FN.UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasesController : ControllerBase
    {
        protected int? GetUserIdFromClaims()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }

            return null;
        }
    }
}
