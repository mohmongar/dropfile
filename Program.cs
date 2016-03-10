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
        static bool ignoreError = false;
        /// <summary>
        /// output usage to console
        /// </summary>
        static void usage()
        {
            Console.Write(
                 "usage: dropfile.exe [options] file1 file2 ...\n"
               + "       Move files to Dust Box.\n"
               + "file1,2... : moveed filename. it can use wildcard.\n"
               + "options    : -T ... Test mode\n"
               + "           : -V ... Verbose mode\n"
               + "           : -I ... Ignore error occurs\n"
            );
        }
        /// <summary>
        /// Check whether exist wild card character in filename string
        /// </summary>
        /// <param name="glob">string : filename </param>
        /// <returns>bool : exist or nothing</returns>
        static bool isWildcard(string glob)
        {
            if (glob.IndexOfAny(new char[] { '?', '*' }) >= 0)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// drop function  
        /// </summary>
        /// <param name="file">string : filename path</param>
        /// <returns>bool : success or fail</returns>
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
        /// <summary>
        /// main routine to analyze command line strings
        /// </summary>
        static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                usage();
                return 1;
            }
            foreach (string arg in args)
            {
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
                        case "I":
                            ignoreError = true;
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
                        if (drop(file) == false && ignoreError == false)
                        {
                            return 1;
                        }
                    }
                    else
                    {
                        foreach (string file in System.IO.Directory.GetFiles(fullpath, glob))
                        {
                            if (drop(file) == false && ignoreError == false){
                                return 1;
                            }
                        }
                    }
                }
            }
            return 0;
        }

    }
}
