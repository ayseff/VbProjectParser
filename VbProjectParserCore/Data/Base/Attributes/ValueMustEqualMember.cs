using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VbProjectParserCore.Data.Base;

namespace VbProjectParserCore.Data.Base.Attributes;

public class ValueMustEqualMember : ValidationAttribute
{
    protected readonly string OtherMemberPath;

    public ValueMustEqualMember(string OtherMemberPath)
    {
        this.OtherMemberPath = OtherMemberPath;
    }

    protected virtual object GetOtherMemberValue(object ValidationObject)
    {
        return ReflectionHelper.GetPropertyValue(ValidationObject, OtherMemberPath);
    }

    public override ValidationResult Validate(object ValidationObject, MemberInfo member)
    {
        if (ValidationObject == null)
            throw new ArgumentNullException(nameof(ValidationObject));

        if (member == null)
            throw new ArgumentNullException(nameof(member));

        var ActualValue = ReflectionHelper.GetValue(ValidationObject, member);
        object OtherMemberValue;
        try
        {
            OtherMemberValue = GetOtherMemberValue(ValidationObject);
        }
        catch (NullReferenceException ex)
        {
            return new ValidationResult(new ArgumentException("Could not access member path " + OtherMemberPath, member.Name, ex));
        }

        if (!ReflectionHelper.AreEqual(OtherMemberValue, ActualValue))
        {
            var ex = new ArgumentException(
                $"Expected {member.Name} to equal the value of member {OtherMemberPath}, but {member.Name}={ActualValue} and {OtherMemberPath}={OtherMemberValue}",
                member.Name);
            return new ValidationResult(ex);
        }

        return new ValidationResult();
    }
}
