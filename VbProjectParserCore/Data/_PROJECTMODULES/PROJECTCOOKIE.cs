using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VbProjectParserCore.Compression;
using VbProjectParserCore.Data.Base;
using VbProjectParserCore.Data.Base.Attributes;

namespace VbProjectParserCore.Data._PROJECTMODULES;

public class PROJECTCOOKIE : DataBase
{
    [MustBe((ushort)0x0013)]
    public readonly ushort Id;

    [MustBe((uint)0x00000002)]
    public readonly uint Size;

    /// <summary>
    /// Must be 0xFFFF on write
    /// </summary>
    public readonly ushort Cookie;

    public PROJECTCOOKIE(XlBinaryReader Data)
    {
        Id = Data.ReadUInt16();
        Size = Data.ReadUInt32();
        Cookie = Data.ReadUInt16();

        Validate();
    }
}
