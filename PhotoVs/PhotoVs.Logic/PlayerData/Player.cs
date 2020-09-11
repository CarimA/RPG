using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhotoVs.Engine.Assets.AssetLoaders;
using PhotoVs.Engine.ECS;
using PhotoVs.Logic.Mechanics.Components;

namespace PhotoVs.Logic.PlayerData
{
    public class Player : GameObject
    {
        private readonly float RunSpeed = 400f;
        private readonly float WalkSpeed = 140f;

        public Player(Config config, IAssetLoader assetLoader)
        {
            Name = "Player";

            PlayerData = new PlayerData();
            Components.Add(PlayerData.Position);

            CanMove = true;

            Components.Add(CCollisionBound.Circle(8, 6));
            Components.Add(new CSize(new Vector2(16, 16)));
            Components.Add(Input = new CInputState());
            Components.Add(new CKeyboard(config.ControlsKeyboard));
            Components.Add(new CController(PlayerIndex.One, config.ControlsGamepad, config.Deadzone / 100f));
            Components.Add(new CInputPriority(InputPriority.GamePad));
            Components.Add(new CTarget());

            Components.Add(new CSprite(assetLoader.Get<Texture2D>("sprites/player.png"), new Vector2(16, 32)));
            
            var animation = new CAnimation();

            animation.AddAnimation("idle-down", new List<AnimationFrame>()
            {
                new AnimationFrame(new Rectangle(0, 0, 32, 48), 0f)
            });

            animation.AddAnimation("walk-down", new List<AnimationFrame>()
            {
                new AnimationFrame(new Rectangle(32, 0, 32, 48), 0.135f),
                new AnimationFrame(new Rectangle(64, 0, 32, 48), 0.135f),
                new AnimationFrame(new Rectangle(96, 0, 32, 48), 0.135f),
                new AnimationFrame(new Rectangle(128, 0, 32, 48), 0.135f),
                new AnimationFrame(new Rectangle(160, 0, 32, 48), 0.135f),
                new AnimationFrame(new Rectangle(192, 0, 32, 48), 0.135f)
            });

            animation.AddAnimation("run-down", new List<AnimationFrame>()
            {
                new AnimationFrame(new Rectangle(32, 0, 32, 48), 0.08f),
                new AnimationFrame(new Rectangle(64, 0, 32, 48), 0.055f),
                new AnimationFrame(new Rectangle(224, 0, 32, 48), 0.125f),
                new AnimationFrame(new Rectangle(128, 0, 32, 48), 0.08f),
                new AnimationFrame(new Rectangle(160, 0, 32, 48), 0.055f),
                new AnimationFrame(new Rectangle(256, 0, 32, 48), 0.125f)
            });


            animation.AddAnimation("idle-up", new List<AnimationFrame>()
            {
                new AnimationFrame(new Rectangle(0, 48, 32, 48), 0f)
            });

            animation.AddAnimation("walk-up", new List<AnimationFrame>()
            {
                new AnimationFrame(new Rectangle(32, 48, 32, 48), 0.135f),
                new AnimationFrame(new Rectangle(64, 48, 32, 48), 0.135f),
                new AnimationFrame(new Rectangle(96, 48, 32, 48), 0.135f),
                new AnimationFrame(new Rectangle(128, 48, 32, 48), 0.135f),
                new AnimationFrame(new Rectangle(160, 48, 32, 48), 0.135f),
                new AnimationFrame(new Rectangle(192, 48, 32, 48), 0.135f)
            });

            animation.AddAnimation("run-up", new List<AnimationFrame>()
            {
                new AnimationFrame(new Rectangle(32, 48, 32, 48), 0.08f),
                new AnimationFrame(new Rectangle(64, 48, 32, 48), 0.055f),
                new AnimationFrame(new Rectangle(224, 48, 32, 48), 0.125f),
                new AnimationFrame(new Rectangle(128, 48, 32, 48), 0.08f),
                new AnimationFrame(new Rectangle(160, 48, 32, 48), 0.055f),
                new AnimationFrame(new Rectangle(256, 48, 32, 48), 0.125f)
            });


            animation.AddAnimation("idle-right", new List<AnimationFrame>()
            {
                new AnimationFrame(new Rectangle(0, 96, 32, 48), 0f)
            });

            animation.AddAnimation("walk-right", new List<AnimationFrame>()
            {
                new AnimationFrame(new Rectangle(32, 96, 32, 48), 0.135f),
                new AnimationFrame(new Rectangle(64, 96, 32, 48), 0.135f),
                new AnimationFrame(new Rectangle(96, 96, 32, 48), 0.135f),
                new AnimationFrame(new Rectangle(128, 96, 32, 48), 0.135f),
                new AnimationFrame(new Rectangle(160, 96, 32, 48), 0.135f),
                new AnimationFrame(new Rectangle(192, 96, 32, 48), 0.135f)
            });

            animation.AddAnimation("run-right", new List<AnimationFrame>()
            {
                new AnimationFrame(new Rectangle(32, 96, 32, 48), 0.08f),
                new AnimationFrame(new Rectangle(64, 96, 32, 48), 0.055f),
                new AnimationFrame(new Rectangle(224, 96, 32, 48), 0.125f),
                new AnimationFrame(new Rectangle(128, 96, 32, 48), 0.08f),
                new AnimationFrame(new Rectangle(160, 96, 32, 48), 0.055f),
                new AnimationFrame(new Rectangle(256, 96, 32, 48), 0.125f)
            });


            animation.AddAnimation("idle-left", new List<AnimationFrame>()
            {
                new AnimationFrame(new Rectangle(0, 144, 32, 48), 0f)
            });

            animation.AddAnimation("walk-left", new List<AnimationFrame>()
            {
                new AnimationFrame(new Rectangle(32, 144, 32, 48), 0.135f),
                new AnimationFrame(new Rectangle(64, 144, 32, 48), 0.135f),
                new AnimationFrame(new Rectangle(96, 144, 32, 48), 0.135f),
                new AnimationFrame(new Rectangle(128, 144, 32, 48), 0.135f),
                new AnimationFrame(new Rectangle(160, 144, 32, 48), 0.135f),
                new AnimationFrame(new Rectangle(192, 144, 32, 48), 0.135f)
            });

            animation.AddAnimation("run-left", new List<AnimationFrame>()
            {
                new AnimationFrame(new Rectangle(32, 144, 32, 48), 0.08f),
                new AnimationFrame(new Rectangle(64, 144, 32, 48), 0.055f),
                new AnimationFrame(new Rectangle(224, 144, 32, 48), 0.125f),
                new AnimationFrame(new Rectangle(128, 144, 32, 48), 0.08f),
                new AnimationFrame(new Rectangle(160, 144, 32, 48), 0.055f),
                new AnimationFrame(new Rectangle(256, 144, 32, 48), 0.125f)
            });


            animation.SetDefaultAnimation("idle-down");
            animation.Play("idle-down");

            Components.Add(animation);
        }

        public CInputState Input { get; }
        public PlayerData PlayerData { get; }
        public bool CanMove { get; set; }

        public float CurrentSpeed(bool runToggled)
        {
            return runToggled ? RunSpeed : WalkSpeed;
        }

        public void LockMovement()
        {
            CanMove = false;
        }

        public void UnlockMovement()
        {
            CanMove = true;
        }
    }
}