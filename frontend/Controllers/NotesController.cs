using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NoteWebMVC.Models;
using NoteWebMVC.Utilities;

namespace NoteWebMVC.Controllers
{
    public class NotesController : Controller
    {
        private ReadUrlConfiguration PersonalConfiguration { get; set; }
        public NotesController()
        {
            PersonalConfiguration = new ReadUrlConfiguration();
        }

        public async Task<ActionResult> Archive(string tag)
        {
            List<string> tags = new();
            if (tag is not null)
            {
                tags.Add(tag.ToString());
            }


            if (HttpContext.Session.GetString("Id") == null)
            {
                return RedirectToAction("Login", "Access");
            }

            List<DtoGetNotes> list = new List<DtoGetNotes>();

            if (tags.Count > 0)
            {
                string ApiUrl = $"{PersonalConfiguration.Url}/NotesApi/GetByTag?UserId={HttpContext.Session.GetString("Id")}&IsDeleted=false&Active=false";
                string response = await Utilities.ApiRest.PostStringObjects(ApiUrl, tags);
                list = JsonConvert.DeserializeObject<List<DtoGetNotes>>(response) ?? new List<DtoGetNotes>();
            }

            else
            {
                string ApiUrl = $"{PersonalConfiguration.Url}/NotesApi/GetByUserId?UserId={HttpContext.Session.GetString("Id")}&IsDeleted=false&Active=false";
                string response = await Utilities.ApiRest.GetStringObjects(ApiUrl);
                list = JsonConvert.DeserializeObject<List<DtoGetNotes>>(response) ?? new List<DtoGetNotes>();

            }


            if (list.Count == 0)
            {
                ViewBag.Error = $"There are no notes in the archive.";
            }

            return View("Archive", list);
        }

        // GET: NotesController
        public async Task<ActionResult> Index(string tag)
        {
            List<string> tags = new();
            if(tag is not null)
            {
                tags.Add(tag.ToString());
            }

            if (HttpContext.Session.GetString("Id") == null)
            {
                return RedirectToAction("Login", "Access");
            }

            List<DtoGetNotes> list = new List<DtoGetNotes>();

            if (tags.Count > 0)
            {
                string ApiUrl = $"{PersonalConfiguration.Url}/NotesApi/GetByTag?UserId={HttpContext.Session.GetString("Id")}&IsDeleted=false&Active=true";
                string response = await Utilities.ApiRest.PostStringObjects(ApiUrl, tags);
                list = JsonConvert.DeserializeObject<List<DtoGetNotes>>(response) ?? new List<DtoGetNotes>();
            }

            else
            {
                string ApiUrl = $"{PersonalConfiguration.Url}/NotesApi/GetByUserId?UserId={HttpContext.Session.GetString("Id")}&IsDeleted=false&Active=true";
                string response = await Utilities.ApiRest.GetStringObjects(ApiUrl);
                list = JsonConvert.DeserializeObject<List<DtoGetNotes>>(response) ?? new List<DtoGetNotes>();

            }

            if (list.Count == 0)
            {
                ViewBag.Error = $"There are no active notes.";
            }

            return View("ActiveNotes", list);
        }


        // GET: NotesController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: NotesController/Create
        public ActionResult Create()
        {
            string UserId = HttpContext.Session.GetString("Id")??string.Empty;
            if (UserId == string.Empty)
            {
                return RedirectToAction("Login", "Access");
            }
            RequestCreateNote createNote = new();
            createNote.UserId = Guid.Parse(UserId);
            return View(createNote);
        }

        // POST: NotesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([FromForm] RequestCreateNote createNote)
        {
            string UserId = HttpContext.Session.GetString("Id") ?? string.Empty;
            createNote.UserId = Guid.Parse(UserId);

            string ApiUrl = $"{PersonalConfiguration.Url}/NotesApi/api/Notes";
            string response = await Utilities.ApiRest.PostStringObjects(ApiUrl, createNote);
            GenericMessage<Note> noteMessage = JsonConvert.DeserializeObject<GenericMessage<Note>>(response) ?? new GenericMessage<Note>(new Note(), "The note was not created.");

            ViewBag.Error = $"{noteMessage.Message}";
            if (noteMessage.Object.Id.Equals(Guid.Empty))
            {
                return View(createNote);
            }
            
            try
            {
                return RedirectToAction(nameof(Index), ViewBag);
            }
            catch
            {
                return View();
            }
        }

