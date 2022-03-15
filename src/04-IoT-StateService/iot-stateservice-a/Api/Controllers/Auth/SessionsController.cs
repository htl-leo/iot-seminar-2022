#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Base.Entities;
using Persistence;
using Core.Contracts;
using Microsoft.AspNetCore.Authorization;
using Base.DataTransferObjects;

namespace Api.Controllers.Auth
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SessionsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="unitOfWork"></param>
        public SessionsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserSessionsDto>>> Get()
        {
            var sessions = await _unitOfWork.Sessions.GetSessionsPerUserAsync();
            return Ok(sessions);
        }

    }
}
