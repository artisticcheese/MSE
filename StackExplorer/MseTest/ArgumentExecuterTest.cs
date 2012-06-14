//---------------------------------------------------------------------
//  This file is part of the Managed Stack Explorer (MSE).
// 
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//---------------------------------------------------------------------

using System.Collections.Generic;
using System;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Mse.Gui;
using Microsoft.Mse.Library;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using NUnit.Framework;

namespace mseTest
{
	/// <summary>
	///This is a test class for Microsoft.Mse.Gui.ArgumentExecuter and is intended
	///to contain all Microsoft.Mse.Gui.ArgumentExecuter Unit Tests
	///</summary>

    [TestFixture]
	public class ArgumentExecuterTest
	{
		/// <summary>
		///Initialize() is called _once_ during test execution before
		///test methods in this test class are executed.
		///</summary>
        [TestFixtureSetUp]
		public void ClassInitialize()
		{
            TestEnvironment.SetUpTestEnvironment();
            ProcessInfoCollection.SingleProcessCollection.RefreshProcessList();
		}

		/// <summary>
		///Cleanup() is called once during test execution after
		///test methods in this class have executed unless
		///this test class' Initialize() method throws an exception.
		///</summary>
        [TestFixtureTearDown]
        public void ClassCleanup()
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
		///A test case for ExecuteArguments ()
		///</summary>
        [Test]
        public void ExecuteArgumentsTest()
        {
            int procID = TestEnvironment.testProcessInfo.ProcessId;
            string fileName = Path.GetTempFileName();
            string[] args = new string[] { "/p", procID.ToString(), "/s", "/o", fileName };

            ArgumentExecuter target = new ArgumentExecuter(args);
            int expected = 0;
     
           
            int actual = target.ExecuteArguments();
            bool success = false;
            using (StreamReader fileReader = new StreamReader(new FileStream(fileName, FileMode.Open)))
            {
                string stackString = fileReader.ReadToEnd();
                success = TestEnvironment.MatchedPredictedStackTrace(stackString);
            }
            try
            {
                File.Delete(fileName);
            }
            catch (FileNotFoundException ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            if ((expected != actual) || !success)
            {
                Assert.Fail("Microsoft.Mse.Gui.ArgumentExecuter.ExecuteArguments failed.");
            }
        }

        /// <summary>
        /// Tests whether passing an invalid process id (0) gracefully terminates MSE (Bug 592)
        /// </summary>
        [Test]
        public void InvalidProcessIdTest()
        {
            ArgumentExecuter target = new ArgumentExecuter(new string[] { "/p", "0" });
            int retValue = target.ExecuteArguments();
            if (retValue != 1)
            {
                Assert.Fail("Invalid process Id test failed for process ID 0!");
            }
        }

        /// <summary>
        /// Tests whether passing invalid file name to /o option crashes mse (Bug 601)
        /// </summary>
        [Test]
        public void InvalidOutFileTest()
        {
            // Pass in an invalid file name and check that IOException is thrown
            ArgumentExecuter target = new ArgumentExecuter(new string[] {"/p", "/o", "???.txt"});

            try
            {
                target.ExecuteArguments();
                Assert.Fail("Invalid out file test failed for ???.txt!");
            }
            catch (IOException)
            {
                // Success
            }
        }

        /// <summary>
        /// Tests whether MSE crashes when trying to use a directory for /o (Bug 601)
        /// </summary>
        [Test]
        public void DirectoryAsOutFileTest()
        {
            // Create a temp directory and try to pass it to mse
            string tempName = Path.GetTempFileName();
            File.Delete(tempName);

            tempName += "d";
            Directory.CreateDirectory(tempName);
            ArgumentExecuter target = new ArgumentExecuter(new string[] { "/p", "/o", tempName });
            try
            {
                target.ExecuteArguments();
                Assert.Fail("Invalid out file test failed when using a directory for /o!");
            }
            catch (IOException)
            {
                // Success
            }
            finally
            {
                Directory.Delete(tempName);
            }
        }
	}
}
