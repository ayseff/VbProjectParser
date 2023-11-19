using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VbProjectParserCore.Data.Exceptions;
using VbProjectParserCore.Compression;
using VbProjectParserCore.Data._PROJECTINFORMATION;
using VbProjectParserCore.Data.Base;
using VbProjectParserCore.Data.Base.Attributes;

namespace VbProjectParserCore.Data._PROJECTREFERENCES
{
    /// <summary>
    /// Page 38
    /// </summary>
    public class REFERENCENAME : DataBase
    {
        [MustBe((ushort)0x0016)]
        public readonly ushort Id;

        public readonly uint SizeOfName;

        public readonly byte[] Name;

        [MustBe((ushort)0x003E)]
        public readonly ushort Reserved;

        public readonly uint SizeOfNameUnicode;

        public readonly byte[] NameUnicode;

        // TODO: RefProjectName, RefLibararyName, Reserved, SizeOfNameUnicode, NameUnicode

        protected readonly PROJECTINFORMATION ProjectInformation;

        public REFERENCENAME(PROJECTINFORMATION ProjectInformation, XlBinaryReader Data)
        {
            this.ProjectInformation = ProjectInformation;

            Id = Data.ReadUInt16();
            SizeOfName = Data.ReadUInt32();
            Name = Data.ReadBytes(SizeOfName);
            Reserved = Data.ReadUInt16();
            SizeOfNameUnicode = Data.ReadUInt32();
            NameUnicode = Data.ReadBytes(SizeOfNameUnicode);

            Validate();
        }

        protected override void Validate()
        {
            base.Validate();
            ValidateName();
            ValidateNameUnicode();
            ValidateCompareNames();
        }

        protected void ValidateName()
        {
            if (Name.Length != SizeOfName)
            {
                throw new WrongValueException("Name.Length", Name.Length, SizeOfName);
            }
        }

        protected void ValidateNameUnicode()
        {
            if (NameUnicode.Length != SizeOfNameUnicode)
            {
                throw new WrongValueException("NameUnicode", NameUnicode.Length, SizeOfNameUnicode);
            }
        }

        protected void ValidateCompareNames()
        {
            if (!GetNameAsString().Equals(GetNameUnicodeAsString()))
            {
                throw new WrongValueException("NameUnicode vs. Name", NameUnicode, Name);
            }
        }

        public string GetNameAsString(Encoding encoding)
        {
            return encoding.GetString(Name);
        }

        public string GetNameAsString(PROJECTCODEPAGE Codepage)
        {
            return GetNameAsString(Codepage.GetEncoding());
        }

        public string GetNameAsString(PROJECTINFORMATION ProjectInformation)
        {
            return GetNameAsString(ProjectInformation.CodePageRecord);
        }

        public string GetNameAsString()
        {
            return GetNameAsString(ProjectInformation);
        }

        public string GetNameUnicodeAsString()
        {
            return Encoding.Unicode.GetString(NameUnicode);
        }
    }
}
