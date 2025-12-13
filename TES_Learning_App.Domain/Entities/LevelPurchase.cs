using System;
using System.ComponentModel.DataAnnotations;

namespace TES_Learning_App.Domain.Entities
{
    public class LevelPurchase
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        [Required]
        public int LevelId { get; set; }
        public Level Level { get; set; } = null!;

        [Required]
        [StringLength(255)]
        public string StripeSessionId { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string PaymentStatus { get; set; } = "pending"; // pending, completed, failed

        [Required]
        public decimal Amount { get; set; }

        [StringLength(10)]
        public string Currency { get; set; } = "INR";

        public DateTime PurchasedAt { get; set; } = DateTime.UtcNow;

        public DateTime? CompletedAt { get; set; }
    }
}

