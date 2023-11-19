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
    /// Page 50.
    /// Specifies that the containing MODULE Record (section 2.3.4.2.3.2) is only usable from within the current VBA project.
    /// </summary>
    public class MODULEPRIVATE : DataBase
    {
        [AutoRead(1)]
        [MustBe((ushort)0x0028)]
        public readonly ushort Id;

        [AutoRead(2)]
        [MustBe((uint)0x00000000)]
        public readonly uint Reserved;

        public MODULEPRIVATE(XlBinaryReader Data)
            : base(Data)
        {
            Validate();
        }
    }
}
