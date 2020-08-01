using System.Collections.Generic;
using System.Drawing;

namespace PhotoVs.EditorSuite.GameData.Events
{
    public sealed class DialogueNode : FunctionNode
    {
        public override string Name => "Say Dialogue";
        public override Color HeaderColor => Color.Violet;
        public override bool CanAddInputs => false;
        public override bool CanAddOutputs => false;

        public override void Assign()
        {
            Inputs.AddRange(new List<NodeInputLabel>
            {
                new NodeInputLabel
                {
                    Name = "In",
                    Requires = typeof(FunctionNode),
                    Removable = false,
                    CanConnect = true,
                    Parent = this
                },
                new NodeInputLabel
                {
                    Name = "Speaker",
                    Removable = false,
                    CanConnect = false,
                    Parent = this,
                    DataType = typeof(Actor)
                },

                new NodeInputLabel
                {
                    Name = "In",
                    Removable = false,
                    CanConnect = false,
                    Parent = this,
                    DataType = typeof(TextEntry)
                }
            });
        }
    }
}