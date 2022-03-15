using Core.Contracts;
using Core.Entities;

using Persistence;

using Serilog;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotServices.Services
{
    public class PersistenceService
    {
        public PersistenceService(IUnitOfWork unitOfWork) 
        {
            UnitOfWork = unitOfWork;
        }

        public IUnitOfWork UnitOfWork { get; }
    }
}
