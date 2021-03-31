using System.Drawing;

namespace Path_Finder
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.grvGrid = new System.Windows.Forms.DataGridView();
            this.btnStart = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.lblInstructions = new System.Windows.Forms.Label();
            this.btnReset = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.grvGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // grvGrid
            // 
            this.grvGrid.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.grvGrid.ColumnHeadersHeight = 10;
            this.grvGrid.ColumnHeadersVisible = false;
            this.grvGrid.GridColor = System.Drawing.SystemColors.ControlDarkDark;
            this.grvGrid.Location = new System.Drawing.Point(13, 39);
            this.grvGrid.Margin = new System.Windows.Forms.Padding(0);
            this.grvGrid.Name = "grvGrid";
            this.grvGrid.ReadOnly = true;
            this.grvGrid.RowHeadersVisible = false;
            this.grvGrid.RowHeadersWidth = 10;
            this.grvGrid.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.grvGrid.Size = new System.Drawing.Size(54, 44);
            this.grvGrid.TabIndex = 0;
            this.grvGrid.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.grvGrid_CellContentClick);
            this.grvGrid.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.grvGrid_CellContentClick);
            this.grvGrid.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.grvGrid_CellMouseDown);
            this.grvGrid.CellMouseEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.grvGrid_CellMouseEnter);
            this.grvGrid.CellMouseUp += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.grvGrid_CellMouseUp);
            this.grvGrid.SelectionChanged += new System.EventHandler(this.grvGrid_SelectionChanged);
            // 
            // btnStart
            // 
            this.btnStart.Enabled = false;
            this.btnStart.Location = new System.Drawing.Point(13, 13);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 1;
            this.btnStart.Text = " Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 400;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // lblInstructions
            // 
            this.lblInstructions.AutoSize = true;
            this.lblInstructions.Location = new System.Drawing.Point(150, 16);
            this.lblInstructions.Name = "lblInstructions";
            this.lblInstructions.Size = new System.Drawing.Size(0, 13);
            this.lblInstructions.TabIndex = 2;
            this.lblInstructions.Font = new Font("Arial", 12, FontStyle.Bold);
            // 
            // btnReset
            // 
            this.btnReset.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnReset.Location = new System.Drawing.Point(581, 13);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 23);
            this.btnReset.TabIndex = 3;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(668, 424);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.lblInstructions);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.grvGrid);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.grvGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView grvGrid;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label lblInstructions;
        private System.Windows.Forms.Button btnReset;
    }
}

