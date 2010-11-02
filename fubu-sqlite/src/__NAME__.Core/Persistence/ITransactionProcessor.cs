using System;
using StructureMap;

namespace __NAME__.Core.Persistence
{
    public interface ITransactionProcessor
    {
        void WithinTransaction(Action<IContainer> action);
    }
}