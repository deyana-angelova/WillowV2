using Microsoft.AspNetCore.Mvc;
using WillowV2.Data;

namespace WillowV2.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        public AccountController(ApplicationDbContext dbContext)
        {
         this.dbContext = dbContext;   
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
