using System.Collections.Generic;
using System.Drawing;

namespace PhotoVs.EditorSuite.GameData.Events
{
    public sealed class FlowControlNode : FunctionNode
    {
        public override string Name => "Flow Control";
        public override Color HeaderColor => Color.LimeGreen;
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
                    Name = "A",
                    Requires = typeof(DataNode),
                    Removable = false,
                    CanConnect = true,
                    Parent = this
                },
                new NodeInputLabel
                {
                    Name = "B",
                    Requires = typeof(DataNode),
                    Removable = false,
                    CanConnect = true,
                    Parent = this
                }
            });

            Outputs.AddRange(new List<NodeOutputLabel>
            {
                new NodeOutputLabel
                {
                    Name = "A = B",
                    Provides = typeof(FunctionNode),
                    Removable = false,
                    Parent = this
                },
                new NodeOutputLabel
                {
                    Name = "A ≠ B",
                    Provides = typeof(FunctionNode),
                    Removable = false,
                    Parent = this
                },
                new NodeOutputLabel
                {
                    Name = "A < B",
                    Provides = typeof(FunctionNode),
                    Removable = false,
                    Parent = this
                },
                new NodeOutputLabel
                {
                    Name = "A > B",
                    Provides = typeof(FunctionNode),
                    Removable = false,
                    Parent = this
                }
            });

            base.Assign();
        }
    }
}