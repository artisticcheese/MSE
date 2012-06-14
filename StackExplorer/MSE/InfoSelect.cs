//---------------------------------------------------------------------
//  This file is part of the Managed Stack Explorer (MSE).
// 
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Globalization;

namespace Microsoft.Mse.Gui
{

	/// <summary>
	/// The dialog box to let you customize the general info data
	/// </summary>
	public partial class InfoSelect : Form
	{

		private List<string> choices; //holds which information had been chose to be displayed
		private List<CheckBox> checkGroup; //hold the group of checkboxes to iterate over


		/// <summary>
		/// Constructor for infoSelect form which lets the user choose which information about the process will be displayed
		/// </summary>
		/// <param name="selected"></param>
		/// <param name="allPossibleData"></param>
		public InfoSelect(IList<string> selected, IList<string> allPossibleData)
		{
			InitializeComponent();
			choices = new List<string>();


			checkGroup = new List<CheckBox>();
			checkGroup.AddRange(new CheckBox[] { checkBox1, checkBox2, checkBox3, checkBox4, checkBox5,
				checkBox6, checkBox7, checkBox8, checkBox9, checkBox10, checkBox11, checkBox13, checkBox14});

			int i = 0;
			//populate checkboxes with the avilable options
			foreach (CheckBox box in checkGroup)
			{
				if (i < allPossibleData.Count)
				{
					box.Text = allPossibleData[i];
					box.Visible = true;
					i++;
				}
				else
				{
					break;
				}
			}

			//check the appropriate boxes
			foreach (CheckBox box in checkGroup)
			{
				if (selected.Contains(box.Text))
				{
					box.Checked = true;
				}
			}
		}

		/// <summary>
		/// Allow public access to which boxes are chosen
		/// </summary>
		public IList<string> Choices
		{
			get { return choices; }
		}

		/// <summary>
		/// Return ok
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">An EventArgs that contains no event data.</param>
		private void okButton_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
		}


		/// <summary>
		/// Update the choices array when checkboxes are clicked
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">An EventArgs that contains no event data.</param> 
		private void checkBox_CheckedChanged(object sender, EventArgs e)
		{
			CheckBox thisCheck = sender as CheckBox;
			if (thisCheck.Checked)
			{
				choices.Add(thisCheck.Text);
			}
			else
			{
				choices.Remove(thisCheck.Text);
			}
		}

	}
}