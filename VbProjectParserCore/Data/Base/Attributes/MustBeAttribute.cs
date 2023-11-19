using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VbProjectParserCore.Data.Base.Attributes
{
    public sealed class MustBeAttribute : ValidationAttribute
    {
        /// <summary>
        /// Must be one of these values
        /// </summary>
        public readonly IEnumerable<object> ExpectedValues;

        /// <summary>
        /// Must be of this type
        /// </summary>
        public readonly Type ExpectedType;

        public MustBeAttribute(params uint[] values)
            : this(values.Cast<object>(), typeof(uint))
        {
        }

        public MustBeAttribute(params ushort[] values)
            : this(values.Cast<object>(), typeof(ushort))
        {
        }

        public MustBeAttribute(params int[] values)
            : this(values.Cast<object>(), typeof(int))
        {
        }

        public MustBeAttribute(params short[] values)
            : this(values.Cast<object>(), typeof(short))
        {
        }

        public MustBeAttribute(params byte[] values)
            : this(values.Cast<object>(), typeof(byte))
        {
        }

        private MustBeAttribute(IEnumerable<object> ExpectedValues, Type ExpectedType)
        {
            this.ExpectedValues = ExpectedValues.ToArray();
            this.ExpectedType = ExpectedType;
        }

        public override ValidationResult Validate(object ValidationObject, MemberInfo member)
        {
            var ActualValue = ReflectionHelper.GetValue(ValidationObject, member);

            if (!ExpectedValues.Contains(ActualValue))
            {
                string message = $"Expected {member.Name} to be {string.Join(" OR ", ExpectedValues)}, but was {ActualValue}";
                return new ValidationResult(new ArgumentException(message, member.Name));
            }

            return new ValidationResult();
        }
    }

}
