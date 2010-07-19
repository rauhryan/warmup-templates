using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace __NAME__.Core.Persistence.Conventions
{
    public class DefaultStringLengthConvention : IPropertyConvention
    {
        public void Apply(IPropertyInstance instance)
        {
            instance.Length(255);
        }
    }
}