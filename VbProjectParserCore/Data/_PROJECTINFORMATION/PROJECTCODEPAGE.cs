using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VbProjectParserCore.Compression;
using VbProjectParserCore.Data.Base;
using VbProjectParserCore.Data.Base.Attributes;

namespace VbProjectParserCore.Data._PROJECTINFORMATION;

public class PROJECTCODEPAGE : DataBase
{
    [MustBe((ushort)0x0003)]
    public readonly ushort Id;

    [MustBe((uint)0x00000002)]
    public readonly uint Size;

    public readonly ushort CodePage;

    public PROJECTCODEPAGE(XlBinaryReader Data)
    {
        Id = Data.ReadUInt16();
        Size = Data.ReadUInt32();
        CodePage = Data.ReadUInt16();

        Validate();
    }

    /// <summary>
    /// Gets the Encoding object as specified by this record
    /// </summary>
    public Encoding GetEncoding()
    {
        return Encoding.GetEncoding(Convert.ToInt32(CodePage));
    }
}
