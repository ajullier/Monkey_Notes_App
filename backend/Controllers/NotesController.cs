using Microsoft.AspNetCore.Mvc;
using Models;
using NotesApi.Application;
using NotesApi.Controllers.Dto;
using NotesApi.Controllers.Requests;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NotesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private Application.Notes ApplicationNotes;

        public NotesController(ApiDbContext context)
        {
            ApplicationNotes = new Application.Notes(context);
        }


        [HttpPut]
        [Route("/ToArchive/{id}")]
        public GenericMessage<Note> ToArchive(Guid id)
        {
            return ApplicationNotes.ToArchive(id);
        }

        [HttpPut]
        [Route("/ToActive/{id}")]
        public GenericMessage<Note> ToActive(Guid id)
        {
            return ApplicationNotes.ToActive(id);
        }

        [HttpPost]
        [Route("/GetByTag")]
        public List<DtoGetNotes> GetByUserId(Guid UserId, bool IsDeleted, bool Active, [FromBody] List<string> Tags)
        {
            return ApplicationNotes.GetByUserIdTags(UserId, IsDeleted, Active, Tags);
        }

        // GET: api/<NotesController>
        [HttpGet]
        [Route("/GetByUserId")]
        public List<DtoGetNotes> GetByUserId(Guid UserId, bool IsDeleted, bool Active)
        {
            return ApplicationNotes.GetByUserId(UserId, IsDeleted, Active);
        }

        // GET api/<NotesController>/5
        [HttpGet("{id}")]
        public GenericMessage<DtoGetNotes> Get(Guid id)
        {
            return ApplicationNotes.GetById(id);
        }

        // POST api/<NotesController>
        [HttpPost]
        public GenericMessage<Note> Post([FromBody] RequestCreateNote request)
        {
            Note note = new Note();
            note.Name = request.Name;
            note.Content = request.Content;
            note.UserId = request.UserId;
            return ApplicationNotes.Create(note, request.Tags);
        }

        // PUT api/<NotesController>/5
        [HttpPut("{id}")]
        public GenericMessage<Note> Put(Guid id, [FromBody] RequestUpdateNote request)
        {
            Note note = new Note();
            note.Name = request.Name;
            note.Content = request.Content;
            return ApplicationNotes.Update(id, note, request.Tags);
        }

        // DELETE api/<NotesController>/5
        [HttpDelete("{id}")]
        public GenericMessage<Note> Delete(Guid id)
        {
            return ApplicationNotes.Delete(id);
        }
    }
}
