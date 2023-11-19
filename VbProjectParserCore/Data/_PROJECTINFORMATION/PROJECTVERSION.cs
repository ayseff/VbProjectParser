using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VbProjectParserCore.Compression;
using VbProjectParserCore.Data.Base;
using VbProjectParserCore.Data.Base.Attributes;

namespace VbProjectParserCore.Data._PROJECTINFORMATION
{
    public class PROJECTVERSION : DataBase
    {
        [MustBe((ushort)0x0009)]
        public readonly ushort Id;

        [MustBe((uint)0x00000004)]
        public readonly uint Reserved;

        public readonly uint VersionMajor;

        public readonly ushort VersionMinor;

        public PROJECTVERSION(XlBinaryReader Data)
        {
            Id = Data.ReadUInt16();
            Reserved = Data.ReadUInt32();
            VersionMajor = Data.ReadUInt32();
            VersionMinor = Data.ReadUInt16();

            Validate();
        }
    }
}
