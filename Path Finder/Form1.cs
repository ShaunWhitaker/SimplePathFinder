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

        public Form1()
        {
            InitializeComponent();
            BuildGrid();
            cellToMoveTo = new Cell(grid.grid);
            timer1.Enabled = false;
            lblInstructions.Text = "Select a starting point";
            grvGrid.DefaultCellStyle.Font = new Font("Arial", 15, FontStyle.Bold);
            grvGrid.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        protected void BuildGrid()
        {
            grid = new Grid();
            grid.NewGrid(15, 15);
            DataTable gridDT = grid.GridAsDataTable();
            grvGrid.DataSource = gridDT;
            grvGrid.AutoSize = true;
            grvGrid.BackgroundColor = System.Drawing.SystemColors.Control;
            grvGrid.BorderStyle = BorderStyle.None;
            grvGrid.AllowUserToResizeColumns = false;
            grvGrid.RowTemplate.Height = 45;
            grvGrid.RowTemplate.MinimumHeight = 45;
            grvGrid.Columns.Cast<DataGridViewColumn>().ToList().ForEach(x => x.Width = 45);
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
                    cell.Style.BackColor = Color.Black;
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
                }
            }

            if (selectedEndPoint && selectedEndPoint)
            {
                btnStart.Enabled = true;
            }

        }

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
            if (!walker.HasWalkablePath(grid) || FoundThing)
            {
                if (walker.MoveOrder.Any())
                {
                    ChangeBlockColour(Color.Green, walker.Coordinates.X, walker.Coordinates.Y);
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
            else
            {
                //TODO: Make a bit more efficient.
                var moved = false;
                while (!moved)
                {
                    char direction = rnd.Next(0, 2) == 1 ? 'X' : 'Y';
                    int move = (rnd.Next(0, 2) * 2 - 1);

                    if (direction == 'X')
                        cellToMoveTo = grid.GetCell(walker.Coordinates.X + move, walker.Coordinates.Y);

                    else
                        cellToMoveTo = grid.GetCell(walker.Coordinates.X, walker.Coordinates.Y + move);

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
            Application.DoEvents();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Move();
        }

        private void grvGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            Form1 NewForm = new Form1();
            NewForm.Show();
            this.Dispose(false);
        }

        private void grvGrid_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (HoldingMouseClick)
            {
                DataGridViewTextBoxCell cell = (DataGridViewTextBoxCell)grvGrid.Rows[e.RowIndex].Cells[e.ColumnIndex];
                cell.Style.BackColor = Color.Black;
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

            }
        }

        private void grvGrid_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            HoldingMouseClick = false;
        }

    }
}
