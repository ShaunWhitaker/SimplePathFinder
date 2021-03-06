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
using System.Data;
using System.Windows.Forms;

namespace Path_Finder
{
	public class Grid
	{
		public int Height { get; set;}
		public int Width { get; set; }

		public Cell[,] grid;

		public Grid ()
		{

		}

		public Cell this[int x, int y]
		{
			get { return grid[x, y]; }
		}
        public Cell GetCell(int x, int y)
        {
            if (x < 0)
                x = Width - 1;

            if (x > Width - 1)
                x = 0;

            if (y < 0)
                y = Height - 1;

            if (y > Height - 1)
                y = 0;

            return grid[x, y];
        }


		public void NewGrid(int height, int width)
		{

            Height = height;
            Width = width;
			grid = new Cell[height, width];
            for (int i = 0; i < height; i++)
            {
                for (int x = 0; x < width; x++)
                {
                    Cell cell = new Cell(grid)
                    {
                        Coordinates = new Point
                        {
                            X = i,
                            Y = x
                        }
                    };
                    grid[i, x] = cell;
                }
            }
		}

		public void SetCell(Cell cell)
		{
            try
            {
                grid[cell.Coordinates.X, cell.Coordinates.Y] = cell;
            }
            catch (Exception)
            {

            }

		}

        public System.Data.DataTable GridAsDataTable()
        {
            DataTable dt = new DataTable();

            for (int column = 0; column < Width; column++)
            {
                dt.Columns.Add();
            }

            for (int row = 0; row < Height - 1; row++)
            {
                dt.Rows.Add();
            }

            return dt;
        }
		
	}
}

