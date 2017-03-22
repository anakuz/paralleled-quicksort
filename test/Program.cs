using System;
using System.Diagnostics;
using System.Linq;

namespace test
{
    class Program
    {
        static void Main()
        {
            while (true)
            {
                Run();
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                Console.Clear();
            }
        }

        static void Run()
        {
            //int n;
            Console.WriteLine("Set array size:");
            int n = Convert.ToInt32(Console.ReadLine());

            int[] a = new int[n];
            var rng = new Random(937525);

            for (int i = 0; i < n; ++i)
            {
                a[i] = rng.Next();
            }

            var b = a.ToArray();
            var d = a.ToArray();

            var sw = new Stopwatch();

            sw.Restart();
            var c = a.OrderBy(x => x).ToArray(); // Need ToArray(), otherwise it does nothing.
            Console.WriteLine("\nLINQ OrderBy() took " + sw.Elapsed);

            sw.Restart();
            a.AsParallel().OrderBy(x => x).ToArray();
            Console.WriteLine("\nPLINQ OrderBy() took " + sw.Elapsed);

            sw.Restart();
            Array.Sort(d);
            Console.WriteLine("\nArray.Sort() took " + sw.Elapsed);

            Console.WriteLine("\nIs logging enabled? 1 for y/2 for n");
            int answer = Convert.ToInt32(Console.ReadKey().KeyChar - '0');
            switch (answer)
            {
                case 1:
                    QuickSort.IsLoggingEnabled = true;
                    break;
                case 2:
                    QuickSort.IsLoggingEnabled = false;
                    break;
                default:
                    throw new Exception();
            }

            sw.Restart();
            QuickSort.Sort(a, 0, a.Length - 1);
            Console.WriteLine("\nSequential took " + sw.Elapsed);

            sw.Restart();
            QuickSortParallel.Sort_(b);
            Console.WriteLine("\nParallel took " + sw.Elapsed);

            Trace.Assert(a.SequenceEqual(c));
            Trace.Assert(b.SequenceEqual(c));

            Console.ReadKey();
        }
    }
}
