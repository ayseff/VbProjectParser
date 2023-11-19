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
    public class MODULEOFFSET : DataBase
    {
        [AutoRead(1)]
        [MustBe((ushort)0x0031)]
        public readonly ushort Id;

        [AutoRead(2)]
        [MustBe((uint)0x00000004)]
        public readonly uint Size;

        /// <summary>
        /// An unsigned integer that specifies the byte offset of the source code in the ModuleStream (section 2.3.4.3) named by MODULESTREAMNAME Record (section 2.3.4.2.3.2.3).
        /// </summary>
        [AutoRead(3)]
        public readonly uint TextOffset;

        public MODULEOFFSET(XlBinaryReader Data)
            : base(Data)
        {
            Validate();
        }
    }
}
