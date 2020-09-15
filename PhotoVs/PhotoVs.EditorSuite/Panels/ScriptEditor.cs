using System;
using System.Drawing;
using PhotoVs.EditorSuite.GameData;
using ScintillaNET;

namespace PhotoVs.EditorSuite.Panels
{
    public partial class ScriptEditor : Editor<Script>
    {
        private const string KEYWORDS =
            "and break do else elseif end for function if in local nil not or repeat return then until while" +
            " false true" + " goto";

        private const string FUNCTIONS =
            "assert collectgarbage dofile error _G getmetatable ipairs loadfile next pairs pcall print rawequal rawget rawset setmetatable tonumber tostring type _VERSION xpcall string table math coroutine io os debug" +
            " getfenv gcinfo load loadlib loadstring require select setfenv unpack _LOADED LUA_PATH _REQUIREDNAME package rawlen package bit32 utf8 _ENV";

        private int maxLineNumberCharLength;

        public ScriptEditor() : base()
        {
            InitializeComponent();

            // autosave after 2 seconds of not typing
            save.Interval = 2 * 1000;
            save.Tick += (sender, args) =>
            {
                Project.Save(false, true);
                save.Stop();
            };

            SetCodeEditor();

            codeEdit.Text = Instance.Code;
        }

        private void SetCodeEditor()
        {
            codeEdit.TextChanged += (sender, args) =>
            {
                Instance.Code = codeEdit.Text;

                save.Stop();
                save.Start();
            };

            // Set the lexer
            codeEdit.Lexer = Lexer.Lua;

            // Extracted from the Lua Scintilla lexer and SciTE .properties file

            var alphaChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var numericChars = "0123456789";
            var accentedChars = "ŠšŒœŸÿÀàÁáÂâÃãÄäÅåÆæÇçÈèÉéÊêËëÌìÍíÎîÏïÐðÑñÒòÓóÔôÕõÖØøÙùÚúÛûÜüÝýÞþßö";

            // Configuring the default style with properties
            // we have common to every lexer style saves time.
            codeEdit.StyleResetDefault();
            codeEdit.Styles[Style.Default].Font = "Consolas";
            codeEdit.Styles[Style.Default].Size = 10;
            codeEdit.StyleClearAll();

            // Configure the Lua lexer styles
            codeEdit.Styles[Style.Lua.Default].ForeColor = Color.Silver;
            codeEdit.Styles[Style.Lua.Comment].ForeColor = Color.Green;
            codeEdit.Styles[Style.Lua.CommentLine].ForeColor = Color.Green;
            codeEdit.Styles[Style.Lua.Number].ForeColor = Color.Olive;
            codeEdit.Styles[Style.Lua.Word].ForeColor = Color.Blue;
            codeEdit.Styles[Style.Lua.Word2].ForeColor = Color.BlueViolet;
            codeEdit.Styles[Style.Lua.Word3].ForeColor = Color.DarkSlateBlue;
            codeEdit.Styles[Style.Lua.Word4].ForeColor = Color.DarkSlateBlue;
            codeEdit.Styles[Style.Lua.String].ForeColor = Color.Red;
            codeEdit.Styles[Style.Lua.Character].ForeColor = Color.Red;
            codeEdit.Styles[Style.Lua.LiteralString].ForeColor = Color.Red;
            codeEdit.Styles[Style.Lua.StringEol].BackColor = Color.Pink;
            codeEdit.Styles[Style.Lua.Operator].ForeColor = Color.Purple;
            codeEdit.Styles[Style.Lua.Preprocessor].ForeColor = Color.Maroon;
            //codeEdit.Styles[Style.BraceLight].BackColor = Color.LightGray;
            //codeEdit.Styles[Style.BraceLight].ForeColor = Color.BlueViolet;
            //codeEdit.Styles[Style.BraceBad].ForeColor = Color.Red;
            codeEdit.Lexer = Lexer.Lua;
            codeEdit.WordChars = alphaChars + numericChars + accentedChars;

            // Console.WriteLine(scintilla.DescribeKeywordSets());

            // Keywords
            codeEdit.SetKeywords(0, KEYWORDS);
            // Basic Functions
            codeEdit.SetKeywords(1, FUNCTIONS);
            // String Manipulation & Mathematical
            codeEdit.SetKeywords(2,
                "string.byte string.char string.dump string.find string.format string.gsub string.len string.lower string.rep string.sub string.upper table.concat table.insert table.remove table.sort math.abs math.acos math.asin math.atan math.atan2 math.ceil math.cos math.deg math.exp math.floor math.frexp math.ldexp math.log math.max math.min math.pi math.pow math.rad math.random math.randomseed math.sin math.sqrt math.tan" +
                " string.gfind string.gmatch string.match string.reverse string.pack string.packsize string.unpack table.foreach table.foreachi table.getn table.setn table.maxn table.pack table.unpack table.move math.cosh math.fmod math.huge math.log10 math.modf math.mod math.sinh math.tanh math.maxinteger math.mininteger math.tointeger math.type math.ult" +
                " bit32.arshift bit32.band bit32.bnot bit32.bor bit32.btest bit32.bxor bit32.extract bit32.replace bit32.lrotate bit32.lshift bit32.rrotate bit32.rshift" +
                " utf8.char utf8.charpattern utf8.codes utf8.codepoint utf8.len utf8.offset");
            // Input and Output Facilities and System Facilities
            codeEdit.SetKeywords(3,
                "coroutine.create coroutine.resume coroutine.status coroutine.wrap coroutine.yield io.close io.flush io.input io.lines io.open io.output io.read io.tmpfile io.type io.write io.stdin io.stdout io.stderr os.clock os.date os.difftime os.execute os.exit os.getenv os.remove os.rename os.setlocale os.time os.tmpname" +
                " coroutine.isyieldable coroutine.running io.popen module package.loaders package.seeall package.config package.searchers package.searchpath" +
                " require package.cpath package.loaded package.loadlib package.path package.preload");

            // Instruct the lexer to calculate folding
            codeEdit.SetProperty("fold", "1");
            codeEdit.SetProperty("fold.compact", "1");

            // Configure a margin to display folding symbols
            codeEdit.Margins[2].Type = MarginType.Symbol;
            codeEdit.Margins[2].Mask = Marker.MaskFolders;
            codeEdit.Margins[2].Sensitive = true;
            codeEdit.Margins[2].Width = 20;

            // Set colors for all folding markers
            for (var i = 25; i <= 31; i++)
            {
                codeEdit.Markers[i].SetForeColor(SystemColors.ControlLightLight);
                codeEdit.Markers[i].SetBackColor(SystemColors.ControlDark);
            }

            // Configure folding markers with respective symbols
            codeEdit.Markers[Marker.Folder].Symbol = MarkerSymbol.BoxPlus;
            codeEdit.Markers[Marker.FolderOpen].Symbol = MarkerSymbol.BoxMinus;
            codeEdit.Markers[Marker.FolderEnd].Symbol = MarkerSymbol.BoxPlusConnected;
            codeEdit.Markers[Marker.FolderMidTail].Symbol = MarkerSymbol.TCorner;
            codeEdit.Markers[Marker.FolderOpenMid].Symbol = MarkerSymbol.BoxMinusConnected;
            codeEdit.Markers[Marker.FolderSub].Symbol = MarkerSymbol.VLine;
            codeEdit.Markers[Marker.FolderTail].Symbol = MarkerSymbol.LCorner;

            // Enable automatic folding
            codeEdit.AutomaticFold = AutomaticFold.Show | AutomaticFold.Click | AutomaticFold.Change;

            codeEdit.CharAdded += CodeEditOnCharAdded;

            codeEdit.Margins[0].Type = MarginType.RightText;
            codeEdit.Margins[0].Width = 35;

            codeEdit.Insert += CodeEditOnChange;
            codeEdit.Delete += CodeEditOnChange;
            codeEdit.TextChanged += CodeEditOnTextChanged;
        }