        // GET: NotesController/Edit/5
        public async Task<ActionResult> Edit(Guid id)
        {
            string UserId = HttpContext.Session.GetString("Id") ?? string.Empty;
            if (UserId == string.Empty)
            {
                return RedirectToAction("Login", "Access");
            }
            RequestUpdateNote updateNote = new();

            string ApiUrl = $"{PersonalConfiguration.Url}/NotesApi/api/Notes/{id}";
            string response = await Utilities.ApiRest.GetStringObjects(ApiUrl);
            GenericMessage<DtoGetNotes> noteMessage = JsonConvert.DeserializeObject<GenericMessage<DtoGetNotes>>(response) ?? new GenericMessage<DtoGetNotes>(new DtoGetNotes(), "Note does not exist.");



            if (noteMessage.Object.Note.Id.Equals(Guid.Empty))
            {
                ViewBag.Error = $"{noteMessage.Message}";
                return View();
            }

            RequestUpdateNote requestUpdateNote = new();
            requestUpdateNote.Name = noteMessage.Object.Note.Name;
            requestUpdateNote.Content = noteMessage.Object.Note.Content;
            requestUpdateNote.Tags = noteMessage.Object.Tags;

            ViewBag.Id = id;
            
            return View(requestUpdateNote);
        }

        [HttpPost]
        public ActionResult EditTag_Edit(Guid Id, RequestUpdateNote requestUpdateNote, string newTag, string removeTag)
        {
            ViewBag.Id = Id;
            string UserId = HttpContext.Session.GetString("Id") ?? string.Empty;
            if (UserId == string.Empty)
            {
                return RedirectToAction("Login", "Access");
            }

            if (!string.IsNullOrWhiteSpace(newTag))
            {
                if (requestUpdateNote.Tags == null)
                {
                    requestUpdateNote.Tags = new List<string>();
                }
                requestUpdateNote.Tags.Add(newTag);
            }
            else if (!string.IsNullOrWhiteSpace(removeTag))
            {
                requestUpdateNote.Tags.Remove(removeTag);
            }

            return View("Edit", requestUpdateNote);
        }


        [HttpPost]
        public ActionResult EditTag_Create(RequestCreateNote requestCreateNote, string newTag, string removeTag)
        {
            string UserId = HttpContext.Session.GetString("Id") ?? string.Empty;
            if (UserId == string.Empty)
            {
                return RedirectToAction("Login", "Access");
            }

            if (!string.IsNullOrWhiteSpace(newTag))
            {
                if (requestCreateNote.Tags == null)
                {
                    requestCreateNote.Tags = new List<string>();
                }
                requestCreateNote.Tags.Add(newTag);
            }
            else if (!string.IsNullOrWhiteSpace(removeTag))
            {
                requestCreateNote.Tags.Remove(removeTag);
            }

            return View("Create", requestCreateNote);
        }

        // POST: NotesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Guid id, RequestUpdateNote requestUpdateNote)
        {
            string ApiUrl = $"{PersonalConfiguration.Url}/NotesApi/api/Notes/{id}";
            string response = await Utilities.ApiRest.PutStringObjects(ApiUrl, requestUpdateNote);
            GenericMessage<Note> noteMessage = JsonConvert.DeserializeObject<GenericMessage<Note>>(response) ?? new GenericMessage<Note>(new Note(), "The note was not updated.");

            ViewBag.Error = $"{noteMessage.Message}";
            if (noteMessage.Object.Id.Equals(Guid.Empty))
            {
                return View(requestUpdateNote);
            }

            try
            {
                return RedirectToAction(nameof(Index), ViewBag);
            }
            catch
            {
                return View();
            }
        }

