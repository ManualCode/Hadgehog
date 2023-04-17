using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hedgehog
{
    class Map
    {
        public const int TILESWIDE = 100, TILESHIGH = 50;
        public const int TILESPANX = 14, TILESPANY = 10;

        public Tile[,] tiles;
        public StartData startData;
        public Point loc;
        Sheet[] sheet;
        ProcessedTile[] overlapTiles;
        int overlapCount;
        public Vector2 scrollOffset;
        public int a1, b1, a2, b2;
        public float sx, sy;
        Texture2D tilesImage;
        Vector2 screenCenter;
        SpriteBatch spriteBatch;

        //CONSTRUCT
        public Map(Sheet[] sht, SpriteBatch sprBatch)
        {
            sheet = sht;
            spriteBatch = sprBatch;
            tiles = new Tile[TILESWIDE, TILESHIGH];
            for(int b = 0; b < TILESHIGH; b++)
            {
                for(int a = 0; a < TILESWIDE; a++)
                {
                    tiles[a, b] = new Tile(0, TileType.empty);
                }
            }
            overlapTiles = new ProcessedTile[250];
            for (int a = 0; a < 250; a++) overlapTiles[a] = new ProcessedTile();

            loc = new Point(5, 5);
            startData = new StartData(loc.X, loc.Y);
            screenCenter = Game1.screenCenter;
        }
        
        public void SetTiesImage(Texture2D tilesPic)
        {
            tilesImage = tilesPic;
        }   

        //ADD TILE
        public void AddTile(int i)
        {
            DeleteTile();
            tiles[loc.X, loc.Y].index = i;
            tiles[loc.X, loc.Y].offset = sheet[i].offset;
            for(int b = loc.Y; b < loc.Y + sheet[i].tilesHigh; b++)
            {
                if (b >= TILESHIGH - 1) break;
                for (int a = loc.X; a < loc.X + sheet[i].tilesWide; a++)
                {
                    if (a >= TILESWIDE - 1) break;
                    TileType type = sheet[i].type;
                    tiles[a, b].type = type;

                    if (type == TileType.solid || type == TileType.spring
                        || type == TileType.platform || type == TileType.spikes)
                    {
                        tiles[a, b].overlap = true; tiles[a, b].standOn = true;
                        if(type == TileType.spikes) { tiles[a, b].spikes = true; tiles[a, b].isSolid = true; }
                        else if(type == TileType.solid) tiles[a, b].isSolid = true;
                    }
                }
            }

        }

        //SET TYPE
        public void SetType(TileType type = TileType.solid)
        {
            int a = loc.X, b = loc.Y;
            tiles[a, b].type = type;

            if (type == TileType.solid || type == TileType.spring
                        || type == TileType.platform || type == TileType.spikes)
            {
                tiles[a, b].overlap = true; tiles[a, b].standOn = true;
                if (type == TileType.spikes) { tiles[a, b].spikes = true; tiles[a, b].isSolid = true; }
                else if (type == TileType.solid) tiles[a, b].isSolid = true;
            }
        }



        //SET MONSTER 
        public void SetMonster(MonsterType monster)
        {
            tiles[loc.X, loc.Y].monsterStart = monster;
        }

        //DELETE TILE
        public void DeleteTile()
        {
            int i = tiles[loc.X, loc.Y].index;
            for (int b = loc.Y; b < loc.Y + sheet[i].tilesHigh; b++)
            {
                if (b >= TILESHIGH - 1) break;
                for(int a = loc.X; a < loc.X + sheet[i].tilesWide; a++)
                {
                    if (a >= TILESWIDE - 1) break;
                    tiles[a, b].Clear();
                }
            }
        }

        //CLEAR MAP
        public void ClearMap()
        {
            for(int b = 0; b < TILESHIGH; b++)
                for(int a = 0; a < TILESWIDE; a++)
                    tiles[a, b].Clear();
        }

        public void AddBorder(int i)
        {
            for(int a = 0; a < TILESWIDE; a++)
            {
                loc.X = a; loc.Y = 0;
                AddTile(i);
                loc.Y = TILESHIGH - 1;
                AddTile(i);
            }

            for (int a = 0; a < TILESHIGH; a++)
            {
                loc.X = 0; loc.Y = a;
                AddTile(i);
                loc.X = TILESWIDE - 1;
                AddTile(i);
            }
        }

        public void UpdateVars()
        {

        }

        //WORLD TO CAMERA
        public void WorldToCamera(Vector2 camPos, ref Vector2 backgroundPos)
        {
            scrollOffset = Vector2.Zero;
            loc = Conv.GetTileCoord(camPos, ref scrollOffset);
            backgroundPos.X = (loc.X * 64 + scrollOffset.X) * -0.5f;
            backgroundPos.Y = (loc.Y * 64 + scrollOffset.Y) * -0.5f;
        }

        // D R A W   T I L E S
        public void DrawTiles(bool drawColliaders = false, bool editMode = false)
        {
            //Получаем область плиток для рисования (сверху вниз, слева направо) на экране в зависимости от положения карты:
            b1 = loc.Y - TILESPANY;
            b2 = loc.Y + TILESPANY;
            a1 = loc.X - TILESPANX;
            a2 = loc.X + TILESPANX;

            //Проверка на выход за пределы:

            if (b1 < 0) { b1 = 0; if (b2 < 0) b2 = 0; }
            if (b2 >= TILESHIGH) { b2 = TILESHIGH - 1; if (b1 >= TILESHIGH) b1 = TILESHIGH - 1; }
            if (a1 < 0) { a1 = 0; if (a2 < 0) a2 = 0; }
            if (a2 >= TILESWIDE) { a2 = TILESWIDE - 1; if (a1 >= TILESWIDE) a1 = TILESWIDE - 1; }

            //Вычесление начала координат для отрисоки плиток:
            int bdif = loc.Y - b1; //сколько плиток находится выше середины экрана
            int adif = loc.X - a1; //сколько плиток находится левее середины экрана

            sx = screenCenter.X - adif * 64.0f; // начальная координата Х для размещения плиток на экране
            sy = screenCenter.Y - bdif * 64.0f; // начальная координата Y для размещения плиток на экране

            //рисование участка плитки, который должен быть виден
            float x, y; //координаты экрана
            int a, b, i; 
            Sheet sh;
            overlapCount = 0;
            Vector2 tilePos; // финальное положение плиток на экране (со смещениями)

            b = b1; y = sy;
            while(b < b2)
            {
                a = a1; x = sx;
                while(a < a2)
                {
                    i = tiles[a, b].index;
                    tilePos = new Vector2(x, y) - scrollOffset;

                    //EDITOR LOCATION HELPERS:
                    if (drawColliaders)
                    {
                        if (tiles[a, b].isSolid || tiles[a, b].standOn)
                        {
                            Vector2 siz = Vector2.One;
                            if (!tiles[a, b].isSolid) siz = new Vector2(1, 0.4f);
                            spriteBatch.Draw(tilesImage, tilePos, new Rectangle(284, 0, 37, 64), Color.Purple, 0f, Vector2.Zero, siz, SpriteEffects.None, 0f);
                            a++; x += 64.0f; continue;
                        }
                    }
                    if (editMode)
                    {
                        if (tiles[a, b].monsterStart != MonsterType.None)
                        {
                            spriteBatch.Draw(tilesImage, tilePos, new Rectangle(284, 0, 37, 64), Color.Yellow, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                        }
                        a++; x += 64.0f; continue;
                    }


                    //DRAW ACTUAL TILES
                    if (i == 0) { a++; x += 64.0f; continue; } //empty so skip to next one
                    sh = sheet[i];
                    tilePos += tiles[a, b].offset;
                    //STORE OVERLAPS (otherwise just draw)
                    if (tiles[a, b].overlap) { overlapTiles[overlapCount].Add(tilePos, sh.rect, tiles[a, b].rot, tiles[a, b].scale); overlapCount++; }
                    else spriteBatch.Draw(tilesImage, tilePos, sh.rect, Color.White, tiles[a, b].rot, Vector2.Zero, tiles[a, b].scale, SpriteEffects.None, 0f);

                    a++; x += 64.0f;
                }

                b++; y += 64.0f;
            }
        } //DRAW TILES

        public void DrawOverlaps()
        {
            int i = 0;
            while (i < overlapCount)
            {
                spriteBatch.Draw(tilesImage, overlapTiles[i].pos, overlapTiles[i].rect, Color.White, overlapTiles[i].rot,
                    Vector2.Zero, overlapTiles[i].scale, SpriteEffects.None, 0f);
                i++;
            }
        }
    }
}
