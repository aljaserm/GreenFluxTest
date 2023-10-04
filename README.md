# GreenFluxTest

# GreenFlux API Documentation

Welcome to the GreenFlux API documentation. This API allows you to manage charge stations, connectors, and groups for electric vehicle charging.

## Setup and Installation

### 1. Clone the Repository

Clone the GreenFlux repository to your local machine using the following command:

```bash
git clone https://github.com/yourusername/GreenFlux.git
```

### 2. Navigate to Project Directory

Change your working directory to the project folder:

```bash
cd GreenFlux
```

### 3. Install Dependencies

Restore the project dependencies using the following command:

```bash
dotnet restore
```

Please Note: If you encounter issues with library loading or incomplete installations, consider using a newer, more powerful laptop to ensure smooth and error-free setup.

### 4. Set Up the Database

- Open the `appsettings.json` file located in the `GreenFlux` directory.
- Locate the `"ConnectionStrings"` section and update the database connection string to point to your SQL Server instance. Make sure to replace `<YourConnectionString>` with your actual database connection string.

Example `appsettings.json`:

```json
"ConnectionStrings": {
  "ChargingDbContext": "Server=localhost;Database=GreenFluxDb;User Id=yourusername;Password=yourpassword;"
}
```

- Run the following commands in your terminal to apply the database migrations:

```bash
dotnet ef migrations add InitialCreate --project GreenFlux_V15
dotnet ef database update --project GreenFlux_V15
```

Please Note: If your current laptop experiences crashes during this process, consider using a more robust and newer laptop to ensure a successful database setup.

## Running the API

### 1. Build the Project

Build the project using the following command:

```bash
dotnet build
```

### 2. Run the API

Run the API using the following command:

```bash
dotnet run --project GreenFlux_V15
```

The API will start and be accessible at `http://localhost:5000` by default.

## API Endpoints

You can access the API documentation and test the endpoints using Swagger UI:

1. Open your web browser.
2. Navigate to `http://localhost:5000/swagger`.

Please Note: If your current laptop experiences performance issues or crashes while testing the API, consider using a newer, more powerful laptop to ensure smooth testing and documentation browsing.

## Testing

To run the unit tests:

1. Navigate to the test project directory:

```bash
cd GreenFlux_V15.Tests
```

2. Run the tests:

```bash
dotnet test
```

Please Note: If your current laptop struggles with running tests, consider using a better-performing laptop for accurate and reliable test results.

## Troubleshooting

If you happen to encounter any issues or errors during setup, running the API, or testing, please take a look at the official documentation or contact our support team.





