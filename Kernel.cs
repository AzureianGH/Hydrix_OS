using Cosmos.HAL.BlockDevice.Registers;
using Cosmos.HAL.Drivers.Video.SVGAII;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sys = Cosmos.System;
using System.IO;
using Cosmos.System.FileSystem.VFS;
using Cosmos.Core_Asm;
using Cosmos.HAL;
using Cosmos.System.FileSystem;
using Cosmos.HAL.BlockDevice;
using Cosmos.System.Graphics;
using System.Drawing;
using Cosmos.Core.Memory;
using IL2CPU.API.Attribs;
using Cosmos.System.Graphics.Fonts;
using HydrixOS.Installer.User;
using HydrixOS.Tools;
using HydrixOS.Core.Shell;
using HydrixOS.Core.Graphics;
using HydrixOS.Core.Plugs;
namespace HydrixOS.Core
{
    public class Environment
    {
        private int sysdefversion = 0;
        public string UserName = "DefaultUser";
        public string MachineName = "DefaultName";
        public string CurrentDirectory = "0:\\";
        private bool isadmin = false;
        public void Exit()
        {
            Sys.Power.Shutdown();
            
        }
        public int GetSYSDEFVERSION()
        {
            return sysdefversion;
        }
        public bool IsAdmin() 
        {
            return isadmin;
        }
        public void SetAdmin(bool isadmin) 
        {
            this.isadmin = isadmin;
        }
    }
    
    
    public static class HydrixSCAN
    {
        public static void InstallHydrix()
        {
            List<Disk> drives = VFSManager.GetDisks();

            //create options for the user to select from
            string[] options = new string[drives.Count];
            for (int i = 0; i < drives.Count; i++)
            {
                options[i] = drives[i].Size.ToString() + " Bytes";
            }
            OptionSelector selector = new OptionSelector(options, "Select a drive to install Hydrix OS to:");
            selector.SelectOption();
            int selected = selector.SelectedOption;
            Disk selecteddisk = drives[selected];
            Console.WriteLine("Selected: " + selecteddisk.ToString());
            //get partitions
            List<ManagedPartition> partitions1 = selecteddisk.Partitions;
            //if 0 partitions
            if (partitions1.Count == 0)
            {
                Console.WriteLine("No Partitions Detected.");
                Console.WriteLine("Create one?");
                Console.WriteLine("Proceed? (y/n) >");
                string inputs = Console.ReadLine();
                inputs = inputs.ToLower();
                if (inputs == "y" || inputs == "yes")
                {
                    bool isworking = true;
                    while (isworking)
                    {
                        Console.WriteLine($"Selected Disk Size: " + (selecteddisk.Size / 1000000).ToString() + " Megabytes");
                        //ask for size
                        Console.WriteLine("Enter Partition Size (in Megabytes) >");
                        string size = Console.ReadLine();
                        long sizeint = long.Parse(size);
                        //check if size is greater than disk size
                        if (sizeint > selecteddisk.Size)
                        {
                            Console.WriteLine("Size is greater than disk size.");
                        }
                        else if (sizeint < 0)
                        {
                            Console.WriteLine("Size is less than 0.");
                        }
                        //cannot be 0
                        else if (sizeint == 0)
                        {
                            Console.WriteLine("Size cannot be 0.");
                        }
                        //check if non number
                        else if (!long.TryParse(size, out long result))
                        {
                            Console.WriteLine("Size is not a number.");
                        }
                        else
                        {
                            Console.WriteLine("Creating Partition...");
                            selecteddisk.CreatePartition((int)sizeint);
                            Console.WriteLine("Partition Created.");
                            isworking = false;
                            Console.WriteLine("Rebooting...");
                            Sys.Power.Reboot();
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Exiting...");
                    return;
                }
            }
            List<ManagedPartition> partitions = selecteddisk.Partitions;
            string[] partoptions = new string[partitions.Count];
            
            OptionSelector partselector = new OptionSelector(partoptions, "Select a partition to install Hydrix OS to:");
            partselector.SelectOption();
            int selectedpart = partselector.SelectedOption;
            ManagedPartition selectedpartition = partitions[selectedpart];
            Console.WriteLine("Selected: " + selectedpartition.ToString());
            Console.WriteLine("Installing Hydrix OS to " + selectedpartition.ToString());
            Console.WriteLine("Warning: This will format the partition and delete all data on it.");
            Console.WriteLine("Proceed? (y/n) >");
            string input = Console.ReadLine();
            input = input.ToLower();
            if (input == "y" || input == "yes")
            {
                    Console.WriteLine("Formatting Partition...");
                    selecteddisk.FormatPartition(selectedpart, "FAT32", false);
                    Console.WriteLine("Copying Hydrix OS to Partition...");
                    Console.WriteLine("Creating System Directories...");
                    Directory.CreateDirectory(@"0:\sys");
                    Directory.CreateDirectory(@"0:\usr");
                    Directory.CreateDirectory(@"0:\sys\reg");
                    Directory.CreateDirectory(@"0:\sys\tmp");
                    File.Create(@"0:\sys\reg\hydrix.sysdef");
                    File.Create(@"0:\sys\tmp\installerfile.tmp");

                    Console.WriteLine("Disk Formatted.");
                    Console.WriteLine("Hydrix OS Installed.");
                    Console.WriteLine("Rebooting...");
                    Sys.Power.Reboot();
            }
            else
            {
                Console.WriteLine("Exiting...");
                return;
            }
        }
        public static void FormatDisks()
        {
            List<Disk> drives = VFSManager.GetDisks();
            string[] options = new string[drives.Count];
            for (int i = 0; i < drives.Count; i++)
            {
                options[i] = drives[i].Size.ToString() + " Bytes";
            }
            OptionSelector selector = new OptionSelector(options, "Select a drive to format:");
            selector.SelectOption();
            int selected = selector.SelectedOption;
            Disk selecteddisk = drives[selected];
            Console.WriteLine("Selected: " + selecteddisk.ToString());
            Console.WriteLine("Warning: This will format the disk and delete all data on it.");
            Console.WriteLine("Proceed? (y/n) >");
            string input = Console.ReadLine();
            input = input.ToLower();
            if (input == "y" || input == "yes")
            {
                Console.WriteLine("Formatting Disk...");
                selecteddisk.Clear();
                Console.WriteLine("Disk Formatted.");
                Console.WriteLine("Rebooting...");
                Sys.Power.Reboot();
            }
            else
            {
                Console.WriteLine("Exiting...");
                return;
            }
        }
        public static void PrepareFormat()
        {
            //Create options with, Format Disks, Format Partitions, Exit
            string[] options = new string[] { "Format Disk", "Format Partition", "Exit" };
            OptionSelector selector = new OptionSelector(options, "Hydrix OS Partitioner");
            selector.SelectOption();
            if (selector.SelectedOption == 0)
            {
                FormatDisks();
            }
            else if (selector.SelectedOption == 1)
            {
                //FormatPartitions();
            }
            else
            {
                Console.WriteLine("Exiting...");
                return;
            }
        }
        
        public static void NewHydrixInstall(bool isinvalid = false)
        {
            if (isinvalid)
            {
                //Write: Hydrix OS failed to fix the OS.
                Console.WriteLine("Hydrix OS failed to fix the OS.");
                // Write: A complete reinstall is required.
                Console.WriteLine("A complete reinstall is required.");
                // Write: Press any key to continue.
                Console.WriteLine("Press any key to continue.");
                Console.ReadKey();
            }
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("-[ Hydrix OS Installer ]-");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Welcome to the Hydrix OS Installer.");
            Console.WriteLine("This installer will guide you through the installation of Hydrix OS.");
            Console.WriteLine("Proceed to installation options? (y/n) >");
                string input = Console.ReadLine();
                input = input.ToLower();
                if (input == "y" || input == "yes")
                {
                    do
                    { 
                        string[] options = new string[] { "Install Hydrix OS", "Check Hydrix OS Installation", "Exit" };
                        OptionSelector selector = new OptionSelector(options, "Hydrix Installer");
                        selector.SelectOption();
                        if (selector.SelectedOption == 0)
                        {
                            InstallHydrix();
                        }
                        else if (selector.SelectedOption == 1)
                        {
                            int returncode = CheckHydrixInstallation();
                            if (returncode == 0)
                            {
                                Console.WriteLine("Hydrix Installer Detected an Existing Installation.");
                                Console.ReadKey();
                            }
                            else if (returncode == 1)
                            {
                                Console.WriteLine("Hydrix Installer Detected an Incomplete Installation.");
                                Console.ReadKey();
                            }
                            else
                            {
                                Console.WriteLine("Hydrix Installer Failed to Detect an Existing Installation.");
                                Console.ReadKey();
                            }
                        }
                        else if (selector.SelectedOption == 2)
                        {
                            Console.WriteLine("Exiting...");
                            return;
                        }
                    } while (true);
                }
                else
                {
                    Console.WriteLine("Exiting...");
                    return;
            }
        }
        public static void RepairHydrixInstall()
        {
            //check for sys and usr

            Console.WriteLine("Check sys");
            if (!Directory.Exists(@"0:\sys"))
            {
                Console.WriteLine("System Directory Not Found.");
                Console.WriteLine("Creating System Directory...");
                Directory.CreateDirectory(@"0:\sys");
                Console.WriteLine("System Directory Created.");
            }
            Console.WriteLine("Check usr");
            if (!Directory.Exists(@"0:\usr"))
            {
                Console.WriteLine("User Directory Not Found.");
                Console.WriteLine("Creating User Directory...");
                Directory.CreateDirectory(@"0:\usr");
                Console.WriteLine("User Directory Created.");
            }
            Console.WriteLine("Check tmp");
            //check if directories were made
            if (!Directory.Exists(@"0:\sys") && !Directory.Exists(@"0:\usr"))
            {
                Console.WriteLine("Failed to create directories. Reinstalling.");
                //Launch installer
                NewHydrixInstall(true);
            }
            else
            {
                Console.WriteLine("Directories Created.");
            }
            try
            {
                string[] dirs = Directory.GetDirectories(@"0:\usr\");
                if (dirs.Length == 0)
                {
                    Console.WriteLine("No Users Detected.");
                    Console.WriteLine("Entering user setup...");
                    User.NewUserSetup(true);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                Console.WriteLine("Entering setup...");
                NewHydrixInstall(true);
            }
        }
        public static int CheckHydrixInstallation()
        {
            bool sys = false;
            bool usr = false;
            if (Directory.Exists(@"0:\sys"))
            {
                sys = true;
            }
            if (Directory.Exists(@"0:\usr"))
            {
                usr = true;
            }
            if (sys && usr)
            {
                Console.WriteLine("Hydrix OS Validated.");
                //check for tmp file
                if (File.Exists(@"0:\sys\tmp\installerfile.tmp"))
                {
                    string[] dirs = Directory.GetDirectories(@"0:\usr\");
                    if (dirs.Length == 0)
                    {
                        Console.WriteLine("Finishing Install...");
                        Console.WriteLine("Entering user setup...");
                        User.NewUserSetup(true);
                    }
                    else
                    {
                        foreach (string dir in dirs)
                        {
                            if (File.Exists(dir + @"\user.def"))
                            {
                                string ret = HydrixTOOLS.ReadSystemDefinitionFile(dir + @"\user.def");
                                string newret = HydrixTOOLS.XorStr("hydrix", ret);
                                if (newret.Contains(":?:"))
                                {
                                    return 0;
                                }
                                else
                                {
                                    Console.WriteLine("No Admin Account Detected");
                                    User.NewUserSetup(true);
                                }
                            }
                        }
                    }

                }
                return 0;
            }
            else if (sys && !usr)
            {
                Console.WriteLine("Hydrix OS Not Validated.");
                //Write what is not validated
                Console.WriteLine("User Directory Not Found.");
                return 1;
            }
            else if (!sys && usr)
            {
                Console.WriteLine("Hydrix OS Not Validated.");
                //Write what is not validated
                Console.WriteLine("System Directory Not Found.");
                return 1;
            }
            else
            {
                Console.WriteLine("Hydrix OS Not Validated.");
                return 2;
            }

        }
        
    }
    public class Kernel : Sys.Kernel
    {
        public bool graphicsmode = false;
        HydrixShell shell;
        Environment env;
        public Canvas canvas;
        CosmosVFS fs = new CosmosVFS();
        protected override void BeforeRun()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("-[ HYDRIX OS PRE META LAUNCH ]-");
                Console.WriteLine("Mounting FS...");
                VFSManager.RegisterVFS(fs);
                UtilShell utilShell = new UtilShell(fs);
                utilShell.Start();
                Console.WriteLine("FS Mounted.");
                Console.WriteLine("Instancing Environment...");
                env = new Environment();
                int rtcode = HydrixSCAN.CheckHydrixInstallation();
                if (rtcode == 0)
                {
                    Console.WriteLine("Hydrix OS Detected.");
                }
                else if (rtcode == 1)
                {
                    Console.WriteLine("Hydrix OS Detected, but not validated.");
                    Console.WriteLine("Starting Repair...");
                    HydrixSCAN.RepairHydrixInstall();

                }
                else
                {
                    Console.WriteLine("Hydrix OS Not Detected.");
                    Console.WriteLine("Starting Installer...");
                    HydrixSCAN.NewHydrixInstall();
                }
                Console.WriteLine("Environment Instanced.");
                bool login = false;
                while (!login)
                {
                    Console.WriteLine("Login to HSHELL:");
                    Console.Write("Username > ");
                    string? username = Console.ReadLine();
                    Console.WriteLine();
                    Console.Write("Password > ");
                    string? password = Console.ReadLine();
                    login = User.TryLogin(username, password, out bool isadmin);
                    if (login)
                    {
                        env.UserName = username;
                        if (isadmin)
                        {
                            env.SetAdmin(true);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Failed to login!");
                    }
                }
                Console.Clear();
                Console.WriteLine("Instancing Shell...");
                shell = new HydrixShell(env, this);
                Console.WriteLine("Shell Instanced.");
                Console.WriteLine("Starting Shell...");
            }
            catch (Exception e)
            {
                Console.Clear();
                Console.SetCursorPosition(0, 0);
                Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine("-[ HYDRIX OS KERNEL PANIC ]-");
                Console.WriteLine("An error occurred while installing Hydrix OS.");
                Console.WriteLine("Error: " + e.Message);
                Console.WriteLine(e.GetType());
                Console.WriteLine();
                Console.WriteLine("-[ END KERNEL PANIC ]-");
                Console.WriteLine("Press any key to reboot...");
                Console.ReadKey();
                Sys.Power.Reboot();
            }
        }
        protected override void Run()
        {
            try
            {
                shell.Start();
                if (graphicsmode)
                {
                    GraphicsHandler handler = new GraphicsHandler();
                    handler.Start(canvas);
                }
            }
            catch (Exception e)
            {
                Console.Clear();
                Console.SetCursorPosition(0, 0);
                Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine("-[ HYDRIX OS KERNEL PANIC ]-");
                Console.WriteLine("An error occurred while running Hydrix OS.");
                Console.WriteLine("Error: " + e.Message);
                Console.WriteLine(e.GetType());
                Console.WriteLine();
                Console.WriteLine("-[ END KERNEL PANIC ]-");
                Console.WriteLine("Press any key to reboot...");
                Console.ReadKey();
                Sys.Power.Reboot();
            }
        }
    }
    
    
}
