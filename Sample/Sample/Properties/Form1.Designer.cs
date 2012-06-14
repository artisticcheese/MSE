namespace Microsoft.Mse.Sample
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
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.threadCount = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// timer1
			// 
			this.timer1.Enabled = true;
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(90, 109);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(103, 30);
			this.button1.TabIndex = 0;
			this.button1.Text = "Add A Thread";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(90, 154);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(103, 30);
			this.button2.TabIndex = 1;
			this.button2.Text = "Remove a Thread";
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(414, 20);
			this.label1.TabIndex = 2;
			this.label1.Text = "Sample program used to test the Managed Stack Explorer";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(214, 118);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(101, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "Created Threads:";
			// 
			// threadCount
			// 
			this.threadCount.AutoSize = true;
			this.threadCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.threadCount.ForeColor = System.Drawing.Color.Red;
			this.threadCount.Location = new System.Drawing.Point(245, 131);
			this.threadCount.Name = "threadCount";
			this.threadCount.Size = new System.Drawing.Size(19, 29);
			this.threadCount.TabIndex = 4;
			this.threadCount.Text = "0";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(35, 40);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(359, 32);
			this.label4.TabIndex = 5;
			this.label4.Text = "This program will allow you to make and remove threads which you can monitor usin" +
				"g the Managed Stack Explorer";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.LightSteelBlue;
			this.ClientSize = new System.Drawing.Size(436, 211);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.threadCount);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.Name = "Form1";
			this.Text = "Sample Program for Managed Stack Explorer";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.Shown += new System.EventHandler(this.Form1_Shown);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
			this.ResumeLayout(false);
			this.PerformLayout();

		}


		#endregion

		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label threadCount;
		private System.Windows.Forms.Label label4;
	}
}

