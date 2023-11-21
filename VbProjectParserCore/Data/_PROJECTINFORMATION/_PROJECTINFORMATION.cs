using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VbProjectParserCore.Compression;
using VbProjectParserCore.Data.Base;

namespace VbProjectParserCore.Data._PROJECTINFORMATION;

/// <summary>
/// Page 31
/// </summary>
public class PROJECTINFORMATION : DataBase
{
    public readonly PROJECTSYSKIND SysKindRecord;
    public readonly PROJECTLCID LcidRecord;
    public readonly PROJECTLCIDINVOKE LcidInvokeRecord;
    public readonly PROJECTCODEPAGE CodePageRecord;
    public readonly PROJECTNAME NameRecord;
    public readonly PROJECTDOCSTRING DocStringRecord;
    public readonly PROJECTHELPFILEPATH HelpFilePathRecord;
    public readonly PROJECTHELPCONTEXT HelpContextRecord;
    public readonly PROJECTLIBFLAGS LibFlagsRecord;
    public readonly PROJECTVERSION VersionRecord;
    public readonly PROJECTCONSTANTS ConstantsRecord;

    public PROJECTINFORMATION(XlBinaryReader Data)
    {
        SysKindRecord = new PROJECTSYSKIND(Data);
        LcidRecord = new PROJECTLCID(Data);
        LcidInvokeRecord = new PROJECTLCIDINVOKE(Data);
        CodePageRecord = new PROJECTCODEPAGE(Data);
        NameRecord = new PROJECTNAME(this, Data);
        DocStringRecord = new PROJECTDOCSTRING(this, Data);
        HelpFilePathRecord = new PROJECTHELPFILEPATH(this, Data);
        HelpContextRecord = new PROJECTHELPCONTEXT(Data);
        LibFlagsRecord = new PROJECTLIBFLAGS(Data);
        VersionRecord = new PROJECTVERSION(Data);
        ConstantsRecord = new PROJECTCONSTANTS(this, Data);

        Validate();
    }
}
