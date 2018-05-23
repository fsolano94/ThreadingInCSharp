using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Threading
{
    class Program
    {
        public bool messageDisplayed = false;
        public object locker = new object();

        static void Main(string[] args)
        {
            Program programInstance = new Program();

            Thread t1 = new Thread(()=> programInstance.DisplayMessage($"Learning Threading using C#. From Thread1."));
            t1.Name = "Thread 1";
            Thread t2 = new Thread(() => programInstance.DisplayMessage("Learning Threading using C#. From Thread 2."));
            t2.Name = "Thread 2";
            t1.Start();
            t2.Start();

            t1.Join();
            Console.WriteLine($"{t1.Name} finished.");

            t2.Join();
            Console.WriteLine($"{t2.Name} finished.");

            Console.ReadLine();
        }

        public  void DisplayMessage(string message)
        {
            lock(locker)
            {
                if (!messageDisplayed)
                {
                    Console.WriteLine(message);
                    messageDisplayed = true;
                }
            }
            
        }

    }
}
