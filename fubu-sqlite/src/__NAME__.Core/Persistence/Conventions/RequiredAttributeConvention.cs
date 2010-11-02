using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;
using __NAME__.Core.Validation;

namespace __NAME__.Core.Persistence.Conventions
{
    public class RequiredAttributeConvention: AttributePropertyConvention<RequiredAttribute>
    {
        protected override void Apply(RequiredAttribute attribute, IPropertyInstance instance)
        {
            instance.Not.Nullable();
        }
    }
}