//---------------------------------------------------------------------
//  This file is part of the Managed Stack Explorer (MSE).
// 
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Collections;
using System;
using Microsoft.Mse.Library;
using NUnit.Framework;

namespace mseTest
{
	/// <summary>
	///This is a test class for Microsoft.Mse.Library.ProcessInfoCollection and is intended
	///to contain all Microsoft.Mse.Library.ProcessInfoCollection Unit Tests
	///</summary>
    [TestFixture]
	public class ProcessInfoCollectionTest
	{
		/// <summary>
		///Initialize() is called once during test execution before
		///test methods in this test class are executed.
		///</summary>
        [TestFixtureSetUp]
		public void Initialize()
		{
			TestEnvironment.SetUpTestEnvironment();
		}

		/// <summary>
		///Cleanup() is called once during test execution after
		///test methods in this class have executed unless
		///this test class' Initialize() method throws an exception.
		///</summary>
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
		///A test case for GetEnumerator ()
		///</summary>
        [Test]
		public void GetEnumeratorTest()
		{
			ProcessInfoCollection target = ProcessInfoCollection.SingleProcessCollection;
			IEnumerator expected = null;
			IEnumerator actual;

			actual = target.GetEnumerator();

			Assert.AreNotEqual(expected, actual, "Microsoft.Mse.Library.ProcessInfoCollection.GetEnumerator did not return the expe" +
					"cted value.");
		}

		/// <summary>
		///A test case for GetProcess (int)
		///</summary>
        [Test]
		public void GetProcessTest()
		{
			ProcessInfoCollection target = ProcessInfoCollection.SingleProcessCollection;
			target.RefreshProcessList();
			ProcessInfo expected = TestEnvironment.testProcessInfo;
			ProcessInfo actual = target.GetProcess(expected.ProcessId);

			Assert.AreEqual(expected.ProcessId, actual.ProcessId, "Microsoft.Mse.Library.ProcessInfoCollection.GetProcess did not return the expecte" +
					"d value.");
		}

		/// <summary>
		///A test case for GetProcessInfoCollection ()
		///</summary>
        [Test]
		public void GetProcessInfoCollectionTest()
		{
			ProcessInfoCollection expected = null;
			ProcessInfoCollection actual;

			actual = ProcessInfoCollection.SingleProcessCollection;

			Assert.AreNotEqual(expected, actual, "Microsoft.Mse.Library.ProcessInfoCollection.GetProcessInfoCollection did not return the expected value.");
		}


		/// <summary>
		///A test case for RefreshProcessList ()
		///</summary>
        [Test]
		public void RefreshProcessListTest()
		{
			ProcessInfoCollection target = ProcessInfoCollection.SingleProcessCollection;
			target.RefreshProcessList();
			if (target.ProccessHash.Count <= 0)
			{
				Assert.Fail("Microsoft.Mse.Library.ProcessInfoCollection.RefreshProcessListTest failed.");
			}
		}



		/// <summary>
		///A test case for this[int value]
		///</summary>
        [Test]
		public void ItemTest()
		{
			ProcessInfoCollection target = ProcessInfoCollection.SingleProcessCollection;
			target.RefreshProcessList();
			ProcessInfo val = TestEnvironment.testProcessInfo;
			int value = val.ProcessId;
			Assert.AreEqual(target[value].ProcessId, value, "Microsoft.Mse.Library.ProcessInfoCollection.ItemTest did not return the expected value.");

		}

	}


}
