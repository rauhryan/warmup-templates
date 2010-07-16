using System;

namespace Kokugen.Core.Persistence
{
    public interface ITransactionBoundary : IDisposable
    {
        void Start();
        void Commit();
        void Rollback();
    }
}