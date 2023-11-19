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
    /// Page 49.
    /// Specifies that the containing MODULE Record (section 2.3.4.2.3.2) is read-only.
    /// </summary>
    public class MODULEREADONLY : DataBase
    {
        [AutoRead(1)]
        [MustBe((ushort)0x0025)]
        public readonly ushort Id;

        [AutoRead(2)]
        [MustBe((uint)0x00000000)]
        public readonly uint Reserved;

        public MODULEREADONLY(XlBinaryReader Data)
            : base(Data)
        {
            Validate();
        }
    }
}
