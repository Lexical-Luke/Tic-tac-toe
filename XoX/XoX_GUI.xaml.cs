#region  All the using directives
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using ThinkLib;
#endregion

namespace XoX
{
   
    public partial class XoX_GUI : Window
    {

        #region Class variable definitions, and initialization of the class

        // The board is represented as an array of int
        int[] theBoard = new int[9];

        // Every cell on the board holds one of these three values:
        const int COMPUTER = -1;
        const int HUMAN = 1;
        const int UNPLAYED = 0;

        int wins = 0;
        int losses = 0;
        int draws = 0;
                      

        // Constructor to initialize everything, and start a new game...
        public XoX_GUI()
        {
            InitializeComponent();
            tess = new Turtle(playground);
            tess.Visible = false;
            tess.BrushDown = false;

            tess.TextFontFamily = new FontFamily("Showcard Gothic");
            tess.TextFontSize = 48;
            tess.TextFontWeight = FontWeights.Bold;
            this.Show();  // Force the main window to display before we pop up the MessageBox.
            MessageBox.Show("Click on a square, or enter its number on the keyboard.", "Welcome to the amazing XoX program");
            startNewGame(HUMAN);
        }
        #endregion


        #region The Model part of the application: the logic for how the game works...

        int[][] winLines = 
            {
            new int[] { 0, 1, 2 },
            new int[] { 3, 4, 5 },
            new int[] { 6, 7, 8 },
            new int[] { 0, 3, 6 },
            new int[] { 1, 4, 7 },
            new int[] { 2, 5, 8 },
            new int[] { 0, 4, 8 },
            new int[] { 2, 4, 6 }
            };  


        private void startNewGame(int whoPlaysFirst)
        {
            // Clear the board to start a new game;
            for (int i = 0; i < theBoard.Length; i++)
            {
                theBoard[i] = UNPLAYED;
            }

            // If the computer has to play first, make some move.
            // No need to check for winner yet, or for a draw on the first move.
            if (whoPlaysFirst == COMPUTER)  
            {
                makeComputerMove();
            }

            updateView();
        }


        private void tryPlayCell(int whichCell)
        {
            

            // This is called whenever the human makes a move.
            // If the human makes an invalid move, do nothing further.
            if (! makeHumanMove(whichCell)) return;
         
            // Check for human win, and respond appropriately. 
            if (haveWinner(HUMAN))
            {
                MessageBox.Show("You start first next time.", "You win!");
                wins++;
                lblWins.Content = wins;
                startNewGame(HUMAN);
                return;
            }

            // Check for a draw, and respond appropriately.
            if (isBoardFull())
            {
                // Declare a draw, start a new game or end the program
                MessageBox.Show("I'll start first this time", "A draw!");
                draws++;
                lblDraws.Content = draws;
                startNewGame(COMPUTER);
                return;
            }
           
            // If we get this far in this method, it means the human has
            // made a valid move, and the game is not over.  So it is the 
            // computer's turn now ...
            makeComputerMove();

            if (haveWinner(COMPUTER))
            {
                // Start a new game or end the program
                MessageBox.Show("I'll start first next time.", "I win!");
                losses++;
                lblLosses.Content = losses;
                startNewGame(COMPUTER);
                return;
            }

            if (isBoardFull())
            {
                // Declare a draw, start a new game
                MessageBox.Show("You play first next time.", "A draw!");
                draws++;
                lblDraws.Content = draws;
                startNewGame(HUMAN);
                return;
            }
        }


        private bool haveWinner(int who)
        {
            foreach (int[] winLine in this.winLines)
            {
                if (this.isWinningLine(who, winLine[0], winLine[1], winLine[2]))
                    return true;
            }
            return false;
        }


        private bool isWinningLine(int who, int u, int v, int w)
        {   // Return true if 'who' owns the three cells at positions u,v,w
            return this.theBoard[u] == who && this.theBoard[v] == who && this.theBoard[w] == who;
        }

        
        private bool isBoardFull()
        {
            // This is complete - it works, no need to change anything here
            for (int i = 0; i < theBoard.Length; i++)
            {
                if (theBoard[i] == UNPLAYED) return false;
            }
            return true;
        }


        private void makeComputerMove()
        {   // Pre-condition: there is always at least one open place to play...

            int pos = chooseComputerPlaceToPlay();
            theBoard[pos] = COMPUTER;
            updateView();
        }


