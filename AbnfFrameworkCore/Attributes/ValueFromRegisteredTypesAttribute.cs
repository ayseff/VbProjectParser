using AbnfFrameworkCore.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AbnfFrameworkCore.Attributes;

public class ValueFromRegisteredTypesAttribute : ValueModifyingAttribute, IModifier
{
    public static readonly int ProcessingPriority = ValueFromRegexAttribute.ProcessingPriority;
    int IModifier.ProcessingPriority => ProcessingPriority;

    public Type[] Types { get; set; }

    public ValueFromRegisteredTypesAttribute(Type[] Types, Type ValueConverterType)
        : base(ValueConverterType)
    {
        if (Types == null || Types.Length == 0)
            throw new ArgumentNullException("Types");

        this.Types = Types;
    }

    public ValueFromRegisteredTypesAttribute(Type[] Types, IValueConverter ConverterInstance)
        : base(ConverterInstance)
    {
        this.Types = Types;
    }

    public ValueFromRegisteredTypesAttribute(Type[] Types)
        : base()
    {
        this.Types = Types;
    }

    public override string DecorateRegexPattern(IEntityBuilder caller, MemberInfo Source, string InitialPattern)
    {
        if (!string.IsNullOrEmpty(InitialPattern))
            throw new ArgumentException("Expected InitialPattern to be null or empty", "InitialPattern");

        var builders = Types.ToDictionary(x => x, x => caller.OwningSyntax.Entity(x));
        var missingBuilderTypes = builders.Where(x => x.Value == null).Select(x => x.Key);

        if (missingBuilderTypes.Any())
            throw new InvalidOperationException("Not all types were registered in the AbnfSyntaxBuilder: " + string.Join(", ", missingBuilderTypes.Select(x => x.Name)));

        var result = new StringBuilder("");

        result.Append(@"(?:");

        bool first = true;
        foreach (var builder in builders.Values)
        {
            if (!first)
                result.Append("|");

            result.Append(@"(");

            var subRegex = builder.GetRegexPattern();
            result.Append(subRegex);

            result.Append(@")");
            first = false;
        }

        result.Append(")");

        return result.ToString();
    }


    public override object ModifyTargetValue(IEntityBuilder caller, Type SourcePropertyType, object NewValue)
    {
        if (caller == null)
            throw new ArgumentNullException("caller");

        if (SourcePropertyType == null)
            throw new ArgumentNullException("Source");

        if (!(NewValue is string))
            throw new ArgumentException($"Expected NewValue to be of type string; {NewValue}", "NewValue");

        string strNewValue = (string)NewValue;

        Type TargetType = SourcePropertyType;
        var TargetSyntaxBuilders = Types.Select(x => caller.OwningSyntax.Entity(x));

        IList<object> candidates = new List<object>();

        object bestCandidate = null;
        Type bestCandidateType = null;
        bool parserFound = false;

        foreach (var TargetSyntaxBuilder in TargetSyntaxBuilders)
        {
            if (TargetSyntaxBuilder.CanParse(strNewValue))
            {
                parserFound = true;

                object result = TargetSyntaxBuilder.FromAbnfSyntax(strNewValue);

                if (bestCandidate == null || bestCandidateType.IsAssignableFrom(TargetSyntaxBuilder.EntityType))
                {
                    bestCandidate = result;
                    bestCandidateType = TargetSyntaxBuilder.EntityType;
                }
            }
        }

        if (!parserFound)
        {
            var p = TargetSyntaxBuilders.Single();
            var _DEBUG_TODO_REMOVE = p.CanParse(strNewValue);
            throw new InvalidOperationException($"No parser found for {NewValue}");
        }


        return bestCandidate;
    }

    public override void ModifyAbnfSyntaxRepresentationFor(IEntityBuilder caller, ref object memberValue, StringBuilder representation)
    {
        if (memberValue == null)
            return;

        Type objType = memberValue.GetType();

        if (!Types.Contains(objType))
            throw new InvalidOperationException($"Encountered unknown object type {objType.Name}");

        var builder = caller.OwningSyntax.Entity(objType);

        if (builder == null)
            throw new InvalidOperationException($"No AbnfSyntaxBuilder-type registered for {objType.Name}");

        string abnfSyntax = builder.ToAbnfSyntax(memberValue);
        representation.Append(abnfSyntax);
    }
}
