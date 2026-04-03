
# Office Management System

![.NET Version](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)
![Angular Version](https://img.shields.io/badge/Angular-20-DD0031?logo=angular)
![Database](https://img.shields.io/badge/Database-SQLite-003B57?logo=sqlite)
![Architecture](https://img.shields.io/badge/Architecture-Clean_Architecture-brightgreen)

Welcome to the **Office Management System**, a full-stack, enterprise-grade application built to streamline typical administrative and HR activities within an organization. This project was developed as a comprehensive Dotnet Individual Project, utilizing modern web development architectures and technologies.

## 🌟 Key Features

The system provides robust functionalities tailored to manage core organizational needs:

- **Employee Management:** Complete lifecycle management of employee records.
- **Department Organization:** Group employees into appropriate departments.
- **Role-Based Access Control:** Define various roles and permissions for administrative and staff levels.
- **Leave Management System:**
  - Track **Leave Balances** for each employee.
  - Submit, review, and approve **Leave Requests**.
- **Secure Authentication:** JWT-based user authentication ensuring secure access.
- **API Documentation:** Integrated Swagger UI for easy API exploration and testing.

## 🛠️ Technology Stack

This application demonstrates modern, scalable development practices by decoupling the frontend and backend.

### Backend (Web API)
- **Framework:** .NET 10.0 
- **Architecture:** Clean Architecture pattern (Core, Infrastructure, API layers)
- **ORM:** Entity Framework Core 10
- **Database:** SQLite (for local development simplicity)
- **Security:** JWT Authentication
- **API Documentation:** Swashbuckle (Swagger)

### Frontend (Client App)
- **Framework:** Angular 20 
- **Language:** TypeScript 5.9
- **Reactive Programming:** RxJS
- **Tooling:** Angular CLI, Prettier

## 📂 Project Architecture

The backend follows **Clean Architecture** principles to separate business logic, infrastructure, and presentation, enhancing testability and maintainability:

* **`OfficeManagementSystem.API/`**: Main entry point; handles HTTP requests/responses, controllers, and JWT auth.
* **`OfficeManagementSystem.Core/`**: Contains core domain entities (`Employee`, `Department`, `LeaveRequest`, `User`, etc.) and system interfaces. No dependencies on external layers.
* **`OfficeManagementSystem.Infrastructure/`**: Implementations for data access (EF Core DbContext), migrations, and external services.
* **`OfficeManagementSystem.Client/`**: The complete Angular frontend SPA application.

## 🚀 Getting Started

To run this project locally, follow these steps:

### Prerequisites
- [.NET 10.0 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) (v18 or later recommended)
- [Angular CLI](https://angular.io/cli) globally installed (`npm install -g @angular/cli`)

### 1. Set Up the Backend
1. Navigate to the API folder:
   ```bash
   cd OfficeManagementSystem.API
   ```
2. Build the application:
   ```bash
   dotnet build
   ```
3. Run the application:
   ```bash
   dotnet run
   ```
4. *The API will start and normally be accessible at `http://localhost:5000` or `https://localhost:5001`. You can access the Swagger UI by appending `/swagger` to the URL.*

### 2. Set Up the Frontend
1. Open a new terminal and navigate to the Client folder:
   ```bash
   cd OfficeManagementSystem.Client
   ```
2. Install dependencies:
   ```bash
   npm install
   ```
3. Start the Angular development server:
   ```bash
   npm start
   ```
4. *Navigate to `http://localhost:4200` in your browser to interact with the application.*

---
*Created as part of a Semester 5 .NET Individual Project.*
