using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace Threading
{
    class Program
    {
        public bool messageDisplayed = false;
        public object locker = new object();
        public string name = string.Empty;

        static void Main(string[] args)
        {

            Program programInstance = new Program();

            Func<string> method = programInstance.DoWork;

            IAsyncResult startMethod = method.BeginInvoke(null,null);

            var startMethodResult = method.EndInvoke(startMethod);

            Console.WriteLine($"Result from DoWork: {startMethodResult}.");

            var task1 = Task.Factory.StartNew(PrintHelloWorld); // this is a thread from the thread pool

            Task<int> task2 = Task.Factory.StartNew(() => programInstance.Sum(10, 5));

            ThreadPool.QueueUserWorkItem(programInstance.CastObjectToStringAndPrintIt);

            ThreadPool.QueueUserWorkItem(programInstance.CastObjectToStringAndPrintIt, "This is some message.");

            Console.WriteLine($"Sum = {task2.Result}.");

            Thread t1 = new Thread(() => programInstance.DisplayMessage($"Learning Threading using C#. From Thread1."));

            t1.Name = "Thread 1";

            Thread t2 = new Thread(() => programInstance.DisplayMessage("Learning Threading using C#. From Thread 2."));

            t2.Name = "Thread 2";

            Thread t3 = new Thread(programInstance.DisplayName);
            t3.Name = "Thead 3";

            t1.Start();

            t2.Start();

            t3.Start();

            t1.Join();
            Console.WriteLine($"{t1.Name} finished.");

            t2.Join();
            Console.WriteLine($"{t2.Name} finished.");

            t3.Join();
            Console.WriteLine($"{t3.Name} finished.");

            task1.Wait();

            task2.Wait();

            Console.ReadLine();
        }

        public static void PrintHelloWorld()
        {
            Console.WriteLine("Hello World");
        }

        public void DisplayMessage(string message)
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

        public void DisplayName()
        {
            Console.Write("Enter your name: ");
            name = Console.ReadLine(); 
            Console.WriteLine($"Your name is {name}.");
        }
        
        public int Sum(int num1, int num2)
        {
            return num1 + num2;
        }

        public void CastObjectToStringAndPrintIt(object message)
        {
            Console.WriteLine((string)message);
        }

        public string DoWork()
        {
            Console.WriteLine("Working ...");
            Thread.Sleep(3000);
            Console.WriteLine("Work completed.");
            return "success";
        }
    }
}
