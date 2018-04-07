using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris_Clone
{
    public class Piece
    {
        #region Properties

        private int positionY;
        public int PositionY
        {
            get { return positionY; }
            set { positionY = value; }
        }

        private int positionX;
        public int PositionX
        {
            get { return positionX; }
            set { positionX = value; }
        }
        public int RightMostX
        {
            get
            {
                return this.Shape.GetLength(0) + this.PositionX;
            }
        }
        public int BottomMostY
        {
            get
            {
                return this.Shape.GetLength(1) + this.PositionY;
            }
        }

        public ShapeEnum[,] Shape;

        private PieceEnum pieceType;

        public PieceEnum PieceType
        {
            get { return pieceType; }
            set { pieceType = value; }
        }
        

        #endregion Properties
        public Piece(PieceEnum pieceToCreate, int posX, int posY)
        {
            this.positionX = posX;
            this.positionY = posY;
            this.pieceType = pieceToCreate;

            switch (pieceToCreate)
            {
                case PieceEnum.I:
                    ShapeIPiece();
                    break;
                case PieceEnum.L:
                    ShapeLPiece();
                    break;
                case PieceEnum.J:
                    ShapeJPiece();
                    break;
                case PieceEnum.T:
                    ShapeTPiece();
                    break;
                case PieceEnum.O:
                    ShapeOPiece();
                    break;
                case PieceEnum.S:
                    ShapeSPiece();
                    break;
                case PieceEnum.Z:
                    ShapeZPiece();
                    break;
                default:
                    throw new Exception("Piece not implemented");
            }
        }

        public void Deactivate()
        {
            for (int i = 0; i < this.Shape.GetLength(0); i++)
            {
                for (int j = 0; j < this.Shape.GetLength(1); j++)
                {
                    if (this.Shape[i, j] == ShapeEnum.Active)
                    {
                        this.Shape[i, j] = ShapeEnum.Filled;
                    }
                }
            }

        }

        public void RotateClockwise()
        {
            int origShapeX = Shape.GetLength(0);
            int origShapeY = Shape.GetLength(1);


            var original = Shape;

            Shape = InitialiseEmptyShape(new ShapeEnum[origShapeY, origShapeX]);

            int newShapeMaxX = Shape.GetUpperBound(0);

            for (int y = 0; y < original.GetLength(1); y++)
            {
                for (int x = 0; x < original.GetLength(0); x++)
                {
                    Shape[newShapeMaxX - y, x] = original[x, y];
                }
                
            }
        }

        public void RotateAntiClockwise()
        {
            int origShapeX = Shape.GetLength(0);
            int origShapeY = Shape.GetLength(1);
            var original = Shape;

            Shape = InitialiseEmptyShape(new ShapeEnum[origShapeY, origShapeX]);

            int newShapeMaxX = Shape.GetUpperBound(0);
            int newShapeMaxY = Shape.GetUpperBound(1);

            for (int y = 0; y < original.GetLength(1); y++)
            {
                for (int x = 0; x < original.GetLength(0); x++)
                {
                    Shape[y,newShapeMaxY - x] = original[x, y];
                }

            }
        }
        
        #region Shape forming

        private ShapeEnum[,] InitialiseEmptyShape(ShapeEnum[,] emptyShape)
        {
            for (int x = 0; x < emptyShape.GetLength(0); x++)
            {
                for (int y = 0; y < emptyShape.GetLength(1); y++)
                {
                    emptyShape[x, y] = ShapeEnum.Empty;
                }
            }
            return emptyShape;
        }
        private void ShapeZPiece()
        {
            this.Shape = new ShapeEnum[3, 2];
            this.Shape[0, 0] = ShapeEnum.Active;
            this.Shape[0, 1] = ShapeEnum.Empty;
            this.Shape[1, 0] = ShapeEnum.Active;
            this.Shape[1, 1] = ShapeEnum.Active;
            this.Shape[2, 0] = ShapeEnum.Empty;
            this.Shape[2, 1] = ShapeEnum.Active;
        }

        private void ShapeSPiece()
        {
            this.Shape = new ShapeEnum[3, 2];
            this.Shape[0, 0] = ShapeEnum.Empty;
            this.Shape[0, 1] = ShapeEnum.Active;
            this.Shape[1, 0] = ShapeEnum.Active;
            this.Shape[1, 1] = ShapeEnum.Active;
            this.Shape[2, 0] = ShapeEnum.Active;
            this.Shape[2, 1] = ShapeEnum.Empty;
        }

        private void ShapeOPiece()
        {
            this.Shape = new ShapeEnum[2, 2];
            this.Shape[0, 0] = ShapeEnum.Active;
            this.Shape[0, 1] = ShapeEnum.Active;
            this.Shape[1, 0] = ShapeEnum.Active;
            this.Shape[1, 1] = ShapeEnum.Active;
        }

        private void ShapeJPiece()
        {
            this.Shape = new ShapeEnum[2, 3];
            this.Shape[0, 0] = ShapeEnum.Empty;
            this.Shape[0, 1] = ShapeEnum.Empty;
            this.Shape[0, 2] = ShapeEnum.Active;
            this.Shape[1, 0] = ShapeEnum.Active;
            this.Shape[1, 1] = ShapeEnum.Active;
            this.Shape[1, 2] = ShapeEnum.Active;
        }

        private void ShapeLPiece()
        {
            this.Shape = new ShapeEnum[2, 3];
            this.Shape[0, 0] = ShapeEnum.Active;
            this.Shape[0, 1] = ShapeEnum.Active;
            this.Shape[0, 2] = ShapeEnum.Active;
            this.Shape[1, 0] = ShapeEnum.Empty;
            this.Shape[1, 1] = ShapeEnum.Empty;
            this.Shape[1, 2] = ShapeEnum.Active;
        }

        private void ShapeTPiece()
        {
            this.Shape = new ShapeEnum[3, 2];
            this.Shape[0, 0] = ShapeEnum.Empty;
            this.Shape[0, 1] = ShapeEnum.Active;
            this.Shape[1, 0] = ShapeEnum.Active;
            this.Shape[1, 1] = ShapeEnum.Active;
            this.Shape[2, 0] = ShapeEnum.Empty;
            this.Shape[2, 1] = ShapeEnum.Active;
        }

        private void ShapeIPiece()
        {
            this.Shape = new ShapeEnum[1, 4];
            this.Shape[0, 0] = ShapeEnum.Active;
            this.Shape[0, 1] = ShapeEnum.Active;
            this.Shape[0, 2] = ShapeEnum.Active;
            this.Shape[0, 3] = ShapeEnum.Active;
        }

        #endregion Shape forming
    }
}
