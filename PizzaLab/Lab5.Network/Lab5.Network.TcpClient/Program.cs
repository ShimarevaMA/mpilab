using System.Numerics;
using Lab5.Network.Common;
using Lab5.Network.Common.UserApi;

internal class Program
{
    private static object _locker = new object();

    public static async Task Main(string[] args)
    {
        var serverAdress = new Uri("tcp://127.0.0.1:5555");
        var client = new NetTcpClient(serverAdress);
        Console.WriteLine($"Connect to server at {serverAdress}");
        await client.ConnectAsync();

        var userApi = new UserApiClient(client);
        await ManageUsers(userApi);
        client.Dispose();
    }

    private static async Task ManageUsers(IUserApi userApi)
    {
        PrintMenu();

        while(true) {
            var key = Console.ReadKey(true);

            PrintMenu();

            if (key.Key == ConsoleKey.D1) 
            {
                var users = await userApi.GetAllAsync();
                Console.WriteLine($"| Id    |      Нзавание лабораторной работы         | Ее описание               |");
                foreach (var user in users)
                {
                    Console.WriteLine($"| {user.Id,5} | {user.nameLab,20} | {user.description,30} ");
                }
            }

            if (key.Key == ConsoleKey.D2) 
            {
                Console.Write("Введите айди лабораторной работы: ");
                var userIdString = Console.ReadLine();
                int.TryParse(userIdString, out var userId);
                var user = await userApi.GetAsync(userId);
                Console.WriteLine($"| {user.Id,5} | {user.nameLab,20} | {user.description,30} ");
            }

            if (key.Key == ConsoleKey.D3) 
            {
                
                
                Console.Write("Напишите навзвание лабораторной работы: ");
                var addName = Console.ReadLine() ?? "empty";
                Console.Write("Напишите ее опписание: ");
                var addDescription = Console.ReadLine() ?? "empty";
                var addUser = new User(Id: 0,
                    nameLab: addName,
                    description: addDescription
                );
                var addResult = await userApi.AddAsync(addUser);
                Console.WriteLine(addResult ? "Ok" : "Error");
                
            }
            if (key.Key == ConsoleKey.D4) // Обновление только статуса пользователя
            {
                Console.Write("Введите айди лабораторной работы для изменения ее описания: ");
                var updateIdString = Console.ReadLine();
                int.TryParse(updateIdString, out var updateId);

                // Получаем текущие данные пользователя
                var existingUser = await userApi.GetAsync(updateId);
                if (existingUser == null)
                {
                    Console.WriteLine("Не найдено.");
                    continue;
                }
                Console.Write("Напишите ее опписание: ");
                var addDescription = Console.ReadLine() ?? "empty";

                // Создаем новый объект пользователя с измененным только статусом
                var updatedUser = new User(
                    Id: existingUser.Id,
                    nameLab: existingUser.nameLab,
                    description: addDescription
                );

                var updateResult = await userApi.UpdateAsync(updateId, updatedUser);

                Console.WriteLine(updateResult ? "Обновлено" : "Ошибка при обновлении");
            }
            if (key.Key == ConsoleKey.D5) 
            {
                Console.Write("Введите айди лабораторной работы: ");
                var deleteIdString = Console.ReadLine();
                int.TryParse(deleteIdString, out var deleteId);

                var deleteResult = await userApi.DeleteAsync(deleteId);

                Console.WriteLine(deleteResult ? "Удалено" : "Ошибка при удалении");
            }

            if (key.Key == ConsoleKey.Escape)
            {
                break;
            }
        }
        Console.ReadKey();
        //while (Console.Read)
    }

    private static void PrintMenu()
    {
        lock (_locker)
        {
            Console.WriteLine("1 - Вывести лабораторные работы");
            Console.WriteLine("2 - Показать лабораторную работу по id");
            Console.WriteLine("3 - Добавить лабораторную работу");
            Console.WriteLine("4 - Изменить описание");
            Console.WriteLine("5 - Удалить лабораторную работу");
            Console.WriteLine("-------");
        }
    }
    
    

}
