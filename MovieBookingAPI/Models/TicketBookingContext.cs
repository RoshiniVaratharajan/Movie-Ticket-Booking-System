using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieBookingAPI.Models
{
    public class TicketBookingContext : DbContext
    {
        public TicketBookingContext(DbContextOptions<TicketBookingContext> options) : base(options) { }

        public DbSet<ShowSeat> ShowSeats { get; set; }
        public DbSet<TicketDetail> TicketDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Enforce unique constraint for ShowId + SeatId
            modelBuilder.Entity<ShowSeat>()
                .HasAlternateKey(s => new { s.ShowId, s.SeatId });
        }
    }

    [Table("ShowSeats")]
    public class ShowSeat
    {
        [Key] public int Id { get; set; }
        public int ShowId { get; set; }
        public int SeatId { get; set; }
        public string SeatNumber { get; set; } = null!;
        public string Status { get; set; } = "Available";
        public DateTime? LockedUntil { get; set; }
        public int? LockedByUserId { get; set; }
        public decimal Price { get; set; }
    }

    [Table("TicketDetails")]
    public class TicketDetail
    {
        [Key] public int Id { get; set; }
        public string TicketNumber { get; set; } = null!;
        public int ShowId { get; set; }
        public int SeatId { get; set; }
        public int UserId { get; set; }
        public decimal AmountPaid { get; set; }
        public string PaymentStatus { get; set; } = "Pending";
        public DateTime BookedAt { get; set; } = DateTime.UtcNow;
    }
}