using Microsoft.EntityFrameworkCore;
using MindKey.Shared.Data;
using MindKey.Shared.Models.MindKey;
using System.Globalization;

namespace MindKey.Server.Models
{
    public class IdeaRepository : IIdeaRepository
    {
        private readonly AppDbContext _appDbContext;
        public IdeaRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<Idea?> AddIdea(Idea idea)
        {
            idea.Id = new Random().NextInt64();
            var person = await _appDbContext.People.FirstOrDefaultAsync(q => q.PersonId == idea.Person.PersonId);
            if (person != null)
            {
                idea.Person = person;
                //idea.Person.User = await _appDbContext.Users.FirstOrDefaultAsync(q => q.Id == idea.Person.User.Id);
                UpdateTags(idea, null);
                var result = await _appDbContext.Ideas.AddAsync(idea);
                await _appDbContext.SaveChangesAsync();
                return result.Entity;
            }
            throw new Exception($"PersonId is Invalid");
        }
        public async Task<Idea?> UpdateIdea(Idea idea)
        {
            var result = await _appDbContext.Ideas.Include(q => q.Tags).FirstOrDefaultAsync(p => p.Id == idea.Id);
            if (result != null)
            {
                UpdateTags(idea, result);
                _appDbContext.Entry(result).CurrentValues.SetValues(idea);
                await _appDbContext.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException("Idea not found");
            }
            return result;
        }
        private void UpdateTags(Idea idea, Idea? result)
        {
            idea.Tags.RemoveAll(q => q.Name == "");
            foreach (var tag in idea.Tags.ToList())
            {
                tag.Name = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(tag.Name.Trim().ToLower());
                if (result != null)
                {
                    var existingTags = result.Tags
                    .Where(a => a.Name == tag.Name && a.IdeaId == idea.Id)
                    .FirstOrDefault();
                    if (existingTags != null)
                    {
                        tag.IdeaId = idea.Id;
                        tag.Id = existingTags.Id;
                        _appDbContext.Entry(existingTags).CurrentValues.SetValues(tag);
                    }
                    else
                    {
                        var newTag = new Tag()
                        {
                            Name = tag.Name,
                            IdeaId = idea.Id,
                        };
                        result.Tags.Add(newTag);
                    }
                }
            }
            if (result != null)
            {
                result.Tags.RemoveAll(q => !idea.Tags.Any(e => e.Name == q.Name));
            }
        }



        public async Task<Idea?> DeleteIdea(long id)
        {
            var result = await _appDbContext.Ideas.FirstOrDefaultAsync(p => p.Id == id);
            if (result != null)
            {
                result.IsDeleted = true;
                _appDbContext.Update(result);
                await _appDbContext.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException("Idea not found");
            }
            return result;
        }

        public PagedResult<IdeaUserComment> GetComments(int page, int pageSize, long? ideaId)
        {
            return _appDbContext.IdeaUserComments
                .Where(q => q.Idea.Id == ideaId)
                 .Include(q => q.Person)
                 .Include(q => q.Person.User)
                 .Include(q => q.Idea.Person)
                 .Include(q => q.Idea.Person.Addresses)
                 .Include(q => q.Idea.Person.User)
                 .OrderByDescending(p => p.PostDateTime)
                 .GetPaged(page, pageSize);
        }

        public async Task<Idea?> GetIdea(long id)
        {

            var result = await _appDbContext.Ideas
                 .Include(q => q.Tags)
                 .Include(q => q.Person)
                 .Include(q => q.Person.User)
                 .Include(q => q.Person.Addresses)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (result != null)
            {
                return result;
            }
            else
            {
                throw new KeyNotFoundException("Idea not found");
            }
        }

        public PagedResult<Idea>? GetTopIdeas(int page)
        {
            int pageSize = 10;
            return _appDbContext.Ideas
                .Where(q => !q.IsDeleted)
                .Include(q => q.Tags)
                .Include(q => q.Person)
                .Include(q => q.Person.User)
                .Include(q => q.Person.Addresses)
                .OrderByDescending(q => q.AgainstCount + q.NetrulCount + q.ForCount)
                .GetPaged(page, pageSize);
        }
        public PagedResult<Idea>? GetIdeas(int page, long? userId)
        {
            int pageSize = 10;
            if (userId == null)
            {
                return _appDbContext.Ideas
                    .Where(q => !q.IsDeleted)
                    .Include(q => q.Tags)
                    .Include(q => q.Person)
                    .Include(q => q.Person.User)
                    .Include(q => q.Person.Addresses)
                    .OrderByDescending(p => p.PostDateTime)
                    .GetPaged(page, pageSize);
            }
            else
            {
                return _appDbContext.Ideas
                    .Where(p => p.Person.User.Id == userId)
                    .Where(q => !q.IsDeleted)
                    .Include(q => q.Tags)
                    .Include(q => q.Person)
                    .Include(q => q.Person.User)
                    .Include(q => q.Person.Addresses)
                    .OrderByDescending(p => p.PostDateTime)
                    .GetPaged(page, pageSize);
            }
        }

