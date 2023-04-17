using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Hedgehog
{
    class Sheet
    {
        public TileType type;
        public Rectangle rect;
        public int tilesWide, tilesHigh;
        public Vector2 offset;

        public Sheet(int x, int y, int x2, int y2, TileType Type,
            int TyleWide, int TyleHigh, float topLeftCornerX, float topLeftCornerY)
        {
            int width = x2 - x + 1;
            int height = y2 - y + 1;
            rect = new Rectangle(x, y, width, height);
            type = Type;
            tilesWide = TyleWide;
            tilesHigh = TyleHigh;
            offset = new Vector2(rect.X, rect.Y) - new Vector2(topLeftCornerX, topLeftCornerY);
        }
    }
    class SheetManager
    {
        int numSheetParts;

        public SheetManager() { numSheetParts = 0; }

        public void SetupSheetLevel1(ref Sheet[] sheet)
        {
            numSheetParts = 0;
            int n = 0;
            sheet[n] = new Sheet(0, 0, 1, 1, TileType.empty, 1, 1, 0f, 0f); n++; //ничего 0
            sheet[n] = new Sheet(10, 14, 39, 63, TileType.solid, 1, 1, 0f, 0f); //гидрант 
            //TODO
            //
            //
            //
            numSheetParts = n;
        }
    }
}