        private int chooseComputerPlaceToPlay()
        {
            // Pre-condition: there is always at least one open place to play...

            // For the moment, just play in the first available cell.

            foreach (int[] winLine in this.winLines)
            {
                int num = this.winPos(-1, winLine[0], winLine[1], winLine[2]);

                if (num >= 0)
                {
                    return num;
                }                
            }

            foreach (int[] winLine in this.winLines)
            {
                int num = this.winPos(1, winLine[0], winLine[1], winLine[2]);

                if (num >= 0)
                {
                    return num;
                }      
            }

            foreach (int i in new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 })
            {
                if (theBoard[i] == UNPLAYED)
                {
                    return i;
                }
            }

            return -1;
        }


        private int winPos(int who, int u, int v, int w)
        {  // Return one of u, v or w if 'who' can win by playing there.  Or return -1.
            if (this.theBoard[u] == 0 && this.theBoard[v] == who && this.theBoard[w] == who)
            {
                return u;
            }
                

            if (this.theBoard[u] == who && this.theBoard[v] == 0 && this.theBoard[w] == who)
            {
                return v;
            }
                

            if (this.theBoard[u] == who && this.theBoard[v] == who && this.theBoard[w] == 0)
            {
                return w;
            }
                

            return -1;
        }

        private bool makeHumanMove(int whichCell)
        {
            if (whichCell < 0)
                return false;
            if (this.theBoard[whichCell] != 0)
            {
                int num = (int)MessageBox.Show("The cell is already taken", "Oops.");
                return false;
            }
            this.theBoard[whichCell] = 1;
            this.updateView();
            return true;
        }

        #endregion


        #region The Controller part of the application:  handling mouse clicks and keypresses from the human.


        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            List<Key> digits = new List<Key>() { Key.D1, Key.D2, Key.D3, Key.D4, Key.D5, Key.D6, Key.D7, Key.D8, Key.D9 };
            int indx = digits.IndexOf(e.Key);
            if (indx >= 0)
            {
                tryPlayCell(indx);
            }
        }

        // We fix and know the canvas size (750x750), so we hard-wire these points on the image.
        Point[] cellMidPoints = 
                  { new Point(125, 125), new Point(375, 125), new Point(625, 125),
                    new Point(125, 375), new Point(375, 375), new Point(625, 375),
                    new Point(125, 625), new Point(375, 625), new Point(625, 625) };

        // If the mouse is clicked within this distance to the midpoint of the cell, we'll
        // take it as an attempt by the user to play in that cell.
        double cellRadius = 100;

        private void playground_MouseDown(object sender, MouseButtonEventArgs e)
        {
            int inxd = findCellAtPosition(e.GetPosition(playground));
            tryPlayCell(inxd);
        }

        private int findCellAtPosition(Point mousePos)
        {
            for (int i = 0; i < cellMidPoints.Length; i++)
            {
                if (distance(mousePos, cellMidPoints[i]) < cellRadius)
                {
                    return i;
                }
            }
            return -1;
        }

        private double distance(Point p1, Point p2)
        {
            double dx = p1.X - p2.X;
            double dy = p1.Y - p2.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        #endregion


        #region The Viewer part of the application

        // We use a turtle to draw the state of the game on the background
        Turtle tess;


        // Redraw the state of the game.
        private void updateView()
        {
            tess.Clear();
            for (int i = 0; i < theBoard.Length; i++)
            {
                tess.Goto(cellMidPoints[i]);

                switch (theBoard[i])
                {
                    case 1:
                        tess.TextBrush = Brushes.IndianRed;
                        tess.Stamp("X", -48, -72);
                        break;
                    case -1:
                        tess.TextBrush = Brushes.Blue;
                        tess.Stamp("0", -48, -72);
                        break;
                    case 0:
                        tess.TextBrush = Brushes.Gray;
                    //   tess.Stamp(Convert.ToString(i + 1), -48, -72);
                        break;
                }
            }
        }


        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // When the window is resized we don't allow the canvas to get bigger.
            // It always stays at its starting width and height, which we know and 
            // hard-code into this program.   Instead, we scale the rendering of
            // the canvas, so that it draws bigger or smaller without actually
            // becoming bigger or smaller. 

            // This event automatically fires when the window is first shown.

            double availableWidth = grid1.ActualWidth - 2* playground.Margin.Left;
            double availableHeight = grid1.ActualHeight -  (playground.Margin.Top) - 20;
            double xScale = availableWidth / playground.Width;
            double yScale = availableHeight / playground.Height;

            TransformGroup tg = new TransformGroup();
            tg.Children.Add(new ScaleTransform(xScale, yScale));
            // tg.Children.Add(new RotateTransform(60, 350, 350));
            //tg.Children.Add(new SkewTransform(20, 20));

             playground.RenderTransform = tg;
        }

        #endregion
 

    }
}
