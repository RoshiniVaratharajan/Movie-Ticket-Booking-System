```markdown
# 🎬 Movie Ticket Booking System - Full Stack Prototype

A full-stack, concurrency-safe movie ticket booking prototype. This application demonstrates how to handle high-traffic booking scenarios using distributed temporary locks, ensuring that multiple users cannot book the same seat simultaneously.

## 📸 Application Flow & Screenshots
*(You can replace these placeholder paths with the actual image files from your "Day 1 Output.pdf" in your repository)*

1. **Show Selection:** Users can view available movie shows.
2. **Seat Layout:** Dynamic rendering of available seats (e.g., A1, A2, A3) with base pricing.
3. **Invoice & Checkout:** Generates a real-time bill showing the selected seat, base price (e.g., ₹150), calculated taxes (e.g., ₹15), and the total amount to be paid.
4. **Ticket Confirmation:** Successful payments generate a unique Ticket Number (e.g., TKT-E01EB9E4) and display the final success status.

## 🚀 Tech Stack
* **Frontend:** Angular 17+, TypeScript, RxJS, HTML5/CSS3
* **Backend:** C#, .NET 8, ASP.NET Core Web API
* **Database:** PostgreSQL via Entity Framework (EF) Core

## ✨ Key Features & System Design

* **Concurrency-Safe Seat Locking:** Implements a 10-minute temporary lock when a user selects a seat. Uses `ExecuteUpdateAsync` to atomically update the seat status in the database, preventing race conditions where two users click the same seat at the exact same millisecond.
* **Atomic Transactions:** The final payment and ticket generation step utilizes EF Core Database Transactions (`BeginTransactionAsync`). This ensures that seat updates and ticket generation either succeed together or fail together, maintaining strict data integrity.
* **Auto-Expiring Locks (Lazy Evaluation):** Lock expirations are evaluated dynamically on read. If a user abandons their cart, the seat lock naturally expires after 10 minutes without requiring an external background worker to clear the data.
* **RESTful Architecture:** Clean separation of concerns with distinct DTOs for requests and isolated endpoints for layout rendering, billing, and payment processing.
* **Normalized Database Foundation:** Structured to accommodate future scaling for multiplexes (Locations, Theatres, Screens, Shows) and dynamic category-based pricing (Economy vs. Premium).

## 🛠️ Setup & Installation

### 1. Database Setup (PostgreSQL)
1. Ensure PostgreSQL is installed and running on your local machine.
2. Create a new empty database for the project.
3. Open your database client and run the following SQL script to insert dummy seed data for testing:
   ```sql
   INSERT INTO "ShowSeats" ("ShowId", "SeatId", "SeatNumber", "Status", "Price") VALUES 
   (1, 1, 'A1', 'Available', 150.00),
   (1, 2, 'A2', 'Available', 150.00),
   (1, 3, 'A3', 'Available', 150.00),
   (2, 1, 'A1', 'Available', 200.00),
   (2, 2, 'A2', 'Available', 200.00);

```

### 2. Backend Setup (.NET API)

1. Ensure you have the [.NET 8 SDK](https://dotnet.microsoft.com/download) installed.
2. Navigate to the `.NET` API project directory.
3. Update the `DefaultConnection` string in `appsettings.json` (or `appsettings.Development.json`) to point to your local PostgreSQL instance.
4. Open a terminal and run the Entity Framework migrations to generate the tables:
```bash
dotnet ef database update

```


5. Start the backend development server:
```bash
dotnet run

```


*The Swagger UI API documentation will be available at `http://localhost:<port>/swagger`.*

### 3. Frontend Setup (Angular)

1. Ensure you have [Node.js](https://nodejs.org/) and the [Angular CLI](https://angular.io/cli) installed.
2. Open a separate terminal and navigate to the Angular project directory.
3. Install the required Node dependencies:
```bash
npm install

```


4. Verify that the API URL in your `src/environments/environment.ts` (or your relevant service file) matches the local port where your .NET backend is running.
5. Start the Angular development server:
```bash
ng serve -o

```


*The application will automatically open in your default browser at `http://localhost:4200`.*

---

**Author:** Roshini V

```

```