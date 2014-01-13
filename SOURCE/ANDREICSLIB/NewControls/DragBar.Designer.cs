namespace ANDREICSLIB.NewControls
{
	partial class DragBar
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.drawpanel = new System.Windows.Forms.Panel();
			this.listpanel = new System.Windows.Forms.Panel();
			this.SuspendLayout();
			// 
			// drawpanel
			// 
			this.drawpanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
			this.drawpanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.drawpanel.Location = new System.Drawing.Point(0, 0);
			this.drawpanel.Name = "drawpanel";
			this.drawpanel.Size = new System.Drawing.Size(434, 74);
			this.drawpanel.TabIndex = 0;
			this.drawpanel.Paint += new System.Windows.Forms.PaintEventHandler(this.drawpanel_Paint);
			this.drawpanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.drawpanel_MouseDown);
			this.drawpanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.drawpanel_MouseMove);
			this.drawpanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.drawpanel_MouseUp);
			// 
			// listpanel
			// 
			this.listpanel.BackColor = System.Drawing.SystemColors.ButtonShadow;
			this.listpanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.listpanel.Location = new System.Drawing.Point(0, 74);
			this.listpanel.Name = "listpanel";
			this.listpanel.Size = new System.Drawing.Size(434, 28);
			this.listpanel.TabIndex = 1;
			this.listpanel.Paint += new System.Windows.Forms.PaintEventHandler(this.listpanel_Paint);
			// 
			// DragBar
			// 
			this.Controls.Add(this.drawpanel);
			this.Controls.Add(this.listpanel);
			this.Name = "DragBar";
			this.Size = new System.Drawing.Size(434, 102);
			this.Load += new System.EventHandler(this.DragBar_Load);
			this.Resize += new System.EventHandler(this.DragBar_Resize);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel drawpanel;
		private System.Windows.Forms.Panel listpanel;
	}
}
