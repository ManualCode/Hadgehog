using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Hedgehog
{
    internal class Input
    {
        public KeyboardState kb, okb;
        public bool shiftDown, controlDown, altDown;
        public bool shiftPress, controlPress, altPress;
        public bool oldShiftDown, oldControlDown, oldAltDown;

        public Input()
        {

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool KeyPress(Keys k) { if (kb.IsKeyDown(k) && okb.IsKeyUp(k)) return true; else return false; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool KeyDown(Keys k) { if (kb.IsKeyDown(k)) return true; else return false; }

        public void Update()
        {
            oldAltDown = altDown; oldShiftDown = shiftDown; oldControlDown = controlDown;
            okb = kb;
            kb = Keyboard.GetState();
            shiftDown = false; shiftPress = false;
            controlDown = false; controlPress = false;
            altDown = false; altPress = false;

            if (kb.IsKeyDown(Keys.LeftShift) || kb.IsKeyDown(Keys.RightShift)) shiftDown = true;
            if (kb.IsKeyDown(Keys.LeftControl) || kb.IsKeyDown(Keys.RightControl)) controlDown = true;
            if (kb.IsKeyDown(Keys.LeftAlt) || kb.IsKeyDown(Keys.RightAlt)) altDown = true;

            if (shiftDown && !oldShiftDown) shiftPress = true;
            if (controlDown && !oldControlDown) controlPress = true;
            if (altDown && !oldAltDown) altPress = true;

        }
    }
}
