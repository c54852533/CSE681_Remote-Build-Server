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
 * RepoMock()
 * void CopyDirectory(String sourcePath, String destinationPath)
 * 
 * Required Files:
 * ---------------
 * Repo.cs
 * 
 * Maintenance History:
 * --------------------
 * ver 1.0 : 05 Oct 2017
 * - first release
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Repo
{
    ///////////////////////////////////////////////////////////////////
    // RepoMock class
    // - begins to simulate basic Repo operations
    public class RepoMock
    {
        public string storagePath { get; set; } = "../../RepoStorage"; 
        public string receivePath { get; set; } = "../../BuilderStorage"; 
        public List<string> files { get; set; } = new List<string>();

        /*----< initialize RepoMock Storage>---------------------------*/

        public RepoMock()
        {
            if (!Directory.Exists(storagePath))
                Directory.CreateDirectory(storagePath);
            if (!Directory.Exists(receivePath))
                Directory.CreateDirectory(receivePath);
        }

        //next function is to copy all the three directories that contain three project in them from repostorage to buildstorage
        public void CopyDirectory(String sourcePath, String destinationPath)
        {
            DirectoryInfo info = new DirectoryInfo(sourcePath);
            Directory.CreateDirectory(destinationPath);
            foreach (FileSystemInfo fsi in info.GetFileSystemInfos())
            {
                String destName = Path.Combine(destinationPath, fsi.Name);

                if (fsi is System.IO.FileInfo)         //if it is a file, then copy it.

                    try
                    {
                        File.Copy(fsi.FullName, destName, true);
                        Console.Write("\n  sending \"{0}\" to \"{1}\"", fsi.FullName, destName);
                    }
                    catch (Exception ex)
                    {
                        Console.Write("\n--{0}--", ex.Message);
                    }
                else                                    //if it is a folder, then create the new folder in the target folder
                                                        //and then recursive it
                {
                    Directory.CreateDirectory(destName);
                    CopyDirectory(fsi.FullName, destName);
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.Write("\n  creating new folder \"{0}\" ", destName);
                    Console.Write("\n");
                    Console.ResetColor();

                }
            }
        }
        ///////////////////////////////////////////////////////////////////




    }
#if (TEST_REPOMOCK)

  ///////////////////////////////////////////////////////////////////
  // TestRepoMock class

  class TestRepoMock
  {
    static void Main(string[] args)
    {
      Console.Write("\n  Demonstration of Mock Repo");
      Console.Write("\n ============================");
      RepoMock repo = new RepoMock();
      repo.CopyDirectory(repo.storagePath, repo.receivePath);
      Console.Write("\n\n");
    }
  }
#endif
}

