using System;
using System.Collections.Generic;
using System.Drawing;
using PhotoVs.EditorSuite.Events;

namespace PhotoVs.EditorSuite
{
    public class GameData
    {
        public GameProperties GameProperties { get; }
        public Dictionary<string, Script> Scripts { get; }
        public List<Graph> Events { get; }

        public GameData()
        {
            GameProperties = new GameProperties();
            Scripts = new Dictionary<string, Script>();
            Events = new List<Graph>();
        }
    }
    
    public class GameProperties
    {
        // the name of the game, duh
        public string Name { get; set; }

        // the key has the name of the language in english (eg. French)
        // the value has the name of the language in its own language (eg. Francais)
        public Dictionary<string, string> Languages { get; }

        public GameProperties()
        {
            Languages = new Dictionary<string, string>();
        }
    }

    public class Script
    {
        public string Code { get; set; }
    }

    public class ProjectData
    {
        public string Name { get; set; }
        public DefaultSoundEffects DefaultSoundEffects { get; }

        public Dictionary<string, TextEntry> Strings { get; }
        public List<Actor> Actors { get; }
        public List<string> Scripts { get; }
        public Dictionary<string, Type> Flags { get; }

        public ProjectData()
        {
            Strings = new Dictionary<string, TextEntry>();
            DefaultSoundEffects = new DefaultSoundEffects();
            Actors = new List<Actor>();
            Scripts = new List<string>();
            Flags = new Dictionary<string, Type>();
        }
    }


    public class TextEntry
    {
        public Dictionary<string, string> LocalisedText;

        public TextEntry()
        {
            LocalisedText = new Dictionary<string, string>();
        }
    }

    public class DefaultSoundEffects
    {
        public string Accept;
        public string Cancel;
        public string Save;
        public string Load;
    }

    public class Actor
    {
        public string NameId;
        public Sprite Sprite;
        public Dictionary<Emotion, Portrait> Portrait; // dictionary used for different emotions/states/etc
        public string Notes;

        public Actor()
        {
            Portrait = new Dictionary<Emotion, Portrait>();
        }
    }

    public enum Emotion
    {
        Neutral,
        Happy
    }

    public class Sprite
    {
        public string Texture;
    }

    public class Portrait
    {
        public string Texture;
        public List<Rectangle> LeftEyeSource; // sources hold a list of the positions in the textures for animating purposes
        public List<Rectangle> RightEyeSource;
        public List<Rectangle> MouthSource;
        public Rectangle LeftEye;
        public Rectangle RightEye;
        public Rectangle Mouth;

        public Portrait()
        {
            LeftEyeSource = new List<Rectangle>();
            RightEyeSource = new List<Rectangle>();
            MouthSource = new List<Rectangle>();
        }
    }

}
