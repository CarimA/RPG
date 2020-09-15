using Microsoft.Xna.Framework;

namespace PhotoVs.Engine.Graphics.Filters
{
    public interface ITransformFilter : IFilter
    {
        void SetTransform(Matrix matrix);
    }
}