

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

namespace Project_Leafburn
{
    /// <summary>
    /// Main class of the game. This class draws the game board, pieces and cursor
    /// </summary>
    public partial class LeafBurn1 : Microsoft.Xna.Framework.Game
    {
        const float gameWindowHeight = 831f;	        //Sets the game window height
        const float gameWindowWidth = 1061f;	        //Sets the game window width
        const float scale = 0.8f;		                //Adjusts the scale for the display
        GraphicsDeviceManager graphics;			        //This sets up your graphics card to use the code
        SpriteBatch spriteBatch;			            //initializes a spritebatch object which is used to draw the board and pieces
        CheckerPiece[] checkers = new CheckerPiece[24];	//creates an array of checker objects, 1 for each piece
        Texture2D backGround;				            //this initializes the background for the checker board
        Texture2D p1Label;                             //Image for player 1 piece's left
        Texture2D p2Label;                             //Image for player 2 piece's left
        TimeSpan player1Time = new TimeSpan();          //Player 1's timer
        TimeSpan player2Time = new TimeSpan();          //Player 2's timer
        TimeSpan playerTempTime = new TimeSpan();       //Timespan holder for pause game
        SpriteFont playerNumFont;                       //Font to show which players turn it is
        SpriteFont labelFont;                           //24 pt font
        SpriteFont playerTimeFont;                      //Font to show the players time and score
        SpriteFont endGameFont;                         //Font to show who won the game upon completion
        cursorType gameCursor=new cursorType();	        //this initializes an object keep track of the mouse
        MouseState updatedMouse;			            //registers clicks of the mouse
        MouseState oldMouse;                            //A storage place for the old mouse input
        int playerTurn = 1;                             //Tells which players turn it is
        int player1Score = 0;                           //Player 1's final game score
        int player2Score = 0;                           //Player 2's final game score
        int player1OldPieceCount = 0;                   //Player 1's Old piece count
        int player2OldPieceCount = 0;                   //Player 2's Old piece count
        int player1OldKingCount = 0;                    //Player 1's Old King count
        int player2OldKingCount = 0;                    //Player 2's Old King count
        int player1Count = 0;                         //Player 1's taken piece count
        int player2Count = 0;                         //Player 2's taken piece count
        bool gameRun=true;                              //Determines if the game is running or not
        string winner;                                  //String to display who won
        string player1Name = "Player 1";                //Default name of player 1
        string player2Name = "Player 2";                //Default name of player 2

        /// <summary>
        /// Constructor:: initializes the graphics device settings
        /// </summary>
        public LeafBurn1()
        {
            graphics = new GraphicsDeviceManager(this);                             // gets info from the graphics card
            Content.RootDirectory = "Content";                                      //Points to images
            graphics.PreferredBackBufferHeight = (Int32)(gameWindowHeight*scale);   //set the window size to the scale of game requires an integer class
            graphics.PreferredBackBufferWidth = (Int32)(gameWindowWidth*scale);
        }

        /// <summary>
        /// Gamestate initializer
        /// This method will make the system cursor disappear
        /// </summary>
        protected override void Initialize()
        {
            this.IsMouseVisible = false;                //Do not display system mouse
            base.Initialize();                          //Initialize the gaming subsystem
            scores.initializeHighScoresFile();          //Make sure that there is a high scores file
        }

        /// <summary>
        /// This loads all the textures for displaying
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);  //get the program ready to draw
            InitializeLayout.setPiece(checkers);            //sets all the x and y coordinates for the pieces
           
