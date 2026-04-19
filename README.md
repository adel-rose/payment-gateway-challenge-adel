# Payment Gateway

A .NET 9 API that acts as a payment gateway, allowing merchants to process card payments and retrieve payment details via an issuing bank simulator.

---

## Prerequisites

Make sure the following are installed on your machine before getting started:

- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- [Rider](https://www.jetbrains.com/rider/) or [Visual Studio](https://visualstudio.microsoft.com/) (optional, for local development)

---

## Getting Started

### 1. Clone the repository

```bash
git clone <your-repo-url>
cd payment-gateway-challenge-v2
```

### 2. Start all services

```bash
docker-compose up
```

This will spin up the following containers:

| Container | Description | Port |
|---|---|---|
| `payment-gateway-database-mssql` | SQL Server database | `1433` |
| `payment-gateway-database-init` | Runs DB creation and seed scripts on startup | — |
| `payment-gateway-api` | The payment gateway API | `5001` |
| `bank_simulator` | Mountebank-based acquiring bank simulator | `8080` |
| `payment-gateway-seq` | Seq log viewer | `5341` |

### 3. Verify everything is running

Hit the health check endpoint:

```
GET http://localhost:5001/health
```

Expected response:

```
Healthy
```

If the response is `Healthy`, the API is up and connected to the database successfully.

---

## API Endpoints

### Process a Payment

```
POST http://localhost:5001/api/paymentgateway/processpayment
```

**Request body:**

```json
{
  "cardNumber": "2222405343248877",
  "expiryMonth": 12,
  "expiryYear": 2028,
  "currency": "GBP",
  "amount": 30000,
  "cvv": "123"
}
```

**Validation rules:**

| Field | Rules |
|---|---|
| `cardNumber` | Required, 14–19 numeric digits |
| `expiryMonth` | Required, 1–12 |
| `expiryYear` | Required, future date (combined with month) |
| `currency` | Required, must be one of: `GBP`, `USD`, `EUR` |
| `amount` | Required, integer in minor currency units (e.g. `1050` = £10.50) |
| `cvv` | Required, 3–4 numeric digits |

**Response:**

```json
{
  "paymentRequestId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "status": "Authorized",
  "lastFourDigits": "8877",
  "expiryMonth": 12,
  "expiryYear": 2028,
  "currency": "GBP",
  "amount": 30000
}
```

Status will be one of: `Authorized`, `Declined`, or `Rejected`.

---

### Retrieve a Payment

```
GET http://localhost:5001/api/paymentgateway/payment/{id}
```

Returns the details of a previously processed payment by its ID.

**Response:**

```json
{
  "paymentRequestId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "status": "Authorized",
  "lastFourDigits": "8877",
  "expiryMonth": 12,
  "expiryYear": 2028,
  "currency": "GBP",
  "amount": 30000
}
```

---

## Bank Simulator Behaviour

The acquiring bank simulator responds based on the last digit of the card number:

| Last digit | Behaviour |
|---|---|
| Odd (1, 3, 5, 7, 9) | `Authorized` — returns a random `authorization_code` |
| Even (2, 4, 6, 8) | `Declined` — no authorization code |
| Zero (0) | `503 Service Unavailable` — gateway returns `502` |

---

## Running the App Locally (Outside Docker)

If you want to run and debug the API through Rider without the full Docker stack:

### 1. Start only the required containers

```bash
docker-compose up payment-gateway-database-mssql payment-gateway-database-init bank_simulator payment-gateway-seq
```

### 2. Run the API through Rider

Open the solution and run the `PaymentGateway.Api` project using the `http` or `https` launch profile. The `appsettings.Development.json` is already configured to point to `localhost` for the database and bank simulator.

### 3. Observability

Once the Seq container is running, structured logs are available at:

```
http://localhost:5341
```

Logs are written to both the console and Seq. If Seq is not running, the console sink continues to work — nothing breaks.

---

## Running the Tests

### Unit Tests

```bash
dotnet test test/PaymentGateway.UnitTests
```

No external dependencies required.

### Integration Tests

Integration tests spin up the API in-memory using `WebApplicationFactory` but still require the SQL Server container to be running.

```bash
docker-compose up payment-gateway-database-mssql payment-gateway-database-init
dotnet test test/PaymentGateway.IntegrationTests
```

### End-to-End Tests (Postman / Newman)

The e2e tests use Newman (Postman's CLI runner) against the full running stack.

**1. Install Newman:**

```bash
npm install -g newman
```

**2. Start all services:**

```bash
docker-compose up
```

**3. Run the collection:**

```bash
newman run test/PaymentGateway.EndToEndTests/newman/collection.json \
  -e test/PaymentGateway.EndToEndTests/newman/env.local.json
```

---

## Project Structure

```
src/
  PaymentGateway.Api/             # Controllers, middleware, health checks, program entry point
  PaymentGateway.Application/     # Services, validators, DTOs, interfaces
  PaymentGateway.Domain/          # Domain models and enums
  PaymentGateway.Infrastructure/  # Dapper repositories, DB connection

test/
  PaymentGateway.UnitTests/       # Unit tests (validators, services, repositories)
  PaymentGateway.IntegrationTests/# Integration tests (health check, payment flows)
  PaymentGateway.EndToEndTests/   # Newman/Postman collection
```

---

## Design Decisions & Assumptions

### Card masking
Card numbers are masked before being stored in the database. Only the last four digits are preserved (e.g. `2222405343248877` → `************8877`). This is handled at the gateway layer before persistence.

### CVV storage
CVV is stored in the database for this exercise only. In a real production implementation this would violate PCI-DSS compliance and should never be persisted.

### Currency support
Only three currencies are supported: `GBP`, `USD`, `EUR`. Requests with any other currency are rejected.

### Payment status mapping
The `Status` column in the `Payments` table is a `TINYINT` mapped as follows:

| Value | Meaning |
|---|---|
| `0` | Authorized |
| `1` | Declined |

### Health checks
The `/health` endpoint runs all registered health checks and returns `Healthy` or `Unhealthy` as plain text. Currently one check is registered: `MsSqlConnectivityHealthcheck`, which executes `SELECT 1` against the database to confirm connectivity. Failures are logged to Serilog/Seq.

The bank simulator is not health checked — it has no dedicated status endpoint and firing a real payment request as a health check would have unintended side effects.

### Bank simulator unavailability
If the bank simulator returns a `503`, the gateway returns a `502 Bad Gateway` with an error message. No payment is stored in this case.

### No authentication
The API has no authentication or merchant identification in this initial phase. `UseAuthentication()` and `UseAuthorization()` are registered in the pipeline but no scheme is configured — this is intentional scaffolding for a future implementation.
