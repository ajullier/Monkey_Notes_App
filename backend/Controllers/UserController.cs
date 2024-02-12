using Models;
using NotesApi.Application;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NotesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private Application.Users ApplicationUser;

        public UserController(ApiDbContext context)
        {
            ApplicationUser = new Application.Users(context);
        }

        // GET: api/<UserController>
        [HttpGet]
        public IEnumerable<User> GetAll()
        {
            return ApplicationUser.GetAll();
        }


        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public GenericMessage<User> Get(Guid id)
        {
            return ApplicationUser.GetById(id);
        }

        [HttpGet]
        [Route("/GetByNamePassword")]
        public GenericMessage<User> GetByNamePassword( string password, string name)
        {
            return ApplicationUser.GetByNamePassword(name, password);
        }

        // POST api/<UserController>
        [HttpPost]
        public GenericMessage<User> Create([FromBody] User user)
        {
            return ApplicationUser.Create(user);
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public GenericMessage<User> Delete(Guid id)
        {
            return ApplicationUser.Delete(id);
        }
    }
}
