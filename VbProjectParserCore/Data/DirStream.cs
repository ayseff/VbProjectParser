using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VbProjectParserCore.Compression;
using VbProjectParserCore.Data._PROJECTINFORMATION;
using VbProjectParserCore.Data._PROJECTMODULES;
using VbProjectParserCore.Data._PROJECTREFERENCES;

namespace VbProjectParserCore.Data
{
    public class DirStream
    {
        public readonly PROJECTINFORMATION InformationRecord;
        public readonly PROJECTREFERENCES ReferencesRecord;
        public readonly PROJECTMODULES ModulesRecord;

        public DirStream(XlBinaryReader Data)
        {
            InformationRecord = new PROJECTINFORMATION(Data);
            ReferencesRecord = new PROJECTREFERENCES(InformationRecord, Data);
            ModulesRecord = new PROJECTMODULES(InformationRecord, Data);
        }
    }
}
