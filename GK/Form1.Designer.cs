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
            VertexContextMenu = new ContextMenuStrip(components);
            deleteVertexToolStripMenuItem = new ToolStripMenuItem();
            EdgeContextMenu = new ContextMenuStrip(components);
            addVertexToolStripMenuItem = new ToolStripMenuItem();
            VertexContextMenu.SuspendLayout();
            EdgeContextMenu.SuspendLayout();
            SuspendLayout();
            // 
            // panel
            // 
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
            EdgeContextMenu.Size = new Size(181, 48);
            // 
            // addVertexToolStripMenuItem
            // 
            addVertexToolStripMenuItem.Name = "addVertexToolStripMenuItem";
            addVertexToolStripMenuItem.Size = new Size(180, 22);
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
    }
}
