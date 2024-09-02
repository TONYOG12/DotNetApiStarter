# DotNetApiStarter

A robust, flexible, and scalable .NET API starter template designed to accelerate the development of enterprise-grade applications. This template includes a pre-configured setup with Entity Framework Core, AutoMapper, JWT authentication, role-based access control, multitenancy, and essential services for cloud storage and logging. Perfect for developers looking to kickstart their .NET projects with a solid foundation.

## Features

- **Entity Framework Core**: Pre-configured for use with SQL databases and includes an in-memory database setup for testing.
- **AutoMapper**: Simplifies object-to-object mapping.
- **JWT Authentication**: Implements token-based authentication and role-based access control.
- **Role Management**: Predefined roles and easy-to-extend role-based permissions.
- **Multitenancy**: Baked-in support for multitenancy to manage multiple clients within a single application instance.
- **Cloud Storage**: Integration with MinIO or Azure Blob Storage for file management.
- **Swagger Integration**: Auto-generated API documentation with Swagger UI.
- **Logging**: Integrated logging with Serilog for better observability and diagnostics.

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server or PostgreSQL](https://www.microsoft.com/sql-server) for database (optional)
- [MinIO](https://min.io/) or [Azure Storage](https://azure.microsoft.com/en-us/services/storage/blobs/) for file storage (optional)

### Setup Instructions

1. **Clone the repository:**

    ```bash
    git clone https://github.com/your-username/DotNetApiStarter.git
    cd DotNetApiStarter
    ```

2. **Configure environment variables:**

   - Create a `.env` file in the root directory.
   - Add necessary configurations for the database, JWT, and cloud storage.

3. **Run the application:**

    ```bash
    dotnet run
    ```

4. **Access Swagger UI:**
   
   Navigate to `https://localhost:5001/` to explore the API documentation.

## Project Structure

- **/API**: Contains the main application logic, controllers, and services.
- **/DOMAIN**: Defines the domain entities and core business logic.
- **/INFRASTRUCTURE**: Includes data access and external services integration.
- **/SHARED**: Holds shared utilities, DTOs, and other cross-cutting concerns.
- **/TESTS**: Contains unit and integration tests.

## Docker Setup

For instructions on how to set up and run the application using Docker, please refer to the [Docker Setup Instructions](DOCKER.md).

## Contributing

Contributions are welcome! Please open an issue or submit a pull request.

