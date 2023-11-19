using AbnfFrameworkCore.Interface;
using System;

namespace AbnfFrameworkCore.Attributes
{
    public abstract class ValueModifyingAttribute : ModifyingAttribute
    {
        protected Lazy<IValueConverter> ValueConverter { get; set; }

        protected ValueModifyingAttribute(Type ValueConverterType)
        {
            if (ValueConverterType == null)
                throw new ArgumentNullException("ValueConverterType");

            Func<IValueConverter> factory = () => (IValueConverter)Activator.CreateInstance(ValueConverterType);
            ValueConverter = new Lazy<IValueConverter>(factory);
        }

        public ValueModifyingAttribute(IValueConverter converterInstance)
        {
            if (converterInstance == null)
                throw new ArgumentNullException("converterInstance");

            Func<IValueConverter> factory = () => converterInstance;
            ValueConverter = new Lazy<IValueConverter>(factory);
        }

        protected ValueModifyingAttribute()
        {
            Func<IValueConverter> factory = () => null;
            ValueConverter = new Lazy<IValueConverter>(factory);
        }

        protected IValueConverter GetConverterFor(Type type)
        {
            return ValueConverter.Value;
        }
    }
}
