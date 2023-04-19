using System.Collections;
using System.Net;
using System.Text;

namespace FlaskeAutomaten
{
    internal class Program
    {
        public static Queue<Beer> beerQ = new();
        public static Queue<Soda> sodaQ = new();
        public static Queue<Drink> drinkQ = new();
        static void Main()
        {

            Thread producerThread = new(Produce);
            Thread splitterConsumerThread = new(SplitterConsumer);
            Thread consumerBeerThread = new(Produce);
            Thread consumerSodaThread = new(Produce);
            Thread printerThread = new(PrintLayout);

            producerThread.Start();
            splitterConsumerThread.Start();
            printerThread.Start();


            producerThread.Join();
            splitterConsumerThread.Join();
            printerThread.Join();
            Console.Read();



        }
        static void PrintLayout()
        {
            while (true)
            {
                try
                {
                    Console.Clear();

                    Monitor.Enter(drinkQ);
                    Monitor.Enter(sodaQ);
                    Monitor.Enter(beerQ);
                }
                finally
                {

                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine($"Drinks waiting to be sorted: {drinkQ.Count}");
                    sb.AppendLine($"Soda bottles: {sodaQ.Count}");
                    sb.AppendLine($"Beer bottles: {beerQ.Count}");
                    Console.WriteLine(sb);

                    Monitor.Exit(drinkQ);
                    Monitor.Exit(sodaQ);
                    Monitor.Exit(beerQ);
                    Thread.Sleep(200);
                }
            }
        }
        static void Produce()
        {
            Random random = new();
            int randomNum;

            while (true)
            {
                try
                {
                    Monitor.Enter(drinkQ);
                    if (drinkQ.Count < 5)
                    {
                        while (drinkQ.Count < 35)
                        {
                            randomNum = random.Next(1, 3);

                            if (randomNum == 1)
                            {
                                drinkQ.Enqueue(new Beer());
                            }
                            else
                            {
                                drinkQ.Enqueue(new Soda());
                            }
                        }
                        Monitor.PulseAll(drinkQ);
                    }
                    else
                    {

                    }
                }
                finally
                {
                    Monitor.Exit(drinkQ);
                    Thread.Sleep(200);
                }
            }
        }

        static void SplitterConsumer()
        {
            Drink drink;
            while (true)
            {
                try
                {
                    if (Monitor.TryEnter(drinkQ))
                    {
                        if (drinkQ.Count == 0)
                        {
                            Monitor.Wait(drinkQ);
                        }
                        else
                        {
                            drink = drinkQ.Dequeue();
                            if (drink is Soda)
                            {
                                sodaQ.Enqueue(((Soda)drink));
                            }
                            else if (drink is Beer)
                            {
                                beerQ.Enqueue(((Beer)drink));
                            }
                            else
                            {
                                Console.WriteLine("Error");
                            }
                        }
                        Monitor.Exit(drinkQ);
                    }
                }
                finally
                {
                    Thread.Sleep(200);
                }

            }
        }
        static void DisposeBottle()
        {
            // TODO: Dispose sodaQ and beerQ when above certain amount
            while (sodaQ.Count != 0)
            {

            }
        }
    }
}