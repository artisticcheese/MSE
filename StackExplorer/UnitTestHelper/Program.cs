//---------------------------------------------------------------------
//  This file is part of the Managed Stack Explorer (MSE).
// 
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;

namespace UnitTestHelper
{
    public class Program
    {
        public Program()
        {
        }

        static void Main(string[] args)
        {
            // create a semphore file so that the unit tests can start
            string file = Path.Combine(Directory.GetCurrentDirectory(), "semFile.sem");
            
            try
            {
                File.Delete(file);
                new FileStream(file, FileMode.CreateNew).Close();
            }
            catch (IOException)
            {
            }

            // Create a thread with the infinite loop function
            Thread a = new Thread(new ThreadStart(aFunc));
            a.Start();

            // Go into an infinite loop
            aFunc();
        }

        static void aFunc()
        {
            while (true)
            {
                Thread.Sleep(System.Threading.Timeout.Infinite);
            }
        }
    }
}