# Quick Start Guide - RBAC System API

This guide will help you get the RBAC System API up and running in less than 10 minutes.

## Prerequisites Installation

### 1. Install .NET 8 SDK
Download and install from: https://dotnet.microsoft.com/download/dotnet/8.0

Verify installation:
```bash
dotnet --version
```

### 2. Setup Database and Cache with Docker (Easiest Method)

If you have Docker installed, simply run:
```bash
docker-compose up -d
```

This will start:
- PostgreSQL on port 5432
- Redis on port 6379
- pgAdmin on port 5050 (optional, for database management)
- Redis Commander on port 8081 (optional, for Redis management)

**Database Credentials:**
- Host: localhost
- Port: 5432
- Database: rbacdb
- Username: postgres
- Password: postgres123

**Redis Credentials:**
- Host: localhost
- Port: 6379
- Password: redis123

### Alternative: Manual Installation

#### PostgreSQL
- Download: https://www.postgresql.org/download/
- Create database: `CREATE DATABASE rbacdb;`

#### Redis
- Windows: https://github.com/microsoftarchive/redis/releases
- Mac: `brew install redis`
- Linux: `sudo apt-get install redis-server`

## Step-by-Step Setup

### Step 1: Clone or Extract the Project
```bash
cd RbacSystem
```

### Step 2: Configure Connection Strings

Copy the example development settings:
```bash
cp appsettings.Development.json.example appsettings.Development.json
```

Edit `appsettings.Development.json` if needed to match your database/Redis setup.

### Step 3: Create Database Schema

Run the provided SQL schema against your PostgreSQL database:
```bash
psql -U postgres -d rbacdb -f schema.sql
```

Or use the SQL directly in pgAdmin or any PostgreSQL client.

### Step 4: Restore NuGet Packages
```bash
dotnet restore
```

### Step 5: Build the Project
```bash
dotnet build
```

### Step 6: Run the Application
```bash
dotnet run
```

The API will start at: `https://localhost:5001`

### Step 7: Access Swagger UI

Open your browser and navigate to:
```
https://localhost:5001
```

You'll see the complete API documentation with all endpoints.

## Testing the API

### Option 1: Using Swagger UI
1. Go to https://localhost:5001
2. Click "Try it out" on any endpoint
3. Fill in the request body
4. Click "Execute"

### Option 2: Using Postman
1. Import the `RBAC-API-Postman-Collection.json` file into Postman
2. Update the `baseUrl` variable to `https://localhost:5001/api`
3. Start making requests

### Option 3: Using cURL

Create a user:
```bash
curl -X POST https://localhost:5001/api/users \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -d '{
    "username": "testuser",
    "email": "test@example.com",
    "isActive": true
  }' \
  -k
```

Get all users:
```bash
curl -X GET "https://localhost:5001/api/users?pageNumber=1&pageSize=10" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -k
```

## Optional: Seed Sample Data

To populate the database with sample data, you can:

1. Add the seeder to your `Program.cs`:
```csharp
// After app.Build() and before app.Run()
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<RbacDbContext>();
    await DataSeeder.SeedDataAsync(context);
}
```

2. Run the application, and sample data will be created automatically.

## Sample Data Includes:
- 5 Users (admin, john.doe, jane.smith, bob.wilson, alice.johnson)
- 4 Groups (Engineering, Sales, HR, Management)
- 5 Roles (Super Admin, Admin, Manager, User, Viewer)
- 15 Permissions (user.*, group.*, role.*, permission.*, report.*, settings.*)

## Authentication

For development, you'll need to generate JWT tokens. You can either:

1. Create an authentication endpoint (not included in basic version)
2. Use the JWT service directly in code for testing
3. Generate tokens using online tools like https://jwt.io

Sample JWT payload:
```json
{
  "sub": "1",
  "name": "admin",
  "permission": ["user.read", "user.write", "user.delete"],
  "jti": "random-guid",
  "exp": 1234567890
}
```

## Common Issues & Solutions

### Issue: Cannot connect to PostgreSQL
**Solution:** Make sure PostgreSQL is running:
```bash
# Check if running
docker ps | grep postgres
# Or on native install
pg_isready
```

### Issue: Cannot connect to Redis
**Solution:** Make sure Redis is running:
```bash
# Check if running
docker ps | grep redis
# Or on native install
redis-cli ping
```

### Issue: Port already in use
**Solution:** Change the port in `Properties/launchSettings.json` or kill the process using the port.

### Issue: SSL/HTTPS errors
**Solution:** Trust the development certificate:
```bash
dotnet dev-certs https --trust
```

## Next Steps

1. **Add Authentication Endpoint**: Create a login endpoint that generates JWT tokens
2. **Customize Permissions**: Add your own permissions relevant to your application
3. **Add Business Logic**: Extend services with your specific business rules
4. **Deploy**: Configure for production with proper secrets management

## Project Structure Reference

```
RbacSystem/
??? Controllers/           # API endpoints
??? Services/             # Business logic
??? Data/                 # Database context & entities
??? Models/               # DTOs and entities
??? Authentication/       # JWT handling
??? Middleware/          # Custom middleware
??? Validators/          # FluentValidation rules
??? Mappings/            # AutoMapper profiles
??? appsettings.json     # Configuration
```

## Useful Commands

```bash
# Build
dotnet build

# Run
dotnet run

# Run with hot reload
dotnet watch run

# Clean
dotnet clean

# Restore packages
dotnet restore

# Create migration (if using EF migrations)
dotnet ef migrations add MigrationName

# Update database (if using EF migrations)
dotnet ef database update
```

## Support

- Check the main README.md for detailed documentation
- Review the Postman collection for API examples
- Consult Swagger UI for endpoint details

## Production Checklist

Before deploying to production:

- [ ] Change JWT secret to a strong random value
- [ ] Enable HTTPS only
- [ ] Configure proper CORS policy
- [ ] Set up proper logging (file rotation, monitoring)
- [ ] Use strong database passwords
- [ ] Enable Redis authentication
- [ ] Set up environment-specific configuration
- [ ] Configure proper error handling
- [ ] Add rate limiting
- [ ] Set up health checks
- [ ] Configure backup strategy

---

**Congratulations!** You now have a fully functional RBAC system API. Happy coding! ??