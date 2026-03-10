using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleAppMSSA8
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Лабораторна робота №8 - Ймовірнісне моделювання");
                Console.WriteLine("1. Завдання 1 (Визначення типу СМО)");
                Console.WriteLine("2. Завдання 2 (Методи підінтервалів та циклів)");
                Console.WriteLine("0. Вихід");
                Console.Write("\nОберіть завдання: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": RunTask1(); break;
                    case "2": RunTask2(); break;
                    case "0": return;
                    default:
                        Console.WriteLine("Невірний вибір. Натисніть будь-яку клавішу...");
                        Console.ReadKey();
                        break;
                }
            }
        }



        static void RunTask1()
        {
            Console.Clear();
            Console.WriteLine("--- Завдання 1: Визначення типу СМО ---");
            Console.Write("Введіть кількість заявок: ");

            if (!int.TryParse(Console.ReadLine(), out int numRequests) || numRequests < 2)
            {
                Console.WriteLine("Будь ласка, введіть число більше або дорівнює 2.");
                Console.ReadKey();
                return;
            }

            DateTime startTime = DateTime.Today.AddHours(8);
            Random random = new Random();
            List<Request1> requests = new List<Request1>();
            DateTime currentArrivalTime = startTime;

            for (int i = 0; i < numRequests; i++)
            {
                TimeSpan serviceTime = TimeSpan.FromMinutes(random.Next(10, 31));
                currentArrivalTime = currentArrivalTime.AddMinutes(23);
                DateTime departureTime = currentArrivalTime.AddMinutes(46);

                requests.Add(new Request1
                {
                    ArrivalTime = currentArrivalTime,
                    DepartureTime = departureTime,
                    ServiceDuration = TimeSpan.FromMinutes(serviceTime.TotalMinutes + 0.05 * 23),
                    IntervalBetweenRequests = TimeSpan.FromMinutes(23 + 0.1 * 23),
                    NumberOfRequests = random.Next(2, 10 + 23)
                });
            }

            var arrivalIntervals = requests.Select((r, i) => i == 0 ? 0 : (r.ArrivalTime - requests[i - 1].ArrivalTime).TotalMinutes).Skip(1).ToList();
            var serviceTimes = requests.Select(r => r.ServiceDuration.TotalMinutes).ToList();

            string resultArrivals = AnalyzeExponentialDistribution(arrivalIntervals, "Час між заявками");
            string resultService = AnalyzeExponentialDistribution(serviceTimes, "Час обслуговування");
            string smoType = DetermineSmoType(resultArrivals, resultService);

            Console.WriteLine($"\nТип СМО: {smoType}");
            Console.WriteLine(resultArrivals);
            Console.WriteLine(resultService);

            Console.WriteLine("\nНатисніть будь-яку клавішу для повернення в меню...");
            Console.ReadKey();
        }

        static string AnalyzeExponentialDistribution(List<double> data, string label)
        {
            double mean = data.Average();
            double variance = data.Select(d => Math.Pow(d - mean, 2)).Average();
            double standardDeviation = Math.Sqrt(variance);
            string distributionType = Math.Abs(mean - standardDeviation) < 0.1 ? "Експоненціальний" : "Не експоненціальний";
            return $"{label}: {distributionType} (Середнє = {mean:F2}, Стандартне відхилення = {standardDeviation:F2})";
        }

        static string DetermineSmoType(string resultArrivals, string resultService)
        {
            if (resultArrivals.Contains("Експоненціальний") && resultService.Contains("Експоненціальний")) return "M/M/1";
            if (resultArrivals.Contains("Експоненціальний") && resultService.Contains("Не експоненціальний")) return "M/G/1";
            if (resultArrivals.Contains("Не експоненціальний") && resultService.Contains("Експоненціальний")) return "G/M/1";
            return "Інший тип СМО";
        }



        static void RunTask2()
        {
            Console.Clear();
            Console.WriteLine("--- Завдання 2: Методи підінтервалів та циклів ---");
            Console.Write("Введіть кількість заявок: ");

            if (!int.TryParse(Console.ReadLine(), out int numRequests) || numRequests < 1)
            {
                Console.WriteLine("Будь ласка, введіть число більше нуля.");
                Console.ReadKey();
                return;
            }

            DateTime startTime = DateTime.Today.AddHours(8);
            Random random = new Random();
            List<Request2> requests = new List<Request2>();
            DateTime currentArrivalTime = startTime;

            for (int i = 0; i < numRequests; i++)
            {
                TimeSpan serviceTime = TimeSpan.FromMinutes(random.Next(10, 31));
                currentArrivalTime = currentArrivalTime.AddMinutes(random.Next(5, 20));
                DateTime departureTime = currentArrivalTime.Add(serviceTime);

                requests.Add(new Request2
                {
                    ArrivalTime = currentArrivalTime,
                    ServiceDuration = serviceTime,
                    DepartureTime = departureTime,
                });
            }

            for (int index = 0; index < requests.Count; index++)
            {
                var request = requests[index];
                if (index == 0)
                {
                    request.QueueTime = TimeSpan.Zero;
                }
                else
                {
                    var previousRequest = requests[index - 1];
                    request.QueueTime = previousRequest.DepartureTime > request.ArrivalTime
                        ? previousRequest.DepartureTime - request.ArrivalTime
                        : TimeSpan.Zero;
                }
                request.SystemTime = request.ServiceDuration + request.QueueTime;
                request.DepartureTime = request.ArrivalTime + request.QueueTime + request.ServiceDuration;
            }

            Console.WriteLine("\nЗагальні результати:");
            foreach (var r in requests)
            {
                Console.WriteLine($"Заявка: Час надходження = {r.ArrivalTime:HH:mm}, Час обслуговування = {r.ServiceDuration.TotalMinutes:F2} хв, Час очікування = {r.QueueTime.TotalMinutes:F2} хв, Час у системі = {r.SystemTime.TotalMinutes:F2} хв, Час відбуття = {r.DepartureTime:HH:mm}");
            }

            Console.WriteLine("\nМетод підінтервалів (кожні 30 хв):");
            var simStartTime = requests.First().ArrivalTime;
            var simEndTime = requests.Last().DepartureTime;
            var currentInterval = simStartTime;

            while (currentInterval < simEndTime)
            {
                var nextInterval = currentInterval.AddMinutes(30);
                var requestsInInterval = requests.Where(r => r.ArrivalTime >= currentInterval && r.ArrivalTime < nextInterval).ToList();
                double avgSystemTime = requestsInInterval.Any() ? requestsInInterval.Average(r => r.SystemTime.TotalMinutes) : 0;

                Console.WriteLine($"Інтервал {currentInterval:HH:mm} - {nextInterval:HH:mm}: Середній час у системі = {avgSystemTime:F2} хв");
                currentInterval = nextInterval;
            }

            Console.WriteLine("\nРезультати за методом циклів:");
            DateTime? cycleStart = null;
            DateTime? cycleEnd = null;
            List<(DateTime Start, DateTime End)> cycles = new List<(DateTime, DateTime)>();

            foreach (var request in requests)
            {
                if (cycleStart == null) cycleStart = request.ArrivalTime;
                cycleEnd = request.DepartureTime;

                int currentIndex = requests.IndexOf(request);
                if (currentIndex < requests.Count - 1 && requests[currentIndex + 1].ArrivalTime > cycleEnd)
                {
                    cycles.Add((cycleStart.Value, cycleEnd.Value));
                    cycleStart = null;
                }
            }
            if (cycleStart != null && cycleEnd != null) cycles.Add((cycleStart.Value, cycleEnd.Value));

            foreach (var cycle in cycles)
            {
                var requestsInCycle = requests.Where(r => r.ArrivalTime >= cycle.Start && r.DepartureTime <= cycle.End).ToList();
                double avgSystemTime = requestsInCycle.Any() ? requestsInCycle.Average(r => r.SystemTime.TotalMinutes) : 0;
                Console.WriteLine($"Цикл {cycle.Start:HH:mm} - {cycle.End:HH:mm}: Середній час у системі = {avgSystemTime:F2} хв");
            }

            Console.WriteLine("\nНатисніть будь-яку клавішу для повернення в меню...");
            Console.ReadKey();
        }



        public class Request1
        {
            public DateTime ArrivalTime { get; set; }
            public DateTime DepartureTime { get; set; }
            public TimeSpan ServiceDuration { get; set; }
            public TimeSpan IntervalBetweenRequests { get; set; }
            public int NumberOfRequests { get; set; }
        }

        public class Request2
        {
            public DateTime ArrivalTime { get; set; }
            public TimeSpan ServiceDuration { get; set; }
            public TimeSpan QueueTime { get; set; }
            public TimeSpan SystemTime { get; set; }
            public DateTime DepartureTime { get; set; }
        }
    }
}