# CourtSide API

CourtSide API is an ASP.NET Core Web API designed to support a basketball management application. The API provides secure endpoints for managing users, teams, players, coaches, attendance, notifications, and other basketball-related functionality.

The project uses **ASP.NET Core**, **Entity Framework Core**, **SQL Server**, **ASP.NET Core Identity**, and **JWT Bearer Authentication**.

---

## Features

* 🔐 JWT-based authentication
* 👤 User registration and login
* 🔄 Refresh token support
* 🛡️ Role-based authorization
* 🏀 Team management
* 👨‍🏫 Coach management
* 🏃 Player management
* 📋 Attendance management
* 🔔 Notification support
* 📱 FCM token support for push notifications
* 🌍 Language preferences
* 🔒 Biometric authentication preference
* 🗄️ SQL Server database using Entity Framework Core
* 📖 Swagger/OpenAPI documentation

---

## Technologies

| Technology            | Purpose                  |
| --------------------- | ------------------------ |
| ASP.NET Core          | Web API framework        |
| .NET 10               | Application runtime      |
| Entity Framework Core | Database access          |
| SQL Server            | Relational database      |
| ASP.NET Core Identity | User and role management |
| JWT Bearer            | Authentication           |
| Swagger / OpenAPI     | API documentation        |
| C#                    | Programming language     |

---

##  User Roles

CourtSide API supports different user roles:

* **Admin** – System administration and management
* **Coach** – Manages teams and players assigned to them
* **Player** – Accesses player-related functionality
* **Guest** – Limited access to public functionality

Role-based authorization is implemented using ASP.NET Core authorization policies and attributes.

---

##  Authentication

The API uses **JWT Bearer Authentication**.

Users first authenticate through the login endpoint. A successful login returns an access token that must be included when accessing protected endpoints.

### Authorization Header

For protected endpoints, include the JWT token in the request header:

```http
Authorization: Bearer YOUR_JWT_TOKEN
```

Swagger can also be configured to accept the token through its **Authorize** button.

---

## Refresh Tokens

CourtSide API provides a refresh-token mechanism so that users can obtain a new access token without logging in again when their current access token expires.

The refresh endpoint is:

```http
POST /refresh-token
```

The exact request body depends on the implementation of the refresh-token endpoint.

---

##  Registration

New users can register through the registration endpoint.

A user account contains information such as:

* Full name
* Email address
* Password
* User role
* Biometric authentication preference
* Language preference
* Notification preferences
* Firebase Cloud Messaging token

Players are not automatically assigned to a team during registration. Team assignment is handled separately.

---

##  Team Management

Team functionality allows authorized users, particularly coaches, to work with teams.

Examples of functionality include:

* Viewing teams
* Viewing teams associated with the current coach
* Managing team information
* Associating players with teams

### Coach's Teams

Coaches can retrieve the teams associated with their account using:

```http
GET /api/teams/my-teams
```

This endpoint requires authentication and coach authorization.

---

##  Player Management

Players are represented separately from the base application user.

Player functionality can include:

* Player profiles
* Player information
* Team membership
* Player-related basketball data

A player is **not assigned a team during account registration**. Team allocation is handled after registration.

---

## Attendance

The API provides functionality for managing attendance associated with basketball activities.

Attendance functionality can be used to:

* Record attendance
* View attendance
* Associate attendance with players
* Track attendance for relevant activities

Protected attendance endpoints require an authenticated user with the appropriate permissions.

---

## Notifications

CourtSide API supports notification-related functionality.

User notification information can include preferences for receiving different types of notifications.

The `ApplicationUser` model also supports an **FCM token**, which can be used when integrating Firebase Cloud Messaging for push notifications.

---

## User Profile

The application user contains several profile and application-specific properties, including:

```text
FullName
BiometricEnabled
LanguagePref
FcmToken
NotificationPreferences
```

These properties allow the API to support personalization and mobile-app functionality.

---

## Database

The API uses **SQL Server** with **Entity Framework Core**.

The database connection is configured through the application's configuration files.

Example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "YOUR_CONNECTION_STRING"
  }
}
```

### Security

Do **not** commit real database credentials, JWT secrets, passwords, or API keys to GitHub.

Use:

* User Secrets
* Environment variables
* Azure Key Vault
* Other secure configuration providers

for sensitive values.

---

## 🔑 JWT Configuration

JWT configuration should be stored securely and should not contain production secrets inside `appsettings.json`.

A typical configuration structure may look like:

```json
{
  "Jwt": {
    "Issuer": "YOUR_ISSUER",
    "Audience": "YOUR_AUDIENCE",
    "Secret": "YOUR_SECRET"
  }
}
```

For development, sensitive values should preferably be stored using **ASP.NET Core User Secrets**.

Example:

```bash
dotnet user-secrets init
```

Then configure the required values using:

```bash
dotnet user-secrets set "Jwt:Secret" "YOUR_SECRET"
```

---

## ⚙️ Getting Started

### Prerequisites

Make sure you have installed:

* .NET 10 SDK
* SQL Server
* Visual Studio 2026 or another compatible IDE
* Git

---

### 1. Clone the repository

```bash
git clone YOUR_REPOSITORY_URL
```

Navigate into the project:

```bash
cd CourtSideAPI
```

---

### 2. Configure the database

Configure the SQL Server connection string using User Secrets or another secure configuration provider.

Example:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "YOUR_CONNECTION_STRING"
```

