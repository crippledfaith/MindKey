using Bogus;
using Microsoft.EntityFrameworkCore;
using MindKey.Shared.Models;
using MindKey.Shared.Models.MindKey;
using System.Text;
using Person = MindKey.Shared.Models.Person;

namespace MindKey.Server.Models
{
    public class DataGenerator
    {
        public static void Initialize(IDbContextFactory<AppDbContext> appDbContextFactory)
        {
            List<Idea> ideas = new List<Idea>();
            List<Person> people = new List<Person>();
            Randomizer.Seed = new Random(32321);
            var appDbContext = appDbContextFactory.CreateDbContext();
            if (!appDbContext.Users.Any())
            {
                var testUsers = new Faker<User>()
                    .RuleFor(u => u.FirstName, u => u.Name.FirstName())
                    .RuleFor(u => u.LastName, u => u.Name.LastName())
                    .RuleFor(u => u.Username, u => u.Internet.UserName())
                    .RuleFor(u => u.Password, u => u.Internet.Password());
                var users = testUsers.Generate(4);

                User customUser = new User()
                {
                    FirstName = "Taufiq",
                    LastName = "Rahman",
                    Username = "admin",
                    Password = "admin",
                    UserType = Shared.UserType.Admin,
                };

                users.Add(customUser);

                foreach (User u in users)
                {
                    u.PasswordHash = BCrypt.Net.BCrypt.HashPassword(u.Password);
                    u.Password = "**********";
                    appDbContext.Users.Add(u);
                }
                appDbContext.SaveChanges();
            }
            if (!appDbContext.People.Any())
            {
                //Create test addresses
                var testAddresses = new Faker<Address>()
                    .RuleFor(a => a.Street, f => f.Address.StreetAddress())
                    .RuleFor(a => a.City, f => f.Address.City())
                    .RuleFor(a => a.State, f => f.Address.State())
                    .RuleFor(a => a.ZipCode, f => f.Address.ZipCode());

                // Create new people
                var testPeople = new Faker<Shared.Models.Person>()
                    .RuleFor(p => p.FirstName, f => f.Name.FirstName())
                    .RuleFor(p => p.LastName, f => f.Name.LastName())
                    .RuleFor(p => p.Gender, f => f.PickRandom<Gender>())
                    .RuleFor(p => p.PhoneNumber, f => f.Phone.PhoneNumber())
                    .RuleFor(p => p.Addresses, f => testAddresses.Generate(2).ToList());

                var xpeople = testPeople.Generate(appDbContext.Users.Count());
                int x = 0;
                foreach (Person p in xpeople)
                {
                    p.User = appDbContext.Users.Skip(x++).First();
                    p.FirstName = p.User.FirstName;
                    p.LastName = p.User.LastName;
                    appDbContext.People.Add(p);
                    people.Add(p);
                }
                appDbContext.SaveChanges();
            }
            //if (appDbContext.People.Any() && !appDbContext.Ideas.Any())
            //{
            //    people = appDbContext.People.ToList();
            //    for (int i = 0; i < 100; i++)
            //    {
            //        var idea = new Idea
            //        {
            //            Id = random.NextInt64(),
            //            Argument = ArgumentType.For,
            //            Description = LoremIpsum(100, 200, 1, 2, 2),
            //            Title = LoremIpsum(5, 20, 1, 1, 1),
            //            Person = people[random.Next(people.Count)],
            //            PostDateTime = DateTime.UtcNow,
            //            ForCount =0,// random.Next(1000, 100000),
            //            AgainstCount =0,// random.Next(1000, 100000),
            //            NetrulCount = random.Next(1000, 100000)
            //        };
            //        ideas.Add(idea);
            //        appDbContext.Ideas.Add(idea);
            //    }
            //    appDbContext.SaveChanges();

            //}
            if (!appDbContext.Uploads.Any())
            {
                string jsonRecord = @"[{""FirstName"": ""Tim"",""LastName"": ""Bucktooth"",""Gender"": 1,""PhoneNumber"": ""717-211-3211"",
                    ""Addresses"": [{""Street"": ""415 McKee Place"",""City"": ""Pittsburgh"",""State"": ""Pennsylvania"",""ZipCode"": ""15140""
                    },{ ""Street"": ""315 Gunpowder Road"",""City"": ""Mechanicsburg"",""State"": ""Pennsylvania"",""ZipCode"": ""17101""  }]}]";
                var testUploads = new Faker<Upload>()
                    .RuleFor(u => u.FileName, u => u.Lorem.Word() + ".json")
                    .RuleFor(u => u.UploadTimestamp, u => u.Date.Past(1, DateTime.UtcNow))
                    .RuleFor(u => u.ProcessedTimestamp, u => u.Date.Future(1, DateTime.UtcNow))
                    .RuleFor(u => u.FileContent, Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(jsonRecord)));
                var uploads = testUploads.Generate(8);

                foreach (Upload u in uploads)
                {
                    appDbContext.Uploads.Add(u);
                }
                appDbContext.SaveChanges();
            }


        }
        private static Random random = new Random();

        static string LoremIpsum(int minWords, int maxWords,
          int minSentences, int maxSentences,
          int numParagraphs)
        {

            var words = new[]{"lorem", "ipsum", "dolor", "sit", "amet", "consectetuer",
        "adipiscing", "elit", "sed", "diam", "nonummy", "nibh", "euismod",
        "tincidunt", "ut", "laoreet", "dolore", "magna", "aliquam", "erat"};

            var rand = new Random();
            int numSentences = rand.Next(maxSentences - minSentences)
                + minSentences + 1;
            int numWords = rand.Next(maxWords - minWords) + minWords + 1;

            StringBuilder result = new StringBuilder();

            for (int p = 0; p < numParagraphs; p++)
            {
                for (int s = 0; s < numSentences; s++)
                {
                    for (int w = 0; w < numWords; w++)
                    {
                        if (w > 0) { result.Append(" "); }
                        result.Append(words[rand.Next(words.Length)]);
                    }
                    result.Append(". ");
                }
            }

            return result.ToString();
        }
    }
}