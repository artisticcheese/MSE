//---------------------------------------------------------------------
//  This file is part of the Managed Stack Explorer (MSE).
//
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//---------------------------------------------------------------------


using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.IO.IsolatedStorage;
using System.Globalization;

using Microsoft.Mse.Library;

namespace Microsoft.Mse.Gui
{

	/// <summary>
	/// Main form of the GUI for the Stack Explorer
	/// </summary>
	public partial class MainGui : Form
	{
		const int rightPanelInitialWidth = 450; //width to open the right panel to
		private bool preferencesChanged;
		private int rightPanelWidth;//holds inital width of right expandable panel

		public delegate void SaveDialogHandler(Stream fileStream);//delegate for the save dialog
		public delegate void MonitorDialogHandler(string file);//delegate for the monitor dialog box

		private ProcessInfoCollection processes; //
		private List<int> listProcessIds;//holds list of all process ids currently being displayed
		private List<int> listThreadIds; //holds list of all thread ids currently being displayed in thread view
		private List<int> threadSelectedIdList; //hold id of all the threads which were selected for the last stack trace

		private Dictionary<ProcessPriorityClass, int> priorityPosition;//hash to map priority positions to numbers
		private Dictionary<int, bool> processViewAscendingSort; //used to determine if a given columns is sorted and in what direction
		private Dictionary<int, bool> threadViewAscendingSort; //used to determine if a given columns is sorted and in what direction

		private Dictionary<int, List<int>> monitorList;//maps a processed id to the threads which are being monitored
		private Dictionary<int, string> monitorFiles; // the file names for each process being monitored

		private GeneralProcessData genProcessData; //holds general process data
		private List<String> selectedGeneralInformation; //list of the general info peices currently being diaplyed
		private List<Label> labelGroup; //list of all the labels in which to show general process info

		private FindBox findBox; //the find box for find info in the databox which hols stack information
		private PreferenceForm prefForm; //form to configure the programs preferences

		private int currentlySelectedProcess;//-1 if not process is select other stores id of that process

		private bool autoRefreshStack; // if set to true the stack trace will automatically refresh
		private bool autoRefreshThreadList; // if set to true the thread list will automatically refresh

		//customizable preferences
		private int processListRefreshRate; //how often the list of processes will refresh
		private int threadListRefreshRate; //how often the thread list will refresh
		private int stackRefreshRate; //how often the stack trace will refresh
		private int loggingRate; // how often will a log file be saved
		private int stackTraceDepth; //how deep a stack trace to display

		private ResourceManager mainStrings;

		public MainGui()
		{
			InitializeComponent();
			mainStrings = new ResourceManager(typeof(Properties.Resources));

			genProcessData = new GeneralProcessData();

			//currently diaplyed general information
			selectedGeneralInformation = new List<string>();
			selectedGeneralInformation.AddRange(new string[] {
				genProcessData.GeneralStrings.GetString("physicalMem"),
				genProcessData.GeneralStrings.GetString("virtualMem"),
				genProcessData.GeneralStrings.GetString("timeInGC"),
				genProcessData.GeneralStrings.GetString("processDescription"),
				genProcessData.GeneralStrings.GetString("cpuUsage"),
				genProcessData.GeneralStrings.GetString("cpuTime"),
				genProcessData.GeneralStrings.GetString("managedHeap")}
				);


			//array of label objects to populate general data into it
			labelGroup = new List<Label>();
			labelGroup.AddRange(new Label[] { label1, label2, label3, label4, label5,
				label6, label7, label8, label9, label10,label11,label12});


			//initialize priorities
			priorityPosition = new Dictionary<ProcessPriorityClass, int>();
			priorityPosition[ProcessPriorityClass.RealTime] = 0;
			priorityPosition[ProcessPriorityClass.High] = 1;
			priorityPosition[ProcessPriorityClass.AboveNormal] = 2;
			priorityPosition[ProcessPriorityClass.Normal] = 3;
			priorityPosition[ProcessPriorityClass.BelowNormal] = 4;
			priorityPosition[ProcessPriorityClass.Idle] = 5;

			//set up sorting process columns
			processViewAscendingSort = new Dictionary<int, bool>();
			processViewAscendingSort[(int)ProcessColumnsEnum.Process] = false;
			processViewAscendingSort[(int)ProcessColumnsEnum.ID] = false;

			//set up sorting for thread columns
			threadViewAscendingSort = new Dictionary<int, bool>();
			threadViewAscendingSort[(int)ThreadColumnsEnum.ID] = false;
			threadViewAscendingSort[(int)ThreadColumnsEnum.Reason] = false;
			threadViewAscendingSort[(int)ThreadColumnsEnum.State] = false;


			listProcessIds = new List<int>();//list of process ids
			listThreadIds = new List<int>(); //list of thread ids

			monitorList = new Dictionary<int, List<int>>();//maping of process ids to threads beings logged
			monitorFiles = new Dictionary<int, string>();//mapping of process id to file name to save log to
			threadSelectedIdList = new List<int>(); //currently selected threads

			processes = ProcessInfoCollection.SingleProcessCollection;

			currentlySelectedProcess = -1;
			rightPanelWidth = -1;

			prefForm = new PreferenceForm();
			try
			{
				//customizable preferences
				//this is where I will read from the settings file and get last saved data
				//default preferences
				processListRefreshRate = Properties.Settings.Default.processRefreshRate;
				threadListRefreshRate = Properties.Settings.Default.threadRefreshRate;
				stackRefreshRate = Properties.Settings.Default.stackRefreshRate;
				loggingRate = Properties.Settings.Default.loggingRate;
				stackTraceDepth = Properties.Settings.Default.stackDepth;

			}
			catch (FileNotFoundException)
			{
			}

			//set the timer interval
			processRefreshTimer.Interval = processListRefreshRate;
			threadRefreshTimer.Interval = threadListRefreshRate;
			stackRefreshTimer.Interval = stackRefreshRate;
			stackLogTimer.Interval = loggingRate;

		}


