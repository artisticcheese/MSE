//---------------------------------------------------------------------
//  This file is part of the Managed Stack Explorer (MSE).
// 
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//---------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;

using Microsoft.Mse.Library;
using Microsoft.Samples.Debugging.CorDebug;
using Microsoft.Samples.Debugging.CorMetadata;
using Microsoft.Samples.Debugging.CorDebug.NativeApi;
using Microsoft.Samples.Debugging.CorPublish;
using Microsoft.Samples.Debugging.CorSymbolStore;

using UnitTestHelper;

public class TestEnvironment
{
    public static readonly string path = Directory.GetCurrentDirectory();
    public static readonly string testProgram = Path.Combine(path, "UnitTestHelper.exe");
    public static readonly string testFile = Path.Combine(path, "semFile.sem");
    public static CorDebugger debugger = null; //corDebugger object which will managed the processes
    public static ProcessInfo testProcessInfo = null;
    public static Process testProcess = null;

    //NOTE: These variables are hardcoded based on the test program

    //the regex to match the specific stack UnitTestHelper.exe will generate 
    public static string[] testStackRegex = new string[3];
    //the total number of frames in all threads for UnitTestHelper.exe debug mode
    public static int testStackTotalFramesDebug = 8;
    //the total number of frames in all threads for UnitTestHelper.exe release mode
    public static int testStackTotalFramesRelease = 6;
    //the total number of threads for UnitTestHelper.exe
    public static int testStackTotalThreads = 3;

    // dummy variable to force the dependency on UnitTestHelper.exe
    private static UnitTestHelper.Program program = new Program();

    public static void SetUpTestEnvironment()
    {
        if (testProcess == null || testProcessInfo == null || debugger == null || testProcess.HasExited)
        {
            Console.WriteLine("Starting the unit test helper from " + testProgram + " ...");
            testProcess = Process.Start(testProgram, "");

            //wait for program to finish initializing
            while (true)
            {
                try
                {
                    if (File.Exists(testFile))
                    {
                        debugger = new CorDebugger(CorDebugger.GetDefaultDebuggerVersion());
                        testProcessInfo = new ProcessInfo(testProcess, debugger);
                        File.Delete(testFile);
                        break;
                    }
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e.ToString());
                    throw;
                }
            }
        }
    }

    /// <summary>
    /// This matches a generated stack trace with a HARDCODED!!!!! predetermined one
    /// This test harness runs an app I made that has a consitent stack trace that I can test however
    /// if the test program is change or the stack format of mseLibrary is change then this won't work!!!!
    /// Remmember that
    /// </summary>
    /// <param name="stackTrace"></param>
    /// <returns></returns>
    public static bool MatchedPredictedStackTrace(string stackString)
    {
        // Make sure there are 3 "Thread ID:" and 2 "UnitTestHelper.Program.aFunc" in the stack trace
        string stringToMatch1 = @".*Thread ID:\s*\d+.*Thread ID:\s*\d+.*Thread ID:\s*\d+.*";
        string stringToMatch2 = @".*UnitTestHelper\.Program\.aFunc.*UnitTestHelper\.Program\.aFunc.*";

        string stackStringToMatch = stackString.Replace("\r\n", " ");
        stackStringToMatch = stackStringToMatch.Replace('\n', ' ');

        bool success = new Regex(stringToMatch1, RegexOptions.Multiline).Match(stackStringToMatch).Success;
        success = success && (new Regex(stringToMatch2, RegexOptions.Multiline).Match(stackStringToMatch).Success);
        return success;
    }
}