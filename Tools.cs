using HydrixOS.Core;
using System;
using System.IO;
using System.Text;
namespace HydrixOS.Tools
{
    public class OptionSelector
    {
        public string[] Options;
        public int SelectedOption = 0;
        private string Print;
        public OptionSelector(string[] options)
        {
            this.Options = options;
            SelectedOption = 0;
        }
        public OptionSelector(string[] options, string print)
        {
            this.Options = options;
            this.Print = print;
            SelectedOption = 0;
        }
        public void SelectOption()
        {
            ConsoleKeyInfo key;
            do
            {
                if (Print != null)
                {
                    Console.WriteLine(Print);
                }
                Console.WriteLine("Select an option:");
                for (int i = 0; i < Options.Length; i++)
                {
                    if (i == SelectedOption)
                    {
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.WriteLine($"[{i}] " + Options[i]);
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.BackgroundColor = ConsoleColor.Black;
                    }
                    else
                    {
                        Console.WriteLine($"[{i}] " + Options[i]);
                    }
                }
                key = Console.ReadKey();
                if (key.Key == ConsoleKey.UpArrow)
                {
                    if (SelectedOption > 0)
                    {
                        SelectedOption--;
                    }
                }
                else if (key.Key == ConsoleKey.DownArrow)
                {
                    if (SelectedOption < Options.Length - 1)
                    {
                        SelectedOption++;
                    }
                }
                Console.Clear();
                if (SelectedOption < 0)
                {
                    SelectedOption = Options.Length - 1;
                }
                else if (SelectedOption > Options.Length - 1)
                {
                    SelectedOption = 0;
                }
            } while (key.Key != ConsoleKey.Enter);
        }
    }
    public static class HConsole
    {
        public static void WriteColor(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = ConsoleColor.White;
        }
        public static void WriteColor(string text, ConsoleColor color, ConsoleColor bgcolor)
        {
            Console.ForegroundColor = color;
            Console.BackgroundColor = bgcolor;
            Console.WriteLine(text);
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
        }
        public static void WriteColor(string text, ConsoleColor color, ConsoleColor bgcolor, bool reset)
        {
            Console.ForegroundColor = color;
            Console.BackgroundColor = bgcolor;
            Console.WriteLine(text);
            if (reset)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.Black;
            }
        }
        public static void WriteColor(string text, ConsoleColor color, bool reset)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            if (reset)
            {
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
        //Write a line with a color
        public static void WriteLineColor(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = ConsoleColor.White;
        }
        public static void WriteLineColor(string text, ConsoleColor color, ConsoleColor bgcolor)
        {
            Console.ForegroundColor = color;
            Console.BackgroundColor = bgcolor;
            Console.WriteLine(text);
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
        }
        public static void WriteLineColor(string text, ConsoleColor color, ConsoleColor bgcolor, bool reset)
        {
            Console.ForegroundColor = color;
            Console.BackgroundColor = bgcolor;
            Console.WriteLine(text);
            if (reset)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.Black;
            }
        }
        public static void WriteLineColor(string text, ConsoleColor color, bool reset)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            if (reset)
            {
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
    }
    public static class HydrixTOOLS
    {
        public static void CreateSystemDefinitionFile(string data, Core.Environment env, string filepath)
        {
            int version = env.GetSYSDEFVERSION();
            string important = version + "|" + data + "|end";
            //convert to bytes
            byte[] bytes = Encoding.ASCII.GetBytes(important);
            // open file and write to it
            File.WriteAllBytes(filepath, bytes);
        }
        public static string ReadSystemDefinitionFile(string filepath)
        {
            byte[] data = File.ReadAllBytes(filepath);
            //convert to string
            string retdata = Encoding.ASCII.GetString(data);
            return retdata;
        }
        public static string XorStr(string key, string input)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < input.Length; i++) sb.Append((char)(input[i] ^ key[(i % key.Length)]));
            string result = sb.ToString();
            return result;
        }
        public static string[] EncryptStr(string input)
        {
            string randkey = new Random(new Random().Next(100)).Next(0, 100).ToString();
            return new string[] { XorStr(randkey, input), randkey };
        }
        public static string EncryptStrWithKey(string input, string key)
        {
            return XorStr(key, input);
        }
        public static string DecryptStr(string input, string pass)
        {
            return XorStr(pass, input);
        }
    }
}