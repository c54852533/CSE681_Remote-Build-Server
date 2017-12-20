/////////////////////////////////////////////////////////////////////
// Repo.cs - Demonstrate a few repo operations                     //
//                                                                 //
// Author: Hao Zhang      CSE681      hzhang98@syr.edu             //
// Source: Jim Fawcett  CSE681     Fall 2017                       //
// Application: Project Template                                   //
// Environment: C# console                                         //
/////////////////////////////////////////////////////////////////////
/*
 * public interface :
 * ===================
 * 
 * Required Files:
 * ---------------
 * Repo.cs
 * Builder.cs
 * DllLoaderDemo.cs
 * InterfaceLib.cs
 * TestedLib.cs
 * TestedLibDependency.cs
 * TesterLib.cs
 * MSBuild Environment
 * 
 * Maintenance History:
 * --------------------
 * ver 1.0 : 05 Oct 2017
 * - first release
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repo;
using System.IO;
using Builder;
using DllLoaderDemo;
namespace Execute
{

    public class Execute
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.BackgroundColor = ConsoleColor.White;
            Console.Write("\n  Demonstration of Mock Repo");
            Console.Write("\n ============================\n");
            Console.ResetColor();


            //fetch the to be built files in the repo to buildstorage
            RepoMock repo = new RepoMock();
            repo.CopyDirectory(repo.storagePath, repo.receivePath);
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.BackgroundColor = ConsoleColor.White;
            Console.Write("\nRepo Send build projects to build server");
            Console.ResetColor();


            Console.Write("\n\n");

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.BackgroundColor = ConsoleColor.White;
            Console.Write("\n  Building .csproj files ");
            Console.Write("\n =========================\n\n");
            Console.ResetColor();
            //starting building process and catch build exception
            BuilderDemo builder = new BuilderDemo();
            try
            {
                builder.BuildCsproj();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.BackgroundColor = ConsoleColor.White;
                Console.Write("\n\n  An error occured while trying to build the csproj file.\n  Details: {0}\n\n", ex.Message);
                Console.ResetColor();
            }



            Console.Write("\n  The TestHarness asked the BuildServer for the Dll files");

            DllLoaderExec testharness= new DllLoaderExec();

            //fetch the alreay built dll files to the testers
            testharness.getFiles("*.dll");
            foreach (string file in testharness.files)
                Console.Write("\n  \"{0}\"", file);
            foreach (string file in testharness.files)
            {
                string fileName = Path.GetFileName(file);
                Console.Write("\n  sending \"{0}\" to \"{1}\"", fileName, testharness.TestreceivePath);
                testharness.sendFile(file);
            }
            Console.Write("\n\n");

            Console.Write("\n  Demonstrating Robust Test Loader");
            Console.Write("\n ==================================\n");

            DllLoaderExec.testersLocation = Path.GetFullPath(DllLoaderExec.testersLocation);
            Console.Write("\n  Loading Test Modules from:\n    {0}\n", DllLoaderExec.testersLocation);

            // run load and tests

            string result = testharness.loadAndExerciseTesters();

            Console.Write("\n\n  {0}", result);
            Console.Write("\n\n");
        
    }
    }

}
