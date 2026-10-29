# i'Kim Distribution Web Service

## Overview

The **i'Kim Distribution Web Service** is a backend REST API for the i'Kim Distribution platform. It provides backend functionality for distribution-related operations and enables client applications to communicate with the server.

The API supports business operations such as order management, inventory, distributor management, deliveries, payments, and reporting, based on the application's implemented functionality.

The project follows a layered architecture and uses Entity Framework Core for database access. The repository also identifies JWT authentication and role-based authorization.

## Live API

The application is deployed and accessible at the following URL:

**Live API URL:** https://ikim-web-service.shastrasoft.net/

This URL serves as the base URL for the deployed API. The available endpoints depend on the application's configured routes and authentication requirements.

- **Live Environment:** Production
- **Protocol:** HTTPS
- **API Base URL:** `https://ikim-web-service.shastrasoft.net/`

If Swagger/OpenAPI documentation is enabled on the live server, its URL can be added here after verification.

## GitHub Repository

Source code is available at:

[https://github.com/drashti-shastra-soft/iKim-Distribution-Web-Service](https://github.com/drashti-shastra-soft/iKim-Distribution-Web-Service)

## Key Functional Areas

The repository describes the following functional areas:

- **Order Management** — Backend operations related to orders.
- **Inventory Management** — Inventory-related operations.
- **Distributor Management** — Distributor-related operations.
- **Delivery Management** — Delivery-related workflows.
- **Payment Management** — Payment-related operations.
- **Reporting** — Reporting functionality.
- **Authentication and Authorization** — Secure access using the authentication and authorization mechanisms implemented by the application.

The exact capabilities of each module depend on the current source code.

## Technology Stack

The repository describes the following technologies and architectural components:

- C#
- ASP.NET Core Web API
- Entity Framework Core
- JWT authentication
- Role-based authorization
- Layered architecture

Check the project files to confirm the exact .NET target framework and package versions.

## Project Structure

The repository contains the following projects:

```text
iKim-Distribution-Web-Service/
├── IKimWebService/
├── IKimWebService.Domain/
├── IKimWebService.Infrastructure/
├── IKimWebService.Persistence/
├── IKimWebService.Repository/
├── IKimWebService.Service/
├── IKimWebService.sln
├── .gitattributes
└── .gitignore
```

### Project Responsibilities

| Project | Responsibility |
|---|---|
| `IKimWebService` | API host and application entry point |
| `IKimWebService.Domain` | Domain models and business concepts |
| `IKimWebService.Infrastructure` | Infrastructure-related implementations |
| `IKimWebService.Persistence` | Database persistence and related components |
| `IKimWebService.Repository` | Data-access and repository components |
| `IKimWebService.Service` | Service-layer and business logic components |

These responsibilities are based on project names and should be verified against the actual implementation.

## Prerequisites

Before running the project locally, install:

- Git
- Visual Studio with the ASP.NET and web development workload, or a compatible .NET SDK
- The database engine required by the application
- An API testing tool, such as Swagger UI if enabled, or Postman

Use the target framework specified in the `.csproj` files to determine the required .NET SDK version.

## Getting Started

### 1. Clone the Repository

Clone the repository using Git:

```bash
git clone https://github.com/drashti-shastra-soft/iKim-Distribution-Web-Service.git
```

Navigate to the repository directory:

```bash
cd iKim-Distribution-Web-Service
```

### 2. Open the Solution

Open `IKimWebService.sln` in Visual Studio.

Alternatively, restore and build the solution from the repository root:

```bash
dotnet restore IKimWebService.sln
dotnet build IKimWebService.sln
```

Ensure that a compatible .NET SDK is installed before building the project.

### 3. Configure the Application

Inspect the API project's configuration files, including `appsettings.json`, environment-specific configuration files, and `Properties/launchSettings.json` if present.

Configure the values required by the actual application, such as:

- Database connection string
- JWT authentication settings
- Logging configuration
- External service URLs or credentials, if applicable

Use local development settings or environment variables for sensitive values.

**Security:** Never commit production credentials, API keys, passwords, access tokens, or JWT signing secrets to the repository.

### 4. Run the API Locally

In Visual Studio:

1. Set `IKimWebService` as the startup project.
2. Select the appropriate development launch profile.
3. Run the application.

Alternatively, use the .NET CLI:

```bash
dotnet run --project IKimWebService/IKimWebService.csproj
```

This command assumes the project file is located at the specified path. Adjust it if the actual repository structure differs.

The local URL and port depend on the project's launch settings and runtime configuration.

## API Documentation and Testing

If Swagger/OpenAPI is enabled, run the application and open the Swagger URL specified by the launch settings or application output.

You can use Swagger, Postman, or another HTTP client to test the API endpoints.

For each endpoint, verify the following details from the source code:

- HTTP method and route
- Request parameters and body
- Response format
- Authentication requirements
- Required roles or permissions
- Validation and error responses

Only document endpoints that exist in the actual implementation.

## Authentication and Authorization

The repository description identifies JWT authentication and role-based access control.

The precise authentication routes, token claims, role names, and authorization policies should be confirmed from the application's authentication configuration and source code.

For local testing:

1. Identify the authentication endpoint implemented by the API.
2. Use valid development credentials.
3. Obtain an authentication token through the configured authentication flow.
4. Include the token in requests to protected endpoints using the authentication scheme expected by the application.

Never publish production credentials or access tokens in the README.

## Database Integration

The application uses Entity Framework Core according to the repository description.

Before connecting to a local database:

1. Identify the configured database provider.
2. Configure the required connection string.
3. Ensure the database server is running and accessible.
4. Review the existing migrations and database initialization process.
5. Apply database changes using the procedure appropriate for this project.

Do not assume a database provider or migration command without checking the project files.

## Error Handling and Logging

Review the application's middleware, filters, controllers, and service implementations to understand how exceptions, validation errors, and logs are handled.

When extending the API, follow the existing error-handling and logging conventions to maintain consistency.

## Troubleshooting

| Issue | What to Check |
|---|---|
| .NET SDK or framework error | Verify the
