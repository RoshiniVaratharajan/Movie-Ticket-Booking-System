import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import { BookingComponent } from './booking/booking';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, BookingComponent], // Add to imports
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class AppComponent {
  title = 'movie-ui';
}