---

### 3. Configure JWT

Set the JWT configuration using secure configuration:

```bash
dotnet user-secrets set "Jwt:Secret" "YOUR_SECRET"
```

Also configure the issuer and audience if required by the application.

---

### 4. Apply migrations

Run:

```bash
dotnet ef database update
```

If Entity Framework CLI is not installed:

```bash
dotnet tool install --global dotnet-ef
```

---

### 5. Run the API

```bash
dotnet run
```

The API will start using the configured ASP.NET Core environment.

---

## 📖 Swagger

When the application is running, Swagger provides interactive API documentation.

Open the Swagger URL displayed by the application, commonly:

```text
https://localhost:<port>/swagger
```

Swagger allows developers to:

* View available endpoints
* Inspect request and response models
* Test API endpoints
* Authenticate using a JWT token

### Using JWT in Swagger

1. Log in using the authentication endpoint.
2. Copy the returned access token.
3. Click **Authorize** in Swagger.
4. Enter:

```text
Bearer YOUR_JWT_TOKEN
```

5. Click **Authorize**.
6. Protected endpoints can now be tested.

---

## Security

CourtSide API uses several security mechanisms:

* JWT authentication
* Role-based authorization
* ASP.NET Core Identity
* Password hashing through Identity
* Protected API endpoints
* Secure configuration for sensitive values

Never commit the following to the repository:

```text
Passwords
JWT secrets
Database passwords
API keys
Firebase credentials
Production connection strings
```

---

## Project Structure

A simplified structure is:

```text
CourtSideAPI/
│
├── Controllers/
│   ├── AccountController.cs
│   ├── TeamsController.cs
│   ├── PlayersController.cs
│   └── ...
│
├── Data/
│   └── AppDbContext.cs
│
├── Models/
│   ├── ApplicationUser.cs
│   ├── Coach.cs
│   ├── Player.cs
│   └── ...
│
├── DTOs/
│   └── ...
│
├── Services/
│   └── ...
│
├── Migrations/
│   └── ...
│
├── Program.cs
├── appsettings.json
└── CourtSideAPI.csproj
```

The exact structure may differ depending on the current implementation.

---

## Testing

The API can be tested using:

* Swagger
* Postman
* .NET HTTP clients
* The CourtSide mobile application

When testing protected endpoints, remember to authenticate first and provide the JWT access token.

---

## Typical Authentication Flow

```text
┌──────────────┐
│    Client    │
└──────┬───────┘
       │
       │ Login
       ▼
┌────────────────────┐
│   CourtSide API    │
└─────────┬──────────┘
          │
          │ Validate credentials
          ▼
┌────────────────────┐
│ ASP.NET Identity   │
└─────────┬──────────┘
          │
          │ Valid
          ▼
┌────────────────────┐
│    JWT Token       │
└─────────┬──────────┘
          │
          │ Authorization: Bearer token
          ▼
┌────────────────────┐
│ Protected Endpoint │
└────────────────────┘
```

---

##  API Architecture

CourtSide API follows a RESTful Web API architecture.

The client communicates with the API using HTTP requests:

```text
Mobile Application
        │
        │ HTTP / HTTPS
        ▼
┌───────────────────────┐
│     CourtSide API     │
│                       │
│ Authentication        │
│ Authorization         │
│ Business Logic        │
│ Controllers           │
└───────────┬───────────┘
            │
            │ Entity Framework Core
            ▼
┌───────────────────────┐
│       SQL Server      │
└───────────────────────┘
```

---

## 🎓Project Purpose

CourtSide API was developed as the backend service for a basketball management application.

The project demonstrates practical implementation of:

* RESTful API development
* Authentication and authorization
* Database management
* Entity Framework Core
* ASP.NET Core Identity
* JWT security
* Role-based access control
* CRUD operations
* Mobile API integration

---

##  Development

This project is intended for educational and development purposes.

Before deploying the API to production, additional security, validation, logging, monitoring, rate limiting, and deployment configuration should be reviewed.

---

##  License

This project was developed for educational purposes.
