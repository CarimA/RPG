using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PhotoVs.Engine.Graphics.Filters
{
    public interface IUpdateFilter : IFilter
    {
        void Update(GameTime gameTime);
    }
}
