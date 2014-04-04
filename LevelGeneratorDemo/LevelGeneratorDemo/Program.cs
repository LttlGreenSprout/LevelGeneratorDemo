using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelGeneratorDemo
{
    enum PrintType { Int, Char };
    class Program
    {
        static void Main(string[] args)
        {
            LevelManager levelManager = new LevelManager();
            Random r = new Random();
            levelManager.Seed = 0;

            char[,] level = levelManager.GenerateLevel(3);
            levelManager.SaveCharMatrixToTextFile(level, "FuckThisShit.txt");

            /**
            levelManager.Seed = 0;
            do
            {
                Console.Clear();
                int[,] chunkLayer1 = levelManager.GenerateIntLayer(true);
                int[,] chunkLayer2 = levelManager.GenerateIntLayer(true);
                int[,] chunkLayer3 = levelManager.GenerateIntLayer(true);
                WriteLevel(chunkLayer1, PrintType.Char);
                WriteLevel(chunkLayer2, PrintType.Char);
                WriteLevel(chunkLayer3, PrintType.Char);
            } while (Console.ReadKey(true).Key == ConsoleKey.R);
            /**/
            Console.ReadLine();
            

            
        }
        private static void WriteLevel(int[,] layerLevel, PrintType type)
        {
            for (int r = 0; r < layerLevel.GetLength(0); r++)
            {
                for (int c = 0; c < layerLevel.GetLength(1); c++)
                {
                    int i = (int)layerLevel[r, c];
                    switch (type)
                    {
                        case PrintType.Int:
                            if (i < 0)
                                i = 8;
                            Console.Write(i);
                            break;
                        case PrintType.Char:
                            char chr = 'x';
                            switch (i)
                            {
                                case 0: chr = '.'; break;
                                case 1: chr = '='; break;
                                case 2: chr = '|'; break;
                                case 3: chr = '#'; break;
                                default: chr = '?'; break;
                            }
                            Console.Write(chr);
                            break;
                    }
                }
                Console.WriteLine();
            }
        }
    }
}
