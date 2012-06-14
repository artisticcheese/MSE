//---------------------------------------------------------------------
//  This file is part of the Managed Stack Explorer (MSE).
// 
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//---------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Resources;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Globalization;

namespace Microsoft.Mse.Gui
{
	public partial class PreferenceForm : Form
	{
		public const int MinimumProcRefresh = 50;
		public const int MinimumThreadRefresh = 250;
		public const int MinimumStackRefresh = 1000;
		public const int MinimumLogRefresh = 1000;
		public const int MinimumStackDepth = 0;
		private ResourceManager mainStrings;

		/// <summary>
		/// Constructor for preference form
		/// </summary>
		public PreferenceForm()
		{
			InitializeComponent();
			mainStrings = new ResourceManager(typeof(Properties.Resources));
		}

		//properties
		/// <summary>
		/// Get the refresh rate setting for the process view
		/// </summary>
		public int ProcessRefreshRate
		{
			get
			{

				return Int32.Parse(procRefresh.Text, CultureInfo.CurrentCulture.NumberFormat);
			}
			set
			{
				procRefresh.Text = value.ToString(CultureInfo.CurrentCulture.NumberFormat);
			}
		}

		/// <summary>
		/// Get the refresh rate setting for the thread view
		/// </summary>
		public int ThreadRefreshRate
		{
			get
			{
				return Int32.Parse(threadRefresh.Text, CultureInfo.CurrentCulture.NumberFormat);
			}

			set
			{
				threadRefresh.Text = value.ToString(CultureInfo.CurrentCulture.NumberFormat);
			}
		}

		/// <summary>
		/// Get the refresh rate setting for the stack trace
		/// </summary>
		public int StackRefreshRate
		{
			get
			{
				return Int32.Parse(stackRefresh.Text, CultureInfo.CurrentCulture.NumberFormat);
			}

			set
			{
				stackRefresh.Text = value.ToString(CultureInfo.CurrentCulture.NumberFormat);
			}
		}

		/// <summary>
		/// Get the rate for which the stacks will be saved to a log file
		/// </summary>
		public int LoggingRate
		{
			get
			{
				return Int32.Parse(logRefresh.Text, CultureInfo.CurrentCulture.NumberFormat);
			}

			set
			{
				logRefresh.Text = value.ToString(CultureInfo.CurrentCulture.NumberFormat);
			}
		}

		/// <summary>
		/// The setting for how deep a stack trace to get
		/// </summary>
		public int StackDepth
		{
			get
			{
				return Int32.Parse(depthBox.Text, CultureInfo.CurrentCulture.NumberFormat);
			}

			set
			{
				depthBox.Text = value.ToString(CultureInfo.CurrentCulture.NumberFormat);
			}
		}

		// We're not localizing it yet
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions")]
		private void ErrorMessageBox(string errorMess)
		{
			MessageBox.Show(
				errorMess,
				mainStrings.GetString("MSEfull"),
				MessageBoxButtons.OK,
				MessageBoxIcon.Exclamation,
				MessageBoxDefaultButton.Button1);
		}

		/// <summary>
		/// Validates if all the form elements are valid
		/// </summary>
		/// <returns></returns>
		private bool ValidateForm()
		{
			string procRefreshErrorString = String.Format(CultureInfo.CurrentCulture, "{0} {1} {2}", mainStrings.GetString("processRefreshRateError"), MinimumProcRefresh, mainStrings.GetString("milliOrGreater"));
			string threadRefreshErrorString = String.Format(CultureInfo.CurrentCulture, "{0} {1} {2}", mainStrings.GetString("threadRefreshRateError"), MinimumThreadRefresh, mainStrings.GetString("milliOrGreater"));
			string stackRefreshErrorString = String.Format(CultureInfo.CurrentCulture, "{0} {1} {2}", mainStrings.GetString("stackRefreshRateError"), MinimumStackRefresh, mainStrings.GetString("milliOrGreater"));
			string logErrorString = String.Format(CultureInfo.CurrentCulture, "{0} {1} {2}", mainStrings.GetString("logRateError"), MinimumLogRefresh, mainStrings.GetString("milliOrGreater"));
			string depthErrorString = String.Format(CultureInfo.CurrentCulture, "{0}", mainStrings.GetString("depthError"));

			int interval = -1;
			try
			{
				interval = Int32.Parse(procRefresh.Text, CultureInfo.CurrentCulture.NumberFormat);
			}
			catch (System.FormatException)
			{
				ErrorMessageBox(procRefreshErrorString);
				procRefresh.Focus();
				return false;
			}
			if (interval < MinimumProcRefresh)
			{
				ErrorMessageBox(procRefreshErrorString);
				procRefresh.Focus();
				return false;
			}


			try
			{
				interval = Int32.Parse(threadRefresh.Text, CultureInfo.CurrentCulture.NumberFormat);
			}
			catch (System.FormatException)
			{
				ErrorMessageBox(threadRefreshErrorString);
				threadRefresh.Focus();
				return false;
			}
			if (interval < MinimumThreadRefresh)
			{
				ErrorMessageBox(threadRefreshErrorString);
				threadRefresh.Focus();
				return false;
			}

			try
			{
				interval = Int32.Parse(stackRefresh.Text, CultureInfo.CurrentCulture.NumberFormat);
			}
			catch (System.FormatException)
			{
				ErrorMessageBox(stackRefreshErrorString);
				stackRefresh.Focus();
				return false;
			}
			if (interval < MinimumStackRefresh)
			{
				ErrorMessageBox(stackRefreshErrorString);
				stackRefresh.Focus();
				return false;
			}


			try
			{
				interval = Int32.Parse(logRefresh.Text, CultureInfo.CurrentCulture.NumberFormat);
			}
			catch (System.FormatException)
			{
				ErrorMessageBox(logErrorString);
				logRefresh.Focus();
				return false;
			}
			if (interval < MinimumLogRefresh)
			{
				ErrorMessageBox(logErrorString);
				logRefresh.Focus();
				return false;
			}


			try
			{
				interval = Int32.Parse(depthBox.Text, CultureInfo.CurrentCulture.NumberFormat);
			}
			catch (System.FormatException)
			{
				ErrorMessageBox(depthErrorString);
				depthBox.Focus();
				return false;
			}
			if (interval < MinimumStackDepth)
			{
				ErrorMessageBox(depthErrorString);
				depthBox.Focus();
				return false;
			}

			return true;

		}

		/// <summary>
		/// Cancel out of prefence menu
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">An EventArgs that contains no event data.</param>
		private void cancelButton_Click(object sender, EventArgs e)
		{
			this.Close();
		}



		/// <summary>
		/// apply settings
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">An EventArgs that contains no event data.</param>
		private void acceptButton_Click(object sender, EventArgs e)
		{
			if (ValidateForm())
			{
				this.DialogResult = DialogResult.OK;
			}
		}

		/// <summary>
		/// Give the ok  button default focus
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">An EventArgs that contains no event data.</param>
		private void PreferenceForm_Shown(object sender, System.EventArgs e)
		{
			acceptButton.Focus();
		}
	}
}