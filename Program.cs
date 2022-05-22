using System;
using System.Threading;
using System.Threading.Tasks;

namespace Flow
{
    class Program
    {
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
            Console.WriteLine($"{textMethod} { Task.CurrentId }");
        }
        async static Task Main(string[] args)
        {
            ConsoleKeyInfo pressed;
            object o = new object();
            string s = new string("");
            pressed = Console.ReadKey(true);
            var task1 = new Task(Method(o, s));
            var task2 = new Task(Method(o, s));
            var task3 = new Task(Method(o, s));
            var task4 = new Task(Method(o, s));
            var task5 = new Task(Method(o, s));
            while (pressed != null)
            {
                var cancelTokSrc = new CancellationTokenSource();

                await task1.Run(() => {
                    Method(cancelTokSrc.Token, "Первый пошел");
                });
                await task2.Run(() => {
                    Method(cancelTokSrc.Token, "Второй пошел");
                });
                await task3.Run(() => {
                    Method(cancelTokSrc.Token, "Третий пошел");
                });
                await task4.Run(() => {
                    Method(cancelTokSrc.Token, "Четвертый пошел");
                });
                await task5.Run(() => {
                    Method(cancelTokSrc.Token, "Пятый пошел");
                });
                
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