		// Implements the manual sorting of items by columns.
		private class ListViewItemComparer : IComparer
		{
			private int col;
			private bool sortAscending;
			private bool treatAsInt;
			public ListViewItemComparer()
				: this(0, false, false)
			{
			}
			public ListViewItemComparer(int column, bool ascending, bool isInt)
			{
				col = column;
				sortAscending = ascending;
				treatAsInt = isInt;
			}

			/// <summary>
			/// Compares two objects which could be strings or ints
			/// </summary>
			/// <param name="x">The first object</param>
			/// <param name="y">The second object</param>
			/// <returns>1 if x > y 0 if  x == y -1 if x < y</returns>
			public int Compare(object x, object y)
			{
				ListViewItem xListViewItem = x as ListViewItem;
				ListViewItem yListViewItem = y as ListViewItem;

				if (xListViewItem == null)
				{
					throw new ArgumentNullException("x");
				}

				if (yListViewItem == null)
				{
					throw new ArgumentNullException("y");
				}

				int xInt = 0;
				int yInt = 0;
				string xString = "";
				string yString = "";

				if (treatAsInt)
				{
					xInt = Int32.Parse(xListViewItem.SubItems[col].Text, CultureInfo.CurrentCulture);
					yInt = Int32.Parse(yListViewItem.SubItems[col].Text, CultureInfo.CurrentCulture);
					if (!sortAscending)
					{
						int zInt;
						zInt = yInt;
						yInt = xInt;
						xInt = zInt;
					}
					if (xInt > yInt)
					{
						return 1;
					}
					else if (yInt > xInt)
					{
						return -1;
					}
					else
					{
						return 0;
					}
				}
				else
				{
					xString = xListViewItem.SubItems[col].Text;
					yString = yListViewItem.SubItems[col].Text;
					if (!sortAscending)
					{
						string zString;
						zString = yString;
						yString = xString;
						xString = zString;
					}
					return String.Compare(xString, yString);
				}
			}
		}


		/// <summary>
		/// Represent the process view columns
		/// </summary>
		private enum ProcessColumnsEnum
		{
			Process,
			ID,
		}

		/// <summary>
		/// Represent the threadview columns
		/// </summary>
		private enum ThreadColumnsEnum
		{
			ID,
			State,
			Reason
		}

		/// <summary>
		/// Represent the elements in the process list context menu
		/// </summary>
		private enum ProcessListMenuEnum
		{
			Priority,
			Kill,
			Properties
		}

		/// <summary>
		/// Represent the elements in the toolbar context menu
		/// </summary>
		private enum ToolBarMenuEnum
		{
			View
		}

		/// <summary>
		/// Refresh the process list
		/// </summary>
		private void RefreshProcessList()
		{
			processes.RefreshProcessList();
			statusBarProcesses.Text = mainStrings.GetString("ManagedProcesses") + ": " + processes.Count;


			//check processView for any processes which are no longer running and remove them
			foreach (ListViewItem listItem in processView.Items)
			{
				int pID = Int32.Parse(listItem.SubItems[(int)ProcessColumnsEnum.ID].Text, CultureInfo.CurrentCulture.NumberFormat);
				if (!processes.ProccessHash.ContainsKey(pID))
				{
					//if removing currently viewed process clear the thread, data areas
					if (currentlySelectedProcess == pID)
					{
						currentlySelectedProcess = -1;
						threadView.Items.Clear();
						threadSelectedIdList.Clear();
					}
					processView.Items.Remove(listItem);
					listProcessIds.Remove(pID);
				}
			}


			//add any new processes to the list and the view
			foreach (ProcessInfo procInfo in processes)
			{
				if (!listProcessIds.Contains(procInfo.ProcessId))
				{
					//add processes icon
					Icon processIcon = ShellIcon.GetIcon(procInfo.FullName, false);
					iconList.Images.Add(procInfo.ProcessId.ToString(CultureInfo.CurrentCulture.NumberFormat), processIcon);

					ListViewItem processItem = new ListViewItem(new string[] {
					procInfo.ShortName,procInfo.ProcessId.ToString(CultureInfo.CurrentCulture.NumberFormat)}, -1);
					//add icon
					processItem.ImageKey = procInfo.ProcessId.ToString(CultureInfo.CurrentCulture.NumberFormat);

					processView.Items.Add(processItem);
					listProcessIds.Add(procInfo.ProcessId);
				}
			}
		}


