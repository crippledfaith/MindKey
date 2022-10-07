using Microsoft.EntityFrameworkCore;
using MindKey.Shared.Data;
using MindKey.Shared.Models.MindKey;

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
            idea.Person = await _appDbContext.People.FirstOrDefaultAsync(q => q.PersonId == idea.Person.PersonId);
            idea.Person.User = await _appDbContext.Users.FirstOrDefaultAsync(q => q.Id == idea.Person.User.Id);
            var result = await _appDbContext.Ideas.AddAsync(idea);
            await _appDbContext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<Idea?> DeleteIdea(long id)
        {
            var result = await _appDbContext.Ideas.FirstOrDefaultAsync(p => p.Id == id);
            if (result != null)
            {
                _appDbContext.Ideas.Remove(result);
                await _appDbContext.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException("Idea not found");
            }
            return result;
        }

        public async Task<Idea?> GetIdea(long id)
        {

            var result = await _appDbContext.Ideas
                 .Include(q => q.Person)
                 .Include(q => q.Person.Addresses)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (result != null)
            {
                return result;
            }
            else
            {
                throw new KeyNotFoundException("Person not found");
            }
        }

        public PagedResult<Idea>? GetIdeas(int page, long? userId)
        {
            int pageSize = 10;
            if (userId == null)
            {
                return _appDbContext.Ideas
                    .Include(q => q.Person)
                    .Include(q => q.Person.Addresses)
                    .OrderByDescending(p => p.PostDateTime)
                    .GetPaged(page, pageSize);
            }
            else
            {
                return _appDbContext.Ideas
                    .Include(q => q.Person)
                    .Include(q => q.Person.Addresses)
                    .Where(p => p.Person.PersonId == userId)
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
                    .Include("Person")
                    .OrderBy(p => p.PostDateTime)
                    .GetPaged(page, pageSize);
            }
            else
            {
                return _appDbContext.Ideas
                    .Include("Person")
                    .Where(p => p.Person.PersonId != userId)
                    .OrderBy(p => p.PostDateTime)
                    .GetPaged(page, pageSize);
            }
        }

        public async Task<Idea?> UpdateIdea(Idea idea)
        {
            var result = await _appDbContext.Ideas.FirstOrDefaultAsync(p => p.Id == idea.Id);
            if (result != null)
            {
                // Update existing person
                _appDbContext.Entry(result).CurrentValues.SetValues(idea);
                await _appDbContext.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException("Idea not found");
            }
            return result;
        }
    }
}