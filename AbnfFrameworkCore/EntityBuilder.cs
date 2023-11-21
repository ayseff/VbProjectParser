using AbnfFrameworkCore.Attributes;
using AbnfFrameworkCore.Converters;
using AbnfFrameworkCore.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AbnfFrameworkCore.Extensions;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.ObjectModel;
using AbnfFrameworkCore.Interface;

namespace AbnfFrameworkCore;

public class EntityBuilder<TObj> : IEntityBuilder<TObj>
    where TObj : class
{
    private IDictionary<MemberInfo, IPropertyBuilder> _PropertyBuilders = new Dictionary<MemberInfo, IPropertyBuilder>();
    public IValueConverter DefaultConverter { get; private set; }
    private readonly Syntax ParentSyntax;

    public Syntax OwningSyntax => ParentSyntax;

    public Type EntityType => typeof(TObj);

    public EntityBuilder(Syntax ParentSyntax, IValueConverter DefaultConverter)
    {
        this.ParentSyntax = ParentSyntax;
        this.DefaultConverter = DefaultConverter;
    }

    public IPropertyBuilder GetPropertyBuilderFor(MemberInfo SelectedMember)
    {
        return _PropertyBuilders[SelectedMember];
    }

    public IDictionary<MemberInfo, IPropertyBuilder> GetPropertyBuilders()
    {
        return new ReadOnlyDictionary<MemberInfo, IPropertyBuilder>(_PropertyBuilders);
    }

    public IPropertyBuilder<TObj, IEnumerable<TProperty>> EnumerableProperty<TProperty>(Expression<Func<TObj, IEnumerable<TProperty>>> PropertySelector, int? MinCount = null, int? MaxCount = null)
    {
        if (PropertySelector == null)
            throw new ArgumentNullException(nameof(PropertySelector));

        MemberInfo mi = GetMemberInfo(PropertySelector);

        if (mi == null)
            throw new ArgumentException($"Could not get MemberInfo from {PropertySelector}. Make sure the expression is correct.", nameof(PropertySelector));

        if (!_PropertyBuilders.ContainsKey(mi))
        {
            var builder = new EnumerablePropertyBuilder<TObj, TProperty>(this, mi, typeof(TProperty), MinCount, MaxCount);
            _PropertyBuilders.Add(mi, builder);
        }

        return (IPropertyBuilder<TObj, IEnumerable<TProperty>>)_PropertyBuilders[mi];
    }

    public IPropertyBuilder<TObj, TProperty> Property<TProperty>(Expression<Func<TObj, TProperty>> PropertySelector)
    {
        if (PropertySelector == null)
            throw new ArgumentNullException(nameof(PropertySelector));

        MemberInfo mi = GetMemberInfo(PropertySelector);

        if (mi == null)
            throw new ArgumentException($"Could not get MemberInfo from {PropertySelector}. Make sure the expression is correct.", nameof(PropertySelector));

        if (!_PropertyBuilders.ContainsKey(mi))
        {
            var builder = new PropertyBuilder<TObj, TProperty>(this, mi);
            _PropertyBuilders.Add(mi, builder);
        }

        return (IPropertyBuilder<TObj, TProperty>)_PropertyBuilders[mi];
    }

    public IEntityBuilder<TObj> HasBaseType<TBaseType>()
        where TBaseType : class
    {
        if (ParentSyntax.Entity(typeof(TBaseType)) == null)
            throw new ArgumentException($"No builder for type {typeof(TBaseType)} registered. Make sure to call .HasBaseType() on an inheriting type AFTER having set up the parent type within the builder", "TBaseType");

        if (!typeof(TBaseType).IsAssignableFrom(typeof(TObj)))
            throw new ArgumentException($"{typeof(TObj).FullName} does not inherit from {typeof(TBaseType).FullName}", "TBaseType");

        var ParentBuilder = ParentSyntax.Entity<TBaseType>();
        foreach (var kvp in ParentBuilder.GetPropertyBuilders())
        {
            _PropertyBuilders.Add(kvp);
        }

        return this;
    }

    public string GetRegexPattern()
    {
        var result = new StringBuilder();

        foreach (var kvp in _PropertyBuilders)
        {
            var builder = kvp.Value;
            Trace.WriteLine($"Processing builder {builder}");

            string capture_group_name = GetRegexGroupNameFor(kvp.Key, kvp.Value);

            string pattern = @"(?<" + capture_group_name + ">";
            pattern += builder.GetRegex();
            pattern += "(?# end " + capture_group_name + ")";
            pattern += ")";
            result.Append(pattern);
        }

        return result.ToString();
    }

    private string GetRegexGroupNameFor(MemberInfo mi, IPropertyBuilder builder)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        return $"Grp_{mi.Name}";
    }

    public bool CanParse(string syntax)
    {
        if (string.IsNullOrWhiteSpace(syntax))
            return true; // debug.. hmm todo

        var regexPattern = "^" + GetRegexPattern() + "$";
        var regex = new Regex(regexPattern);
        var result = regex.IsMatch(syntax);
        return result;
    }

    object IEntityBuilder.FromAbnfSyntax(string syntax)
    {
        return FromAbnfSyntax(syntax);
    }


    public TObj FromAbnfSyntax(string syntax)
    {
        if (string.IsNullOrWhiteSpace(syntax) && !EntityType.IsValueType)
            return default; // todo: hmm..


        TObj result = (TObj)Activator.CreateInstance(typeof(TObj));

        var regexPattern = "^" + GetRegexPattern() + "$";
        var regex = new Regex(regexPattern);
        var match = regex.Match(syntax);

        if (!match.Success)
            throw new InvalidOperationException($"For entity type {EntityType.Name}, could not match syntax {syntax} with regex pattern {regexPattern}");

        foreach (var kvp in _PropertyBuilders)
        {
            var builder = kvp.Value;
            string capture_group_name = GetRegexGroupNameFor(kvp.Key, kvp.Value);
            var captured_group = match.Groups[capture_group_name];

            if (captured_group == null)
                throw new InvalidOperationException($"Did not find any results for capture group {capture_group_name}");

            string captured_value = captured_group.Value;
            builder.ModifyObj(result, captured_value);
        }

        return result;
    }

    public string ToAbnfSyntax(TObj obj)
    {
        var result = new StringBuilder();

        foreach (var builder in _PropertyBuilders.Values)
        {
            var abnf = builder.ToAbnfSyntax(obj);
            result.Append(abnf);
        }

        return result.ToString();
    }

    string IEntityBuilder.ToAbnfSyntax(object obj)
    {
        return ToAbnfSyntax(obj as TObj);
    }

    private static MemberInfo GetMemberInfo(LambdaExpression lambda)
    {
        MemberExpression memberExpression;
        if (lambda.Body is UnaryExpression)
        {
            var unaryExpression = (UnaryExpression)lambda.Body;
            memberExpression = (MemberExpression)unaryExpression.Operand;
        }
        else
            memberExpression = (MemberExpression)lambda.Body;

        return memberExpression.Member;
    }
}
