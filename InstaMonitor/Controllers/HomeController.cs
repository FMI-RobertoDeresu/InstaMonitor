using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using InstaMonitor.Models;
using InstaSharper.API;
using InstaSharper.API.Builder;
using InstaSharper.Classes;
using InstaSharper.Logger;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace InstaMonitor.Controllers
{
    public class HomeController : Controller
    {
        private static IInstaApi _instaApi;

        public HomeController(IConfiguration config)
        {
            if (_instaApi == null)
            {
                var userSession = new UserSessionData
                {
                    UserName = config["insta:username"],
                    Password = config["insta:password"]
                };

                var delay = RequestDelay.FromSeconds(0, 0);

                _instaApi = InstaApiBuilder.CreateBuilder()
                    .SetUser(userSession)
                    .UseLogger(new DebugLogger(LogLevel.Exceptions))
                    .SetRequestDelay(delay)
                    .Build();

                _instaApi.LoginAsync().GetAwaiter().GetResult();
            }
        }

        public async Task<IActionResult> Index(string username)
        {
            var model = !string.IsNullOrEmpty(username) ? await GetData(username) : new IndexModel();
            return View(model);
        }

        private async Task<IndexModel> GetData(string username)
        {
            var model = new IndexModel();
            model.Username = username;

            var followers = await _instaApi.GetUserFollowersAsync(username, PaginationParameters.Empty);
            var following = await _instaApi.GetUserFollowingAsync(username, PaginationParameters.Empty);

            if (!followers.Succeeded || !following.Succeeded)
            {
                model.Message = "User not found.";
                return model;
            }

            model.Followers = followers.Value.ToList();
            model.Following = following.Value.ToList();

            model.FollowersNotFollowed = model.Followers.Where(x => model.Following.All(y => y.UserName != x.UserName)).ToList();
            model.FollowingNotFollowers = model.Following.Where(x => model.Followers.All(y => y.UserName != x.UserName)).ToList();

            model.SearchedWithSuccess = true;

            return model;
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}