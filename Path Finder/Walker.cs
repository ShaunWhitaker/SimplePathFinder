using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Path_Finder
{
    class Walker : Cell
    {
        public Stack<Point> MoveOrder { get; set; }

        public Walker(Cell[,] grid, int x, int y) : base(grid)
        {
            MoveOrder = new Stack<Point>();
            Coordinates = new Point
            {
                X = x,
                Y = y
            };

            MoveOrder.Push(
                        new Point
                        {
                            X = Coordinates.X,
                            Y = Coordinates.Y
                        });

            grid[x, y].hasSteppedOn = true;
        }

        public void Walk(Grid grid, Cell cellToMoveTo, bool walkingBack = false, bool FoundThing = false)
        {
            this.Coordinates.Y = cellToMoveTo.Coordinates.Y;
            this.Coordinates.X = cellToMoveTo.Coordinates.X;

            cellToMoveTo.hasSteppedOn = true;

            if (!walkingBack)
            {
                MoveOrder.Push(new Point
                {
                    X = Coordinates.X,
                    Y = Coordinates.Y
                });
            }
            else if (cellToMoveTo.HasWalkablePath(grid) && !FoundThing)
            {
                MoveOrder.Push(new Point
                {
                    X = Coordinates.X,
                    Y = Coordinates.Y
                });
            }
        }
    }
}
