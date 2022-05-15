using System;
using System.Threading;
using System.Threading.Tasks;

namespace Flow
{
    class Program
    {
        async Task Method(object cancel, string textMethod)
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
            Console.WriteLine($"{textMethod} { Task.CurrentId }");
        }
        async static Task Main(string[] args)
        {
            ConsoleKeyInfo pressed;
            pressed = Console.ReadKey(true);
            while (pressed != null)
            {
                var cancelTokSrc = new CancellationTokenSource();

                Task task1 = await Task.Factory.StartNew(Method, cancelTokSrc.Token, "Первый пошел");
                Task task2 = await Task.Factory.StartNew(Method, cancelTokSrc.Token, "Второй пошел");
                Task task3 = await Task.Factory.StartNew(Method, cancelTokSrc.Token, "Третий пошел");
                Task task4 = await Task.Factory.StartNew(Method, cancelTokSrc.Token, "Четвертый пошел");
                Task task5 = await Task.Factory.StartNew(Method, cancelTokSrc.Token, "Пятый пошел");

                try
                {
                    cancelTokSrc.Cancel();
                }
                catch (AggregateException e)
                {
                    if (task1.IsCanceled || task2.IsCanceled || task3.IsCanceled || task4.IsCanceled || task5.IsCanceled)
                        Console.WriteLine("\nЗадача task отменена.\n");

                    Console.WriteLine("- " + e.InnerException.Message);
                }
                finally
                {
                    task1.Dispose();
                    task2.Dispose();
                    task3.Dispose();
                    task4.Dispose();
                    task5.Dispose();
                    cancelTokSrc.Dispose();
                }
            }
        }
    }
}
