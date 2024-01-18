using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ManyInOneAPI.Models.ImageFile
{
    public class Image
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Unicode(false)]
        [Column("Image Name")]
        [StringLength(50)]
        public string ImageName { get; set; } = DateTime.Now.ToString();

        [Column("Image")]
        public byte[]? ImageBytes { get; set; }
    }
}
