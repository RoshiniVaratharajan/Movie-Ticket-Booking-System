import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BookingService, Show, Seat, Bill, Ticket } from '../../services/booking.service';

@Component({
  selector: 'app-booking',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './booking.html',
  styleUrl: './booking.css'
})
export class BookingComponent implements OnInit {
  private bookingService = inject(BookingService);
  private cdr = inject(ChangeDetectorRef);
  step = 1;
  userId = 1; // Simulated logged-in user
  shows: Show[] = [];
  seats: Seat[] = [];
  selectedShowId!: number;
  selectedSeatId!: number;
  bill: Bill | null = null;
  ticket: Ticket | null = null;
  errorMessage = '';

  ngOnInit() {
    this.loadShows();
  }

  loadShows() {
    this.bookingService.getShows().subscribe({
      next: (res) => {  
        this.shows = res; 
        console.log('Data received:', this.shows);
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error:', err);
      }
    });
  }

  selectShow(showId: number) {
    this.selectedShowId = showId;
    this.bookingService.getLayout(showId).subscribe(res => {
      this.seats = res;
      this.step = 2;
      this.cdr.detectChanges();
    });
  }

  lockSeat(seat: Seat) {
    this.errorMessage = '';
    this.bookingService.lockSeat(this.selectedShowId, seat.seatId, this.userId).subscribe({
      next: () => {
        this.selectedSeatId = seat.seatId;
        this.loadBill();
      },
      error: (err) => {
        this.errorMessage = err.error?.message || 'Seat locking failed.';
        this.selectShow(this.selectedShowId); 
        this.cdr.detectChanges();
      }
    });
  }

  loadBill() {
    this.bookingService.getBill(this.selectedShowId, this.selectedSeatId, this.userId).subscribe(res => {
      this.bill = res;
      this.step = 3;
      this.cdr.detectChanges();
    });
  }

  pay() {
    this.bookingService.payAndBook(this.selectedShowId, this.selectedSeatId, this.userId).subscribe({
      next: (res) => {
        this.ticket = res.ticket;
        this.step = 4;
        this.cdr.detectChanges();
      },
      error: (err) => alert(err.error?.message || 'Payment failed or session expired.')
    });
  }

  reset() {
    this.step = 1;
    this.ticket = null;
    this.bill = null;
    this.loadShows();
  }
}