using Bogus;
using Bogus.DataSets;
using Microsoft.EntityFrameworkCore;
using MindKey.Shared.Data;
using MindKey.Shared.Models;

namespace MindKey.Server.Models
{
    public class ChatLineRepository : IChatLineRepository
    {
        private readonly AppDbContext _appDbContext;
        public ChatLineRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<ChatLine> Add(ChatLine chatLine)
        {
            var user = await _appDbContext.Users.FirstOrDefaultAsync(q => q.Id == chatLine.User.Id);
            if(user==null)
            {
                throw new KeyNotFoundException("User not found");
            }
            chatLine.User = user;
            var idea = await _appDbContext.Ideas.FirstOrDefaultAsync(q=>q.Id == chatLine.Idea.Id);
            if (user == null)
            {
                throw new KeyNotFoundException("Idea not found");
            }
            chatLine.Idea = idea;
            var result = await _appDbContext.ChatLines.AddAsync(chatLine);
            chatLine.DateTime = DateTime.UtcNow;
            await _appDbContext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<ChatLine> Delete(ChatLine chatLine)
        {
            return await Delete(chatLine.Id);
        }

        public async Task<ChatLine> Delete(long id)
        {
            var result = await _appDbContext.ChatLines.FirstOrDefaultAsync(p => p.Id == id);
            if (result != null)
            {
                _appDbContext.ChatLines.Remove(result);
                await _appDbContext.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException("ChatLine not found");
            }
            return result;
        }

        public async Task<ChatLine?> Get(long id)
        {
            var result = await _appDbContext.ChatLines
                .Include(p => p.Idea)
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (result != null)
            {
                return result;
            }
            else
            {
                throw new KeyNotFoundException("ChatLine not found");
            }
        }

        public async Task<IEnumerable<ChatLine>?> GetByIdea(long ideaId)
        {
            var result = await _appDbContext.ChatLines
                .Include(p => p.Idea)
                .Include(p => p.User)
                .OrderBy(q=>q.DateTime)
                .Where(p => p.Idea.Id == ideaId).ToListAsync();
            
            if (result != null)
            {
                return result;
            }
            else
            {
                throw new KeyNotFoundException("ChatLine not found");
            }
        }

        public PagedResult<ChatLine> GetPaged(int page)
        {
            int pageSize = 10;

            return _appDbContext.ChatLines
                .OrderBy(p => p.DateTime)
                .Include(p => p.Idea)
                .Include(p => p.User)
                .GetPaged(page, pageSize);

        }

        public PagedResult<ChatLine> GetPagedByIdea(int page, long ideaId)
        {
            int pageSize = 10;

            return _appDbContext.ChatLines
                .OrderBy(p => p.DateTime)
                .Include(p => p.Idea)
                .Include(p => p.User)
                .Where(p=>p.Idea.Id == ideaId)
                .GetPaged(page, pageSize);
        }

        public  async Task<ChatLine> Update(ChatLine chatLine)
        {
            var user = await _appDbContext.Users.FirstOrDefaultAsync(q => q.Id == chatLine.User.Id);
            if(user == null) 
            {
                throw new Exception($"User is Invalid");
            }
            var idea = await _appDbContext.Ideas.FirstOrDefaultAsync(q=>q.Id == chatLine.Idea.Id);
            if (idea == null)
            {
                throw new Exception($"Idea is Invalid");
            }
            chatLine.User = user;
            chatLine.Idea = idea;
            var existingchatLine = await _appDbContext.ChatLines
                                 .Where(a => a.Id == chatLine.Id)
                                 .FirstOrDefaultAsync();
            if (existingchatLine == null)
            {
                throw new Exception($"ChatLine is Invalid");
            }
            _appDbContext.Entry(existingchatLine).CurrentValues.SetValues(chatLine);
            return chatLine;
        }
    }
}
