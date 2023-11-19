using AbnfFrameworkCore.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VbProjectParserCore.Data.ABNF.Enums;

namespace VbProjectParserCore.Data.ABNF
{
    public class ProjectReference
    {
        /// <summary>
        /// The kind of the project reference
        /// </summary>
        public ProjectKind ProjectKind { get; set; }

        /// <summary>
        /// The path to the VBA project
        /// </summary>
        public string ProjectPath { get; set; }

        public static void Setup(ISyntax Syntax)
        {

        }
    }
}
