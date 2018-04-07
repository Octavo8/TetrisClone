using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tetris_Clone;

namespace Test_Display
{
    class Program
    {
        public static int xOffset = 10;
        public static int yOffset = 5;
        static void Main(string[] args)
        {
            Console.CursorVisible = false;




            var game = new Game(800,15,10, 5);
           
            DrawPlayArea(game);
            DrawPiece(game);
            
            var timer = Stopwatch.StartNew();
            bool running = true;
            while (running)
            {
                if (Console.KeyAvailable)
                {
                    var input = Console.ReadKey(true);
                    game.PlayerInput(MapKeypressToInput(input));
                    RefreshPlayArea(game);
                    DrawPiece(game);
                }
                if (timer.ElapsedMilliseconds >= game.GameTick)
                {
                    timer.Restart();
                    game.TriggerGameTick();
                    RefreshPlayArea(game);
                    DrawPiece(game);
                }
            }

            Console.ReadLine();
        }

        private static PlayerInput MapKeypressToInput(ConsoleKeyInfo input)
        {
            switch (input.Key)
            {
                case ConsoleKey.DownArrow:
                    return PlayerInput.Down;
                case ConsoleKey.LeftArrow:
                    return PlayerInput.Left;
                case ConsoleKey.RightArrow:
                    return PlayerInput.Right;
                case ConsoleKey.Q:
                    return PlayerInput.RotateClockwise;
                case ConsoleKey.W:
                    return PlayerInput.RotateAntiClockwise;
                default:
                    return PlayerInput.Unknown;
            }
        }
        private static void RefreshPlayArea(Game game)
        {

            for (int j = 0; j < game.Height; j++)
            {
                for (int i = 1; i < game.Width+1; i++)
                {
                    MoveWrite(i + xOffset-1, j + yOffset, " ");
                }
            }
            
        }
        static void DrawPlayArea(Game currentGame)
        {
            int maxX = currentGame.Width + 1;
            int currentX = xOffset - 1;
            int currentY = yOffset;

            Console.Clear();
            MoveWrite(currentX, currentY, "╗");

            currentX += maxX;

            MoveWrite(currentX, currentY, "╔");

            for (int i = 0; i < currentGame.Height-1; i++)
            {
                currentX = xOffset -1;
                currentY += 1;
                MoveWrite(currentX, currentY, "║");
                currentX += maxX;
                MoveWrite(currentX, currentY, "║");
            }
            currentX = xOffset -1;
            currentY += 1;
            MoveWrite(currentX, currentY, "╚");
            currentX += maxX;
            MoveWrite(currentX, currentY, "╝");

            currentX -= maxX - 1;

            for (int i = 0; i < maxX - 1; i++)
            {
                MoveWrite(currentX, currentY, "═");
                currentX += 1;
            }
        }
        static void MoveWrite(int x, int y, string character)
        {
            Console.SetCursorPosition(x, y);
            Console.Write(character);
        }
        static void DrawPiece(Game currentGame)
        {
            for (int j = 0; j < currentGame.CurrentPiece.Shape.GetLength(1); j++)
            {
                for (int i = 0; i < currentGame.CurrentPiece.Shape.GetLength(0); i++)
                {
                    if (currentGame.CurrentPiece.Shape[i, j] == ShapeEnum.Active)
                    {
                        Console.SetCursorPosition(i + currentGame.CurrentPiece.PositionX + xOffset, j + currentGame.CurrentPiece.PositionY + yOffset);
                        Console.Write("█");
                    }
                }
            }

            for (int x = 0; x < currentGame.DeadGrid.GetLength(0); x++)
            {
                for (int y = 0; y < currentGame.DeadGrid.GetLength(1); y++)
                {
                    if (currentGame.DeadGrid[x,y] == ShapeEnum.Filled)
                    {
                        Console.SetCursorPosition(x + xOffset, y + yOffset);
                        Console.Write("▒");
                    }
                }
            }
        }
    }
}
