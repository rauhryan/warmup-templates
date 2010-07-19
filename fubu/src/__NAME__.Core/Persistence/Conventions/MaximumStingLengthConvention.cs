using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;
using __NAME__.Core.Validation;

namespace __NAME__.Core.Persistence.Conventions
{
    public class MaximumStingLengthConvention : AttributePropertyConvention<MaximumStringLengthAttribute>
    {
        protected override void Apply(MaximumStringLengthAttribute attribute, IPropertyInstance instance)
        {
            instance.Length(attribute.Length);
        }
    }
}