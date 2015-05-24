using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Entity;

namespace UnicornStore.AspNet.Models.UnicornStore
{
    public interface IUnicornStoreContext
    {
        DbSet<CartItem> CartItems { get; set; }
        DbSet<Category> Categories { get; set; }
        DbSet<Order> Orders { get; set; }
        DbSet<Product> Products { get; set; }
        DbSet<WebsiteAd> WebsiteAds { get; set; }

        //int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}