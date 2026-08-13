using ComprobanteRDAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ComprobanteRDAPI.Data
{
    public class AppDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Receipt> Receipts { get; set; }
        public DbSet<Voucher> Vouchers { get; set; }

    }
}
