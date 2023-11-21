using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VbProjectParserCore.Data;
using VbProjectParserCore.Data._PROJECTINFORMATION;
using VbProjectParserCore.Compression;
using VbProjectParserCore.Data.Base;
using VbProjectParserCore.Data.Base.Attributes;

namespace VbProjectParserCore.Data._PROJECTMODULES._MODULE;

public class MODULENAMEUNICODE : DataBase
{
    [MustBe((ushort)0x0047)]
    public readonly ushort Id;

    [IsEvenNumber]
    public readonly uint SizeOfModuleNameUnicode;

    [LengthMustEqualMember("SizeOfModuleNameUnicode")]
    public readonly byte[] ModuleNameUnicode;

    public MODULENAMEUNICODE(XlBinaryReader Data)
    {
        Id = Data.ReadUInt16();
        SizeOfModuleNameUnicode = Data.ReadUInt32();
        ModuleNameUnicode = Data.ReadBytes(SizeOfModuleNameUnicode);

        Validate();
    }

    public string GetModuleNameUnicodeAsString()
    {
        return Encoding.Unicode.GetString(ModuleNameUnicode);
    }
}
