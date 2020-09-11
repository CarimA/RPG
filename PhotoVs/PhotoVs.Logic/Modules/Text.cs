using System;
using MoonSharp.Interpreter;
using PhotoVs.Engine.Scripting;
using PhotoVs.Logic.Text;

namespace PhotoVs.Logic.Modules
{
    public class Text
    {
        private readonly ITextDatabase _textDatabase;

        public Text(IInterpreter<Closure> interpreter, ITextDatabase textDatabase)
        {
            _textDatabase = textDatabase;

            interpreter.AddFunction("Text", (Func<string, string>) GetText);
        }

        private string GetText(string id)
        {
            return _textDatabase.GetText(id);
        }
    }
}