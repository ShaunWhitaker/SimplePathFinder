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
        private bool selectedStartingPoint;
        private bool selectedEndPoint;
        private Grid grid;
        private Stack<Cell> pathWalked = new Stack<Cell>();
        private Walker walker;
        Cell cellToMoveTo;
        private bool HoldingMouseClick = false;
        Random rnd = new Random();
        private bool FoundThing = false;
        private DataGridViewTextBoxCell StartPoint;
        private DataGridViewTextBoxCell EndPoint;
        bool WalkerHasThingInSight = false;
        object CurrentSender;

        DataTable DT = new DataTable();


        public Form1()
        {
            InitializeComponent();
            BuildGrid();
        }

        private void grvGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            HoldingMouseClick = true;
            DataGridViewTextBoxCell cell = (DataGridViewTextBoxCell)grvGrid.Rows[e.RowIndex].Cells[e.ColumnIndex];

            //Clear the cell if it's a right click
            if (e.Button == MouseButtons.Right)
            {
                var clearCell = new Cell(grid.grid)
                {
                    Coordinates = new Point
                    {
                        X = e.ColumnIndex,
                        Y = e.RowIndex
                    }
                };

                grid.SetCell(clearCell);
                cell.Style.BackColor = Color.White;
                cell.Value = "";

                //if the start/end point is cleared, it will be selected on next mouse down.
                if (cell == StartPoint)
                {
                    selectedStartingPoint = false;
                    btnStart.Enabled = false;
                }
                else if (cell == EndPoint)
                {
                    selectedEndPoint = false;
                    btnStart.Enabled = false;
                }
            }
            else
            {
                //If starting point has not been selected it should create it first.
                if (!selectedStartingPoint)
                {
                    cell.Style.BackColor = Color.Red;
                    selectedStartingPoint = true;
                    cell.Value = "S";

                    walker = new Walker(grid.grid, e.ColumnIndex, e.RowIndex);

                    lblInstructions.Text = "Select an end point.";

                    StartPoint = cell;

                }
                else if (!selectedEndPoint)
                {
                    cell.Style.BackColor = Color.Blue;
                    selectedEndPoint = true;
                    grid[e.ColumnIndex, e.RowIndex].IsEndPoint = true;
                    lblInstructions.Text = "build a maze and select start when ready.";
                    EndPoint = cell;
                }
                else
                {
                    //Add a wall
                    cell.Style.BackColor = Color.Black;

                    var gridCell = grid.GetCell(e.ColumnIndex, e.RowIndex);

                    gridCell.Coordinates = new Point
                    {
                        X = e.ColumnIndex,
                        Y = e.RowIndex
                    };

                    gridCell.isObstical = true;
                }
            }

            //only enable the start button once both the start point and the end point have been selected.
            if (selectedEndPoint && selectedEndPoint)
            {
                btnStart.Enabled = true;
            }

        }

        /// <summary>
        ///Changes the block color when walker is progressing through the maze. 
        ///Walked paths will change green.
        ///Path where walker is on will change red.
        ///If walker has found the end point it will be a combination of red + blue.
        /// </summary>
        /// <param name="colour"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        protected void ChangeBlockColour(Color colour, int x, int y)
        {
            DataGridViewTextBoxCell cell = (DataGridViewTextBoxCell)grvGrid.Rows[y].Cells[x];
            if (colour == Color.Green)
            {
                if (FoundThing)
                    cell.Value = "";
            }
            else if (colour == Color.Red && FoundThing)
            {
                cell.Value = "█";
                cell.Style.ForeColor = Color.Blue;
            }
            cell.Style.BackColor = colour;
        }
        private void grvGrid_SelectionChanged(object sender, EventArgs e)
        {
            grvGrid.ClearSelection();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;
        }


        private void Move()
        {
            //if the walker is back tracking.
            if (!walker.HasWalkablePath(grid) || FoundThing)
            {
                if (walker.MoveOrder.Any())
                {
                    //change the block color when walked on.
                    ChangeBlockColour(Color.Green, walker.Coordinates.X, walker.Coordinates.Y);
                    //Get the previous block that the walker is on to determine where to walk next.
                    var previousPoint = walker.MoveOrder.Pop();
                    cellToMoveTo = grid.GetCell(previousPoint.X, previousPoint.Y);
                    walker.Walk(grid, cellToMoveTo, true, FoundThing);
                    ChangeBlockColour(Color.Red, walker.Coordinates.X, walker.Coordinates.Y);
                }
                else
                {
                    timer1.Enabled = false;
                    if (FoundThing)
                    {
                        MessageBox.Show("Here you go!");
                    }
                    else
                        MessageBox.Show("I could not find it!");
                }
            }
            //if the walker is moving forward
            else
            {
                //TODO: Make a bit more efficient.
                var moved = false;
                while (!moved)
                {
                    //Get a random direction (up/down, left/righ)
                    char direction = rnd.Next(0, 2) == 1 ? 'X' : 'Y';

                    //Get either a 1 or a - 1 to indicate which direction on the axis to move.
                    int move = (rnd.Next(0, 2) * 2 - 1);

                    if (direction == 'X')
                        cellToMoveTo = grid.GetCell(walker.Coordinates.X + move, walker.Coordinates.Y);

                    else
                        cellToMoveTo = grid.GetCell(walker.Coordinates.X, walker.Coordinates.Y + move);

                    //check if the cell is a wall or has been stepped on before. if either is true, try get the next walkable cell.
                    if (!cellToMoveTo.isObstical && !cellToMoveTo.hasSteppedOn)
                    {
                        ChangeBlockColour(Color.Green, walker.Coordinates.X, walker.Coordinates.Y);
                        walker.Walk(grid, cellToMoveTo);
                        pathWalked.Push(grid[walker.Coordinates.X, walker.Coordinates.Y]);
                        if (cellToMoveTo.IsEndPoint)
                        {
                            var walkerCell = grvGrid[walker.Coordinates.X, walker.Coordinates.Y];
                            FoundThing = true;
                        }
                        ChangeBlockColour(Color.Red, walker.Coordinates.X, walker.Coordinates.Y);

                        moved = true;
                    }
                }
            }

            //Coding the line of site. Unfinished.
            //This will enable the walker to walk straight to the end thing instead of going random directions.

            if ((walker.Coordinates.X == EndPoint.ColumnIndex || walker.Coordinates.Y == EndPoint.RowIndex) && !FoundThing)
            {
                bool isBlockedByObstical = false;
                if (walker.Coordinates.X == EndPoint.ColumnIndex)
                {
                    int loopStartPoint = 0;
                    int loopEndPoint = 0;

                    if (walker.Coordinates.Y < EndPoint.RowIndex)
                    {
                        loopStartPoint = walker.Coordinates.Y;
                        loopEndPoint = EndPoint.RowIndex;

                        for (int i = loopStartPoint; i < loopEndPoint; i ++)
                        {
                            var cell = grid.GetCell(walker.Coordinates.X, i);
                            if (cell.isObstical)
                            {
                                isBlockedByObstical = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        loopStartPoint = EndPoint.RowIndex;
                        loopEndPoint = walker.Coordinates.Y;

                        for (int i = loopStartPoint; i > loopEndPoint; i--)
                        {
                            var cell = grid.GetCell(walker.Coordinates.X, i);
                            if (cell.isObstical)
                            {
                                isBlockedByObstical = true;
                                break;
                            }
                        }
                    }
                }

                if (!isBlockedByObstical)
                {
                    WalkerHasThingInSight = true;
                    MessageBox.Show("I see it!");
                }
            }
            Application.DoEvents();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Move();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            Form1 NewForm = new Form1();
            NewForm.Show();
            this.Dispose(false);
        }

        private void grvGrid_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            //Check if the mouse is being held down so we can build a wall without clicking.
            if (CurrentSender == null)
                CurrentSender = e;
            if (CurrentSender != e)
            {
                CurrentSender = e;
                while (HoldingMouseClick)
                {
                    var cell = new DataGridViewTextBoxCell();
                    cell.Style.BackColor = Color.Black;


                    grvGrid.Rows[e.RowIndex].Cells[e.ColumnIndex] = cell;

                    var obstical = new Cell(grid.grid)
                    {
                        Coordinates = new Point
                        {
                            X = e.ColumnIndex,
                            Y = e.RowIndex
                        },
                        isObstical = true
                    };

                    grid.SetCell(obstical);
                    break;
                }
            }
        }

        private void grvGrid_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            HoldingMouseClick = false;
        }

        protected void BuildGrid()
        {
            lblInstructions.Text = "Select a starting point";
            grid = new Grid();
            grid.NewGrid(15, 15);
            DT = grid.GridAsDataTable();
            grvGrid.DataSource = DT;
            grvGrid.AutoSize = true;
            grvGrid.BackgroundColor = System.Drawing.SystemColors.Control;
            grvGrid.BorderStyle = BorderStyle.None;
            grvGrid.AllowUserToResizeColumns = false;
            grvGrid.RowTemplate.Height = 45;
            grvGrid.RowTemplate.MinimumHeight = 45;
            grvGrid.Columns.Cast<DataGridViewColumn>().ToList().ForEach(x => x.Width = 45);
            grvGrid.DefaultCellStyle.Font = new Font("Arial", 15, FontStyle.Bold);
            grvGrid.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            cellToMoveTo = new Cell(grid.grid);
            timer1.Enabled = false;
        }

    }
}
