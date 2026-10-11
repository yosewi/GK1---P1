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
            panel.Margin = new Padding(3, 2, 3, 2);
            panel.Name = "panel";
            panel.Size = new Size(1052, 532);
            panel.TabIndex = 0;
            panel.Paint += panel_Paint;
            panel.MouseDown += panel_MouseDown;
            panel.MouseMove += panel_MouseMove;
            panel.MouseUp += panel_MouseUp;
            // 
            // panel1
            // 
            panel1.Controls.Add(groupBox1);
            panel1.Location = new Point(3, 3);
            panel1.Name = "panel1";
            panel1.Size = new Size(160, 84);
            panel1.TabIndex = 0;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(radioButton2);
            groupBox1.Controls.Add(radioButton1);
            groupBox1.Location = new Point(3, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(154, 77);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Drawing Type";
            // 
            // radioButton2
            // 
            radioButton2.AutoSize = true;
            radioButton2.Location = new Point(6, 47);
            radioButton2.Name = "radioButton2";
            radioButton2.Size = new Size(84, 19);
            radioButton2.TabIndex = 1;
            radioButton2.Text = "Bresenham";
            radioButton2.UseVisualStyleBackColor = true;
            radioButton2.CheckedChanged += radioButton2_CheckedChanged;
            // 
            // radioButton1
            // 
            radioButton1.AutoSize = true;
            radioButton1.Checked = true;
            radioButton1.Location = new Point(6, 22);
            radioButton1.Name = "radioButton1";
            radioButton1.Size = new Size(101, 19);
            radioButton1.TabIndex = 0;
            radioButton1.TabStop = true;
            radioButton1.Text = "Biblio drawing";
            radioButton1.UseVisualStyleBackColor = true;
            radioButton1.CheckedChanged += radioButton1_CheckedChanged;
            // 
            // VertexContextMenu
            // 
            VertexContextMenu.Items.AddRange(new ToolStripItem[] { deleteVertexToolStripMenuItem });
            VertexContextMenu.Name = "VertexContextMenu";
            VertexContextMenu.Size = new Size(143, 26);
            // 
            // deleteVertexToolStripMenuItem
            // 
            deleteVertexToolStripMenuItem.Name = "deleteVertexToolStripMenuItem";
            deleteVertexToolStripMenuItem.Size = new Size(142, 22);
            deleteVertexToolStripMenuItem.Text = "Delete vertex";
            deleteVertexToolStripMenuItem.Click += deleteVertexToolStripMenuItem_Click;
            // 
            // EdgeContextMenu
            // 
            EdgeContextMenu.Items.AddRange(new ToolStripItem[] { addVertexToolStripMenuItem });
            EdgeContextMenu.Name = "EdgeContextMenu";
            EdgeContextMenu.Size = new Size(132, 26);
            // 
            // addVertexToolStripMenuItem
            // 
            addVertexToolStripMenuItem.Name = "addVertexToolStripMenuItem";
            addVertexToolStripMenuItem.Size = new Size(131, 22);
            addVertexToolStripMenuItem.Text = "Add vertex";
            addVertexToolStripMenuItem.Click += addVertexToolStripMenuItem_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1052, 532);
            Controls.Add(panel);
            Margin = new Padding(3, 2, 3, 2);
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
    }
}
