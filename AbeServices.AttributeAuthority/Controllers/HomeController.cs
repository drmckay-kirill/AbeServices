using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AbeServices.AttributeAuthority.Models;
using AbeServices.AttributeAuthority.Models.ViewModels;
using AbeServices.AttributeAuthority.Services;

namespace AbeServices.AttributeAuthority.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILoginService _loginService;

        public HomeController(ILoginService loginService)
        {
            _loginService = loginService;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new LoginViewModel() 
            {
                Logins = await _loginService.GetList()
            };

            return View(viewModel);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateLogin([FromForm] CreateLoginViewModel viewModel)
        {
            var login = new Login()
            {
                Name = viewModel.Login,
                SharedKey = viewModel.SharedKey,
                Attributes = viewModel.Attributes ?? new string[] { "default" }
            };
            
            await _loginService.Create(login);
            return RedirectToAction("Index", "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
