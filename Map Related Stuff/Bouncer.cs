using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hedgehog
{
    //BOUNCER
    class Bouncer
    {
        public int x, y; //tile to run bounce animation on
        public Vector2 originalOffset;
        double distance, frequency; // controls wobble motion
        double f;

        public Bouncer(int tileX, int tileY, Vector2 originalOffset, double startWobbleDistance, double wobbleFrequency)
        {

        }
    }
}
