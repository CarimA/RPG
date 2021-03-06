﻿using System;
using System.CodeDom;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Engine.Assets.AssetLoaders;
using PhotoVs.Engine.ECS;
using PhotoVs.Engine.Graphics;
using PhotoVs.Engine.Graphics.Filters;
using PhotoVs.Engine.Graphics.Particles;
using PhotoVs.Logic.Filters;
using PhotoVs.Logic.Mechanics.Components;
using PhotoVs.Logic.Mechanics.World;
using PhotoVs.Utils.Collections;
using PhotoVs.Utils.Extensions;

namespace PhotoVs.Logic.Mechanics
{
    public class DrawMap
    {
        private Random _random;

        private readonly IOverworld _overworld;
        private readonly VirtualResolution _virtualResolution;
        private readonly SpriteBatch _spriteBatch;
        private readonly IRenderer _renderer;
        private readonly IAssetLoader _assetLoader;
        private readonly Camera _camera;
        private readonly Primitive _primitive;

        private readonly TilemapFilter _tileMapFilter;
        private readonly WaterReflectionFilter _waterReflectionFilter;
        private readonly WaterFilter _waterFilter;
        private readonly WaterDisplacementFilter _waterDisplacementFilter;
        private readonly DayNightFilter _dayNightFilter;

        private readonly Texture2D _superTileset;
        private readonly Texture2D _tempTileMap;

        private RenderTarget2D _tilemapRenderTarget;
        private RenderTarget2D _waterTarget;
        private RenderTarget2D _reflectionTarget;
        private RenderTarget2D _displaceTarget;
        private RenderTarget2D _maskTarget;
        private RenderTarget2D _fringeTarget;
        private RenderTarget2D _finalTarget;
        private RenderTarget2D _colorGradeTarget;

        private GameObject _windTrail;
        private float _nextWindTrail;

        public DrawMap(GameState gameState, IOverworld overworld, VirtualResolution virtualResolution, SpriteBatch spriteBatch, IRenderer renderer, IAssetLoader assetLoader, Camera camera, Primitive primitive, GameDate gameDate)
        {
            _random = new Random();

            _overworld = overworld;
            _virtualResolution = virtualResolution;
            _spriteBatch = spriteBatch;
            _renderer = renderer;
            _assetLoader = assetLoader;
            _camera = camera;
            _primitive = primitive;

            _tempTileMap = _assetLoader.Get<Texture2D>("debug/outmap.png");
            _superTileset = _assetLoader.Get<Texture2D>("debug/supertileset.png");

            _tileMapFilter = new TilemapFilter(_renderer, _spriteBatch,
                _assetLoader.Get<Effect>("shaders/tilemap.fx"), 16);
            _tileMapFilter.SetSuperTileset(_superTileset);
            _tileMapFilter.SetTileMapTexture(_tempTileMap);

            _waterReflectionFilter = new WaterReflectionFilter(_renderer, _spriteBatch,
                _assetLoader.Get<Effect>("shaders/water_reflection.fx"));

            _waterFilter = new WaterFilter(_renderer, _spriteBatch, _camera,
                _assetLoader.Get<Effect>("shaders/water.fx"), 
                _assetLoader.Get<Texture2D>("ui/noise.png"),
                _assetLoader.Get<Texture2D>("ui/noise2.png"), _virtualResolution);

            _waterDisplacementFilter = new WaterDisplacementFilter(_renderer, _spriteBatch, _virtualResolution,
                _assetLoader.Get<Effect>("shaders/displace.fx"),
                _assetLoader.Get<Texture2D>("ui/displacement.png"),
                _assetLoader.Get<Texture2D>("ui/displacement2.png"));

            _dayNightFilter = new DayNightFilter(_renderer, _spriteBatch, gameDate,
                _assetLoader.Get<Effect>("shaders/average.fx"),
                _assetLoader.Get<Effect>("shaders/color.fx"),
                new List<(float, Texture2D)>()
                {
                    (0, assetLoader.Get<Texture2D>("luts/daycycle1.png")),
                    (0.1875f, assetLoader.Get<Texture2D>("luts/daycycle1.png")),
                    (0.208334f, assetLoader.Get<Texture2D>("luts/daycycle2.png")),
                    (0.30208333f, assetLoader.Get<Texture2D>("luts/daycycle3.png")),
                    (0.41667f, assetLoader.Get<Texture2D>("luts/daycycle4.png")),
                    (0.46f, assetLoader.Get<Texture2D>("luts/daycycle5.png")),
                    (0.54f, assetLoader.Get<Texture2D>("luts/daycycle5.png")),
                    (0.58333f, assetLoader.Get<Texture2D>("luts/daycycle6.png")),
                    (0.6875f, assetLoader.Get<Texture2D>("luts/daycycle7.png")),
                    (0.7708333f, assetLoader.Get<Texture2D>("luts/daycycle8.png")),
                    (0.82291667f, assetLoader.Get<Texture2D>("luts/daycycle9.png")),
                    (0.90625f, assetLoader.Get<Texture2D>("luts/daycycle10.png"))
                });

            CreateRenderTargets();

            _windTrail = new GameObject();
            CAnimation windTrailAnimation;
            _windTrail.Components.Add(new CPosition(Vector2.Zero));
            _windTrail.Components.Add(new CSprite(assetLoader.Get<Texture2D>("ui/windtrail.png"), Vector2.Zero));
            _windTrail.Components.Add(windTrailAnimation = new CAnimation() {Loop = false, OnComplete = OnComplete});
            windTrailAnimation.AddAnimation("effect", new List<AnimationFrame>()
            {
                new AnimationFrame(new Rectangle(0, 0, 512, 54), 0.07f),
                new AnimationFrame(new Rectangle(0, 64, 512, 54), 0.07f),
                new AnimationFrame(new Rectangle(0, 128, 512, 54), 0.07f),
                new AnimationFrame(new Rectangle(0, 192, 512, 54), 0.07f),
                new AnimationFrame(new Rectangle(0, 256, 512, 54), 0.07f),
                new AnimationFrame(new Rectangle(0, 320, 512, 54), 0.07f),
                new AnimationFrame(new Rectangle(0, 384, 512, 54), 0.07f),
                new AnimationFrame(new Rectangle(0, 448, 512, 54), 0.07f),
                new AnimationFrame(new Rectangle(0, 512, 512, 54), 0.07f),
            });
            _windTrail.Components.Add(new CFringeEntity { Opacity = 0.15f });

            gameState.Stage.GameObjects.Add(_windTrail);
        }


