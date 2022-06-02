using System;
using System.Threading;
using System.Threading.Tasks;

namespace Flow
{
    class Program
    {
        static double j = 1;
        static void Method(object cancel, string textMethod)
        {
            var token = (CancellationToken)cancel;

            Random rand = new Random();
            int y = rand.Next(200, 1000);
            Console.WriteLine($"{textMethod} {Task.CurrentId}");
            for (int i = 1; i < 10; i++)
            {
                j *= i;
                if (token.IsCancellationRequested)
                {
                    Console.WriteLine("Получен запрос на отмену задачи.");
                    throw new OperationCanceledException();
                }
                //Task.Delay(y);
                Thread.Sleep(y);
            }
            Console.WriteLine(j);            
        }
        static void Main(string[] args)
        {
            Console.WriteLine("ESC - выход");

            var cancelTokSrc = new CancellationTokenSource();
            Task.Run(() =>
                {
                    Method(cancelTokSrc.Token, "Первый пошел");
                });
            Task.Run(() =>
            {
                Method(cancelTokSrc.Token, "Второй пошел");
            });
            Task.Run(() =>
            {
                Method(cancelTokSrc.Token, "Третий пошел");
            });
            Task.Run(() =>
            {
                Method(cancelTokSrc.Token, "Четвертый пошел");
            });
            Task.Run(() =>
            {
                Method(cancelTokSrc.Token, "Пятый пошел");
            });
            while (true)
            {
                if(Console.ReadKey().Key == ConsoleKey.Escape)
                {
                    cancelTokSrc.Cancel();
                    break;
                }                
            }  
        }
    }
}
