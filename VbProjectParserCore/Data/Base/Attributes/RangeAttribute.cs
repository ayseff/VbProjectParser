using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VbProjectParserCore.Data.Base.Attributes;

public class RangeAttribute : ValidationAttribute
{
    protected long UpperBound;
    protected long LowerBound;

    public RangeAttribute(ushort LowerBound, ushort UpperBound)
    {
        if (UpperBound < LowerBound)
            throw new ArgumentException("UpperBound must be greater or equal than lower bound", nameof(UpperBound));

        this.LowerBound = Convert.ToInt64(LowerBound);
        this.UpperBound = Convert.ToInt64(UpperBound);
    }

    public RangeAttribute(uint LowerBound, uint UpperBound)
    {
        if (UpperBound < LowerBound)
            throw new ArgumentException("UpperBound must be greater or equal than lower bound", nameof(UpperBound));

        this.LowerBound = Convert.ToInt64(LowerBound);
        this.UpperBound = Convert.ToInt64(UpperBound);
    }

    public RangeAttribute(short LowerBound, short UpperBound)
    {
        if (UpperBound < LowerBound)
            throw new ArgumentException("UpperBound must be greater or equal than lower bound", nameof(UpperBound));

        this.LowerBound = Convert.ToInt64(LowerBound);
        this.UpperBound = Convert.ToInt64(UpperBound);
    }

    public RangeAttribute(int LowerBound, int UpperBound)
    {
        if (UpperBound < LowerBound)
            throw new ArgumentException("UpperBound must be greater or equal than lower bound", nameof(UpperBound));

        this.LowerBound = Convert.ToInt64(LowerBound);
        this.UpperBound = Convert.ToInt64(UpperBound);
    }

    public RangeAttribute(long LowerBound, long UpperBound)
    {
        if (UpperBound < LowerBound)
            throw new ArgumentException("UpperBound must be greater or equal than lower bound", nameof(UpperBound));

        this.LowerBound = Convert.ToInt64(LowerBound);
        this.UpperBound = Convert.ToInt64(UpperBound);
    }

    public override ValidationResult Validate(object ValidationObject, MemberInfo member)
    {
        var ActualValue = ReflectionHelper.GetValue(ValidationObject, member);

        if (!ReflectionHelper.IsNumber(ActualValue))
        {
            var ex = new ArgumentException($"Expected a numeric type for {member.Name}, however found {(ActualValue == null ? "null" : ActualValue.GetType().Name)}", member.Name);
            return new ValidationResult(ex);
        }

        long _ActualValue = Convert.ToInt64(ActualValue);
        if (_ActualValue < LowerBound || _ActualValue > UpperBound)
        {
            var ex = new ArgumentException($"Expected {member.Name} to be within range {LowerBound} to {UpperBound}, but was {ActualValue}", member.Name);
            return new ValidationResult(ex);
        }

        return new ValidationResult();
    }
}
