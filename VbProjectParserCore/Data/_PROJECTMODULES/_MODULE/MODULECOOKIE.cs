using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VbProjectParserCore.Compression;
using VbProjectParserCore.Data.Base;
using VbProjectParserCore.Data.Base.Attributes;

namespace VbProjectParserCore.Data._PROJECTMODULES._MODULE
{
    /// <summary>
    /// Page 49
    /// </summary>
    public class MODULECOOKIE : DataBase
    {
        [AutoRead(1)]
        [MustBe((ushort)0x002C)]
        public readonly ushort Id;

        [AutoRead(2)]
        [MustBe((uint)0x00000002)]
        public readonly uint Size;

        [AutoRead(3)]
        public readonly ushort Cookie;

        public MODULECOOKIE(XlBinaryReader Data)
            : base(Data)
        {
            Validate();
        }
    }
}
