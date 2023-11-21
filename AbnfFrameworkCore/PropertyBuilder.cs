using AbnfFrameworkCore.Attributes;
using AbnfFrameworkCore.Converters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Collections;
using AbnfFrameworkCore.Attributes;
using AbnfFrameworkCore.Tokens;
using AbnfFrameworkCore.Interface;
using AbnfFrameworkCore.Extensions;

namespace AbnfFrameworkCore;

public class PropertyBuilder<TObj, TProperty> : IPropertyBuilder<TObj, TProperty>
    where TObj : class
{
    protected readonly List<IModifier> Modifiers = new List<IModifier>();

    public MemberInfo TargetMember { get; private set; }
    public EntityBuilder<TObj> Builder { get; private set; }

    public override string ToString()
    {
        return $"{typeof(PropertyBuilder<TObj, TProperty>).Name} ({TargetMember})";
    }

    public PropertyBuilder(EntityBuilder<TObj> Builder, MemberInfo TargetMember)
    {
        this.Builder = Builder;
        this.TargetMember = TargetMember;

        ReadModifierAttributes();
    }

    void IPropertyBuilder.ModifyObj(object obj, string FromText)
    {
        ModifyObj(obj as TObj, FromText);
    }

    public virtual void ModifyObj(TObj obj, string FromText)
    {
        if (string.IsNullOrWhiteSpace(FromText))
            return;

        object property_value = FromText;

        var modifiers = GetModifiers().Reverse();
        foreach (var modifier in modifiers)
        {
            property_value = modifier.ModifyTargetValue(Builder, TargetMember.GetUnderlyingType(), property_value);
        }

        TargetMember.SetValue(obj, property_value);
    }

    public virtual string ToAbnfSyntax(object obj)
    {
        var result = new StringBuilder();

        object value = TargetMember.GetValue(obj);

        foreach (var modifier in GetModifiers())
        {
            modifier.ModifyAbnfSyntaxRepresentationFor(Builder, /*TargetMember, */ref value, result);
        }

        return result.ToString();
    }

    public IOrderedEnumerable<IModifier> GetModifiers()
    {
        return Modifiers.OrderBy(x => x.ProcessingPriority);
    }

    public virtual string GetRegex()
    {
        var pattern = string.Empty;

        Trace.WriteLine($"PropertyBuilder.GetRegex(): Processing {TargetMember.DeclaringType.Name}.{TargetMember.Name} - begin");

        var modifiers = GetModifiers();
        foreach (var modifier in modifiers)
        {
            Trace.WriteLine($"PropertyBuilder.GetRegex(): Processing {TargetMember.DeclaringType.Name}.{TargetMember.Name} - applying modifier {modifier}");
            pattern = modifier.DecorateRegexPattern(Builder, TargetMember, pattern);
        }

        Trace.WriteLine($"PropertyBuilder.GetRegex(): Processing {TargetMember.DeclaringType.Name}.{TargetMember.Name} - completed");

        return pattern;
    }

    private void ReadModifierAttributes()
    {
        var attributes = TargetMember.GetCustomAttributes<ModifyingAttribute>(true);
        Modifiers.AddRange(attributes);
    }

    public IPropertyBuilder<TObj, TProperty> ByRegexPattern(string RegexPattern)
    {
        var modifier = new ValueFromRegexAttribute(RegexPattern);
        Modifiers.Add(modifier);
        return this;
    }

    public IPropertyBuilder<TObj, TProperty> ByRegexPattern(string RegexPattern, IValueConverter Converter)
    {
        var modifier = new ValueFromRegexAttribute(RegexPattern, Converter);
        Modifiers.Add(modifier);
        return this;
    }

    public IPropertyBuilder<TObj, TProperty> ByRegisteredTypes(params Type[] types)
    {
        var modifier = new ValueFromRegisteredTypesAttribute(types);
        Modifiers.Add(modifier);
        return this;
    }

    public IPropertyBuilder<TObj, TProperty> WithPrefix(Token Prefix)
    {
        var modifier = new TokenModifier(TokenModifier.AppendDirection.Prefix, Prefix);
        Modifiers.Add(modifier);
        return this;
    }

    public IPropertyBuilder<TObj, TProperty> WithPostfix(Token Postfix)
    {
        var modifier = new TokenModifier(TokenModifier.AppendDirection.Postfix, Postfix);
        Modifiers.Add(modifier);
        return this;
    }

    public IPropertyBuilder<TObj, TProperty> IsOptional()
    {
        var modifier = new IsOptionalAttribute();
        Modifiers.Add(modifier);
        return this;
    }
}
