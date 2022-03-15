using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Base.Contracts.Persistence;
using Base.DataTransferObjects;
using Base.Entities;

using Microsoft.EntityFrameworkCore;

namespace Base.Persistence.Repositories
{
    public class SessionRepository : GenericRepository<Session>, ISessionRepository
    {
        private readonly BaseApplicationDbContext _dbContext;

        public SessionRepository(BaseApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Session> GetLastByUserAsync(string userId)
        {
            return await _dbContext.Sessions
                    .Where(s => s.ApplicationUserId == userId)
                    .OrderByDescending(s => s.Login)
                    .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<UserSessionsDto>> GetSessionsPerUserAsync()
        {
            return await _dbContext.Sessions
                .GroupBy(s => s.ApplicationUserId)
                .Select(grp =>
                    new UserSessionsDto(
                        grp.Key,
                        grp
                            .OrderByDescending(s => s.Login)
                            .Select(s => new SessionDto(s.Login, s.Logout))
                            .ToArray()))
                .ToListAsync();
        }

        public async Task RemoveAllByUserAsync(string userId)
        {
            var sessions = await _dbContext.Sessions
                .Where(s=>s.ApplicationUserId == userId)
                .ToArrayAsync();

            _dbContext.Sessions.RemoveRange(sessions);
        }
    }
}
