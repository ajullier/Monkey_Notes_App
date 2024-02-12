using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NoteWebMVC.Models;
using NoteWebMVC.Utilities;

namespace NoteWebMVC.Controllers
{
    public class UserController : Controller
    {
        private ReadUrlConfiguration PersonalConfiguration { get; set; }
        public UserController()
        {
            PersonalConfiguration = new ReadUrlConfiguration();
        }

        // GET: UserController
        public ActionResult Index()
        {
            return View();
        }

        // GET: UserController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: UserController/Create
        public ActionResult Create()
        {
            User user = new();
            return View(user);
        }

        // POST: UserController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([FromForm] User user)
        {
            user.Id = Guid.Empty;
            string ApiUrl = $"{PersonalConfiguration.Url}/NotesApi/api/User";
            string response = await Utilities.ApiRest.PostStringObjects(ApiUrl, user);
            GenericMessage<User> noteMessage = JsonConvert.DeserializeObject<GenericMessage<User>>(response) ?? new GenericMessage<User>(new User(), "The user was not created.");

            ViewBag.Error = $"{noteMessage.Message}";

            return View();
        }

        // GET: UserController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: UserController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UserController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UserController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
