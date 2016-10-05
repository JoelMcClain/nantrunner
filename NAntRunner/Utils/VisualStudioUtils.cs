﻿// The MIT License(MIT)
// 
// Copyright(c) <year> <copyright holders>
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using EnvDTE;
using EnvDTE80;
using System.Linq;
using NAntRunner.Common;

namespace NAntRunner.Utils
{
    public static class VisualStudioUtils
    {
        public static Solution2 GetSolution(DTE2 applicationObject)
        {
            Solution2 solution = applicationObject.Solution as Solution2;
            
            if (solution.IsOpen)
                return applicationObject.Solution as Solution2;

            return null;
        }
        
        /// <summary>
        /// Open a file in the VisualStudio editor.
        /// </summary>
        public static void ShowFile(DTE2 applicationObject, string filename)
        {
            applicationObject.ItemOperations.OpenFile(filename, EnvDTE.Constants.vsViewKindAny);
        }

        public static void ShowWindow(DTE2 applicationObject, string windowTitle)
        {
            if (windowTitle == "Output")
            {
                applicationObject.Windows.Item(Constants.vsWindowKindOutput).Activate();

                var window = applicationObject.ToolWindows.OutputWindow;
                var windowPane = window.OutputWindowPanes.OfType<OutputWindowPane>().FirstOrDefault(x => x.Name == AppConstants.NAntRunner);

                if (windowPane != null)
                    windowPane.Activate();
            }
            else
                applicationObject.Windows.Item(windowTitle).Activate();
        }

        public static void ShowDocument(DTE2 applicationObject, string documentTitle)
        {
            applicationObject.Documents.Item(documentTitle).Activate();
        }
        
        /// <summary>
        /// Activate the console.
        /// </summary>
        /// <param name="applicationObject">The application object for the output window.</param>
        public static void ShowErrors(DTE2 applicationObject)
        {
            applicationObject.ToolWindows.ErrorList.Parent.Activate();
        }

        /// <summary>
        /// Show the editor with specified file and select specified line.
        /// The file must be opened.
        /// </summary>
        /// <param name="applicationObject">The application objectname of the addin.</param>
        /// <param name="filename">The line to show.</param>
        /// <param name="lineNumber">The file name.</param>
        /// <param name="selectLine">Select the line.</param>
        public static void ShowLine(DTE2 applicationObject, string filename, int lineNumber, bool selectLine)
        {
            if (lineNumber > 0)
            {
                try
                {
                    // Retrieve the document
                    Document document = applicationObject.Documents.Item(filename);

                    // Select the node's line
                    TextSelection selection = (TextSelection)document.Selection;
                    selection.GotoLine(lineNumber, selectLine);
                }
                catch (ArgumentException)
                {
                    // Do nothing
                }
            }
        }

        public static OutputWindowPane GetConsole(DTE2 applicationObject, string title)
        {
            OutputWindow outputWindow = applicationObject.ToolWindows.OutputWindow;

            // Scan all output panes
            foreach (OutputWindowPane outputPane in outputWindow.OutputWindowPanes)
            {
                if (outputPane.Name == title)
                    return outputPane;
            }

            // If the pane is not found, create a new one
            return outputWindow.OutputWindowPanes.Add(title);
        }

        /// <summary>
        /// Return the solution configuration mode (Debug, Release...)
        /// </summary>
        public static string GetSolutionConfiguration(DTE2 applicationObject)
        {
            string configurationMode = null;

            if (applicationObject.Solution != null
                && applicationObject.Solution.SolutionBuild != null
                && applicationObject.Solution.SolutionBuild.ActiveConfiguration != null)
            {
                configurationMode = applicationObject.Solution.SolutionBuild.ActiveConfiguration.Name;
            }

            return configurationMode;
        }
    }
}
