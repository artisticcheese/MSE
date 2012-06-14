//---------------------------------------------------------------------
//  This file is part of the Managed Stack Explorer (MSE).
// 
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Text;
using System.Resources;
using System.Threading;
using System.Reflection;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Diagnostics.SymbolStore;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

using Microsoft.Mse.Gui.Properties;

using Microsoft.Samples.Debugging.CorDebug;
using Microsoft.Samples.Debugging.CorPublish;
using Microsoft.Samples.Debugging.CorMetadata;
using Microsoft.Samples.Debugging.CorSymbolStore;
using Microsoft.Samples.Debugging.CorDebug.NativeApi;
using System.Security.Permissions;

//[assembly: SecurityPermission(SecurityAction.RequestMinimum)]
namespace Microsoft.Mse.Library
{
	/// <summary>
	/// Gives a plethora of information regarding a specific frame on the stack
	/// A frame on the stack refers to the specific function call on the stack
	/// FrameInfo will contain info about the function such as its name and line number
	/// </summary>
	public class FrameInfo : IDisposable
	{
		//private members

		//the information about this function
		private string functionFullName;
		private string functionShortName;
		private string moduleShortName;
		private string moduleFullName;
		private string functionFileName;
		private int functionLineNumber;


		private CorFrame thisFrame;
		private CorMetadataImport metaImporter;


		//fields dealing with position in the source of the module
		// as of now I am having this info retrived in this class incase the .pdb file
		// changes during the programs life, it will be more efficient to get this info once in the stacktrace class
		//then check if the pdb changed the update it
		private ISymbolDocument[] symDocs;
		private int[] offsets;
		private int[] startLines;
		private int[] endLines;
		private int[] startColumns;
		private int[] endColumns;




		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="frame">The CorFrame frame object</param>
		/// <param name="importer">The meta data importer for the module this frame is contained within</param>
		internal FrameInfo(CorFrame frame, CorMetadataImport importer)
		{

			if (frame == null)
			{
				throw new ArgumentNullException("frame");
			}

			if (importer == null)
			{
				throw new ArgumentNullException("importer");
			}

			thisFrame = frame;
			metaImporter = importer;
			functionShortName = "";
			functionFullName = "";
			moduleShortName = "";
			moduleFullName = "";
			functionFileName = "";
			functionLineNumber = -1;//-1 is set by deafult which means no line number

			SourcePosition functionPos = null; //position in this function where we are

			//make binder and reader for the metadata

			if (thisFrame.FrameType == CorFrameType.InternalFrame)
			{
				functionFullName = functionShortName = InternalFrameToString();
			}
			else
			{
				functionPos = GetMetaDataInfo(importer);
			}

			if (functionPos != null)
			{
				functionLineNumber = functionPos.Line;
				functionFileName = Path.GetFileName(functionPos.Path);
			}
			else
			{
				ResourceManager stackStrings = new ResourceManager(typeof(Resources));
				functionLineNumber = -1;//no line number available
				functionFileName = stackStrings.GetString("sourceUnavailable");
			}

		}


		//properties

		/// <summary>
		/// The full path name of the function
		/// </summary>
		public string FunctionFullName
		{
			get { return functionFullName; }
		}

		/// <summary>
		/// Just the function name and now path info along with it 
		/// </summary>
		public string FunctionShortName
		{
			get { return functionShortName; }
		}

		/// <summary>
		/// Just the Module name no path with it
		/// </summary>
		public string ModuleShortName
		{
			get { return moduleShortName; }
		}

		/// <summary>
		/// Full module name with path
		/// </summary>
		public string ModuleFullName
		{
			get { return moduleFullName; }
		}

		/// <summary>
		/// Name of the file the function is in
		/// </summary>
		public string FunctionFileName
		{
			get { return functionFileName; }
		}

		/// <summary>
		/// Line number in file which the function is on
		/// </summary>
		public int FunctionLineNumber
		{
			get { return functionLineNumber; }
		}




		//methods

