namespace GK
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            panel = new BufferingPanel();
            panel1 = new Panel();
            groupBox1 = new GroupBox();
            radioButton2 = new RadioButton();
            radioButton1 = new RadioButton();
            VertexContextMenu = new ContextMenuStrip(components);
            deleteVertexToolStripMenuItem = new ToolStripMenuItem();
            EdgeContextMenu = new ContextMenuStrip(components);
            addVertexToolStripMenuItem = new ToolStripMenuItem();
            bezierOnoffToolStripMenuItem = new ToolStripMenuItem();
            setLengthToolStripMenuItem = new ToolStripMenuItem();
            removeConstraintsToolStripMenuItem = new ToolStripMenuItem();
            panel.SuspendLayout();
            panel1.SuspendLayout();
            groupBox1.SuspendLayout();
            VertexContextMenu.SuspendLayout();
            EdgeContextMenu.SuspendLayout();
            SuspendLayout();
            // 
            // panel
            // 
            panel.Controls.Add(panel1);
            panel.Dock = DockStyle.Fill;
            panel.Location = new Point(0, 0);
            panel.Name = "panel";
            panel.Size = new Size(1202, 709);
            panel.TabIndex = 0;
            panel.Paint += panel_Paint;
            panel.MouseDown += panel_MouseDown;
            panel.MouseMove += panel_MouseMove;
            panel.MouseUp += panel_MouseUp;
            // 
            // panel1
            // 
            panel1.Controls.Add(groupBox1);
            panel1.Location = new Point(3, 4);
            panel1.Margin = new Padding(3, 4, 3, 4);
            panel1.Name = "panel1";
            panel1.Size = new Size(183, 112);
            panel1.TabIndex = 0;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(radioButton2);
            groupBox1.Controls.Add(radioButton1);
            groupBox1.Location = new Point(3, 4);
            groupBox1.Margin = new Padding(3, 4, 3, 4);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(3, 4, 3, 4);
            groupBox1.Size = new Size(176, 103);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Drawing Type";
            // 
            // radioButton2
            // 
            radioButton2.AutoSize = true;
            radioButton2.Location = new Point(7, 63);
            radioButton2.Margin = new Padding(3, 4, 3, 4);
            radioButton2.Name = "radioButton2";
            radioButton2.Size = new Size(103, 24);
            radioButton2.TabIndex = 1;
            radioButton2.Text = "Bresenham";
            radioButton2.UseVisualStyleBackColor = true;
            radioButton2.CheckedChanged += radioButton2_CheckedChanged;
            // 
            // radioButton1
            // 
            radioButton1.AutoSize = true;
            radioButton1.Checked = true;
            radioButton1.Location = new Point(7, 29);
            radioButton1.Margin = new Padding(3, 4, 3, 4);
            radioButton1.Name = "radioButton1";
            radioButton1.Size = new Size(127, 24);
            radioButton1.TabIndex = 0;
            radioButton1.TabStop = true;
            radioButton1.Text = "Biblio drawing";
            radioButton1.UseVisualStyleBackColor = true;
            radioButton1.CheckedChanged += radioButton1_CheckedChanged;
            // 
            // VertexContextMenu
            // 
            VertexContextMenu.ImageScalingSize = new Size(20, 20);
            VertexContextMenu.Items.AddRange(new ToolStripItem[] { deleteVertexToolStripMenuItem });
            VertexContextMenu.Name = "VertexContextMenu";
            VertexContextMenu.Size = new Size(167, 28);
            // 
            // deleteVertexToolStripMenuItem
            // 
            deleteVertexToolStripMenuItem.Name = "deleteVertexToolStripMenuItem";
            deleteVertexToolStripMenuItem.Size = new Size(166, 24);
            deleteVertexToolStripMenuItem.Text = "Delete vertex";
            deleteVertexToolStripMenuItem.Click += deleteVertexToolStripMenuItem_Click;
            // 
            // EdgeContextMenu
            // 
            EdgeContextMenu.ImageScalingSize = new Size(20, 20);
            EdgeContextMenu.Items.AddRange(new ToolStripItem[] { addVertexToolStripMenuItem, bezierOnoffToolStripMenuItem, setLengthToolStripMenuItem, removeConstraintsToolStripMenuItem });
            EdgeContextMenu.Name = "EdgeContextMenu";
            EdgeContextMenu.Size = new Size(211, 128);
            // 
            // addVertexToolStripMenuItem
            // 
            addVertexToolStripMenuItem.Name = "addVertexToolStripMenuItem";
            addVertexToolStripMenuItem.Size = new Size(210, 24);
            addVertexToolStripMenuItem.Text = "Add vertex";
            addVertexToolStripMenuItem.Click += addVertexToolStripMenuItem_Click;
            // 
            // bezierOnoffToolStripMenuItem
            // 
            bezierOnoffToolStripMenuItem.Name = "bezierOnoffToolStripMenuItem";
            bezierOnoffToolStripMenuItem.Size = new Size(210, 24);
            bezierOnoffToolStripMenuItem.Text = "Bezier on/off";
            bezierOnoffToolStripMenuItem.Click += bezierOnoffToolStripMenuItem_Click;
            // 
            // setLengthToolStripMenuItem
            // 
            setLengthToolStripMenuItem.Name = "setLengthToolStripMenuItem";
            setLengthToolStripMenuItem.Size = new Size(210, 24);
            setLengthToolStripMenuItem.Text = "Set length";
            setLengthToolStripMenuItem.Click += setLengthToolStripMenuItem_Click;
            // 
            // removeConstraintsToolStripMenuItem
            // 
            removeConstraintsToolStripMenuItem.Name = "removeConstraintsToolStripMenuItem";
            removeConstraintsToolStripMenuItem.Size = new Size(210, 24);
            removeConstraintsToolStripMenuItem.Text = "Remove constraints";
            removeConstraintsToolStripMenuItem.Click += removeConstraintsToolStripMenuItem_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1202, 709);
            Controls.Add(panel);
            Name = "Form1";
            Text = "Form1";
            panel.ResumeLayout(false);
            panel1.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            VertexContextMenu.ResumeLayout(false);
            EdgeContextMenu.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private BufferingPanel panel;
        private ContextMenuStrip VertexContextMenu;
        private ToolStripMenuItem deleteVertexToolStripMenuItem;
        private ContextMenuStrip EdgeContextMenu;
        private ToolStripMenuItem addVertexToolStripMenuItem;
        private Panel panel1;
        private GroupBox groupBox1;
        private RadioButton radioButton2;
        private RadioButton radioButton1;
        private ToolStripMenuItem bezierOnoffToolStripMenuItem;
        private ToolStripMenuItem setLengthToolStripMenuItem;
        private ToolStripMenuItem removeConstraintsToolStripMenuItem;
    }
}
