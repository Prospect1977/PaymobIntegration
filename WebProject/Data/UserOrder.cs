using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebProject.Data
{
    public class UserOrder
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public IdentityUser? user { get; set; }

        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product? Product { get; set; }
        public double UnitPrice { get; set; }
        public int Quantity { get; set; }
        public double Payment { get; set; }
        public double? NetPayment { get; set; }
       
        public int? OrderId { get; set; }
        public int? TransactionId { get; set; }

        public bool PaymentCompleted { get; set; } = false;
        public DateTime DataDate { get; set; }
        [Comment("iframe/kiosk")]
        public String Source { get; set; } = "iframe";
    }
}