		/// <summary>
		/// Change the thread view when the process changes
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">An EventArgs that contains no event data.</param>
		private void processView_SelectedIndexChanged(object sender, System.EventArgs e)
		{

			if (processView.SelectedItems.Count > 0)
			{
				int processID = Int32.Parse(processView.SelectedItems[0].SubItems[(int)ProcessColumnsEnum.ID].Text, CultureInfo.CurrentCulture.NumberFormat);
				if (processID != currentlySelectedProcess)
				{
					listThreadIds.Clear();
					threadSelectedIdList.Clear();
					dataBox.Clear();

				}
				currentlySelectedProcess = processID;//update which process is currently selected
				//update performance counter to correct process name
				try
				{
					genProcessData.ChangeDataInstance(processes[currentlySelectedProcess]);
				}
				catch (NullReferenceException ex)
				{
					Debug.WriteLine(ex.ToString());
				}
				UpdateGeneralInfoArea();

				//Update file names for saving
				monitorFileSaveDialog.FileName = logFileSaveDialog.FileName = DefaultLogFileName;

				if (monitorList.ContainsKey(currentlySelectedProcess))//is this process being logged/monitored
				{
					toolMonitorStack.ToolTipText = toolMonitorStack.Text = startMonitoringStackToolStripMenuItem.Text = mainStrings.GetString("StopLogging");
					toolMonitorStack.Image = Properties.Resources.NoAction;
					toolMonitorStack.Enabled = true;
					startMonitoringStackToolStripMenuItem.Enabled = true;
				}
				else
				{
					toolMonitorStack.ToolTipText = toolMonitorStack.Text = startMonitoringStackToolStripMenuItem.Text = mainStrings.GetString("StartLogging");
					toolMonitorStack.Image = Properties.Resources.FormRun;
				}
			}
			else if (processView.SelectedItems.Count == 0)//if no processes are selected
			{
				dataBox.Clear();
				listThreadIds.Clear();
				currentlySelectedProcess = -1;
				threadView.Items.Clear();
				ResetGeneralInfoArea();
				logFileSaveDialog.FileName = "";
				monitorFileSaveDialog.FileName = "";
				toolMonitorStack.Enabled = false;
				startMonitoringStackToolStripMenuItem.Enabled = false;
				toolViewStack.Enabled = false;
				viewStackToolStripMenuItem.Enabled = false;
			}
		}

		/// <summary>
		/// Returns the default name for the log file
		/// </summary>
		private string DefaultLogFileName
		{
			get
			{
				int processID = 0;
				if (processView.SelectedItems.Count > 0)
				{
					processID = Int32.Parse(processView.SelectedItems[0].SubItems[(int)ProcessColumnsEnum.ID].Text, CultureInfo.CurrentCulture.NumberFormat);
				}

				return "StackLog" + processID.ToString(CultureInfo.CurrentCulture.NumberFormat) +
						 String.Format(CultureInfo.CurrentCulture.DateTimeFormat, "_{0:s}", DateTime.Now).Replace(':', '_');
			}
		}

		/// <summary>
		/// Update the thread view on the click of a process
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">An EventArgs that contains no event data.</param>
		private void processView_DoubleClick(object sender, System.EventArgs e)
		{
			if (processView.SelectedItems.Count > 0)
			{
				int processID = Int32.Parse(processView.SelectedItems[0].SubItems[(int)ProcessColumnsEnum.ID].Text, CultureInfo.CurrentCulture.NumberFormat);
				if (processID != currentlySelectedProcess)
				{
					listThreadIds.Clear();
					threadSelectedIdList.Clear();
					dataBox.Clear();
				}

				currentlySelectedProcess = processID;
				UpdateThreadView(processID);
			}
			else if (processView.SelectedItems.Count == 0)
			{
				dataBox.Clear();
				listThreadIds.Clear();
				currentlySelectedProcess = -1;
				threadView.Items.Clear();
				ResetGeneralInfoArea();
				logFileSaveDialog.FileName = "";
				monitorFileSaveDialog.FileName = "";
			}
		}

		/// <summary>
		/// When user presses enter do the same as a double click
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">An EventArgs that contains no event data.</param>
		private void viewThreadsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			processView_DoubleClick(null, null);
		}

