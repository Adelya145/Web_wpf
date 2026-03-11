using ApiParfum.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiParfum.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpPost("Authorization")]
        public ActionResult Post(string email, string password)
        {
            using (var context = new DbParfumeContext())
            {
                var client = context.Users.FirstOrDefault(c => c.UserLogin == email && c.UserPassword == password);


                if (client == null)
                {
                    var client1 = context.Users.FirstOrDefault(c => c.UserLogin == email);

                    if (client1 == null)
                    {
                        return BadRequest("Пользователь с таким логином не найден");
                    }
                    else
                    {
                        return BadRequest("Неверный пароль");
                    }
                }
                return Ok(client);
            }
        }
    }
}
