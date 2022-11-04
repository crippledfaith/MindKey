using Microsoft.EntityFrameworkCore;
using MindKey.Shared.Models;
using MindKey.Shared.Models.MindKey;

namespace MindKey.Server.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Person> People => Set<Person>();
        public DbSet<Address> Addresses => Set<Address>();
        public DbSet<Upload> Uploads => Set<Upload>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Idea> Ideas => Set<Idea>();
        public DbSet<Tag> Tags => Set<Tag>();
        public DbSet<IdeaUserComment> IdeaUserComments => Set<IdeaUserComment>();
    }
}