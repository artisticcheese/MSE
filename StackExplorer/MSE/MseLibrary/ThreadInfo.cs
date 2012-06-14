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

using System.Windows.Forms;

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
	/// Encapsulates the information retrieved about a Thread of a managed process
	/// </summary>
	public class ThreadInfo : IDisposable
	{

		//private members
		private CorThread thread;
		private Dictionary<string, CorMetadataImport> metaImportHash;
		private List<FrameInfo> frameStack;
		private ProcessThread generalThreadInfo;
		ProcessInfo processInfo;
		private int threadId;

		//constructors
		/// <summary>
		/// Initialze the StackTrace Class and create all the FrameInfo objects
		/// </summary>
		/// <param name="proc">The thread to get the stack trace of</param>
		internal ThreadInfo(CorThread thread, ProcessInfo procInfo)
		{
			if (thread == null)
			{
				throw new ArgumentNullException("thread");
			}
			if (procInfo == null)
			{
				throw new ArgumentNullException("procInfo");
			}

			this.thread = thread;
			metaImportHash = new Dictionary<string, CorMetadataImport>();
			frameStack = new List<FrameInfo>();
			threadId = thread.Id;
			processInfo = procInfo;
			//get the general thread information object
			generalThreadInfo = procInfo.GeneralThreads[threadId];
		}


		//properties
		/// <summary>
		/// get the arraylist containing all FrameInfo objects
		/// </summary>
		public IList<FrameInfo> FrameStack
		{
			get
			{
				return frameStack;
			}

		}

		/// <summary>
		/// Return the thread id
		/// </summary>
		public int ThreadId
		{
			get { return threadId; }
		}

		/// <summary>
		/// Return a ProcessThread object which contains general information about the thread
		/// </summary>
		public ProcessThread GeneralThreadInfo
		{
			get { return generalThreadInfo; }

		}

		//methods
		/// <summary>
		/// Loop through all frames in thread getting info on them then building
		/// a arrayList representation of the stack trace with them
		/// If called when already attached it will skip attaching and detaching and let the function that called it worry about that
		/// otherwise this function will deal with attaching and deataching
		/// </summary>
		/// <param name="depth">How many frames deep to display, 0 means show all</param>
		public void UpdateStackTrace(int depth)
		{

			frameStack.Clear();  //not needed right now cause this class is remade each update
			bool enteredAttached = processInfo.Attached;
			if (!enteredAttached)
			{
				processInfo.AttachAndStop();
			}
			try
			{
				int frameNum = 0;

				foreach (CorChain chain in thread.Chains)
				{

					foreach (CorFrame frame in chain.Frames)
					{
						if ((frameNum >= depth) && (depth != 0))
						{
							break;
						}
						if (frame.Function == null)
						{
							continue;
						}
						CorModule module = frame.Function.Module;
						string moduleName = module.Name;
						CorMetadataImport importer = null;
						if (metaImportHash.ContainsKey(moduleName))
						{
							importer = metaImportHash[moduleName] as CorMetadataImport;
						}
						else
						{//make a new importer then add it to the hash
							importer = new CorMetadataImport(module);
							metaImportHash[moduleName] = importer;
						}

						//add the next frame to the frame stack
						frameStack.Add(new FrameInfo(frame, importer));
						frameNum++;

					}
					if ((frameNum > depth) && (depth != 0))
					{
						break;
					}
				}
			}
			catch (COMException ex)
			{
				Debug.WriteLine(ex.ToString());
			}
			finally
			{
				if (!enteredAttached)
				{
					processInfo.DetachAndResume();
				}
			}
		}

		/// <summary>
		/// Dispose pattern
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Dispose of any outstanding reources like the lock on the pdb file
		/// </summary>
		/// <param name="disposing">Dispose managed objects</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				// Free other state (managed objects).
				metaImportHash = null;
				foreach (FrameInfo frame in frameStack)
				{
					frame.Dispose();
				}

				//GC collect hack from MDBG 
				//not sure if its needed but it cant hurt
				//HACK: Call Collect many times, determined empirically in the MDBG
				//      to make sure the .pdb file is released
				GC.Collect();
				GC.Collect();
				GC.WaitForPendingFinalizers();

				GC.Collect();
				GC.Collect();
				GC.WaitForPendingFinalizers();

				GC.Collect();
				GC.Collect();
				GC.WaitForPendingFinalizers();
			}
			// Free your own state (unmanaged objects).
		}
	}



}