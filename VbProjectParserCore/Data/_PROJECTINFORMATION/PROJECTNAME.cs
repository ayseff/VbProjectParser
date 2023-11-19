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
    public class PROJECTNAME : DataBase
    {
        [MustBe((ushort)0x0004)]
        public readonly ushort Id;

        [Range(1, 128)]
        public readonly uint Size;

        [LengthMustEqualMember("Size")]
        public readonly byte[] ProjectName;

        protected readonly PROJECTINFORMATION parent;

        public PROJECTNAME(PROJECTINFORMATION parent, XlBinaryReader Data)
        {
            this.parent = parent;

            Id = Data.ReadUInt16();
            Size = Data.ReadUInt32();
            ProjectName = Data.ReadBytes((int)Size);

            Validate();
        }

        /// <summary>
        /// Returns a string of the Project Name, where encoding needs to be the project's
        /// Encoding (as specified in the PROJECTCODEPAGE record)
        /// </summary>
        public string GetProjectNameAsString(Encoding encoding)
        {
            return encoding.GetString(ProjectName);
        }

        public string GetProjectNameAsString(PROJECTCODEPAGE Codepage)
        {
            return GetProjectNameAsString(Codepage.GetEncoding());
        }

        public string GetProjectNameAsString(PROJECTINFORMATION ProjectInformation)
        {
            return GetProjectNameAsString(ProjectInformation.CodePageRecord);
        }

        public string GetProjectNameAsString()
        {
            return GetProjectNameAsString(parent);
        }
    }
}
