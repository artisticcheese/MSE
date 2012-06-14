//---------------------------------------------------------------------
//  This file is part of the Managed Stack Explorer (MSE).
// 
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//---------------------------------------------------------------------

using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;
using Microsoft.Mse.Library;
using Microsoft.Samples.Debugging.CorDebug;
using System;
using NUnit.Framework;

namespace mseTest
{
	/// <summary>
	/// Summary description for ProcessInfoTest
	/// </summary>
    [TestFixture]
	public class ProcessInfoTest
	{
		public ProcessInfoTest()
		{
		}

		/// <summary>
		/// Initialize() is called once during test execution before
		/// test methods in this test class are executed.
		/// </summary>
        [TestFixtureSetUp]
		public void Initialize()
		{
            TestEnvironment.SetUpTestEnvironment();
		}

		/// <summary>
		/// Cleanup() is called once during test execution after
		/// test methods in this class have executed unless the
		/// corresponding Initialize() call threw an exception.
		/// </summary>
        [TestFixtureTearDown]
		public void Cleanup()
		{
			try
			{
				TestEnvironment.testProcess.Kill();
			}
			catch (System.InvalidOperationException e)
			{
				Console.Error.WriteLine(e.ToString());
			}
		}


		/// <summary>
		/// A test case for RefreshProcessInfo ()
		/// </summary>
        [Test]
		public void RefreshProcessInfoTest()
		{
			ProcessInfo target = TestEnvironment.testProcessInfo;
			target.RefreshProcessInfo();

			Assert.AreEqual(target.ThreadInfos.Count, TestEnvironment.testStackTotalThreads, "Microsoft.Mse.Library.ProcessInfo.RefreshProcessInfo failed");

		}

		/// <summary>
		/// A test case for GetDisplayStackTrace (List&lt;int&gt;, int)
		/// </summary>
        [Test]
		public void GetDisplayStackTraceTest()
		{

			ProcessInfo target = TestEnvironment.testProcessInfo;
			List<int> threadSelectedIdList = null;
			int depth = 0;

			List<string> actual = new List<string>(target.GetDisplayStackTrace(threadSelectedIdList, depth));

			string stackString = "";
			foreach (string line in actual)
			{
				stackString += line;
			}

			if (!TestEnvironment.MatchedPredictedStackTrace(stackString))
			{
				Assert.Fail("Microsoft.Mse.Library.ProcessInfo.GetDisplayStackTrace did not return the expected value.");
			}
		}

		/// <summary>
		/// A test case for UpdateAllStackTraces (int)
		/// </summary>
        [Test]
		public void UpdateAllStackTracesTest()
		{
			ProcessInfo target = TestEnvironment.testProcessInfo;
			int depth = 0; // TODO: Initialize to an appropriate value
			target.UpdateAllStackTraces(depth);
			int totalFrames = 0;
			foreach(ThreadInfo tInfo in target.ThreadInfos.Values)
			{
				totalFrames += tInfo.FrameStack.Count;
			}

			if ((target.ThreadInfos.Count != TestEnvironment.testStackTotalThreads) || 
                ((totalFrames != TestEnvironment.testStackTotalFramesDebug) && (totalFrames != TestEnvironment.testStackTotalFramesRelease)))
			{
				Assert.Fail("Microsoft.Mse.Library.ProcessInfo.UpdateAllStackTraces failed");
			}
		}


		/// <summary>
		/// A test case for UpdateSelectedThreadStacks (List&lt;int&gt;, int)
		/// </summary>
        [Test]
		public void UpdateSelectedThreadStacksTest()
		{
			ProcessInfo target = TestEnvironment.testProcessInfo;
			List<int> threadIds = null; // should chose all threads
			int depth = 0;
			target.UpdateSelectedThreadStacks(threadIds, depth);

			int totalFrames = 0;
			foreach (ThreadInfo tInfo in target.ThreadInfos.Values)
			{
				totalFrames += tInfo.FrameStack.Count;
			}

			if ((target.ThreadInfos.Count != TestEnvironment.testStackTotalThreads) || 
                ((totalFrames != TestEnvironment.testStackTotalFramesDebug) && (totalFrames != TestEnvironment.testStackTotalFramesRelease)))

			{
				Assert.Fail("Microsoft.Mse.Library.ProcessInfo.UpdateSelectedThreadStacksTest failed");
			}
		}
	}
}
