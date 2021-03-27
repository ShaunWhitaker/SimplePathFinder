using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Path_Finder
{
    public partial class Form1 : Form
    {
        private Cell pathFindingCell;
        private bool selectedStartingPoint;
        private bool selectedEndPoint;
        private Grid grid;
        private List<Cell> cellWithAltPath = new List<Cell>();
        private List<Cell> pathWalked = new List<Cell>();
        private bool walkingBack = false;
        private Walker walker;
        Thread MouseDownCountThread = null;
        bool Stop = false;

        public Form1()
        {
            InitializeComponent();
            BuildGrid();
            timer1.Enabled = false;
            lblInstructions.Text = "Select a starting point";
        }

        protected void BuildGrid()
        {
            grid = new Grid();
            grid.NewGrid(10, 10);
            DataTable gridDT = grid.GridAsDataTable();
            grvGrid.DataSource = gridDT;
            grvGrid.AutoSize = true;
            grvGrid.BackgroundColor = System.Drawing.SystemColors.Control;
            grvGrid.BorderStyle = BorderStyle.None;
            foreach (DataGridViewColumn col in grvGrid.Columns)
            {
                col.Width = 40;
            }
        }


        private void grvGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            DataGridViewTextBoxCell cell = (DataGridViewTextBoxCell)grvGrid.Rows[e.RowIndex].Cells[e.ColumnIndex];
            //Clear the cell if it's a right click
            if (e.Button == MouseButtons.Right)
            {
                var clearCell = new Cell
                {
                    RowPosition = e.RowIndex,
                    ColumnPosition = e.ColumnIndex
                };

                grid.SetCell(clearCell);
                cell.Style.BackColor = Color.White;
            }
            //else set obsticle/starting point
            else
            {
                if (!selectedStartingPoint)
                {
                    cell.Style.BackColor = Color.Red;
                    selectedStartingPoint = true;
                    pathFindingCell = new Cell
                    {
                        RowPosition = e.RowIndex,
                        ColumnPosition = e.ColumnIndex
                    };
                    walker = new Walker
                    {
                        currentCell = pathFindingCell
                    };
                    grid.SetCell(pathFindingCell);
                    lblInstructions.Text = "Select an end point.";

                }
                else if (!selectedEndPoint)
                {
                    cell.Style.BackColor = Color.Blue;
                    selectedEndPoint = true;
                    lblInstructions.Text = "build a maze and select start when ready.";
                }
                else
                {
                    cell.Style.BackColor = Color.Black;
                    var obstical = new Cell();
                    obstical.RowPosition = e.RowIndex;
                    obstical.ColumnPosition = e.ColumnIndex;
                    obstical.isObstical = true;
                    grid.SetCell(obstical);
                }
            }

        }

        
        protected void ChangeBlockColour(Color colour)
        {
            DataGridViewTextBoxCell cell = (DataGridViewTextBoxCell)grvGrid.Rows[pathFindingCell.RowPosition].Cells[pathFindingCell.ColumnPosition];
            cell.Style.BackColor = colour;
            Application.DoEvents();
        }

        private void grvGrid_SelectionChanged(object sender, EventArgs e)
        {
            grvGrid.ClearSelection();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;
        }

        protected void CheckTotalNumberOfMoveOptions(Cell cell)
        {
            cell.SetNeighbors(grid);
            int walkablePaths = 0;
            foreach (Cell c in cell.cellNeighbors)
            {
                if (!c.isWall && !c.isObstical && !c.hasSteppedOn)
                {
                    walkablePaths++;
                }
            }

            if (walkablePaths > 1)
            {
                if (!cellWithAltPath.Contains(cell))
                {
                    cellWithAltPath.Add(cell.Clone());
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!walkingBack)
            {
                bool MovedOnce = false;
                CheckTotalNumberOfMoveOptions(pathFindingCell);
                for (int i = 0; i < 4; i++)
                {
                    var neighborCell = new Cell();
                    bool pathFinderMovedRow = false;
                    bool pathFinderMovedColumn = false;
                    string directionMoved = "";
                    if (i == 0)
                    {
                        try
                        {
                            neighborCell = grid[pathFindingCell.RowPosition + 1, pathFindingCell.ColumnPosition];
                            if (!neighborCell.isObstical && (!neighborCell.hasSteppedOn || cellWithAltPath.Contains(neighborCell)))
                            {
                                pathFinderMovedRow = true;
                                directionMoved = "Down";
                            }
                        }
                        catch (Exception)
                        {
                            continue;
                        }

                    }
                    else if (i == 1)
                    {
                        try
                        {
                            neighborCell = grid[pathFindingCell.RowPosition - 1, pathFindingCell.ColumnPosition];
                            if (!neighborCell.isObstical && (!neighborCell.hasSteppedOn || cellWithAltPath.Contains(neighborCell)))
                            {
                                pathFinderMovedRow = true;
                                directionMoved = "Up";
                            }
                        }
                        catch (Exception)
                        {
                            continue;
                        }

                    }
                    else if (i == 2)
                    {
                        try
                        {
                            neighborCell = grid[pathFindingCell.RowPosition, pathFindingCell.ColumnPosition + 1];
                            if (!neighborCell.isObstical && (!neighborCell.hasSteppedOn || cellWithAltPath.Contains(neighborCell)))
                            {
                                pathFinderMovedColumn = true;
                                directionMoved = "Right";
                            }
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                    }
                    else if (i == 3)
                    {
                        try
                        {
                            neighborCell = grid[pathFindingCell.RowPosition, pathFindingCell.ColumnPosition - 1];
                            if (!neighborCell.isObstical && (!neighborCell.hasSteppedOn || cellWithAltPath.Contains(neighborCell)))
                            {
                                pathFinderMovedColumn = true;
                                directionMoved = "Left";
                            }
                        }
                        catch (Exception)
                        {
                            continue;
                        }

                    }
                    if (pathFinderMovedRow || pathFinderMovedColumn)
                    {
                        MovedOnce = true;
                        Cell cellToAssignSteppedOn = grid[pathFindingCell.RowPosition, pathFindingCell.ColumnPosition];
                        if (cellWithAltPath.Count > 0 && cellToAssignSteppedOn.hasSteppedOn == false)
                        {
                            pathWalked.Add(cellToAssignSteppedOn.Clone());
                        }
                        cellToAssignSteppedOn.hasSteppedOn = true;
                        DataGridViewTextBoxCell cell = (DataGridViewTextBoxCell)grvGrid.Rows[pathFindingCell.RowPosition].Cells[pathFindingCell.ColumnPosition];
                        cell.Style.BackColor = Color.Green;

                        switch (directionMoved)
                        {
                            case "Up":
                                pathFindingCell = grid[pathFindingCell.RowPosition - 1, pathFindingCell.ColumnPosition];
                                break;
                            case "Down":
                                pathFindingCell = grid[pathFindingCell.RowPosition + 1, pathFindingCell.ColumnPosition];
                                break;
                            case "Left":
                                pathFindingCell = grid[pathFindingCell.RowPosition, pathFindingCell.ColumnPosition - 1];
                                break;
                            case "Right":
                                pathFindingCell = grid[pathFindingCell.RowPosition, pathFindingCell.ColumnPosition + 1];
                                break;
                            default:
                                break;
                        }
                        ChangeBlockColour(Color.Red);
                    }
                }
                if (MovedOnce == false)
                {
                    if (cellWithAltPath.Count > 0)
                    {
                        walkingBack = true;
                    }
                    else
                    {
                        timer1.Enabled = false;
                        MessageBox.Show("No where for me to go!");
                    }
                }
            }
            else
            {
                if (pathWalked.Count > 0)
                {
                    Cell previousCell = pathWalked[pathWalked.Count - 1];
                    DataGridViewTextBoxCell cell = (DataGridViewTextBoxCell)grvGrid.Rows[pathFindingCell.RowPosition].Cells[pathFindingCell.ColumnPosition];
                    cell.Style.BackColor = Color.Green;
                    pathFindingCell = grid[previousCell.RowPosition, previousCell.ColumnPosition];
                    pathWalked.RemoveAt(pathWalked.Count - 1);
                    if (cellWithAltPath.Contains(pathFindingCell))
                    {
                        cellWithAltPath.Remove(pathFindingCell);
                    }
                    ChangeBlockColour(Color.Red);
                }
                else
                {
                    walkingBack = false;
                }
            }

        }

        private void grvGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
