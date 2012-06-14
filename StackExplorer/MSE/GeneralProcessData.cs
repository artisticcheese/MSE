//---------------------------------------------------------------------
//  This file is part of the Managed Stack Explorer (MSE).
// 
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//---------------------------------------------------------------------

using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Globalization;

using Microsoft.Mse.Library;
using System.Security.Permissions;

//[assembly: SecurityPermission(SecurityAction.RequestMinimum)]
namespace Microsoft.Mse.Gui
{
	/// <summary>
	/// Class which coordinates performance counters and system.diagnostic data
	/// </summary>
	public class GeneralProcessData : IDisposable
	{

		//performance counters
		private PerformanceCounter usageCounter;
		private PerformanceCounter managedHeap;
		private PerformanceCounter gen0Collections;
		private PerformanceCounter gen1Collections;
		private PerformanceCounter gen2Collections;
		private PerformanceCounter percentInGC;
		private System.ComponentModel.ComponentResourceManager generalStrings;

		public delegate string GeneralInfoDellegate();//deleagte for any function which will return some generalProcessInfo data
		private Dictionary<string, GeneralInfoDellegate> allGeneralInfo;//maps name of info to the fucntions which gets the data for it

		private ProcessInfo currProcess;

		public GeneralProcessData()
		{
			try
			{
				Process p = Process.GetCurrentProcess();
				string thisProcessName = System.IO.Path.GetFileNameWithoutExtension(p.MainModule.FileName);
				usageCounter = new PerformanceCounter();
				usageCounter.CategoryName = "Process";
				usageCounter.CounterName = "% Processor Time";
				usageCounter.InstanceName = thisProcessName;
			}
			catch (InvalidOperationException) { }

			Thread perfInitializeThread = new Thread(new ThreadStart(InitializePerfCounters));
			perfInitializeThread.SetApartmentState(ApartmentState.STA);
			perfInitializeThread.Start();

			try
			{
				//managed heap size
				managedHeap = new PerformanceCounter();
				managedHeap.CategoryName = ".NET CLR Memory";
				managedHeap.CounterName = "# Bytes in all Heaps";
			}
			catch (InvalidOperationException) { }

			try
			{
				//generation 0 collecitons
				gen0Collections = new PerformanceCounter();
				gen0Collections.CategoryName = ".NET CLR Memory";
				gen0Collections.CounterName = "# Gen 0 Collections";
			}
			catch (InvalidOperationException) { }

			try
			{
				//generation 1 collecitons
				gen1Collections = new PerformanceCounter();
				gen1Collections.CategoryName = ".NET CLR Memory";
				gen1Collections.CounterName = "# Gen 1 Collections";
			}
			catch (InvalidOperationException) { }

			try
			{
				//generation 2 collecitons
				gen2Collections = new PerformanceCounter();
				gen2Collections.CategoryName = ".NET CLR Memory";
				gen2Collections.CounterName = "# Gen 2 Collections";
			}
			catch (InvalidOperationException) { }

			try
			{
				//percent time in GC
				percentInGC = new PerformanceCounter();
				percentInGC.CategoryName = ".NET CLR Memory";
				percentInGC.CounterName = "% Time in GC";
			}
			catch (InvalidOperationException) { }

			generalStrings = new System.ComponentModel.ComponentResourceManager(typeof(Properties.Resources));
			//set up what counter/data is availabe
			allGeneralInfo = new Dictionary<string, GeneralInfoDellegate>();
			allGeneralInfo[generalStrings.GetString("processDescription")] = new GeneralInfoDellegate(SelectedCompanyDescription);
			allGeneralInfo[generalStrings.GetString("processCompany")] = new GeneralInfoDellegate(SelectedCompanyName);
			allGeneralInfo[generalStrings.GetString("cpuUsage")] = new GeneralInfoDellegate(SelectedCpuUsage);
			allGeneralInfo[generalStrings.GetString("cpuTime")] = new GeneralInfoDellegate(SelectedCpuTime);
			allGeneralInfo[generalStrings.GetString("physicalMem")] = new GeneralInfoDellegate(SelectedPhysicalMemory);
			allGeneralInfo[generalStrings.GetString("virtualMem")] = new GeneralInfoDellegate(SelectedVirtualMemory);
			allGeneralInfo[generalStrings.GetString("managedHeap")] = new GeneralInfoDellegate(SelectedManagedHeap);
			allGeneralInfo[generalStrings.GetString("gen0Coll")] = new GeneralInfoDellegate(SelectedGeneration0Collections);
			allGeneralInfo[generalStrings.GetString("gen1Coll")] = new GeneralInfoDellegate(SelectedGeneration1Collections);
			allGeneralInfo[generalStrings.GetString("gen2Coll")] = new GeneralInfoDellegate(SelectedGeneration2Collections);
			allGeneralInfo[generalStrings.GetString("timeInGC")] = new GeneralInfoDellegate(SelectedPercentGC);
		}

		public Dictionary<string, GeneralInfoDellegate> ProcessData
		{
			get { return allGeneralInfo; }
		}

		public System.ComponentModel.ComponentResourceManager GeneralStrings
		{
			get { return generalStrings; }
		}

