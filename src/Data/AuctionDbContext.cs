using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuctionService.Data
{
    public class AuctionDbContext : DbContext
    {
        public AuctionDbContext(DbContextOptions options) 
        {
            this.Options = options;
   
        }
        public DbSet<Auction> Auctions { get; set; }
    }
}