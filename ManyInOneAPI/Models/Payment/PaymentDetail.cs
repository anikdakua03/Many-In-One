using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ManyInOneAPI.Models.Payment
{
    public class PaymentDetail
    {
        [Key]
        public int PaymentDetailId { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string CardOwnerName { get; set; } = "";

        [Column(TypeName = "nvarchar(16)")]
        public string CardNumber { get; set; } = "";

        [Column(TypeName = "nvarchar(3)")]
        [Category("Security")]
        [Description("CVV")]
        [PasswordPropertyText(true)]
        public string SecurityCode { get; set; } = "";

        // MM/YY
        [Column(TypeName = "nvarchar(50)")]
        public string ExpirationDate { get; set; } = "";
    }
}
