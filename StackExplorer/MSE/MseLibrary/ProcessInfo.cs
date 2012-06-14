//---------------------------------------------------------------------
//  This file is part of the Managed Stack Explorer (MSE).
// 
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//---------------------------------------------------------------------

using System;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Reflection;
using System.Globalization;
using System.Resources;

using Microsoft.Mse.Gui.Properties;

using Microsoft.Samples.Debugging.CorDebug;
using Microsoft.Samples.Debugging.CorMetadata;
using Microsoft.Samples.Debugging.CorDebug.NativeApi;
using Microsoft.Samples.Debugging.CorPublish;
using Microsoft.Samples.Debugging.CorSymbolStore;
using System.Security.Permissions;

//[assembly: SecurityPermission(SecurityAction.RequestMinimum)]
namespace Microsoft.Mse.Library
{

	/// <summary>
	/// Encapsulates the information retrieved about a process
	/// </summary>
	public class ProcessInfo : IDisposable
	{

		//private members
		public delegate void ProcessInfoUpdate(ProcessInfo process);

		//Supress Naming, Design warnings for the event handlers. These are simple event handlers and we want to keep it that way.
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
		public event ProcessInfoUpdate ProcessInfoUpdateHandler;

		//Supress Naming, Design warnings for the event handlers. These are simple event handlers and we want to keep it that way.
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
		public event ProcessInfoUpdate AllStacksUpdateHandler;


		//general process info
		private int processId;
		private string processShortName;
		private string processFullName;
		private string description;
		private string company;

		private CorDebugger debugger;
		private CorProcess debuggeeProcess;
		//private CorPublishProcess cpp;
		private Process generalProcessInfo;

		private Dictionary<int, ProcessThread> generalThreadInfos; //assocaite a thread id with its general info object
		private Dictionary<int, ThreadInfo> processThreads; //has relating thread id to its threadInfo object
		private List<CorThread> processCorThreads;

		private ManualResetEvent attachedCompletedProcessEvent;


		// A bool which will mimic the state of attachedCompletedProcessEvent
		// I don't think I can directly find out if attachedCompletedProcessEvent is signaled or not
		// so I'll use this until I can
		private bool isAttached;

		/// <summary>
		/// Constructor for ProcessInfo
		/// </summary>
		/// <param name="procID">The id of the process</param>
		/// <param name="debugger">The debugger</param>
		public ProcessInfo(Process process, CorDebugger debug)
		{
			if (debug == null)
			{
				throw new ArgumentException("Null Debugger Exception");
			}

			processThreads = new Dictionary<int, ThreadInfo>();
			processCorThreads = new List<CorThread>();
			generalThreadInfos = new Dictionary<int, ProcessThread>();

			attachedCompletedProcessEvent = new ManualResetEvent(false);
			debugger = debug;
			processId = process.Id;
			generalProcessInfo = Process.GetProcessById(processId);

			//CorPublish cp = new CorPublish();
			//cpp = cp.GetProcess(processId);
			processFullName = process.MainModule.FileName;
			processShortName = System.IO.Path.GetFileName(process.MainModule.FileName);
			FileVersionInfo fileInfo = FileVersionInfo.GetVersionInfo(processFullName);
			description = fileInfo.FileDescription;
			company = fileInfo.CompanyName;

			//debuggerProcessInfo will be set when the updateInfo function is called
			//the reason for this is that the process must be stopped for this to take place 
			//this happen only when we want it to
		}

		//properties

		/// <summary>
		/// Get the current process id
		/// </summary>
		public int ProcessId
		{
			get { return processId; }
		}

		/// <summary>
		/// Get the full path name of the process
		/// </summary>
		public string FullName
		{
			get { return processFullName; }
		}

		/// <summary>
		/// Get the short name of the process
		/// </summary>
		public string ShortName
		{
			get { return processShortName; }
		}

		/// <summary>
		/// Get the company of the process
		/// </summary>
		public string Company
		{
			get { return company; }
		}

		/// <summary>
		/// Get the description of the process
		/// </summary>
		public string Description
		{
			get { return description; }
		}

		public bool Attached
		{
			get { return isAttached; }
		}

		public CorProcess CorProc
		{
			get { return debuggeeProcess; }
		}

		/// <summary>
		/// Gets the array of StackTraceInof objects for this process
		/// </summary>
		public ICollection<ThreadInfo> Threads
		{
			get
			{
				ThreadInfo[] threadArray = new ThreadInfo[processThreads.Values.Count];
				processThreads.Values.CopyTo(threadArray, 0);
				return threadArray;
			}
		}

