﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoVs.Logic.Events.Plugins.Attributes
{
    public class FlagConditionAttribute : Attribute
    {
        public string Flag;
        public bool State;

        public FlagConditionAttribute(string flag, bool state)
        {
            Flag = flag;
            State = state;
        }
    }
}
