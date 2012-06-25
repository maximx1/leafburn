using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//**********************************************//
//               InitializeLayout.cs            //
//**********************************************//
//  This class lays out the pieces on the grid  //
//**********************************************//
//***
//                **Things to do**
//
// 1. Remove the tests at the end of the coding projects
// 2. Rename tmpX and tmpY to something more meaningfull
// 3. Use this Class to teach c# to the others in the group
// 4. Make sure to comment everything

namespace Project_Leafburn
{
    static class InitializeLayout
    {
        static private int countId = 0;
        static private int tmpX = 0;
        static private int tmpY = 0;

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
