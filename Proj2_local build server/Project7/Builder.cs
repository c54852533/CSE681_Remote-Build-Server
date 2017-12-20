/////////////////////////////////////////////////////////////////////
// Builder.cs - Demonstrate builder operations                     //
//                                                                 //
// Author: Hao Zhang      CSE681      hzhang98@syr.edu             //
// Source: Ammar Salman  CSE681     Fall 2017                      //
// Application: Project Template                                   //
// Environment: C# console                                         //
/////////////////////////////////////////////////////////////////////
/*
 * public interface :
 * ===================
 * void BuildCsproj()
 * 
 * Required Files:
 * ---------------
 * Repo.cs
 * Builder.cs
 * MSBuild Environment
 * 
 * Maintenance History:
 * --------------------
 * ver 1.0 : 05 Oct 2017
 * - first release
 */

using System;
using System.Collections.Generic;

using Microsoft.Build.BuildEngine;
using Microsoft.Build.Framework;
using Microsoft.Build.Execution;
using System.IO;

namespace Builder
{
    public class BuilderDemo
    {
        /* 
         * This method uses MSBuild to build a .csproj file.
         * The csproj file is configured to build as Debug/AnyCPU
         * Therefore, there is no need to specify the parameters here.
         * This is useful for the build server because it should be as
         * general as it can get. The build server shouldn't have to
         * specify different build parameters for each project. 
         * Instead, the csproj file sets the configuration settings.
         * 
         * In the csproj file, the OutputPath is set to "csproj_Debug" 
         * for the Debug configuration, and "csproj_Release" for the
         * Release configuration. Moreover, if Debug was selected, the
         * project will be build into an x86 library (DLL), while if Release
         * was selected, the project will build into an x64 executable (EXE)
         * 
         * To change the default configuration, the first PropertyGroup
         * in the ..\..\..\files\Builder.csproj must be modified.
         */
        public void BuildCsproj()
        {

            string projectFileName = @"..\..\..\Execute\BuilderStorage";
            projectFileName = Path.GetFullPath(projectFileName);
            var Builderfolders = Directory.EnumerateFiles(projectFileName, "*.csproj", SearchOption.AllDirectories);
            //get all the .csproj files that conclude build information in them, and then build each of them
            foreach( string file in Builderfolders)
            {
                ConsoleLogger logger = new ConsoleLogger();

                Dictionary<string, string> GlobalProperty = new Dictionary<string, string>();
                BuildRequestData BuildRequest = new BuildRequestData(file, GlobalProperty, null, new string[] { "Rebuild" }, null);
                BuildParameters bp = new BuildParameters();
                bp.Loggers = new List<ILogger> { logger };

                BuildResult buildResult = BuildManager.DefaultBuildManager.Build(bp, BuildRequest);

                Console.WriteLine();
            }

        }

    }

#if (TEST_Builder)

  ///////////////////////////////////////////////////////////////////
  // TestBuilder class

    class TestBuilder
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.BackgroundColor = ConsoleColor.White;
            Console.Write("\n  Building Builder.csproj ");
            Console.Write("\n =========================\n\n");
            Console.ResetColor();
            BuilderDemo build= new BuilderDemo();
            try
            {
                build.BuildCsproj();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.BackgroundColor = ConsoleColor.White;
                Console.Write("\n\n  An error occured while trying to build the csproj file.\n  Details: {0}\n\n", ex.Message);
                Console.ResetColor();
            }
        }
    }
#endif
}
