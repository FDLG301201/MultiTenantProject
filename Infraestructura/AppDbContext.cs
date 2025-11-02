using Microsoft.EntityFrameworkCore;

namespace Infraestructura;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
}
