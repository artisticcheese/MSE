//---------------------------------------------------------------------
//  This file is part of the Managed Stack Explorer (MSE).
// 
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//---------------------------------------------------------------------

using System.Diagnostics;
using System.Security.Permissions;

//[assembly: SecurityPermission(SecurityAction.RequestMinimum)]
namespace Microsoft.Mse.Library
{

	/// <summary>
	/// Represents info about a specifc point in a function
	/// </summary>
	public class SourcePosition
	{

		//private members
		//internal string fixedFile = null; //<strip>@TODO ENC HACK diasymreader</strip> saves the ENC file name
		internal string path;
		private bool isSpecial;
		private int startLine;
		private int endLine;
		private int startColumn;
		private int endColumn;

		//constructors
		/// <summary>
		/// Contructor of SourcePosition type.
		/// </summary>
		/// <param name="isSpecial">Indicates if this source position is a special position.</param>
		/// <param name="path">Path for the source file.</param>
		/// <param name="startLine">Start line of the location in the source file.</param>  
		/// <param name="endLine">Final line of the location in the source file.</param>                
		/// <param name="startColumn">Start column of the location in the source file.</param>          
		/// <param name="endColumn">Endcolumn of the location in the source file.</param>                  
		public SourcePosition(bool special,
									string pa, int startL, int endL, int startC, int endC)
		{
			// special sequence points are handled elsewhere.

			path = pa;
			startLine = startL;
			endLine = endL;
			startColumn = startC;
			endColumn = endC;
			isSpecial = special;
		}

		//properties

		/// <summary>
		/// Gets if this source position is a special position.
		/// </summary>
		/// <value>true if special, else false.</value>
		public bool IsSpecial
		{
			get
			{
				return isSpecial;
			}
		}

		/// <summary>
		/// Same as StartLine.
		/// </summary>
		/// <value>StartLine.</value>
		public int Line
		{
			get
			{
				return startLine;
			}
		}

		/// <summary>
		/// Gets the start line of the location in the source file.
		/// </summary>
		/// <value>The start line.</value>
		public int StartLine
		{
			get
			{
				return startLine;
			}
		}

		/// <summary>
		/// Gets the start column of the location in the source file.
		/// </summary>
		/// <value>The start column.</value>
		public int StartColumn
		{
			get
			{
				return startColumn;
			}
		}

		/// <summary>
		/// Gets the final line of the location in the source file.
		/// </summary>
		/// <value>The final line.</value>
		public int EndLine
		{
			get
			{
				return endLine;
			}
		}

		/// <summary>
		/// Gets the end column of the location in the source file.
		/// </summary>
		/// <value>The End Column.</value>
		public int EndColumn
		{
			get
			{
				return endColumn;
			}
		}

		/// <summary>
		/// Gets the Path for the source file.
		/// </summary>
		/// <value>The Path.</value>
		public string Path
		{
			get
			{
				//if (fixedFile != null)     //<strip>@TODO ENC HACK</strip> .pdb currently doesn't support
				//having new version of function in different file, therefore
				//we have to pass in the file name manually.
				//return fixedFile;

				return path;
			}
		}
	}
}