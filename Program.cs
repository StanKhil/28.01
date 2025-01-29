using System;
using System.IO;
using System.Linq;
using System.Threading;

class Program
{
    static Mutex mutex1 = new Mutex();
    static Mutex mutex2 = new Mutex();

    static void Main()
    {
        Thread thread1 = new Thread(GenerateNumbers);
        Thread thread2 = new Thread(FilterPrimes);
        Thread thread3 = new Thread(FilterPrimesEnding);

        thread1.Start();
        thread2.Start();
        thread3.Start();

        thread1.Join();
        thread2.Join();
        thread3.Join();

        Console.WriteLine("Обробка завершена!");
    }

    static void GenerateNumbers()
    {
        mutex1.WaitOne();
        Random rand = new Random();
        using (StreamWriter writer = new StreamWriter("numbers.txt"))
        {
            for (int i = 0; i < 100; i++)
            {
                int num = rand.Next(1, 1000);
                writer.WriteLine(num);
            }
        }

        Console.WriteLine("Перший потік: Числа згенеровано.");
        mutex1.ReleaseMutex();
    }

    static void FilterPrimes()
    {
        mutex1.WaitOne();
        var numbers = File.ReadAllLines("numbers.txt").Select(int.Parse);
        using (StreamWriter writer = new StreamWriter("primes.txt"))
        {
            foreach (var num in numbers)
            {
                if (IsPrime(num))
                    writer.WriteLine(num);
            }
        }
        Console.WriteLine("Другий потік: Прості числа записані.");
        mutex1.ReleaseMutex();
    }

    static void FilterPrimesEnding()
    {
        mutex2.WaitOne();

        var primes = File.ReadAllLines("primes.txt").Select(int.Parse);
        using (StreamWriter writer = new StreamWriter("primes_7.txt"))
        {
            foreach (var num in primes)
            {
                if (num % 10 == 7)
                    writer.WriteLine(num);
            }
        }

        Console.WriteLine("Третій потік: Фільтровані прості числа записані.");
        mutex2.ReleaseMutex();
    }

    static bool IsPrime(int num)
    {
        if (num < 2) return false;
        for (int i = 2; i * i <= num; i++)
            if (num % i == 0) return false;
        return true;
    }
}