		/// <summary>
		/// When user presses enter do the same as a double click
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">An EventArgs that contains no event data.</param>
		private void processView_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				processView_DoubleClick(null, null);
			}
		}


		/// <summary>
		/// Enable key press command in thread view
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">An EventArgs that contains no event data.</param>
		private void threadView_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.Control && e.KeyCode == Keys.A)
			{
				foreach (ListViewItem item in threadView.Items)
				{
					item.Selected = true;
				}
			}
			else if (e.KeyCode == Keys.Enter)
			{
				toolViewStack_Click(null, null);
			}
		}


		/// <summary>
		/// Update which toolbar/menu items are enabled when index changes
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">An EventArgs that contains no event data.</param>
		private void threadView_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (threadView.SelectedItems.Count > 0)
			{
				toolMonitorStack.Enabled = true;
				startMonitoringStackToolStripMenuItem.Enabled = true;

				toolViewStack.Enabled = true;
				viewStackToolStripMenuItem.Enabled = true;
			}
			else
			{
				if (!monitorList.ContainsKey(currentlySelectedProcess))
				{
					toolMonitorStack.Enabled = false;
					startMonitoringStackToolStripMenuItem.Enabled = false;
				}

				toolViewStack.Enabled = false;
				viewStackToolStripMenuItem.Enabled = false;
			}

		}

		/// <summary>
		/// Display the stack trace of currently selected threads
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">An EventArgs that contains no event data.</param>
		private void viewStackToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ShowSelectedThreadsStackTraces();
			threadView.Focus();
		}

		/// <summary>
		/// Display the stack trace of currently selected threads
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">An EventArgs that contains no event data.</param>
		private void threadView_DoubleClick(object sender, System.EventArgs e)
		{
			ShowSelectedThreadsStackTraces();
			threadView.Focus();
		}

		/// <summary>
		/// When user clicks menu do the same as a double click
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">An EventArgs that contains no event data.</param>
		private void viewStackTraceToolStripMenuItem_Click(object sender, EventArgs e)
		{
			threadView_DoubleClick(null, null);
		}

		private void toolViewStack_Click(object sender, EventArgs e)
		{
			ShowSelectedThreadsStackTraces();
			threadView.Focus();
		}


		/// <summary>
		/// Displays the stack trace of the current loaded threads ids in threadSelectedIdList
		/// </summary>
		private void DisplayLoadedStackTrace()
		{
			dataBox.Clear();
			try
			{
				List<string> stackStrings = new List<string>(processes[currentlySelectedProcess].GetDisplayStackTrace(threadSelectedIdList, stackTraceDepth));
				foreach (string stackLine in stackStrings)
				{
					dataBox.AppendText(stackLine);
				}
			}
			catch (COMException ex) { Debug.WriteLine(ex.ToString()); }
		}


		/// <summary>
		/// Display the stack trace of all the threads that are currently selected
		/// </summary>
		private void ShowSelectedThreadsStackTraces()
		{
			if (threadView.SelectedItems.Count > 0 && processView.SelectedItems.Count > 0)
			{
				ExpandAndCollapse(true);
				threadSelectedIdList.Clear();
				foreach (ListViewItem item in threadView.SelectedItems)
				{
					threadSelectedIdList.Add(Int32.Parse(item.SubItems[(int)ThreadColumnsEnum.ID].Text, CultureInfo.CurrentCulture.NumberFormat));
				}
				DisplayLoadedStackTrace();

				dataBox.Select(0, 0);
				threadView.Focus();
			}
		}


		/// <summary>
		/// Update the thread view with the current threads for the selected process
		/// </summary>
		/// <param name="id">Id of the process to update the thread view of</param>
		private void UpdateThreadView(int id)
		{
			try
			{
				ProcessInfo procInfo = processes[id];
				procInfo.RefreshProcessInfo();//update processInfo to get correct thread list


				//remove threads which no longer exist
				foreach (ListViewItem listItem in threadView.Items)
				{
					//get thread id from list view
					int tID = Int32.Parse(listItem.SubItems[(int)ThreadColumnsEnum.ID].Text, CultureInfo.CurrentCulture.NumberFormat);

					bool contained = false;
					foreach (ThreadInfo threadInfo in procInfo.Threads)
					{
						if (threadInfo.ThreadId == tID)
						{
							contained = true;
							break;
						}
					}
					if (!contained)
					{
						threadView.Items.Remove(listItem);
						listThreadIds.Remove(tID);
					}
				}

				//add new threads
				foreach (ThreadInfo threadInfo in procInfo.Threads)
				{
					if (!listThreadIds.Contains(threadInfo.ThreadId))
					{
						String waitReason = "Running";
						try
						{
							waitReason = threadInfo.GeneralThreadInfo.WaitReason.ToString();
						}
						catch (InvalidOperationException) // if thread is not waiting anymore
						{
							// ignore, it just means the state has changed to Running.
						}
						ListViewItem threadItem = new ListViewItem(new string[] { threadInfo.ThreadId.ToString(CultureInfo.CurrentCulture.NumberFormat), threadInfo.GeneralThreadInfo.ThreadState.ToString(), waitReason }, -1);
						listThreadIds.Add(threadInfo.ThreadId);

						if (monitorList.ContainsKey(id))//change color if they are being monitored
						{
							if (monitorList[id].Contains(threadInfo.ThreadId))
							{
								threadItem.BackColor = Color.Yellow;
								threadItem.ToolTipText = mainStrings.GetString("BeingLoggedToolTip");
							}
						}

						threadView.Items.Add(threadItem);
					}
				}
			}
			catch (COMException ex) { Debug.WriteLine(ex.ToString()); }
			catch (KeyNotFoundException) { } // if the process is killed, we might get this exception
		}


		/// <summary>
		/// Reset info so that just name of the fields remain
		/// </summary>
		private void ResetGeneralInfoArea()
		{
			foreach (Label label in labelGroup)
			{
				label.Text = label.Text.Substring(0, label.Text.IndexOf(':') + 1);
			}
		}

		/// <summary>
		/// Clear everything from the general info area
		/// </summary>
		private void ClearGeneralInfoArea()
		{
			foreach (Label label in labelGroup)
			{
				label.Text = "";
			}
		}

		/// <summary>
		/// Update the data in the general info area by calling the appropriate selected data functions
		/// </summary>
		///
		private void UpdateGeneralInfoArea()
		{
			try
			{
				if (currentlySelectedProcess != -1)
				{
					int position = 0;
					foreach (string infoItem in selectedGeneralInformation)
					{
						labelGroup[position].Text = infoItem + ": " + genProcessData.ProcessData[infoItem]();
						position++;
					}
				}
				else
				{
					ResetGeneralInfoArea();
				}
			}
			catch (ArgumentException) { }

				// ignore Win32 exceptions which get thrown when the process dies and you're trying to get data
			catch (System.ComponentModel.Win32Exception) { }
		}

		/// <summary>
		/// Open the about dialog box
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">An EventArgs that contains no event data.</param>

		// We're not localizing it yet
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions")]
		private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Process p = Process.GetCurrentProcess();
			MessageBox.Show(mainStrings.GetString("mseTitleString") + " v" + p.MainModule.FileVersionInfo.FileVersion + Environment.NewLine + mainStrings.GetString("mseCopyright"),
			mainStrings.GetString("AboutBoxTitle"), MessageBoxButtons.OK,
			MessageBoxIcon.Information,
			MessageBoxDefaultButton.Button1);

		}

		/// <summary>
		/// Exit the program
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">An EventArgs that contains no event data.</param>
		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.Close();
		}


		/// <summary>
		/// Load the current priority of the process into the menu
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">An EventArgs that contains no event data.</param>
		void setPriorityMenuItem_DropDownOpened(object sender, System.EventArgs e)
		{
			//clear the checks
			foreach (ToolStripMenuItem menu in setPriorityMenuItem.DropDownItems)
			{
				menu.Checked = false;
			}

			if (processView.SelectedItems.Count > 0)
			{
				int pId = Int32.Parse(processView.SelectedItems[0].SubItems[(int)ProcessColumnsEnum.ID].Text, CultureInfo.CurrentCulture.NumberFormat);
				int newPos = priorityPosition[processes.GetProcess(pId).GeneralProcessInfo.PriorityClass];
				ToolStripMenuItem menu = setPriorityMenuItem.DropDownItems[newPos] as ToolStripMenuItem;
				menu.Checked = true;
			}
		}


		/// <summary>
		/// Kill the currently selected process
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">An EventArgs that contains no event data.</param>
		private void killMenuItem_Click(object sender, EventArgs e)
		{
			if (processView.SelectedItems.Count > 0)
			{
				int killId = Int32.Parse(processView.SelectedItems[0].SubItems[(int)ProcessColumnsEnum.ID].Text, CultureInfo.CurrentCulture.NumberFormat);
				processes[killId].Kill();
			}
		}

		/// <summary>
		/// Set priority level of the currently selected process
		/// </summary>
		/// <param name="level"></param>
		private void SetPriority(ProcessPriorityClass level)
		{
			if (processView.SelectedItems.Count > 0)
			{
				int pId = Int32.Parse(processView.SelectedItems[0].SubItems[(int)ProcessColumnsEnum.ID].Text, CultureInfo.CurrentCulture.NumberFormat);
				processes[pId].GeneralProcessInfo.PriorityClass = level;
			}
		}

		/// <summary>
		/// Set the priority level to real time
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">An EventArgs that contains no event data.</param>
		private void realTimeMenuItem_Click(object sender, EventArgs e)
		{
			SetPriority(ProcessPriorityClass.RealTime);
		}

		/// <summary>
		/// Set the priority level to high
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">An EventArgs that contains no event data.</param>
		private void highMenuItem_Click(object sender, EventArgs e)
		{
			SetPriority(ProcessPriorityClass.High);

		}

		/// <summary>
		/// Set the priority level toabove normal
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">An EventArgs that contains no event data.</param>
		private void aboveNormalItem_Click(object sender, EventArgs e)
		{
			SetPriority(ProcessPriorityClass.AboveNormal);
		}

		/// <summary>
		/// Set the priority level to normal
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">An EventArgs that contains no event data.</param>
		private void normalItem_Click(object sender, EventArgs e)
		{
			SetPriority(ProcessPriorityClass.Normal);
		}

		/// <summary>
		/// Set the priority level to below normal
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">An EventArgs that contains no event data.</param>
		private void belowNormalItem_Click(object sender, EventArgs e)
		{
			SetPriority(ProcessPriorityClass.BelowNormal);
		}

		/// <summary>
		/// Set the priority level to idle
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">An EventArgs that contains no event data.</param>
		private void lowMenuItem_Click(object sender, EventArgs e)
		{
			SetPriority(ProcessPriorityClass.Idle);
		}


		/// <summary>
		/// Set whether or not the window is staying ontop of all others
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">An EventArgs that contains no event data.</param>
		private void keepontopToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.TopMost = !this.TopMost;
		}


		/// <summary>
		/// Toggle the window between its open and closed states
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">An EventArgs that contains no event data.</param>
		private void collapseDataViewButton_Click(object sender, EventArgs e)
		{
			ExpandAndCollapse(false);

		}

		/// <summary>
		/// Toggle the window between its open and closed states
		/// </summary>
		/// <param name="stayOpen">Keep window open if already open</param>
		private void ExpandAndCollapse(bool stayOpen)
		{
			if (rightPanelWidth == -1)
			{
				rightPanelWidth = rightPanelInitialWidth;
			}
			if (listsSplitData.Panel2Collapsed == false)
			{
				if (!stayOpen)
				{
					rightPanelWidth = listsSplitData.Panel2.Width;
					this.Width -= listsSplitData.Panel2.Width;
					listsSplitData.Panel2Collapsed = true;
					collapseDataViewButton.Text = mainStrings.GetString("collapseDataViewButtonExpand");
					processView.Focus();
					toolSaveStack.Enabled = false;
					findToolStripMenuItem1.Enabled = findButton.Enabled = false;

					takeStackSnapShotToolStripMenuItem.Enabled = false;
				}
			}
			else
			{
				toolSaveStack.Enabled = true;
				findToolStripMenuItem1.Enabled = findButton.Enabled = true;
				takeStackSnapShotToolStripMenuItem.Enabled = true;

				listsSplitData.Panel2Collapsed = false;
				collapseDataViewButton.Text = mainStrings.GetString("collapseDataViewButtonCollapse");
				this.Width += rightPanelWidth;
				dataBox.Focus();
			}
		}


		/// <summary>
		/// The timer which will call the refresh list function
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">An EventArgs that contains no event data.</param>
		private void processRefreshTimer_Tick(object sender, EventArgs e)
		{
			RefreshProcessList();
		}


		/// <summary>
		/// Determine whether or not the thread refresher is enabled
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">An EventArgs that contains no event data.</param>
		private void toolThreadViewRefresh_Click(object sender, EventArgs e)
		{
			autoRefreshThreadList = !autoRefreshThreadList;
			toolThreadViewRefresh.Checked = autoRefreshThreadList;
			autoRefreshThreadListToolStripMenuItem.Checked = autoRefreshThreadList;
			threadRefreshTimer.Enabled = autoRefreshThreadList;
		}

		/// <summary>
		/// The timer which will call the updateThreadView function to reset it
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">An EventArgs that contains no event data.</param>
		private void threadRefreshTimer_Tick(object sender, EventArgs e)
		{
			if (autoRefreshThreadList)
			{
				try
				{
					int pId = Int32.Parse(processView.SelectedItems[0].SubItems[(int)ProcessColumnsEnum.ID].Text, CultureInfo.CurrentCulture.NumberFormat);
					UpdateThreadView(pId);
				}
				catch (System.ArgumentOutOfRangeException)
				{
					// Swallow this exception.
					// It happens when the process is killed and you're monitoring it.
				}
			}
		}

		/// <summary>
		/// Determine whether or not the stack refrsher is enabled
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">An EventArgs that contains no event data.</param>
		private void toolStackRefresh_Click(object sender, EventArgs e)
		{
			autoRefreshStack = !autoRefreshStack;
			toolStackRefresh.Checked = autoRefreshStack;
			autoRefreshStackTraceToolStripMenuItem.Checked = autoRefreshStack;
			stackRefreshTimer.Enabled = autoRefreshStack;
		}


		/// <summary>
		/// The timer which will call the refresh stack function
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">An EventArgs that contains no event data.</param>
		private void stackRefreshTimer_Tick(object sender, EventArgs e)
		{
			if (autoRefreshStack &&
				threadView.SelectedItems.Count > 0 &&
				processView.SelectedItems.Count > 0)
			{
				threadSelectedIdList.Clear();
				foreach (ListViewItem item in threadView.SelectedItems)
				{
					threadSelectedIdList.Add(Int32.Parse(item.SubItems[(int)ThreadColumnsEnum.ID].Text, CultureInfo.CurrentCulture.NumberFormat));
				}

				DisplayLoadedStackTrace();
			}
		}

		/// <summary>
		/// Start the thread to handle saving the stack trace
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">An EventArgs that contains no event data.</param>
		private void toolStackSnapShot_Click(object sender, EventArgs e)
		{
			if (processView.SelectedItems.Count > 0)
			{
				Thread saveDialogThread = new Thread(new ThreadStart(logFileSaveThread));
				saveDialogThread.SetApartmentState(ApartmentState.STA);
				saveDialogThread.Start();
			}
		}


		/// <summary>
		/// Thread to open dialog box and invoke function to save stack trace
		/// </summary>
		private void logFileSaveThread()
		{
			Stream saveStream;
			logFileSaveDialog.FileName = DefaultLogFileName;
			if (logFileSaveDialog.ShowDialog() == DialogResult.OK)
			{
				if ((saveStream = logFileSaveDialog.OpenFile()) != null)
				{
					//process results of the dialog on the main thread
					this.Invoke(new SaveDialogHandler(threadSafeLogFileSave), new object[] { saveStream });
				}
			}
		}

		/// <summary>
		/// Save the stack trace to a file
		/// </summary>
		/// <param name="saveStream"></param>
		private void threadSafeLogFileSave(Stream saveStream)
		{
			using (StreamWriter fileWriter = new StreamWriter(saveStream))
			{
				string stackData = dataBox.Text.Replace("\n", Environment.NewLine);
				fileWriter.Write(stackData);
			}
		}


		/// <summary>
		/// Create a new thread to open the monitor dialog box or stop monitoring currently selected process
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">An EventArgs that contains no event data.</param>
		private void toolMonitorStack_Click(object sender, EventArgs e)
		{
			if (!monitorList.ContainsKey(currentlySelectedProcess))
			{
				if (processView.SelectedItems.Count > 0 && threadView.SelectedItems.Count > 0)
				{
					//open the logging dialog box in a new thread
					Thread monitorDialogThread = new Thread(new ThreadStart(startMonitoringFileThread));
					monitorDialogThread.SetApartmentState(ApartmentState.STA);
					monitorDialogThread.Start();
				}
			}
			else
			{
				//stop logging the current process
				processView.SelectedItems[0].BackColor = Color.White;
				processView.SelectedItems[0].ToolTipText = "";
				foreach (ListViewItem threadItem in threadView.Items)
				{
					if (monitorList[currentlySelectedProcess].Contains(
						Int32.Parse(threadItem.SubItems[(int)ThreadColumnsEnum.ID].Text, CultureInfo.CurrentCulture.NumberFormat)))
					{
						threadItem.BackColor = Color.White;
						threadItem.ToolTipText = "";
					}
				}

				monitorList.Remove(currentlySelectedProcess);
				monitorFiles.Remove(currentlySelectedProcess);
				toolMonitorStack.ToolTipText = toolMonitorStack.Text = startMonitoringStackToolStripMenuItem.Text = "Start Stack Logging";
				toolMonitorStack.Image = Properties.Resources.FormRun;
			}
			threadView.Focus();
		}

		/// <summary>
		/// The thread which opens the dialog box and invokes the method to start the periodic monitoring
		/// </summary>
		private void startMonitoringFileThread()
		{
			monitorFileSaveDialog.FileName = DefaultLogFileName;
			if (monitorFileSaveDialog.ShowDialog() == DialogResult.OK)
			{
				//process results of the dialog on the main thread
				this.Invoke(new MonitorDialogHandler(monitorStackStart), new object[] { monitorFileSaveDialog.FileName });
			}
		}


		/// <summary>
		/// Starts periodic monitoring of the stack
		/// </summary>
		/// <param name="file"></param>
		private void monitorStackStart(string file)
		{
			if (!monitorList.ContainsKey(currentlySelectedProcess))
			{
				List<int> monitorThreadsIds = new List<int>();
				//add thread which are going to be logged
				foreach (ListViewItem item in threadView.SelectedItems)
				{
					monitorThreadsIds.Add(Int32.Parse(item.SubItems[(int)ThreadColumnsEnum.ID].Text, CultureInfo.CurrentCulture.NumberFormat));
				}
				monitorList[currentlySelectedProcess] = monitorThreadsIds;
				monitorFiles[currentlySelectedProcess] = file;

				toolMonitorStack.ToolTipText = toolMonitorStack.Text = startMonitoringStackToolStripMenuItem.Text = mainStrings.GetString("StopLogging");
				toolMonitorStack.Image = Properties.Resources.NoAction;

				processView.SelectedItems[0].BackColor = Color.Yellow;
				processView.SelectedItems[0].ToolTipText = mainStrings.GetString("BeingLoggedToolTip");
				foreach (ListViewItem threadItem in threadView.SelectedItems)
				{
					threadItem.BackColor = Color.Yellow;
					threadItem.ToolTipText = mainStrings.GetString("BeingLoggedToolTip");
				}

				stackLogTimer.Enabled = true;
			}
		}

		/// <summary>
		/// Timer tick which will save the monitored processes stacks
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">An EventArgs that contains no event data.</param>
		private void stackLogTimer_Tick(object sender, EventArgs e)
		{
			if (monitorList.Count == 0)
			{
				stackLogTimer.Enabled = false;
			}
			foreach (int pid in monitorFiles.Keys)
			{
				StringBuilder stackImage = new StringBuilder();

				if (listProcessIds.Contains(pid))
				{
					try
					{
						ProcessInfo process = processes[pid];
						process.RefreshProcessInfo();
						//build stack trace for file
						List<int> logIdList = new List<int>();
						List<int> removeThreadIdList = new List<int>();
						foreach (int threadId in monitorList[pid])
						{
							//if the thread if being logged and it still exists add it to the id list
							if (process.ThreadInfos.ContainsKey(threadId))
							{
								logIdList.Add(threadId);
							}
							else
							{
								//remember to remove this thread id
								removeThreadIdList.Add(threadId);
							}
						}

						//remove threads which no longer exist
						foreach (int threadId in removeThreadIdList)
						{
							monitorList[pid].Remove(threadId);
						}

						List<string> stackList = new List<string>(process.GetDisplayStackTrace(logIdList, stackTraceDepth));//get stack trace
						foreach (string line in stackList)
						{
							stackImage.Append(line);//turn the array into a string
						}
					}
					// COMException is thrown when the process being monitored dies
					catch (COMException ex)
					{
						stackImage.AppendLine("Caught a COMException: the process with PID " + pid + " might be dead.");
						Debug.WriteLine(ex.ToString());
					}
				}
				else
				{
					monitorList.Remove(pid);
					stackImage.AppendLine("Process with PID " + pid + " seems to be dead.");
				}

				try
				{
					//save stack to file
					using (StreamWriter fileWriter = new StreamWriter(monitorFiles[pid], true))
					{
						fileWriter.Write(stackImage.ToString());
						fileWriter.Flush();
					}
				}
				catch (IOException ex) { Debug.WriteLine(ex.ToString()); }
			}
		}

		/// <summary>
		/// Open the change General Info dialog box
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">An EventArgs that contains no event data.</param>
		private void changeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			List<string> allGeneralKeys = new List<string>(genProcessData.ProcessData.Keys);
			InfoSelect infoSelectWindow = new InfoSelect(selectedGeneralInformation, allGeneralKeys);
			infoSelectWindow.TopMost = this.TopMost;
			if (infoSelectWindow.ShowDialog() == DialogResult.OK)
			{
				selectedGeneralInformation = new List<string>(infoSelectWindow.Choices);
				ClearGeneralInfoArea();
			}
		}


		/// <summary>
		/// Sort the process view columns
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">An EventArgs that contains no event data.</param>
		private void processView_ColumnClick(object sender, ColumnClickEventArgs e)
		{
			processViewAscendingSort[e.Column] = !processViewAscendingSort[e.Column];//switch sorting from ascent to descent or vice versa
			processView.ListViewItemSorter = new ListViewItemComparer(e.Column, processViewAscendingSort[e.Column], (e.Column == (int)ProcessColumnsEnum.ID));
			//processView.Columns[e.Column].Text = "^";
		}

		/// <summary>
		/// Sort the thread view columns
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">An EventArgs that contains no event data.</param>
		private void threadView_ColumnClick(object sender, System.Windows.Forms.ColumnClickEventArgs e)
		{
			threadViewAscendingSort[e.Column] = !threadViewAscendingSort[e.Column];//switch sorting from ascent to descent or vice versa
			threadView.ListViewItemSorter = new ListViewItemComparer(e.Column, threadViewAscendingSort[e.Column], (e.Column == (int)ThreadColumnsEnum.ID));

		}

		/// <summary>
		/// Timer which will refresh the general info
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">An EventArgs that contains no event data.</param>
		private void dataRefreshTimer_Tick(object sender, EventArgs e)
		{
			if (currentlySelectedProcess != -1)
			{
				UpdateGeneralInfoArea();
			}
		}

		/// <summary>
		/// General function for syncrhonuze the toolbar with the action menu
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="type"></param>
		private void UpdateToolBarViewContextMenu(object sender, ToolStripItemDisplayStyle type)
		{
			foreach (ToolStripMenuItem menuItem in viewToolStripMenuItem.DropDownItems)
			{
				menuItem.Checked = false;
			}
			foreach (Object toolItem in mainToolBar.Items)
			{
				ToolStripButton toolButton = toolItem as ToolStripButton;
				if (toolButton != null)
				{
					toolButton.DisplayStyle = type;
				}
			}
			ToolStripMenuItem item = sender as ToolStripMenuItem;
			item.Checked = true;
		}

		private void imageToolStripMenuItem_Click(object sender, EventArgs e)
		{
			UpdateToolBarViewContextMenu(sender, ToolStripItemDisplayStyle.Image);
		}

		private void imageandTextToolStripMenuItem_Click(object sender, EventArgs e)
		{
			UpdateToolBarViewContextMenu(sender, ToolStripItemDisplayStyle.ImageAndText);
		}

		private void textToolStripMenuItem_Click(object sender, EventArgs e)
		{
			UpdateToolBarViewContextMenu(sender, ToolStripItemDisplayStyle.Text);
		}

		/// <summary>
		/// Copy selected text in data box
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">An EventArgs that contains no event data.</param>
		private void copyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			dataBox.Copy();
		}

		/// <summary>
		/// Select all text in the data box
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">An EventArgs that contains no event data.</param>
		private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			dataBox.SelectAll();

		}

		/// <summary>
		/// Handle key presses in data box
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">An EventArgs that contains no event data.</param>
		private void dataBox_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.Control && e.KeyCode == Keys.F)
			{
				OpenFindBox();
			}
		}


		/// <summary>
		/// Dispose of the find box
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">An EventArgs that contains no event data.</param>
		private void findBox_Disposed(object sender, EventArgs e)
		{
			findBox = null;
		}

		/// <summary>
		/// Open the find box
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">An EventArgs that contains no event data.</param>
		private void findToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OpenFindBox();
		}

		/// <summary>
		/// Open and find box and position it and initialize it
		/// </summary>
		private void OpenFindBox()
		{
			if (findBox == null)
			{
				findBox = new FindBox(dataBox);
				findBox.Disposed += new EventHandler(findBox_Disposed);
				findBox.Location = new Point(this.Left + this.Width / 2, this.Top + this.Height / 2);
			}
			findBox.Show();
			findBox.Focus();
		}

		/// <summary>
		/// Show the help file
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">An EventArgs that contains no event data.</param>
		private void helpToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			//not implemented yet
		}


		/// <summary>
		/// Open preference menu and save changes if desired
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">An EventArgs that contains no event data.</param>
		private void preferencesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			prefForm.ProcessRefreshRate = processListRefreshRate;
			prefForm.ThreadRefreshRate = threadListRefreshRate;
			prefForm.StackRefreshRate = stackRefreshRate;
			prefForm.LoggingRate = loggingRate;
			prefForm.StackDepth = stackTraceDepth;
			//make sure the top most glitch doesnt happen, it occurs if mse is set in top most mode and the dialog box isnt
			prefForm.TopMost = this.TopMost;
			if (prefForm.ShowDialog() == DialogResult.OK)
			{

				if (prefForm.ProcessRefreshRate != processListRefreshRate ||
					threadListRefreshRate != prefForm.ThreadRefreshRate ||
					stackRefreshRate != prefForm.StackRefreshRate ||
					loggingRate != prefForm.LoggingRate ||
					stackTraceDepth != prefForm.StackDepth)
				{
					preferencesChanged = true;
				}

				//set the intervals
				processRefreshTimer.Interval = processListRefreshRate = prefForm.ProcessRefreshRate;
				threadRefreshTimer.Interval = threadListRefreshRate = prefForm.ThreadRefreshRate;
				stackRefreshTimer.Interval = stackRefreshRate = prefForm.StackRefreshRate;
				stackLogTimer.Interval = loggingRate = prefForm.LoggingRate;
				stackTraceDepth = prefForm.StackDepth;
			}
		}


		/// <summary>
		/// If settings have been changed ask the user if he wants to save
		/// </summary>

		// We're not localizing it yet
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions")]
		private void saveSettignsCheck()
		{
			if (preferencesChanged)
			{

				if (MessageBox.Show(
						mainStrings.GetString("saveSettings"),
						mainStrings.GetString("MSEfull"),
						MessageBoxButtons.YesNo,
						MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
				{
					Properties.Settings.Default.processRefreshRate = processListRefreshRate;
					Properties.Settings.Default.threadRefreshRate = threadListRefreshRate;
					Properties.Settings.Default.stackRefreshRate = stackRefreshRate;
					Properties.Settings.Default.loggingRate = loggingRate;
					Properties.Settings.Default.stackDepth = stackTraceDepth;
					Properties.Settings.Default.Save();
				}
			}
		}

		private void goToMSEHomepageToolStripMenuItem_Click(object sender, EventArgs e)
		{
			new Thread(delegate()
			{
				System.Diagnostics.Process.Start("http://go.microsoft.com/fwlink/?LinkId=59380");
			}).Start();
		}

		private void gotoMSEForumToolStripMenuItem_Click(object sender, EventArgs e)
		{
			new Thread(delegate()
			{
				System.Diagnostics.Process.Start("http://go.microsoft.com/fwlink/?LinkId=59379");
			}).Start();
		}
	}
}