using System;
using StructureMap;

namespace Kokugen.Core.Persistence
{
    public interface ITransactionProcessor
    {
        void WithinTransaction(Action<IContainer> action);
    }
}