		/// <summary>
		/// UPdates the instance names and the processInfo object
		/// </summary>
		/// <param name="instanceName">The name of the process without the extension e.g. test and not text.exe</param>
		/// <param name="proc">A process info object</param>
		public void ChangeDataInstance(ProcessInfo proc)
		{
			if (proc == null)
			{
				throw new ArgumentNullException("proc");
			}

			try
			{
				string instanceName = System.IO.Path.GetFileNameWithoutExtension(proc.GeneralProcessInfo.MainModule.FileName);
				usageCounter.InstanceName = instanceName;
				managedHeap.InstanceName = instanceName;
				gen0Collections.InstanceName = instanceName;
				gen1Collections.InstanceName = instanceName;
				gen2Collections.InstanceName = instanceName;
				percentInGC.InstanceName = instanceName;
			}
			catch (InvalidOperationException) { }

			currProcess = proc;
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
		/// Dispose of the performance counters
		/// </summary>
		/// <param name="disposing">Dispose managed objects</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				// Free other state (managed objects).
				usageCounter.Dispose();
				managedHeap.Dispose();
				gen0Collections.Dispose();
				gen1Collections.Dispose();
				gen2Collections.Dispose();
				percentInGC.Dispose();
			}
			// Free your own state (unmanaged objects).


		}

		/// <summary>
		/// Finalizer
		/// </summary>
		~GeneralProcessData()
		{
			Dispose(false);

		}


		/// <summary>
		/// Thread which initializes performance counters
		/// this is done in a seperate thread when the program first starts to allow performance counters
		/// to run smoothly
		/// </summary>
		private void InitializePerfCounters()
		{
			try
			{
				usageCounter.NextValue();
			}
			catch (InvalidOperationException) { }
		}


		// The General info function, each fucntion represent a different peice of data about the process

		/// <summary>
		/// Get the size of the physically memory used by the process
		/// </summary>
		/// <returns></returns>
		private string SelectedPhysicalMemory()
		{
			Process genInfo = currProcess.GeneralProcessInfo;
			long physicalMemory = genInfo.WorkingSet64 / 1024;
			return physicalMemory.ToString(CultureInfo.CurrentCulture.NumberFormat) + "k";
		}

		/// <summary>
		/// Get the size of the virtual memory used by the process
		/// </summary>
		/// <returns></returns>
		private string SelectedVirtualMemory()
		{
			Process genInfo = currProcess.GeneralProcessInfo;
			long virtualMemory = genInfo.VirtualMemorySize64 / 1024;
			return virtualMemory.ToString(CultureInfo.CurrentCulture.NumberFormat) + "k";
		}

		/// <summary>
		/// Get the total time spent in the cpu
		/// </summary>
		/// <returns></returns>
		private string SelectedCpuTime()
		{
			Process genInfo = currProcess.GeneralProcessInfo;
			return String.Format(CultureInfo.CurrentCulture.NumberFormat, "{0}:{1}:{2}", genInfo.TotalProcessorTime.Hours,
								genInfo.TotalProcessorTime.Minutes,
								genInfo.TotalProcessorTime.Seconds);
		}

		/// <summary>
		/// Get the comapny name from the process executable file
		/// </summary>
		/// <returns></returns>
		private string SelectedCompanyName()
		{
			Process genInfo = currProcess.GeneralProcessInfo;
			return genInfo.MainModule.FileVersionInfo.CompanyName;
		}

		/// <summary>
		/// Get the descirption from the process executable file
		/// </summary>
		/// <returns></returns>
		private string SelectedCompanyDescription()
		{
			Process genInfo = currProcess.GeneralProcessInfo;
			return genInfo.MainModule.FileVersionInfo.FileDescription;
		}

		/// <summary>
		/// Get the size of the managed heap
		/// </summary>
		/// <returns></returns>
		private string SelectedManagedHeap()
		{
			try
			{
				return String.Format(CultureInfo.CurrentCulture.NumberFormat, "{0}k", (int)(managedHeap.NextValue() / 1024));
			}
			catch (InvalidOperationException)
			{
				return "";
			}
		}

		/// <summary>
		/// Get the cpu usage percentage
		/// </summary>
		/// <returns></returns>
		private string SelectedCpuUsage()
		{
			try
			{
				return String.Format(CultureInfo.CurrentCulture.NumberFormat, "{0:##0.##}%", usageCounter.NextValue() / Environment.ProcessorCount);
			}
			catch (InvalidOperationException)
			{
				return "";
			}
		}

		/// <summary>
		/// Get the number of generation 0 collections
		/// </summary>
		/// <returns></returns>
		private string SelectedGeneration0Collections()
		{
			try
			{
				return gen0Collections.NextValue().ToString(CultureInfo.CurrentCulture.NumberFormat);
			}
			catch (InvalidOperationException)
			{
				return "";
			}
		}

		/// <summary>
		/// Get the number of generation 1 collections
		/// </summary>
		/// <returns></returns>
		private string SelectedGeneration1Collections()
		{
			try
			{
				return gen1Collections.NextValue().ToString(CultureInfo.CurrentCulture.NumberFormat);
			}
			catch (InvalidOperationException)
			{
				return "";
			}
		}

		/// <summary>
		/// Get the number of generation 2 collections
		/// </summary>
		/// <returns></returns>
		private string SelectedGeneration2Collections()
		{
			try
			{
				return gen2Collections.NextValue().ToString(CultureInfo.CurrentCulture.NumberFormat);
			}
			catch (InvalidOperationException)
			{
				return "";
			}
		}

		/// <summary>
		/// Get the time spent in the garbage collector
		/// </summary>
		/// <returns></returns>
		private string SelectedPercentGC()
		{
			try
			{
				return String.Format(CultureInfo.CurrentCulture.NumberFormat, "{0:##0.#0}%", percentInGC.NextValue());
			}
			catch (InvalidOperationException)
			{
				return "";
			}
		}

	}
}