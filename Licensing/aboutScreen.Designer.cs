namespace ANDREICSLIB.Licensing
{
	partial class AboutScreen
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
			this.otherapptext = new System.Windows.Forms.RichTextBox();
			this.apptitlelabel = new System.Windows.Forms.Label();
			this.appversionlabel = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.closebutton = new System.Windows.Forms.Button();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// otherapptext
			// 
			this.otherapptext.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.otherapptext.BackColor = System.Drawing.Color.White;
			this.otherapptext.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.otherapptext.CausesValidation = false;
			this.otherapptext.Cursor = System.Windows.Forms.Cursors.Arrow;
			this.otherapptext.Font = new System.Drawing.Font("Times New Roman", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.otherapptext.Location = new System.Drawing.Point(25, 110);
			this.otherapptext.Name = "otherapptext";
			this.otherapptext.ReadOnly = true;
			this.otherapptext.Size = new System.Drawing.Size(279, 158);
			this.otherapptext.TabIndex = 2;
			this.otherapptext.Text = "ASDu jhASJHDu ASHd AUSdh AUSd hASUd ASD aSD8 aSd AS&d 8ASdasd asy7d as7d yas7d as" +
				"6d 7asd ";
			this.otherapptext.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.otherapptext_LinkClicked);
			// 
			// apptitlelabel
			// 
			this.apptitlelabel.AutoSize = true;
			this.apptitlelabel.Cursor = System.Windows.Forms.Cursors.Arrow;
			this.apptitlelabel.Font = new System.Drawing.Font("Garamond", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(1)), true);
			this.apptitlelabel.ForeColor = System.Drawing.Color.Navy;
			this.apptitlelabel.Location = new System.Drawing.Point(19, 50);
			this.apptitlelabel.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
			this.apptitlelabel.Name = "apptitlelabel";
			this.apptitlelabel.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.apptitlelabel.Size = new System.Drawing.Size(36, 36);
			this.apptitlelabel.TabIndex = 3;
			this.apptitlelabel.Text = "Z";
			// 
			// appversionlabel
			// 
			this.appversionlabel.AutoSize = true;
			this.appversionlabel.CausesValidation = false;
			this.appversionlabel.Cursor = System.Windows.Forms.Cursors.Arrow;
			this.appversionlabel.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(1)), true);
			this.appversionlabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
			this.appversionlabel.Location = new System.Drawing.Point(50, 87);
			this.appversionlabel.Name = "appversionlabel";
			this.appversionlabel.Size = new System.Drawing.Size(85, 19);
			this.appversionlabel.TabIndex = 4;
			this.appversionlabel.Text = "123123.123";
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.SystemColors.Control;
			this.panel1.Controls.Add(this.closebutton);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 274);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(327, 45);
			this.panel1.TabIndex = 5;
			// 
			// closebutton
			// 
			this.closebutton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.closebutton.Location = new System.Drawing.Point(229, 10);
			this.closebutton.Name = "closebutton";
			this.closebutton.Size = new System.Drawing.Size(75, 23);
			this.closebutton.TabIndex = 0;
			this.closebutton.Text = "OK";
			this.closebutton.UseVisualStyleBackColor = true;
			this.closebutton.Click += new System.EventHandler(this.closebutton_Click);
			// 
			// aboutScreen
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.BackColor = System.Drawing.Color.White;
			this.ClientSize = new System.Drawing.Size(327, 319);
			this.Controls.Add(this.otherapptext);
			this.Controls.Add(this.appversionlabel);
			this.Controls.Add(this.apptitlelabel);
			this.Controls.Add(this.panel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AboutScreen";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "aboutScreen";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.aboutScreen_FormClosing);
			this.Load += new System.EventHandler(this.aboutScreen_Load);
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

        #endregion

        /// <summary>
        /// The otherapptext
        /// </summary>
        public System.Windows.Forms.RichTextBox otherapptext;
        /// <summary>
        /// The apptitlelabel
        /// </summary>
        public System.Windows.Forms.Label apptitlelabel;
        /// <summary>
        /// The appversionlabel
        /// </summary>
        public System.Windows.Forms.Label appversionlabel;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button closebutton;

	}
}