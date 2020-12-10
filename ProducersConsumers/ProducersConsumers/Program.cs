using System;
using System.Collections.Generic;
using System.Threading;

namespace ProducersConsumers
{
    internal class Program
    {
        static Queue<int> Queue = new Queue<int>(200);
        static Random Random = new Random();
        
        static ManualResetEvent producersMRE = new ManualResetEvent(true); //событие синхронизации потока
        static ManualResetEvent consumersMRE = new ManualResetEvent(true); //событие синхронизации потока

        private static bool producersSleep;
        private static bool consumersSleep;
        
        public static void Main(string[] args)
        {
            // создааются производители
            for(int i = 0; i < 3; i++)
            {
                Thread t = new Thread(Produce);
                t.Name = "Producer " + i;
                t.Start();
            }
            
            // создаются потребители
            for(int i = 0; i < 2; i++)
            {
                Thread t = new Thread(Consume);
                t.Name = "Consumer " + i;
                t.Start();
            }
            
            // бессконечный цикл для проверки нажатия кнопки
            while (true)
            {
                if (Console.ReadKey().Key == ConsoleKey.Q)
                {
                    Environment.Exit(0);
                }
            }
        }

        // проверка числа элементов в очереди
        public static void CheckQueue()
        {
            if (Queue.Count <= 80 && producersSleep)
            {
                Console.WriteLine("-----------------------------------------------");
                Console.WriteLine("Число элементов <= 80, производители проснулись");
                Console.WriteLine("-----------------------------------------------");
                producersMRE.Set(); // возобновление потоков - включает сигнал
                producersSleep = false;
            }
                
            if (Queue.Count >= 100 && !producersSleep)
            {
                producersSleep = true;
                Console.WriteLine("------------------------------------------");
                Console.WriteLine("Число элементов >= 100, производители спят");
                Console.WriteLine("------------------------------------------");
                producersMRE.Reset(); // остановка потоков - выключает сигнал
            }
            
            if (Queue.Count > 0 && consumersSleep)
            {
                Console.WriteLine("-------------------------------------------");
                Console.WriteLine("Число элементов > 0, потребители проснулись");
                Console.WriteLine("-------------------------------------------");
                consumersMRE.Set(); // возобновление потоков - включает сигнал
                consumersSleep = false;
            }
                
            if (Queue.Count <= 0 && !consumersSleep) 
            {
                consumersSleep = true;
                Console.WriteLine("--------------------------------------");
                Console.WriteLine("Число элементов == 0, потребители спят");
                Console.WriteLine("--------------------------------------");
                consumersMRE.Reset(); // остановка потоков - выключает сигнал
            }
        }
        
        // производство
        public static void Produce()
        {
            while (producersMRE.WaitOne()) // true - пока получает сигнал
            {
                int number = Random.Next(0, 200);
                
                if (Queue.Count < 100)
                {
                    Queue.Enqueue(number);
                    Console.WriteLine($"Производитель [{Thread.CurrentThread.Name}] добавил число {number}; Число элементов: {Queue.Count}"); 
                }
                CheckQueue();
                Thread.Sleep(100);
            }
        }

        // потребление
        public static void Consume()
        {
            while (consumersMRE.WaitOne()) // true - пока получает сигнал
            {
                if (Queue.Count > 0)
                {
                    Console.WriteLine($"Потребитель [{Thread.CurrentThread.Name}] забрал число {Queue.Dequeue()}; Число элементов: {Queue.Count}"); 
                }
                CheckQueue();
                Thread.Sleep(100);
            }
        }
    }
}