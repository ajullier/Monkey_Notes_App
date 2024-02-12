namespace NoteWebMVC.Models
{
    public class RequestUpdateNote
    {
        public string Name { get; set; } = String.Empty;
        public string Content { get; set; } = String.Empty;
        public List<string> Tags { get; set; } = new List<string>();


    }
}
