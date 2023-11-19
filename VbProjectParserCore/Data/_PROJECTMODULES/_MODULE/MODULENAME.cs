using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VbProjectParserCore.Compression;
using VbProjectParserCore.Data._PROJECTINFORMATION;
using VbProjectParserCore.Data.Base;
using VbProjectParserCore.Data.Base.Attributes;

namespace VbProjectParserCore.Data._PROJECTMODULES._MODULE
{
    public class MODULENAME : DataBase
    {
        [AutoRead(1)]
        [MustBe((ushort)0x0019)]
        public readonly ushort Id;

        [AutoRead(2)]
        public readonly uint SizeOfModuleName;

        [AutoRead(3, "SizeOfModuleName")]
        [LengthMustEqualMember("SizeOfModuleName")]
        public readonly byte[] ModuleName;

        protected readonly PROJECTINFORMATION ProjectInformation;

        public MODULENAME(PROJECTINFORMATION ProjectInformation, XlBinaryReader Data)
            : base(Data)
        {
            this.ProjectInformation = ProjectInformation;

            //this.Id = Data.ReadUInt16();
            //this.SizeOfModuleName = Data.ReadUInt32();
            //this.ModuleName = Data.ReadBytes(this.SizeOfModuleName);

            Validate();
        }

        public string GetModuleNameAsString(Encoding encoding)
        {
            return encoding.GetString(ModuleName);
        }

        public string GetModuleNameAsString(PROJECTCODEPAGE Codepage)
        {
            return GetModuleNameAsString(Codepage.GetEncoding());
        }

        public string GetModuleNameAsString(PROJECTINFORMATION ProjectInformation)
        {
            return GetModuleNameAsString(ProjectInformation.CodePageRecord);
        }

        public string GetModuleNameAsString()
        {
            return GetModuleNameAsString(ProjectInformation);
        }
    }
}
