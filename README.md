# SlotSync — Smart Venue & Slot Booking API

SlotSync is a backend-focused venue and time-slot booking system built using ASP.NET Core Web API.

The application allows customers to browse venues and available time slots, create bookings for specific dates, and manage their bookings. Administrators can manage venues, slots, users, and booking status.

The project was built to practice real-world .NET backend development, including authentication, authorization, Entity Framework Core, PostgreSQL, DTOs, service-layer architecture, validation, exception handling, Docker, and cloud deployment.

---

## 🔗 Project Links

### GitHub Repository

[SlotSync — GitHub](https://github.com/GopalBharti223/SlotSync)

### Live API / Swagger

[SlotSync — Live Swagger](https://slotsync-gv0r.onrender.com/swagger/index.html)

### Frontend

Frontend is deployed separately on Render.

**Frontend URL:** Add the final Render frontend URL here once available.

---

## 🚀 Live Application

The backend API is deployed on **Render**.

The production database is hosted on **Neon PostgreSQL**.

### Architecture

```text
Client / Frontend
       ↓
ASP.NET Core Web API
       ↓
Controllers
       ↓
Services
       ↓
Entity Framework Core
       ↓
Neon PostgreSQL
```

---

# 🛠️ Tech Stack

### Backend

* C#
* ASP.NET Core Web API
* .NET 10
* Entity Framework Core
* LINQ
* Async/Await
* Dependency Injection
* DTOs

### Database

* PostgreSQL
* Neon PostgreSQL
* Npgsql
* Entity Framework Core

### Authentication & Security

* JWT Bearer Authentication
* Role-Based Authorization
* Admin / Customer roles
* Password Hashing
* Ownership-based authorization

### API & Development

* RESTful APIs
* Swagger / OpenAPI
* Custom Exception Middleware
* HTTP Status Codes
* Model Validation

### Deployment

* Docker
* Render
* Neon PostgreSQL
* GitHub

---

# 👥 User Roles

SlotSync has two main roles:

## Customer

Customers can:

* Register
* Login
* Receive JWT authentication token
* View venues
* View available slots
* Create bookings
* View their own bookings
* View individual bookings
* Cancel their own pending bookings

## Admin

Administrators can:

* View users
* Delete users
* Create venues
* Update venues
* Delete venues
* Create slots
* Update slots
* Delete slots
* View all bookings
* Confirm bookings
* Complete bookings
* Cancel bookings

---

# 🔐 Authentication

SlotSync uses **JWT Bearer Authentication**.

After login, the API generates a JWT containing claims such as:

```text
UserId
Email
Role
```

The role claim is used for role-based authorization.

Example:

```text
Customer
   ↓
JWT
   ↓
Protected API
   ↓
Customer permissions
```

For admin operations:

```text
Admin
   ↓
JWT with Role = Admin
   ↓
[Authorize(Roles = "Admin")]
   ↓
Admin endpoint
```

A customer attempting to access an admin-only endpoint receives:

```text
403 Forbidden
```

A request without a valid token to a protected endpoint receives:

```text
401 Unauthorized
```

---

# 🏢 Venue Management

Venues represent the locations where bookings can be made.

Each venue contains information such as:

* Venue ID
* Name
* Venue Type
* Capacity
* Available Slots

Customers can view venues publicly.

Administrators can:

```text
Create Venue
Update Venue
Delete Venue
```

The venue API also returns associated slots using DTO projection.

This avoids exposing the complete EF Core entity relationship and prevents circular JSON serialization.

---

# 🕐 Slot Management

A slot represents a bookable time period inside a venue.

Each slot contains:

* Slot ID
* Venue ID
* Cost
* Start Time
* End Time

Example:

```text
Venue: Test Venue

Slot:
10:00 AM → 11:00 AM
Cost: ₹600
```

Administrators can create, update, and delete slots.

Customers can view available slots.

Slot management endpoints are protected using Admin role authorization.

---

# 📅 Booking System

The booking system is the main business functionality of SlotSync.

A customer creates a booking by providing:

```json
{
  "slotId": 3,
  "bookingDate": "2026-09-18"
}
```

The user ID is **not taken from the request body**.

Instead, it is extracted from the authenticated JWT.

This prevents a customer from creating a booking on behalf of another user.

---

# 💰 Server-Side Cost Calculation

The final booking cost is calculated by the server from the selected slot.

For example:

```text
Slot Cost = ₹600

Customer Request
      ↓
SlotId = 3
      ↓
Server finds Slot 3
      ↓
FinalCost = ₹600
```

The customer cannot simply send a different price and change the booking cost.

---

# 🔄 Booking Status Workflow

Bookings follow a controlled status flow.

```text
              ┌──────────────┐
              │   Pending    │
              └──────┬───────┘
                     │
              Admin Confirm
                     ↓
              ┌──────────────┐
              │  Confirmed   │
              └──────┬───────┘
                     │
              Admin Complete
                     ↓
              ┌──────────────┐
              │  Completed   │
              └──────────────┘
```

A pending booking can also be cancelled:

```text
Pending → Cancelled
```

Invalid transitions are rejected.

For example:

```text
Pending → Completed ❌
Cancelled → Completed ❌
```

---

# 🛡️ Booking Validation

The BookingService contains important business rules.

### Past Date Validation

Bookings cannot be created for a date in the past.

### Slot Validation

The selected slot must exist.

### Duplicate Booking Prevention

The same slot cannot be booked twice for the same date.

The database also contains a unique constraint/index on:

```text
SlotId + BookingDate
```

This provides an additional layer of protection against duplicate bookings.

### User Validation

The authenticated user must exist.

### Slot Time Validation

The slot must have a valid start/end time.

---

# 👤 Booking Ownership

Customers can only access their own bookings.

For example:

```text
Customer A
   ↓
Booking A
   ↓
Allowed ✅

Customer A
   ↓
Booking B owned by Customer B
   ↓
Forbidden ❌
```

Administrators can access bookings across users.

This ownership check is implemented in the booking service/controller logic.

---

# 🧱 Project Architecture

SlotSync follows a layered backend structure.

```text
Controllers
     ↓
Services
     ↓
Entity Framework Core
     ↓
PostgreSQL
```

### Controllers

Responsible for:

* HTTP requests
* Routing
* Authorization
* Returning HTTP responses

Examples:

```text
AuthController
UsersController
VenuesController
SlotController
BookingController
```

### Services

Business logic is kept inside services rather than putting everything directly inside controllers.

Example:

```text
BookingController
       ↓
BookingService
       ↓
Database
```

This makes the application easier to maintain and test.

---

# 📦 DTOs

SlotSync uses Data Transfer Objects instead of directly exposing database entities through API endpoints.

Examples include:

```text
CreateBookingRequestDto
BookingResponseDto
VenueWithSlotsResponseDto
SlotResponseDto
```

DTOs help control exactly what information the API accepts and returns.

They also helped avoid circular references between:

```text
Venue → Slots → Venue
```

---

# 🗄️ Database Design

The main database tables are:

```text
Users
  │
  │ 1
  │
  └──────< Bookings >────── Slots
                             │
                             │
                             ▼
                           Venues
```

### Users

Stores:

* User ID
* Email
* Password Hash
* Role

### Venues

Stores:

* Venue ID
* Name
* Venue Type
* Capacity

### Slots

Stores:

* Slot ID
* Venue ID
* Cost
* Start Time
* End Time

### Bookings

Stores:

* Booking ID
* User ID
* Slot ID
* Booking Date
* Final Cost
* Status

---

# ⚠️ Exception Handling

SlotSync uses custom exceptions and centralized exception handling.

Custom exceptions include:

```text
NotFoundException
BadRequestException
ConflictException
```

A custom exception middleware converts these exceptions into appropriate HTTP responses.

Example:

```text
Resource not found
        ↓
404 Not Found
```

```text
Invalid request
        ↓
400 Bad Request
```

```text
Duplicate booking
        ↓
409 Conflict
```

This keeps error handling consistent across the API.

---

# 📖 Swagger / OpenAPI

Swagger is included for API documentation and testing.

It allows endpoints to be tested directly through the browser.

The live Swagger API is available here:

[Open SlotSync Swagger](https://slotsync-gv0r.onrender.com/swagger/index.html)

Swagger can be used to test:

* Authentication
* Venues
* Slots
* Bookings
* Admin operations
* Customer operations

JWT Bearer authentication is configured in Swagger so protected endpoints can be tested using an access token.

---

# 🧪 Testing

The project can be extended with an xUnit test project for automated unit testing of the service layer.

Important BookingService scenarios include:

* Successful booking creation
* Duplicate booking prevention
* Past booking date validation
* Invalid booking status transitions

The goal of these tests is to automatically verify important booking business rules without manually testing every scenario through Swagger.

---

# 🐳 Docker

The backend includes Docker configuration for containerized deployment.

The application can be packaged into a Docker container and deployed to a cloud hosting platform such as Render.

---

# ☁️ Deployment

### Backend

```text
GitHub
   ↓
Render
   ↓
ASP.NET Core Web API
```

### Database

```text
Neon PostgreSQL
        ↑
        │
ASP.NET Core API
```

The production backend uses PostgreSQL through **Npgsql** and Entity Framework Core.

---

# 🔌 Main API Endpoints

## Authentication

```text
POST /api/Auth/login
```

## Users

```text
POST   /api/Users
GET    /api/Users
GET    /api/Users/{id}
DELETE /api/Users/{id}
```

## Venues

```text
GET    /api/Venues
GET    /api/Venues/{id}
POST   /api/Venues
PUT    /api/Venues/{id}
DELETE /api/Venues/{id}
```

## Slots

```text
GET    /api/Slot
GET    /api/Slot/{id}
POST   /api/Slot
PUT    /api/Slot/{id}
DELETE /api/Slot/{id}
```

## Bookings

```text
GET /api/Bookings
GET /api/Bookings/{id}

POST /api/Bookings

PUT /api/Bookings/{id}/confirm
PUT /api/Bookings/{id}/cancel
PUT /api/Bookings/{id}/complete
```

---

# 🧪 Example Booking Flow

```text
1. Customer registers
          ↓
2. Customer logs in
          ↓
3. JWT token generated
          ↓
4. Customer views venues
          ↓
5. Customer views slots
          ↓
6. Customer selects slot + date
          ↓
7. Booking created
          ↓
8. Status = Pending
          ↓
9. Admin confirms
          ↓
10. Status = Confirmed
          ↓
11. Admin completes
          ↓
12. Status = Completed
```

---

# 🎯 Project Objective

The main objective of SlotSync was to build a practical ASP.NET Core backend that demonstrates real-world concepts such as:

* REST API development
* JWT authentication
* Role-based authorization
* Entity Framework Core
* PostgreSQL
* Service-layer architecture
* DTO-based API design
* Business-rule validation
* Database constraints
* Exception middleware
* Docker
* Cloud deployment
* Automated testing concepts

---

## 📌 Repository

**GitHub:**
https://github.com/GopalBharti223/SlotSync

**Live Swagger:**
https://slotsync-gv0r.onrender.com/swagger/index.html

**Frontend:**
Add the final Render frontend URL when available.