        private void CreateRenderTargets()
        {
            _tilemapRenderTarget = _renderer.CreateRenderTarget(_virtualResolution.MaxWidth, _virtualResolution.MaxHeight);
            _waterTarget = _renderer.CreateRenderTarget(_virtualResolution.MaxWidth, _virtualResolution.MaxHeight);
            _reflectionTarget = _renderer.CreateRenderTarget(_virtualResolution.MaxWidth, _virtualResolution.MaxHeight);
            _displaceTarget = _renderer.CreateRenderTarget(_virtualResolution.MaxWidth, _virtualResolution.MaxHeight);
            _maskTarget = _renderer.CreateRenderTarget(_virtualResolution.MaxWidth, _virtualResolution.MaxHeight);
            _fringeTarget = _renderer.CreateRenderTarget(_virtualResolution.MaxWidth, _virtualResolution.MaxHeight);
            _finalTarget = _renderer.CreateRenderTarget(_virtualResolution.MaxWidth, _virtualResolution.MaxHeight);
            _colorGradeTarget = _renderer.CreateRenderTarget(_virtualResolution.MaxWidth, _virtualResolution.MaxHeight);
        }

        [System(RunOn.Update)]
        public void UpdateWindTrailTime(GameTime gameTime, GameObjectList gameObjects)
        {
            _nextWindTrail -= gameTime.GetElapsedSeconds();
            if (_nextWindTrail <= 0f)
            {
                _windTrail.Components.Get<CPosition>().Position = _random.NextVector2(_camera.VisibleArea);
                _windTrail.Components.Get<CAnimation>().Play("effect");

                _nextWindTrail = float.MaxValue;
            }
        }

        private void OnComplete(string obj)
        {
            _nextWindTrail = _random.NextFloat(2f, 5f);
        }

