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
    /// <summary>
    /// Size: 10 bytes
    /// </summary>
    public class PROJECTSYSKIND : DataBase
    {
        [MustBe((ushort)0x0001)]
        public readonly ushort Id;

        [MustBe((uint)0x00000004)]
        public readonly uint Size;

        [MustBe((uint)0x00000000, (uint)0x00000001, (uint)0x00000002, (uint)0x00000003)]
        public readonly uint SysKind;

        public PROJECTSYSKIND(XlBinaryReader Data)
        {
            Id = Data.ReadUInt16();
            Size = Data.ReadUInt32();
            SysKind = Data.ReadUInt32();

            Validate();
        }
    }
}
