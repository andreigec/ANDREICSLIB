namespace ANDREICSLIB
{
	partial class multipleMessageBox
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
			this.textarea = new System.Windows.Forms.TextBox();
            this.buttonpanel = new PanelReplacement();
			this.SuspendLayout();
			// 
			// textarea
			// 
			this.textarea.BackColor = System.Drawing.Color.White;
			this.textarea.Dock = System.Windows.Forms.DockStyle.Top;
			this.textarea.Location = new System.Drawing.Point(0, 0);
			this.textarea.Multiline = true;
			this.textarea.Name = "textarea";
			this.textarea.ReadOnly = true;
			this.textarea.Size = new System.Drawing.Size(306, 93);
			this.textarea.TabIndex = 1;
			// 
			// buttonpanel
			// 
			this.buttonpanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.buttonpanel.Location = new System.Drawing.Point(0, 93);
			this.buttonpanel.Name = "buttonpanel";
			this.buttonpanel.Size = new System.Drawing.Size(306, 36);
			this.buttonpanel.TabIndex = 0;
			// 
			// multipleMessageBox
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(306, 129);
			this.Controls.Add(this.buttonpanel);
			this.Controls.Add(this.textarea);
			this.Name = "multipleMessageBox";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "multipleMessageBox";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

        private PanelReplacement buttonpanel;
		private System.Windows.Forms.TextBox textarea;
	}
}