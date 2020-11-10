using Microsoft.EntityFrameworkCore;

namespace WebBrowserDataBase
{
    public class DataContext : DbContext
    {
        string _cnnStr;

        public DataContext(string cnnStr)
        {
            _cnnStr = cnnStr;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_cnnStr);
        }

        public DbSet<Link> Link { get; set; }
        public DbSet<Function> Function { get; set; }
    }
}
