/*
 * HorizonOS
 * (c) Openlight, 2021
 * Modification of this code is authorized
 * as long as the final product is open-sourced
 * and this notice remains in the final product.
 */

using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace entendreOS
{
    class Program
    {
        static void Main(string[] args)
        {

            // HorizonOS differentiates between debug mode and the actual operating system.

            if (System.Diagnostics.Debugger.IsAttached)
            {
                Console.Title = "Horizon Emulator (DEBUG)";
            }
            else
            {
                Console.Title = "HorizonOS";
            }

            Console.ForegroundColor = ConsoleColor.White;
            Thread.Sleep(1000); // Thread is put to sleep for 1 second in order to keep reasonable loading times and reassure the user
            Horizon_Logon(); // The OS now starts. Logon function is initiated.
        }

        static void Separator()
        {
            // This is just for visualization
            Console.WriteLine("--------------------");
        }

        static void Horizon_Logon()
        {
            // System runs through registry checks
            Console.WriteLine("Looking for your accounts...");

            if(! Properties.reg64.Default.isOOBEdone)
            {
                // If registry flag isOOBEdone is false, launch Out Of Box Experience.
                Horizon_OOBE();
            }
            else if (File.Exists("globalreg/autologin.horizonconf"))
            {
                // If autologin enabled, start main process.
                Horizon();
            }
            else
            {
                // Login process
                Horizon_Login();
            }
        }

        static void Horizon_OOBE()
        {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.Clear();
            Console.WriteLine("Welcome to Horizon!");
            Console.WriteLine("To get started, you need to create a profile.");

            Separator();
            Console.Write("Choose a fancy username: ");
            string username = Console.ReadLine();

            Console.Write("Choose a secure password: ");
            string password = ReadPassword();

            Console.Clear();
            Console.Write("Hi, "+username);
            // globalreg created
            Directory.CreateDirectory("globalreg");
            // username property set to username
            Properties.reg64.Default.username = username;
            // Creates user config file
            File.Delete(@"globalreg/username.horizonconf");
            // Appends username to said config file
            File.AppendAllText(@"globalreg/username.horizonconf", username);
            // password property set to password
            Properties.reg64.Default.password = password;
            // OOBE marked as done
            Properties.reg64.Default.isOOBEdone = true;
            // Saves all properties to registry
            Properties.reg64.Default.Save();
            Thread.Sleep(1000);
            Horizon_Logon();
        }

        public static string ReadPassword()
        {
            // Function to mask password entry in terminal
            string password = "";
            ConsoleKeyInfo info = Console.ReadKey(true);
            while (info.Key != ConsoleKey.Enter)
            {
                if (info.Key != ConsoleKey.Backspace)
                {
                    Console.Write("*");
                    password += info.KeyChar;
                }
                else if (info.Key == ConsoleKey.Backspace)
                {
                    if (!string.IsNullOrEmpty(password))
                    {
                        // remove one character from the list of password characters
                        password = password.Substring(0, password.Length - 1);
                        // get the location of the cursor
                        int pos = Console.CursorLeft;
                        // move the cursor to the left by one character
                        Console.SetCursorPosition(pos - 1, Console.CursorTop);
                        // replace it with space
                        Console.Write(" ");
                        // move the cursor to the left by one character again
                        Console.SetCursorPosition(pos - 1, Console.CursorTop);
                    }
                }
                info = Console.ReadKey(true);
            }
            // add a new line because user pressed enter at the end of their password
            Console.WriteLine();
            return password;
        }

        static void Horizon_Login()
        {
            // Background colour set to black because OOBE sets colour to blue
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();

            Console.WriteLine("Horizon 20.07 beta clui1");
            Console.WriteLine("  ");

            Console.Write("horizon login: ");
            string loginu = Console.ReadLine();

            Thread.Sleep(1000);

            if (loginu != Properties.reg64.Default.username)
            {
                Console.WriteLine("Unknown user: " + loginu);
                Thread.Sleep(1000);
                Horizon_Login();
            }

            Console.Write("password for "+loginu+": ");
            string loginp = ReadPassword();

            Console.WriteLine("  ");
            Thread.Sleep(1000);

            if (loginp != Properties.reg64.Default.password)
            {
                Console.WriteLine("Sorry... Try again...");
                Thread.Sleep(1000);
                Horizon_Login();
            }
            else if (loginp == Properties.reg64.Default.password)
            {
                Thread.Sleep(1000);
                Horizon();
            }
        }
        static void Horizon()
        {
            Console.Clear();
            Console.WriteLine("Welcome to Horizon 20.07 beta (Windows/.NET Framework 4.7)");
            Console.WriteLine("   * Help and support  : @Vincent.#0705 on Discord");
            Console.WriteLine("                       : @ecnivtwelve on Github");
            Console.WriteLine("                       : @Rexxt on Github");
            Console.WriteLine("   * My website        : https://www.ectw.fr/");
            Console.WriteLine("   * Openlight website : https://openlight.me/");
            Console.WriteLine("  ");

            Horizon_Cmd();
        }

        static void Horizon_Cmd()
        {
            string path = Directory.GetCurrentDirectory();
            string fullPath = Path.GetFullPath(path).TrimEnd(Path.DirectorySeparatorChar);
            string projectName = Path.GetFileName(fullPath);

            Console.Write("horizon:"+Properties.reg64.Default.username+"/"+projectName+"() ");
            string cmd = Console.ReadLine();
            
            // HzCmd builtins

            if(cmd == "horizon flush")
            {
                // Resets OS

                Console.Write("Are you sure you want to reset reg64 (yes/no) ? ");
                string answer = Console.ReadLine();

                if(answer == "yes")
                {
                    Properties.reg64.Default.Reset();
                    Console.Clear();
                    Console.WriteLine("  ");
                    Console.WriteLine("reg64 has been reset to its default values.");
                    Horizon_Logon();
                }
                else {
                    Horizon_Cmd();
                }
            }

            if (cmd.StartsWith("edit"))
            {
                // Builtin text file editor
                // vim for HorizonOS when

                string filename2 = cmd.Replace("edit ", ""); ;

                Console.WriteLine("  ");

                Console.WriteLine("Appending to " + filename2);
                Console.WriteLine("Write [#save] to save the document");
                Separator();

                string lastline;
                bool save = false;

                try
                {
                    while (save == false)
                    {
                        lastline = Console.ReadLine();
                        if (lastline == "#save")
                        {
                            save = true;
                            Console.WriteLine("  ");
                            Horizon_Cmd();
                        }
                        else
                        {
                            File.AppendAllText(@filename2, lastline + Environment.NewLine);
                        }
                    }
                }
                catch
                {
                    Console.WriteLine(" ");
                    Console.WriteLine("Cannot save, this folder doesn't exist");
                    Separator();
                }
            }

            if (cmd.StartsWith("delete"))
            {
                // Deletes file
                string filename2 = cmd.Replace("delete ", ""); ;
                if (File.Exists(filename2))
                {
                    File.Delete(filename2);
                    Console.WriteLine("  ");
                    Horizon_Cmd();
                }
                else
                {
                    Console.WriteLine("The file doesn't exist.");
                    Console.WriteLine("  ");
                    Horizon_Cmd();
                }
            }

            if (cmd.StartsWith("print"))
            {
                // Prints text to console
                Console.WriteLine("  ");
                Console.WriteLine(cmd.Replace("print ", ""));
                Console.WriteLine("  ");
            }

            // list/ls/dir are aliased functions
            // Cist every file/folder in parent folder or specified folder
            if (cmd.StartsWith("list"))
            {
                list(cmd.Replace("list ", ""));
            }

            if (cmd.StartsWith("ls"))
            {
                list(cmd.Replace("ls ", ""));
            }

            if (cmd.StartsWith("dir"))
            {
                list(cmd.Replace("dir ", ""));
            }

            if (cmd.StartsWith("mkdir"))
            {
                // Creates directory
                Directory.CreateDirectory(cmd.Replace("mkdir ", ""));
            }

            // copy/cp are aliased functions
            // Copy file to other location
            if (cmd.StartsWith("copy"))
            {
                Console.Write("What's your destination? ");
                string dest = Console.ReadLine();
                try
                {
                    File.Copy(cmd.Replace("copy ", ""), dest);
                }
                catch
                {
                    Console.WriteLine("This file doesn't exist.");
                }
            }

            if (cmd.StartsWith("cp"))
            {
                Console.Write("What's your destination? ");
                string dest = Console.ReadLine();
                try
                {
                    File.Copy(cmd.Replace("cp ", ""), dest);
                }
                catch
                {
                    Console.WriteLine("This file doesn't exist.");
                }
            }

            if (cmd.StartsWith("del"))
            {
                // Delete directory
                try
                {
                    Directory.Delete(cmd.Replace("del ", ""));
                }
                catch
                {
                    Console.WriteLine("This directory doesn't exist.");
                }
            }

            if (cmd.StartsWith("get"))
            {
                // Package downloader
                Console.Write("What is your destination file? ");
                string file3 = Console.ReadLine();
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                using (var client = new WebClient())
                {
                    client.DownloadFile(cmd.Replace("get ", ""), file3);
                }
            }

            // Sets parent working directory
            if (cmd.StartsWith("cd"))
            {
                try
                {
                    Directory.SetCurrentDirectory(cmd.Replace("cd ", ""));
                }
                catch
                {
                    Console.WriteLine("This directory doesn't exist.");
                }
            }

            if (cmd == "help")
            {
                // Help command
                help();
            }

            if (cmd == "exit")
            {
                System.Environment.Exit(1);
            }

            if (cmd == "?")
            {
                // Help command
                help();
            }

            else
            {
                // executes or prints content of given file
                string args = "";
                string exec = "";

                string[] textSplit = cmd.Split(':');
                exec = textSplit[0];

                if(cmd.Contains(':'))
                {
                    args = textSplit[1];
                }

                System.Diagnostics.Process p = new System.Diagnostics.Process();
                p.StartInfo.FileName = exec;
                p.StartInfo.Arguments = args;
                p.StartInfo.UseShellExecute = false;


                if (File.Exists(exec))
                {
                    bool isExe = CheckIfFileIsExecutable(exec);
                    if (isExe)
                    {
                        // Start executable
                        p.Start();
                        Horizon_Cmd();
                    }
                    else
                    {
                        // Print contents
                        if (File.Exists(cmd))
                        {
                            Console.WriteLine("  ");
                            string data = File.ReadAllText(cmd);
                            Console.WriteLine(data);
                            Console.WriteLine("  ");
                            Horizon_Cmd();
                        }
                        else if (File.Exists(exec))
                        {
                            Console.WriteLine("  ");
                            string data = File.ReadAllText(exec);
                            Console.WriteLine(data);
                            Console.WriteLine("  ");
                            Horizon_Cmd();
                        }
                        else
                        {
                            Horizon_Cmd();
                        }
                    }
                }
                else
                {
                    Horizon_Cmd();
                }
            }
        }

        static bool CheckIfFileIsExecutable(string file)
        {
            try
            {
                var firstTwoBytes = new byte[2];
                using (var fileStream = File.Open(file, FileMode.Open))
                {
                    fileStream.Read(firstTwoBytes, 0, 2);
                }
                return Encoding.UTF8.GetString(firstTwoBytes) == "MZ";
            }
            catch (Exception ex)
            {
                Console.WriteLine("  ");
                Console.WriteLine(ex);
                Console.WriteLine("  ");
            }
            return false;
        }

        static void help()
        {
            Console.WriteLine("  ");
            Console.WriteLine("List of [horizon] commands");
            Separator();
            Console.WriteLine("edit <filename>         | edit a file");
            Console.WriteLine("delete <filename>       | delete a file  ");
            Console.WriteLine("print <text>            | print a given text  ");
            Console.WriteLine("[filename][:args]       | execute or print the content of this file");
            Console.WriteLine("horizon flush           | reset settings (reg64.settings)");
            Console.WriteLine("list/ls/dir <directory> | list all files in directory");
            Console.WriteLine("cd <directory>          | change current directory");
            Console.WriteLine("mkdir <directory>       | create a new directory");
            Console.WriteLine("del <directory>         | delete a directory");
            Console.WriteLine("copy/cp <filename>      | copy a file");
            Console.WriteLine("get <url>               | download from Internet");
            Console.WriteLine("  ");
            Horizon_Cmd();
        }

        static void list(string directory)
        {
            // List files in directory
            if(directory == "")
            {
                directory = System.IO.Directory.GetCurrentDirectory();
                Console.WriteLine("  ");
                Console.WriteLine("List of all files in current directory");
            }
            else if (directory == "list")
            {
                directory = System.IO.Directory.GetCurrentDirectory();
                Console.WriteLine("  ");
                Console.WriteLine("List of all files in current directory");
            }
            else if (directory == "ls")
            {
                directory = System.IO.Directory.GetCurrentDirectory();
                Console.WriteLine("  ");
                Console.WriteLine("List of all files in current directory");
            }
            else if (directory == "dir")
            {
                directory = System.IO.Directory.GetCurrentDirectory();
                Console.WriteLine("  ");
                Console.WriteLine("List of all files in current directory");
            }
            else
            {
                Console.WriteLine("  ");
                Console.WriteLine("List of all files in " + directory);
            }
            
            Separator();
            try {
                string[] allfiles = Directory.GetFiles(directory, "*.*");
                foreach (var file in allfiles)
                {
                    FileInfo info = new FileInfo(file);
                    Console.WriteLine(info.Name);
                }
                string[] allfolders = Directory.GetDirectories(directory, "*.*");
                foreach (var folder in allfolders)
                {
                    FileInfo info2 = new FileInfo(folder);
                    Console.WriteLine("\\" + info2.Name);
                }
            }
            catch
            {
                Console.WriteLine("This directory doesn't exist.");
            }
            Console.WriteLine("  ");
        }
    }
}
