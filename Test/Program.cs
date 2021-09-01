using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = string.Empty;
            if(args.Length == 0)
            {
                Console.WriteLine("Enter a path:");
                path = Console.ReadLine();
            }
            else
            {
                path = args[0];
            }

            ITripletService tripletService = new SimpleTripletService();
            Execute(tripletService,path);

            ITripletService tripletService1 = new MultiThreadTripletService();
            Execute(tripletService1,path);

            ITripletService tripletService2 = new ParallelTaskTripletService();
            Execute(tripletService2,path);

            Console.ReadKey();

        }

        private static void Execute(ITripletService tripletService, string path)
        {
            Console.WriteLine("-------------");
            Console.WriteLine(tripletService.GetType().Name);
            Stopwatch timer = new Stopwatch();
            
            timer.Start();
            var results = tripletService.GetTop10Triplets(path);
            timer.Stop();

            Console.WriteLine(string.Join(";", results));
            Console.WriteLine($"Lead time: { timer.ElapsedMilliseconds}");
            Console.WriteLine("-------------");
        }
    }
}
