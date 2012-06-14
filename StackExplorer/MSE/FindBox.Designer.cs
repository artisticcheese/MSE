namespace Microsoft.Mse.Gui
{
	partial class FindBox
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FindBox));
			this.label1 = new System.Windows.Forms.Label();
			this.findButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.searchBox = new System.Windows.Forms.TextBox();
			this.wholeWordCheckBox = new System.Windows.Forms.CheckBox();
			this.matchCaseCheckBox = new System.Windows.Forms.CheckBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.downDirectionRadio = new System.Windows.Forms.RadioButton();
			this.upDirectionRadio = new System.Windows.Forms.RadioButton();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// findButton
			// 
			resources.ApplyResources(this.findButton, "findButton");
			this.findButton.Name = "findButton";
			this.findButton.Click += new System.EventHandler(this.findButton_Click);
			// 
			// cancelButton
			// 
			resources.ApplyResources(this.cancelButton, "cancelButton");
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
			// 
			// searchBox
			// 
			resources.ApplyResources(this.searchBox, "searchBox");
			this.searchBox.Name = "searchBox";
			this.searchBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.searchBox_KeyUp);
			this.searchBox.TextChanged += new System.EventHandler(this.searchBox_TextChanged);
			// 
			// wholeWordCheckBox
			// 
			resources.ApplyResources(this.wholeWordCheckBox, "wholeWordCheckBox");
			this.wholeWordCheckBox.Name = "wholeWordCheckBox";
			this.wholeWordCheckBox.CheckedChanged += new System.EventHandler(this.wholeWordCheckBox_CheckedChanged);
			// 
			// matchCaseCheckBox
			// 
			resources.ApplyResources(this.matchCaseCheckBox, "matchCaseCheckBox");
			this.matchCaseCheckBox.Name = "matchCaseCheckBox";
			this.matchCaseCheckBox.CheckedChanged += new System.EventHandler(this.matchCaseCheckBox_CheckedChanged);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.downDirectionRadio);
			this.groupBox1.Controls.Add(this.upDirectionRadio);
			this.groupBox1.ForeColor = System.Drawing.SystemColors.InfoText;
			resources.ApplyResources(this.groupBox1, "groupBox1");
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.TabStop = false;
			// 
			// downDirectionRadio
			// 
			resources.ApplyResources(this.downDirectionRadio, "downDirectionRadio");
			this.downDirectionRadio.Checked = true;
			this.downDirectionRadio.Name = "downDirectionRadio";
			this.downDirectionRadio.CheckedChanged += new System.EventHandler(this.downDirectionRadio_CheckedChanged);
			// 
			// upDirectionRadio
			// 
			resources.ApplyResources(this.upDirectionRadio, "upDirectionRadio");
			this.upDirectionRadio.Name = "upDirectionRadio";
			this.upDirectionRadio.TabStop = false;
			this.upDirectionRadio.CheckedChanged += new System.EventHandler(this.upDirectionRadio_CheckedChanged);
			// 
			// FindBox
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.matchCaseCheckBox);
			this.Controls.Add(this.wholeWordCheckBox);
			this.Controls.Add(this.searchBox);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.findButton);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FindBox";
			this.Padding = new System.Windows.Forms.Padding(9);
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.TopMost = true;
			this.GotFocus += new System.EventHandler(this.FindBox_GotFocus);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}






		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button findButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.TextBox searchBox;
		private System.Windows.Forms.CheckBox wholeWordCheckBox;
		private System.Windows.Forms.CheckBox matchCaseCheckBox;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton upDirectionRadio;
		private System.Windows.Forms.RadioButton downDirectionRadio;
	}
}