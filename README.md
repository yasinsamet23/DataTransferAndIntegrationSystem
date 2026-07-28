# Installation Guide

## Prerequisites

Install the following software before running the project.

- .NET 8 SDK
- Node.js (LTS)
- Git
- PostgreSQL 16+ **or** Docker Desktop

---

# 1. Clone the Repository

```bash
git clone https://github.com/yasinsamet23/DataTransferAndIntegrationSystem.git

cd DataTransferAndIntegrationSystem
```

---

# 2. Database Setup

Choose **one** of the following options.

---

## Option A - Local PostgreSQL

### Create a PostgreSQL User

```sql
CREATE USER dtis_user
WITH PASSWORD 'GucluBirParola!';
```

### Create the Database

```sql
CREATE DATABASE dtis_db
OWNER dtis_user;
```

Connection String

```text
Host=localhost;
Port=5432;
Database=dtis_db;
Username=dtis_user;
Password=GucluBirParola!
```

---

## Option B - PostgreSQL with Docker

### Linux / macOS

```bash
docker run --name dtis-postgres \
-e POSTGRES_DB=dtis_db \
-e POSTGRES_USER=dtis_user \
-e POSTGRES_PASSWORD=GucluBirParola! \
-p 5432:5432 \
-d postgres:16
```

### Windows Command Prompt (CMD)

```cmd
docker run --name dtis-postgres ^
-e POSTGRES_DB=dtis_db ^
-e POSTGRES_USER=dtis_user ^
-e POSTGRES_PASSWORD=GucluBirParola! ^
-p 5432:5432 ^
-d postgres:16
```

### Windows PowerShell

```powershell
docker run --name dtis-postgres `
-e POSTGRES_DB=dtis_db `
-e POSTGRES_USER=dtis_user `
-e POSTGRES_PASSWORD=GucluBirParola! `
-p 5432:5432 `
-d postgres:16
```

Verify that the container is running.

```bash
docker ps
```

Stop the container.

```bash
docker stop dtis-postgres
```

Start the container again.

```bash
docker start dtis-postgres
```

Remove the container.

```bash
docker rm -f dtis-postgres
```

Connection String

```text
Host=localhost;
Port=5432;
Database=dtis_db;
Username=dtis_user;
Password=GucluBirParola!
```

---

# 3. Configure User Secrets

Navigate to the API project.

```bash
cd DataTransferAndIntegrationSystem.API
```

Initialize User Secrets (only if the project has not already been initialized).

```bash
dotnet user-secrets init
```

> If the project already contains a `UserSecretsId`, this command can be skipped.

Set the database connection string.

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=dtis_db;Username=dtis_user;Password=GucluBirParola!"
```

Set the JWT secret key.

```bash
dotnet user-secrets set "Jwt:Key" "YOUR_SECRET_KEY"
```

Set the Mockaroo API key.

```bash
dotnet user-secrets set "Mockaroo:ApiKey" "YOUR_API_KEY"
```

Verify the configured secrets.

```bash
dotnet user-secrets list
```

---

# 4. Restore Dependencies

From the solution directory, restore all NuGet packages.

```bash
dotnet restore
```

---

# 5. Apply Database Migrations

Run the following command from the solution directory.

```bash
dotnet ef database update --project .\DataTransferAndIntegrationSystem.Persistence --startup-project .\DataTransferAndIntegrationSystem.API
```

---

# 6. Run the Backend

```bash
dotnet run --project .\DataTransferAndIntegrationSystem.API
```

After the API starts:

Swagger

```
http://localhost:5207/swagger
```

Hangfire Dashboard

```
http://localhost:5207/hangfire
```

API Base URL

```
http://localhost:5207/api
```

---

# 7. Run the Frontend

Open a new terminal.

```bash
cd frontend

npm install

npm run dev
```

Frontend URL

```
http://localhost:5173
```

---

# 8. Login Credentials

Administrator

```text
Username: admin
Password: 123456
```

Standard User

```text
Username: user
Password: 123456
```

---

# 9. Run Unit Tests

From the solution directory:

```bash
dotnet test
```

---

# Notes

- Choose **either Local PostgreSQL or Docker PostgreSQL**.
- PostgreSQL must be running before starting the application.
- The connection string in User Secrets must match the database configuration.
- Replace `YOUR_SECRET_KEY` with your own JWT secret.
- Replace `YOUR_API_KEY` with your own Mockaroo API key.
- Do not store sensitive information in `appsettings.json`.
- User Secrets should only be used in development environments.