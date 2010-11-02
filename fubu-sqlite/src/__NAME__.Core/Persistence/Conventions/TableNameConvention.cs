using __NAME__.Core.Util;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace __NAME__.Core.Persistence.Conventions
{
    public class TableNameConvention : IClassConvention
    {
        public void Apply(IClassInstance instance)
        {
            instance.Table(instance.EntityType.Name.Pluralize());
        }
    }
}