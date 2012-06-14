namespace Microsoft.Mse.Gui
{
	partial class PreferenceForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PreferenceForm));
			this.cancelButton = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.stackLogLabel = new System.Windows.Forms.Label();
			this.processRefLabel = new System.Windows.Forms.Label();
			this.stackRefLabel = new System.Windows.Forms.Label();
			this.threadRefLabel = new System.Windows.Forms.Label();
			this.procRefresh = new System.Windows.Forms.TextBox();
			this.threadRefresh = new System.Windows.Forms.TextBox();
			this.stackRefresh = new System.Windows.Forms.TextBox();
			this.logRefresh = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this.stackDepthLabel = new System.Windows.Forms.Label();
			this.depthBox = new System.Windows.Forms.TextBox();
			this.acceptButton = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// cancelButton
			// 
			resources.ApplyResources(this.cancelButton, "cancelButton");
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.MinimumSize = new System.Drawing.Size(80, 25);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.tableLayoutPanel1);
			this.groupBox1.ForeColor = System.Drawing.SystemColors.InfoText;
			resources.ApplyResources(this.groupBox1, "groupBox1");
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.TabStop = false;
			// 
			// tableLayoutPanel1
			// 
			resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.Controls.Add(this.stackLogLabel, 0, 4);
			this.tableLayoutPanel1.Controls.Add(this.processRefLabel, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.stackRefLabel, 0, 3);
			this.tableLayoutPanel1.Controls.Add(this.threadRefLabel, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this.procRefresh, 1, 1);
			this.tableLayoutPanel1.Controls.Add(this.threadRefresh, 1, 2);
			this.tableLayoutPanel1.Controls.Add(this.stackRefresh, 1, 3);
			this.tableLayoutPanel1.Controls.Add(this.logRefresh, 1, 4);
			this.tableLayoutPanel1.Controls.Add(this.label5, 1, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
			// 
			// stackLogLabel
			// 
			resources.ApplyResources(this.stackLogLabel, "stackLogLabel");
			this.stackLogLabel.Name = "stackLogLabel";
			// 
			// processRefLabel
			// 
			resources.ApplyResources(this.processRefLabel, "processRefLabel");
			this.processRefLabel.Name = "processRefLabel";
			// 
			// stackRefLabel
			// 
			resources.ApplyResources(this.stackRefLabel, "stackRefLabel");
			this.stackRefLabel.Name = "stackRefLabel";
			// 
			// threadRefLabel
			// 
			resources.ApplyResources(this.threadRefLabel, "threadRefLabel");
			this.threadRefLabel.Name = "threadRefLabel";
			// 
			// procRefresh
			// 
			resources.ApplyResources(this.procRefresh, "procRefresh");
			this.procRefresh.Name = "procRefresh";
			// 
			// threadRefresh
			// 
			resources.ApplyResources(this.threadRefresh, "threadRefresh");
			this.threadRefresh.Name = "threadRefresh";
			// 
			// stackRefresh
			// 
			resources.ApplyResources(this.stackRefresh, "stackRefresh");
			this.stackRefresh.Name = "stackRefresh";
			// 
			// logRefresh
			// 
			resources.ApplyResources(this.logRefresh, "logRefresh");
			this.logRefresh.Name = "logRefresh";
			// 
			// label5
			// 
			resources.ApplyResources(this.label5, "label5");
			this.label5.Name = "label5";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.tableLayoutPanel2);
			this.groupBox2.ForeColor = System.Drawing.SystemColors.InfoText;
			resources.ApplyResources(this.groupBox2, "groupBox2");
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.TabStop = false;
			// 
			// tableLayoutPanel2
			// 
			resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 77.35849F));
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 22.64151F));
			this.tableLayoutPanel2.Controls.Add(this.stackDepthLabel, 0, 0);
			this.tableLayoutPanel2.Controls.Add(this.depthBox, 1, 0);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			// 
			// stackDepthLabel
			// 
			resources.ApplyResources(this.stackDepthLabel, "stackDepthLabel");
			this.stackDepthLabel.Name = "stackDepthLabel";
			// 
			// depthBox
			// 
			resources.ApplyResources(this.depthBox, "depthBox");
			this.depthBox.Name = "depthBox";
			// 
			// acceptButton
			// 
			resources.ApplyResources(this.acceptButton, "acceptButton");
			this.acceptButton.MinimumSize = new System.Drawing.Size(80, 25);
			this.acceptButton.Name = "acceptButton";
			this.acceptButton.Click += new System.EventHandler(this.acceptButton_Click);
			// 
			// PreferenceForm
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.acceptButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PreferenceForm";
			this.Padding = new System.Windows.Forms.Padding(9);
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Shown += new System.EventHandler(this.PreferenceForm_Shown);
			this.groupBox1.ResumeLayout(false);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.tableLayoutPanel2.ResumeLayout(false);
			this.tableLayoutPanel2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}





		#endregion

		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Label stackLogLabel;
		private System.Windows.Forms.Label processRefLabel;
		private System.Windows.Forms.Label stackRefLabel;
		private System.Windows.Forms.Label threadRefLabel;
		private System.Windows.Forms.TextBox procRefresh;
		private System.Windows.Forms.TextBox threadRefresh;
		private System.Windows.Forms.TextBox stackRefresh;
		private System.Windows.Forms.TextBox logRefresh;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
		private System.Windows.Forms.Label stackDepthLabel;
		private System.Windows.Forms.TextBox depthBox;
		private System.Windows.Forms.Button acceptButton;
	}
}