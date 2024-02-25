using Cosmos.System.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HydrixOS.Core;
using Cosmos.System.FileSystem;
using Cosmos.HAL.BlockDevice;
using System.Security;
using Cosmos.HAL;
using HydrixOS.Tools;
using System.Threading;
using HydrixOS.Core.Threading;
namespace HydrixOS.Core.Shell
{
    public class ThreadTest
    {
        public static void ThreadMain()
        {
            // Initialize the scheduler with a maximum of 2 tasks
            Scheduler.Initialize(2);

            // Create two tasks
            Scheduler.CreateTask(PrintNumbers);
            Scheduler.CreateTask(PrintLetters);

            // Start the scheduler
            Scheduler.Start();
        }

        // Task to print numbers from 1 to 5
        private static void PrintNumbers()
        {
            for (int i = 1; i <= 5; i++)
            {
                Console.WriteLine($"Number: {i}");
                Thread.Sleep(1000); // Sleep for 1 second
            }
        }

        // Task to print letters from 'A' to 'E'
        private static void PrintLetters()
        {
            for (char c = 'A'; c <= 'E'; c++)
            {
                Console.WriteLine($"Letter: {c}");
                Thread.Sleep(1500); // Sleep for 1.5 seconds
            }
        }
    }
    public class UtilShell
    {
        CosmosVFS vFS;
        bool running = true;
        public UtilShell(CosmosVFS vFS)
        {
            this.vFS = vFS;
        }
        public void Start() 
        {
            while (running)
            {
                Console.Write("Hydrix UtilShell \\:>");
                string input = Console.ReadLine();
                Console.WriteLine();
                Parse(input);
            }
        }
        void Parse(string input)
        {
            string[] commands = input.Split(' ');
            string command = commands[0];
            string[] args = new string[commands.Length - 1];
            for (int i = 1; i < commands.Length; i++)
            {
                args[i - 1] = commands[i];
            }
            Execute(command, args);
        }
        void Execute(string cmd, string[] args)
        {
            switch (cmd.ToLower())
            {
                case "udisk":
                    DiskUtil();
                    break;
                case "exit":
                    Exit();
                    break;
                case "thread":
                    ThreadTest.ThreadMain();
                    break;
                default:
                    UnknownCommand(cmd);
                    break;
            }
        }
        void DiskUtil()
        {
            Disk selecteddisk = null;
            int? diskindex = null;
            ManagedPartition selectedpar = null;
            int? parindex = null;
            string selectedvol = null;
            Console.WriteLine("Hydrix DiskUtil Version [0.0.1b]");
            while (true)
            {
                Console.Write("UDISK \\:>");
                string input = Console.ReadLine();
                input = input.ToLower();
                if (input == "")
                {
                    Console.WriteLine();
                }

                else if (input.ToLower().StartsWith("list"))
                {
                    //get args
                    string[] args = input.Split(' ');

                    if (args.Length == 0)
                    {
                        Console.WriteLine($"{args[1]} {{disk | partition}}");
                    }
                    else if (args[1] == "disk")
                    {
                        List<Disk> disks = vFS.Disks;
                        Console.WriteLine("  Disk Num  Type         Size          GPT");
                        Console.WriteLine("  --------  -----------  ------------  ---");
                        for (int i = 0; i < disks.Count; i++)
                        {
                            if (selecteddisk != null)
                            {
                                if (selecteddisk == disks[i])
                                {
                                    Console.Write(">");
                                    Console.Write($" Disk {i}");
                                }
                                else
                                {
                                    Console.Write($"  Disk {i}");
                                }
                            }
                            else
                            {
                                Console.Write($"  Disk {i}");
                            }
                            //if not 2 digits, add 2 spaces
                            if (i < 10)
                            {
                                Console.Write("  ");
                            }
                            else if (i < 100)
                            {
                                Console.Write(" ");
                            }
                            //HardDrive
                            //RemovableCD
                            //Removable
                            if (disks[i].Type == BlockDeviceType.HardDrive)
                            {
                                Console.Write("  HardDrive  ");
                            }
                            else if (disks[i].Type == BlockDeviceType.RemovableCD)
                            {
                                Console.Write("  RemovableCD");
                            }
                            else if (disks[i].Type == BlockDeviceType.Removable)
                            {
                                Console.Write("  Removable  ");
                            }
                            int stringsize = 0;
                            long size = disks[i].Size; //in bytes
                            if (size > 1000000)
                            {
                                Console.Write($"  {size / 1000000} MB");
                                stringsize = $"  {size / 1000000} MB".Length;
                            }
                            else if (size > 1000)
                            {
                                Console.Write($"  {size / 1000} KB");
                                stringsize = $"  {size / 1000} KB".Length;
                            }
                            else
                            {
                                Console.Write($"  {size} B");
                                stringsize = $"  {size} B".Length;
                            }
                            Console.Write(new string(' ', 12 - stringsize));
                            if (!disks[i].IsMBR)
                            {
                                Console.Write("   * ");
                            }
                            else
                            {
                                Console.Write("     ");
                            }
                            Console.WriteLine();
                        }
                    }
                    else if (args[1] == "par" || args[1] == "partition")
                    {
                        if (selecteddisk == null)
                        {
                            Console.WriteLine("A disk must be selected before this operation can complete.");
                            continue;
                        }
                        Console.WriteLine("  Partition Num  Type         Size");
                        Console.WriteLine("  -------------  -----------  ------------");
                        int i = 0;
                        foreach (ManagedPartition mp in selecteddisk.Partitions)
                        {
                            if (selectedpar != null)
                            {
                                if (selectedpar == mp)
                                {
                                    Console.Write(">");
                                    Console.Write($" Partition {i}");
                                }
                                else
                                {
                                    Console.Write($"  Partition {i}");
                                }
                            }
                            else
                            {
                                Console.Write($"  Partition {i}");
                            }
                            Partition ump = mp.Host;

                            //if not 2 digits, add 2 spaces
                            if (i < 10)
                            {
                                Console.Write("  ");
                            }
                            else if (i < 100)
                            {
                                Console.Write(" ");
                            }
                            //HardDrive
                            //RemovableCD
                            //Removable
                            if (ump.Type == BlockDeviceType.HardDrive)
                            {
                                Console.Write("  HardDrive  ");
                            }
                            else if (ump.Type == BlockDeviceType.RemovableCD)
                            {
                                Console.Write("  RemovableCD");
                            }
                            else if (ump.Type == BlockDeviceType.Removable)
                            {
                                Console.Write("  Removable  ");
                            }
                            if (mp.MountedFS != null)
                            {
                                long size = mp.MountedFS.Size; //in bytes
                                size = size * 1000000;
                                if (size > 1000000)
                                {
                                    Console.Write($"  {size / 1000000} MB");
                                }
                                else if (size > 1000)
                                {
                                    Console.Write($"  {size / 1000} KB");
                                }
                                else
                                {
                                    Console.Write($"  {size} B");
                                }
                            }
                            else
                            {
                                Console.Write("  Unformatted");
                            }
                            i++;
                            Console.WriteLine();
                        }
                    }
                    else
                    {
                        //print args
                        Console.WriteLine($"{args[1]} {{disk | par | vol}}");
                    }
                }
                else if (input.ToLower().StartsWith("select") || input.ToLower().StartsWith("sel"))
                {
                    //get args
                    string[] args = input.Split(' ');
                    if (args[1] == "disk")
                    {
                        //get disk num
                        bool isnum = int.TryParse(args[2], out int disknum);
                        if (isnum)
                        {
                            if (disknum < vFS.Disks.Count)
                            {
                                selecteddisk = vFS.Disks[disknum];
                                diskindex = disknum;
                            }
                            else
                            {
                                Console.WriteLine("Invalid Disk Number");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid Disk Number");
                        }
                    }
                    else if (args[1] == "par" || args[1] == "partition")
                    {
                        if (selecteddisk == null)
                        {
                            Console.WriteLine("A disk must be selected before this operation can complete.");
                            continue;
                        }
                        //get partition num
                        bool isnum = int.TryParse(args[2], out int parnum);
                        if (isnum)
                        {
                            if (parnum < selecteddisk.Partitions.Count)
                            {
                                selectedpar = selecteddisk.Partitions[parnum];
                                parindex = parnum;
                            }
                            else
                            {
                                Console.WriteLine("Invalid Partition Number");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid Partition Number");
                        }
                    }

                    else
                    {
                        Console.WriteLine("select {disk | partition} {index}");
                    }
                }
                else if (input.ToLower().StartsWith("clear"))
                {
                    if (selecteddisk != null)
                    {
                        selecteddisk.Clear();
                    }
                }
                else if (input.ToLower().StartsWith("format"))
                {
                    if (selectedpar != null)
                    {
                        //get args
                        string[] args = input.Split(' ');
                        if (args[1] == "quick")
                        {
                            selecteddisk.FormatPartition((int)parindex, "FAT32", true);
                        }
                        else if (args[1] == "full")
                        {
                            selecteddisk.FormatPartition((int)parindex, "FAT32", false);
                        }
                        else
                        {
                            Console.WriteLine("format {quick | full}");
                        }
                    }
                }
                else if (input.ToLower().StartsWith("create") || input.ToLower().StartsWith("cre") || input.ToLower().StartsWith("crea"))
                {
                    if (selecteddisk != null)
                    {
                        //get args
                        string[] args = input.Split(' ');
                        if (args[1] == "partition" || args[1] == "par")
                        {
                            bool isint = int.TryParse(args[2], out int result);
                            if (isint)
                            {
                                selecteddisk.CreatePartition(result);
                                Console.WriteLine("Partition Created");
                            }
                            else
                            {
                                Console.WriteLine("create {partition | par} {size}");
                            }
                        }
                    }
                }
                else if (input.ToLower().StartsWith("delete") || input.ToLower().StartsWith("del"))
                {
                    //get args
                    string[] args = input.Split(' ');
                    if (args[1] == "partition" || args[1] == "par")
                    {
                        if (selecteddisk != null)
                        {
                            bool isint = int.TryParse(args[2], out int result);
                            if (isint)
                            {
                                selecteddisk.DeletePartition(result);
                                Console.WriteLine("Partition Deleted");
                            }
                            else
                            {
                                Console.WriteLine("delete {partition | par} {index}");
                            }
                        }
                    }
                }
                else if (input.ToLower().StartsWith("exit"))
                {
                    break;
                }
                else if (input.ToLower().StartsWith("help"))
                {
                    Console.WriteLine("list {disk | partition}");
                    Console.WriteLine("select {disk | partition} {index}");
                    Console.WriteLine("clear");
                    Console.WriteLine("format {quick | full}");
                    Console.WriteLine("create {partition | par} {size}");
                    Console.WriteLine("delete {partition | par} {index}");
                    Console.WriteLine("exit");
                }
                else
                {
                    UnknownCommand(input);
                }
            }
        }
        void Exit()
        {
            Console.WriteLine("Exiting UtilShell...");
            running = false;
            return;
        }
        void UnknownCommand(string cmd)
        {
            HConsole.WriteLineColor("Can't Find: ", ConsoleColor.Red);
            Console.WriteLine($"\"{cmd}\"");
            Console.WriteLine();
        }
    }
    public class HydrixShell
    {
        bool shellrunning = true;
        Core.Environment env;
        Kernel kern;
        public HydrixShell(Core.Environment env, Kernel kern)
        {
            this.env = env;
            this.kern = kern;
        }
        public Dictionary<string, string> Variables = new Dictionary<string, string>();
        public bool currentrunning = false;
        public void Start()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Hydrix Shell");
            Console.WriteLine("Type 'exit' to exit");
            Variables.Add("username", env.UserName);
            Variables.Add("machinename", env.MachineName);
            Variables.Add("pwd", env.CurrentDirectory);
            //Start another thread that constantly checks for key combinations
            while (shellrunning)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("(" + env.UserName + "=" + env.MachineName + ") ");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("{" + env.CurrentDirectory + "}");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("$ ");
                Console.ForegroundColor = ConsoleColor.White;
                string input = Console.ReadLine();
                Parse(input);
            }
            return;
        }
        void Parse(string input)
        {
            string[] commands = input.Split(' ');
            string command = commands[0];
            string[] args = new string[commands.Length - 1];
            for (int i = 1; i < commands.Length; i++)
            {
                args[i - 1] = commands[i];
            }
            Execute(command, args);
        }
        void Execute(string cmd, string[] args)
        {
            switch (cmd)
            {
                case "echo":
                    Echo(args);
                    break;
                case "exit":
                    Exit();
                    break;
                case "clear":
                    Console.Clear();
                    break;
                case "run":
                    ExternRun(args);
                    break;
                case "let":
                    //first arg is let, second is variable name, third is =, fourth is value (some dont have a value)
                    if (args.Length == 1)
                    {
                        Variables.Add(args[0], "null");
                    }
                    else if (args.Length == 3)
                    {
                        //check if the second arg is a variable, if so, see if it's valid
                        if (args[2].StartsWith("%"))
                        {
                            string newarg = args[2].Replace("%", "");
                            if (Variables.ContainsKey(newarg))
                            {
                                Variables.Add(args[0], Variables[newarg]);
                            }
                            else
                            {
                                InvalidArgument(cmd, args);
                            }
                        }
                        else
                        {
                            Variables.Add(args[0], args[2]);
                        }
                    }

                    else
                    {
                        InvalidArgument(cmd, args);
                    }
                    break;
                case "cd":
                    if (args.Length == 1)
                    {
                        if (args[0] == "..")
                        {
                            if (env.CurrentDirectory == "0:\\")
                            {
                                CustomError("Cannot go back any further.");
                                //the root dir is: 0:\
                                break;
                            }
                            env.CurrentDirectory = env.CurrentDirectory.Remove(env.CurrentDirectory.Length - 1);
                            env.CurrentDirectory = env.CurrentDirectory.Remove(env.CurrentDirectory.LastIndexOf("\\") + 1);
                            Directory.SetCurrentDirectory(env.CurrentDirectory);
                        }
                        if (!Directory.Exists(args[0]))
                            if (!System.IO.Directory.Exists(env.CurrentDirectory + args[0]))
                            {
                                FileError(args[0]);
                                break;
                            }
                        env.CurrentDirectory = env.CurrentDirectory + args[0];
                        Directory.SetCurrentDirectory(env.CurrentDirectory);
                    }
                    else
                    {
                        InvalidArgument(cmd, args);
                    }
                    break;
                case "ls":
                    if (args.Length == 0)
                    {
                        ls(env.CurrentDirectory);
                    }
                    else
                    {
                        InvalidArgument(cmd, args);
                    }
                    break;
                case "mkdir":
                    if (args.Length == 1)
                    {
                        Mkdir(args[0]);
                    }
                    else
                    {
                        InvalidArgument(cmd, args);
                    }
                    break;
                case "help":
                    help(args);
                    break;
                case "cat":
                    cat(args);
                    break;
                case "load":
                    if (args.Length == 1)
                    {
                        if (args[0] == "graphics")
                        {
                            switchtographics(kern.canvas);
                        }
                    }
                    else
                    {
                        InvalidArgument(cmd, args);
                    }
                    break;
                default:
                    if (cmd.StartsWith("%"))
                    {
                        string newcmd = cmd.Replace("%", "");
                        if (Variables.ContainsKey(newcmd))
                        {
                            Echo($"{newcmd} is a variable.");
                            break;
                        }
                    }
                    UnknownCommand(cmd);
                    break;
            }
        }
        void Mkdir(string dir)
        {
            try
            {
                Directory.CreateDirectory(dir);
            }
            catch
            {
                CustomError("Cannot create directory: " + dir);
            }
        }
        void ExternRun(string[] args)
        {
            //run first arg in bash

        }
        void cat(string[] args)
        {
            //check if first arg is a file, if more than one arg, check for > and file after
            //if >, write to file
            //if >>, append to file
            //if no > or >>, print to console
            if (args.Length == 1)
            {
                if (File.Exists(env.CurrentDirectory + args[0]))
                {
                    string[] lines = File.ReadAllLines(args[0]);
                    for (int i = 0; i < lines.Length; i++)
                    {
                        Console.WriteLine(lines[i]);
                    }
                }
                else
                {
                    FileError(args[0]);
                }
            }
            else if (args.Length == 2)
            {
                //check if >
                if (args[0] == ">")
                {
                    if (File.Exists(env.CurrentDirectory + args[1]))
                    {
                        //create file
                        File.Create(args[1]);
                    }
                    else
                    {
                        FileError(args[0]);
                    }
                }
                else
                {
                    InvalidArgument("cat", args);
                }
            }
            else if (args.Length == 3)
            {
                if (args[1] == ">")
                {
                    if (File.Exists(env.CurrentDirectory + args[2]))
                    {
                        File.WriteAllText(args[2], args[0]);
                    }
                    else
                    {
                        FileError(args[2]);
                    }
                }
                else if (args[1] == ">>")
                {
                    if (File.Exists(env.CurrentDirectory + args[2]))
                    {
                        File.AppendAllText(args[2], args[0]);
                    }
                    else
                    {
                        FileError(args[2]);
                    }
                }
                else
                {
                    InvalidArgument("cat", args);
                }
            }
            else
            {
                InvalidArgument("cat", args);
            }


        }
        void help(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Hydrix Shell Help:");
                Console.WriteLine("echo - prints to console");
                Console.WriteLine("exit - exits the shell");
                Console.WriteLine("clear - clears the console");
                Console.WriteLine("let - creates a variable");
                Console.WriteLine("cd - changes directory");
                Console.WriteLine("ls - lists files in directory");
                Console.WriteLine("mkdir - creates a directory");
                Console.WriteLine("cat - prints file contents");
                Console.WriteLine("help - prints this message");
            }
            else
            {
                //print help for specific command
                if (args[0] == "echo")
                {
                    Console.WriteLine("echo - prints to console");
                    Console.WriteLine("Usage: echo [args]");
                }
                else if (args[0] == "exit")
                {
                    Console.WriteLine("exit - exits the shell");
                    Console.WriteLine("Usage: exit");
                }
                else if (args[0] == "clear")
                {
                    Console.WriteLine("clear - clears the console");
                    Console.WriteLine("Usage: clear");
                }
                else if (args[0] == "let")
                {
                    Console.WriteLine("let - creates a variable");
                    Console.WriteLine("Usage: let [varname] = [value]");
                }
                else if (args[0] == "cd")
                {
                    Console.WriteLine("cd - changes directory");
                    Console.WriteLine("Usage: cd [dir]");
                }
                else if (args[0] == "ls")
                {
                    Console.WriteLine("ls - lists files in directory");
                    Console.WriteLine("Usage: ls");
                }
                else if (args[0] == "mkdir")
                {
                    Console.WriteLine("mkdir - creates a directory");
                    Console.WriteLine("Usage: mkdir [dir]");
                }
                else if (args[0] == "help")
                {
                    Console.WriteLine("help - prints this message");
                    Console.WriteLine("Usage: help [command]");
                }
                else
                {
                    UnknownCommand(args[0]);
                }
            }
        }
        string[] GetFilesInDirectory(string dir)
        {
            return Directory.GetFiles(dir);
        }
        string[] GetDirectoriesInDirectory(string dir)
        {
            return Directory.GetDirectories(dir);
        }
        void ls(string dir)
        {
            //if directory, color light blue
            //if file, color white
            string[] files = GetFilesInDirectory(dir);
            string[] dirs = GetDirectoriesInDirectory(dir);
            for (int i = 0; i < dirs.Length; i++)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write(dirs[i] + "\t");
            }
            for (int i = 0; i < files.Length; i++)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(files[i] + "\t");
            }
            Console.WriteLine();
        }
        void switchtographics(Canvas canvas)
        {
            kern.graphicsmode = true;
            this.shellrunning = false;
        }
        void Echo(string[] args)
        {
            string output = "";
            //if %, print variable contents
            if (args[0].StartsWith("%"))
            {
                string newarg = args[0].Replace("%", "");
                //check if variable exists
                if (!Variables.ContainsKey(newarg))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Variable not found:");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"\"{newarg}\"");
                    return;
                }
                output = Variables[newarg];
                Console.WriteLine(output);
                return;
            }
            for (int i = 0; i < args.Length; i++)
            {
                output += args[i] + " ";
            }
            System.Console.WriteLine(output);
        }
        void Echo(string arg)
        {
            System.Console.WriteLine(arg);
        }
        void Exit()
        {
            env.Exit();
        }
        void InvalidArgument(string cmd, string[] invalidargs)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Invalid Arguments for Command:");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"\"{cmd}\"");
            Console.Write("Arguments: ");
            for (int i = 0; i < invalidargs.Length; i++)
            {
                Console.Write(invalidargs[i] + " ");
            }
            Console.WriteLine();
        }
        void CustomError(string error)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error:");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(error);
        }
        void FileError(string file)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Invalid File Handler Passed In Transaction:");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"Unable to locate \"{file}\".");
        }
        void UnknownCommand(string cmd)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Invalid {Unfound} Command:");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"\"{cmd}\"");
            //for length, write ^ under the unknown command
            for (int i = 0; i < cmd.Length + 2; i++)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("^");
                Console.ForegroundColor = ConsoleColor.White;
            }
            Console.WriteLine();
        }

    }
}
