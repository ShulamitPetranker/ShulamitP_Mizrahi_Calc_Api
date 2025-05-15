# ShulamitP_Mizrahi_Calc_Api - ASP.NET Core 8.0 Server

# Mizrahi Arithmetic API

A REST API implemented in ASP.NET Core 8 that performs basic arithmetic operations between two numbers. Operation type is specified via a custom HTTP header. The API is secured with JWT (Bearer tokens) and documented using Swagger/OpenAPI.

---
## Run

Linux/OS X:

```
sh build.sh
```

Windows:

```
build.bat
```

## Features

- Arithmetic operations: Add, Subtract, Multiply, Divide
- Operation defined via HTTP header: `X-Operation`
- JWT-based authentication (Bearer token)
- Full OpenAPI (Swagger) specification
- Dockerized for easy deployment
- Unit tests included

---

##  Technologies

- .NET 8 (ASP.NET Core)
- SwaggerHub & OpenAPI 3.0
- JWT Authentication
- Docker + Docker Compose
- xUnit
- Postman for testing requests

---

## Running with Docker

### Prerequisites

- Docker
- Docker Compose

- ## Run in Docker

```
cd src/ShulamitP_Mizrahi_Calc_Api
docker build -t shulamitp_mizrahi_calc_api .
docker run -p 5000:8080 shulamitp_mizrahi_calc_api
```

### Build the container

```bash
docker-compose build
```

### Run the container

```bash
docker-compose up
```

### Stop the container

```bash
docker-compose down
```

## Running Tests and the API with Docker Compose

You can run everything — both tests and the API — in a single command using Docker Compose.

---

### Option 1: Run tests and keep the API running (recommended)

This command builds all services, runs the unit tests, and keeps the API container up and running:

```bash
docker-compose up --build
```

###  Option 2: Run the tests (and exit afterward)

```bash
docker-compose up --build --abort-on-container-exit
```


Visit Swagger UI at:

```
http://localhost:5000/swagger
```

---

## Authentication (JWT)

### Get Token - (Get Token (for testing purposes))

```http
GET /api/auth/login
```

Sample response:

```Json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR..." 
}
```

### Use Token

Send in the request header:

```
Authorization: Bearer <token>
```

---

## API Call: Arithmetic Operation

```http
POST /api/arithmetic
```

### Headers:

- `Authorization: Bearer <token>`
- `X-Operation: Add` _(or Subtract, Multiply, Divide)_

### Body (JSON):

```json
{
  "number1": 10,
  "number2": 5
}
```

### Response:

```json
{
  "result": 15
}
```

---

## API Documentation

Based on OpenAPI 3.0, generated using SwaggerHub.

### Endpoints

- `POST /api/arithmetic` — perform arithmetic operation
- `GET /api/auth/login` — obtain JWT token

### Schemas

- `CalculationRequest`, `CalculationResponse`
- `LoginResponse`
- `OperationType`: Add, Subtract, Multiply, Divide

---

## Run Unit Tests

```bash
dotnet test
```

Ensure a test project exists inside the `Tests/` or similar directory.

---

## ?? Project Structure

```
.
??? Controllers/
??? DTOs/
??? Services/
??? Models/
??? Tests/
??? Dockerfile
??? docker-compose.yml
??? openapi.yaml
??? README.md
```

---

### Error Handling

- The API includes centralized error handling to ensure stability and clarity.
- Common input validation errors, such as missing values or invalid operation types, are properly validated and result in descriptive exceptions.
- Division by zero is explicitly checked and handled, throwing a clear error before attempting the operation.
- All exceptions are logged using `ILogger<T>` with appropriate severity (`Information`, `Warning`, `Error`).
- Exceptions are propagated properly to ensure the caller receives an appropriate HTTP error response (e.g., `400 Bad Request` for invalid input).

- 
## Notes

- JWT token includes expiration claim (`exp`)
- Unauthorized calls return HTTP 401

---

## Credits

Developed by Shulamit Petranker for Matrix/BM technical assignment.
