using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            
            ITripletService tripletService = new SimpleTripletService();
            Execute(tripletService);

            ITripletService tripletService1 = new MultiThreadTripletService();
            Execute(tripletService1);

            ITripletService tripletService2 = new ParallelTaskTripletService();
            Execute(tripletService2);

            Console.ReadKey();

        }

        private static void Execute(ITripletService tripletService)
        {
            Console.WriteLine("-------------");
            Console.WriteLine(tripletService.GetType().Name);
            Stopwatch timer = new Stopwatch();
            
            timer.Start();
            var results = tripletService.GetTop10Triplets("TEST.txt");
            timer.Stop();

            Console.WriteLine(string.Join(";", results));
            Console.WriteLine($"Lead time: { timer.ElapsedMilliseconds}");
            Console.WriteLine("-------------");
        }
    }
}
