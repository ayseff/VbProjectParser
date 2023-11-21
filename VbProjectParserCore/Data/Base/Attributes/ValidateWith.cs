using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VbProjectParserCore.Data.Exceptions;
using VbProjectParserCore.Data.Base;

namespace VbProjectParserCore.Data.Base.Attributes;

public class ValidateWithAttribute : ValidationAttribute
{
    public delegate ValidationResult ValidationDelegate(object ValidationObject, MemberInfo member);
    //private delegate ValidationDelegate GetDelegate(object ValidationObject, MemberInfo member);

    protected readonly ValidationDelegate InvokeDelegate;

    public ValidateWithAttribute(ValidationDelegate DelegateFunction)
    {
        if (DelegateFunction == null)
            throw new ArgumentNullException(nameof(DelegateFunction));

        InvokeDelegate = DelegateFunction;
    }

    public ValidateWithAttribute(string MethodName)
    {
        InvokeDelegate = (ValidationResult, member) =>
            {
                var mi = ValidationResult.GetType().GetMethod(MethodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                if (mi == null)
                    throw new NullReferenceException($"Could not find method '{MethodName}' to validate member '{member.Name}' with");

                ValidationResult result = (ValidationResult)mi.Invoke(ValidationResult, new object[] { ValidationResult, member });
                return result;
            };
    }

    public override ValidationResult Validate(object ValidationObject, MemberInfo member)
    {
        var result = InvokeDelegate(ValidationObject, member);
        return result;
    }
}
