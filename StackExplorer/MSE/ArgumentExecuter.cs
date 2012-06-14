//---------------------------------------------------------------------
//  This file is part of the Managed Stack Explorer (MSE).
// 
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Globalization;
using System.Resources;

using Microsoft.Mse.Library;
using System.Security.Permissions;

//[assembly: SecurityPermission(SecurityAction.RequestMinimum)]
namespace Microsoft.Mse.Gui
{
	/// <summary>
	/// Deals with execution of any command line arguments
	/// </summary>
	public class ArgumentExecuter : IDisposable
	{
		private string[] arguments; //array of the command line arguments
		private int argLength;

		private string saveTo; //file to save output to, if its empty it means print to screen
		private string errorMessage; //the current error that has occured

		//command flags
		private bool listProcessFlag;
		private bool identifiedProcessFlag;
		private bool listThreadFlag;
		private bool identifiedThreadFlag;
		private bool listStackFlag;
		private bool killProcessFlag;
		private bool saveFileFlag;
		private bool infoFlag;
		private bool helpFlag;
		private bool logStackFlag;

		private int logInterval;
		private int logDuration;
		private int logIteration;
		private int logTotalIterations;

		private int stackDepth;//how many frames to show at once

		private List<int> processIds;//list of processes to act on
		private List<int> threadIds; //list of threads to act on

		private Dictionary<string, List<string>> commandHash; //maping of names of comman

		private GeneralProcessData processData;

		// Create the delegate that invokes methods for the timer.
		private TimerCallback timerDelegate;
		private Timer logTimer;
		private ManualResetEvent waitTimer;


		const int DefaultInterval = 5;//5 second default interval

		private ResourceManager mainStrings;//get strings from resource file

		public ArgumentExecuter(string[] args)
		{
			mainStrings = new ResourceManager(typeof(Properties.Resources));
			arguments = args;
			argLength = arguments.Length;

			saveTo = "";
			errorMessage = "";

			logInterval = -1;
			logDuration = -1;
			logTotalIterations = -1;

			processIds = new List<int>();
			threadIds = new List<int>();

			commandHash = new Dictionary<string, List<string>>();
			commandHash["Process Command"] = new List<string>(new string[] { "/p", "/process", "/processes" });
			commandHash["Thread Command"] = new List<string>(new string[] { "/t", "/thread", "/threads" });
			commandHash["Stack Command"] = new List<string>(new string[] { "/s", "/stack", "/stacks", "/stacktrace" });
			commandHash["Kill Command"] = new List<string>(new string[] { "/k", "/kill" });
			commandHash["Info Command"] = new List<string>(new string[] { "/i", "/info" });
			commandHash["Help Command"] = new List<string>(new string[] { "/h", "/help", "/?" });
			commandHash["Out Command"] = new List<string>(new string[] { "/o", "/out", "/save" });
			commandHash["Log Command"] = new List<string>(new string[] { "/l", "/log" });
			commandHash["Interval Command"] = new List<string>(new string[] { "/int", "/interval" });
			commandHash["Duration Command"] = new List<string>(new string[] { "/dur", "/duration" });
			commandHash["Depth Command"] = new List<string>(new string[] { "/dep", "/depth" });

			processData = new GeneralProcessData();

			timerDelegate = new TimerCallback(LogStack);
			waitTimer = new ManualResetEvent(false);
		}


		/// <summary>
		/// Give public access to view the current error message
		/// </summary>
		public String ErrorMessage
		{
			get { return errorMessage; }
		}


