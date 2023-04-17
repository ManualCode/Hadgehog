using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hedgehog
{
    static public class Conv
    {
        static public Point GetTileCoord(Vector2 position, ref Vector2 offset)
        {
            Point tile;
            tile.X = (int)position.X / 64;
            tile.Y = (int)position.Y / 64;
            offset.X = (int)(position.X - tile.X * 64);
            offset.Y = (int)(position.Y - tile.Y * 64);
            return tile;
        } 

        static public Point GetTileCoord(Vector2 position)
        {
            Point tile;
            tile.X = (int)position.X / 64;
            tile.Y = (int)position.Y / 64;
            return tile;
        }

        static public Vector2 WorldToScreen(Vector2 worldPosition)
        {
            return worldPosition - Game1.CamPos + Game1.screenCenter;
        }

        //BOX WORLD TO SCREEN
        static public Rectangle BboxWorldToScreen(Vector4 bbox)
        {
            bbox.X = bbox.X - Game1.CamPos.X + Game1.screenCenter.X;
            bbox.Y = bbox.Y - Game1.CamPos.Y + Game1.screenCenter.Y;
            bbox.Z = bbox.Z - Game1.CamPos.X + Game1.screenCenter.X;
            bbox.W = bbox.W - Game1.CamPos.Y + Game1.screenCenter.Y;
            Rectangle sBox = new Rectangle((int)bbox.X, (int)bbox.Y, (int)(bbox.Z - bbox.X), (int)(bbox.W - bbox.Y));
            return sBox;
        }

        static public Vector2 TileToWorld(Point tileLoc) { return new Vector2(tileLoc.X * 64, tileLoc.Y * 64); }
    }
}
