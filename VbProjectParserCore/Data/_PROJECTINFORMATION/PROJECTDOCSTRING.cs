using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VbProjectParserCore.Data;
using VbProjectParserCore.Data.Exceptions;
using VbProjectParserCore.Compression;
using VbProjectParserCore.Data.Base;
using VbProjectParserCore.Data.Base.Attributes;

namespace VbProjectParserCore.Data._PROJECTINFORMATION;

public class PROJECTDOCSTRING : DataBase
{
    [MustBe((ushort)0x0005)]
    public readonly ushort Id;

    [Range(uint.MinValue, 2000)]
    public readonly uint SizeOfDocString;

    [LengthMustEqualMember("SizeOfDocString")]
    public readonly byte[] DocString;

    [MustBe((ushort)0x0040)]
    public readonly ushort Reserved;

    [IsEvenNumber]
    public readonly uint SizeOfDocStringUnicode;

    [LengthMustEqualMember("SizeOfDocStringUnicode")]
    [ValidateWith("ValidateBothDocStrings")]
    public readonly byte[] DocStringUnicode;

    protected readonly PROJECTINFORMATION parent;

    public PROJECTDOCSTRING(PROJECTINFORMATION parent, XlBinaryReader Data)
    {
        this.parent = parent;

        Id = Data.ReadUInt16();
        SizeOfDocString = Data.ReadUInt32();
        DocString = Data.ReadBytes(Convert.ToInt32(SizeOfDocString));
        Reserved = Data.ReadUInt16();
        SizeOfDocStringUnicode = Data.ReadUInt32();
        DocStringUnicode = Data.ReadBytes(Convert.ToInt32(SizeOfDocStringUnicode));

        Validate();
    }


    protected ValidationResult ValidateBothDocStrings(object ValidationObject, MemberInfo member)
    {
        if (!GetDocStringAsString().Equals(GetDocStringUnicodeAsString()))
        {
            var ex = new ArgumentException("DocStringUnicode (string) was not equal DocString (string)", "DocStringUnicde");
            return new ValidationResult(ex);
        }

        return new ValidationResult();
    }

    public string GetDocStringAsString(Encoding encoding)
    {
        return encoding.GetString(DocString);
    }

    public string GetDocStringAsString(PROJECTCODEPAGE Codepage)
    {
        return GetDocStringAsString(Codepage.GetEncoding());
    }

    public string GetDocStringAsString(PROJECTINFORMATION ProjectInformation)
    {
        return GetDocStringAsString(ProjectInformation.CodePageRecord);
    }

    public string GetDocStringAsString()
    {
        return GetDocStringAsString(parent);
    }

    public string GetDocStringUnicodeAsString()
    {
        return Encoding.Unicode.GetString(DocStringUnicode);
    }

}
