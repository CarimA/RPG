using System;

namespace PhotoVs.Logic.Events.Plugins.Attributes
{
    public class VariableConditionAttribute : Attribute
    {
        public string Variable { get; }
        public Equality Equality { get; }
        public IComparable Value { get; }

        public VariableConditionAttribute(string variable, Equality equality, IComparable value)
        {
            Variable = variable;
            Equality = equality;
            Value = value;
        }
    }

    public enum Equality
    {
        Equals,
        GreaterThan,
        GreaterThanOrEquals,
        LessThan,
        LessThanOrEquals,
        Not
    }
}
