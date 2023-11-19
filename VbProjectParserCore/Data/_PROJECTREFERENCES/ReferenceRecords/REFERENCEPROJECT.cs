using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VbProjectParserCore.Data.Exceptions;
using VbProjectParserCore.Compression;
using VbProjectParserCore.Data._PROJECTINFORMATION;
using VbProjectParserCore.Data.Base;
using VbProjectParserCore.Data.Base.Attributes;

namespace VbProjectParserCore.Data._PROJECTREFERENCES.ReferenceRecords
{
    public class REFERENCEPROJECT : DataBase
    {
        [MustBe((ushort)0x000E)]
        public readonly ushort Id;

        public readonly uint Size;

        public readonly uint SizeOfLibidAbsolute;

        [LengthMustEqualMember("SizeOfLibidAbsolute")]
        public readonly byte[] LibidAbsolute;

        public readonly uint SizeOfLibidRelative;

        [LengthMustEqualMember("SizeOfLibidRelative")]
        public readonly byte[] LibidRelative;

        [ValueMustEqualMember("ProjectInformation.VersionRecord.VersionMajor")]
        public readonly uint MajorVersion;

        [ValueMustEqualMember("ProjectInformation.VersionRecord.VersionMinor")]
        public readonly ushort MinorVersion;

        protected readonly PROJECTINFORMATION ProjectInformation;

        public REFERENCEPROJECT(PROJECTINFORMATION ProjectInformation, XlBinaryReader Data)
        {
            this.ProjectInformation = ProjectInformation;

            Id = Data.ReadUInt16();
            Size = Data.ReadUInt32();
            SizeOfLibidAbsolute = Data.ReadUInt32();
            LibidAbsolute = Data.ReadBytes(SizeOfLibidAbsolute);
            SizeOfLibidRelative = Data.ReadUInt32();
            LibidRelative = Data.ReadBytes(SizeOfLibidRelative);
            MajorVersion = Data.ReadUInt32();
            MinorVersion = Data.ReadUInt16();

            Validate();
        }

        protected ValidationResult ValidateMajorVersion(object ValidationObject, MemberInfo member)
        {
            throw new NotImplementedException();
            /*
            if(this.MajorVersion != this.ProjectInformation.VersionRecord.VersionMajor))
            {

            }*/
        }
    }
}