		//disposable pattern
		/// <summary>
		/// Implenting dispose
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);

		}

		/// <summary>
		/// free timer
		/// </summary>
		/// <param name="disposing">Dispose managed objects</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				// Free other state (managed objects).
				if (waitTimer != null) waitTimer.Close();
				if (logTimer != null) logTimer.Dispose();
				if (processData != null) processData.Dispose();
			}
			// Free your own state (unmanaged objects).
		}

		/// <summary>
		/// Finalizer
		/// </summary>
		~ArgumentExecuter()
		{
			Dispose(false);
		}


		/// <summary>
		/// Parses the arguments and executes them
		/// *Note: This function has a cyclomatic complexity of 80!!!!!!!
		/// This needs to be revised if possible since that is apparantly very very high
		/// </summary>
		/// <returns>0 if no error occured and an error code otherwise</returns>

		// Ignore the complexity warning - don't want to rewrite it now.
		public int ExecuteArguments()
		{
			//parse command out of array
			for (int i = 0; i < argLength; i++)
			{
				if (commandHash["Process Command"].Contains(arguments[i]))
				{
					if (listProcessFlag || identifiedProcessFlag)
					{
						errorMessage = mainStrings.GetString("Error") + ": " + mainStrings.GetString("repeatedCommandError");
						return 1;
					}

					listProcessFlag = true;
					if (i < (argLength - 1))
					{
						if ((arguments[i + 1])[0] != '/')
						{
							i++;
							listProcessFlag = false;
							identifiedProcessFlag = true;
							string pIds = arguments[i];
							string[] processStringIds = pIds.Split(',', ';');
							foreach (string pid in processStringIds)
							{
								try
								{
									processIds.Add(Int32.Parse(pid, CultureInfo.CurrentCulture.NumberFormat));
								}
								catch (FormatException)
								{
									errorMessage = mainStrings.GetString("Error") + ": " + mainStrings.GetString("numberFormatError") + " \"" + pid + "\"";
									return 1;
								}
							}
						}
					}
				}
				else if (commandHash["Thread Command"].Contains(arguments[i]))
				{
					if (listThreadFlag || identifiedThreadFlag)
					{
						errorMessage = mainStrings.GetString("Error") + ": " + mainStrings.GetString("repeatedCommandError");
						return 1;
					}
					listThreadFlag = true;
					if (i < (argLength - 1))
					{
						if ((arguments[i + 1])[0] != '/')
						{
							i++;
							listThreadFlag = false;
							identifiedThreadFlag = true;
							string tIds = arguments[i];
							string[] threadStringIds = tIds.Split(',', ';');
							foreach (string tid in threadStringIds)
							{
								threadIds.Add(Int32.Parse(tid, CultureInfo.CurrentCulture.NumberFormat));
							}
						}
					}
				}
				else if (commandHash["Stack Command"].Contains(arguments[i]))
				{
					if (listStackFlag)
					{
						errorMessage = mainStrings.GetString("Error") + ": " + mainStrings.GetString("repeatedCommandError");
						return 1;
					}
					listStackFlag = true;

				}
				else if (commandHash["Kill Command"].Contains(arguments[i]))
				{
					if (killProcessFlag)
					{
						errorMessage = mainStrings.GetString("Error") + ": " + mainStrings.GetString("repeatedCommandError");
						return 1;
					}
					killProcessFlag = true;
				}
				else if (commandHash["Out Command"].Contains(arguments[i]))
				{
					if (saveFileFlag)
					{
						errorMessage = mainStrings.GetString("Error") + ": " + mainStrings.GetString("repeatedCommandError");
						return 1;
					}

					if (i < (argLength - 1))
					{
						if ((arguments[i + 1])[0] != '/')
						{
							i++;
							saveTo = arguments[i];

						}
					}
					else
					{
						errorMessage = mainStrings.GetString("Error") + ": " + mainStrings.GetString("parameterExpectedError") + " " + arguments[i];
						return 1;
					}
					saveFileFlag = true;
				}
				else if (commandHash["Help Command"].Contains(arguments[i]))
				{
					if (helpFlag)
					{
						errorMessage = mainStrings.GetString("Error") + ": " + mainStrings.GetString("repeatedCommandError");
						return 1;
					}
					helpFlag = true;
				}
				else if (commandHash["Info Command"].Contains(arguments[i]))
				{
					if (infoFlag)
					{
						errorMessage = mainStrings.GetString("Error") + ": " + mainStrings.GetString("repeatedCommandError");
						return 1;
					}
					infoFlag = true;
				}
				else if (commandHash["Interval Command"].Contains(arguments[i]))
				{
					if (logInterval != -1)
					{
						errorMessage = mainStrings.GetString("Error") + ": " + mainStrings.GetString("repeatedCommandError");
						return 1;
					}
					if (i < (argLength - 1))
					{
						if ((arguments[i + 1])[0] != '/')
						{
							i++;
							logInterval = Int32.Parse(arguments[i], CultureInfo.CurrentCulture.NumberFormat);

							if (logInterval <= 1)
							{

								errorMessage = mainStrings.GetString("Error") + ": " + mainStrings.GetString("intervalTimeError");
								return 1;
							}
						}
					}
					else
					{
						errorMessage = mainStrings.GetString("Error") + ": " + mainStrings.GetString("parameterExpectedError") + " " + arguments[i];
						return 1;
					}
				}
				else if (commandHash["Duration Command"].Contains(arguments[i]))
				{
					if (logDuration != -1)
					{
						errorMessage = mainStrings.GetString("Error") + ": " + mainStrings.GetString("repeatedCommandError");
						return 1;
					}
					if (i < (argLength - 1))
					{
						if ((arguments[i + 1])[0] != '/')
						{
							i++;
							logDuration = Int32.Parse(arguments[i], CultureInfo.CurrentCulture.NumberFormat);

							if (logDuration <= 1)
							{

								errorMessage = mainStrings.GetString("Error") + ": " + mainStrings.GetString("durationsError");
								return 1;
							}
						}
					}
					else
					{
						errorMessage = mainStrings.GetString("Error") + ": " + mainStrings.GetString("parameterExpectedError") + " " + arguments[i];
						return 1;
					}
				}
				else if (commandHash["Log Command"].Contains(arguments[i]))
				{
					if (logStackFlag)
					{
						errorMessage = mainStrings.GetString("Error") + ": " + mainStrings.GetString("repeatedCommandError");
						return 1;
					}
					else
					{
						logStackFlag = true;
					}
				}
				else if (commandHash["Depth Command"].Contains(arguments[i]))
				{
					if (stackDepth != 0)
					{
						errorMessage = mainStrings.GetString("Error") + ": " + mainStrings.GetString("repeatedCommandError");
						return 1;
					}
					if (i < (argLength - 1))
					{
						if ((arguments[i + 1])[0] != '/')
						{
							i++;
							stackDepth = Int32.Parse(arguments[i], CultureInfo.CurrentCulture.NumberFormat);

							if (stackDepth < 0)
							{
								errorMessage = mainStrings.GetString("Error") + ": " + mainStrings.GetString("depthError");
								return 1;
							}
						}
					}
				}
				else
				{
					errorMessage = mainStrings.GetString("Error") + ": " + mainStrings.GetString("unknownArgError") + "\"" + arguments[i] + "\"";
					return 1;
				}
			}



			//examine what flags are set and perform operations accordingly
			if (identifiedProcessFlag)
			{
				if (killProcessFlag && (listStackFlag || listThreadFlag || infoFlag || logStackFlag))
				{

					errorMessage = mainStrings.GetString("Error") + ": " + mainStrings.GetString("killError");
					return 1;
				}
				else if ((listStackFlag && infoFlag) || (listStackFlag && logStackFlag) || (infoFlag && logStackFlag))
				{

					errorMessage = mainStrings.GetString("Error") + ": " + mainStrings.GetString("combinationArgsError");
					return 1;
				}
				else if (listStackFlag)
				{
					if (threadIds.Count == 0 && listThreadFlag)
					{
						errorMessage = mainStrings.GetString("Error") + ": " + mainStrings.GetString("combinationArgsError");
						return 1;
					}
					foreach (int pid in processIds)
					{
						if (!PrintStackTrace(pid, threadIds))
						{
							return 1;
						}
					}
				}
				else if (listThreadFlag)
				{
					foreach (int pid in processIds)
					{
						PrintThreadList(pid);
					}
				}
				else if (killProcessFlag)
				{
					foreach (int pid in processIds)
					{
						if (KillProcess(pid) == false)
						{
							return 1;
						}
					}
				}
				else if (logStackFlag)
				{
					if (logInterval < 0)
					{
						logInterval = DefaultInterval;
					}

					if ((logDuration >= logInterval) || logDuration < 0)
					{
						if (logDuration > 0)
						{
							logTotalIterations = logDuration / logInterval;
						}
						else
						{
							logTotalIterations = -1; //indicats no stop keep repeating
						}

						if ((logTotalIterations > 0) || (logTotalIterations < 0))
						{
							string durString = "";
							if (logTotalIterations >= 0)
							{
								durString = String.Format(
									CultureInfo.CurrentCulture.NumberFormat,
									"{0} {1} {2}",
									mainStrings.GetString("forWord"),
									logDuration,
									mainStrings.GetString("secondsWord")
									);
							}
							Console.WriteLine(String.Format(
								CultureInfo.CurrentCulture.NumberFormat,
								"{0} {1} {2} {3}", mainStrings.GetString("logEveryWord"),
								logInterval,
								mainStrings.GetString("secondsWord"),
								durString)
								);

							logTimer = new Timer(timerDelegate, null, 0, logInterval * 1000);
							waitTimer.WaitOne();
							//if the asynchornus counter set the error message return an error to have this message shown
							if (!String.IsNullOrEmpty(errorMessage))
							{
								return 1;
							}
						}
					}
					else
					{
						errorMessage = mainStrings.GetString("Error") + ": " + mainStrings.GetString("invalidIntervalError");
						return 1;
					}
				}
				else if (infoFlag)
				{
					foreach (int pid in processIds)
					{
						if (PrintProcessInfo(pid) == false)
						{
							return 1;
						}
					}
				}
				else
				{
					if (threadIds.Count > 0)
					{
						errorMessage = mainStrings.GetString("Error") + ": " + mainStrings.GetString("moreArgsExpectedError");
						return 1;
					}
					foreach (int pid in processIds)
					{
						if (PrintProcessInfo(pid) == false)
						{
							return 1;
						}
					}
				}
			}
			else if (listProcessFlag)
			{
				if (killProcessFlag || listStackFlag || listThreadFlag || infoFlag)
				{
					errorMessage = mainStrings.GetString("Error") + ": " + mainStrings.GetString("invalidArgsError");
					return 1;
				}
				else
				{
					PrintProcessList();
				}
			}
			else if (helpFlag)
			{
				PrintHelpList();
			}
			else
			{
				errorMessage = mainStrings.GetString("Error") + ": " + mainStrings.GetString("invalidArgsError");
				return 1;
			}

			return 0;
		}


		/// <summary>
		/// Display info and data regarding the given process
		/// </summary>
		/// <param name="processId">Process to display info of</param>
		/// <returns></returns>
		private bool PrintProcessInfo(int processId)
		{
			ProcessInfoCollection processCollection = ProcessInfoCollection.SingleProcessCollection;
			ProcessInfo procInfo = null;

			try
			{
				procInfo = processCollection[processId];
				processData.ChangeDataInstance(procInfo);

				string dataString = String.Format(CultureInfo.CurrentCulture.NumberFormat, "{0} {1} [PID: {2}]{3}", mainStrings.GetString("processInfoFor"), procInfo.ShortName, processId, Environment.NewLine);
				foreach (string key in processData.ProcessData.Keys)
				{

					dataString += String.Format(CultureInfo.CurrentCulture.NumberFormat, "{0,-25}{1}{2}", key + ":", processData.ProcessData[key](), Environment.NewLine);
				}

				if (!String.IsNullOrEmpty(saveTo))
				{
					PrintToFile(dataString, false);
				}
				else
				{
					Console.WriteLine(dataString);
				}

			}
			catch (COMException ex)
			{
				Debug.WriteLine(ex.ToString());
			}
			catch (ArgumentNullException)
			{
				errorMessage = mainStrings.GetString("Error") + ": " + mainStrings.GetString("processIdError") + " \"" + processId + "\"";
				return false;
			}

			return true;
		}


		/// <summary>
		/// Asynchronus logging function - keeps logging until duration has ended
		/// </summary>
		/// <param name="state">Not used</param>
		private void LogStack(object state)
		{
			if ((logIteration < logTotalIterations) || (logTotalIterations < 0))
			{
				try
				{
					string stackString = "";
					foreach (int pid in processIds)
					{
						string tempString = GetStackTraceString(pid, threadIds);

						if (String.IsNullOrEmpty(tempString))
						{
							logIteration++;
							waitTimer.Set();
							return;
						}
						stackString += tempString;
					}

					if (!String.IsNullOrEmpty(saveTo))
					{
						PrintToFile(stackString, true);
					}
					else
					{
						Console.Write(stackString);
					}
					logIteration++;
					return; // return and don't wake up the main thread
				}
				catch (IOException ex)
				{
					// set errorMessage so that the main thread can figure out there is an error
					errorMessage = ex.Message;
				}
			}
			// wake up the main thread
			waitTimer.Set();
		}


		/// <summary>
		/// Gets a string of the full stack trace of a process or of some of its threads
		/// </summary>
		/// <param name="args">
		///	*required*	args[0] = process id
		///	*optional*  args[1]	= list of thread ids seperated by commas ex. 222,323,2322,1212			
		/// </param>
		/// <returns>false if arguments are incorrect</returns>
		private string GetStackTraceString(int processId, List<int> threadIDs)
		{
			string stackTrace = "";
			ProcessInfoCollection processCollection = ProcessInfoCollection.SingleProcessCollection;
			ProcessInfo procInfo = null;
			try
			{
				procInfo = processCollection[processId];
				stackTrace += String.Format(CultureInfo.CurrentCulture.NumberFormat, "{3}{2} {0} [PID: {1}] {3}", procInfo.ShortName, procInfo.ProcessId, mainStrings.GetString("stackTraceFor"), Environment.NewLine);
				List<string> stackList = new List<string>(procInfo.GetDisplayStackTrace(threadIDs, stackDepth));
				foreach (string line in stackList)
				{
					stackTrace += line;
				}
			}
			catch (COMException ex)
			{
				if (procInfo != null) procInfo.Dispose();
				Debug.WriteLine(ex.ToString());

				errorMessage = mainStrings.GetString("Error") + ": " + mainStrings.GetString("invalidProcOrThread");
				return "";
			}
			catch (NullReferenceException)
			{
				errorMessage = mainStrings.GetString("Error") + ": " + mainStrings.GetString("invalidProcOrThread");
				return "";
			}
			return stackTrace;

		}

		/// <summary>
		/// Print the full stack trace of a process or of some of its threads
		/// </summary>
		/// <param name="args">
		///	*required*	args[0] = process id
		///	*optional*  args[1]	= list of thread ids seperated by commas ex. 222,323,2322,1212			
		/// </param>
		/// <returns>false if arguments are incorrect</returns>
		private bool PrintStackTrace(int processId, List<int> threadIDs)
		{
			string stackTrace = GetStackTraceString(processId, threadIDs);
			if (String.IsNullOrEmpty(stackTrace))
			{
				return false;
			}
			if (!String.IsNullOrEmpty(saveTo))
			{
				PrintToFile(stackTrace, false);
			}
			else
			{
				Console.Write(stackTrace);
			}

			return true;
		}

		/// <summary>
		/// Print the list of processes
		/// </summary>
		/// <param name="args">
		/// *optional*	args[0] = true mean show all details
		/// </param>
		/// <returns>false if arguments are incorrect</returns>
		private bool PrintProcessList()
		{
			try
			{
				ProcessInfoCollection processCollection = ProcessInfoCollection.SingleProcessCollection;
				processCollection.RefreshProcessList();
				//print all the managed processes

				StringBuilder processListSb = new StringBuilder();
				string columnHead = String.Format(CultureInfo.CurrentCulture.NumberFormat, "{0,-10}{1}{2}", "PID", mainStrings.GetString("processName"), Environment.NewLine);
				if (!String.IsNullOrEmpty(saveTo))
				{
					processListSb.Append(columnHead);
				}
				else
				{
					Console.ForegroundColor = ConsoleColor.Cyan;
					Console.Write(columnHead);
					Console.ResetColor();
				}
				foreach (ProcessInfo procInf in processCollection)
				{
					processListSb.AppendFormat(CultureInfo.CurrentCulture.NumberFormat, "{0,-10}{1}{2}", procInf.ProcessId, procInf.ShortName, Environment.NewLine);
				}

				if (!String.IsNullOrEmpty(saveTo))
				{
					PrintToFile(processListSb.ToString(), false);
				}
				else
				{
					Console.Write(processListSb.ToString());
				}
			}
			catch (COMException ex)
			{
				Debug.WriteLine(ex.ToString());
			}
			catch (NullReferenceException ex)
			{
				Debug.WriteLine(ex.ToString());
			}

			return true;
		}

		/// <summary>
		/// Print all the managed threads of a process
		/// </summary>
		/// <param name="args">
		/// *required*	args[0] = process id
		/// </param>
		/// <returns>false if arguments are incorrect</returns>
		private bool PrintThreadList(int processId)
		{
			StringBuilder threadListSb = new StringBuilder();
			ProcessInfo procInfo = null;
			try
			{
				ProcessInfoCollection processCollection = ProcessInfoCollection.SingleProcessCollection;
				procInfo = processCollection[processId];
				procInfo.RefreshProcessInfo();

				threadListSb.AppendFormat(CultureInfo.CurrentCulture.NumberFormat, "{3}{2} {0} [PID: {1}] {3}", procInfo.GeneralProcessInfo.ProcessName, procInfo.ProcessId, mainStrings.GetString("threadsOf"), Environment.NewLine);
				foreach (ThreadInfo threadInfo in procInfo.Threads)
				{
					threadListSb.AppendFormat(CultureInfo.CurrentCulture.NumberFormat, "{1}# {0}{2}", threadInfo.ThreadId, mainStrings.GetString("threadWord"), Environment.NewLine);
				}

				if (!String.IsNullOrEmpty(saveTo))
				{
					PrintToFile(threadListSb.ToString(), false);
				}
				else
				{
					Console.Write(threadListSb.ToString());
				}
			}
			catch (COMException ex)
			{
				if (procInfo != null) procInfo.Dispose();
				Debug.WriteLine(ex.ToString());
			}
			catch (NullReferenceException ex)
			{
				Debug.WriteLine(ex.ToString());
			}

			return true;
		}

		/// <summary>
		/// Kill a process 
		/// </summary>
		/// <param name="args">
		///  *required*	args[0] = process id
		/// </param>
		/// <returns>false if arguments are incorrect</returns>
		private bool KillProcess(int processId)
		{
			try
			{
				ProcessInfoCollection processCollection = ProcessInfoCollection.SingleProcessCollection;
				processCollection[processId].Kill();
			}
			catch (NullReferenceException)
			{
				errorMessage = mainStrings.GetString("Error") + ": " + mainStrings.GetString("killIdError") + " \"" + processId + "\"";
				return false;
			}
			return true;
		}

		/// <summary>
		/// Saves output to the saveTo file define by comand line argument out
		/// </summary>
		/// <param name="data">The data to print to the saveTo file</param>
		private void PrintToFile(string data, bool append)
		{
			bool hardError = false;

			// set the default error message
			string errorMsg = String.Format(CultureInfo.CurrentCulture, mainStrings.GetString("invalidOutFileName"), mainStrings.GetString("Error"), saveTo);

			try
			{
				using (StreamWriter fileWriter = new StreamWriter(saveTo, append))
				{
					fileWriter.Write(data);
					fileWriter.Flush();
				}
			}
			catch (ArgumentException ex)
			{
				// Found invalid file name
				Debug.WriteLine(ex.ToString());
				hardError = true;
			}
			catch (UnauthorizedAccessException ex)
			{
				// Issue with accessing the file
				Debug.WriteLine(ex.ToString());
				hardError = true;
				errorMsg = String.Format(CultureInfo.CurrentCulture, mainStrings.GetString("outFileAccessDenied"), mainStrings.GetString("Error"), saveTo);
			}
			catch (PathTooLongException ex)
			{
				// Found invalid file name
				Debug.WriteLine(ex.ToString());
				hardError = true;
			}
			catch (DirectoryNotFoundException ex)
			{
				// Found invalid file name
				Debug.WriteLine(ex.ToString());
				hardError = true;
			}
			catch (IOException ex)
			{
				// most probably a transient error, try again the next time
				Console.Error.WriteLine(ex.ToString());
			}

			if (hardError)
			{
				// if there was an irrecoverable error, throw a new IOException
				throw new IOException(errorMsg);
			}
		}

		/// <summary>
		/// Print the help list for all commands
		/// </summary>
		private void PrintHelpList()
		{
			Process p = Process.GetCurrentProcess();
			string appName = System.IO.Path.GetFileNameWithoutExtension(p.MainModule.FileName).ToLower(CultureInfo.CurrentCulture);

			Console.WriteLine(String.Format(CultureInfo.CurrentCulture, "{0} v{1}", mainStrings.GetString("mseTitleString"), p.MainModule.FileVersionInfo.FileVersion));
			Console.WriteLine(String.Format(CultureInfo.CurrentCulture, "{0}{1}", mainStrings.GetString("mseCopyright"), Environment.NewLine));

			Console.WriteLine(String.Format(CultureInfo.CurrentCulture, "{0}:", mainStrings.GetString("descriptionWord")));
			Console.WriteLine(String.Format(CultureInfo.CurrentCulture, "{0}{1}", mainStrings.GetString("mseDescription"), Environment.NewLine));
			Console.WriteLine(String.Format(CultureInfo.CurrentCulture, mainStrings.GetString("usageString"), appName));
		}
	}
}