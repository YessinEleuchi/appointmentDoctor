# 🏥 Doctor Appointment Management System

<div align="center">

![.NET](https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white) ![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white) ![SQL Server](https://img.shields.io/badge/SQL_Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white) ![JWT](https://img.shields.io/badge/JWT-000000?style=for-the-badge&logo=JSON%20web%20tokens&logoColor=white) ![Swagger](https://img.shields.io/badge/Swagger-85EA2D?style=for-the-badge&logo=Swagger&logoColor=white)

</div>

## 📝 Description

A robust and secure Doctor Appointment Management System built with ASP.NET Core. This system facilitates the management of medical appointments, patient records, and doctor schedules while ensuring secure authentication and authorization.

## ✨ Features

- 🔐 Secure Authentication using JWT
- 👥 Role-based Authorization (Admin, Doctor, Patient)
- 📅 Appointment Management
- 📋 Medical History Tracking
- 📧 Email Notifications
- 📱 RESTful API Architecture
- 📚 Swagger Documentation
- 🔄 CORS Support
- 📊 SQL Server Database

## 🛠️ Technologies Used

- ASP.NET Core
- Entity Framework Core
- SQL Server
- JWT Authentication
- AutoMapper
- Swagger/OpenAPI
- Identity Framework
- SMTP Email Service

## 🚀 Getting Started

### Prerequisites

- .NET 7.0 SDK or later
- SQL Server
- Visual Studio 2022 or VS Code

### Installation

1. Clone the repository
```bash
git clone https://github.com/YessinELEUCHI/doctor-appointment-system.git
```

2. Update the connection string in `appsettings.json`
```json
"ConnectionStrings": {
    "dbcon": "Your_Connection_String_Here"
}
```

3. Update the JWT settings in `appsettings.json`
```json
"JWT": {
    "iss": "Your_Issuer",
    "aud": "Your_Audience",
    "SecretKey": "Your_Secret_Key"
}
```

4. Update email settings in `appsettings.json`
```json
"EmailSettings": {
    "SmtpHost": "Your_SMTP_Host",
    "SmtpPort": Your_Port,
    "SenderEmail": "Your_Email",
    "SenderPassword": "Your_Password"
}
```

5. Run the application
```bash
dotnet run
```

## 📚 API Documentation

Once the application is running, you can access the Swagger documentation at:
```
https://localhost:5001/swagger
```

## 🔐 Default Admin Credentials

- **Username**: admin
- **Email**: admin@admin.com
- **Password**: Admin@123

## 📁 Project Structure

```
appointmentDoctor/
├── Controllers/     # API Controllers
├── Models/         # Domain Models
├── DTO/            # Data Transfer Objects
├── Service/        # Business Logic Services
├── Mapping/        # AutoMapper Profiles
├── Config/         # Configuration Files
└── Migrations/     # Database Migrations
```

## 👨‍💻 Author

**Yessine ELEUCHI**


