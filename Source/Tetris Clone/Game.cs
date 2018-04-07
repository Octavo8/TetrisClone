using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris_Clone
{
    public class Game
    {
        #region Properties

        private Piece piece;
        private int gameTick;
        private ShapeEnum[,] deadGrid;
        private bool isTicking;
        private bool isGameOver;

        public event EventHandler GameOver;

        public event EventHandler<ShapeEnum[,]> LinesAboutToClear;
        public event EventHandler<int[]> LinesCleared;
        public int GameTick
        {
            get { return gameTick; }
            set { gameTick = value; }
        }
        public int StartXPosition { get; set; }
        public int StartYPosition { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Piece CurrentPiece
        {
            get { return piece; }
            set { piece = value; }
        }
        public ShapeEnum[,] DeadGrid
        {
            get
            {
                return deadGrid;
            }
            set
            {
                deadGrid = value;
            }
        }

        private Piece nextPiece;

        public Piece NextPiece
        {
            get { return nextPiece; }
            set { nextPiece = value; }
        }
        

        private int totalLinesCleared;

        public int TotalLinesCleared
        {
            get { return totalLinesCleared; }
            private set { totalLinesCleared = value; }
        }
        

        #endregion Properties
        public Game(int tickInterval, int height, int width, int startXPosition)
        {
            isGameOver = false;
            isTicking = false;
            gameTick = tickInterval;
            totalLinesCleared = 0;

            this.StartXPosition = startXPosition;
            this.StartYPosition = 0;
            this.CurrentPiece =  AddNewPiece();
            this.NextPiece = AddNewPiece();
            this.Height = height;
            this.Width = width;

            InitialiseDeadGrid();

        }
        public void PlayerInput(PlayerInput input)
        {
            if (isTicking)
            {
                return;
            }

            switch (input)
            {
                case Tetris_Clone.PlayerInput.Down:
                    TriggerGameTick();
                    break;
                case Tetris_Clone.PlayerInput.Left:
                    if (piece.PositionX > 0 )
                    {
                        piece.PositionX -= 1;
                        if (CheckForDeactivations())
                        {
                            piece.PositionX += 1;
                        }
                    }                    
                    break;
                case Tetris_Clone.PlayerInput.Right:
                    if (piece.RightMostX < this.Width)
                    {
                        piece.PositionX += 1;
                        if (CheckForDeactivations())
                        {
                            piece.PositionX -= 1;
                        }
                    }                    
                    break;
                case Tetris_Clone.PlayerInput.RotateAntiClockwise:
                    piece.RotateAntiClockwise();
                    CorrectPiecePositionAfterRotation();
                    break;
                case Tetris_Clone.PlayerInput.RotateClockwise:
                    piece.RotateClockwise();
                    CorrectPiecePositionAfterRotation();
                    break;
                default:
                    break;
            }
        }
        public void TriggerGameTick()
        {
            if (isGameOver)
            {
                return;
            }

            isTicking = true;
            piece.PositionY += 1;

            if (CheckForDeactivations())
            {
                piece.PositionY -= 1;
                piece.Deactivate();
                AddPieceToDeadGrid(piece);
                ClearLinesFromDeadGrid();
                piece = nextPiece;
                nextPiece = AddNewPiece();
                if (CheckForDeactivations())
                {
                    isGameOver = true;
                    GameOver.Invoke(this, new EventArgs());
                }
            }
            isTicking = false;
        }
        private bool CheckForDeactivations()
        {
            if (piece.BottomMostY > this.Height)
            {
                return true;
            }

            for (int x = 0; x < piece.Shape.GetLength(0); x++)
            {
                for (int y = 0; y < piece.Shape.GetLength(1); y++)
                {
                    if (piece.Shape[x,y] != ShapeEnum.Empty &&
                        deadGrid[piece.PositionX + x, piece.PositionY + y] != ShapeEnum.Empty)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        private void ClearLinesFromDeadGrid()
        {
            var oldGrid = (ShapeEnum[,])deadGrid.Clone();
            List<int> yLinesToClear = new List<int>();

            for (int y = 0; y < deadGrid.GetLength(1); y++)
            {
                bool fullLine = true;
                for (int x = 0; x < deadGrid.GetLength(0); x++)
                {
                    if (deadGrid[x, y] == ShapeEnum.Empty)
                    {
                        fullLine = false;                        
                        break;
                    }
                }
                if (fullLine)
                {
                    yLinesToClear.Add(y);

                    for (int ytoClear = y; ytoClear > 0; ytoClear--)
                    {
                        for (int x = 0; x < deadGrid.GetLength(0); x++)
                        {
                            deadGrid[x, ytoClear] = deadGrid[x, ytoClear - 1];
                        }
                    }
                }
            }

            if (yLinesToClear.Any())
            {
                totalLinesCleared += yLinesToClear.Count();
                LinesAboutToClear.Invoke(this, oldGrid);
                LinesCleared.Invoke(this, yLinesToClear.ToArray());
            }
            

        }
        private void AddPieceToDeadGrid(Piece piece)
        {
            for (int x = 0; x < piece.Shape.GetLength(0); x++)
            {
                for (int y = 0; y < piece.Shape.GetLength(1); y++)
                {
                    if (piece.Shape[x, y] != ShapeEnum.Empty)
                    {
                        deadGrid[piece.PositionX + x, piece.PositionY + y] = ShapeEnum.Filled;
                    }
                }
            }
        }
        private Piece AddNewPiece()
        {
            Random rand = new Random();
            var randomNumber = rand.Next(0, 7);
            return new Piece((PieceEnum)randomNumber, this.StartXPosition, this.StartYPosition);
        }
        private void CorrectPiecePositionAfterRotation()
        {
            if (piece.RightMostX > this.Width)
            {
                piece.PositionX -= piece.RightMostX - this.Width;
            }
        }
        private void InitialiseDeadGrid()
        {
            deadGrid = new ShapeEnum[this.Width, this.Height];
            for (int x = 0; x < deadGrid.GetLength(0); x++)
            {
                for (int y = 0; y < deadGrid.GetLength(1); y++)
                {
                    deadGrid[x, y] = ShapeEnum.Empty;
                    
                }
            }
        }
    }
}
