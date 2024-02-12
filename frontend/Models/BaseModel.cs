namespace NoteWebMVC.Models
{
    public class BaseModel
    {
        public Guid Id { get; set; } = Guid.Empty;
        public string Name {get; set;} = string.Empty;
        public DateTime CreateDate {get; set;}
        public DateTime EditDate {get; set;}
        public bool IsDeleted { get; set; } = false;

    }
}