using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hedgehog
{
    class StartData
    {
        public int X, Y;
        public StartData(int X, int Y) { this.X = X; this.Y = Y; }
    }

    public enum TileType
    {
        empty,
        solid,
        reflector,
        spring,
        platform,
        spikes
    }

    //ПЛИТКИ
    class Tile
    {
        public int index;
        public TileType type;
        public Vector2 scale;
        public Vector2 offset;
        public float rot;
        public bool overlap;
        public bool standOn;
        public bool isSolid;
        public bool spikes;
        public bool eventActive;
        public MonsterType monsterStart;

        public Tile(int Index, TileType Type)
        {
            index = Index;
            type = Type;
            scale = Vector2.One;
            monsterStart = MonsterType.None; 
        }

        public void Clear()
        {
            index = 0;
            type = TileType.empty;
            scale = Vector2.One;
            offset = Vector2.Zero;
            rot = 0;
            overlap = false; standOn = false; spikes = false; isSolid = false;
            eventActive = false; monsterStart = MonsterType.None;
        }
    }//Плитки

    //PROCESSED TILE
    class ProcessedTile
    {
        public Vector2 pos;
        public Vector2 scale;
        public float rot;
        public Rectangle rect;

        public void Add(Vector2 position, Rectangle srtRect, float rotation, Vector2 Size)
        {
            pos = position;
            rect = srtRect;
            rot = rotation;
            scale = Size;
        }
    }
}
