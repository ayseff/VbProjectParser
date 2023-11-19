using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VbProjectParserCore.Data;
using VbProjectParserCore.Compression;
using VbProjectParserCore.Data.Base;
using VbProjectParserCore.Data.Base.Attributes;

namespace VbProjectParserCore.Data._PROJECTMODULES._MODULE
{
    /// <summary>
    /// Page 48
    /// </summary>
    public class MODULEDOCSTRING : DataBase
    {
        [AutoRead(1)]
        [MustBe((ushort)0x001C)]
        public readonly ushort Id;

        [AutoRead(2)]
        public readonly uint SizeOfDocString;

        [AutoRead(3, "SizeOfDocString")]
        [LengthMustEqualMember("SizeOfDocString")]
        public readonly byte[] DocString;

        [AutoRead(4)]
        [MustBe((ushort)0x0048)]
        public readonly ushort Reserved;

        [AutoRead(5)]
        [IsEvenNumber]
        public readonly uint SizeOfDocStringUnicode;

        [AutoRead(6, "SizeOfDocStringUnicode")]
        [LengthMustEqualMember("SizeOfDocStringUnicode")]
        public readonly byte[] DocStringUnicode;

        public MODULEDOCSTRING(XlBinaryReader Data)
            : base(Data)
        {
            Validate();
        }
    }
}
