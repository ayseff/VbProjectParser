using AbnfFrameworkCore.Attributes;
using AbnfFrameworkCore.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AbnfFrameworkCore.Tokens;

public class TokenModifier : IModifier
{
    public enum AppendDirection
    {
        Prefix,
        Postfix
    }

    public static readonly int ProcessingPriority = ValueFromRegexAttribute.ProcessingPriority + 1;
    int IModifier.ProcessingPriority => ProcessingPriority;

    public AppendDirection Direction { get; private set; }
    public Token Token { get; set; }

    public TokenModifier(AppendDirection Direction, Token Token)
    {
        this.Direction = Direction;
        this.Token = Token;
    }

    public string DecorateRegexPattern(IEntityBuilder caller, MemberInfo Source, string InitialPattern)
    {
        string pattern = Token.GetRegexPattern(Source);

        if (Direction == AppendDirection.Prefix)
            return pattern + InitialPattern;
        else
            return InitialPattern + pattern;
    }

    public object ModifyTargetValue(IEntityBuilder caller, Type SourcePropertyType, object NewValue)
    {
        if (!(NewValue is string))
            throw new ArgumentException($"Expected NewValue ({NewValue}) to be string", "NewValue");

        string strValue = (string)NewValue;

        string tokenPattern = "(?<TokenMatch>" + Token.GetRegexPattern(null) + "){1}";
        string nontokenPattern = "(?<NontokenMatch>.*)";

        string pattern = Direction == AppendDirection.Postfix ? nontokenPattern + tokenPattern : tokenPattern + nontokenPattern;
        pattern = "^" + pattern + "$";
        var regex = new Regex(pattern, RegexOptions.Singleline);
        var match = regex.Match(strValue);

        if (!match.Success)
            throw new InvalidOperationException($"Could not match value {strValue} with regex {pattern}");

        var grp = match.Groups["NontokenMatch"];
        var grpValue = grp.Value;

        return grpValue;
    }

    public void ModifyAbnfSyntaxRepresentationFor(IEntityBuilder caller, /*MemberInfo source, object obj, */ ref object obj, StringBuilder representation)
    {
        var syntax = Token.GetAbnfSyntaxRepresentationFor(obj);

        if (Direction == AppendDirection.Prefix)
            representation.Insert(0, syntax);
        else
            representation.Append(syntax);

    }

    public override string ToString()
    {
        return $"{base.ToString()} ({Token})";
    }
}
