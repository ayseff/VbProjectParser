using AbnfFrameworkCore.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VbProjectParserCore.Data.ABNF.Common
{
    public class CommonTokens
    {
        public static readonly Token NWLN = new RegexToken(CommonRegexPatterns._NWLN, "\r\n");
        public static readonly Token DQUOTE = new RegexToken(CommonRegexPatterns._DQUOTE, "\"");
    }

}
