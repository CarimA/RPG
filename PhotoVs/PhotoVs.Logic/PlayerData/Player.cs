﻿using System;
using Microsoft.Xna.Framework;
using PhotoVs.Engine;
using PhotoVs.Engine.ECS.GameObjects;
using PhotoVs.Logic.Mechanics.Input;
using PhotoVs.Logic.Mechanics.Input.Components;
using PhotoVs.Logic.Mechanics.Movement.Components;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Newtonsoft.Json;

namespace PhotoVs.Logic.PlayerData
{
    public class PlayerData
    {
        public string CurrentMap { get; set; }
        public CPosition Position { get; }
        public double TimePlayed { get; set; }
        private Dictionary<string, bool> Flags { get; }
        private Dictionary<string, IComparable> Variables { get; }

        public PlayerData()
        {
            Position = new CPosition();
            Flags = new Dictionary<string, bool>();
            Variables = new Dictionary<string, IComparable>();
        }

        public Stream Save()
        {
            var json = JsonConvert.SerializeObject(this);
            using var ms = new MemoryStream(UTF8Encoding.Default.GetBytes(json));
            return ms;
        }

        public static PlayerData Load(Stream stream)
        {
            using var reader = new StreamReader(stream, Encoding.UTF8);
            var json = reader.ReadToEnd();
            var obj = JsonConvert.DeserializeObject<PlayerData>(json);
            return obj;
        }

        public static PlayerData New()
        {
            var playerData = new PlayerData();

            return playerData;
        }

        public void SetFlag(string flag, bool value)
        {
            if (!Flags.ContainsKey(flag))
            {
                Flags.Add(flag, value);
            }
            else
            {
                Flags[flag] = value;
            }
        }

        public bool GetFlag(string flag)
        {
            if (Flags.TryGetValue(flag, out var v))
            {
                return v;
            }

            SetFlag(flag, false);
            return false;
        }

        public void SetVariable(string variable, IComparable value)
        {
            if (!Variables.ContainsKey(variable))
            {
                Variables.Add(variable, value);
            }
            else
            {
                Variables[variable] = value;
            }
        }

        public IComparable GetVariable(string variable)
        {
            if (Variables.TryGetValue(variable, out var v))
            {
                return v;
            }

            SetVariable(variable, null);
            return null;
        }

        public IComparable<T> GetVariable<T>(string variable)
        {
            if (Variables.TryGetValue(variable, out var v))
            {
                return (IComparable<T>)v;
            }

            SetVariable(variable, default);
            return default;
        }
    }


    public class Player : GameObject
    {
        private readonly CInput _input;
        public PlayerData PlayerData { get; }

        private readonly float RunSpeed = 150f;
        private readonly float WalkSpeed = 95f;
        public bool CanMove { get; set; }

        public GameInput Input => _input.Input;

        public Player(Services services)
        {
            var config = services.Get<Config>();

            Name = "Player";

            PlayerData = new PlayerData();
            Components.Add(PlayerData.Position);

            CanMove = true;

            _input = new CInput(new GameInput(
                PlayerIndex.One,
                config.ControlsGamepad,
                config.ControlsKeyboard));

            Components.Add(_input);
            Components.Add(CCollisionBound.Circle(16, 8));
            Components.Add(new CSize { Size = new Vector2(32, 32) });
        }

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