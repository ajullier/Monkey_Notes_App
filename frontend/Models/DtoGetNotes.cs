namespace NoteWebMVC.Models
{
    public class DtoGetNotes
    {
        public Note Note { get; set; } = new Note();
        public List<string> Tags { get; set; } = new List<string>();
    }
}