        [System(RunOn.Draw)]
        public void DrawMapBuffers(GameTime gameTime, GameObjectList gameObjectList)
        {
            DrawMask(ref _maskTarget, gameTime);
            DrawFringe(ref _fringeTarget, gameTime);
        }

        [System(RunOn.Draw)]
        public void SetSubRenderer(GameTime gameTime, GameObjectList gameObjectList)
        {
            _renderer.RequestSubRenderer(_finalTarget);
        }

        [System(RunOn.Draw)]
        public void DrawMaskBuffer(GameTime gameTime, GameObjectList gameObjects)
        {
            _spriteBatch.Begin();
            _spriteBatch.Draw(_maskTarget, Vector2.Zero, Color.White);
            _spriteBatch.End();
        }

        [System(RunOn.Draw, typeof(CPosition), typeof(CSprite), typeof(CAnimation), typeof(CMaskEntity))]
        public void DrawMaskEntities(GameTime gameTime, GameObjectList gameObjects)
        {
            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp,
                transformMatrix: _camera.Transform);
            DrawMaskParticles(gameTime);
            DrawEntities<CMaskEntity>(gameObjects);
            _spriteBatch.End();
        }

        public void DrawMaskParticles(GameTime gameTime)
        {
            DrawParticles(gameTime, _overworld.GetMap().GetMaskEmitters(_camera));
        }

        [System(RunOn.Draw)]
        public void DrawFringeBuffer(GameTime gameTime, GameObjectList gameObjects)
        {
            _spriteBatch.Begin();
            _spriteBatch.Draw(_fringeTarget, Vector2.Zero, Color.White);
            _spriteBatch.End();
        }

