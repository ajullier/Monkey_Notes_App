using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Models
{
    public class User: BaseModel
    {
        public string Password { get; set; } = string.Empty;
        [JsonIgnore]
        public virtual ICollection<Note>? Notes { get; set; } = new List<Note>();
    }
}