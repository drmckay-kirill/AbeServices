using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using AbeServices.AttributeAuthority.Models;
using AbeServices.AttributeAuthority.Models.ViewModels;
using MongoDB.Driver;

namespace AbeServices.AttributeAuthority.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMongoCollection<Login> _logins;

        public HomeController(IOptions<DatabaseSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _logins = database.GetCollection<Login>(settings.Value.LoginCollectionName);
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new LoginViewModel() 
            {
                Logins = await _logins
                    .Find(login => true)
                    .SortBy(login => login.Name)
                    .ToListAsync()
            };

            return View(viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
