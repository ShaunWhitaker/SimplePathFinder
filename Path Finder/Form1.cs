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
        public Grid grid;
        public Stack<Cell> pathWalked = new Stack<Cell>();
        private Walker walker;
        public Cell cellToMoveTo;
        private bool HoldingMouseClick = false;
        Random rnd = new Random();
        private bool FoundThing = false;
        private DataGridViewTextBoxCell StartPoint;
        private DataGridViewTextBoxCell EndPoint;
        private bool WalkerHasThingInSight = false;
        object CurrentSender;
        private Queue<Cell> LineOfSitePath = new Queue<Cell>();

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

        private void CheckIfPathIsBlockedByObstical(char axis)
        {
            //This is not very effecient.
            //first check which direction is the fastest path to the endpoint 

            Queue<Cell> path1 = new Queue<Cell>();
            Queue<Cell> path2 = new Queue<Cell>();

            var tempWalkerCell = grid.GetCell(walker.Coordinates.X, walker.Coordinates.Y);

            //check down & right
            while (!tempWalkerCell.IsEndPoint)
            {
                if (tempWalkerCell.isObstical)
                {
                    path1 = null;
                    break;
                }
                path1.Enqueue(tempWalkerCell);
                if (axis == 'Y')
                {
                    tempWalkerCell = grid.GetCell(walker.Coordinates.X, tempWalkerCell.Coordinates.Y + 1);
                }
                else
                {
                    tempWalkerCell = grid.GetCell(tempWalkerCell.Coordinates.X + 1, tempWalkerCell.Coordinates.Y);
                }

            }

            if (tempWalkerCell.IsEndPoint)
            {
                path1.Enqueue(tempWalkerCell);
            }

            //Check up & left
            tempWalkerCell = grid.GetCell(walker.Coordinates.X, walker.Coordinates.Y);
            while (!tempWalkerCell.IsEndPoint)
            {
                if (tempWalkerCell.isObstical)
                {
                    path2 = null;
                    break;
                }
                path2.Enqueue(tempWalkerCell);
                if (axis == 'Y')
                {
                    tempWalkerCell = grid.GetCell(walker.Coordinates.X, tempWalkerCell.Coordinates.Y - 1);
                }
                else
                {
                    tempWalkerCell = grid.GetCell(tempWalkerCell.Coordinates.X - 1, walker.Coordinates.Y);
                }
            }

            if (tempWalkerCell.IsEndPoint)
            {
                path2.Enqueue(tempWalkerCell);
            }

            //If neither of the paths are null then there are 2 possible LOS paths to the end point. Get the shortest one.
            if (path1 != null && path2 != null)
            {
                LineOfSitePath = path1.Count < path2.Count ? path1 : path2;
            }
            else if (path2 != null)
            {
                LineOfSitePath = path2;
            }
            else if (path1 != null)
            {
                LineOfSitePath = path1;
            }

        }


        private void Move()
        {
            //Check if the walker is on the same axis as the end point.
            //if it is on the same axis check if it is not blocked by an obstical (it can see it) and if it isn't, run straight to it.
            if ((walker.Coordinates.X == EndPoint.ColumnIndex || walker.Coordinates.Y == EndPoint.RowIndex) && !FoundThing && !LineOfSitePath.Any())
            {
                if (walker.Coordinates.X == EndPoint.ColumnIndex)
                {
                    CheckIfPathIsBlockedByObstical('Y');

                }
                else if (walker.Coordinates.Y == EndPoint.RowIndex)
                {
                    CheckIfPathIsBlockedByObstical('X');
                }
            }

            //if there is a line of site path straight to the end point we use that.
            if (LineOfSitePath.Any() && !FoundThing)
            {
                ChangeBlockColour(Color.Green, walker.Coordinates.X, walker.Coordinates.Y);
                cellToMoveTo = LineOfSitePath.Dequeue();
                walker.Walk(this);
                if (cellToMoveTo.IsEndPoint)
                {
                    FoundThing = true;
                }
                ChangeBlockColour(Color.Red, walker.Coordinates.X, walker.Coordinates.Y);
            }

            //if the walker is back tracking.
            else if (!walker.HasWalkablePath(grid) || FoundThing)
            {
                if (walker.MoveOrder.Any())
                {
                    //change the block color when walked on.
                    ChangeBlockColour(Color.Green, walker.Coordinates.X, walker.Coordinates.Y);
                    //Get the previous block that the walker is on to determine where to walk next.
                    var previousPoint = walker.MoveOrder.Pop();
                    cellToMoveTo = grid.GetCell(previousPoint.X, previousPoint.Y);
                    walker.Walk(this, true, FoundThing);
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
                        walker.Walk(this);
                        if (cellToMoveTo.IsEndPoint)
                        {
                            FoundThing = true;
                        }                        ChangeBlockColour(Color.Red, walker.Coordinates.X, walker.Coordinates.Y);
                        ChangeBlockColour(Color.Red, walker.Coordinates.X, walker.Coordinates.Y);

                        moved = true;
                    }
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