        public PagedResult<Idea>? GetIdeasOfOthers(int page, long? userId)
        {
            int pageSize = 10;
            if (userId == null)
            {
                return _appDbContext.Ideas
                    .Include(q => q.Tags)
                    .Include(q => q.Person)
                    .Include(q => q.Person.User)
                    .Include(q => q.Person.Addresses)
                    .Where(q => !q.IsDeleted && q.IsPublished)
                    .OrderByDescending(p => p.PostDateTime)
                    .GetPaged(page, pageSize);
            }
            else
            {
                return _appDbContext.Ideas
                    .Include(q => q.Person)
                    .Include(q => q.Person.User)
                    .Include(q => q.Person.Addresses)
                    .Where(p => p.Person.User.Id != userId && !p.IsDeleted && p.IsPublished)
                    .OrderByDescending(p => p.PostDateTime)
                    .GetPaged(page, pageSize);
            }
        }

        public async Task<IdeaUserComment?> GetComment(IdeaUserComment ideaUserComment)
        {
            var users = _appDbContext.IdeaUserComments.Include(q => q.Person).ThenInclude(q => q.User).Include(q => q.Idea).Include(q => q.Idea.Person).Include(q => q.Idea.Person.User).Include(q => q.Idea.Person.Addresses);
            var result = await users.FirstOrDefaultAsync(q => q.Person.PersonId == ideaUserComment.Person.PersonId && q.Idea.Id == ideaUserComment.Idea.Id);
            if (result != null)
            {
                result.Person.User.Password = "";
                result.Idea.Person.User.PasswordHash = "";
            }

            return result;
        }

        public async Task<bool?> SetComment(IdeaUserComment ideaUserComment)
        {
            try
            {
                if (ideaUserComment == null)
                    throw new Exception("Idea not given.");

                if (ideaUserComment.Person.PersonId == ideaUserComment.Idea.Person.PersonId)
                {
                    throw new Exception("Posting user and arugment set user can't be same");
                }
                var previous = await _appDbContext.IdeaUserComments.FirstOrDefaultAsync(q => q.Person.PersonId == ideaUserComment.Person.PersonId && q.Idea.Id == ideaUserComment.Idea.Id);
                if (previous != null && previous.Argument != ideaUserComment.Argument)
                {
                    throw new Exception("One Vote Per User");
                }
                var argumentUser = await _appDbContext.People.FirstOrDefaultAsync(p => p.PersonId == ideaUserComment.Person.PersonId);
                var argumentIdea = await GetIdea(ideaUserComment.Idea.Id);
                if (argumentUser == null)
                {
                    throw new KeyNotFoundException("User not found.");
                }
                if (argumentIdea == null)
                {
                    throw new KeyNotFoundException("Idea not found.");
                }



                ideaUserComment.Person = argumentUser;
                ideaUserComment.Idea = argumentIdea;
                if (previous == null)
                {
                    if (ideaUserComment.Argument == ArgumentType.For)
                        argumentIdea.ForCount = argumentIdea.ForCount + 1;
                    if (ideaUserComment.Argument == ArgumentType.Against)
                        argumentIdea.AgainstCount = argumentIdea.AgainstCount + 1;
                    if (ideaUserComment.Argument == ArgumentType.Nutral)
                        argumentIdea.NetrulCount = argumentIdea.NetrulCount + 1;
                    _appDbContext.IdeaUserComments.Add(ideaUserComment);

                }
                else
                    _appDbContext.Entry(previous).CurrentValues.SetValues(ideaUserComment);
                await _appDbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<Dictionary<string, int>> GetTags(int count)
        {
            var list = await _appDbContext.Tags.ToListAsync();
            return list.GroupBy(q => q.Name).OrderByDescending(q => q.Count()).Take(count).ToDictionary(q => q.Key, q => q.Count());
        }

    }
}