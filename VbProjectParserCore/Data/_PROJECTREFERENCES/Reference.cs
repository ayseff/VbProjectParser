using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VbProjectParserCore.Data._PROJECTREFERENCES.ReferenceRecords;
using VbProjectParserCore.Data.Exceptions;
using VbProjectParserCore.Compression;
using VbProjectParserCore.Data._PROJECTINFORMATION;
using VbProjectParserCore.Data.Base;

namespace VbProjectParserCore.Data._PROJECTREFERENCES
{
    public class REFERENCE : DataBase
    {
        public readonly REFERENCENAME NameRecord;
        public readonly object ReferenceRecord;

        public REFERENCE(PROJECTINFORMATION ProjectInformation, XlBinaryReader Data)
        {
            NameRecord = new REFERENCENAME(ProjectInformation, Data);

            var peek = Data.PeekUInt16();

            if (peek == 0x002F)
            {
                ReferenceRecord = new REFERENCECONTROL(ProjectInformation, Data);
            }
            else if (peek == 0x0033)
            {
                // todo: Test this, documentation says 0x0033 is REFERENCECONTROL too but this seems odd
                ReferenceRecord = new REFERENCEORIGINAL(Data);
            }
            else if (peek == 0x000D)
            {
                ReferenceRecord = new REFERENCEREGISTERED(Data);
            }
            else if (peek == 0x000E)
            {
                ReferenceRecord = new REFERENCEPROJECT(ProjectInformation, Data);
            }
            else
            {
                throw new WrongValueException("peek", peek, "0x002F, 0x0033, 0x000D or 0x000E");
            }

            Validate();
        }
    }
}
