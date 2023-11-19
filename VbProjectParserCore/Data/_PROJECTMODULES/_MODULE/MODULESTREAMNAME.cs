using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VbProjectParserCore.Data;
using VbProjectParserCore.Compression;
using VbProjectParserCore.Data._PROJECTINFORMATION;
using VbProjectParserCore.Data.Base;
using VbProjectParserCore.Data.Base.Attributes;

namespace VbProjectParserCore.Data._PROJECTMODULES._MODULE
{
    public class MODULESTREAMNAME : DataBase
    {
        [MustBe((ushort)0x001A)]
        public readonly ushort Id;

        public readonly uint SizeOfStreamName;

        [LengthMustEqualMember("SizeOfStreamName")]
        public readonly byte[] StreamName;

        [MustBe((ushort)0x0032)]
        public readonly ushort Reserved;

        public readonly uint SizeOfStreamNameUnicode;

        [LengthMustEqualMember("SizeOfStreamNameUnicode")]
        public readonly byte[] StreamNameUnicode;

        protected readonly PROJECTINFORMATION ProjectInformation;

        public MODULESTREAMNAME(PROJECTINFORMATION ProjectInformation, XlBinaryReader Data)
        {
            this.ProjectInformation = ProjectInformation;

            Id = Data.ReadUInt16();
            SizeOfStreamName = Data.ReadUInt32();
            StreamName = Data.ReadBytes(SizeOfStreamName);
            Reserved = Data.ReadUInt16();
            SizeOfStreamNameUnicode = Data.ReadUInt32();
            StreamNameUnicode = Data.ReadBytes(SizeOfStreamNameUnicode);

            Validate();
        }

        public string GetStreamNameAsString(Encoding encoding)
        {
            return encoding.GetString(StreamName);
        }

        public string GetStreamNameAsString(PROJECTCODEPAGE Codepage)
        {
            return GetStreamNameAsString(Codepage.GetEncoding());
        }

        public string GetStreamNameAsString(PROJECTINFORMATION ProjectInformation)
        {
            return GetStreamNameAsString(ProjectInformation.CodePageRecord);
        }

        public string GetStreamNameAsString()
        {
            return GetStreamNameAsString(ProjectInformation);
        }

        public string GetStreamNameUnicodeAsString()
        {
            return Encoding.Unicode.GetString(StreamNameUnicode);
        }
    }
}
