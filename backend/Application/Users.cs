using Models;

namespace NotesApi.Application
{
    public class Users
    {
        private readonly ApiDbContext _context;

        public Users(ApiDbContext context)
        {
            _context = context;
        }
        public List<User> GetAll()
        {
           return _context.Users?.ToList() ?? new List<User>();
        }

        public GenericMessage<User> GetById(Guid id)
        {
            string message = string.Empty;
            User user = _context.Users?.FirstOrDefault(a => a.Id.Equals(id)) ?? new User();
            if (user.Id.Equals(Guid.Empty))
            {
                message = "User does not exist.";
            }
            else
            {
                message = "User find correctly.";
            }
            return new GenericMessage<User>(user, message);
        }

        public GenericMessage<User> GetByNamePassword(string name, string password)
        {
            string message = string.Empty;
            User user = _context.Users?.FirstOrDefault(u => u.Name.Equals(name) && u.Password.Equals(password)) ?? new User();
            if (user.Id.Equals(Guid.Empty))
            {
                message = "User does not exist.";
            }
            else
            {
                message = "User find correctly.";
            }
            return new GenericMessage<User>(user, message);
        }

        public GenericMessage<User> Create (User user)
        {
            if (user == null)
            {
                return new GenericMessage<User>(new User(), "You have sent an empty user");
            }
            else
            {
                if (user.Id.Equals(Guid.Empty))
                {
                    user.Id = Guid.NewGuid();
                }
                else
                {
                    return new GenericMessage<User>(user, "Guid must be in empty format (00000000-0000-0000-0000-000000000000).");
                }
                User existUser = _context.Users?.FirstOrDefault(u => u.Name.Equals(user.Name)) ?? new User();

                if (existUser.Id.Equals(Guid.Empty))
                {
                    _context.Users?.Add(user);
                    _context.SaveChanges();
                    User createdUser = _context.Users?.FirstOrDefault(a => a.Id.Equals(user.Id)) ?? throw new Exception("User not created.");
                    return new GenericMessage<User>(createdUser, "User created correctly.");
                }
                else
                {
                    return new GenericMessage<User>(new User(), "User already exist");
                }
            }
        } 

        public GenericMessage<User> Delete(Guid Id)
        {
            User user = _context.Users?.FirstOrDefault(a => a.Id.Equals(Id)) ?? throw new Exception("User not found.");
            user.IsDeleted = true;
            _context.Users.Update(user);
            _context.SaveChanges();
            User updatedUser = _context.Users?.FirstOrDefault(a => a.Id.Equals(user.Id)) ?? throw new Exception("User not found.");
            string message = string.Empty;
            if (updatedUser.IsDeleted)
            {
                message = "User deleted correctly.";
            }
            else
            {
                message = "There was an error and the user is still active.";
            }
            return new GenericMessage<User>(updatedUser, "User created correctly.");
        }

    }
}
