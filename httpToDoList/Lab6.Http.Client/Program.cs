using Lab6.Http.Common;

internal class Program
{
    private static object _locker = new object();

    public static async Task Main(string[] args)
    {
        var httpClient = new HttpClient()
        {
            BaseAddress = new Uri("http://localhost:5214/api/")
        };

        var taskApiClient = new TaskApiClient(httpClient);

        await ManageTasks(taskApiClient);
    }

    private static async Task ManageTasks(ITaskApi taskApi)
    {
        PrintMenu();

        while (true)
        {
            var key = Console.ReadKey(true);

            PrintMenu();

            if (key.Key == ConsoleKey.D1)
            {
                var tasks = await taskApi.GetAllAsync();
                Console.WriteLine($"| Id    |   Title        |                  Descriptionb                          |");
                foreach (var task in tasks)
                {
                    Console.WriteLine($"| {task.Id,5} | {task.Title,15} | {task.Description,20} |");
                }
            }

            if (key.Key == ConsoleKey.D2)
            {
                Console.Write("Введите id фильма: ");
                var taskIdString = Console.ReadLine();
                int.TryParse(taskIdString, out var taskId);
                var task = await taskApi.GetAsync(taskId);
                if (task == null)
                {
                    Console.WriteLine("Не найдена.");
                }
                else
                {
                    Console.WriteLine($"| {task.Id,5} | {task.Title,15} | {task.Description,20} |");
                }
            }

            if (key.Key == ConsoleKey.D3)
            {
                Console.Write("Введите название фильма: ");
                var title = Console.ReadLine() ?? "Без названия";
                Console.Write("Введите описание фильма: ");
                var description = Console.ReadLine() ?? "Без описания";
                
                var newTask = new TaskItem(
                    id: 0,
                    title: title,
                    description: description
                );

                var addResult = await taskApi.AddAsync(newTask);
                Console.WriteLine(addResult ? "Добавлено." : "Ошибка добавления.");
            }

            if (key.Key == ConsoleKey.D4)
            {
                Console.Write("Введите id фильма: ");
                var taskIdString = Console.ReadLine();
                int.TryParse(taskIdString, out var taskId);
                var task = await taskApi.GetAsync(taskId);
                if (task == null)
                {
                    Console.WriteLine("Не найдена.");
                    continue;
                }

                Console.Write("Введите новое описание фмльма: ");
                var descrip = Console.ReadLine() ?? "Новая";
                task.Description = descrip;

                var updateResult = await taskApi.UpdateAsync(taskId, task);
                Console.WriteLine(updateResult ? "Обновлено." : "Ошибка обновления.");
            }

            if (key.Key == ConsoleKey.D5)
            {
                Console.Write("Введите id фильма: ");
                var taskIdString = Console.ReadLine();
                int.TryParse(taskIdString, out var taskId);

                var deleteResult = await taskApi.DeleteAsync(taskId);
                Console.WriteLine(deleteResult ? "Удалена." : "Ошибка удаления.");
            }

            if (key.Key == ConsoleKey.Escape)
            {
                break;
            }
        }
    }

    private static void PrintMenu()
    {
        lock (_locker)
        {
            Console.WriteLine("1 - Показать все фильмы");
            Console.WriteLine("2 - Показать фильма по id");
            Console.WriteLine("3 - Добавить фильм");
            Console.WriteLine("4 - Обновить описание фильма");
            Console.WriteLine("5 - Удалить фильм");
            Console.WriteLine("-------");
        }
    }
}
