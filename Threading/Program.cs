using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace Threading
{

    class Person
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string BirthCity { get; set; }
        public string BirthState { get; set; }
        public int Age { get; set; }

        public Person(string f, string m, string l, string bc, string bs, int age)
        {
            FirstName = f;
            MiddleName = m;
            LastName = l;
            BirthCity = bc;
            BirthState = bs;
            Age = age;
        }
    }

    class Program
    {
        public bool messageDisplayed = false;
        public object locker = new object();
        public string name = string.Empty;
        public int Counter { get; set; } = 0;

        static void Main(string[] args)
        {

            PrintFooBard();

            Console.WriteLine(DownloadHtml(@"http://www.albahari.com/threading/"));

            Program programInstance = new Program();

            Action<int> someNewAction = new Action<int>(programInstance.PrintNumberToConsole);

            someNewAction(1234567890);

            Func<int, int, int> someNewFunc = new Func<int, int, int>(programInstance.Sum);

            Console.WriteLine($"Sum(199, 1) = {someNewFunc(199, 1)}.");

            var employees = programInstance.GetAllEmployees();

            string middleName = string.Empty;

            lock (programInstance.locker)
            {
                Console.Write("Enter middle name of person to find: ");

                middleName = Console.ReadLine();

            }

            Predicate<Person> findPerson = (person=>person.MiddleName==middleName);

            foreach (var person in employees)
            {
                if (findPerson(person))
                {
                    programInstance.PrintPerson(person);
                }
            }

            for (int i = 0; i < 10; i++)
            {
                programInstance.PrintCounter();
                programInstance.IncrementCounter();
            }

            Task.Factory.StartNew(() => programInstance.PrintAndIncrementCounterNTimes(10));

            Func<string> method = programInstance.DoWork;

            IAsyncResult startMethod = method.BeginInvoke(null, null);

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

            lock (programInstance.locker)
            {
                Console.WriteLine($"{t1.Name} finished.");
            }

            t2.Join();

            lock (programInstance.locker)
            {
                Console.WriteLine($"{t2.Name} finished.");
            }

            t3.Join();

            lock (programInstance.locker)
            {
                Console.WriteLine($"{t3.Name} finished.");
            }

            task1.Wait();

            task2.Wait();

            Console.ReadLine();
        }



        public static void PrintFooBard()
        {
            List<Thread> threads = new List<Thread>();
            for (int i = 0; i < 5; i++)
            {
                Thread t = new Thread(new ParameterizedThreadStart(PrintNumber));
                t.Start(i);
                threads.Add(t);
            }

            foreach (var t in threads)
            {
                t.Join();
            }
        }

        public void PrintPerson(Person person)
        {
            lock (locker)
            {
                Console.WriteLine("Person information: ");
                Console.WriteLine($"First name: {person.FirstName}.");
                Console.WriteLine($"Middle name: {person.MiddleName}.");
                Console.WriteLine($"Last name: {person.LastName}.");
                Console.WriteLine($"Birth city: {person.BirthCity}.");
                Console.WriteLine($"Birth state: {person.BirthState}.");
                Console.WriteLine($"Age: {person.Age}.");
            }
        }

        public static string DownloadHtml(string websiteUrl)
        {
            using (var webClient = new System.Net.WebClient())
            {
                return webClient.DownloadString(websiteUrl);
            }
        }

        public static void PrintNumber(object number)
        {
            Console.WriteLine($"I Foobar'd {(int)number}.");
        }

        public static void PrintHelloWorld()
        {
            Console.WriteLine("Hello World");
        }

        public List<Person> GetAllEmployees()
        {
            return new List<Person>()
            {
                new Person("Francisco", "Elena","Solano", "Las Vegas", "NV", 23),
                new Person("Jane", "Middle", "Doe", "NC", "State1", 10),
                new Person("First1", "Middle2", "Last1", "BC2", "BS1", 11),
                new Person("First2", "Middle3", "Last2", "BC3", "BS2", 12)
            };
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
            lock (locker)
            {
                Console.Write("Enter your name: ");
                name = Console.ReadLine();
                Console.WriteLine($"Your name is {name}.");
            }
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

        public void PrintCounter()
        {
            Console.WriteLine($"Counter = {Counter}.");
        }

        public void IncrementCounter()
        {
            Counter += 1;
        }

        public void PrintAndIncrementCounterNTimes(int numberOfTimes)
        {
            for (int i = 0; i < numberOfTimes; i++)
            {
                PrintCounter();
                IncrementCounter();
            }
        }

        public void PrintNumberToConsole(int number)
        {
            Console.WriteLine(number);
        }

    }
}
