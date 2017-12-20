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
using System.Reflection;
using System.IO;
using System.Collections.Generic;

namespace DllLoaderDemo
{
  public class DllLoaderExec
  {
    public static string testersLocation { get; set; } = ".";
    public string TeststoragePath { get; set; } = "../../BuilderStorage";
    public string TestreceivePath { get; set; } = "../../Testers";
    public List<string> files { get; set; } = new List<string>();

    /*----< demonstrate method for finding files and copy files >-----------*/
    /*
    *  getFilesHelper() helps to search and find all the files in
    *  a certain file with a certain pattern
    */
        private void getFilesHelper(string path, string pattern)
    {
      string[] tempFiles = Directory.GetFiles(path, pattern);
      for(int i=0; i<tempFiles.Length; ++i)
      {
        tempFiles[i] = Path.GetFullPath(tempFiles[i]);
      }
      files.AddRange(tempFiles);

      string[] dirs = Directory.GetDirectories(path);
      foreach (string dir in dirs)
      {
        getFilesHelper(dir, pattern);
      }
    }
    /*----< find all the files in BuilderStorage >-----------*/
    /*
    *  Finds all the files, matching pattern, in the entire 
    *  directory tree rooted at BuilderStorage.
    */
        public void getFiles(string pattern)
    {
      files.Clear();
      getFilesHelper(TeststoragePath, pattern);
    }
        /*----< send the file in BuilderStorage >-----------*/
        /*
        *  send the dll files to the Testers folder
        */
        public bool sendFile(string fileSpec)
    {
      try
      {
        string fileName = Path.GetFileName(fileSpec);
        string destSpec = Path.Combine(TestreceivePath, fileName);
        File.Copy(fileSpec, destSpec, true);
        return true;
      }
      catch(Exception ex)
      {
        Console.Write("\n--{0}--", ex.Message);
        return false;
      }
    }
    /*----< library binding error event handler >------------------*/
    /*
     *  This function is an event handler for binding errors when
     *  loading libraries.  These occur when a loaded library has
     *  dependent libraries that are not located in the directory
     *  where the Executable is running.
     */
    static Assembly LoadFromComponentLibFolder(object sender, ResolveEventArgs args)
    {
      Console.Write("\n  called binding error event handler");
      string folderPath = testersLocation;
      string assemblyPath = Path.Combine(folderPath, new AssemblyName(args.Name).Name + ".dll");
      if (!File.Exists(assemblyPath)) return null;
      Assembly assembly = Assembly.LoadFrom(assemblyPath);
      return assembly;
    }
    //----< load assemblies from testersLocation and run their tests >-----

    public string loadAndExerciseTesters()
    {
      AppDomain currentDomain = AppDomain.CurrentDomain;
      currentDomain.AssemblyResolve += new ResolveEventHandler(LoadFromComponentLibFolder);

      try
      {
        DllLoaderExec loader = new DllLoaderExec();

        // load each assembly found in testersLocation

        string[] files = Directory.GetFiles(testersLocation,"*.dll");
        foreach(string file in files)
        {
          //Assembly asm = Assembly.LoadFrom(file);
          Assembly asm = Assembly.LoadFile(file);
          string fileName = Path.GetFileName(file);
          Console.Write("\n  loaded {0}",fileName);

          // exercise each tester found in assembly

          Type[] types = asm.GetTypes();
          foreach(Type t in types)
          {
            // if type supports ITest interface then run test

            if(t.GetInterface("DllLoaderDemo.ITest",true) != null)
              if(!loader.runSimulatedTest(t,asm))
                Console.Write("\n  test {0} failed to run",t.ToString());
          }
        }
      }
      catch(Exception ex)
      {
        return ex.Message;
      }
      return "Simulated Testing completed";
    }
    //
    //----< run tester t from assembly asm >-------------------------------

    bool runSimulatedTest(Type t, Assembly asm)
    {
      try
      {
        Console.Write(
          "\n  attempting to create instance of {0}",t.ToString()
          );
        object obj = asm.CreateInstance(t.ToString());

        // announce test

        MethodInfo method = t.GetMethod("say");
        if(method != null)
          method.Invoke(obj,new object[0]);

        // run test

        bool status = false;
        method = t.GetMethod("test");
        if(method != null)
          status = (bool)method.Invoke(obj,new object[0]);

        Func<bool, string> act = (bool pass) =>
        {
          if (pass)
            return "passed";
          return "failed";
        };
        Console.Write("\n  test {0}", act(status));
      }
      catch(Exception ex)
      {
        Console.Write("\n  test failed with message \"{0}\"", ex.Message);
        return false;
      }
            
     return true;
    }
    //
    //----< extract name of current directory without its parents ---------

    public string GuessTestersParentDir()
    {
      string dir = Directory.GetCurrentDirectory();
      int pos = dir.LastIndexOf(Path.DirectorySeparatorChar);
      string name = dir.Remove(0,pos+1).ToLower();
      if(name == "debug")
        return "../..";
      else
        return ".";
    }
        //----< run demonstration >--------------------------------------------


    }
#if (TEST_DllLoader)

  ///////////////////////////////////////////////////////////////////
  // TestDllLoader class

  class TestDllLoader
  {
    static void Main(string[] args)
    {
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
#endif
}
