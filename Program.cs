using System;
using System.Threading;
using System.Threading.Tasks;

namespace Flow
{
    class Program
    {
        static bool exit = false;
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
            await Task.Delay(rand.Next(100, 2000));
            Console.WriteLine($"{textMethod} {rand}");
        }
        async static Task Main(string[] args)
        {
            Console.WriteLine("ESC - выход");
            await Task.Factory.StartNew(() =>
            {
                if (Console.ReadKey().Key == ConsoleKey.Escape)
                    exit = true;
            });

            var cancelTokSrc = new CancellationTokenSource();

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
