using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
// to have models defined, we need to use models like this:
using HobbyMood.Models;

namespace HobbyMood.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // creating a Hobbies table from the model "Hobby" - singular hobby referring to one instance of that model
        public DbSet<Hobby> Hobbies { get; set; }

        // creating an Experiences table from the model "Experience" - table is plural in this case, and singualr experience refers to a singular instance of an experience

        public DbSet<Experience> Experiences { get; set; }

        //creating a Moods table from the model "Mood" - table is plural in this case, and singular mood refers to a singular description of a mood
        public DbSet<Mood> Moods { get; set; }

        // creating an ExperienceMoods table from the model "ExperienceMood" - table is plural in this case, and singular experienceMood refers to a singular instance of an mood felt during an experience
        public DbSet<ExperienceMood> ExperienceMoods { get; set; }

    }
}
