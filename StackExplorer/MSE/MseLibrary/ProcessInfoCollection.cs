//---------------------------------------------------------------------
//  This file is part of the Managed Stack Explorer (MSE).
// 
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//---------------------------------------------------------------------

using System;
using System.Text;
using System.Collections;
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
	using System.Linq;

	/// <summary>
	/// The ProcessInfoCollection class which holds all ProcessIngo objects
	/// This is a singleton class since mroe than one isnt needed and will just cause conflicts
	/// </summary>

	// Supress message to implement a strongly typed CopyTo – not needed
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1035:ICollectionImplementationsHaveStronglyTypedMembers")]
	public class ProcessInfoCollection : ICollection, IDisposable
	{

		//private members
		private Dictionary<int, ProcessInfo> processes;//contains hashtable mapping process id's to their ProcessInfo objects

		static private ProcessInfoCollection singleProcessCollection = null;//hold the only object of this class

		//constructors
		private ProcessInfoCollection()
		{
			processes = new Dictionary<int, ProcessInfo>();
			RefreshProcessList();//refresh process hash   
		}


		//types
		internal class Enumerator : IEnumerator
		{
			//private members
			private ProcessInfoCollection processCollection;
			private int position;

			//constructor
			public Enumerator(ProcessInfoCollection procCollection)
			{
				position = -1;
				processCollection = procCollection;
			}

			//properties
			/// <summary>
			/// get the current object
			/// </summary>
			public object Current
			{
				get
				{
					if (processCollection.Count > 0 && position >= 0 && position < processCollection.Count)
					{
						ProcessInfo[] procArray = new ProcessInfo[processCollection.processes.Count];
						processCollection.processes.Values.CopyTo(procArray, 0);
						return procArray[position];
					}
					else
					{
						return null;
					}
				}
			}


			//methods
			/// <summary>
			/// Advance to the next element
			/// </summary>
			/// <returns>true if another element exists, false if not</returns>
			public bool MoveNext()
			{
				position++;
				if (position >= processCollection.Count)
				{
					position--;
					return false;
				}
				else
				{
					return true;
				}
			}

			/// <summary>
			/// reset the state of the internal element pointer
			/// </summary>
			public void Reset()
			{
				position = -1;
			}
		}//end enumerator


		//properties
		/// <summary>
		/// Get the hash containg the processInfo objects
		/// </summary>
		public Dictionary<int, ProcessInfo> ProccessHash
		{
			get { return processes; }
		}

		/// <summary>
		/// Get an array with all processInfo objects
		/// </summary>
		public ICollection<ProcessInfo> ProcessInfos
		{
			get
			{
				return processes.Values;
			}
		}

		/// <summary>
		/// Get array with all managed process id's
		/// </summary>
		public ICollection<int> ProccessIds
		{
			get
			{
				return processes.Keys;
			}
		}

		public ProcessInfo this[int value]
		{

			get { return GetProcess(value); }
		}

		/// <summary>
		/// Number of managed process stored
		/// </summary>
		public int Count
		{
			get { return processes.Values.Count; }
		}

		/// <summary>
		/// Is it synchronized?
		/// </summary>
		public bool IsSynchronized
		{
			//not sure so i'll say false
			get { return false; }
		}
		/// <summary>
		/// Object which when locked will make this class synchronized
		/// </summary>
		public object SyncRoot
		{
			get { return processes; }
		}

		//method
		/// <summary>
		/// Created and returns a ProcessInfoCollection object
		/// If one exists already it returns that
		/// </summary>
		/// <returns></returns>

		public static ProcessInfoCollection SingleProcessCollection
		{
			get
			{
				if (singleProcessCollection == null)
				{
					singleProcessCollection = new ProcessInfoCollection();
				}

				return singleProcessCollection;
			}
		}

		/// <summary>
		/// Get enumartor object for this collecion
		/// </summary>
		/// <returns>Enumarator</returns>
		public IEnumerator GetEnumerator()
		{
			return new Enumerator(this);
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
		/// Dispose managed and unmanaged objects
		/// </summary>
		/// <param name="disposing">Dispose managed objects</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				// Free other state (managed objects).
				foreach (ProcessInfo proc in processes.Values)
				{
					proc.Dispose();
				}
			}
			// Free your own state (unmanaged objects).
		}

		#region ICollection Members

		/// <summary>
		/// Copy to an array
		/// </summary>
		/// <param name="array">array to copy to</param>
		/// <param name="index">offset into array</param>
		public void CopyTo(Array array, int index)
		{
			((ICollection)(processes.Values)).CopyTo(array, index);
		}

		#endregion

		/// <summary>
		/// Given a process id return the ProcessInfo Object
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public ProcessInfo GetProcess(int id)
		{
			if (processes.ContainsKey(id))
			{
				return processes[id];
			}
			else
			{
				Debug.WriteLine("Invalid process ID");
				return null;//invalid id
			}
		}


		/// <summary>
		/// Refreshes the processes hash which stores info on all managed process running
		/// </summary>
		public void RefreshProcessList()
		{
			processes.Clear();

			foreach (var process in Process.GetProcesses())
			{
				if (Process.GetCurrentProcess().Id == process.Id)
				{
					// let's hide our process
					continue;
				}

				// list the loaded runtimes in each process, if the ClrMetaHost APIs are available
				CLRMetaHost mh;
				try
				{
					mh = new CLRMetaHost();
				}
				catch (Exception)
				{
					continue;
				}

				IEnumerable<CLRRuntimeInfo> runtimes;
				try
				{
					runtimes = mh.EnumerateLoadedRuntimes(process.Id);
				}
				catch (Exception)
				{
					continue;
				}

				// TODO: only one CLR version for now...
				if (runtimes.Any())
				{
					var version = MdbgVersionPolicy.GetDefaultAttachVersion(process.Id);
					var debugger = new CorDebugger(version);
					processes[process.Id] = new ProcessInfo(process, debugger);
				}
			}
		}
	}
}
