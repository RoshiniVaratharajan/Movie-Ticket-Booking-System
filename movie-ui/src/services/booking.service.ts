import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment.development';


export interface Show { showId: number; title: string; }
export interface Seat { seatId: number; seatNumber: string; price: number; status: string; }
export interface Bill { title: string; showId: number; seatNumber: string; basePrice: number; taxes: number; totalAmount: number; }
export interface Ticket { ticketNumber: string; showId: number; seatId: number; amountPaid: number; paymentStatus: string; }

@Injectable({
  providedIn: 'root'
})
export class BookingService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/Booking`;

  getShows(): Observable<Show[]> {
    return this.http.get<Show[]>(`${this.apiUrl}/shows`);
  }

  getLayout(showId: number): Observable<Seat[]> {
    return this.http.get<Seat[]>(`${this.apiUrl}/shows/${showId}/layout`);
  }

  lockSeat(showId: number, seatId: number, userId: number): Observable<any> {
    return this.http.post(`${this.apiUrl}/lock`, { showId, seatId, userId });
  }

  getBill(showId: number, seatId: number, userId: number): Observable<Bill> {
    return this.http.get<Bill>(`${this.apiUrl}/bill/${showId}/${seatId}/${userId}`);
  }

  payAndBook(showId: number, seatId: number, userId: number): Observable<{ message: string, ticket: Ticket }> {
    return this.http.post<{ message: string, ticket: Ticket }>(`${this.apiUrl}/pay`, { showId, seatId, userId });
  }
}