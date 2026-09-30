using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieBookingAPI.Models;

namespace MovieBookingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly TicketBookingContext _context;
        public BookingController(TicketBookingContext context) => _context = context;

        // DTOs for requests
        public class LockRequest { public int ShowId { get; set; } public int SeatId { get; set; } public int UserId { get; set; } }
        public class PayRequest { public int ShowId { get; set; } public int SeatId { get; set; } public int UserId { get; set; } }

        // 1. Listing API for all shows (Extracts unique ShowIds from seats)
        [HttpGet("shows")]
        public async Task<IActionResult> GetShows()
        {
            var shows = await _context.ShowSeats.Select(s => s.ShowId).Distinct().ToListAsync();
            return Ok(shows.Select(id => new { ShowId = id, Title = $"Movie Show {id}" }));
        }

        // 2. Booking layout page API
        [HttpGet("shows/{showId}/layout")]
        public async Task<IActionResult> GetLayout(int showId)
        {
            var seats = await _context.ShowSeats
                .Where(s => s.ShowId == showId)
                .Select(s => new
                {
                    s.SeatId,
                    s.SeatNumber,
                    s.Price,
                    // If locked but time has passed, represent it as Available to the frontend
                    Status = (s.Status == "Reserved" && s.LockedUntil < DateTime.UtcNow) ? "Available" : s.Status
                })
                .ToListAsync();

            return Ok(seats);
        }

        // 3. Temporary Lock API (Concurrency Safe)
        [HttpPost("lock")]
        public async Task<IActionResult> LockSeat([FromBody] LockRequest req)
        {
            // Atomically update only if Available OR lock expired
            var rowsAffected = await _context.ShowSeats
                .Where(s => s.ShowId == req.ShowId && s.SeatId == req.SeatId &&
                            (s.Status == "Available" || (s.Status == "Reserved" && s.LockedUntil < DateTime.UtcNow)))
                .ExecuteUpdateAsync(s => s
                    .SetProperty(p => p.Status, "Reserved")
                    .SetProperty(p => p.LockedByUserId, req.UserId)
                    .SetProperty(p => p.LockedUntil, DateTime.UtcNow.AddSeconds(10)));

            if (rowsAffected == 0) return Conflict(new { message = "Seat is already locked or booked." });
            return Ok(new { message = "Seat locked successfully for 10 seconds." });
        }

        // 4. Listing the Bill API
        [HttpGet("bill/{showId}/{seatId}/{userId}")]
        public async Task<IActionResult> GetBill(int showId, int seatId, int userId)
        {
            var seat = await _context.ShowSeats
                .FirstOrDefaultAsync(s => s.ShowId == showId && s.SeatId == seatId && 
                                     s.LockedByUserId == userId && s.Status == "Reserved" && s.LockedUntil >= DateTime.UtcNow);

            if (seat == null) return NotFound(new { message = "Reservation expired or invalid." });

            return Ok(new
            {
                Title = "Invoice",
                ShowId = seat.ShowId,
                SeatNumber = seat.SeatNumber,
                BasePrice = seat.Price,
                Taxes = seat.Price * 0.1m, // Example 10% tax
                TotalAmount = seat.Price * 1.1m
            });
        }

        // 5. Successful Payment & Ticket Details API
        [HttpPost("pay")]
        public async Task<IActionResult> PayAndBook([FromBody] PayRequest req)
        {
            // Open a transaction because we update a seat AND insert a ticket
            using var transaction = await _context.Database.BeginTransactionAsync();

            var seat = await _context.ShowSeats
                .FirstOrDefaultAsync(s => s.ShowId == req.ShowId && s.SeatId == req.SeatId && 
                                     s.LockedByUserId == req.UserId && s.Status == "Reserved" && s.LockedUntil >= DateTime.UtcNow);

            if (seat == null) return BadRequest(new { message = "Reservation expired or invalid user." });

            // Update Seat
            seat.Status = "Booked";
            seat.LockedUntil = null; // Clear lock timer

            // Generate Ticket
            var ticket = new TicketDetail
            {
                TicketNumber = $"TKT-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}",
                ShowId = seat.ShowId,
                SeatId = seat.SeatId,
                UserId = req.UserId,
                AmountPaid = seat.Price * 1.1m, // Including dummy tax from bill
                PaymentStatus = "Success"
            };

            _context.TicketDetails.Add(ticket);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok(new { message = "Payment Successful! Ticket Confirmed.", Ticket = ticket });
        }
    }
}