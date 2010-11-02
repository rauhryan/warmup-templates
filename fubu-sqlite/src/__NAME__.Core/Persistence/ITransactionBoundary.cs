using System;

namespace __NAME__.Core.Persistence
{
    public interface ITransactionBoundary : IDisposable
    {
        void Start();
        void Commit();
        void Rollback();
    }
}