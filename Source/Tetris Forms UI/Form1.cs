using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tetris_Clone;

namespace Tetris_Forms_UI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private Game game;
        public static int xOffset = 10;
        public static int yOffset = 5;
        private BufferedGraphics buffer;
        private int[] linesToClear;
        private ShapeEnum[,] localDeadGrid;
        Image tex;

        private void Form1_Load(object sender, EventArgs e)
        {
            BufferedGraphicsContext context = BufferedGraphicsManager.Current;
            context.MaximumBuffer = new Size(this.Width + 1, this.Height + 1);
            buffer = context.Allocate(this.CreateGraphics(), new Rectangle(0, 0, this.Width, this.Height));
            StartNewGame();
        }

        private void StartNewGame()
        {
            game = new Game(800, 20, 10, 4);

            localDeadGrid = game.DeadGrid;
            game.GameOver += game_GameOver;
            game.LinesCleared += game_LinesCleared;
            game.LinesAboutToClear += game_LinesAboutToClear;
            gameTimer.Start();
            isGameOver = false;
            DrawGame();
        }

        void game_LinesAboutToClear(object sender, ShapeEnum[,] e)
        {
            localDeadGrid = e;
        }


        private bool isGameOver;
        void game_GameOver(object sender, EventArgs e)
        {
            isGameOver = true;
        }


        private Color GetColorByPieceType(PieceEnum pieceEnum)
        {
            switch (pieceEnum)
            {
                case PieceEnum.I:
                    return Color.Red;
                case PieceEnum.L:
                    return Color.Yellow;
                case PieceEnum.J:
                    return Color.Purple;
                case PieceEnum.T:
                    return Color.Brown;
                case PieceEnum.O:
                    return Color.Blue;
                case PieceEnum.S:
                    return Color.Green;
                case PieceEnum.Z:
                    return Color.Teal;
                default:
                    return Color.DarkGray;
            }
        }

        private static PlayerInput MapKeypressToInput(KeyEventArgs input)
        {
            switch (input.KeyCode)
            {
                case Keys.Down:
                    return PlayerInput.Down;
                case Keys.Left:
                    return PlayerInput.Left;
                case Keys.Right:
                    return PlayerInput.Right;
                case Keys.Q:
                    return PlayerInput.RotateClockwise;
                case Keys.W:
                    return PlayerInput.RotateAntiClockwise;
                default:
                    return PlayerInput.Unknown;
            }
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {

        }


        private void timer_Tick(object sender, EventArgs e)
        {
            game.TriggerGameTick();
            DrawGame();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.N:
                    if (isGameOver)
                    {
                        StartNewGame();
                    }
                    break;
                case Keys.P:
                    game.CurrentPiece = new Piece(PieceEnum.I, 5, 0);
                    break;
                case Keys.Up:
                    game.CurrentPiece.PositionY -= 1;
                    DrawGame();
                    break;
                default:
                    game.PlayerInput(MapKeypressToInput(e));
                    DrawGame();
                    break;
            }
        }

        void game_LinesCleared(object sender, int[] e)
        {
            //gameTimer.Stop();
            tickCount = 0;
            linesToClear = e; 
            timLinesClearing.Start();
        }

        int tickCount;

        private void timLinesClearing_Tick(object sender, EventArgs e)
        {
            tickCount++;

            Graphics g = buffer.Graphics;
            g.Clear(Color.Black);
            DrawPiece(game, g);
            RefreshPlayArea(game, g);
            
            int multiplier = 20;
            int brushSize = 20;
            int squareSize = 20;

            var clearBrush = new SolidBrush(Color.DarkSlateBlue);
            var textBrush = new SolidBrush(Color.GhostWhite);
            var pen = new Pen(clearBrush, brushSize);

            string textToDraw = GetLineClearText(linesToClear.Count());

            foreach (var item in linesToClear)
            {
                var rect = new Rectangle
                {
                    X = (xOffset * multiplier)-1,
                    Y = (yOffset + item) * multiplier,
                    Height = multiplier,
                    Width = (tickCount * 100) / 10,
                };
                g.FillRectangle(clearBrush, rect);
                
                g.DrawString(textToDraw, new Font("Arial", 8), textBrush, (xOffset * multiplier), ((yOffset + item) * multiplier)+2);
                //g.FillRectangle(brush, new Rectangle(xOffset * multiplier, (yOffset + item) * multiplier, (game.Width + xOffset) * multiplier, 10));
            }


            buffer.Render(Graphics.FromHwnd(this.Handle));
            if (tickCount == 20)
            {
                timLinesClearing.Stop();
                localDeadGrid = game.DeadGrid;
                //gameTimer.Start();
                //game.TriggerGameTick();
                //DrawGame();
            }
        }

        private string GetLineClearText(int p)
        {
            switch (p)
            {
                case 1:
                    return "Only a single? ";
                case 2:
                    return "Double. Not bad...";
                case 3:
                    return "Triple! So close!";
                case 4:
                    return "ZOMG TETRIS!";
                default:
                    return string.Empty;
            }
        }
        
        private void DrawGame()
        {
            Graphics g = buffer.Graphics;
            g.Clear(Color.Black);
            DrawPiece(game, g);
            RefreshPlayArea(game, g);

            if (isGameOver)
            {
                gameTimer.Stop();

                var brush = new SolidBrush(Color.Crimson);

                g.DrawString("GAME OVER :-(", new Font("Arial", 28), brush, 80, 50);
                g.DrawString("Press 'n' for a new game", new Font("Arial", 11), brush, 480, 300);
            }

            buffer.Render(Graphics.FromHwnd(this.Handle));
        }

        private void DrawPiece(Game game, Graphics graphics)
        {
            int multiplier = 20;
            int brushSize = 1;
            int squareSize = 19;

            var brush = new SolidBrush(GetColorByPieceType(game.CurrentPiece.PieceType));
            //var brush = new TextureBrush(tex);
            var pen = new Pen(brush, brushSize);

            for (int j = 0; j < game.CurrentPiece.Shape.GetLength(1); j++)
            {
                for (int i = 0; i < game.CurrentPiece.Shape.GetLength(0); i++)
                {
                    if (game.CurrentPiece.Shape[i, j] == ShapeEnum.Active)
                    {
                        graphics.FillRectangle(brush, new Rectangle((i + game.CurrentPiece.PositionX + xOffset) * multiplier, (j + game.CurrentPiece.PositionY + yOffset) * multiplier, squareSize, squareSize));
                    }
                }
            }


            int xRightSide = 24;
            brush = new SolidBrush(GetColorByPieceType(game.NextPiece.PieceType));
            for (int j = 0; j < game.NextPiece.Shape.GetLength(1); j++)
            {
                for (int i = 0; i < game.NextPiece.Shape.GetLength(0); i++)
                {
                    if (game.NextPiece.Shape[i, j] == ShapeEnum.Active)
                    {
                        graphics.FillRectangle(brush, new Rectangle((i + xRightSide) * multiplier, (j + yOffset) * multiplier, squareSize, squareSize));
                    }
                }
            }


            brush = new SolidBrush(Color.DarkGray);
            pen = new Pen(brush, brushSize);

            for (int x = 0; x < localDeadGrid.GetLength(0); x++)
            {
                for (int y = 0; y < localDeadGrid.GetLength(1); y++)
                {
                    if (localDeadGrid[x, y] == ShapeEnum.Filled)
                    {
                        graphics.FillRectangle(brush, new Rectangle((x + xOffset) * multiplier, (y + yOffset) * multiplier, squareSize, squareSize));
                    }
                }
            }

        }
        private void RefreshPlayArea(Game game, Graphics g)
        {
            int multiplier = 20;
            int brushSize = 20;

            var brush = new SolidBrush(Color.SteelBlue);
            var pen = new Pen(brush, brushSize);
            g.DrawLine(pen, (xOffset * multiplier) - 11, yOffset * multiplier, (xOffset * multiplier) - 11, (game.Height + yOffset) * multiplier);
            g.DrawLine(pen, ((game.Width + xOffset) * multiplier) + 10, yOffset * multiplier, ((game.Width + xOffset) * multiplier) + 10, (game.Height + yOffset) * multiplier);
            g.DrawLine(pen, (xOffset * multiplier) - brushSize - 1, ((game.Height + yOffset) * multiplier) + 10, (game.Width + xOffset) * multiplier + brushSize, ((game.Height + yOffset) * multiplier) + 10);

            g.DrawString("Lines cleared: " + game.TotalLinesCleared.ToString(), new Font("Arial", 13), brush, 440, 60);
            g.DrawString("Q & W to rotate", new Font("Arial", 13), brush, 440, 80);

        }


    }
}
