using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PhotoVs.Engine.Graphics
{
    public class PolygonPrimitive
    {
        private readonly BasicEffect _basicEffect;
        private readonly Color _color;
        private readonly GraphicsDevice _graphicsDevice;
        private int[] _indices;
        private VertexPositionColor[] _triangulatedVertices;
        private VertexPositionColor[] _vertices;

        public PolygonPrimitive(GraphicsDevice graphicsDevice, Color color)
        {
            _basicEffect = new BasicEffect(graphicsDevice);
            _graphicsDevice = graphicsDevice;

            _basicEffect.TextureEnabled = false;
            _basicEffect.VertexColorEnabled = true;
            _basicEffect.Alpha = 1f;
            _basicEffect.LightingEnabled = false;

            //_basicEffect.World = Matrix.Identity;
            //* Matrix.CreateTranslation(-(canvasSize.GetWidth() / 2),
            //   -(canvasSize.GetHeight() / 2), 0);
            //_basicEffect.View = Matrix.Identity;

            _color = color;
        }

        public void SetPoints(List<Vector2> points)
        {
            var count = points.Count;
            _vertices = new VertexPositionColor[count];
            for (var i = 0; i < count; i++)
            {
                _vertices[i].Position = new Vector3(points[i], 0);
                //new Vector3(new Vector2(points[i].X, 
                //_canvasSize.GetHeight() - points[i].Y), 0);
                _vertices[i].Color = _color;
            }

            Triangulate();
        }

        private void Triangulate()
        {
            var centre = CalculateCentre();
            SetupIndexes();

            for (var i = 0; i < _indices.Length; i++)
                SetupDrawableTriangle(_indices[i], centre);
        }

        private Vector3 CalculateCentre()
        {
            var centre = Vector3.Zero;
            foreach (var vertex in _vertices)
            {
                centre.X += vertex.Position.X;
                centre.Y += vertex.Position.Y;
            }

            return centre / _vertices.Length;
        }

        private void SetupIndexes()
        {
            _triangulatedVertices = new VertexPositionColor[_vertices.Length * 3];
            _indices = new int[_vertices.Length];

            for (var i = 1; i < _triangulatedVertices.Length; i += 3)
                _indices[i / 3] = i - 1;
        }

        private void SetupDrawableTriangle(int index, Vector3 centre)
        {
            _triangulatedVertices[index] = _vertices[index / 3];
            if (index / 3 != _vertices.Length - 1)
                _triangulatedVertices[index + 1] = _vertices[index / 3 + 1];
            else
                _triangulatedVertices[index + 1] = _vertices[0];

            _triangulatedVertices[index + 2].Position = centre;
            _triangulatedVertices[index + 2].Color = _color;
        }

        public void Draw()
        {
            if (_vertices == null)
                return;

            _basicEffect.Projection =
                Matrix.Identity
                * Matrix.CreateOrthographic(_graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height, 0,
                    1) //Matrix.CreateOrthographic(canvasSize.GetWidth(), canvasSize.GetHeight(), 0, 1)
                * Matrix.CreateScale(1, -1, 1)
                * Matrix.CreateTranslation(-1, 1, 0);

            foreach (var pass in _basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                _graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                    _triangulatedVertices, 0, _vertices.Length);
            }
        }
    }
}