using System.Web.Routing;
using FluentNHibernate;
using __NAME__.Core;
using __NAME__.Core.Util;
using __NAME__.Web.Behaviors;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.StructureMap;
using StructureMap;

namespace __NAME__.Web
{
    public class __NAME__StructureMapBootstrapper
    {
        private readonly RouteCollection _routes;

        private __NAME__StructureMapBootstrapper(RouteCollection routes)
        {
            _routes = routes;
        }

        public static void Bootstrap(RouteCollection routes, FubuRegistry fubuRegistry)
        {
            new __NAME__StructureMapBootstrapper(routes).BootstrapStructureMap(fubuRegistry);
        }

        private void BootstrapStructureMap(FubuRegistry fubuRegistry)
        {
            UrlContext.Reset();

            ObjectFactory.Initialize(x =>
                                         {
                                             x.AddRegistry(new __NAME__CoreRegistry());
                                             x.AddRegistry(new __NAME__WebRegistry());
                                         });

            var fubuBootstrapper = new StructureMapBootstrapper(ObjectFactory.Container, fubuRegistry);
            fubuBootstrapper.Builder = (c, args, id) =>
                                           {
                                               return new TransactionalContainerBehavior(c, args, id);
                                           };
            fubuBootstrapper.Bootstrap(_routes);

            ObjectFactory.Container.GetInstance<ISessionSource>().BuildSchema();

            ObjectFactory.Container.StartStartables();
            
        }
    }
}