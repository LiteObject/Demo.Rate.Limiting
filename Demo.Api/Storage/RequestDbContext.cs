using Demo.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Demo.Api.Storage
{
    public class RequestDbContext : DbContext
    {
        public RequestDbContext(DbContextOptions<RequestDbContext> options) : base(options)
        {
        }

        public DbSet<Request> Requests { get; set; }
    }
}
