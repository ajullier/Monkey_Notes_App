using Microsoft.AspNetCore.Mvc;
using NoteWebMVC.Models;
using NoteWebMVC.Utilities;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Components.Routing;

namespace NoteWebMVC.Controllers
{
    public class AccessController : Controller
    {

        private ReadUrlConfiguration PersonalConfiguration { get; set; }
        public AccessController()
        {
            PersonalConfiguration = new ReadUrlConfiguration();
        }

        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Login([Bind("Id,Name,Password")] User user)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string ApiUrl = $"{PersonalConfiguration.Url}/NotesApi/GetByNamePassword?password={user.Password}&name={user.Password}";
                    string response = await Utilities.ApiRest.GetStringObjects(ApiUrl);
                    GenericMessage<User> genericMessage = JsonConvert.DeserializeObject<GenericMessage<User>>(response)?? new GenericMessage<User>(new Models.User(), "No se encontró el usuario en la base de datos.");
                    
                    if (genericMessage.Object.Id.Equals(Guid.Empty))
                    {
                        ViewBag.Error = "Usuario o contraseña incorrectos";
                        return View();
                    }
                    HttpContext.Session.SetString("Name", genericMessage.Object.Name);
                    HttpContext.Session.SetString("Id", genericMessage.Object.Id.ToString());

                    return RedirectToAction("Index", "Notes");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

    }
}
