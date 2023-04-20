using System.Collections;
using System.Net;
using System.Text;

namespace FlaskeAutomaten
{
    internal class Program
    {
        // Queues
        public static Queue<Drink> drinkQ = new();
        public static Queue<Beer> beerQ = new();
        public static Queue<Soda> sodaQ = new();

        static void Main()
        {
            
            Thread producerThread = new(Produce);
            Thread splitterConsumerThread = new(SplitterConsumer);
            Thread consumerDrinkThread = new(DisposeBottle);

            producerThread.Start();
            splitterConsumerThread.Start();
            consumerDrinkThread.Start();


            producerThread.Join();
            splitterConsumerThread.Join();


            Console.Read();
        }

        /// <summary>
        /// Produces drink bottles (beer or soda) and adds them to a shared queue (drinkQ).
        /// It uses a random number generator to decide whether to add a beer or soda bottle.
        /// </summary>
        static void Produce()
        {
            Random random = new();
            int randomNum;
            string writeProduceString = "";
            while (true)
            {
                try
                {
                    Monitor.Enter(drinkQ);
                    Console.Clear();
                    if (drinkQ.Count < 3)
                    {
                        while (drinkQ.Count < 20)
                        {
                            writeProduceString = "";
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
                        writeProduceString = "[Info] Bottle producer waits";
                    }
                }
                finally
                {
                    Monitor.Exit(drinkQ);
                    Console.WriteLine($"Unsorted bottles: {drinkQ.Count}");
                    Console.WriteLine($"Soda bottles: {sodaQ.Count}");
                    Console.WriteLine($"Beer bottles: {beerQ.Count}");
                    Console.WriteLine($"{writeProduceString}");
                    Thread.Sleep(500);
                }
            }
        }

        /// <summary>
        /// Consumes drink bottles from a shared queue (drinkQ) and adds them to their respective drink type queues (sodaQ or beerQ).
        /// If drinkQ is empty, the method waits until it is notified
        /// </summary>
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

                            // Check drink type
                            drink = drinkQ.Dequeue();

                            if (drink.Name == "soda")
                            {
                                sodaQ.Enqueue((Soda)drink);
                            }
                            else if (drink.Name == "beer")
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
                    Thread.Sleep(500);
                }

            }
        }

        /// <summary>
        /// It uses a while loop to continually check if there are at least 10 bottles in the drink type queue.
        /// If there are, it dequeues 10 bottles and disposes of them.
        /// </summary>
        static void DisposeBottle()
        {
            
            while (true)
            {
                try
                {
                    if (Monitor.TryEnter(sodaQ))
                    {
                        // start disposing soda
                        if (sodaQ.Count >= 10)
                        {
                            Console.WriteLine("[Info] Disposing soda");
                            for (int i = 0; i < 10; i++)
                            {
                                sodaQ.Dequeue();
                            }
                        }
                        Monitor.Exit(sodaQ);
                    }
                    if (Monitor.TryEnter(beerQ))
                    {
                        if (beerQ.Count >= 10)
                        {
                            Console.WriteLine("[Info] Disposing beer");
                            for (int i = 0; i < 10; i++)
                            {
                                beerQ.Dequeue();
                            }
                        }
                        Monitor.Exit(beerQ);
                    }
                }
                finally
                {
                    Thread.Sleep(500);
                }
            }
        }
    }
}