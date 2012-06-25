

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

//**********************************************//
//               InitializeLayout.cs            //
//**********************************************//
//   This class handles the update and draw     //
//**********************************************//
//***
//                **Things to do**
//
// 21. Add show possible moves
// 22. Add move to possible locations




namespace Project_Leafburn
{
    public partial class LeafBurn1 : Microsoft.Xna.Framework.Game
    {
        const float gameWindowHeight = 831f;	//Sets the game window height
        const float gameWindowWidth = 1061f;	//Sets the game window width
        const float scale = 0.8f;		//Adjusts the scale for the display
        GraphicsDeviceManager graphics;			// This sets up your graphics card to use the code
        SpriteBatch spriteBatch;			// initializes a spritebatch object which is used to draw the board and pieces
        CheckerPiece[] checkers = new CheckerPiece[24];	// creates an array of checker objects, 1 for each piece
        Texture2D backGround;				// this initializes the background for the checker board
        SpriteFont playerNumFont;
        SpriteFont otherFont;
        SpriteFont endGameFont;
        cursorType gameCursor=new cursorType();		// this initializes an object keep track of the mouse
        MouseState updatedMouse;			// registers clicks of the mouse
        MouseState oldMouse;                // A storage place for the old mouse input
        //int chosenPiece = 25;
        int playerTurn = 1;
        bool gameRun=true;
        string winner;

        // this class draws the game board, pieces and cursor

        public LeafBurn1()
        {
            graphics = new GraphicsDeviceManager(this);         // gets info from the graphics card
            Content.RootDirectory = "Content";                //Points to images
            graphics.PreferredBackBufferHeight = (Int32)(gameWindowHeight*scale); //set the window size to the scale of game requires an integer class
            graphics.PreferredBackBufferWidth = (Int32)(gameWindowWidth*scale);
        }

        // this method will make the normal cursor dissapear
        protected override void Initialize()
        {
            this.IsMouseVisible = false;    //Do not display system mouse
            base.Initialize();              //Initialize the gaming subsystem
        }

        //this draws everything
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);  //get the program ready to draw
            InitializeLayout.setPiece(checkers);    //sets all the x and y coordinates for the pieces
           
