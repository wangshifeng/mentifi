using Hub3c.Mentify.Repository;
using Microsoft.EntityFrameworkCore;

namespace Hub3c.Mentify.ServiceTest
{
    public class InMemoryContext : MentifiContext
    {

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("MentifiTestDb");
        }

        public InMemoryContext(DbContextOptions<MentifiContext> options) : base(options)
        {
        }

        public InMemoryContext() : base(new DbContextOptions<MentifiContext>())
        {
            
        }
    }
}
