using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab6
{
    public class Request
    {
        public int RequestNumber { get; set; }
        public DateTime ArriveTime { get; set; }
        public TimeSpan ServiceTime { get; set; }
        public DateTime StartServiceTime { get; set; }
        public DateTime EndServiceTime { get; set; }
        public TimeSpan TimeInSystem { get; set; }
        public TimeSpan TimeInQueue { get; set; }
        public int ClientNumber { get; set; }
        public int ClientInService { get; set; }
        public int QueueLength { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("Імітаційне моделювання СМО (Варіант 23)");
            Console.Write("Введіть кількість заявок для моделі: ");

            if (int.TryParse(Console.ReadLine(), out int numberOfRequests) && numberOfRequests > 0)
            {
                RunSimulation(numberOfRequests);
            }
            else
            {
                Console.WriteLine("Будь ласка, введіть коректне ціле число більше нуля.");
            }

            Console.WriteLine("\nНатисніть будь-яку клавішу для завершення...");
            Console.ReadKey();
        }

        static void RunSimulation(int numberOfRequests)
        {
            DateTime startTime = DateTime.Today.AddHours(8);
            List<Request> requests = new List<Request>();
            Random random = new Random();

            DateTime currentTime = startTime;
            DateTime currentServiceEndTime = startTime;

            for (int i = 0; i < numberOfRequests; i++)
            {
                int intervalMinutes = random.Next(1, 33);
                int serviceMinutes = random.Next(1, 18);

                currentTime = currentTime.AddMinutes(intervalMinutes);
                TimeSpan serviceTime = TimeSpan.FromMinutes(serviceMinutes);

                DateTime startServiceTime = (currentTime > currentServiceEndTime) ? currentTime : currentServiceEndTime;
                DateTime endServiceTime = startServiceTime.Add(serviceTime);

                TimeSpan timeInSystem = endServiceTime - currentTime;
                TimeSpan timeInQueue = startServiceTime - currentTime;

                int queueLength = requests.Count(r => r.EndServiceTime > currentTime);
                int clientInService = requests.LastOrDefault(r => r.EndServiceTime <= currentTime)?.RequestNumber ?? 0;

                requests.Add(new Request
                {
                    RequestNumber = i + 1,
                    ArriveTime = currentTime,
                    ServiceTime = serviceTime,
                    StartServiceTime = startServiceTime,
                    EndServiceTime = endServiceTime,
                    TimeInSystem = timeInSystem,
                    TimeInQueue = timeInQueue,
                    ClientNumber = i + 1,
                    ClientInService = clientInService,
                    QueueLength = queueLength
                });

                currentServiceEndTime = endServiceTime;
            }

            PrintResults(requests);
        }

        static void PrintResults(List<Request> requests)
        {
            Console.WriteLine("\nРезультати симуляції:");
            Console.WriteLine(new string('-', 135));
            Console.WriteLine($"| {"№",-3} | {"Час надходж.",-16} | {"Обслугов.",-9} | {"Початок обс.",-16} | {"Кінець обс.",-16} | {"У системі",-9} | {"У черзі",-8} | {"Клієнт",-6} | {"Обслуг-ся",-9} | {"Довж. черги",-11} |");
            Console.WriteLine(new string('-', 135));

            foreach (var r in requests)
            {
                Console.WriteLine($"| {r.RequestNumber,-3} | {r.ArriveTime:dd/MM/yyyy HH:mm} | {r.ServiceTime:hh\\:mm\\:ss} | {r.StartServiceTime:dd/MM/yyyy HH:mm} | {r.EndServiceTime:dd/MM/yyyy HH:mm} | {r.TimeInSystem:hh\\:mm\\:ss} | {r.TimeInQueue:hh\\:mm\\:ss} | {r.ClientNumber,-6} | {r.ClientInService,-9} | {r.QueueLength,-11} |");
            }
            Console.WriteLine(new string('-', 135));
        }
    }
}