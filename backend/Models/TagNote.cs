using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Models
{
    public class TagNote
    {
        public Guid Id { get; set; } = Guid.Empty;
        public Guid NoteId { get; set; }
        public string Tag { get; set; } = String.Empty;
        public DateTime CreateDate { get; set; }
        [JsonIgnore]
        public virtual Note? Note { get; set; }
    }
}