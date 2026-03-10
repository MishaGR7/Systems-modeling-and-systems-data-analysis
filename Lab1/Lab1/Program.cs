using System;
using System.Collections.Generic;

namespace ConsoleAppMonteCarlo
{
    public struct PointF
    {
        public float X { get; set; }
        public float Y { get; set; }

        public PointF(float x, float y)
        {
            X = x;
            Y = y;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            MonteCarloArea monteCarlo = new MonteCarloArea();

            float xmin = 0.1f;
            float xmax = (float)Math.PI / 2 - 0.1f;
            float ymin = 0;
            float ymax = 3;

            Console.WriteLine("Моделювання методом Монте-Карло");
            Console.Write("Введіть кількість випробувань (точок): ");

            string point = Console.ReadLine();

            if (int.TryParse(point, out int pointCount))
            {
                monteCarlo.GeneratePoints(pointCount, xmin, xmax, ymin, ymax);
                double area = monteCarlo.CalculateArea(xmin, xmax, ymin, ymax);

                Console.WriteLine($"\nОцінена площа фігури: {area}");
            }
            else
            {
                Console.WriteLine("Будь ласка, введіть коректне ціле число для кількості точок.");
            }

            Console.WriteLine("\nНатисніть будь-яку клавішу для завершення...");
            Console.ReadKey();
        }
    }

    public class MonteCarloArea
    {
        private List<PointF> points = new List<PointF>();

        public void GeneratePoints(int pointCount, float xmin, float xmax, float ymin, float ymax)
        {
            Random random = new Random();
            for (int i = 0; i < pointCount; i++)
            {
                float x = (float)(random.NextDouble() * (xmax - xmin) + xmin);
                float y = (float)(random.NextDouble() * (ymax - ymin) + ymin);
                points.Add(new PointF(x, y));
            }
        }

        public double CalculateArea(float xmin, float xmax, float ymin, float ymax)
        {
            int pointsInside = 0;
            foreach (PointF point in points)
            {
                if (IsInsideFigure(point.X, point.Y))
                {
                    pointsInside++;
                }
            }
            double rectangleArea = (xmax - xmin) * (ymax - ymin);

            if (points.Count == 0) return 0;

            return (double)pointsInside / points.Count * rectangleArea;
        }

        private bool IsInsideFigure(float x, float y)
        {
            return y <= 3 && y <= Math.Tan(x) && y <= 1 / x;
        }

        public List<PointF> GetPoints()
        {
            return points;
        }
    }
}