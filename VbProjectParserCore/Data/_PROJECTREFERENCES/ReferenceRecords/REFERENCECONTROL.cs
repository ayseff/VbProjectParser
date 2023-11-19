using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VbProjectParserCore.Data;
using VbProjectParserCore.Compression;
using VbProjectParserCore.Data._PROJECTINFORMATION;
using VbProjectParserCore.Data.Base;
using VbProjectParserCore.Data.Base.Attributes;

namespace VbProjectParserCore.Data._PROJECTREFERENCES.ReferenceRecords
{
    // page 41
    // todo: the specification for this one is weird...
    public class REFERENCECONTROL : DataBase
    {
        [MustBe((ushort)0x002F)]
        public readonly ushort Id;

        public readonly uint SizeTwiddled;

        public readonly uint SizeOfLibidTwiddled;

        [LengthMustEqualMember("SizeOfLibidTwiddled")]
        public readonly byte[] LibidTwiddled;

        [MustBe((uint)0x00000000)]
        public readonly uint Reserved1;

        [MustBe((ushort)0x0000)]
        public readonly ushort Reserved2;

        /// <summary>
        /// A REFERENCENAME Record (section 2.3.4.2.2.2) that specifies the name of the extended type library. This field is optional (= can be null)
        /// </summary>
        public readonly REFERENCENAME NameRecordExtended;

        [MustBe((ushort)0x0030)]
        public readonly ushort Reserved3;

        [ValidateWith("ValidateSizeExtended")]
        public readonly uint SizeExtended;

        public readonly uint SizeOfLibidExtended;

        [LengthMustEqualMember("SizeOfLibidExtended")]
        public readonly byte[] LibidExtended;

        [MustBe((uint)0x00000000)]
        public readonly uint Reserved4;

        [MustBe((ushort)0x0000)]
        public readonly ushort Reserved5;

        public readonly Guid OriginalTypeLib;

        public readonly uint Cookie;

        public REFERENCECONTROL(PROJECTINFORMATION ProjectInformation, XlBinaryReader Data)
        {
            Id = Data.ReadUInt16();
            SizeTwiddled = Data.ReadUInt32();
            SizeOfLibidTwiddled = Data.ReadUInt32();
            LibidTwiddled = Data.ReadBytes(SizeOfLibidTwiddled);
            Reserved1 = Data.ReadUInt32();
            Reserved2 = Data.ReadUInt16();

            ushort peek = Data.PeekUInt16();
            if (peek == 0x0016)
            {
                // REFERENCENAME record
                NameRecordExtended = new REFERENCENAME(ProjectInformation, Data);
            }

            Reserved3 = Data.ReadUInt16();
            SizeExtended = Data.ReadUInt32();
            SizeOfLibidExtended = Data.ReadUInt32();
            LibidExtended = Data.ReadBytes(SizeOfLibidExtended);
            Reserved4 = Data.ReadUInt32();
            Reserved5 = Data.ReadUInt16();
            OriginalTypeLib = Data.ReadGuid();
            Cookie = Data.ReadUInt32();

            Validate();
        }

        /// <summary>
        /// SizeExnteded must be sum of the size in bytes of SizeOfLibidExtended, LibidExtended, Reserved4, Reserved5, OriginalTypeLib, and Cookie.
        /// </summary>
        protected ValidationResult ValidateSizeExtended(object ValidationObject, MemberInfo member)
        {
            var t = sizeof(uint) +        // size of SizeOfLibidExtended
                    (uint)LibidExtended.Length +
                    sizeof(uint) +        // Reserved4
                    sizeof(ushort) +        // Reserved5
                    16 +          // OriginalTypeLib
                    sizeof(uint);         // Cookie

            if (SizeExtended != t)
            {
                var ex = new ArgumentException($"SizeExtended expected size {t}, but was {SizeExtended}", "SizeExtended");
                return new ValidationResult(ex);
            }

            return new ValidationResult();
        }
    }
}
