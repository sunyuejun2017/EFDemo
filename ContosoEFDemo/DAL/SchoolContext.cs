using ContosoEFDemo.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace ContosoEFDemo.DAL
{
    public class SchoolContext: DbContext
    {
        public SchoolContext() : base("SchoolContext")
        {
            Database.SetInitializer<SchoolContext>(null);
        }

        // 通过DbSet来创建数据库中的Table
        public DbSet<Student> Students { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Course> Courses { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}