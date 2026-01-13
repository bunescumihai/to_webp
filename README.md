# To WebP Project Setup

## Project Structure

This solution contains two projects:

1. **to_webp** - Main ASP.NET Core Razor Pages web application
2. **DbFirst** - Razor Class Library containing database models and context

## Database Connection

The application connects to SQL Server using the following credentials:
- **Server**: localhost
- **Database**: conversions_db_first
- **User**: mihai
- **Password**: admin

Connection string is configured in `appsettings.json`.

## DbFirst Package

The DbFirst package contains:
- Entity models generated from the database (User, Plan, Conversion, Image)
- ConversionsDbContext for database operations
- All entity configurations and relationships

The models were generated using Entity Framework Core scaffold from the existing database.

## Running the Application

1. **Make sure SQL Server is running** with the database `conversions_db_first`
2. **Ensure the user credentials are correct** (mihai:admin)
3. **Press Shift+F10** in JetBrains Rider to run the application
4. **Or use command line**:
   ```bash
   cd to_webp
   dotnet run
   ```
5. Open your browser to the URL shown in the console (typically https://localhost:5001)

## Pages

### Index Page (/)
- Displays "Hello World"
- Shows database connection status
- Lists all users with their plans and roles
- Lists all conversions with user and datetime

The page has no layout/styling - just pure content as requested.

## Working with the Database

### Example: Query Users
```csharp
using DbFirst.Data;
using DbFirst.Models;

public class MyPageModel : PageModel
{
    private readonly ConversionsDbContext _context;
    
    public MyPageModel(ConversionsDbContext context)
    {
        _context = context;
    }
    
    public async Task OnGetAsync()
    {
        var users = await _context.Users
            .Include(u => u.Plan)
            .Include(u => u.Conversions)
            .ToListAsync();
    }
}
```

### Example: Add a Conversion
```csharp
var conversion = new Conversion
{
    UserId = userId,
    ImageIdFrom = fromImageId,
    ImageIdTo = toImageId,
    Datetime = DateTime.Now
};

_context.Conversions.Add(conversion);
await _context.SaveChangesAsync();
```

## Database Schema

### Tables
- **users** - User accounts (id, email, password, planId, role)
- **plans** - Subscription plans (id, name, price, limit)
- **images** - Image metadata (id, md5, path, size, format)
- **conversions** - Conversion history (id, userId, datetime, imageId_from, imageId_to)

### Relationships
- Each User belongs to one Plan
- Each User has many Conversions
- Each Conversion references two Images (from and to)

## Next Steps

To add new functionality:
1. Create new Razor Pages in the `Pages` folder
2. Inject `ConversionsDbContext` in the page model
3. Use LINQ and Entity Framework to query/update data
4. Reference models from `DbFirst.Models` namespace

## Regenerating Models

If the database schema changes:
```bash
cd DbFirst
dotnet ef dbcontext scaffold "Server=localhost;Database=conversions_db_first;User Id=mihai;Password=admin;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer --output-dir Models --context-dir Data --context ConversionsDbContext --force
```

Then update namespaces from `to_webp` to `DbFirst`.

