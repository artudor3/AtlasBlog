using AtlasBlog.Data;
using Microsoft.EntityFrameworkCore;

namespace AtlasBlog.Services
{
    public class DataService
    {
        //Calling a method or an instruction that executes the migration
        readonly ApplicationDbContext _dbContext;

        public DataService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task SetupDbAsync()
        {
            //Run the Migrations async
            await _dbContext.Database.MigrateAsync();
        }

        public DateTime GetPostGresDate(DateTime datetime)
        {
            return DateTime.SpecifyKind(datetime, DateTimeKind.Utc);
        }

    }
}
