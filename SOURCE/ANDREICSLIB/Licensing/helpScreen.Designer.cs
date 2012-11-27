namespace ANDREICSLIB
{
	partial class helpScreen
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
			this.panel1 = new System.Windows.Forms.Panel();
			this.closebutton = new System.Windows.Forms.Button();
			this.helpbox = new System.Windows.Forms.RichTextBox();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.SystemColors.Control;
			this.panel1.Controls.Add(this.closebutton);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 511);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(456, 45);
			this.panel1.TabIndex = 6;
			// 
			// closebutton
			// 
			this.closebutton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.closebutton.Location = new System.Drawing.Point(343, 10);
			this.closebutton.Name = "closebutton";
			this.closebutton.Size = new System.Drawing.Size(101, 23);
			this.closebutton.TabIndex = 0;
			this.closebutton.Text = "OK";
			this.closebutton.UseVisualStyleBackColor = true;
			this.closebutton.Click += new System.EventHandler(this.closebutton_Click);
			// 
			// helpbox
			// 
			this.helpbox.BackColor = System.Drawing.Color.White;
			this.helpbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.helpbox.Cursor = System.Windows.Forms.Cursors.Arrow;
			this.helpbox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.helpbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.helpbox.Location = new System.Drawing.Point(0, 0);
			this.helpbox.Name = "helpbox";
			this.helpbox.ReadOnly = true;
			this.helpbox.Size = new System.Drawing.Size(456, 511);
			this.helpbox.TabIndex = 7;
			this.helpbox.Text = "";
			// 
			// helpScreen
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(456, 556);
			this.Controls.Add(this.helpbox);
			this.Controls.Add(this.panel1);
			this.Name = "helpScreen";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "helpScreen";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.helpScreen_FormClosing);
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button closebutton;
		public System.Windows.Forms.RichTextBox helpbox;
	}
}