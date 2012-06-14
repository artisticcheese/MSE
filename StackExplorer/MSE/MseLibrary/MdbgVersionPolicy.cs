namespace Microsoft.Mse.Library
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Runtime.InteropServices;
	using System.Text;
	using Microsoft.Samples.Debugging.CorDebug;
	using Microsoft.Samples.Debugging.CorPublish;

	/// <summary>
	/// A default implementation of run/attach versioning policy
	/// </summary>
	public static class MdbgVersionPolicy
	{
		/// <summary>
		/// Given the full path to a binary, determines a default CLR runtime version which
		/// should be used to debug it.
		/// </summary>
		/// <param name="filePath">The full path to a binary.</param>
		/// <returns>The CLR version string to use for debugging; null if unknown.</returns>
		public static string GetDefaultLaunchVersion(string filePath)
		{
			string version = GetDefaultRuntimeForFile(filePath);
			if (version != null)
				return version;

			// If the binary doesn't bind to a clr then just debug it with the same
			// runtime this debugger is running in (a very arbitrary choice)
			return RuntimeEnvironment.GetSystemVersion();
		}

		/// <summary>
		/// Given the full path to a binary, finds the CLR runtime version which
		/// it will bind to.
		/// </summary>
		/// <param name="filePath">The full path to a binary.</param>
		/// <returns>The version string that the binary will bind to; null if unknown.</returns>
		/// <remarks>If ICLRMetaHostPolicy can be asked, it is used. Otherwise
		/// fall back to mscoree!GetRequestedRuntimeVersion.</remarks>
		public static String GetDefaultRuntimeForFile(String filePath)
		{
			String version = null;

			CLRMetaHostPolicy policy;
			try
			{
				policy = new CLRMetaHostPolicy();
			}
			catch (NotImplementedException)
			{
				policy = null;
			}
			catch (System.EntryPointNotFoundException)
			{
				policy = null;
			}

			if (policy != null)
			{
				// v4 codepath
				StringBuilder ver = new StringBuilder();
				StringBuilder imageVer = new StringBuilder();
				CLRRuntimeInfo rti = null;

				String configPath = null;
				if (System.IO.File.Exists(filePath + ".config"))
				{
					configPath = filePath + ".config";
				}

				try
				{
					rti = policy.GetRequestedRuntime(CLRMetaHostPolicy.MetaHostPolicyFlags.metaHostPolicyHighCompat,
																	filePath,
																	configPath,
																	ref ver,
																	ref imageVer);
				}
				catch (System.Runtime.InteropServices.COMException)
				{
					Debug.Assert(rti == null);
				}

				if (rti != null)
				{
					version = rti.GetVersionString();
				}
				else
				{
					version = null;
				}
			}
			else
			{
				// v2 codepath
				try
				{
					version = CorDebugger.GetDebuggerVersionFromFile(filePath);
				}
				catch (System.Runtime.InteropServices.COMException)
				{
					// we could not retrieve dee version. 
					// Leave version null;
					Debug.Assert(version == null);
				}
			}

			return version;
		}

		/// <summary>
		/// Returns the version of the runtime to debug in a process
		/// we are attaching to, assuming we can only pick one
		/// </summary>
		/// <param name="processId">The process to attach to</param>
		/// <returns>The version of the runtime to debug, or null if the policy can't decide</returns>
		public static string GetDefaultAttachVersion(int processId)
		{
			try
			{
				CLRMetaHost mh = new CLRMetaHost();
				List<CLRRuntimeInfo> runtimes = new List<CLRRuntimeInfo>(mh.EnumerateLoadedRuntimes(processId));
				if (runtimes.Count > 1)
				{
					// It is ambiguous so just give up
					return null;
				}
				else if (runtimes.Count == 1)
				{
					return runtimes[0].GetVersionString();
				}
			}
			catch (EntryPointNotFoundException)
			{
				try
				{
					return CorDebugger.GetDebuggerVersionFromPid(processId);
				}
				catch (COMException) { }
			}

			// if we have neither failed nor detected a version at this point then there was no
			// CLR loaded. Now we try to determine what CLR the process will load by examining its
			// binary
			string binaryPath = GetBinaryPathFromPid(processId);
			if (binaryPath != null)
			{
				string version = GetDefaultRuntimeForFile(binaryPath);
				if (version != null)
				{
					return version;
				}
			}

			// and if that doesn't work, return the version of the CLR the debugger is
			// running against (a very arbitrary choice)
			return Environment.Version.ToString();
		}

		/// <summary>
		/// Attempts to retrieve the path to the binary from a running process
		/// Returns null on failure
		/// </summary>
		/// <param name="processId">The process to get the binary for</param>
		/// <returns>The path to the primary executable or null if it could not be determined</returns>
		private static string GetBinaryPathFromPid(int processId)
		{
			string programBinary = null;
			try
			{
				CorPublish cp = new CorPublish();
				CorPublishProcess cpp = cp.GetProcess(processId);
				programBinary = cpp.DisplayName;
			}
			catch
			{
				// try an alternate method
				using (ProcessSafeHandle ph = NativeMethods.OpenProcess(
						  (int)(NativeMethods.ProcessAccessOptions.ProcessVMRead |
								  NativeMethods.ProcessAccessOptions.ProcessQueryInformation |
								  NativeMethods.ProcessAccessOptions.ProcessDupHandle |
								  NativeMethods.ProcessAccessOptions.Synchronize),
						  false, // inherit handle
						  processId))
				{
					if (!ph.IsInvalid)
					{
						StringBuilder sb = new StringBuilder(NativeMethods.MAX_PATH);
						int neededSize = sb.Capacity;
						NativeMethods.QueryFullProcessImageName(ph, 0, sb, ref neededSize);
						programBinary = sb.ToString();
					}
				}
			}

			return programBinary;
		}
	}
}
