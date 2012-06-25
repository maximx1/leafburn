using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project_Leafburn
{
    /// <summary>
    /// This class lays out the pieces on the grid
    /// </summary>
    static class InitializeLayout
    {
        static private int countId = 0;
        static private int tmpX = 0;
        static private int tmpY = 0;

        /// <summary>
        /// Initializes the checker pieces to the American Checkers standard
        /// </summary>
        /// <param name="checkers">The array of 24 checker pieces</param>
        public static void setPiece(CheckerPiece[] checkers)
        {
            int tmp = 0;
            for (countId = 0; countId < 24; countId++)
            {
                checkers[countId] = new CheckerPiece();
                checkers[countId].id = countId;
            }
            for (int side = 0; side < 2; side++)
            {
                if (side == 0)
                {
                    tmpY = 0;
                }
                else
                {
                    tmpY = 5;
                }
                for (int row = 0; row < 3; row++)
                {
                    if (side == 0)
                    {
                        tmpX = 1;
                        if (row == 1)
                        {
                            tmpX -= 1;
                        }
                    }
                    if (side == 1)
                    {
                        tmpX = 0;
                        if (row == 1)
                        {
                            tmpX += 1;
                        }
                    }
                    for (int col = 0; col < 4; col++)
                    {
                        checkers[tmp].xValue = tmpX;
                        checkers[tmp].yValue = tmpY;
                        tmpX += 2;
                        tmp++;
                    }
                    tmpY += 1;
                }
            }

               ////////////////////////////////TEST/////////////////////////////////
               //checkers[3].showSelect = true;
               //checkers[6].showNext1 = true;
               //checkers[8].taken = true;
               //checkers[12].taken = true;
               //checkers[22].isKing = true;
               //checkers[23].xValue = 3;
               //checkers[23].yValue = 4;
               ////////////////////////////////TEST/////////////////////////////////
        }
    }
}
