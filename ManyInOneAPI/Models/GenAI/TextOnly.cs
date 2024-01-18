using System.ComponentModel.DataAnnotations;

namespace ManyInOneAPI.Models.GenAI
{
    public class TextOnly
    {
        [Required]
        public string InputText { get; set; } = "";
    }
}
