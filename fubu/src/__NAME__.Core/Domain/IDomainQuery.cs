#region Using Directives

using System;
using System.Linq.Expressions;

#endregion

namespace __NAME__.Core.Domain
{
    public interface IDomainQuery<ENTITY>
        where ENTITY : Entity
    {
        Expression<Func<ENTITY, bool>> Expression { get; }
    }
}