            // this loop loads all the pieces and possible moves
            for (int i = 0; i < 24; i++)
            {
                if (checkers[i].id < 12)  // id less than 12 are black pieces
                {
                    checkers[i].player = 2; //black is player 2
                    checkers[i].image = Content.Load<Texture2D>("black_piece");       //these next lines load the pieces and the shape to highlight a move
                }
                else
                {
                    checkers[i].player = 1;
                    checkers[i].image = Content.Load<Texture2D>("red_piece");
                }
                checkers[i].select = Content.Load<Texture2D>("highlight_select");
                checkers[i].nextMove1 = Content.Load<Texture2D>("highlight_move_to");
                checkers[i].nextMove2 = Content.Load<Texture2D>("highlight_move_to");
                checkers[i].nextMove3 = Content.Load<Texture2D>("highlight_move_to");
                checkers[i].nextMove4 = Content.Load<Texture2D>("highlight_move_to");
                checkers[i].king = Content.Load<Texture2D>("king_piece");
            }
            //Load background
            backGround = Content.Load<Texture2D>("board");
            gameCursor.image = Content.Load<Texture2D>("cursor");
            playerNumFont = Content.Load<SpriteFont>("PlayerNum");
            otherFont = Content.Load<SpriteFont>("otherFont");
            endGameFont = Content.Load<SpriteFont>("endgame");
        }

        // let garbage collection deal with this
        protected override void UnloadContent()
        {
        }

        //will exit the game if escape is clicked

        protected override void Update(GameTime gameTime)
        {
            KeyboardState updatedKeyboard;      //initialize variable to register keyboard action
            updatedKeyboard = Keyboard.GetState(); //find current state of keyboard
            

            if (updatedKeyboard.IsKeyDown(Keys.Escape)) //check if the escape key is pressed, if it is, close the game
            {
                this.Exit(); //terminate program
            }

            //Update Mouse coordinates and trap the the cursor sprite inside the boards parameters
            oldMouse = updatedMouse;
            updatedMouse = Mouse.GetState();
            if ((updatedMouse.X < gameWindowWidth * scale && updatedMouse.X >= 0f && updatedMouse.Y < gameWindowHeight * scale && updatedMouse.Y >= 0f))
            {
                gameCursor.x = updatedMouse.X;
                gameCursor.y = updatedMouse.Y;
                onMouseClick();
            }

            int playerOneCount = 0;
            int playerTwoCount = 0;

            foreach (CheckerPiece i in checkers)
            {//Takes the x and y locations and converts them into x and y coordinates(pixels)
                i.calcPiecePix();
                i.calcNextLocationsPix();
                if (i.player == 1 && i.taken)
                    playerOneCount++;
                if (i.player == 2 &&i.taken)
                    playerTwoCount++;
            }

            if (playerOneCount == 12)
            {
                winner = "2";
                gameRun = false;
            }

            if (playerTwoCount == 12)
            {
                winner = "1";
                gameRun = false;
            }


            base.Update(gameTime);
        }

        //this is the method that actually draws the board and pieces

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Green); //this is the lowest lvl being drawn, the only visible part is the green menu
            spriteBatch.Begin(); //begin the sequence to draw the objects
            spriteBatch.Draw(backGround, Vector2.Zero, null,
                Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f); //this draws the board on top of the background
            if (gameRun)
            {
                for (int i = 0; i < 24; i++)
                {
                    //this draws all the pieces and also possible moves if they are ready to be drawn
                    if (checkers[i].showSelect) //show select is true when a square with a piece is clicked, if it's clicked, highlight that piece's square
                        spriteBatch.Draw(checkers[i].select, new Vector2((checkers[i].xLocationPix - (8f * scale)) * scale, (checkers[i].yLocationPix - (8f * scale)) * scale), null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                    //Select is at the correct location
                    if (checkers[i].showNext1)
                        spriteBatch.Draw(checkers[i].nextMove1, new Vector2((checkers[i].sn1XPix - (8f * scale)) * scale, (checkers[i].sn1YPix - (8f * scale)) * scale), null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                    if (checkers[i].showNext2)
                        spriteBatch.Draw(checkers[i].nextMove2, new Vector2((checkers[i].sn2XPix - (8f * scale)) * scale, (checkers[i].sn2YPix - (8f * scale)) * scale), null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                    if (checkers[i].showNext3)
                        spriteBatch.Draw(checkers[i].nextMove3, new Vector2((checkers[i].sn3XPix - (8f * scale)) * scale, (checkers[i].sn3YPix - (8f * scale)) * scale), null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                    if (checkers[i].showNext4)
                        spriteBatch.Draw(checkers[i].nextMove4, new Vector2((checkers[i].sn4XPix - (8f * scale)) * scale, (checkers[i].sn4YPix - (8f * scale)) * scale), null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                    if (!checkers[i].taken) //if it's not taken draw the piece
                    {
                        spriteBatch.Draw(checkers[i].image, new Vector2(checkers[i].xLocationPix * scale, checkers[i].yLocationPix * scale), null,
                            Color.White, 0f/*This is piece rotation*/, Vector2.Zero, scale, SpriteEffects.None, 0f);
                    }
                    if (checkers[i].isKing)
                    {
                        spriteBatch.Draw(checkers[i].king, new Vector2(checkers[i].xLocationPix * scale, checkers[i].yLocationPix * scale), null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                    }
                }

                //draws the cursor for the game
                spriteBatch.DrawString(otherFont, "Player", new Vector2(880f * scale, 2f * scale), Color.Yellow);
                spriteBatch.DrawString(playerNumFont, playerTurn.ToString(), new Vector2(890f * scale, 5f * scale), Color.Yellow);
            }
            else
            {
                spriteBatch.DrawString(endGameFont, "Player " + winner + " wins!", new Vector2(50f * scale, 50f * scale), Color.Black);
            }
            spriteBatch.Draw(gameCursor.image, new Vector2(gameCursor.x, gameCursor.y), null,
                Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.End();  //ends the sequence of drawing
            base.Draw(gameTime); 
        }

        public void onMouseClick()
        {
            if (updatedMouse.LeftButton == ButtonState.Pressed && updatedMouse.LeftButton != oldMouse.LeftButton)
            {
                for (int chosen = 0; chosen < 24; chosen++)
                {
                    if (updatedMouse.X >= checkers[chosen].sn1XPix * scale && updatedMouse.X < (checkers[chosen].sn1XPix + (100f * scale)) * scale &&
                        updatedMouse.Y >= checkers[chosen].sn1YPix * scale && updatedMouse.Y < (checkers[chosen].sn1YPix + (100f * scale)) * scale && checkers[chosen].showNext1)
                    {
                        foreach (CheckerPiece i in checkers)
                        {
                            if (i.xValue == checkers[chosen].xValue - 1 && i.yValue == checkers[chosen].yValue - 1 && playerTurn == 1)
                            {
                                i.taken = true;
                                i.xValue = 8;
                                i.yValue = 8;
                            }
                            if (i.xValue == checkers[chosen].xValue - 1 && i.yValue == checkers[chosen].yValue + 1 && playerTurn == 2)
                            {
                                i.taken = true;
                                i.xValue = 8;
                                i.yValue = 8;
                            }
                        }
                        checkers[chosen].xValue = checkers[chosen].sn1X;
                        checkers[chosen].yValue = checkers[chosen].sn1Y;
                        checkers[chosen].showSelect = false;
                        checkers[chosen].showNext1 = false;
                        checkers[chosen].showNext2 = false;
                        checkers[chosen].showNext3 = false;
                        checkers[chosen].showNext4 = false;
                        if (playerTurn == 1)
                        {
                            if (checkers[chosen].yValue == 0)
                                checkers[chosen].isKing = true;
                            playerTurn = 2;
                        }
                        else if (playerTurn == 2)
                        {
                            if (checkers[chosen].yValue == 7)
                                checkers[chosen].isKing = true;
                            playerTurn = 1;
                        }
                    }
                    else if (updatedMouse.X >= checkers[chosen].sn2XPix * scale && updatedMouse.X < (checkers[chosen].sn2XPix + (100f * scale)) * scale &&
                        updatedMouse.Y >= checkers[chosen].sn2YPix * scale && updatedMouse.Y < (checkers[chosen].sn2YPix + (100f * scale)) * scale && checkers[chosen].showNext2)
                    {
                        foreach (CheckerPiece i in checkers)
                        {
                            if (i.xValue == checkers[chosen].xValue + 1 && i.yValue == checkers[chosen].yValue - 1 && playerTurn == 1)
                            {
                                i.taken = true;
                                i.xValue = 8;
                                i.yValue = 8;
                            }
                            if (i.xValue == checkers[chosen].xValue + 1 && i.yValue == checkers[chosen].yValue + 1 && playerTurn == 2)
                            {
                                i.taken = true;
                                i.xValue = 8;
                                i.yValue = 8;
                            }
                        }
                        checkers[chosen].xValue = checkers[chosen].sn2X;
                        checkers[chosen].yValue = checkers[chosen].sn2Y;
                        checkers[chosen].showSelect = false;
                        checkers[chosen].showNext1 = false;
                        checkers[chosen].showNext2 = false;
                        checkers[chosen].showNext3 = false;
                        checkers[chosen].showNext4 = false;
                        if (playerTurn == 1)
                        {
                            if (checkers[chosen].yValue == 0)
                                checkers[chosen].isKing = true;
                            playerTurn = 2;
                        }
                        else if (playerTurn == 2)
                        {
                            if (checkers[chosen].yValue == 7)
                                checkers[chosen].isKing = true;
                            playerTurn = 1;
                        }
                    }
                    else if (updatedMouse.X >= checkers[chosen].sn3XPix * scale && updatedMouse.X < (checkers[chosen].sn3XPix + (100f * scale)) * scale &&
                             updatedMouse.Y >= checkers[chosen].sn3YPix * scale && updatedMouse.Y < (checkers[chosen].sn3YPix + (100f * scale)) * scale && checkers[chosen].showNext3)
                    {
                        foreach (CheckerPiece i in checkers)
                        {
                            if (i.xValue == checkers[chosen].xValue - 1 && i.yValue == checkers[chosen].yValue + 1 && playerTurn == 1)
                            {
                                i.taken = true;
                                i.xValue = 8;
                                i.yValue = 8;
                            }
                            if (i.xValue == checkers[chosen].xValue - 1 && i.yValue == checkers[chosen].yValue - 1 && playerTurn == 2)
                            {
                                i.taken = true;
                                i.xValue = 8;
                                i.yValue = 8;
                            }
                        }
                        checkers[chosen].xValue = checkers[chosen].sn3X;
                        checkers[chosen].yValue = checkers[chosen].sn3Y;
                        checkers[chosen].showSelect = false;
                        checkers[chosen].showNext1 = false;
                        checkers[chosen].showNext2 = false;
                        checkers[chosen].showNext3 = false;
                        checkers[chosen].showNext4 = false;
                        if (playerTurn == 1)
                            playerTurn = 2;
                        else if (playerTurn == 2)
                            playerTurn = 1;
                    }
                    else if (updatedMouse.X >= checkers[chosen].sn4XPix * scale && updatedMouse.X < (checkers[chosen].sn4XPix + (100f * scale)) * scale &&
                            updatedMouse.Y >= checkers[chosen].sn4YPix * scale && updatedMouse.Y < (checkers[chosen].sn4YPix + (100f * scale)) * scale && checkers[chosen].showNext4)
                    {
                        foreach (CheckerPiece i in checkers)
                        {
                            if (i.xValue == checkers[chosen].xValue + 1 && i.yValue == checkers[chosen].yValue + 1 && playerTurn == 1)
                            {
                                i.taken = true;
                                i.xValue = 8;
                                i.yValue = 8;
                            }
                            if (i.xValue == checkers[chosen].xValue + 1 && i.yValue == checkers[chosen].yValue - 1 && playerTurn == 2)
                            {
                                i.taken = true;
                                i.xValue = 8;
                                i.yValue = 8;
                            }
                        }
                        checkers[chosen].xValue = checkers[chosen].sn4X;
                        checkers[chosen].yValue = checkers[chosen].sn4Y;
                        checkers[chosen].showSelect = false;
                        checkers[chosen].showNext1 = false;
                        checkers[chosen].showNext2 = false;
                        checkers[chosen].showNext3 = false;
                        checkers[chosen].showNext4 = false;
                        if (playerTurn == 1)
                            playerTurn = 2;
                        else if (playerTurn == 2)
                            playerTurn = 1;
                    }
                    else if (updatedMouse.X >= checkers[chosen].xLocationPix * scale && updatedMouse.X < (checkers[chosen].xLocationPix + (100f * scale)) * scale &&
                             updatedMouse.Y >= checkers[chosen].yLocationPix * scale && updatedMouse.Y < (checkers[chosen].yLocationPix + (100f * scale)) * scale)
                    {
                        if (checkers[chosen].player == playerTurn)
                        {
                            for (int prevSelected = 0; prevSelected < 24; prevSelected++)
                            {
                                if (checkers[prevSelected].showSelect == true && prevSelected != chosen)
                                {
                                    checkers[prevSelected].showSelect = false;
                                    checkers[prevSelected].showNext1 = false;
                                    checkers[prevSelected].showNext2 = false;
                                    checkers[prevSelected].showNext3 = false;
                                    checkers[prevSelected].showNext4 = false;
                                }
                            }
                            if (checkers[chosen].showSelect == true)
                            {
                                checkers[chosen].showSelect = false;
                                checkers[chosen].showNext1 = false;
                                checkers[chosen].showNext2 = false;
                                checkers[chosen].showNext3 = false;
                                checkers[chosen].showNext4 = false;
                            }
                            else if (checkers[chosen].showSelect == false)
                            {
                                checkers[chosen].showSelect = true;
                                showNextMoves(chosen);
                            }
                            break;
                        }
                    }
                }
            }
        }

        private void showNextMoves(int chosen)
        {
            int sn1X;
            int sn1Y;
            int sn2X;
            int sn2Y;
            int sn3X;
            int sn3Y;
            int sn4X;
            int sn4Y;

            if (playerTurn == 1)
            {
                sn1X = checkers[chosen].xValue - 1;
                sn1Y = checkers[chosen].yValue - 1;
                sn2X = checkers[chosen].xValue + 1;
                sn2Y = checkers[chosen].yValue - 1;
                sn3X = checkers[chosen].xValue - 1;
                sn3Y = checkers[chosen].yValue + 1;
                sn4X = checkers[chosen].xValue + 1;
                sn4Y = checkers[chosen].yValue + 1;
            }
            else
            {

                sn1X = checkers[chosen].xValue - 1;
                sn1Y = checkers[chosen].yValue + 1;
                sn2X = checkers[chosen].xValue + 1;
                sn2Y = checkers[chosen].yValue + 1;
                sn3X = checkers[chosen].xValue - 1;
                sn3Y = checkers[chosen].yValue - 1;
                sn4X = checkers[chosen].xValue + 1;
                sn4Y = checkers[chosen].yValue - 1;
            }
            checkers[chosen].showNext1 = true;
            checkers[chosen].showNext2 = true;
            checkers[chosen].showNext3 = true;
            checkers[chosen].showNext4 = true;
            foreach (CheckerPiece i in checkers)
            {
                if ((i.xValue == sn1X && i.yValue == sn1Y) || sn1X < 0 || sn1Y < 0 || sn1X > 7 || sn1Y > 7)
                {
                    if (i.xValue == sn1X && i.yValue == sn1Y && i.player != playerTurn)
                    {//sn1
                        if (playerTurn == 1)
                        {
                            sn1X--;
                            sn1Y--;
                        }
                        else
                        {
                            sn1X--;
                            sn1Y++;
                        }

                        foreach (CheckerPiece i2 in checkers)
                        {
                            if ((i2.xValue == sn1X && i2.yValue == sn1Y) || sn1X < 0 || sn1Y < 0 || sn1X > 7 || sn1Y > 7)
                            {
                                checkers[chosen].showNext1 = false;
                            }
                        }
                    }
                    else
                    {
                        checkers[chosen].showNext1 = false;
                    }
                }
                if ((i.xValue == sn2X && i.yValue == sn2Y) || sn2X < 0 || sn2Y < 0 || sn2X > 7 || sn2Y > 7)
                {
                    if (i.xValue == sn2X && i.yValue == sn2Y && i.player != playerTurn)
                    {//sn2
                        if (playerTurn == 1)
                        {
                            sn2X++;
                            sn2Y--;
                        }
                        else
                        {
                            sn2X++;
                            sn2Y++;
                        }
                        foreach (CheckerPiece i2 in checkers)
                        {
                            if ((i2.xValue == sn2X && i2.yValue == sn2Y) || sn2X < 0 || sn2Y < 0 || sn2X > 7 || sn2Y > 7)
                            {
                                checkers[chosen].showNext2 = false;
                            }
                        }
                    }
                    else
                    {
                        checkers[chosen].showNext2 = false;
                    }
                }
                if (checkers[chosen].isKing)
                {
                    if ((i.xValue == sn3X && i.yValue == sn3Y) || sn3X < 0 || sn3Y < 0 || sn3X > 7 || sn3Y > 7)
                    {//sn3
                        if (i.xValue == sn3X && i.yValue == sn3Y && i.player != playerTurn)
                        {
                            if (playerTurn == 1)
                            {
                                sn3X--;
                                sn3Y++;
                            }
                            else
                            {
                                sn3X--;
                                sn3Y--;
                            }
                            foreach (CheckerPiece i2 in checkers)
                            {
                                if ((i2.xValue == sn3X && i2.yValue == sn3Y) || sn3X < 0 || sn3Y < 0 || sn3X > 7 || sn3Y > 7)
                                {
                                    checkers[chosen].showNext3 = false;
                                }
                            }
                        }
                        else
                        {
                            checkers[chosen].showNext3 = false;
                        }
                    }
                    if ((i.xValue == sn4X && i.yValue == sn4Y) || sn4X < 0 || sn4Y < 0 || sn4X > 7 || sn4Y > 7)
                    {
                        if (i.xValue == sn4X && i.yValue == sn4Y && i.player != playerTurn)
                        {//sn4
                            if (playerTurn == 1)
                            {
                                sn4X++;
                                sn4Y++;
                            }
                            else
                            {
                                sn4X++;
                                sn4Y--;
                            }
                            foreach (CheckerPiece i2 in checkers)
                            {
                                if ((i2.xValue == sn4X && i2.yValue == sn4Y) || sn4X < 0 || sn4Y < 0 || sn4X > 7 || sn4Y > 7)
                                {
                                    checkers[chosen].showNext4 = false;
                                }
                            }
                        }
                        else
                        {
                            checkers[chosen].showNext4 = false;
                        }
                    }
                }
                else
                {
                    checkers[chosen].showNext3 = false;
                    checkers[chosen].showNext4 = false;
                }
            }
            checkers[chosen].sn1X = sn1X;
            checkers[chosen].sn1Y = sn1Y;
            checkers[chosen].sn2X = sn2X;
            checkers[chosen].sn2Y = sn2Y;
            checkers[chosen].sn3X = sn3X;
            checkers[chosen].sn3Y = sn3Y;
            checkers[chosen].sn4X = sn4X;
            checkers[chosen].sn4Y = sn4Y;
        }
    }
}
