using System;
using System.Threading;
using System.Threading.Tasks;

namespace Flow
{
    class Program
    {
        static bool exit = false;
        static double j = 1;
        async static Task Method(object cancel, string textMethod)
        {
            var token = (CancellationToken)cancel;

            token.ThrowIfCancellationRequested();
            if (token.IsCancellationRequested)
            {
                Console.WriteLine("Получен запрос на отмену задачи.");
                token.ThrowIfCancellationRequested();
            }
            Random rand = new Random();
            int y = rand.Next(100, 2000);
            Console.WriteLine($"{textMethod} {y}");
            for (int i = 1; i < 20; i++)
            {
                j *= i;
                await Task.Delay(y);
            }
            Console.WriteLine(j);
            j = 1;
        }
        async static Task Main(string[] args)
        {
            Console.WriteLine("ESC - выход");
            
            var cancelTokSrc = new CancellationTokenSource();
            while (Console.ReadKey().Key != ConsoleKey.Escape)
            {
                Console.WriteLine("bu");
                await Task.Run(() =>
                {
                    Method(cancelTokSrc.Token, "Первый пошел");
                });
                await Task.Run(() =>
                {
                    Method(cancelTokSrc.Token, "Второй пошел");
                });
                await Task.Run(() =>
                {
                    Method(cancelTokSrc.Token, "Третий пошел");
                });
                await Task.Run(() =>
                {
                    Method(cancelTokSrc.Token, "Четвертый пошел");
                });
                await Task.Run(() =>
                {
                    Method(cancelTokSrc.Token, "Пятый пошел");
                });                
            }
            exit = true;
            while (exit)
            {
                try
                {
                    cancelTokSrc.Cancel();
                }
                catch (AggregateException e)
                {
                    Console.WriteLine("\nЗадача task отменена.\n");

                    Console.WriteLine("- " + e.InnerException.Message);
                }
                finally
                {
                    cancelTokSrc.Dispose();
                }
            }  
        }
    }
}
