# Chrome

**Chrome** is a powerful and scalable backend framework designed for **inventory**, **manufacturing**, and **warehouse management**.  
It provides a **modular architecture**, **secure APIs**, and **comprehensive business logic** to help developers build **robust supply chain solutions** efficiently.

---

## 📜 Overview

Chrome streamlines complex logistics workflows and ensures **data consistency** across different systems.  
With its **clean architecture** and **extensive services**, it can easily integrate into existing ERP or WMS systems, or be the foundation for a new enterprise-grade solution.

---

## 🚀 Why Chrome?

- 🧩 **Modular Architecture** – Organized solution with clear dependencies & configurations.
- 🔐 **Secure Authorization** – Custom role-based access control with flexible permission policies.
- ⚙️ **Comprehensive Services** – Business logic for stock, transfers, manufacturing, and inventory.
- 📊 **Standardized Data Exchange** – DTO pattern for seamless backend–frontend communication.
- 🛠 **API-Driven** – RESTful endpoints supporting CRUD operations and complex queries.
- 📦 **Automation & Tracking** – QR/Barcode generation for asset and inventory tracking.

---

## 🏗️ Project Structure

```
Chrome/
├── Chrome.API             # API layer (Controllers, Filters, Middleware)
├── Chrome.Application     # Business logic services, DTOs, validation
├── Chrome.Domain          # Entities, enums, interfaces
├── Chrome.Infrastructure  # EF Core repositories, database context
└── Chrome.Shared          # Common utilities, constants, exceptions
```

---

## 🛠️ Tech Stack

- **Backend Framework:** [.NET 8](https://dotnet.microsoft.com/) / .NET Core  
- **Database:** SQL Server (via Entity Framework Core)  
- **Authentication:** JWT Bearer Tokens  
- **API Documentation:** Swagger / OpenAPI  
- **Automation:** QR/Barcode Generation  
- **Architecture:** Clean Architecture + SOLID Principles  

---

## 📦 Installation & Setup

### 1️⃣ Clone the Repository
```bash
git clone https://github.com/lntb1712/Chrome.git
cd Chrome
```

### 2️⃣ Configure Database Connection
Edit **`appsettings.json`** in `Chrome.API`:
```json
"ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=ChromeDB;User Id=sa;Password=YOUR_PASSWORD;"
}
```

### 3️⃣ Apply Migrations
```bash
dotnet ef database update
```

### 4️⃣ Run the Application
```bash
dotnet run --project Chrome.API
```

### 5️⃣ Access API Documentation
Open Swagger UI in your browser:  
👉 [http://localhost:5000/swagger](http://localhost:5000/swagger)

---

## 📚 Core Features

### 🔐 Authentication & Authorization
- JWT-based secure authentication
- Role-based access control (RBAC)
- Flexible permission policies

### 📦 Inventory Management
- Stock In / Stock Out
- Warehouse Transfers
- Real-time quantity tracking

### 🏭 Manufacturing Module
- Bill of Materials (BOM) management
- Production order tracking

### 🔄 Data Exchange
- Standardized DTOs for all APIs
- Consistent request/response structure

### 🤖 Automation
- QR & Barcode generation
- Asset and shipment tracking

---

## 📂 API Endpoints (Sample)

| Method | Endpoint               | Description                  | Auth Required |
|--------|-----------------------|------------------------------|---------------|
| GET    | `/api/products`       | Get all products              | ✅            |
| POST   | `/api/products`       | Create a new product          | ✅            |
| GET    | `/api/warehouses`     | List all warehouses           | ✅            |
| POST   | `/api/stockin`        | Create stock-in record        | ✅            |
| POST   | `/api/auth/login`     | User login & token generation | ❌            |

*(Full API documentation available in Swagger UI)*

---

## 🤝 Contributing

We welcome contributions!  
To contribute:
1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Open a pull request

Make sure to follow **Clean Architecture** and **SOLID principles** in your code.

---

## 📜 License

This project is licensed under the **MIT License** – see the [LICENSE](LICENSE) file for details.

---

## 📧 Contact

For any inquiries or collaboration opportunities, please contact:  
**Author:** Thanh Bình Lê Nguyễn  
**GitHub:** [@lntb1712](https://github.com/lntb1712)  
