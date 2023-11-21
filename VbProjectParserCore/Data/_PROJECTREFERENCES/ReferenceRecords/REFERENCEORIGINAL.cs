using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VbProjectParserCore.Compression;
using VbProjectParserCore.Data.Base;
using VbProjectParserCore.Data.Base.Attributes;

namespace VbProjectParserCore.Data._PROJECTREFERENCES.ReferenceRecords;

// todo
public class REFERENCEORIGINAL : DataBase
{
    [MustBe((ushort)0x000E)]
    public readonly ushort Id;

    public readonly uint SizeOfLibidOriginal;

    [LengthMustEqualMember("SizeOfLibidOriginal")]
    public byte[] LibidOriginal;

    public REFERENCEORIGINAL(XlBinaryReader Data)
    {
        Id = Data.ReadUInt16();
        SizeOfLibidOriginal = Data.ReadUInt32();
        LibidOriginal = Data.ReadBytes(SizeOfLibidOriginal);

        Validate();
    }
}
