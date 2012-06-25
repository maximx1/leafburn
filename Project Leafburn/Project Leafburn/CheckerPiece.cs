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
    /// <summary>
    /// Base class for the checker pieces
    /// </summary>
    class CheckerPiece
    {
        public int id=0;
        public int xValue = 0;
        public int yValue = 0;
        public float xLocationPix = 20f;
        public float yLocationPix = 20f;
        public int sn1X = 8;
        public int sn1Y = 8;
        public int sn2X = 8;
        public int sn2Y = 8;
        public int sn3X = 8;
        public int sn3Y = 8;
        public int sn4X = 8;
        public int sn4Y = 8;
        public float sn1XPix = 20f;
        public float sn1YPix = 20f;
        public float sn2XPix = 20f;
        public float sn2YPix = 20f;
        public float sn3XPix = 20f;
        public float sn3YPix = 20f;
        public float sn4XPix = 20f;
        public float sn4YPix = 20f;
        private float xOffset = 20f;
        private float yOffset = 20f;
        public bool showSelect = false;
        public bool showNext1 = false;
        public bool showNext2 = false;
        public bool showNext3 = false;
        public bool showNext4 = false;
        public bool isKing = false;
        public bool taken = false;
        public int player = 0;
        public Texture2D image;
        public Texture2D select;
        public Texture2D nextMove1;
        public Texture2D nextMove2;
        public Texture2D nextMove3;
        public Texture2D nextMove4;
        public Texture2D king;

        /// <summary>
        /// Calculates the pixel count of each checker piece
        /// </summary>
        public void calcPiecePix()
        {
            xLocationPix = ((float)xValue * 100f + xOffset);
            yLocationPix = ((float)yValue * 100f + yOffset);
        }

        /// <summary>
        /// Calculates the pixel count of the next possible move overlay
        /// </summary>
        public void calcNextLocationsPix()
        {
            sn1XPix = ((float)sn1X * 100f + xOffset);
            sn1YPix = ((float)sn1Y * 100f + yOffset);
            sn2XPix = ((float)sn2X * 100f + xOffset);
            sn2YPix = ((float)sn2Y * 100f + yOffset);
            sn3XPix = ((float)sn3X * 100f + xOffset);
            sn3YPix = ((float)sn3Y * 100f + yOffset);
            sn4XPix = ((float)sn4X * 100f + xOffset);
            sn4YPix = ((float)sn4Y * 100f + yOffset);
        }
    }
}