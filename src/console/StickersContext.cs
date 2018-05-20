using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace console.SqlServer
{
    public class StickersContext : DbContext
    {
        public DbSet<Sticker> Stickers { get; set; }
        public string ConnectionString { get; set; }

        public StickersContext(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder){
            optionsBuilder.UseSqlServer(ConnectionString);
        }
    }
}