		/// <summary>
		/// Gets the array of corThreads in this process
		/// </summary>
		public IList<CorThread> CorThreads
		{
			get { return processCorThreads; }
		}

		public Dictionary<int, ThreadInfo> ThreadInfos
		{
			get { return processThreads; }
		}

		/// <summary>
		/// Get hash of general threadinfo objects
		/// </summary>
		public Dictionary<int, ProcessThread> GeneralThreads
		{
			get { return generalThreadInfos; }
		}

		/// <summary>
		/// Return the System.Diagnostics Process object
		/// </summary>
		public Process GeneralProcessInfo
		{
			get { return generalProcessInfo; }
		}

		/// <summary>
		/// The debugger object which controls the process
		/// </summary>
		public CorDebugger Debugger
		{
			get { return debugger; }
			set { debugger = value; }
		}

		/// <summary>
		/// Gets the Stop Event.
		/// </summary>
		/// <value>The Stop Event.</value>
		public WaitHandle AttachedProcessEvent
		{
			get { return attachedCompletedProcessEvent; }
		}

		//methods

		/// <summary>
		/// Dispose pattern
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Dispose managed and unmanaged objects
		/// </summary>
		/// <param name="disposing">Dispose managed objects</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				// Free other state (managed objects).
				attachedCompletedProcessEvent.Close();
				foreach (ThreadInfo thread in processThreads.Values)
				{
					thread.Dispose();
				}
			}
			// Free your own state (unmanaged objects).
		}

		/// <summary>
		/// Get the thread info object of a specifc thread
		/// </summary>
		/// <param name="id">Id of the thread</param>
		/// <returns></returns>
		public ThreadInfo GetThreadInfo(int id)
		{
			return processThreads[id];
		}

		/// <summary>
		/// Update thread information in the process without building a stack trace
		/// </summary>
		public void RefreshProcessInfo()
		{
			UpdateProcessInfo(false, false, null, 0);

			//fire event to notify that processor info is up to date
			if (ProcessInfoUpdateHandler != null)
			{
				ProcessInfoUpdateHandler(this);
			}
		}

		/// <summary>
		/// Returns an array of formated strings for printing the stack trace
		/// *NOTE: changing the format of the stack trace will cause the unit test to fail since
		/// the unit test has hardcoded stack traces in it: be advised!
		/// </summary>
		/// <param name="threadSelectedIdList">List of thread ids to get stack trace of, </param>
		/// <param name="depth">How many frames depe to display, 0 means show all</param>
		/// <returns></returns>
		public IList<string> GetDisplayStackTrace(IList<int> threadSelectedIdList, int depth)
		{
			ResourceManager stackStrings = new ResourceManager(typeof(Resources));
			if (threadSelectedIdList == null)
			{
				threadSelectedIdList = new List<int>();
			}

			List<string> stackTrace = new List<string>();
			try
			{
				if (threadSelectedIdList.Count == 0)
				{
					UpdateAllStackTraces(depth);
					foreach (ThreadInfo t in processThreads.Values)
					{
						threadSelectedIdList.Add(t.ThreadId);
					}
				}
				else
				{
					UpdateSelectedThreadStacks(threadSelectedIdList, depth);
				}

				stackTrace.Add(String.Format(CultureInfo.CurrentCulture.DateTimeFormat, "{1}{0:G}{1}", DateTime.Now, Environment.NewLine));
				if (depth == 1)
				{

					stackTrace.Add(String.Format(CultureInfo.CurrentCulture, "{0} {1} {2}{3}", stackStrings.GetString("StackDepthIs"), depth, stackStrings.GetString("wordFrame"), Environment.NewLine));
				}
				else if (depth > 1)
				{
					stackTrace.Add(String.Format(CultureInfo.CurrentCulture, "{0} {1} {2}{3}", stackStrings.GetString("StackDepthIs"), depth, stackStrings.GetString("wordFrames"), Environment.NewLine));
				}
				else
				{
					stackTrace.Add(stackStrings.GetString("stackDepthAll") + Environment.NewLine);
				}
				foreach (int threadId in threadSelectedIdList)
				{
					try
					{
						ThreadInfo threadInfo = processThreads[threadId];
						stackTrace.Add(String.Format(CultureInfo.CurrentCulture.NumberFormat, "{3}{1} {2}: {0}{3}", threadInfo.ThreadId, stackStrings.GetString("wordThread"), stackStrings.GetString("wordID"), Environment.NewLine));
						int i = 0;
						foreach (FrameInfo frameInfo in threadInfo.FrameStack)
						{
							if (frameInfo.FunctionLineNumber > -1)
							{
								stackTrace.Add(String.Format(CultureInfo.CurrentCulture.NumberFormat, "\t{0}. {1} ({2}:{3}){4}", i, frameInfo.FunctionFullName, frameInfo.FunctionFileName, frameInfo.FunctionLineNumber, Environment.NewLine));
							}
							else
							{
								stackTrace.Add(String.Format(CultureInfo.CurrentCulture.NumberFormat, "\t{0}. {1} ({2}){3}", i, frameInfo.FunctionFullName, frameInfo.FunctionFileName, Environment.NewLine));
							}
							i++;
						}
					}
					catch (COMException ex) { Debug.WriteLine(ex.ToString()); }
					catch (KeyNotFoundException ex)
					{
						Debug.WriteLine(ex.ToString());
					}
				}
			}
			catch (COMException ex) { Debug.WriteLine(ex.ToString()); }

			return stackTrace;
		}

		/// <summary>
		/// Briefly stops the process and updates the stack traces of all the threads.  This is more efficient than calling 
		/// ThreadInfo.UpdateStackTrace for each thread because that will attach, stop and detach the process for each thread
		/// UpdateAllStackTraces will only do that once.
		/// </summary>
		/// <param name="depth">How many frames deep to display, 0 means show all</param>
		public void UpdateAllStackTraces(int depth)
		{
			UpdateProcessInfo(true, true, null, depth);
			//fire event if we have a subscriber to notify all stacks were updated
			if (AllStacksUpdateHandler != null)
			{
				AllStacksUpdateHandler(this);
			}
			//fire event to notify that processor info is up to date
			if (ProcessInfoUpdateHandler != null)
			{
				ProcessInfoUpdateHandler(this);
			}
		}

		/// <summary>
		/// Update stack traces of selected threads
		/// </summary>
		/// <param name="threadIds">List of thread ids to get stack traces of</param>
		/// <param name="depth">How many frames deep to display, 0 means show all</param>
		public void UpdateSelectedThreadStacks(IList<int> threadIds, int depth)
		{
			bool displayAll = false;
			//make sure not null
			if (threadIds == null)
			{
				//if threadIds is null it means display all also
				threadIds = new List<int>();
				displayAll = true;
			}
			UpdateProcessInfo(true, displayAll, threadIds, depth);
		}

		/// <summary>
		/// Kills this process safely
		/// </summary>
		public void Kill()
		{
			try
			{
				if (isAttached)
				{
					DetachAndResume();
				}

				//we are now sure its detached so kill
				generalProcessInfo.Kill();
			}
			catch (InvalidOperationException) { }
		}

		/// <summary>
		/// Attach and stop process
		/// </summary>
		internal void AttachAndStop()
		{
			if (!isAttached)
			{
				try
				{
					debuggeeProcess = debugger.DebugActiveProcess(processId, false);

					debuggeeProcess.OnCreateProcess += new CorProcessEventHandler(this.CreateProcessEventHandler);
					debuggeeProcess.OnCreateThread += new CorThreadEventHandler(this.CreateThreadEventHandler);
					debuggeeProcess.OnThreadExit += new CorThreadEventHandler(this.ExitThreadEventHandler);
					debuggeeProcess.OnCreateAppDomain += new CorAppDomainEventHandler(this.CreateAppDomainEventHandler);
					debuggeeProcess.OnAppDomainExit += new CorAppDomainEventHandler(this.ExitAppDomainEventHandler);
					debuggeeProcess.OnProcessExit += new CorProcessEventHandler(this.ExitProcessEventHandler);
					debuggeeProcess.OnModuleLoad += new CorModuleEventHandler(this.CreateModuleEventHandler);

					Go().WaitOne(); //run the process but wait until its been attached before proceeding
					//it should be stopped now
				}
				catch (COMException ex)
				{
					Debug.WriteLine(ex.ToString());
				}
			}
		}

		/// <summary>
		/// Detach from the process and resume its normal operation
		/// </summary>
		// Supress warning to catch Exception in DetachAndResume since we don’t want to crash for any exception thrown during Detach()
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		internal void DetachAndResume()
		{
			try
			{
				if (debuggeeProcess != null && isAttached)
				{
					Detach();
					debuggeeProcess.Dispose();
					debuggeeProcess = null;
				}

			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.ToString());
			}
		}

		/// <summary>
		/// Update process info and thread stack traces
		/// </summary>
		/// <param name="withStack">Updates stack traces or just general info</param>
		/// <param name="allThreads">If updating stack trace should we update all of them</param>
		/// <param name="threadIds">If not updating all which ones do we update</param>
		/// <param name="depth">How many frames deep to display, 0 means show all</param>
		private void UpdateProcessInfo(bool withStack, bool allThreads, IList<int> threadIds, int depth)
		{
			processThreads.Clear();
			processCorThreads.Clear();
			generalThreadInfos.Clear();

			try
			{
				AttachAndStop();

				//build the general thread information hash
				foreach (ProcessThread gThread in generalProcessInfo.Threads)
				{
					generalThreadInfos[gThread.Id] = gThread;
				}

				if (isAttached)
				{
					//add the threads to an arraylist
					foreach (CorThread cThread in debuggeeProcess.Threads)
					{
						try
						{
							ThreadInfo thread = new ThreadInfo(cThread, this);
							if (withStack)//update stacks also
							{
								if (allThreads || threadIds.Contains(cThread.Id))
								{
									thread.UpdateStackTrace(depth);
								}
							}
							processCorThreads.Add(cThread);
							processThreads[cThread.Id] = thread;
						}
						catch (KeyNotFoundException ex)
						{
							// This can happen if the thread died before we could get to it
							Debug.WriteLine(ex.ToString());
						}
					}
				}
			}
			catch (COMException ex)
			{
				Debug.WriteLine(ex.ToString());
			}
			finally
			{
				DetachAndResume();
			}
		}

		/// <summary>
		/// Event Handler for the creation of the Debugee's process
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">An EventArgs that contains no event data.</param>
		private void CreateProcessEventHandler(object sender, CorProcessEventArgs e)
		{
			//Console.WriteLine("OnCreateProcess");
			//e.Continue = true;
			e.Continue = true;
		}


		/// <summary>
		/// Event Handler for the creation of the Debugee's appdomains
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">An EventArgs that contains no event data.</param>
		private void CreateAppDomainEventHandler(object sender, CorAppDomainEventArgs e)
		{
			//Console.WriteLine("OnCreateAppDomain");
			e.AppDomain.Attach();
			e.Continue = true;

		}


		void CreateModuleEventHandler(object sender, CorModuleEventArgs e)
		{
			//Console.WriteLine("OnCreateModule" + e.Module.Name);
			e.Continue = true;
		}

		/// <summary>
		/// Event Handler for the creation of the Debugee's threads
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">An EventArgs that contains no event data.</param>
		private void CreateThreadEventHandler(object sender, CorThreadEventArgs e)
		{
			if (!e.Thread.Process.HasQueuedCallbacks(null))
			{
				e.Continue = false;
				SignalAttachedProcess();
			}
			else
			{
				e.Continue = true;
			}

		}

		/// <summary>
		/// Event handler for when the process exits
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">An EventArgs that contains no event data.</param>
		private void ExitProcessEventHandler(object sender, CorProcessEventArgs e)
		{
			Dispose();
		}

		/// <summary>
		/// Event handler for when a thread of the process exits
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">An EventArgs that contains no event data.</param>
		private void ExitThreadEventHandler(object sender, CorThreadEventArgs e)
		{

		}


		/// <summary>
		/// Event handler for when a AppDomain of the process exits
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">An EventArgs that contains no event data.</param>
		private void ExitAppDomainEventHandler(object sender, CorAppDomainEventArgs e)
		{

		}



		/// <summary>
		/// Whenever process is finsihed attaching this is called
		/// could be combined into oncreate thread since that seems to be the only time this is needed
		/// however for now it will stay seperate incase I want to expand and make this more general
		/// </summary>
		private void SignalAttachedProcess()
		{

			//<strip>Learn what the reason for this sleep statement is his comments dont help</strip>
			Thread.Sleep(100); // <strip>@TODO HACK -- when we are running with crun command we need to give some
			// time to thread capturing output to process.</strip>

			//is attahed now
			isAttached = true;
			//set signal
			attachedCompletedProcessEvent.Set();
		}

		/// <summary>
		/// Detach current process from debugger
		/// change signaled on attachedCompletedProcessEvent
		/// </summary>
		private void Detach()
		{
			try
			{
				try
				{
					debuggeeProcess.Stop(int.MaxValue);
				}
				catch (COMException ex)
				{
					Debug.WriteLine(ex.ToString());
				}
				debuggeeProcess.Detach();
				isAttached = false;
				attachedCompletedProcessEvent.Reset();//not attached anymore
			}
			catch (COMException ex)
			{
				Debug.WriteLine(ex.ToString());
			}
		}

		/// <summary>
		/// Continues the process and returns a waitHandle object which is signaled if the process 
		/// is currently attached to a debugger
		/// </summary>
		/// <returns></returns>
		private WaitHandle Go()
		{
			try
			{
				debuggeeProcess.Continue(false);
			}
			catch (COMException)
			{

			}
			return attachedCompletedProcessEvent;
		}
	}
}
