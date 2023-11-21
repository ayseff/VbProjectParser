using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VbProjectParserCore.Data.Base;

namespace VbProjectParserCore.Data.Base.Attributes;

/// <summary>
/// A property of the member must equal the value of another member of this class
/// </summary>
public class PropertyMustEqualMemberAttribute : ValueMustEqualMember
{
    protected readonly string PropertyName;

    public PropertyMustEqualMemberAttribute(string PropertyName, string OtherMemberName)
        : base(OtherMemberName)
    {
        this.PropertyName = PropertyName;
    }

    public override ValidationResult Validate(object ValidationObject, MemberInfo member)
    {
        if (ValidationObject == null)
            throw new ArgumentNullException(nameof(ValidationObject));

        if (member == null)
            throw new ArgumentNullException(nameof(member));

        var OtherMemberValue = GetOtherMemberValue(ValidationObject);

        object ActualValue = ReflectionHelper.GetValue(ValidationObject, member);
        var propertyMember = ReflectionHelper.GetTypeOf(member).GetMember(PropertyName).First();
        var propertyValue = ReflectionHelper.GetValue(ActualValue, propertyMember);

        if (!ReflectionHelper.AreEqual(OtherMemberValue, propertyValue))
        {
            var ex = new ArgumentException(
                $"Expected {member.Name}.{PropertyName} to equal the value of member {OtherMemberPath}, but {member.Name}.{PropertyName}={propertyValue} and {OtherMemberPath}={OtherMemberValue}",
                member.Name);
            return new ValidationResult(ex);
        }

        return new ValidationResult();
    }




}
