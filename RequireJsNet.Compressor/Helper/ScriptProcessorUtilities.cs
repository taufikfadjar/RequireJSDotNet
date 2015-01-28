// RequireJS.NET
// Copyright VeriTech.io
// http://veritech.io
// Dual licensed under the MIT and GPL licenses:
// http://www.opensource.org/licenses/mit-license.php
// http://www.gnu.org/licenses/gpl.html

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Jint.Parser.Ast;
using RequireJsNet.Compressor.Parsing;


namespace RequireJsNet.Compressor.Helper
{
	internal static class ScriptProcessorUtilities
	{
		
		public static void EnsureHasRange(SyntaxNode node, List<ScriptLine> lineList)
		{
			if (node == null || node.Range != null)
			{
				return;
			}

			var location = node.Location;
			var startLine = lineList[location.Start.Line - 1];
			var endLine = lineList[location.End.Line - 1];
			var startingIndex = startLine.StartingIndex + location.Start.Column;
			var endIndex = endLine.StartingIndex + location.End.Column;
			node.Range = new[] { startingIndex, endIndex };
		}

		public static void EnsureHasRange(RequireCall call, List<ScriptLine> lineList)
		{
			EnsureHasRange(call.ParentNode.Node, lineList);
			EnsureHasRange(call.DependencyArrayNode, lineList);
			EnsureHasRange(call.ModuleDefinitionNode, lineList);
			EnsureHasRange(call.ModuleIdentifierNode, lineList);
			EnsureHasRange(call.SingleDependencyNode, lineList);
			var arguments = call.ParentNode.Node.As<CallExpression>().Arguments;
			foreach (var arg in arguments)
			{
				EnsureHasRange(arg, lineList);
			}
		}

		public static List<ScriptLine> GetScriptLines(string scriptText)
		{
			var currentLineBuilder = new StringBuilder();
			var result = new List<ScriptLine>();
			var currentLine = new ScriptLine();

			if (scriptText.Length > 0)
			{
				result.Add(currentLine);
			}

			for (var i = 0; i < scriptText.Length; i++)
			{
				var currChar = scriptText[i];
				var separatorLength = 1;

				// this is either a legacy maCOs newline, or it will be followed by \n for the windows one
				// we could also be at the last character of the file when that isn't a newline
				if ((currChar == '\r' || currChar == '\n') || i == scriptText.Length - 1)
				{
					if (currChar != '\r' && currChar != '\n')
					{
						currentLineBuilder.Append(scriptText[i]); 
					}

					if (currChar == '\r' && i < scriptText.Length - 1 && scriptText[i + 1] == '\n')
					{
						// skip the next character since it's part of an \r\n sequence
						i++;
						separatorLength = 2;
					}

					var prevIndex = 0;
					var prevCount = 0;
					var prevSeparator = 0;

					// if we're not still processing the first line, look at the prev line's starting index
					if (result.Count > 1)
					{
						var rpev = result[result.Count - 2];
						prevIndex = rpev.StartingIndex;
						prevCount = rpev.LineText.Length;
						prevSeparator = rpev.NewLineLength;
					}

					currentLine.LineText = currentLineBuilder.ToString();
					currentLine.StartingIndex = prevIndex + prevCount + prevSeparator;
					currentLine.NewLineLength = separatorLength;
					currentLine = new ScriptLine();
					result.Add(currentLine);
					currentLineBuilder.Clear();
				}
				else
				{
					currentLineBuilder.Append(scriptText[i]);
				}
			}

			if (currentLine.LineText == null && currentLine.StartingIndex == 0)
			{
				result.Remove(currentLine);
			}

			return result;
		}
	}
}
