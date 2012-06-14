//---------------------------------------------------------------------
//  This file is part of the Managed Stack Explorer (MSE).
// 
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Security.Permissions;

//[assembly: SecurityPermission(SecurityAction.RequestMinimum)]
namespace Microsoft.Mse.Gui
{
	public static class Program
	{
		struct STARTUPINFO
		{
			public Int32 cb;
			public string lpReserved;
			public string lpDesktop;
			public string lpTitle;
			public Int32 dwX;
			public Int32 dwY;
			public Int32 dwXSize;
			public Int32 dwYSize;
			public Int32 dwXCountChars;
			public Int32 dwYCountChars;
			public Int32 dwFillAttribute;
			public Int32 dwFlags;
			public Int16 wShowWindow;
			public Int16 cbReserved2;
			public IntPtr lpReserved2;
			public IntPtr hStdInput;
			public IntPtr hStdOutput;
			public IntPtr hStdError;
		}

		internal struct PROCESS_INFORMATION
		{
			public IntPtr hProcess;
			public IntPtr hThread;
			public int dwProcessId;
			public int dwThreadId;
		}

		// Keep in sync with native names
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1705:LongAcronymsShouldBePascalCased"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
		internal struct SECURITY_ATTRIBUTES
		{
			public int nLength;
			public IntPtr lpSecurityDescriptor;
			public bool bInheritHandle;
		}

		// Ignore P/Invoke warning
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass"), DllImport("kernel32.dll")]
		static extern IntPtr GetStdHandle(int stdHandle);

		// Ignore P/Invoke warning
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1414:MarkBooleanPInvokeArgumentsWithMarshalAs"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass"), DllImport("kernel32.dll")]
		static extern bool CreateProcess(string lpApplicationName,
			string lpCommandLine, ref SECURITY_ATTRIBUTES lpProcessAttributes,
			ref SECURITY_ATTRIBUTES lpThreadAttributes, bool bInheritHandles,
			uint dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory,
			[In] ref STARTUPINFO lpStartupInfo,
			out PROCESS_INFORMATION lpProcessInformation);


		/// <summary>
		/// The main entry point for the application.
		/// If command line arguments are passed in start as a command line application
		/// however if none are passed in create a new process to start as a gui
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
		public static int Main(string[] args)
		{
			//if true then new process isnt created which allows easier debugging
			bool debugMode = false;

			if (Debugger.IsAttached)
				debugMode = true;

			IntPtr INVALID_HANDLE_VALUE = (IntPtr)(-1);
			if (args == null || args.Length == 0)
			{
				const int STD_OUTPUT_HANDLE = -11;
				IntPtr stdOut = GetStdHandle(STD_OUTPUT_HANDLE);

				//if the stdOuit handle is -1 or we are in debug mode then run the gui
				if ((stdOut == INVALID_HANDLE_VALUE) || debugMode)
				{
					Application.EnableVisualStyles();
					Application.Run(new MainGui());
				}
				else
				{
					//create a new process to run the gui in

					const int DETACHED_PROCESS = 0x00000008;
					const int STARTF_USESHOWWINDOW = 0x00000001;
					const int STARTF_USESTDHANDLES = 0x00000100;
					const int SW_SHOW = 5;

					Process p = Process.GetCurrentProcess();

					//path to the exe
					string Application = System.IO.Path.GetFullPath(p.MainModule.FileName);
					string CommandLine = "";
					PROCESS_INFORMATION pInfo = new PROCESS_INFORMATION();
					STARTUPINFO sInfo = new STARTUPINFO();
					sInfo.cb = System.Runtime.InteropServices.Marshal.SizeOf(sInfo);
					sInfo.dwFlags = STARTF_USESHOWWINDOW | STARTF_USESTDHANDLES;
					sInfo.wShowWindow = SW_SHOW;

					//send stdOutput as -1 to tell new process to open as gui
					sInfo.hStdOutput = (IntPtr)(-1);

					SECURITY_ATTRIBUTES pSec = new SECURITY_ATTRIBUTES();
					SECURITY_ATTRIBUTES tSec = new SECURITY_ATTRIBUTES();
					pSec.nLength = Marshal.SizeOf(pSec);
					tSec.nLength = Marshal.SizeOf(tSec);

					//create the new process
					CreateProcess(Application, CommandLine, ref pSec, ref tSec, true, DETACHED_PROCESS,
									IntPtr.Zero, null, ref sInfo, out pInfo);
				}
			}
			else
			{
				//run in command line mode if arguments are present
				int error = 1;
				using (ArgumentExecuter argExecute = new ArgumentExecuter(args))
				{
					try
					{
						if ((error = argExecute.ExecuteArguments()) != 0)
						{
							Console.WriteLine(argExecute.ErrorMessage);
						}
					}
					catch (System.IO.IOException ex)
					{
						error = 1;
						Console.WriteLine(ex.Message);
					}
				}
				return error;
			}

			return 0;
		}
	}
}