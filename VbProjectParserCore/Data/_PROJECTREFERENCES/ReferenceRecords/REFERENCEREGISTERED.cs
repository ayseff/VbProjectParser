using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VbProjectParserCore.Data.Exceptions;
using VbProjectParserCore.Compression;
using VbProjectParserCore.Data._PROJECTINFORMATION;
using VbProjectParserCore.Data.Base;
using VbProjectParserCore.Data.Base.Attributes;

namespace VbProjectParserCore.Data._PROJECTREFERENCES.ReferenceRecords;

public class REFERENCEREGISTERED : DataBase
{
    [MustBe((ushort)0x000D)]
    public readonly ushort Id;

    public readonly uint Size;

    public readonly uint SizeOfLibid;

    public byte[] Libid;

    [MustBe((uint)0x00000000)]
    public readonly uint Reserved1;

    [MustBe((ushort)0x0000)]
    public readonly ushort Reserved2;

    protected readonly PROJECTINFORMATION ProjectInformation;

    public REFERENCEREGISTERED(XlBinaryReader Data)
    {
        Id = Data.ReadUInt16();
        Size = Data.ReadUInt32();
        SizeOfLibid = Data.ReadUInt32();
        Libid = Data.ReadBytes(SizeOfLibid);
        Reserved1 = Data.ReadUInt32();
        Reserved2 = Data.ReadUInt16();

        Validate();
    }

    protected override void Validate()
    {
        base.Validate();
        ValidateLibid();
    }

    protected void ValidateLibid()
    {
        if (Libid.Length != SizeOfLibid)
        {
            throw new WrongValueException("Libid.Length", Libid.Length, SizeOfLibid);
        }
    }
}
