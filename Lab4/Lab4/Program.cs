using System;
using System.Collections.Generic;

namespace Lab4Console
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Лабораторна робота №4 - Моделювання СМО");
                Console.WriteLine("1. Завдання 1 (Відмова ланцюга)");
                Console.WriteLine("2. Завдання 2-3 (Пральні машини + Фінанси)");
                Console.WriteLine("3. Завдання 4 (Стратегії автобусів)");
                Console.WriteLine("0. Вихід");
                Console.Write("\nОберіть завдання: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": RunTask1(); break;
                    case "2": RunTask2And3(); break;
                    case "3": RunTask4(); break;
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
            Console.WriteLine("--- Завдання 1: Відмова ланцюга ---");
            Console.Write("Введіть кількість симуляцій (наприклад, 525): ");

            if (int.TryParse(Console.ReadLine(), out int num))
            {
                ChainFailureSimulation simulation = new ChainFailureSimulation();
                double failureProbability = simulation.SimulateChainFailure(num);
                Console.WriteLine($"\nЙмовірність відмови ланцюга після {num} симуляцій: {failureProbability:F2}%");
            }
            else
            {
                Console.WriteLine("Будь ласка, введіть коректне число.");
            }

            Console.WriteLine("\nНатисніть будь-яку клавішу для повернення в меню...");
            Console.ReadKey();
        }

        static void RunTask2And3()
        {
            Console.Clear();
            Console.WriteLine("--- Завдання 2-3: Пральні машини ---");
            WashingMachineSimulator simulator = new WashingMachineSimulator();
            simulator.RunSimulation();

            foreach (var day in simulator.Days)
            {
                Console.WriteLine($"День {day.DayNumber}: Попит {day.Demand}, Залишок: {day.StockAfterSale}");
                if (day.OrderedAmount > 0)
                    Console.WriteLine($"    Замовлено {day.OrderedAmount} машин.");
                if (day.ReceivedAmount > 0)
                    Console.WriteLine($"    Отримано {day.ReceivedAmount} машин.");
            }

            Console.WriteLine($"\nЙмовірність дефіциту: {simulator.GetDeficitProbability():0.00}%");
            Console.WriteLine("\nФінансові результати:");
            Console.WriteLine($"Загальний дохід: {simulator.GetTotalRevenue():0.00} грн");
            Console.WriteLine($"Загальні витрати на закупівлю: {simulator.GetTotalCost():0.00} грн");
            Console.WriteLine($"Загальні витрати на замовлення: {simulator.GetTotalOrderCost():0.00} грн");
            Console.WriteLine($"Загальні збитки через дефіцит: {simulator.GetTotalDeficitCost():0.00} грн");
            Console.WriteLine($"Чистий прибуток: {simulator.GetTotalProfit():0.00} грн");

            Console.WriteLine("\nНатисніть будь-яку клавішу для повернення в меню...");
            Console.ReadKey();
        }

        static void RunTask4()
        {
            Console.Clear();
            Console.WriteLine("--- Завдання 4: Стратегії обслуговування автобусів ---");
            BusSimulation simulation = new BusSimulation();

            Console.WriteLine("\nСТРАТЕГІЯ 1: ремонт одразу після погіршення");
            var result1 = simulation.SimulateStrategy1();
            Console.WriteLine(result1.log);
            Console.WriteLine($"Середній відсоток виконаних рейсів: {result1.averageCompletionRate:F2}%\n");

            Console.WriteLine("СТРАТЕГІЯ 2: продовження роботи в погіршеному стані");
            var result2 = simulation.SimulateStrategy2();
            Console.WriteLine(result2.log);
            Console.WriteLine($"Середній відсоток виконаних рейсів: {result2.averageCompletionRate:F2}%");

            Console.WriteLine("\nНатисніть будь-яку клавішу для повернення в меню...");
            Console.ReadKey();
        }
    }


    public class ChainFailureSimulation
    {
        private static Random random = new Random();
        private int[] failureProbabilities = { 5, 13, 14, 8, 10, 31 };

        public double SimulateChainFailure(int numberOfSimulations)
        {
            int failureCount = 0;
            for (int i = 0; i < numberOfSimulations; i++)
            {
                if (IsChainFailed()) failureCount++;
            }
            return (double)failureCount / numberOfSimulations * 100;
        }

        private bool IsChainFailed()
        {
            if (IsDeviceFailed(failureProbabilities[0])) return true;
            if (IsDeviceFailed(failureProbabilities[1]) && IsDeviceFailed(failureProbabilities[2])) return true;
            if (IsDeviceFailed(failureProbabilities[3]) && IsDeviceFailed(failureProbabilities[4])) return true;
            return IsDeviceFailed(failureProbabilities[5]);
        }

        private bool IsDeviceFailed(int failureProbability)
        {
            int randomValue = random.Next(0, 99);
            return randomValue < failureProbability;
        }
    }



    public class Day
    {
        public int DayNumber { get; set; }
        public int Demand { get; set; }
        public int StockBeforeSale { get; set; }
        public int StockAfterSale { get; set; }
        public int OrderedAmount { get; set; }
        public int ReceivedAmount { get; set; }

        public Day(int dayNumber) { DayNumber = dayNumber; }
    }

    public class WashingMachineSimulator
    {
        private static Random random = new Random();
        private const int DaysToSimulate = 19;
        private const int OrderCycle = 4;
        private const int ReorderLevel = 14;
        private const int OrderAmount = 19;
        private static int initialStock = 14;
        private static int deficitDays = 0;

        private const double RetailPrice = 9140;
        private const double PurchasePrice = 5065;
        private const double OrderCost = 1508;
        private const double DeficitCost = 4280;

        private static double totalRevenue = 0;
        private static double totalCost = 0;
        private static double totalDeficitCost = 0;
        private static double totalOrderCost = 0;

        public List<Day> Days { get; private set; }

        public WashingMachineSimulator() { Days = new List<Day>(); }

        public void RunSimulation()
        {
            Days.Clear();
            deficitDays = 0; totalRevenue = 0; totalCost = 0; totalDeficitCost = 0; totalOrderCost = 0;
            int stock = initialStock;

            for (int day = 1; day <= DaysToSimulate; day++)
            {
                Day currentDay = new Day(day);
                int demand = GenerateDemand();
                currentDay.Demand = demand;
                int stockBeforeSale = stock;

                if (stockBeforeSale >= demand)
                {
                    currentDay.StockAfterSale = stockBeforeSale - demand;
                    totalRevenue += demand * RetailPrice;
                }
                else
                {
                    currentDay.StockAfterSale = 0;
                    deficitDays++;
                    int unsatisfiedDemand = demand - stockBeforeSale;
                    totalRevenue += stockBeforeSale * RetailPrice;
                    totalDeficitCost += unsatisfiedDemand * DeficitCost;
                }

                if (currentDay.StockAfterSale <= ReorderLevel && day >= 2)
                {
                    if (day < 3 || Days[day - 2].OrderedAmount == 0)
                    {
                        currentDay.OrderedAmount = OrderAmount;
                        totalOrderCost += OrderCost;
                        totalCost += OrderAmount * PurchasePrice;
                    }
                }

                if (day >= OrderCycle && day - OrderCycle >= 0 && Days[day - OrderCycle].OrderedAmount == OrderAmount)
                {
                    currentDay.ReceivedAmount = OrderAmount;
                    currentDay.StockAfterSale += OrderAmount;
                }

                stock = currentDay.StockAfterSale;
                Days.Add(currentDay);
            }
        }

        private int GenerateDemand()
        {
            int x = random.Next(0, 100);
            if (x < 55.93) return 1;
            else if (x < 61.13) return 2;
            else if (x < 90.23) return 3;
            else if (x < 96.39) return 4;
            else if (x < 98.57) return 5;
            return 6;
        }

        public double GetTotalRevenue() => totalRevenue;
        public double GetTotalCost() => totalCost;
        public double GetTotalOrderCost() => totalOrderCost;
        public double GetTotalDeficitCost() => totalDeficitCost;
        public double GetTotalProfit() => totalRevenue - totalCost - totalOrderCost - totalDeficitCost;
        public double GetDeficitProbability() => (double)deficitDays / DaysToSimulate * 100;
    }



    public class BusSimulation
    {
        private static Random random = new Random();
        private const int FlightsPerDay = 14;
        private const int Days = 28;
        private const int DegradationProbability = 40;
        private const int BreakdownProbability = 15;

        public (double averageCompletionRate, string log) SimulateStrategy1()
        {
            int totalCompletedFlights = 0;
            string log = "";
            for (int day = 1; day <= Days; day++)
            {
                bool isBusDamaged = false;
                bool isRepairing = false;
                int completedFlights = 0;
                for (int flight = 1; flight <= FlightsPerDay; flight++)
                {
                    if (isRepairing)
                    {
                        log += $"День {day}, рейс {flight}: Автобус на ремонті, рейс пропущено.\n";
                        isRepairing = false;
                        continue;
                    }
                    if (!isBusDamaged && random.Next(100) < DegradationProbability)
                    {
                        isBusDamaged = true;
                        log += $"День {day}, рейс {flight}: Автобус погіршився.\n";
                        isRepairing = true;
                        isBusDamaged = false;
                        log += $"День {day}, рейс {flight}: Автобус іде на ремонт, наступний рейс пропущено.\n";
                    }
                    completedFlights++;
                }
                totalCompletedFlights += completedFlights;
                log += $"День {day}: Виконано рейсів {completedFlights}/{FlightsPerDay}\n";
            }
            double averageCompletionRate = (double)totalCompletedFlights / (Days * FlightsPerDay) * 100;
            return (averageCompletionRate, log);
        }

        public (double averageCompletionRate, string log) SimulateStrategy2()
        {
            int totalCompletedFlights = 0;
            string log = "";
            for (int day = 1; day <= Days; day++)
            {
                bool isBusDamaged = false;
                int completedFlights = 0;
                for (int flight = 1; flight <= FlightsPerDay; flight++)
                {
                    if (isBusDamaged && random.Next(100) < BreakdownProbability)
                    {
                        log += $"День {day}, рейс {flight}: Автобус зламався. Решта рейсів скасовані.\n";
                        break;
                    }
                    completedFlights++;
                    if (!isBusDamaged && random.Next(100) < DegradationProbability)
                    {
                        isBusDamaged = true;
                        log += $"День {day}, рейс {flight}: Автобус погіршився.\n";
                    }
                }
                totalCompletedFlights += completedFlights;
                log += $"День {day}: Виконано рейсів {completedFlights}/{FlightsPerDay}\n";
            }
            double averageCompletionRate = (double)totalCompletedFlights / (Days * FlightsPerDay) * 100;
            return (averageCompletionRate, log);
        }
    }
}