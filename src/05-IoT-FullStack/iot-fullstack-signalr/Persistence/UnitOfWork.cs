
using Base.Persistence;

using Core.Contracts;

namespace Persistence
{
    public class UnitOfWork : BaseUnitOfWork, IUnitOfWork
    {
        public ApplicationDbContext ApplicationDbContext => BaseApplicationDbContext as ApplicationDbContext;

        public UnitOfWork(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            ActorRepository = new ActorRepository(applicationDbContext);
            SensorRepository = new SensorRepository(applicationDbContext);
            MeasurementRepository = new MeasurementRepository(applicationDbContext);
        }

        public UnitOfWork() : this(new ApplicationDbContext())
        {
        }

        public ISensorRepository SensorRepository { get; }
        public IActorRepository ActorRepository { get; }
        public IMeasurementRepository MeasurementRepository { get; }


    }
}
