
using Base.Persistence;

using Core.Contracts;

namespace Persistence
{
    public class UnitOfWork : BaseUnitOfWork, IUnitOfWork
    {
        public ApplicationDbContext ApplicationDbContext => BaseApplicationDbContext as ApplicationDbContext;

        public UnitOfWork(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            //PupilRepository = new PupilRepository(_dbContext);
        }

        //public IPupilRepository PupilRepository { get; }


    }
}