        [System(RunOn.Draw, typeof(CPosition), typeof(CSprite), typeof(CAnimation), typeof(CFringeEntity))]
        public void DrawFringeEntities(GameTime gameTime, GameObjectList gameObjects)
        {
            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp,
                transformMatrix: _camera.Transform);
            DrawFringeParticles(gameTime);
            DrawEntities<CFringeEntity>(gameObjects);
            _spriteBatch.End();
        }

        public void DrawFringeParticles(GameTime gameTime)
        {
            DrawParticles(gameTime, _overworld.GetMap().GetFringeEmitters(_camera));
        }

        private void DrawEntities<T>(GameObjectList gameObjects) where T : COpacity
        {
            var mapObjects = gameObjects.All(typeof(CSprite), typeof(CAnimation), typeof(CPosition));
            foreach (var mapObject in mapObjects)
            {
                var sprite = mapObject.Components.Get<CSprite>();
                var animation = mapObject.Components.Get<CAnimation>();
                var position = mapObject.Components.Get<CPosition>();
                var opacity = 1f;
                
                // todo: figure out a nice way of handling opacity when entities have both
                if (mapObject.Components.TryGet(out T op))
                    opacity = op.Opacity != 0 ? op.Opacity : 1;

                var positionVec = position.Position;
                if (mapObject.Components.TryGet(out CSize size))
                    positionVec += (size.Size / 2);

                _spriteBatch.Draw(sprite.Texture, positionVec, 
                    animation.GetFrame(), Color.White * opacity, 0, sprite.Origin, Vector2.One, SpriteEffects.None, 0f);
            }
        }

        [System(RunOn.Draw)]
        public void FinaliseRender(GameTime gameTime, GameObjectList gameObjects)
        {
            _renderer.RelinquishSubRenderer();

            _dayNightFilter.Filter(ref _colorGradeTarget, _spriteBatch, _finalTarget);

            _spriteBatch.Begin();
            _spriteBatch.Draw(_colorGradeTarget, Vector2.Zero, Color.White);
            _spriteBatch.End();
        }

        public void DrawMask(ref RenderTarget2D target, GameTime gameTime)
        {
            var mask = DrawTilemap(true);

            _waterFilter.Update(gameTime);
            _waterFilter.Filter(ref _waterTarget, _spriteBatch, mask);
            _waterReflectionFilter.Filter(ref _reflectionTarget, _spriteBatch, mask);

            _waterDisplacementFilter.SetTextures(_reflectionTarget);
            _waterDisplacementFilter.Update(gameTime);
            _waterDisplacementFilter.Filter(ref _displaceTarget, _spriteBatch, _displaceTarget);

            _renderer.RequestSubRenderer(target);
            _spriteBatch.Begin();
            _spriteBatch.Draw(_waterTarget, Vector2.Zero, Color.White);
            _spriteBatch.Draw(_displaceTarget, Vector2.Zero, Color.White * 0.45f);
            _spriteBatch.End();
            _renderer.RelinquishSubRenderer();
        }

        public void DrawFringe(ref RenderTarget2D target, GameTime gameTime)
        {
            var fringe = DrawTilemap(false);

            _renderer.RequestSubRenderer(target);
            _spriteBatch.Begin();
            _spriteBatch.Draw(fringe, Vector2.Zero, Color.White);
            _spriteBatch.End();
            _renderer.RelinquishSubRenderer();
        }

        [System(RunOn.Draw, typeof(CPosition), typeof(CCollisionBound))]
        public void DebugDrawCollidableEntities(GameTime gameTime, GameObjectList gameObjects)
        {
            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp,
                transformMatrix: _camera.Transform);
            
            DrawQuadtree(_overworld.GetMap().MaskEmitters);

            foreach (var collider in _overworld.GetMap().GetCollisions(_camera))
                DebugDraw(collider);

            foreach (var script in _overworld.GetMap().GetScripts(_camera))
                DebugDraw(script);

            foreach (var zone in _overworld.GetMap().GetZones(_camera))
                DebugDraw(zone);

            foreach (var emitter in _overworld.GetMap().GetMaskEmitters(_camera))
                DrawEmitter(emitter);

            foreach (var emitter in _overworld.GetMap().GetFringeEmitters(_camera))
                DrawEmitter(emitter);

            foreach (var gameObject in gameObjects)
                DebugDraw(gameObject);

            _spriteBatch.End();
        }

        private void DrawParticles(GameTime gameTime, IEnumerable<IEmitter> emitters)
        {
            foreach (var emitter in emitters)
            {
                emitter.Update(gameTime);
                emitter.Draw(_spriteBatch, gameTime);
            }
        }

        private RenderTarget2D DrawTilemap(bool isMask)
        {
            _tileMapFilter.SetSuperTileset(_superTileset);
            _tileMapFilter.SetTransform(_camera.Transform);
            _tileMapFilter.SetMode(isMask 
                ? TilemapFilter.Mode.Mask
                : TilemapFilter.Mode.Fringe);
            _tileMapFilter.Filter(ref _tilemapRenderTarget, _spriteBatch, _tempTileMap);
            return _tilemapRenderTarget;
        }

        private void DrawQuadtree<T>(Quadtree<T> quadtree)
        {
            _primitive.DrawBox(quadtree.Boundaries.X, quadtree.Boundaries.Y, quadtree.Boundaries.Width,
                quadtree.Boundaries.Height, Color.White * 0.5f);

            foreach (var child in quadtree.GetChildren())
                DrawQuadtree(child);
        }

        private void DrawEmitter(IEmitter emitter)
        {
            _primitive.DrawBox(emitter.Boundaries.X, emitter.Boundaries.Y, emitter.Boundaries.Width,
                emitter.Boundaries.Height, Color.HotPink);
        }

        private void DebugDraw(GameObject entity)
        {
            var position = entity.Components.Get<CPosition>();
            var bounds = entity.Components.Get<CCollisionBound>();
            var boxColor = entity.Components.Has<CSolid>() ? Color.Green : Color.White;

            // draw a box around the inflated boundaries
            var iTL = position.Position + bounds.InflatedBounds.TopLeft;
            _primitive.DrawBox(iTL.X, iTL.Y, bounds.InflatedBounds.Width, bounds.InflatedBounds.Height, Color.Red);

            // draw a box around the actual boundaries
            var bTL = position.Position + bounds.Bounds.TopLeft;
            _primitive.DrawBox(bTL.X, bTL.Y, bounds.Bounds.Width, bounds.Bounds.Height, Color.Yellow);

            // draw the points
            _primitive.DrawPolygon(position.Position, bounds.Points, boxColor);

            // draw the center
            var bCentre = position.Position + bounds.Center;
            _primitive.DrawBox(bCentre.X, bCentre.Y, 1, 1, Color.White);
        }
    }
}
