using AbnfFrameworkCore.Tokens;
using AbnfFrameworkCore.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VbProjectParserCore.Data.ABNF.Common;

namespace VbProjectParserCore.Data.ABNF
{
    // p 26
    public class HostExtenders
    {
        public IList<HostExtenderRef> HostExtenderRef { get; set; }

        public HostExtenders()
        {
            HostExtenderRef = new List<HostExtenderRef>();
        }

        public static void Setup(ISyntax Syntax)
        {
            Syntax
                .Entity<HostExtenders>()
                .EnumerableProperty(x => x.HostExtenderRef)
                .ByRegisteredTypes(typeof(HostExtenderRef))
                .WithPrefix(new LiteralToken("[Host Extender Info]") + CommonTokens.NWLN);
        }
    }
}
