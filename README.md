# E-Payroll-System


A web-based payroll management system developed for community schools under local municipal administration in Nepal. Built as a final year project for the Bachelor of Information and Communication Technology Education (BICTE) at Tribhuvan University.

---

## Overview

Many public schools in Nepal still process teacher salaries manually using spreadsheets or handwritten records — a process prone to errors, delays, and lack of transparency. This system replaces that with a centralized, automated platform that handles salary calculation, payroll history, and a structured approval workflow between schools and municipalities.

---

## Features

- **Automated Salary Calculation** — Computes salaries based on teacher level, grade, and government-mandated allowances (PF 10%, CIT 10%, Festival Allowance, Dress Allowance, Dearness Allowance, Headmaster Allowance)
- **Role-Based Access Control** — Three user roles with clearly defined permissions: Super Admin, Municipality Admin, and School
- **Payroll Submission & Approval Workflow** — Schools submit salary requests; municipalities receive real-time notifications and can approve or reject with remarks
- **Real-Time Status Updates** — Approval/rejection status is immediately reflected on the School dashboard
- **Teacher Record Management** — Full CRUD operations on teacher profiles via a 3-step registration form
- **Salary Slip Generation** — Generate and store monthly salary slips
- **Payroll History & Reporting** — Centralized records with summary reports for schools and municipalities
- **Notification System** — In-app notifications for salary request events

---

## Tech Stack

| Layer | Technology |
|---|---|
| Frontend | HTML5, CSS3, JavaScript, Bootstrap 5 |
| Backend | ASP.NET MVC, C# |
| ORM | Entity Framework Core |
| Database | SQL Server Express / LocalDB |
| Design / Prototyping | Figma |
| Version Control | Git, GitHub |
| IDE | Visual Studio 2022 |

---

## User Roles

### Super Admin
- Manages Municipality Admin accounts (create, edit, delete)
- Monitors overall system activity

### Municipality Admin
- Registers and manages schools
- Receives notifications for salary submissions
- Reviews, approves, or rejects salary requests
- Views teachers per school and consolidated payroll reports

### School
- Manages teacher records (personal info, qualifications, appointment details)
- Configures salary components (levels, categories, grades, allowances)
- Calculates monthly salaries automatically
- Submits payroll requests to the municipality
- Tracks approval/rejection status in real time

---

## System Architecture

The system follows a three-tier architecture:

```
Presentation Layer  ←→  Application Layer (Business Logic)  ←→  Data Layer (SQL Server)
  (Browser/UI)               (ASP.NET MVC / C#)                  (EF Core ORM)
```

---

## Database Schema (Key Tables)

- `Users` — Authentication and role management
- `Admins` — Municipality admin profiles with geographic references
- `Schools` — School records linked to admins
- `Teachers` — Full teacher profiles including personal, address, appointment, and qualification data
- `Salaries` — Salary records with all allowance and deduction components
- `TeacherLevel`, `TeacherCategory`, `GradeNumber`, `BasicSalary` — Master data for salary configuration
- `AppointmentType` — Teacher appointment classifications
- `Notification` — In-app notification records
- `Countries`, `Provinces`, `Municipality` — Geographic reference tables

---

## Getting Started

### Prerequisites

- [Visual Studio 2022](https://visualstudio.microsoft.com/)
- [SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) or LocalDB
- .NET SDK (compatible with ASP.NET Core MVC)
- Git

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/your-username/e-payroll-system.git
   cd e-payroll-system
   ```

2. **Open the solution** in Visual Studio 2022

3. **Configure the database connection** in `appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=EPayrollDb;Trusted_Connection=True;"
   }
   ```

4. **Apply migrations** to set up the database:
   ```bash
   dotnet ef database update
   ```

5. **Run the application**:
   ```bash
   dotnet run
   ```
   Or press `F5` in Visual Studio.

6. **Access the app** at `https://localhost:5001` (or the port shown in the console)

---

## Development Methodology

The project was built using the **Incremental SDLC** model, delivering functionality in independent modules (Super Admin → Municipality Admin → School) before full integration. This allowed continuous testing, iterative refinement, and systematic quality control throughout development.

---

## Testing

All modules were tested across three levels:

- **Unit Testing** — Individual functions tested per role (login, CRUD, salary calculation, approval workflow)
- **Integration Testing** — Module interactions verified end-to-end across all three user roles
- **System Testing** — Full system validation; all test suites passed

---

## Limitations

- No real-time bank integration; salary disbursement beyond approval remains manual
- Basic security only (password hashing, RBAC); MFA and enterprise-grade compliance not included
- Requires internet access and a modern browser; no offline or mobile-native support
- Salary rules are configurable but tied to current government policy structure
- Basic audit logging; no automated disaster recovery

---

## Future Recommendations

- Banking API integration for direct salary disbursement post-approval
- Mobile app for school admins and municipal officers in remote areas
- Two-factor authentication and data encryption at rest
- Offline mode with queued sync
- Configurable allowance engine for policy updates without code changes
- Multi-tenant architecture for province- or nation-wide rollout

---



**Supervisor:** Er. Om Prakash Yadav  
**Institution:** Nilkantha Multiple Campus, Dhading Besi, Dhading  
**University:** Tribhuvan University, Faculty of Education  
**Submitted:** February 2026

---

## License

This project was developed for academic purposes under Tribhuvan University. Please contact the authors for reuse or adaptation.
