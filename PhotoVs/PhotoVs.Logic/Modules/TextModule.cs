using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoVs.Engine.Scripting;
using PhotoVs.Logic.Text;

namespace PhotoVs.Logic.Modules
{
    public class TextModule : Module
    {
        private TextDatabase _textDatabase;

        public TextModule(TextDatabase textDatabase)
        {
            _textDatabase = textDatabase;
        }

        public override void DefineApi(MoonSharpInterpreter interpreter)
        {
            if (interpreter == null) throw new ArgumentNullException(nameof(interpreter));

            interpreter.AddFunction("Text", (Func<string, string>)GetText);

            base.DefineApi(interpreter);
        }

        private string GetText(string id)
        {
            return _textDatabase.GetText(id);
        }
    }
}
