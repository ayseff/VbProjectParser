using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VbProjectParserCore.Compression;
using VbProjectParserCore.Data._PROJECTINFORMATION;
using VbProjectParserCore.Data.Base;
using VbProjectParserCore.Data.Base.Attributes;

namespace VbProjectParserCore.Data._PROJECTMODULES._MODULE
{
    /// <summary>
    /// Page 44
    /// </summary>
    public class MODULE : DataBase
    {
        public readonly MODULENAME NameRecord;

        [ValidateWith("ValidateNameRecords")]
        public readonly MODULENAMEUNICODE NameUnicodeRecord;

        public readonly MODULESTREAMNAME StreamNameRecord;

        public readonly MODULEDOCSTRING DocStringRecord;

        public readonly MODULEOFFSET OffsetRecord;

        public readonly MODULEHELPCONTEXT HelpContextRecord;

        public readonly MODULECOOKIE CookieRecord;

        public readonly MODULETYPE TypeRecord;

        public readonly MODULEREADONLY ReadOnlyRecord;

        public readonly MODULEPRIVATE PrivateRecord;

        [MustBe((ushort)0x002B)]
        public readonly ushort Terminator;

        [MustBe((uint)0x00000000)]
        public readonly uint Reserved;

        public bool IsPrivate => PrivateRecord != null;

        public bool IsReadOnly => ReadOnlyRecord != null;

        public MODULE(PROJECTINFORMATION ProjectInformation, XlBinaryReader Data)
        {
            NameRecord = new MODULENAME(ProjectInformation, Data);
            NameUnicodeRecord = new MODULENAMEUNICODE(Data);
            StreamNameRecord = new MODULESTREAMNAME(ProjectInformation, Data);
            DocStringRecord = new MODULEDOCSTRING(Data);
            OffsetRecord = new MODULEOFFSET(Data);
            HelpContextRecord = new MODULEHELPCONTEXT(Data);
            CookieRecord = new MODULECOOKIE(Data);
            TypeRecord = new MODULETYPE(Data);

            if (Data.PeekUInt16() == 0x0025)
            {
                ReadOnlyRecord = new MODULEREADONLY(Data);
            }

            if (Data.PeekUInt16() == 0x0028)
            {
                PrivateRecord = new MODULEPRIVATE(Data);
            }

            Terminator = Data.ReadUInt16();
            Reserved = Data.ReadUInt32();

            Validate();
        }

        protected ValidationResult ValidateNameRecords(object ValidationObject, MemberInfo member)
        {
            if (!NameRecord.GetModuleNameAsString().Equals(NameUnicodeRecord.GetModuleNameUnicodeAsString()))
            {
                var ex = new ArgumentException("NameUnicodeRecord.ModuleNameUnicode (string) was not equal NameRecord.ModuleName (string)", "NameUnicordRecord");
                return new ValidationResult(ex);
            }

            return new ValidationResult();
        }
    }
}
