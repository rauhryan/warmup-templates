using System;
using __NAME__.Core.Util;
using FubuCore.Binding;
using FubuMVC.Core.Behaviors;
using StructureMap;

namespace __NAME__.Web.Behaviors
{
    public class TransactionalContainerBehavior : IActionBehavior
    {
        private readonly IContainer _container;
        private readonly ServiceArguments _arguments;
        private readonly Guid _behaviorId;

        public TransactionalContainerBehavior(IContainer container, ServiceArguments arguments, Guid behaviorId)
        {
            _container = container;
            _arguments = arguments;
            _behaviorId = behaviorId;

        }

        public void Invoke()
        {
            _container.ExecuteInTransaction(invokeRequestedBehavior);
        }

        public void InvokePartial()
        {

        }


        private void invokeRequestedBehavior(IContainer c)
        {

            var behavior = c.GetInstance<IActionBehavior>(_arguments.ToExplicitArgs(), _behaviorId.ToString());

            behavior.Invoke();

        }

    }
}