		/// <summary>
		/// Dispose pattern
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Dispose managed and unmanaged objects
		/// </summary>
		/// <param name="disposing">Dispose managed objects</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				// Free other state (managed objects).
				metaImporter = null;
				symDocs = null;
				offsets = null;
				startLines = null;
				endLines = null;
				startColumns = null;
				endColumns = null;
			}
			// Free your own state (unmanaged objects).
		}

		/// <summary>
		/// Read the pdb file for this module and frame
		/// Retrieve infomation about the function
		/// </summary>
		/// <remarks>
		/// When an unmanaged app like reflector loads CLR, "Function.Module.Name"
		/// doesn't return a valid value and so this function returns null.
		/// </remarks>
		/// <returns>SourcePosition of the function</returns>
		private SourcePosition GetMetaDataInfo(CorMetadataImport importer)
		{
			SourcePosition functionPos = null; //position in this function where we are
			try
			{
				moduleFullName = thisFrame.Function.Module.Name;
				moduleShortName = System.IO.Path.GetFileName(moduleFullName);
			}
			catch (ArgumentException)
			{
				moduleFullName = "";
				moduleShortName = "";
				return null;
			}


			//TODO: Implement a better method to determine the symbol path than just assuming it's in the same
			//      directory
			string sympath = ".";

			//dealing with readinf the source in the module
			ISymbolReader metaReader = null;
			ISymbolBinder1 symbolBinder = new SymbolBinder();

			try
			{
				if (moduleFullName.Length > 0)
				{
					metaReader = (symbolBinder as ISymbolBinder2).
						GetReaderForFile(importer.RawCOMObject, moduleFullName, sympath);
				}
			}
			catch (COMException)
			{
				//Debug.WriteLine(ed.ToString(CultureInfo.CurrentCulture.NumberFormat));
				//will get here for any function which we cant read the .pdb file for
				//its not a big deal we just wont have source and line info
			}

			if (metaReader != null)
			{
				ISymbolMethod symMethod = null;
				try
				{
					symMethod = metaReader.GetMethod(new SymbolToken((int)thisFrame.Function.Token), thisFrame.Function.Version);
					int sequenceCount = symMethod.SequencePointCount;
					symDocs = new ISymbolDocument[sequenceCount];
					offsets = new int[sequenceCount];
					startLines = new int[sequenceCount];
					startColumns = new int[sequenceCount];
					endLines = new int[sequenceCount];
					endColumns = new int[sequenceCount];

					//Get the sequence points and store them in the apporpriate arrays. Seqeunce points
					//represent the different points in the files which correlate to il instruction and lines
					symMethod.GetSequencePoints(offsets, symDocs, startLines, startColumns, endLines, endColumns);

					functionPos = GetSourcePositionFromFrame();
				}
				catch (COMException)
				{
					functionPos = null;
				}
				finally
				{
					symDocs = null;
					symMethod = null;
				}
			}


			CorType ctype = GetClassType();
			if (ctype != null)
			{
				StringBuilder sb = new StringBuilder();
				GetFunctionClassPath(sb, ctype);
				functionFullName = sb.ToString();
			}
			else
			{
				functionFullName = "";
			}

			MethodInfo methIn = importer.GetMethodInfo(thisFrame.Function.Token);
			functionFullName += "." + methIn.Name;
			functionShortName = methIn.Name;
			return functionPos;

		}


		/// <summary>
		/// Gets the source position from a given Instruction Pointer
		/// </summary>
		/// <param name="ip">The Instruction Pointer.</param>
		/// <returns>The Source Position.</returns>
		private SourcePosition GetSourcePositionFromIP(int ip)
		{
			//EnsureIsUpToDate();
			//if (!m_haveSymbols)
			//return null;
			const int SpecialSequencePoint = 0xfeefee;
			int seqCount = offsets.Length;
			if ((seqCount > 0) && (offsets[0] <= ip))
			{
				int i;
				for (i = 0; i < seqCount; ++i)
				{
					if (offsets[i] >= ip)
					{
						break;
					}
				}
				if (i == seqCount || offsets[i] != ip)
				{
					--i;
				}

				SourcePosition sp = null;

				if (startLines[i] == SpecialSequencePoint)
				{
					int j = i;
					// let's try to find a sequence point that is not special somewhere earlier in the code
					// stream.
					while (j > 0)
					{
						--j;
						if (startLines[j] != SpecialSequencePoint)
						{
							sp = new SourcePosition(false, symDocs[j].URL, startLines[j],
								endLines[j], startColumns[j], endColumns[j]);
							break;
						}
					}

					if (sp == null)
					{
						// we didn't find any non-special seqeunce point before current one, let's try to search
						// after.
						j = i;
						while (++j < seqCount)
						{
							if (startLines[j] != SpecialSequencePoint)
							{
								sp = new SourcePosition(false, symDocs[j].URL, startLines[j],
									endLines[j], startColumns[j], endColumns[j]);
								break;
							}
						}
					}
					// The following Assert somewhow is not always true for w3wp.exe (ASP.Net) process
					// Debug.Assert(sp != null, "Only SpecialSequence point detected");
				}
				else
				{
					sp = new SourcePosition(false, symDocs[i].URL, startLines[i],
						endLines[i], startColumns[i], endColumns[i]);

				}

				//<strip>@TODO ENC HACK diasymreader</strip>
				//if (CorFunction.Version != 1) // function has been edited
				//sp.m_fixedFile = Module.GetEditsSourceFile(CorFunction.Version - 1);

				return sp;
			}
			return null;
		}


		/// <summary>
		/// Gets the SourcePosition from a given CorFrame.
		/// </summary>
		/// <param name="frame">The CorFrame.</param>
		/// <returns>The SourcePosition of the given frame.</returns>
		private SourcePosition GetSourcePositionFromFrame()
		{
			uint ip;
			CorDebugMappingResult mappingResult;
			thisFrame.GetIP(out ip, out mappingResult);

			// MAPPING_APPROXIMATE, MAPPING_EXACT, MAPPING_PROLOG, or MAPPING_EPILOG are all ok and we should show sources.
			// But these two indicate that something went wrong and nothing is available.
			if (mappingResult == CorDebugMappingResult.MAPPING_NO_INFO ||
				mappingResult == CorDebugMappingResult.MAPPING_UNMAPPED_ADDRESS)
				return null;

			return GetSourcePositionFromIP((int)ip);
		}



		/// <summary>
		/// Get a CorType that function for this frame is declared in.
		/// </summary>
		/// <returns>The cortype of the function</returns>
		private CorType GetClassType()
		{

			CorClass c = thisFrame.Function.Class;

			if (TokenUtils.RidFromToken(c.Token) != 1 && TokenUtils.RidFromToken(c.Token) != 0)
			{

				// ICorDebug API lets us always pass ET_Class
				CorElementType et = CorElementType.ELEMENT_TYPE_CLASS;
				// Get type parameters.
				IEnumerable typars = thisFrame.TypeParameters;
				//IEnumerator tyenum = typars.GetEnumerator();
				int cNumTyParamsOnClass = metaImporter.CountGenericParams(c.Token);
				CorType[] types = new CorType[cNumTyParamsOnClass];
				int it = 0;
				foreach (CorType arg in typars)
				{
					if (it == cNumTyParamsOnClass)
					{
						break;
					}
					types[it] = arg;
					it++;
				}
				return c.GetParameterizedType(et, types);
			}
			else return null;
		}



		/// <summary>
		/// Gets the full class path of the function
		/// </summary>
		/// <param name="sb">string builder to be used to create full class path</param>
		/// <param name="ct">The cortype of the function</param>

		// Ignore the complexity warning - don't want to rewrite it now.
		private void GetFunctionClassPath(StringBuilder sb, CorType ct)
		{
			//StringBuilder sb, CorType ct, CorMetadataImport importer
			switch (ct.Type)
			{
				case CorElementType.ELEMENT_TYPE_CLASS:
				case CorElementType.ELEMENT_TYPE_VALUETYPE:
					// We need to get the name from the metadata. We can get a cached metadata importer
					// from a MDbgModule, or we could get a new one from the CorModule directly.
					// Is this hash lookup to get a MDbgModule cheaper than just re-querying for the importer?
					CorClass cc = ct.Class;
					Type tn = metaImporter.GetType(cc.Token);

					sb.Append(tn.FullName);
					//AddGenericArgs(sb, proc, ct.TypeParameters);
					return;

				// Primitives
				case CorElementType.ELEMENT_TYPE_BOOLEAN:
					sb.Append("System.Boolean"); return;
				case CorElementType.ELEMENT_TYPE_CHAR:
					sb.Append("System.Char"); return;
				case CorElementType.ELEMENT_TYPE_I1:
					sb.Append("System.SByte"); return;
				case CorElementType.ELEMENT_TYPE_U1:
					sb.Append("System.Byte"); return;
				case CorElementType.ELEMENT_TYPE_I2:
					sb.Append("System.Int16"); return;
				case CorElementType.ELEMENT_TYPE_U2:
					sb.Append("System.UInt16"); return;
				case CorElementType.ELEMENT_TYPE_I4:
					sb.Append("System.Int32"); return;
				case CorElementType.ELEMENT_TYPE_U4:
					sb.Append("System.Uint32"); return;
				case CorElementType.ELEMENT_TYPE_I8:
					sb.Append("System.Int64"); return;
				case CorElementType.ELEMENT_TYPE_U8:
					sb.Append("System.UInt64"); return;
				case CorElementType.ELEMENT_TYPE_I:
					sb.Append("System.IntPtr"); return;
				case CorElementType.ELEMENT_TYPE_U:
					sb.Append("System.UIntPtr"); return;
				case CorElementType.ELEMENT_TYPE_R4:
					sb.Append("System.Single"); return;
				case CorElementType.ELEMENT_TYPE_R8:
					sb.Append("System.Double"); return;

				// Well known class-types.
				case CorElementType.ELEMENT_TYPE_OBJECT:
					sb.Append("System.Object"); return;
				case CorElementType.ELEMENT_TYPE_STRING:
					sb.Append("System.String"); return;


				// Special compound types. Based off first type-param
				case CorElementType.ELEMENT_TYPE_SZARRAY:
				case CorElementType.ELEMENT_TYPE_ARRAY:
				case CorElementType.ELEMENT_TYPE_BYREF:
				case CorElementType.ELEMENT_TYPE_PTR:
					CorType t = ct.FirstTypeParameter;
					GetFunctionClassPath(sb, t);
					switch (ct.Type)
					{
						case CorElementType.ELEMENT_TYPE_SZARRAY:
							sb.Append("[]");
							return;
						case CorElementType.ELEMENT_TYPE_ARRAY:
							int rank = ct.Rank;
							sb.Append('[');
							for (int i = 0; i < rank - 1; i++)
							{
								// <strip>@todo- could we print out exact boundaries?
								// Probably would have to do some sort of func-eval on the array object.</strip>
								sb.Append(',');
							}
							sb.Append(']');
							return;
						case CorElementType.ELEMENT_TYPE_BYREF:
							sb.Append("&");
							return;
						case CorElementType.ELEMENT_TYPE_PTR:
							sb.Append("*");
							return;
					}
					Debug.Assert(false); // shouldn't have gotten here.             
					return;
				// <strip>Other wacky stuff.
				// @todo - can we do a better job of printing these?</strip>
				case CorElementType.ELEMENT_TYPE_FNPTR:
					sb.Append("*(...)");
					return;
				case CorElementType.ELEMENT_TYPE_TYPEDBYREF:
					sb.Append("typedbyref");
					return;
				default:
					sb.Append("<unknown>");
					return;
			}
		}

		/// <summary>
		/// Builds a string representation of an internal frame
		/// With info like what type of internal frame it is
		/// </summary>
		/// <returns>string representation of internal frame</returns>
		private string InternalFrameToString()
		{
			Debug.Assert(thisFrame.FrameType == CorFrameType.InternalFrame);
			StringBuilder sb = new StringBuilder();
			sb.Append("[Internal thisFrame, ");
			switch (thisFrame.InternalFrameType)
			{
				case CorDebugInternalFrameType.STUBFRAME_M2U:
					sb.Append("'M-->U', ");
					break;

				case CorDebugInternalFrameType.STUBFRAME_U2M:
					sb.Append("'U-->M', ");
					break;

				case CorDebugInternalFrameType.STUBFRAME_APPDOMAIN_TRANSITION:
					sb.Append("'AD switch':(AD '");
					CorFrame c = thisFrame.Caller;
					if (c != null)
						sb.Append(c.Function.Module.Assembly.AppDomain.Name);
					sb.Append("'. #");
					if (c != null)
						sb.Append(c.Function.Module.Assembly.AppDomain.Id);
					sb.Append(") -->(AD '");
					c = thisFrame.Callee;
					if (c != null)
						sb.Append(c.Function.Module.Assembly.AppDomain.Name);
					sb.Append("'. #");
					if (c != null)
						sb.Append(c.Function.Module.Assembly.AppDomain.Id);
					sb.Append(")]");
					return sb.ToString();

				case CorDebugInternalFrameType.STUBFRAME_LIGHTWEIGHT_FUNCTION:
					sb.Append("'Dynamic Method', ");
					break;

				case CorDebugInternalFrameType.STUBFRAME_FUNC_EVAL:
					sb.Append("'FuncEval', ");
					break;

				case CorDebugInternalFrameType.STUBFRAME_NONE:
				default:
					sb.Append("'Unknown Type', ");
					break;
			}

			Type t = null;
			try
			{
				MethodInfo methin = metaImporter.GetMethodInfo(thisFrame.Function.Token);
				t = methin.DeclaringType;

				sb.Append((t == null ? "<module>" : t.FullName))
					.Append("::")
					.Append((methin == null) ? "<unknown function>" : methin.Name);
			}
			catch (COMException ex)
			{
				if (ex.ErrorCode != (int)HResult.CORDBG_E_CODE_NOT_AVAILABLE)
					throw;
				sb.Append(String.Format(CultureInfo.CurrentCulture.NumberFormat, "(no function, hr=0x{0:x})", ex.ErrorCode));
			}

			sb.Append("]");
			return sb.ToString();
		}

	}
}
