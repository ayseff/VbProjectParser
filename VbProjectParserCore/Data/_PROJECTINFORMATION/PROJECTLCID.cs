using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VbProjectParserCore.Data.Exceptions;
using VbProjectParserCore.Compression;
using VbProjectParserCore.Data.Base;
using VbProjectParserCore.Data.Base.Attributes;

namespace VbProjectParserCore.Data._PROJECTINFORMATION
{
    public class PROJECTLCID : DataBase
    {
        [MustBe((ushort)0x0002)]
        public readonly ushort Id;

        [MustBe((uint)0x00000004)]
        public readonly uint Size;

        [MustBe((uint)0x00000409)]
        public readonly uint Lcid;

        public PROJECTLCID(XlBinaryReader Data)
        {
            Id = Data.ReadUInt16();
            Size = Data.ReadUInt32();
            Lcid = Data.ReadUInt32();

            Validate();
        }
    }
}
