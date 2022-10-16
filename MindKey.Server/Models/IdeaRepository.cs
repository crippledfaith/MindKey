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

        public PagedResult<IdeaUserComment> GetComments(int page, int pageSize, long? ideaId)
        {
            return _appDbContext.IdeaUserComments
                .Where(q => q.Idea.Id == ideaId)
                 .Include(q => q.User)
                 .Include(q => q.Idea.Person)
                 .Include(q => q.Idea.Person.User)
                 .OrderByDescending(p => p.PostDateTime)
                 .GetPaged(page, pageSize);
        }

        public async Task<Idea?> GetIdea(long id)
        {

            var result = await _appDbContext.Ideas
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
                    .Include(q => q.Person.User)
                    .Include(q => q.Person.Addresses)
                    .OrderByDescending(p => p.PostDateTime)
                    .GetPaged(page, pageSize);
            }
            else
            {
                return _appDbContext.Ideas
                    .Include(q => q.Person)
                    .Include(q => q.Person.User)
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
                    .Include(q => q.Person)
                    .Include(q => q.Person.User)
                    .Include(q => q.Person.Addresses)
                    .OrderByDescending(p => p.PostDateTime)
                    .GetPaged(page, pageSize);
            }
            else
            {
                return _appDbContext.Ideas
                    .Include(q => q.Person)
                    .Include(q => q.Person.User)
                    .Include(q => q.Person.Addresses)
                    .Where(p => p.Person.PersonId != userId)
                    .OrderByDescending(p => p.PostDateTime)
                    .GetPaged(page, pageSize);
            }
        }

        public async Task<IdeaUserComment?> GetSetAgument(IdeaUserComment ideaUserComment)
        {
            var result = await _appDbContext.IdeaUserComments.FirstOrDefaultAsync(q => q.User.Id == ideaUserComment.User.Id && q.Idea.Id == ideaUserComment.Idea.Id);
            return result;
        }

        public async Task<bool?> SetArgument(IdeaUserComment ideaUserComment)
        {
            try
            {
                if (ideaUserComment == null)
                    throw new Exception("Idea not given.");

                if (ideaUserComment.User.Id == ideaUserComment.Idea.Person.User.Id)
                {
                    throw new Exception("Posting user and arugment set user can't be same");
                }
                if (_appDbContext.IdeaUserComments.Any(q => q.User.Id == ideaUserComment.User.Id && q.Idea.Id == ideaUserComment.Idea.Id))
                {
                    throw new Exception("One Vote Per User");
                }
                var argumentUser = _appDbContext.Users.FirstOrDefault(p => p.Id == ideaUserComment.User.Id);
                var argumentIdea = await GetIdea(ideaUserComment.Idea.Id);
                if (argumentUser == null)
                {
                    throw new KeyNotFoundException("User not found.");
                }
                if (argumentIdea == null)
                {
                    throw new KeyNotFoundException("Idea not found.");
                }

                if (ideaUserComment.Argument == ArgumentType.For)
                    argumentIdea.ForCount = argumentIdea.ForCount + 1;
                if (ideaUserComment.Argument == ArgumentType.Against)
                    argumentIdea.AgainstCount = argumentIdea.AgainstCount + 1;
                if (ideaUserComment.Argument == ArgumentType.Nutral)
                    argumentIdea.NetrulCount = argumentIdea.NetrulCount + 1;

                ideaUserComment.User = argumentUser;
                ideaUserComment.Idea = argumentIdea;
                _appDbContext.IdeaUserComments.Add(ideaUserComment);
                _appDbContext.Entry(argumentUser).CurrentValues.SetValues(argumentUser);
                await _appDbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
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