        private void CodeEditOnTextChanged(object sender, EventArgs e)
        {
            // Did the number of characters in the line number display change?
            // i.e. nnn VS nn, or nnnn VS nn, etc...
            var maxLineNumberCharLength = codeEdit.Lines.Count.ToString().Length;
            if (maxLineNumberCharLength == this.maxLineNumberCharLength)
                return;

            // Calculate the width required to display the last line number
            // and include some padding for good measure.
            const int padding = 2;
            codeEdit.Margins[0].Width =
                codeEdit.TextWidth(Style.LineNumber, new string('9', maxLineNumberCharLength + 1)) + padding;
            this.maxLineNumberCharLength = maxLineNumberCharLength;
        }

        private void CodeEditOnChange(object sender, ModificationEventArgs e)
        {
            // Only update line numbers if the number of lines changed
            if (e.LinesAdded != 0)
                UpdateLineNumbers(codeEdit.LineFromPosition(e.Position));
        }

        private void UpdateLineNumbers(int startingAtLine)
        {
            // Starting at the specified line index, update each
            // subsequent line margin text with a hex line number.
            for (var i = startingAtLine; i < codeEdit.Lines.Count; i++)
            {
                codeEdit.Lines[i].MarginStyle = Style.LineNumber;
                codeEdit.Lines[i].MarginText = i.ToString();
            }
        }

        private void CodeEditOnCharAdded(object sender, CharAddedEventArgs e)
        {
            // Find the word start
            var currentPos = codeEdit.CurrentPosition;
            var wordStartPos = codeEdit.WordStartPosition(currentPos, true);

            // Display the autocompletion list
            var lenEntered = currentPos - wordStartPos;
            if (lenEntered > 0)
                if (!codeEdit.AutoCActive)
                    codeEdit.AutoCShow(lenEntered, KEYWORDS);
        }
    }
}