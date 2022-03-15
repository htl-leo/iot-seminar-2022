using System.Collections.Generic;
using System.Threading.Tasks;
using Base.DataTransferObjects;
using Base.Entities;

namespace Base.Contracts.Persistence
{
    public interface ISessionRepository : IGenericRepository<Session>
    {
        /// <summary>
        /// Liefert letzte Session des Benutzers.
        /// Ist der Benutzer noch eingeloggt, ist das Logout-Date null
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<Session> GetLastByUserAsync(string userId);
        Task RemoveAllByUserAsync(string userId);
        Task<IEnumerable<UserSessionsDto>> GetSessionsPerUserAsync();
    }
}
