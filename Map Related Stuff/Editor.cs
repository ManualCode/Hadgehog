using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Hedgehog
{
    class Editor
    {
        Map mp;
        Tile[,] tiles;
        Sheet[] sheet;
        Input inp;
        string timeSaved = "-";
        bool showColliders;

        //CONSTRUCT
        public Editor(Map map, Input inpt, Sheet[] sht)
        {
            mp = map;
            tiles = mp.tiles;
            inp = inpt;
            sheet = sht;
        }

        //UPDATE
        public void Update()
        {
            mp.scrollOffset = Vector2.Zero;
            bool shiftDown = inp.shiftDown;
            if(inp.KeyPress(Keys.Right) || shiftDown && inp.KeyDown(Keys.Right)) { mp.loc.X++; Game1.backGroundPos.X--; }
            if (inp.KeyPress(Keys.Left) || shiftDown && inp.KeyDown(Keys.Left)) { mp.loc.X--; Game1.backGroundPos.X++; }
            if (inp.KeyPress(Keys.Down) || shiftDown && inp.KeyDown(Keys.Down)) { mp.loc.Y++; }
            if (inp.KeyPress(Keys.Up) || shiftDown && inp.KeyDown(Keys.Up)) { mp.loc.Y--; }

            //PREVENT OUT OF BOUNDS
            if(mp.loc.X >= Map.TILESWIDE) { mp.loc.X = Map.TILESWIDE - 1; Game1.backGroundPos.X++; }
            if (mp.loc.X< 0) { mp.loc.X = 0; Game1.backGroundPos.X--; }
            if (mp.loc.Y >= Map.TILESHIGH) { mp.loc.Y = Map.TILESHIGH - 1; }
            if (mp.loc.X < 0) { mp.loc.Y = 0; }

            //DLETE TILE
            if (inp.KeyPress(Keys.Delete) || inp.KeyPress(Keys.Back)) mp.DeleteTile();

            //SET COLLIDERS
            if (inp.KeyDown(Keys.Insert)) mp.SetType();

            //ADD TILE   TODO:
            if (inp.KeyPress(Keys.Q)) mp.AddTile(1);
            if (inp.KeyPress(Keys.W)) mp.AddTile(2);
            if (inp.KeyPress(Keys.E)) mp.AddTile(3);

            //GET WORLD POS OF TILE (to edit)
            Vector2 worldCoord = Conv.TileToWorld(mp.loc);

            //PLACE PLAYER
            if (inp.KeyPress(Keys.M))
                mp.startData.X = mp.loc.X; mp.startData.Y = mp.loc.Y;

            //SHOW HELPERS (ie: colliders)
            if (inp.KeyPress(Keys.F12)) showColliders = !showColliders;

            //EDIT A TILE
            if (tiles[mp.loc.X, mp.loc.Y].index > 0)
            {
                if (inp.KeyDown(Keys.OemPeriod)) tiles[mp.loc.X, mp.loc.Y].rot += 1.0f;
                if (inp.KeyDown(Keys.OemComma)) tiles[mp.loc.X, mp.loc.Y].rot -= 1.0f;
                if (inp.KeyDown(Keys.OemPlus)) tiles[mp.loc.X, mp.loc.Y].scale += new Vector2(0.01f, 0.01f);
                if (inp.KeyDown(Keys.OemMinus)) tiles[mp.loc.X, mp.loc.Y].scale -= new Vector2(0.01f, 0.01f);
                if (inp.KeyDown(Keys.NumPad8)) tiles[mp.loc.X, mp.loc.Y].offset.Y--;
                if (inp.KeyDown(Keys.NumPad2)) tiles[mp.loc.X, mp.loc.Y].offset.Y++;
                if (inp.KeyDown(Keys.NumPad4)) tiles[mp.loc.X, mp.loc.Y].offset.X--;
                if (inp.KeyDown(Keys.NumPad6)) tiles[mp.loc.X, mp.loc.Y].offset.X++;
                if (inp.KeyDown(Keys.PageUp)) tiles[mp.loc.X, mp.loc.Y].overlap = true;
                if (inp.KeyDown(Keys.PageDown)) tiles[mp.loc.X, mp.loc.Y].overlap = false;

                //reset tile modifications:
                if (inp.KeyPress(Keys.Home))
                {
                    tiles[mp.loc.X, mp.loc.Y].offset = sheet[tiles[mp.loc.X, mp.loc.Y].index].offset;
                    tiles[mp.loc.X, mp.loc.Y].rot = 0;
                    tiles[mp.loc.X, mp.loc.Y].scale = Vector2.One;
                }
            }
            //SAVE LEVEL MAP
            if (shiftDown && inp.KeyPress(Keys.D4))
            {
                SaveLevel(Game1.LevelName);
            }

            //LOAD LEVEL MAP
            if (shiftDown && inp.KeyPress(Keys.D1))
                if (File.Exists(Game1.LevelName))
                {
                    LoadLevel(Game1.LevelName);
                }
        }
        public void DrawLocators(SpriteBatch spriteBatch, Texture2D tilesImage, Vector2 screenCenter)
        {
            //show the tile location
            spriteBatch.Draw(tilesImage, screenCenter, new Rectangle(284, 0, 37, 64), new Color(100, 100, 100, 100), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            //show player starting location if visible:
            if(mp.startData.X > mp.a1 && mp.startData.X < mp.a2 && mp.startData.Y > mp.b1 && mp.startData.Y < mp.b2)
            {
                float x = mp.sx + 64.0f *(mp.startData.X - mp.a1);
                float y = mp.sy + 64.0f * (mp.startData.Y - mp.b1);
                spriteBatch.Draw(tilesImage, new Vector2(x, y), new Rectangle(284, 0, 37, 64), Color.Yellow, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }

            //MONSTER LOCATORS:
            mp.DrawTiles(showColliders, editMode: true); // show monster start locations 
        }


        //DRAW INSTRUCTIONS
        public void DrawInstructions(SpriteBatch spriteBatch, Texture2D farBackground, SpriteFont font, int screenH)
        {
            spriteBatch.Draw(farBackground, new Rectangle(0, 0, 200, screenH),  // темный прямоугольник (что бы легче было читать)
                new Rectangle(0, 0, 1, 1), new Color(0, 0, 0, 150));
            spriteBatch.DrawString(font, "A-Z = add tile", new Vector2(10, 10), Color.LimeGreen);
            spriteBatch.DrawString(font, "del = delete (set empty)", new Vector2(10, 30), Color.LimeGreen);
            spriteBatch.DrawString(font, "ins = set colliders", new Vector2(10, 50), Color.LimeGreen);
            spriteBatch.DrawString(font, "$ =Ssave to: " + Game1.LevelName +" -- time saved: " + timeSaved, new Vector2(10, 70), Color.LimeGreen);
            spriteBatch.DrawString(font, "! = Load from: " + Game1.LevelName, new Vector2(10, 90), Color.LimeGreen);
            spriteBatch.DrawString(font, "* = Load from buackup: " + Game1.LevelName, new Vector2(10, 110), Color.LimeGreen);
            spriteBatch.DrawString(font, "M = player position", new Vector2(10, 130), Color.LimeGreen);
            spriteBatch.DrawString(font, "< > = rotate", new Vector2(10, 150), Color.LimeGreen);
            spriteBatch.DrawString(font, "+- = scale", new Vector2(10, 170), Color.LimeGreen);
            spriteBatch.DrawString(font, "Page Up/Dn = overlap", new Vector2(10, 190), Color.LimeGreen);
            spriteBatch.DrawString(font, "numpad = offset", new Vector2(10, 210), Color.LimeGreen);
            spriteBatch.DrawString(font, "home = reset tile", new Vector2(10, 230), Color.LimeGreen);
            spriteBatch.DrawString(font, "enter = test level", new Vector2(10, 250), Color.LimeGreen);
            spriteBatch.DrawString(font, "F12 = Show colliders", new Vector2(10, 270), Color.LimeGreen);
            spriteBatch.DrawString(font, "1-9 = add characters", new Vector2(10, 290), Color.LimeGreen);

        }

        // L O A D  L E V E L
        public void LoadLevel(string levelName)
        {
            if (!File.Exists(levelName)) return;
            mp.ClearMap();
            using(StreamReader reader = new StreamReader(levelName))
            {
                string line = reader.ReadLine(); string[] parts = line.Split(',');
                mp.startData.X = int.Parse(parts[0]);
                mp.startData.Y = int.Parse(parts[1]);
                mp.loc.X = mp.startData.X;
                mp.loc.Y = mp.startData.Y;

                //SET PLAYER'S POSITION ::TODO

                int x = 0, y = 0;
                while(reader.EndOfStream != true)
                {
                    line = reader.ReadLine(); parts = line.Split(',');
                    switch (parts[0])
                    {
                        case "XY": x = int.Parse(parts[1]); y = int.Parse(parts[2]); break;
                        case "INDEX": tiles[x, y].index = int.Parse(parts[1]); break;
                        case "TYPE": tiles[x, y].type = (TileType)int.Parse(parts[1]); break;
                        case "OFFSET": tiles[x, y].offset.X = int.Parse(parts[1]); tiles[x, y].offset.Y = int.Parse(parts[2]); break;
                        case "ROT": tiles[x, y].rot = Single.Parse(parts[1]); break;
                        case "SCALE": tiles[x, y].scale.X = Single.Parse(parts[1]); tiles[x, y].scale.Y = Single.Parse(parts[2]); break;
                        case "OVERLAP": tiles[x, y].overlap = Boolean.Parse(parts[1]); break;
                        case "MONSTER": x = int.Parse(parts[1]); y = int.Parse(parts[2]);
                            tiles[x, y].monsterStart = (MonsterType)int.Parse(parts[3]); break;
                    }
                }
            }

            //PRE-PROCESS LOADED TILES (fix tile data)
            for (int b = 0; b < Map.TILESHIGH; b++)
            {
                for(int a = 0; a < Map.TILESWIDE; a++)
                {
                    int i = tiles[a, b].index;
                    if (tiles[a, b].monsterStart != MonsterType.None)
                    {
                        //ADD A MONSTER AT THIS ::TODO
                    }
                    TileType typ = sheet[i].type;
                    if (typ == TileType.empty) continue;


                    //PROVIDE DATA FOR TILE CLUSTER
                    if (typ == TileType.solid || typ == TileType.spring
                       || typ == TileType.platform || typ == TileType.spikes)
                    {
                        for (int d = b; d < b + sheet[i].tilesHigh; d++)
                        {
                            for(int c = a; a < a + sheet[i].tilesWide; c++)
                            {
                                tiles[c, d].overlap = true; tiles[c, d].standOn = true;
                                if (typ == TileType.spikes)
                                {
                                    tiles[c, d].spikes = true;
                                    tiles[c, d].isSolid = true;
                                }
                                else if (typ == TileType.solid) tiles[c, d].isSolid = true;
                            }
                        }
                    }
                }
            }
        }//load level

        // S A V E  L E V E L
        void SaveLevel(string levelName)
        {
            if (File.Exists(levelName)) File.Copy(levelName, Game1.BackupName, true);
            using(StreamWriter writer = new StreamWriter(levelName))
            {
                writer.Write(mp.startData.X.ToString() + ","); writer.Write(mp.startData.Y.ToString() + ","); writer.WriteLine();

                for(int y = 0; y < Map.TILESWIDE; y++)
                {
                    for(int x = 0; x < Map.TILESHIGH; x++)
                    {
                        int temp;
                        if (tiles[x, y].index != 0 || tiles[x, y].type != TileType.empty)
                        {
                            writer.Write("XY,"); writer.Write(x.ToString() + ","); writer.Write(y.ToString() + ","); writer.WriteLine();
                            writer.Write("INDEX,"); writer.Write(tiles[x, y].index.ToString() + ","); writer.WriteLine();
                            temp = (int)tiles[x, y].type;
                            writer.Write("TYPE,"); writer.Write(temp.ToString() + ","); writer.WriteLine();
                            writer.Write("OFFSET,"); writer.Write(tiles[x, y].offset.X.ToString() + ","); writer.Write(tiles[x, y].offset.Y.ToString() + ","); writer.WriteLine();
                            writer.Write("ROT,"); writer.Write(tiles[x, y].rot.ToString() + ","); writer.WriteLine();
                            writer.Write("SCALE,"); writer.Write(tiles[x, y].scale.X.ToString() + ","); writer.Write(tiles[x, y].scale.Y.ToString() + ","); writer.WriteLine();
                            writer.Write("OVERLAP,"); writer.Write(tiles[x, y].overlap.ToString() + ","); writer.WriteLine();
                        }
                        if (tiles[x, y].monsterStart != MonsterType.None)
                        {
                            temp = (int)tiles[x, y].monsterStart;
                            writer.Write("MONSTER,"); writer.Write(x.ToString() + ","); writer.Write(y.ToString() + ","); writer.Write(temp.ToString() + ",");writer.WriteLine();
                        }
                    }
                }
                timeSaved = DateTime.Now.ToShortTimeString();
            }
        }// Save Level






    }
}
