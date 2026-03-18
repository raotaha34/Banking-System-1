using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystemweb.Controllers
{
    [Authorize(Roles = "User,Customer")]
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
