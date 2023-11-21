using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VbProjectParserCore.Compression;
using VbProjectParserCore.Data.Base;
using VbProjectParserCore.Data.Base.Attributes;

namespace VbProjectParserCore.Data._PROJECTINFORMATION;

public class PROJECTLIBFLAGS : DataBase
{
    [MustBe((ushort)0x0008)]
    public readonly ushort Id;

    [MustBe((uint)0x00000004)]
    public readonly uint Size;

    [MustBe((uint)0x00000000)]
    public readonly uint ProjectLibFlags;

    public PROJECTLIBFLAGS(XlBinaryReader Data)
    {
        Id = Data.ReadUInt16();
        Size = Data.ReadUInt32();
        ProjectLibFlags = Data.ReadUInt32();

        Validate();
    }
}