        // GET: NotesController/Delete/5
        public async Task<ActionResult> Delete(Guid id)
        {
            if (HttpContext.Session.GetString("Id") == null)
            {
                return RedirectToAction("Login", "Access");
            }

            string ApiUrl = $"{PersonalConfiguration.Url}/NotesApi/api/Notes/{id}";
            string response = await Utilities.ApiRest.GetStringObjects(ApiUrl);
            GenericMessage<DtoGetNotes> noteMessage = JsonConvert.DeserializeObject<GenericMessage<DtoGetNotes>>(response) ?? new GenericMessage<DtoGetNotes>(new DtoGetNotes(), "Note does not exist.");

            if (noteMessage.Object.Note.Id.Equals(Guid.Empty))
            {
                ViewBag.Error = $"{noteMessage.Message}";
                return View(noteMessage.Object.Note);
            }

            return View(noteMessage.Object.Note);
        }

        // POST: NotesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(Guid id, Note note)
        {
            string ApiUrl = $"{PersonalConfiguration.Url}/NotesApi/api/Notes/{id}";
            string response = await Utilities.ApiRest.DeleteStringObjects(ApiUrl);
            GenericMessage<Note> noteMessage = JsonConvert.DeserializeObject<GenericMessage<Note>>(response) ?? new GenericMessage<Note>(new Note(), "Note does not exist.");

            if (noteMessage.Object.Id.Equals(Guid.Empty))
            {
                ViewBag.Error = $"{noteMessage.Message}";
                return View(noteMessage.Object);
            }

            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }


        ///TO ACTIVE///
        ///
        public async Task<ActionResult> ToActive(Guid id)
        {
            if (HttpContext.Session.GetString("Id") == null)
            {
                return RedirectToAction("Login", "Access");
            }

            string ApiUrl = $"{PersonalConfiguration.Url}/NotesApi/api/Notes/{id}";
            string response = await Utilities.ApiRest.GetStringObjects(ApiUrl);
            GenericMessage<DtoGetNotes> noteMessage = JsonConvert.DeserializeObject<GenericMessage<DtoGetNotes>>(response) ?? new GenericMessage<DtoGetNotes>(new DtoGetNotes(), "Note does not exist.");

            if (noteMessage.Object.Note.Id.Equals(Guid.Empty))
            {
                ViewBag.Error = $"{noteMessage.Message}";
                return View(noteMessage.Object.Note);
            }

            return View(noteMessage.Object.Note);
        }

        // POST: NotesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ToActive(Guid id, Note note)
        {
            string ApiUrl = $"{PersonalConfiguration.Url}/NotesApi/ToActive/{id}";
            string response = await Utilities.ApiRest.PutStringObjects(ApiUrl, note);
            GenericMessage<Note> noteMessage = JsonConvert.DeserializeObject<GenericMessage<Note>>(response) ?? new GenericMessage<Note>(new Note(), "Note does not exist.");

            if (noteMessage.Object.Id.Equals(Guid.Empty))
            {
                ViewBag.Error = $"{noteMessage.Message}";
                return View(noteMessage.Object);
            }

            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        ///TO ARCHIVE///
        ///
        public async Task<ActionResult> ToArchive(Guid id)
        {
            if (HttpContext.Session.GetString("Id") == null)
            {
                return RedirectToAction("Login", "Access");
            }

            string ApiUrl = $"{PersonalConfiguration.Url}/NotesApi/api/Notes/{id}";
            string response = await Utilities.ApiRest.GetStringObjects(ApiUrl);
            GenericMessage<DtoGetNotes> noteMessage = JsonConvert.DeserializeObject<GenericMessage<DtoGetNotes>>(response) ?? new GenericMessage<DtoGetNotes>(new DtoGetNotes(), "Note does not exist.");

            if (noteMessage.Object.Note.Id.Equals(Guid.Empty))
            {
                ViewBag.Error = $"{noteMessage.Message}";
                return View(noteMessage.Object.Note);
            }

            return View(noteMessage.Object.Note);
        }

        // POST: NotesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ToArchive(Guid id, Note note)
        {
            string ApiUrl = $"{PersonalConfiguration.Url}/NotesApi/ToArchive/{id}";
            string response = await Utilities.ApiRest.PutStringObjects(ApiUrl, note);
            GenericMessage<Note> noteMessage = JsonConvert.DeserializeObject<GenericMessage<Note>>(response) ?? new GenericMessage<Note>(new Note(), "Note does not exist.");

            if (noteMessage.Object.Id.Equals(Guid.Empty))
            {
                ViewBag.Error = $"{noteMessage.Message}";
                return View(noteMessage.Object);
            }

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