            // this loop loads all the pieces and possible moves
            for (int i = 0; i < 24; i++)
            {
                if (checkers[i].id < 12)  // id less than 12 are black pieces
                {
                    checkers[i].player = 2; //black is player 2
                    checkers[i].image = Content.Load<Texture2D>(@"Game Visuals\black_piece");       //these next lines load the pieces and the shape to highlight a move
                }
                else
                {
                    checkers[i].player = 1;
                    checkers[i].image = Content.Load<Texture2D>(@"Game Visuals\red_piece");
                }
                checkers[i].select = Content.Load<Texture2D>(@"Game Visuals\highlight_select");
                checkers[i].nextMove1 = Content.Load<Texture2D>(@"Game Visuals\highlight_move_to");
                checkers[i].nextMove2 = Content.Load<Texture2D>(@"Game Visuals\highlight_move_to");
                checkers[i].nextMove3 = Content.Load<Texture2D>(@"Game Visuals\highlight_move_to");
                checkers[i].nextMove4 = Content.Load<Texture2D>(@"Game Visuals\highlight_move_to");
                checkers[i].king = Content.Load<Texture2D>(@"Game Visuals\king_piece");
            }
            //Load background
            backGround = Content.Load<Texture2D>(@"Game Visuals\board");
            gameCursor.image = Content.Load<Texture2D>(@"Game Visuals\cursor");
            p1Label = Content.Load<Texture2D>(@"Game Visuals\red_piece");
            p2Label = Content.Load<Texture2D>(@"Game Visuals\black_piece");
            playerNumFont = Content.Load<SpriteFont>(@"Fonts\PlayerNum");
            labelFont = Content.Load<SpriteFont>(@"Fonts\otherFont");
            endGameFont = Content.Load<SpriteFont>(@"Fonts\endgame");
            playerTimeFont = Content.Load<SpriteFont>(@"Fonts\playerTimeText");
        }

        /// <summary>
        /// Removes all the objects textures, let garbage collection deal with this
        /// </summary>
        protected override void UnloadContent() { }

        /// <summary>
        /// Updates the logic and state of the game.
        /// </summary>
        /// <param name="gameTime">Pass in the game time</param>
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

            playerTempTime = gameTime.ElapsedGameTime;              //keeps polling the game time so that the game can pause

            if (gameRun)
            {
                player1Count = checkers.Where(i => i.player == 1 && i.taken == true).Select(i => i).ToArray().Length;
                player2Count = checkers.Where(i => i.player == 2 && i.taken == true).Select(i => i).ToArray().Length;
                int player1King = checkers.Where(i => i.player == 1 && i.isKing == true).Select(i => i).ToArray().Length;
                int player2King = checkers.Where(i => i.player == 2 && i.isKing == true).Select(i => i).ToArray().Length;

                foreach (CheckerPiece i in checkers)
                {//Takes the x and y locations and converts them into x and y coordinates(pixels)
                    i.calcPiecePix();
                    i.calcNextLocationsPix();
                }                

                //Points from Taking a piece
                if (player1Count > player1OldPieceCount)
                {
                    player2Score += player1Count - player1OldPieceCount;
                    player1OldPieceCount = player1Count;
                }
                if (player2Count > player2OldPieceCount)
                {
                    player1Score += player2Count - player2OldPieceCount;
                    player2OldPieceCount = player2Count;
                }

                //Points from kinging
                if (player1King > player1OldKingCount)
                {
                    player1Score += player1King * 3 - player1OldKingCount;
                    player1OldKingCount = player1King;
                }
                if (player2King > player2OldKingCount)
                {
                    player2Score += player2King * 3 - player2OldKingCount;
                    player2OldKingCount = player2King;
                }
                if (player1Count == 12)
                {
                    winner = "2";
                    gameRun = false;
                    player2Score += 8;
                    player1Score += 3;
                }
                if (player2Count == 12)
                {
                    winner = "1";
                    gameRun = false;
                    player2Score += 3;
                    player1Score += 8;
                }
                if (!gameRun)
                {
                    player2Score += 100 - (player2Time.Minutes * 60 + player2Time.Seconds) / 6;
                    player1Score += 100 - (player1Time.Minutes * 60 + player1Time.Seconds) / 6;
                    player1Score -= player1Count / 4;
                    player2Score -= player2Count / 4;
                    scores.recordScores(player1Name, player2Name, player1Score, player2Score);
                }

                //This adds the time to the players time based on who's turn it is
                if (playerTurn == 1)
                    player1Time += playerTempTime;
                if (playerTurn == 2)
                    player2Time += playerTempTime;
            }
            base.Update(gameTime);
        }

        //this is the method that actually draws the board and pieces
        /// <summary>
        /// Draws the object textures to the display.
        /// </summary>
        /// <param name="gameTime">Pass in the game time</param>
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

                //Draws the player's turn
                spriteBatch.DrawString(labelFont, "Player", new Vector2(880f * scale, 2f * scale), Color.Yellow);
                spriteBatch.DrawString(playerNumFont, playerTurn.ToString(), new Vector2(890f * scale, 5f * scale), Color.Yellow);
            }
            else
            {
                spriteBatch.DrawString(endGameFont, "Player " + winner + " wins!", new Vector2(50f * scale, 50f * scale), Color.Black);
            }
            //Displays Player 1's clock time
            spriteBatch.DrawString(playerTimeFont, "Player 1 stats:", new Vector2(839f * scale, 280f * scale), Color.Yellow);
            spriteBatch.DrawString(playerTimeFont, "Timer: " + this.player1Time.ToString(@"mm\:ss"), new Vector2(850f * scale, 310f * scale), Color.Yellow);
            spriteBatch.DrawString(playerTimeFont, "Score: " + this.player1Score.ToString(), new Vector2(850f * scale, 330f * scale), Color.Yellow);
            spriteBatch.Draw(p1Label, new Vector2(850f * scale, 360 * scale), null, Color.White, 0f, Vector2.Zero, scale * .15f, SpriteEffects.None, 0f);
            spriteBatch.DrawString(playerTimeFont, " Left: " + (12 - this.player1Count).ToString(), new Vector2(850f * scale, 350f * scale), Color.Yellow);
            //Displays Player 2's clock time
            spriteBatch.DrawString(playerTimeFont, "Player 2 stats:", new Vector2(839f * scale, 395f * scale), Color.Yellow);
            spriteBatch.DrawString(playerTimeFont, "Timer: " + this.player2Time.ToString(@"mm\:ss"), new Vector2(850f * scale, 425f * scale), Color.Yellow);
            spriteBatch.DrawString(playerTimeFont, "Score: " + this.player2Score.ToString(), new Vector2(850f * scale, 445 * scale), Color.Yellow);
            spriteBatch.Draw(p2Label, new Vector2(850f * scale, 475 * scale), null, Color.White, 0f, Vector2.Zero, scale * .15f, SpriteEffects.None, 0f);
            spriteBatch.DrawString(playerTimeFont, " Left: " + (12 - this.player2Count).ToString(), new Vector2(850f * scale, 465f * scale), Color.Yellow);
            spriteBatch.Draw(gameCursor.image, new Vector2(gameCursor.x, gameCursor.y), null,
                Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.End();  //ends the sequence of drawing
            base.Draw(gameTime); 
        }
       
        /// <summary>
        /// Handles the click events and registers the location to check against the checker piece location
        /// </summary>
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

        /// <summary>
        /// Determines the next possible locations of the checker pieces.
        /// </summary>
        /// <param name="chosen">The index of the checkerpiece to check next moves.</param>
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
