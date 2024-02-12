﻿namespace NoteWebMVC.Models
{
    public class RequestCreateNote
    {
        public string Name { get; set; } = String.Empty;
        public string Content { get; set; } = String.Empty;
        public Guid UserId { get; set; }
        public List<string> Tags { get; set; } = new List<string>();


    }
}
