using System.Web;
using __NAME__.Core;
using StructureMap.Configuration.DSL;

namespace __NAME__.Web
{
    public class __NAME__WebRegistry : Registry
    {
        public __NAME__WebRegistry()
        {
            Scan(x =>
            {
                x.TheCallingAssembly();
                x.WithDefaultConventions();

                x.AddAllTypesOf<IStartable>();

            }
               );

            For<HttpContextBase>().Use(ctx => new HttpContextWrapper(HttpContext.Current));
            For<HttpRequestBase>().Use(ctx => new HttpRequestWrapper(HttpContext.Current.Request));

        }
    }
}