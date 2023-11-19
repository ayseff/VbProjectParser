using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VbProjectParserCore.Data.Exceptions;
using VbProjectParserCore.Compression;
using VbProjectParserCore.Data.Base;
using VbProjectParserCore.Data.Base.Attributes;

namespace VbProjectParserCore.Data._PROJECTINFORMATION
{
    public class PROJECTCONSTANTS : DataBase
    {
        [MustBe((ushort)0x000C)]
        public readonly ushort Id;

        [Range(uint.MinValue, 1015)]
        public readonly uint SizeOfConstants;

        [LengthMustEqualMember("SizeOfConstants")]
        public readonly byte[] Constants;

        [MustBe((ushort)0x003C)]
        public readonly ushort Reserved;

        public readonly uint SizeOfConstantsUnicode;

        [LengthMustEqualMember("SizeOfConstantsUnicode")]
        [ValidateWith("ValidateCompareConstants")]
        public readonly byte[] ConstantsUnicode;

        protected readonly PROJECTINFORMATION parent;

        public PROJECTCONSTANTS(PROJECTINFORMATION parent, XlBinaryReader Data)
        {
            this.parent = parent;

            Id = Data.ReadUInt16();
            SizeOfConstants = Data.ReadUInt32();
            Constants = Data.ReadBytes(SizeOfConstants);
            Reserved = Data.ReadUInt16();
            SizeOfConstantsUnicode = Data.ReadUInt32();
            ConstantsUnicode = Data.ReadBytes(SizeOfConstantsUnicode);

            Validate();
        }

        protected ValidationResult ValidateCompareConstants(object ValidationObject, MemberInfo member)
        {
            if (!GetConstantsAsString().Equals(GetConstantsUnicodeAsString()))
            {
                var ex = new ArgumentException("ConstantsUnicode (string) did not equals Constants (string)", "ConstantsUnicode");
                return new ValidationResult(ex);
            }

            return new ValidationResult();
        }

        public string GetConstantsAsString(Encoding encoding)
        {
            return encoding.GetString(Constants);
        }

        public string GetConstantsAsString(PROJECTCODEPAGE Codepage)
        {
            return GetConstantsAsString(Codepage.GetEncoding());
        }

        public string GetConstantsAsString(PROJECTINFORMATION ProjectInformation)
        {
            return GetConstantsAsString(ProjectInformation.CodePageRecord);
        }

        public string GetConstantsAsString()
        {
            return GetConstantsAsString(parent);
        }

        public string GetConstantsUnicodeAsString()
        {
            return Encoding.Unicode.GetString(ConstantsUnicode);
        }
    }
}
