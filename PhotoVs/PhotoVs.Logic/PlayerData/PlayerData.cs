using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using PhotoVs.Logic.Mechanics.Components;

namespace PhotoVs.Logic.PlayerData
{
    public class PlayerData
    {
        public PlayerData()
        {
            Position = new CPosition(Vector2.Zero);
            Flags = new Dictionary<string, bool>();
            Variables = new Dictionary<string, IComparable>();
        }

        public string CurrentMap { get; set; }
        public CPosition Position { get; }
        public double TimePlayed { get; set; }
        private Dictionary<string, bool> Flags { get; }
        private Dictionary<string, IComparable> Variables { get; }

        public Stream Save()
        {
            var json = JsonConvert.SerializeObject(this);
            using var ms = new MemoryStream(Encoding.Default.GetBytes(json));
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
                Flags.Add(flag, value);
            else
                Flags[flag] = value;
        }

        public bool GetFlag(string flag)
        {
            if (Flags.TryGetValue(flag, out var v)) return v;

            SetFlag(flag, false);
            return false;
        }

        public void SetVariable(string variable, IComparable value)
        {
            if (!Variables.ContainsKey(variable))
                Variables.Add(variable, value);
            else
                Variables[variable] = value;
        }

        public IComparable GetVariable(string variable)
        {
            if (Variables.TryGetValue(variable, out var v)) return v;

            SetVariable(variable, null);
            return null;
        }

        public IComparable<T> GetVariable<T>(string variable)
        {
            if (Variables.TryGetValue(variable, out var v)) return (IComparable<T>) v;

            SetVariable(variable, default);
            return default;
        }
    }
}