using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VbProjectParserCore.Compression;
using VbProjectParserCore.Data._PROJECTINFORMATION;
using VbProjectParserCore.Data._PROJECTMODULES._MODULE;
using VbProjectParserCore.Data.Base;
using VbProjectParserCore.Data.Base.Attributes;

namespace VbProjectParserCore.Data._PROJECTMODULES
{
    public class PROJECTMODULES : DataBase
    {
        /// <summary>
        /// An unsigned integer that specifies the identifier for this record. MUST be 0x000F.
        /// </summary>
        [MustBe((ushort)0x000F)]
        public readonly ushort Id;

        /// <summary>
        /// An unsigned integer that specifies the size of Count. MUST be 0x00000002.
        /// </summary>
        [MustBe((uint)0x00000002)]
        public readonly uint Size;

        /// <summary>
        /// An unsigned integer that specifies the number of elements in Modules.
        /// </summary>
        public ushort Count { get; internal set; }

        /// <summary>
        /// A PROJECTCOOKIE Record (section 2.3.4.2.3.1).
        /// </summary>
        public readonly PROJECTCOOKIE ProjectCookieRecord;

        /// <summary>
        /// A PROJECTCOOKIE Record (section 2.3.4.2.3.1). Contains $Count elements.
        /// </summary>
        [LengthMustEqualMember("Count")]
        public readonly MODULE[] Modules;

        public PROJECTMODULES(PROJECTINFORMATION ProjectInformation, XlBinaryReader Data)
        {
            Id = Data.ReadUInt16();
            Size = Data.ReadUInt32();
            Count = Data.ReadUInt16();
            ProjectCookieRecord = new PROJECTCOOKIE(Data);

            // Read Modules
            Modules = new MODULE[Count];
            for (int i = 0; i < Count; i++)
            {
                var module = new MODULE(ProjectInformation, Data);
                Modules[i] = module;

            }

            Validate();
        }
    }
}
