using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualBasic.FileIO;

namespace dropfile
{
    class Program
    {

        const string PROGRAM = "dropfile.exe";

        static bool verbose = false;
        static bool test = false;

        static void usage()
        {
            Console.Write(
                 "usage: dropfile.exe [options] file1 file2 ...\n"
               + "       Move files to Dust Box.\n"
               + "file1,2... : moveed filename. it can use wildcard.\n"
               + "options    : -T ... Test mode\n"
               + "           : -V ... Verbose mode\n"
            );
        }
        static bool isWildcard(string glob)
        {
            if (glob.IndexOfAny(new char[] { '?', '*' }) >= 0)
            {
                return true;
            }
            return false;
        }
        static bool  drop(string file)
        {
            if (System.IO.File.Exists(file))
            {
                if (verbose)
                {
                    Console.WriteLine("dropfile: Move {0} to Dust Box", file);
                }
                if (!test)
                {
                    FileSystem.DeleteFile(file, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                }
                return true;
            }
            else
            {
                Console.Error.WriteLine("dropfile: File not exist {0}", file);
                return false;
            }
        }

        static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                usage();
                return 1;
            }
            foreach (string arg in args)
            {
 //               Console.WriteLine("arg:{0}",arg);
                if (arg.StartsWith("-"))
                {
                    switch(arg.Substring(1).ToUpper())
                    {
                        case "V":
                            verbose = true;
                            break;
                        case "T":
                            test = true;
                            break;
                        default:
                            Console.Error.WriteLine("dropfile: illegal option :{0}", arg);
                            return 1;
                    }
                    continue;
                }
                else
                {
                    string path = System.IO.Path.GetDirectoryName(arg);
                    if (path == "")
                    {
                        path = ".";
                    }
                    string fullpath = System.IO.Path.GetFullPath(path);
                    string glob = System.IO.Path.GetFileName(arg);

                    if (!isWildcard(glob))
                    {
                        string file = System.IO.Path.Combine(fullpath, glob);
                        drop(file);
                    }
                    else {
                        foreach (string file in System.IO.Directory.GetFiles(fullpath, glob))
                        {
                            drop(file);
                        }
                    }
                }
            }
            return 0;
        }

    }
}
