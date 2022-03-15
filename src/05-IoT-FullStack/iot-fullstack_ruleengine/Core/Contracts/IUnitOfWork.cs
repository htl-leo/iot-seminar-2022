using Base.Contracts.Persistence;

namespace Core.Contracts
{
    public interface IUnitOfWork : IBaseUnitOfWork
    {
        IActorRepository ActorRepository { get; }
        ISensorRepository SensorRepository { get; }
        IMeasurementRepository MeasurementRepository { get; }

    }
}
