//---------------------------------------------------------------------
//  This file is part of the Managed Stack Explorer (MSE).
// 
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//---------------------------------------------------------------------

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security.Permissions;

//[assembly: SecurityPermission(SecurityAction.RequestMinimum)]
namespace Microsoft.Mse.Gui
{
	/// <summary>
	/// Gets the icon of an application from the shell
	/// Either a large 32 x 32 or small 16 x 16 icon
	/// </summary>
	/// 
	public abstract class ShellIcon
	{
		//members
		const uint SHGFI_ICON = 0x100;
		const uint SHGFI_LARGEICON = 0x0;
		const uint SHGFI_SMALLICON = 0x1;

		//types
		[StructLayout(LayoutKind.Sequential)]
		private struct ShellFileInfo
		{
			public IntPtr iconHandle;
			public IntPtr iconInterPtr;
			public uint dwAttributes;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			public string displayName;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
			public string typeName;
		};

		//methods

		// Ignore P/Invoke warning
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass"), DllImport("shell32.dll")]
		private static extern IntPtr SHGetFileInfo(string path, uint fileAttributes, ref ShellFileInfo fileInfo, uint fileInfoSize, uint flags);

		//destroy the handles since c# gc wont
		// Ignore P/Invoke warning
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass"), DllImport("User32.dll")]
		private static extern int DestroyIcon(System.IntPtr iconHandle);

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
		public static Icon GetIcon(string fileName, bool bigSize)
		{
			ShellFileInfo shellInfo = new ShellFileInfo();

			uint iconSize = SHGFI_SMALLICON;
			if (bigSize)
			{
				iconSize = SHGFI_LARGEICON;
			}

			SHGetFileInfo(fileName, 0, ref shellInfo, (uint)Marshal.SizeOf(shellInfo), SHGFI_ICON | iconSize);

			Icon copiedIcon = System.Drawing.Icon.FromHandle(shellInfo.iconHandle).Clone() as Icon;
			DestroyIcon(shellInfo.iconHandle);
			return copiedIcon;
		}
	}
}