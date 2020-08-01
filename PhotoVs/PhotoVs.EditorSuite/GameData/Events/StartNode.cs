using System.Drawing;

namespace PhotoVs.EditorSuite.GameData.Events
{
    public sealed class StartNode : FunctionNode
    {
        // todo: attach trigger/condition data to start node

        public override string Name => "Start";
        public override Color HeaderColor => Color.CadetBlue;
        public override bool CanAddInputs => true;
        public override bool CanAddOutputs => false;
        public override bool IsRemovable => false;

        public override void AddInput()
        {
            Inputs.Add(new NodeInputLabel
            {
                Name = "Trigger",
                Requires = typeof(Trigger),
                Removable = true,
                CanConnect = false,
                Parent = this
            });

            base.AddInput();
        }

        public override void Assign()
        {
            Outputs.Add(new NodeOutputLabel
            {
                Name = "Out",
                Provides = typeof(FunctionNode),
                Removable = false,
                Parent = this
            });

            base.Assign();
        }
    }
}