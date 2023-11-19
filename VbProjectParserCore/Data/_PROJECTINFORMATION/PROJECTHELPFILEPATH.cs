using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VbProjectParserCore.Data;
using VbProjectParserCore.Data.Exceptions;
using VbProjectParserCore.Compression;
using VbProjectParserCore.Data.Base;
using VbProjectParserCore.Data.Base.Attributes;

namespace VbProjectParserCore.Data._PROJECTINFORMATION
{
    public class PROJECTHELPFILEPATH : DataBase
    {
        [MustBe((ushort)0x0006)]
        public readonly ushort Id;

        [Range(uint.MinValue, 260)]
        public readonly uint SizeOfHelpFile1;

        [LengthMustEqualMember("SizeOfHelpFile1")]
        public readonly byte[] HelpFile1;

        [MustBe((ushort)0x003D)]
        public readonly ushort Reserved;

        [ValueMustEqualMember("SizeOfHelpFile1")]
        public readonly uint SizeOfHelpFile2;

        [LengthMustEqualMember("SizeOfHelpFile2")]
        public readonly byte[] HelpFile2;

        protected PROJECTINFORMATION parent;

        public PROJECTHELPFILEPATH(PROJECTINFORMATION parent, XlBinaryReader Data)
        {
            this.parent = parent;

            Id = Data.ReadUInt16();
            SizeOfHelpFile1 = Data.ReadUInt32();
            HelpFile1 = Data.ReadBytes(SizeOfHelpFile1);
            Reserved = Data.ReadUInt16();
            SizeOfHelpFile2 = Data.ReadUInt32();
            HelpFile2 = Data.ReadBytes(SizeOfHelpFile2);

            Validate();
        }



        public string GetHelpFile1AsString(Encoding encoding)
        {
            return encoding.GetString(HelpFile1);
        }

        public string GetHelpFile1AsString(PROJECTCODEPAGE Codepage)
        {
            return GetHelpFile1AsString(Codepage.GetEncoding());
        }

        public string GetHelpFile1AsString(PROJECTINFORMATION ProjectInformation)
        {
            return GetHelpFile1AsString(ProjectInformation.CodePageRecord);
        }

        public string GetHelpFile1AsString()
        {
            return GetHelpFile1AsString(parent);
        }
    }
}
