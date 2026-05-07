# EduProcessManager

ASP.NET Core MVC вебзастосунок для управління освітнім процесом з модулем тестування та реєстрації на заходи.

## Запуск
1. Відкрити папку проєкту у Visual Studio або Rider.
2. Перевірити рядок підключення у `appsettings.json`.
3. У Package Manager Console виконати:
   `Add-Migration InitialCreate`
   `Update-Database`
4. Запустити проєкт.

## Демо-акаунти
- superadmin@edu.local / 123456
- admin@edu.local / 123456
- teacher@edu.local / 123456
- student@edu.local / 123456

У `Program.cs` база автоматично мігрується, а початкові дані додаються через `DbSeeder`.
