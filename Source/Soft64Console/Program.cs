using System;
using Soft64;

namespace Soft64Console
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Machine machine = new Machine();

            Console.Clear();

            /* Main Bar */
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("Soft64 Emulator ^^^                 ");

            Console.ReadLine();
        }
    }
}