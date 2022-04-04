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
            //set the current coordinates of the walker to the cell that we are moving to.
            this.Coordinates.Y = cellToMoveTo.Coordinates.Y;
            this.Coordinates.X = cellToMoveTo.Coordinates.X;

            //update the cell to indicate that it has been stepped on.
            cellToMoveTo.hasSteppedOn = true;

            //if the walker is not moving backwards push the new cell into the move order.
            if (!walkingBack)
            {
                MoveOrder.Push(new Point
                {
                    X = Coordinates.X,
                    Y = Coordinates.Y
                });
            }
            //if it is moving backwards + it has a new walkable path, add it to the move order.
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
