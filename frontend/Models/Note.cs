using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace NoteWebMVC.Models
{
    public class Note: BaseModel
    {
        public string Content { get; set; } = string.Empty;
        public bool Active {get; set;} = true;
        public Guid UserId {get; set;}
        [JsonIgnore]
        public virtual User User { get; set; } = new User();
        [JsonIgnore]
        public virtual ICollection<TagNote>? Tags { get; set; } = new List<TagNote>();
    }
}