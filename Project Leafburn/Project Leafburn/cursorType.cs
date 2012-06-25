using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Project_Leafburn
{
    class cursorType
    {
        public float x;
        public float y;
        public Texture2D image;
        public ButtonState leftMouse;
        private ButtonState leftMousePrev;

        public cursorType()
        {
            x = 0f;
            y = 0f;
            leftMouse = ButtonState.Released;
            leftMousePrev = leftMouse;
        }
        public void setLeftMousePrev()
        {
            this.leftMousePrev = this.leftMouse;
        }
    }
}
