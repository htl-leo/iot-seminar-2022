using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.DataTransferObjects
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="Login"></param>
    /// <param name="Logout"></param>
    public record SessionDto(DateTime Login, DateTime? Logout);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="ApplicationUserId"></param>
    /// <param name="UserSessions"></param>
    public record UserSessionsDto(string ApplicationUserId, SessionDto[] UserSessions);
}
