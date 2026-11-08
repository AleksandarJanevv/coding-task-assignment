# Claims API

A RESTful Web API built with **ASP.NET Core (.NET 10)** for managing insurance covers and claims. The application provides CRUD operations, business rule validation, premium calculation, and asynchronous auditing.

## Technologies

- C# / .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- Microsoft SQL Server
- MongoDB
- Docker
- xUnit (unit testing)

## Features

### Claims Management
- Retrieve all claims or a specific claim.
- Create claims associated with existing insurance covers.
- Delete existing claims.
- Validate claim damage costs and coverage periods.

### Covers Management
- Retrieve all covers or a specific cover.
- Create and delete insurance covers.
- Automatically calculate insurance premiums based on cover type and duration.
- Prevent deletion of covers that have associated claims.

### Architecture and Design
- **Service Layer:** Separates business logic from HTTP controllers.
- **Data Access:** Uses Entity Framework Core and SQL Server for application data.
- **Validation:** Dedicated validators enforce business rules.
- **Builder Pattern:** Provides a consistent way to construct custom API exceptions.
- **Global Exception Handling:** Centralizes exception handling and standardized API error responses.
- **Asynchronous Auditing:** Uses a background service and an audit queue to process audit events independently of API requests.
- **MongoDB Auditing:** Uses MongoDB running in Docker to store audit records.

## Getting Started

### Prerequisites

- .NET 10 SDK
- Microsoft SQL Server
- Docker Desktop
- MongoDB (Docker container)

### Configuration

Configure your SQL Server connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=ClaimsDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Configure your MongoDB connection string and database name:

```json
{
  "MongoDbSettings": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "ClaimsAuditDb"
  }
}
```

Adjust these settings according to your environment.

### Start MongoDB with Docker

Start the MongoDB container:

```bash
docker run -d \
  --name claims-mongodb \
  -p 27017:27017 \
  -v claims-mongo-data:/data/db \
  mongo
```

The named Docker volume ensures that MongoDB data persists when the container is stopped or removed.

To start the existing container again:

```bash
docker start claims-mongodb
```

### Database Setup

Apply Entity Framework Core migrations:

```bash
dotnet ef database update
```

### Running the Application

```bash
dotnet restore
dotnet build
dotnet run
```

### Testing

Run the unit tests:

```bash
dotnet test
```

## API Endpoints

| Method | Endpoint | Description |
|---|---|---|
| GET | `/claims` | Retrieve all claims |
| GET | `/claims/{id}` | Retrieve a specific claim |
| POST | `/claims` | Create a claim |
| DELETE | `/claims/{id}` | Delete a claim |
| GET | `/covers` | Retrieve all covers |
| GET | `/covers/{id}` | Retrieve a specific cover |
| POST | `/covers` | Create a cover |
| DELETE | `/covers/{id}` | Delete a cover |
| POST | `/covers/compute` | Calculate a cover premium |

---

**Note:** API error responses follow a standardized structure containing a return code, error details, and an optional entity identifier.
