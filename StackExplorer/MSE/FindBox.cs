//---------------------------------------------------------------------
//  This file is part of the Managed Stack Explorer (MSE).
// 
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.Resources;

namespace Microsoft.Mse.Gui
{
	public partial class FindBox : Form
	{
		private RichTextBox dataBox;
		private int startPosition;
		private int endPosition;

		private RichTextBoxFinds upOrDown;
		private RichTextBoxFinds matchCase;
		private RichTextBoxFinds matchWord;

		private ResourceManager mainStrings;

		/// <summary>
		/// Constructor for findbox class,
		/// </summary>
		/// <param name="box">the text box which the find box will search</param>
		public FindBox(RichTextBox box)
		{
			InitializeComponent();
			dataBox = box;
			endPosition = -1;
			upOrDown = RichTextBoxFinds.None;
			matchCase = RichTextBoxFinds.None;
			matchWord = RichTextBoxFinds.None;
			mainStrings = new ResourceManager(typeof(Properties.Resources));
		}

		/// <summary>
		/// When the find button is clicked search the text box passed on the find options chosen
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">An EventArgs that contains no event data.</param>

		// We're not localizing it yet
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions")]
		private void findButton_Click(object sender, EventArgs e)
		{
			RichTextBoxFinds currOptions = upOrDown | matchCase | matchWord;
			int currPos = 0;
			if (startPosition >= 0 && !String.IsNullOrEmpty(searchBox.Text))
			{
				if ((currPos = dataBox.Find(searchBox.Text, startPosition, endPosition, currOptions)) != -1)
				{
					if (upOrDown == RichTextBoxFinds.None)
					{
						startPosition = currPos + 1;
					}
					else
					{
						endPosition = currPos - 1;
					}

				}
				else
				{

					MessageBox.Show(
								mainStrings.GetString("findBoxFinished"),
								mainStrings.GetString("findBoxCaption"),
						MessageBoxButtons.OK,
						MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
				}
			}

		}

		/// <summary>
		/// Cancel the find box
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">An EventArgs that contains no event data.</param>
		private void cancelButton_Click(object sender, EventArgs e)
		{
			this.Close();
			this.Dispose();
		}


		/// <summary>
		/// Search going up
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">An EventArgs that contains no event data.</param>
		private void upDirectionRadio_CheckedChanged(object sender, EventArgs e)
		{
			if (upDirectionRadio.Checked)
			{
				upOrDown = RichTextBoxFinds.Reverse;
				endPosition = startPosition;
				startPosition = 0;
			}
		}

		/// <summary>
		/// Search going down
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">An EventArgs that contains no event data.</param>
		private void downDirectionRadio_CheckedChanged(object sender, EventArgs e)
		{
			if (downDirectionRadio.Checked)
			{
				upOrDown = RichTextBoxFinds.None;
				startPosition = endPosition;
				endPosition = -1;
			}
		}

		/// <summary>
		/// Search only whole words
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">An EventArgs that contains no event data.</param>
		private void wholeWordCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			if (wholeWordCheckBox.Checked)
			{
				matchWord = RichTextBoxFinds.WholeWord;
			}
			else
			{
				matchWord = RichTextBoxFinds.None;
			}
		}


		/// <summary>
		/// Match case during search
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">An EventArgs that contains no event data.</param>
		private void matchCaseCheckBox_CheckedChanged(object sender, EventArgs e)
		{

			if (matchCaseCheckBox.Checked)
			{
				matchCase = RichTextBoxFinds.MatchCase;
			}
			else
			{
				matchCase = RichTextBoxFinds.None;
			}
		}

		/// <summary>
		/// Disable the find button if the text search box is empty
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">An EventArgs that contains no event data.</param>
		private void searchBox_TextChanged(object sender, EventArgs e)
		{
			if (searchBox.Text.Length == 0)
			{
				findButton.Enabled = false;
			}
			else
			{
				findButton.Enabled = true;
			}
		}

		/// <summary>
		/// Search when the user presses enter
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">An EventArgs that contains no event data.</param>
		private void searchBox_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				findButton.Focus();
				findButton_Click(null, null);
			}
		}

		/// <summary>
		/// By default give the find button focus
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">An EventArgs that contains no event data.</param>
		private void FindBox_GotFocus(object sender, System.EventArgs e)
		{
			findButton.Focus();
		}

	}
}