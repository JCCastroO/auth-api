# Auth.Api

Authentication API built with `.NET 10`, `ASP.NET Core Minimal API`, `PostgreSQL`, and `Redis`.

This repository is a project focused on authentication flows and clean architecture. It demonstrates how to organize an API into clear layers, integrate relational and cache storage, and implement common auth operations such as registration, login, access token issuance, and refresh token rotation.

## Highlights

- Layered architecture with clear separation of responsibilities
- Minimal API implementation with focused endpoint handlers
- Password hashing with `Argon2id`
- User persistence in `PostgreSQL`
- Refresh token storage and rotation with `Redis`
- Lightweight data access with `Dapper`
- Local orchestration with `.NET Aspire`
- Unit and integration tests with `xUnit`, `NSubstitute`, and `Testcontainers`

## Overview

The API exposes three main workflows:

- Register a new user
- Authenticate an existing user
- Refresh an access token using a refresh token

The project is structured to keep transport, business logic, persistence, and shared contracts isolated from each other. That makes the code easier to understand, test, and evolve.

## Architecture

The solution is split into the following projects:

- `Auth.Api.View`
  HTTP layer. Contains application startup, dependency injection, endpoint mapping, and response handling.
- `Auth.Api.Controller`
  Application layer. Contains use cases and services such as registration, login, refresh token flow, password encryption, and token generation.
- `Auth.Api.Model`
  Infrastructure/data layer. Contains entities, repositories, and cache services for `PostgreSQL` and `Redis`.
- `Auth.Api.Share`
  Shared contracts. Contains requests, responses, and DTOs shared across layers.
- `Auth.Api.AppHost`
  Aspire orchestration project used to run the API with its local dependencies.
- `Auth.Api.ServiceDefaults`
  Shared Aspire defaults and related configuration helpers.

## Project Structure

```text
src/
  Auth.Api.AppHost/           Aspire orchestration project
  Auth.Api.Controller/        Use cases and application services
  Auth.Api.Model/             Entities, repositories, and cache services
  Auth.Api.ServiceDefaults/   Shared service defaults
  Auth.Api.Share/             Requests, responses, and DTOs
  Auth.Api.View/              Minimal API entrypoint and endpoints

tests/
  Auth.Api.Controller.Tests/  Unit tests for use cases and services
  Auth.Api.Model.Tests/       Integration tests for repository and cache behavior
  Auth.Api.Share.Tests/       Unit tests for shared contracts
  Auth.Api.View.Tests/        Integration tests for endpoints and DI
```

## Main Features

- User registration with duplicate email check
- Password hashing using `Argon2id`
- Login with credential validation
- Access token generation
- Refresh token generation and rotation
- Refresh token state stored in `Redis`
- PostgreSQL persistence with `Dapper`
- Swagger enabled in development
- Automated unit and integration tests

## Tech Stack

- `.NET 10`
- `ASP.NET Core Minimal API`
- `PostgreSQL`
- `Redis`
- `Dapper`
- `Npgsql`
- `StackExchange.Redis`
- `Newtonsoft.Json`
- `xUnit`
- `NSubstitute`
- `Testcontainers`
- `.NET Aspire`

## Authentication Flow

### Register

The client sends `name`, `email`, and `password`.

The API:

1. Checks whether the email already exists
2. Hashes the password with `Argon2id`
3. Stores the user in `PostgreSQL`

### Login

The client sends `email` and `password`.

The API:

1. Loads the user by email
2. Validates the password against the stored hash
3. Generates an access token
4. Generates a refresh token
5. Stores refresh token data in `Redis`
6. Returns both tokens and expiration metadata

### Refresh Token

The client sends a refresh token.

The API:

1. Reads the refresh token entry from `Redis`
2. Generates a new access token
3. Generates a new refresh token
4. Removes the previous refresh token
5. Stores the new refresh token in `Redis`
6. Returns the new token pair

## Endpoints

### `POST /api/v1/auth/register`

Registers a new user.

Request:

```json
{
  "name": "John Doe",
  "email": "john.doe@email.com",
  "password": "john@123"
}
```

Response:

```http
200 OK
```

### `POST /api/v1/auth/login`

Authenticates a user and returns token data.

Request:

```json
{
  "email": "john.doe@email.com",
  "password": "john@123"
}
```

Response:

```json
{
  "accessToken": "string",
  "refreshToken": "string",
  "expiresOn": "2026-03-18T18:00:00Z",
  "expiresRefreshOn": "2026-03-25T18:00:00Z"
}
```

### `POST /api/v1/auth/refresh_token`

Generates a new access token from a refresh token.

Request:

```json
{
  "refreshToken": "string"
}
```

Response:

```json
{
  "accessToken": "string",
  "refreshToken": "string",
  "expiresOn": "2026-03-18T18:00:00Z",
  "expiresRefreshOn": "2026-03-25T18:00:00Z"
}
```

## Running Locally

### Prerequisites

- `.NET 10 SDK`
- `Docker`

### Recommended: Run with Aspire

From the repository root:

```powershell
dotnet run --project .\src\Auth.Api.AppHost\
```

This starts the API together with its local dependencies, including:

- `PostgreSQL`
- `Redis`

### Run Only the API

If you already have `PostgreSQL` and `Redis` available, you can run only the API project:

```powershell
dotnet run --project .\src\Auth.Api.View\
```

## Configuration

Main configuration file:

- [`appsettings.json`](D:/Devs/Projetos/Auth.Api/src/Auth.Api.View/appsettings.json)

Important settings:

- `ConnectionStrings:Database`
- `AppName`
- `EncryptPasswordService:*`
- `TokenService:AccessToken:*`
- `TokenService:RefreshToken:*`

For real deployments, sensitive values should come from environment variables or a secret manager rather than tracked configuration files.

## Database Initialization

The Aspire host initializes PostgreSQL with a basic `users` table and a seed user for local development.

Initialization script:

- [`init.sql`](D:/Devs/Projetos/Auth.Api/src/Auth.Api.AppHost/Script/init.sql)

## Testing

The solution includes:

- Unit tests for services and use cases
- Integration tests for endpoints
- Integration tests for repository and cache behavior
- Contract tests for shared request and response objects

Run all tests:

```powershell
dotnet test .\Auth.Api.slnx
```

## API Documentation

Swagger is enabled in development. When the API is running locally, use the generated Swagger UI to inspect and test the endpoints.

## Portfolio Notes

This project is meant to showcase backend engineering practices such as:

- separation of concerns
- dependency injection
- auth workflow design
- password security
- integration with infrastructure services
- automated testing

It is a practical example of how to build a small but structured authentication service using modern `.NET` tooling.

## Author

Developed by João Carlos de Castro Oliveira.
