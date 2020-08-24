using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Effect = Microsoft.Xna.Framework.Graphics.Effect;
using SamplerState = Microsoft.Xna.Framework.Graphics.SamplerState;

namespace PhotoVs.Engine.Graphics.Filters
{
    public class TilemapFilter : ITransformFilter
    {
        public enum Mode
        {
            Mask,
            Fringe,
            Both
        }

        private readonly IRenderer _renderer;
        private readonly SpriteBatch _spriteBatch;
        private readonly Effect _effect;
        private EffectParameter _superTilesetParam;
        private EffectParameter _tileMapParam;
        private EffectParameter _tileSizeParam;
        private EffectParameter _inverseSuperTilesetSizeParam;
        private EffectParameter _tileMapSizeParam;
        private EffectPass _effectPass;

        private int _tileSize;
        private Matrix _transform;

        private Mode _mode;

        public TilemapFilter(IRenderer renderer, SpriteBatch spriteBatch, Effect effect, int tileSize)
        {
            _renderer = renderer;
            _spriteBatch = spriteBatch;
            _effect = effect;

            _tileMapParam = _effect.Parameters["Texture"];
            _superTilesetParam = _effect.Parameters["texIndex"];
            _tileSizeParam = _effect.Parameters["tileSize"];
            _inverseSuperTilesetSizeParam = _effect.Parameters["inverseIndexTexSize"];
            _tileMapSizeParam = _effect.Parameters["mapSize"];
            _effectPass = _effect.CurrentTechnique.Passes[0];

            _tileSizeParam.SetValue(new Vector2(tileSize, tileSize));
            _tileSize = tileSize;
        }

        public void SetSuperTileset(Texture2D superTileset)
        {
            _superTilesetParam.SetValue(superTileset);
            _inverseSuperTilesetSizeParam.SetValue(
                new Vector2(1f / superTileset.Width, 1f / superTileset.Height));
        }

        public void SetTileMapTexture(Texture2D tileMap)
        {
            _tileMapParam.SetValue(tileMap);
            _tileMapSizeParam.SetValue(new Vector2(
                tileMap.Width * _tileSize, tileMap.Height * _tileSize));
        }

        public void SetTransform(Matrix matrix)
        {
            _transform = matrix;
        }

        public void SetMode(Mode mode)
        {
            _mode = mode;
        }

        public void Filter(ref RenderTarget2D renderTarget, SpriteBatch spriteBatch, Texture2D inputTexture)
        {
            _renderer.RequestSubRenderer(renderTarget);

            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp,
                transformMatrix: _transform);
            _effectPass.Apply();

            switch (_mode)
            {
                case Mode.Mask:
                    DrawTilemap(spriteBatch, inputTexture, true);
                    break;
                case Mode.Fringe:
                    DrawTilemap(spriteBatch, inputTexture, false);
                    break;
                case Mode.Both:
                    DrawTilemap(spriteBatch, inputTexture, true);
                    DrawTilemap(spriteBatch, inputTexture, false);
                    break;
            }
                
            _spriteBatch.End();

            _renderer.RelinquishSubRenderer();
        }

        private void DrawTilemap(SpriteBatch spriteBatch, Texture2D inputTexture, bool isMask)
        {
            spriteBatch.Draw(inputTexture,
                new Rectangle(0, 0, inputTexture.Width / 2 * _tileSize, inputTexture.Height * _tileSize),
                new Rectangle(isMask ? 0 : inputTexture.Width / 2, 0, inputTexture.Width / 2, inputTexture.Height),
                Color.White);
        }
    }
}
