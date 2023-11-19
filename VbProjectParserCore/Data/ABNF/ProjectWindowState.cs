using AbnfFrameworkCore.Tokens;
using AbnfFrameworkCore.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VbProjectParserCore.Data.ABNF
{
    public class ProjectWindowState
    {
        public ProjectWindow CodeWindow { get; set; }

        public ProjectWindow DesignerWindow { get; set; }

        public static void Setup(ISyntax Syntax)
        {
            Syntax
                .Entity<ProjectWindowState>()
                .Property(x => x.CodeWindow)
                .ByRegisteredTypes(typeof(ProjectWindow));

            Syntax
                .Entity<ProjectWindowState>()
                .Property(x => x.DesignerWindow)
                .ByRegisteredTypes(typeof(ProjectWindow))
                .WithPrefix(new LiteralToken(", "))
                .IsOptional();
        }
    }
}
