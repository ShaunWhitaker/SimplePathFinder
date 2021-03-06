//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;

namespace Path_Finder
{
    public class Cell
    {
        public bool isObstical { get; set; }
        public Point Coordinates { get; set; }
        public bool hasSteppedOn { get; set; }
        public bool IsEndPoint { get; set; }

        public bool HasWalkablePath(Grid grid)
        {
            List<Cell> neighbourCells = new List<Cell>
                {
                    grid.GetCell(Coordinates.X, Coordinates.Y + 1),
                    grid.GetCell(Coordinates.X, Coordinates.Y - 1),
                    grid.GetCell(Coordinates.X - 1, Coordinates.Y),
                    grid.GetCell(Coordinates.X + 1, Coordinates.Y)
                };

            if (neighbourCells.Where(x => !x.hasSteppedOn && !x.isObstical).Any())
            {
                return true;
            }
            return false;
        }

    public Cell(Cell[,] _grid)
    {
        //grid = _grid;
        //Height = grid.GetLength(0);
        //Width = grid.GetLength(1);
    }
}
}

