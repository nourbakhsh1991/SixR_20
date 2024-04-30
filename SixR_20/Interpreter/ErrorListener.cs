using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;

namespace SixR_20.Interpreter
{
    class ErrorListener : BaseErrorListener
    {
        public static string AllErrors = "";
        public override void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg,
            RecognitionException e)
        {
            AllErrors += msg + " in Line: " + line + "\r\n";
        }
    }
}
