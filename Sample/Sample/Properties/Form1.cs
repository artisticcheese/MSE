//---------------------------------------------------------------------
//  This file is part of the Managed Stack Explorer (MSE).
// 
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//---------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace Microsoft.Mse.Sample
{
	
	public partial class Form1 : Form
	{
		private Random r;
		private Queue<Thread> threadQueue;

		~Form1()
		{
		}
		public Form1()
		{
			InitializeComponent();
			r = new Random();
			threadQueue = new Queue<Thread>();
		}

		private void recu1(){
			while (true)
			{
				int d = r.Next() % 5 + 2;
				if (d == 2) recu2();
				else if (d == 3) recu3();
				else if (d == 4) recu4();
				else if (d == 5) recu5();
				else if (d == 6) recu6();

			}
		}
		private void recu2()
		{
			int d = r.Next() % 7 + 0;
				if (d == 2) recu2();
			else if (d == 3) recu3();
			else if (d == 4) recu4();
			else if (d == 5) recu5();
			else if (d == 6) recu6();
		}
		private void recu3()
		{
			int d = r.Next() % 7 + 0;
			if (d == 2) recu2();
			else if (d == 3) recu3();
			else if (d == 4) recu4();
			else if (d == 5) recu5();
			else if (d == 6) recu6();
		}
		private void recu4()
		{
			int d = r.Next() % 7 + 0;
			if (d == 2) recu2();
			else if (d == 3) recu3();
			else if (d == 4) recu4();
			else if (d == 5) recu5();
			else if (d == 6) recu6();
		}
		private void recu5()
		{
			int d = r.Next() % 7 + 0;
			if (d == 2) recu2();
			else if (d == 3) recu3();
			else if (d == 4) recu4();
			else if (d == 5) recu5();
			else if (d == 6) recu6();
		}
		private void recu6()
		{
			int d = r.Next() % 7 + 0;
			if (d == 2) recu2();
			else if (d == 3) recu3();
			else if (d == 4) recu4();
			else if (d == 5) recu5();
			else if (d == 6) recu6();
		}

		void Form1_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
		{
			while(threadQueue.Count > 0){
				Thread t = threadQueue.Dequeue();
				t.Abort();
				t = null;
			}
		}

		private void Form1_Load(object sender, EventArgs e)
		{

		}
		private void Form1_Shown(object sender, EventArgs e)
		{
			
		}

		private void threadFunction()
		{
			while (true)
			{
				Thread.Sleep(1);
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			Thread t = new Thread(new ThreadStart(threadFunction));
			t.SetApartmentState(ApartmentState.STA);
			t.Start();
			threadQueue.Enqueue(t);
			threadCount.Text = (int.Parse(threadCount.Text) + 1).ToString();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			if (threadQueue.Count > 0)
			{
				Thread t = threadQueue.Dequeue();
				t.Abort();
				t = null;
				threadCount.Text = (int.Parse(threadCount.Text) - 1).ToString();
			}
		}

	}
}

