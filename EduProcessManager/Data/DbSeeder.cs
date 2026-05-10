using EduProcessManager.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EduProcessManager.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var db = services.GetRequiredService<ApplicationDbContext>();

        string[] roles = { "SuperAdmin", "Admin", "Teacher", "Student" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        var superAdmin = await CreateUser(userManager, "superadmin@edu.local", "Суперадмін системи", "SuperAdmin");
        var admin = await CreateUser(userManager, "admin@edu.local", "Адміністратор коледжу", "Admin");

        var teacher1 = await CreateUser(userManager, "shevchuk@edu.local", "Олена Шевчук", "Teacher");
        var teacher2 = await CreateUser(userManager, "kovalenko@edu.local", "Андрій Коваленко", "Teacher");
        var teacher3 = await CreateUser(userManager, "melnyk@edu.local", "Ірина Мельник", "Teacher");

        var student1 = await CreateUser(userManager, "bondarenko@edu.local", "Марія Бондаренко", "Student");
        var student2 = await CreateUser(userManager, "tkachuk@edu.local", "Олександр Ткачук", "Student");
        var student3 = await CreateUser(userManager, "hrytsenko@edu.local", "Софія Гриценко", "Student");
        var student4 = await CreateUser(userManager, "romaniv@edu.local", "Дмитро Романів", "Student");
        var student5 = await CreateUser(userManager, "klymenko@edu.local", "Анна Клименко", "Student");
        var student6 = await CreateUser(userManager, "savchuk@edu.local", "Назар Савчук", "Student");
        var student7 = await CreateUser(userManager, "moroz@edu.local", "Катерина Мороз", "Student");
        var student8 = await CreateUser(userManager, "levytskyi@edu.local", "Владислав Левицький", "Student");

        if (!await db.StudentGroups.AnyAsync())
        {
            db.StudentGroups.AddRange(
                new StudentGroup { Name = "ІПЗ-41", Description = "4 курс, спеціальність Інженерія програмного забезпечення" },
                new StudentGroup { Name = "ІПЗ-42", Description = "4 курс, спеціальність Інженерія програмного забезпечення" },
                new StudentGroup { Name = "КН-31", Description = "3 курс, комп'ютерні науки" }
            );

            await db.SaveChangesAsync();
        }

        var groupIpz41 = await db.StudentGroups.FirstAsync(g => g.Name == "ІПЗ-41");
        var groupIpz42 = await db.StudentGroups.FirstAsync(g => g.Name == "ІПЗ-42");
        var groupKn31 = await db.StudentGroups.FirstAsync(g => g.Name == "КН-31");

        student1.StudentGroupId = groupIpz41.Id;
        student2.StudentGroupId = groupIpz41.Id;
        student3.StudentGroupId = groupIpz41.Id;

        student4.StudentGroupId = groupIpz42.Id;
        student5.StudentGroupId = groupIpz42.Id;
        student6.StudentGroupId = groupIpz42.Id;

        student7.StudentGroupId = groupKn31.Id;
        student8.StudentGroupId = groupKn31.Id;

        await db.SaveChangesAsync();

        if (!await db.BellSchedules.AnyAsync())
        {
            db.BellSchedules.AddRange(
                new BellSchedule
                {
                    LessonNumber = 1,
                    Name = "1 пара",
                    StartTime = new TimeSpan(8, 30, 0),
                    EndTime = new TimeSpan(10, 0, 0)
                },
                new BellSchedule
                {
                    LessonNumber = 2,
                    Name = "2 пара",
                    StartTime = new TimeSpan(10, 10, 0),
                    EndTime = new TimeSpan(11, 40, 0)
                },
                new BellSchedule
                {
                    LessonNumber = 3,
                    Name = "3 пара",
                    StartTime = new TimeSpan(12, 0, 0),
                    EndTime = new TimeSpan(13, 30, 0)
                },
                new BellSchedule
                {
                    LessonNumber = 4,
                    Name = "4 пара",
                    StartTime = new TimeSpan(13, 40, 0),
                    EndTime = new TimeSpan(15, 10, 0)
                },
                new BellSchedule
                {
                    LessonNumber = 5,
                    Name = "5 пара",
                    StartTime = new TimeSpan(15, 20, 0),
                    EndTime = new TimeSpan(16, 50, 0)
                }
            );

            await db.SaveChangesAsync();
        }

        if (!await db.LessonSchedules.AnyAsync())
        {
            db.LessonSchedules.AddRange(
                new LessonSchedule
                {
                    StudentGroupId = groupIpz41.Id,
                    SubjectName = "ASP.NET Core MVC",
                    TeacherName = "Олена Шевчук",
                    Room = "Аудиторія 212",
                    DayOfWeek = DayOfWeek.Monday,
                    StartTime = new TimeSpan(8, 30, 0),
                    EndTime = new TimeSpan(10, 0, 0),
                    Description = "Практичне заняття"
                },
                new LessonSchedule
                {
                    StudentGroupId = groupIpz41.Id,
                    SubjectName = "Entity Framework Core",
                    TeacherName = "Андрій Коваленко",
                    Room = "Аудиторія 305",
                    DayOfWeek = DayOfWeek.Monday,
                    StartTime = new TimeSpan(10, 10, 0),
                    EndTime = new TimeSpan(11, 40, 0),
                    Description = "Лекція"
                },
                new LessonSchedule
                {
                    StudentGroupId = groupIpz41.Id,
                    SubjectName = "Тестування програмного забезпечення",
                    TeacherName = "Ірина Мельник",
                    Room = "Аудиторія 101",
                    DayOfWeek = DayOfWeek.Monday,
                    StartTime = new TimeSpan(12, 0, 0),
                    EndTime = new TimeSpan(13, 30, 0),
                    Description = "Лабораторне заняття"
                },
                new LessonSchedule
                {
                    StudentGroupId = groupIpz41.Id,
                    SubjectName = "Бази даних",
                    TeacherName = "Андрій Коваленко",
                    Room = "Аудиторія 214",
                    DayOfWeek = DayOfWeek.Tuesday,
                    StartTime = new TimeSpan(8, 30, 0),
                    EndTime = new TimeSpan(10, 0, 0),
                    Description = "Практичне заняття"
                },
                new LessonSchedule
                {
                    StudentGroupId = groupIpz41.Id,
                    SubjectName = "Об'єктно-орієнтоване програмування",
                    TeacherName = "Олена Шевчук",
                    Room = "Аудиторія 404",
                    DayOfWeek = DayOfWeek.Tuesday,
                    StartTime = new TimeSpan(10, 10, 0),
                    EndTime = new TimeSpan(11, 40, 0),
                    Description = "Лекція"
                },
                new LessonSchedule
                {
                    StudentGroupId = groupIpz41.Id,
                    SubjectName = "Інформаційна безпека",
                    TeacherName = "Ірина Мельник",
                    Room = "Аудиторія 401",
                    DayOfWeek = DayOfWeek.Wednesday,
                    StartTime = new TimeSpan(12, 0, 0),
                    EndTime = new TimeSpan(13, 30, 0),
                    Description = "Семінар"
                },
                new LessonSchedule
                {
                    StudentGroupId = groupIpz41.Id,
                    SubjectName = "Конструювання програмного забезпечення",
                    TeacherName = "Андрій Коваленко",
                    Room = "Аудиторія 305",
                    DayOfWeek = DayOfWeek.Thursday,
                    StartTime = new TimeSpan(8, 30, 0),
                    EndTime = new TimeSpan(10, 0, 0),
                    Description = "Лекція"
                },
                new LessonSchedule
                {
                    StudentGroupId = groupIpz41.Id,
                    SubjectName = "Playwright E2E Testing",
                    TeacherName = "Ірина Мельник",
                    Room = "Online",
                    DayOfWeek = DayOfWeek.Friday,
                    StartTime = new TimeSpan(13, 40, 0),
                    EndTime = new TimeSpan(15, 10, 0),
                    Description = "Практикум"
                },

                new LessonSchedule
                {
                    StudentGroupId = groupIpz42.Id,
                    SubjectName = "C# та .NET",
                    TeacherName = "Олена Шевчук",
                    Room = "Аудиторія 404",
                    DayOfWeek = DayOfWeek.Monday,
                    StartTime = new TimeSpan(8, 30, 0),
                    EndTime = new TimeSpan(10, 0, 0),
                    Description = "Практичне заняття"
                },
                new LessonSchedule
                {
                    StudentGroupId = groupIpz42.Id,
                    SubjectName = "ASP.NET Core MVC",
                    TeacherName = "Олена Шевчук",
                    Room = "Аудиторія 212",
                    DayOfWeek = DayOfWeek.Monday,
                    StartTime = new TimeSpan(10, 10, 0),
                    EndTime = new TimeSpan(11, 40, 0),
                    Description = "Лекція"
                },
                new LessonSchedule
                {
                    StudentGroupId = groupIpz42.Id,
                    SubjectName = "SQL-запити та нормалізація",
                    TeacherName = "Андрій Коваленко",
                    Room = "Аудиторія 214",
                    DayOfWeek = DayOfWeek.Tuesday,
                    StartTime = new TimeSpan(12, 0, 0),
                    EndTime = new TimeSpan(13, 30, 0),
                    Description = "Лабораторне заняття"
                },
                new LessonSchedule
                {
                    StudentGroupId = groupIpz42.Id,
                    SubjectName = "Тестування програмного забезпечення",
                    TeacherName = "Ірина Мельник",
                    Room = "Аудиторія 101",
                    DayOfWeek = DayOfWeek.Wednesday,
                    StartTime = new TimeSpan(8, 30, 0),
                    EndTime = new TimeSpan(10, 0, 0),
                    Description = "Практичне заняття"
                },
                new LessonSchedule
                {
                    StudentGroupId = groupIpz42.Id,
                    SubjectName = "SOLID та рефакторинг",
                    TeacherName = "Андрій Коваленко",
                    Room = "Аудиторія 305",
                    DayOfWeek = DayOfWeek.Thursday,
                    StartTime = new TimeSpan(10, 10, 0),
                    EndTime = new TimeSpan(11, 40, 0),
                    Description = "Семінар"
                },
                new LessonSchedule
                {
                    StudentGroupId = groupIpz42.Id,
                    SubjectName = "Playwright E2E Testing",
                    TeacherName = "Ірина Мельник",
                    Room = "Online",
                    DayOfWeek = DayOfWeek.Friday,
                    StartTime = new TimeSpan(13, 40, 0),
                    EndTime = new TimeSpan(15, 10, 0),
                    Description = "Практикум"
                },

                new LessonSchedule
                {
                    StudentGroupId = groupKn31.Id,
                    SubjectName = "Бази даних",
                    TeacherName = "Андрій Коваленко",
                    Room = "Аудиторія 214",
                    DayOfWeek = DayOfWeek.Monday,
                    StartTime = new TimeSpan(12, 0, 0),
                    EndTime = new TimeSpan(13, 30, 0),
                    Description = "Лекція"
                },
                new LessonSchedule
                {
                    StudentGroupId = groupKn31.Id,
                    SubjectName = "Інформаційна безпека",
                    TeacherName = "Ірина Мельник",
                    Room = "Аудиторія 401",
                    DayOfWeek = DayOfWeek.Tuesday,
                    StartTime = new TimeSpan(10, 10, 0),
                    EndTime = new TimeSpan(11, 40, 0),
                    Description = "Семінар"
                },
                new LessonSchedule
                {
                    StudentGroupId = groupKn31.Id,
                    SubjectName = "Основи програмування C#",
                    TeacherName = "Олена Шевчук",
                    Room = "Аудиторія 404",
                    DayOfWeek = DayOfWeek.Wednesday,
                    StartTime = new TimeSpan(8, 30, 0),
                    EndTime = new TimeSpan(10, 0, 0),
                    Description = "Практичне заняття"
                },
                new LessonSchedule
                {
                    StudentGroupId = groupKn31.Id,
                    SubjectName = "Алгоритми та структури даних",
                    TeacherName = "Андрій Коваленко",
                    Room = "Аудиторія 305",
                    DayOfWeek = DayOfWeek.Thursday,
                    StartTime = new TimeSpan(13, 40, 0),
                    EndTime = new TimeSpan(15, 10, 0),
                    Description = "Лабораторне заняття"
                },
                new LessonSchedule
                {
                    StudentGroupId = groupKn31.Id,
                    SubjectName = "Вебтехнології",
                    TeacherName = "Олена Шевчук",
                    Room = "Аудиторія 212",
                    DayOfWeek = DayOfWeek.Friday,
                    StartTime = new TimeSpan(10, 10, 0),
                    EndTime = new TimeSpan(11, 40, 0),
                    Description = "Практикум"
                }
            );

            await db.SaveChangesAsync();
        }

        if (await db.Tests.AnyAsync())
            return;

        var softwareEngineering = new Subject
        {
            Name = "Конструювання програмного забезпечення",
            Description = "Дисципліна про проєктування, рефакторинг, патерни та якість програмного забезпечення."
        };

        var webDevelopment = new Subject
        {
            Name = "ASP.NET Core MVC",
            Description = "Дисципліна про створення вебзастосунків на ASP.NET Core MVC з Entity Framework Core."
        };

        var databases = new Subject
        {
            Name = "Бази даних",
            Description = "Дисципліна про моделювання даних, SQL-запити, зв'язки між таблицями та нормалізацію."
        };

        var testing = new Subject
        {
            Name = "Тестування програмного забезпечення",
            Description = "Дисципліна про ручне та автоматизоване тестування, тест-кейси, баг-репорти й Playwright."
        };

        var security = new Subject
        {
            Name = "Інформаційна безпека",
            Description = "Дисципліна про захист інформації, автентифікацію, авторизацію та шифрування."
        };

        var oop = new Subject
        {
            Name = "Об'єктно-орієнтоване програмування",
            Description = "Дисципліна про класи, об'єкти, інкапсуляцію, спадкування, поліморфізм та інтерфейси."
        };

        var tests = new List<Test>
        {
            new Test
            {
                Title = "ASP.NET Core MVC та маршрутизація",
                Description = "Тест перевіряє знання архітектури MVC, контролерів, представлень, моделей та маршрутизації.",
                TimeLimitMinutes = 25,
                MaxAttempts = 2,
                IsPublished = true,
                AvailableFrom = DateTime.Now.AddDays(-2),
                AvailableUntil = DateTime.Now.AddDays(5),
                Subject = webDevelopment,
                AuthorId = teacher1.Id,
                TestStudentGroups =
                {
                    new TestStudentGroup { StudentGroupId = groupIpz41.Id },
                    new TestStudentGroup { StudentGroupId = groupIpz42.Id }
                },
                Questions =
                {
                    Question("Що в ASP.NET Core MVC відповідає за обробку HTTP-запиту користувача?", 1, false,
                        ("Controller", true),
                        ("View", false),
                        ("CSS-файл", false),
                        ("Migration", false)),

                    Question("Яке призначення View у MVC?", 1, false,
                        ("Відображення даних користувачу", true),
                        ("Створення таблиць у базі даних", false),
                        ("Обробка маршрутизації на рівні сервера", false),
                        ("Збереження паролів користувачів", false)),

                    Question("Які компоненти належать до MVC?", 2, true,
                        ("Model", true),
                        ("View", true),
                        ("Controller", true),
                        ("Compiler", false)),

                    Question("Для чого використовується asp-controller у Razor-посиланні?", 1, false,
                        ("Для вказання контролера, до якого веде посилання", true),
                        ("Для підключення Bootstrap", false),
                        ("Для створення міграції", false),
                        ("Для перевірки ролі користувача", false)),

                    Question("Що означає IActionResult у методі контролера?", 1, false,
                        ("Тип результату, який повертає дія контролера", true),
                        ("Тип підключення до бази даних", false),
                        ("Назва таблиці в SQL Server", false),
                        ("Тип ролі користувача", false)),

                    Question("Який метод зазвичай повертає Razor-представлення?", 1, false,
                        ("View()", true),
                        ("SaveChanges()", false),
                        ("Include()", false),
                        ("AddMigration()", false)),

                    Question("Що дозволяє зробити атрибут [Authorize]?", 1, false,
                        ("Обмежити доступ до дії або контролера", true),
                        ("Створити нову таблицю", false),
                        ("Автоматично згенерувати HTML", false),
                        ("Запустити JavaScript-таймер", false)),

                    Question("Які HTTP-методи найчастіше використовуються для відкриття форми і її відправлення?", 2, true,
                        ("GET", true),
                        ("POST", true),
                        ("PATCH", false),
                        ("TRACE", false))
                }
            },

            new Test
            {
                Title = "Entity Framework Core Code First",
                Description = "Тест на знання DbContext, DbSet, міграцій, зв'язків між сутностями та LINQ-запитів.",
                TimeLimitMinutes = 30,
                MaxAttempts = 3,
                IsPublished = true,
                AvailableFrom = DateTime.Now.AddDays(2),
                AvailableUntil = DateTime.Now.AddDays(10),
                Subject = webDevelopment,
                AuthorId = teacher1.Id,
                TestStudentGroups =
                {
                    new TestStudentGroup { StudentGroupId = groupIpz41.Id }
                },
                Questions =
                {
                    Question("Що таке DbContext в Entity Framework Core?", 1, false,
                        ("Клас для роботи з базою даних через об'єкти", true),
                        ("Файл стилів сторінки", false),
                        ("Контролер авторизації", false),
                        ("HTML-шаблон", false)),

                    Question("Для чого використовується DbSet?", 1, false,
                        ("Для представлення таблиці або набору сутностей", true),
                        ("Для створення ролей користувачів", false),
                        ("Для запуску JavaScript-коду", false),
                        ("Для зберігання CSS-класів", false)),

                    Question("Яка команда створює міграцію через Package Manager Console?", 1, false,
                        ("Add-Migration", true),
                        ("Update-Database", false),
                        ("Create-Controller", false),
                        ("Run-Seed", false)),

                    Question("Яка команда застосовує міграції до бази даних?", 1, false,
                        ("Update-Database", true),
                        ("Add-Migration", false),
                        ("Drop-View", false),
                        ("Build-Layout", false)),

                    Question("Які можливості надає Entity Framework Core?", 2, true,
                        ("Роботу з базою через C#-об'єкти", true),
                        ("Міграції структури бази даних", true),
                        ("LINQ-запити до даних", true),
                        ("Компіляцію SCSS у CSS", false)),

                    Question("Для чого використовується Include()?", 1, false,
                        ("Для завантаження пов'язаних даних", true),
                        ("Для створення нового користувача", false),
                        ("Для перевірки пароля", false),
                        ("Для зміни кольору кнопки", false)),

                    Question("Що означає підхід Code First?", 1, false,
                        ("Спочатку створюються C#-класи, а потім на їх основі база даних", true),
                        ("Спочатку створюється HTML, а потім контролер", false),
                        ("Спочатку створюється CSS, а потім модель", false),
                        ("Спочатку створюється користувач, а потім роль", false)),

                    Question("Які типи зв'язків можуть бути між сутностями?", 2, true,
                        ("One-to-One", true),
                        ("One-to-Many", true),
                        ("Many-to-Many", true),
                        ("One-to-CSS", false)),

                    Question("Що робить SaveChangesAsync()?", 1, false,
                        ("Зберігає зміни у базі даних", true),
                        ("Створює Razor-сторінку", false),
                        ("Перевіряє Bootstrap", false),
                        ("Видаляє всі міграції", false))
                }
            },

            new Test
            {
                Title = "SOLID та рефакторинг коду",
                Description = "Тест перевіряє розуміння принципів SOLID, якісного кодування та покращення структури програм.",
                TimeLimitMinutes = 20,
                MaxAttempts = 1,
                IsPublished = true,
                AvailableFrom = DateTime.Now.AddDays(-10),
                AvailableUntil = DateTime.Now.AddDays(-2),
                Subject = softwareEngineering,
                AuthorId = teacher2.Id,
                TestStudentGroups =
                {
                    new TestStudentGroup { StudentGroupId = groupIpz42.Id }
                },
                Questions =
                {
                    Question("Що означає принцип єдиної відповідальності?", 1, false,
                        ("Клас повинен мати одну основну причину для змін", true),
                        ("Усі класи повинні бути в одному файлі", false),
                        ("Метод повинен завжди бути статичним", false),
                        ("Клас не може мати властивостей", false)),

                    Question("Яка мета рефакторингу?", 1, false,
                        ("Покращити внутрішню структуру коду без зміни його поведінки", true),
                        ("Змінити тему оформлення сайту", false),
                        ("Видалити базу даних", false),
                        ("Зменшити кількість користувачів", false)),

                    Question("Які ознаки поганого коду можуть бути підставою для рефакторингу?", 2, true,
                        ("Дублювання коду", true),
                        ("Занадто великі класи", true),
                        ("Незрозумілі назви змінних", true),
                        ("Наявність простору імен", false)),

                    Question("Що означає Open/Closed Principle?", 1, false,
                        ("Код має бути відкритим для розширення, але закритим для зміни", true),
                        ("Код має бути відкритим тільки в браузері", false),
                        ("Клас має бути завжди public", false),
                        ("Метод не може повертати значення", false)),

                    Question("Що таке інтерфейс у C#?", 1, false,
                        ("Контракт, який визначає набір методів або властивостей", true),
                        ("Файл бази даних", false),
                        ("HTML-сторінка", false),
                        ("Тип міграції", false)),

                    Question("Який принцип SOLID пов'язаний із заміною базового типу його нащадками без порушення роботи програми?", 1, false,
                        ("Liskov Substitution Principle", true),
                        ("Single Responsibility Principle", false),
                        ("Dependency Inversion Principle", false),
                        ("Interface Segregation Principle", false)),

                    Question("Що означає Dependency Inversion Principle?", 1, false,
                        ("Залежати потрібно від абстракцій, а не від конкретних реалізацій", true),
                        ("Код не повинен використовувати класи", false),
                        ("База даних повинна залежати від CSS", false),
                        ("Контролер повинен містити всю бізнес-логіку", false)),

                    Question("Що є хорошою практикою після рефакторингу?", 1, false,
                        ("Запустити тести і перевірити, що поведінка не змінилася", true),
                        ("Одразу видалити всі старі файли", false),
                        ("Змінити всі назви таблиць", false),
                        ("Вимкнути валідацію", false))
                }
            },

            new Test
            {
                Title = "SQL-запити та нормалізація баз даних",
                Description = "Тест охоплює SELECT, JOIN, ключі, нормальні форми та логіку побудови реляційної бази даних.",
                TimeLimitMinutes = 35,
                MaxAttempts = 2,
                IsPublished = true,
                AvailableFrom = DateTime.Now.AddDays(-1),
                AvailableUntil = DateTime.Now.AddDays(8),
                Subject = databases,
                AuthorId = teacher3.Id,
                TestStudentGroups =
                {
                    new TestStudentGroup { StudentGroupId = groupKn31.Id }
                },
                Questions =
                {
                    Question("Для чого використовується команда SELECT?", 1, false,
                        ("Для вибірки даних з таблиці", true),
                        ("Для створення нового CSS-файлу", false),
                        ("Для запуску контролера", false),
                        ("Для очищення кешу браузера", false)),

                    Question("Що таке первинний ключ?", 1, false,
                        ("Поле або набір полів, які унікально ідентифікують запис", true),
                        ("Назва бази даних", false),
                        ("Тип HTML-форми", false),
                        ("Роль користувача", false)),

                    Question("Для чого використовується JOIN?", 1, false,
                        ("Для об'єднання даних із кількох таблиць", true),
                        ("Для створення нового користувача", false),
                        ("Для редагування CSS", false),
                        ("Для запуску міграції", false)),

                    Question("Які бувають види JOIN?", 2, true,
                        ("INNER JOIN", true),
                        ("LEFT JOIN", true),
                        ("RIGHT JOIN", true),
                        ("HTML JOIN", false)),

                    Question("Що таке зовнішній ключ?", 1, false,
                        ("Поле, яке посилається на первинний ключ іншої таблиці", true),
                        ("Пароль адміністратора", false),
                        ("Назва View", false),
                        ("Файл конфігурації Bootstrap", false)),

                    Question("Навіщо потрібна нормалізація бази даних?", 1, false,
                        ("Для зменшення дублювання даних і покращення структури", true),
                        ("Для збільшення кількості CSS-класів", false),
                        ("Для автоматичного створення паролів", false),
                        ("Для приховування всіх таблиць", false)),

                    Question("Яка команда використовується для фільтрації рядків?", 1, false,
                        ("WHERE", true),
                        ("ORDER CSS", false),
                        ("PRINT VIEW", false),
                        ("MODEL ONLY", false)),

                    Question("Що робить ORDER BY?", 1, false,
                        ("Сортує результат запиту", true),
                        ("Створює нову базу", false),
                        ("Видаляє всі таблиці", false),
                        ("Додає роль користувачу", false)),

                    Question("Які агрегатні функції існують у SQL?", 2, true,
                        ("COUNT", true),
                        ("AVG", true),
                        ("SUM", true),
                        ("ROUTE", false))
                }
            },

            new Test
            {
                Title = "Основи тестування ПЗ",
                Description = "Тест перевіряє знання тест-кейсів, баг-репортів, видів тестування та критеріїв якості.",
                TimeLimitMinutes = 18,
                MaxAttempts = 4,
                IsPublished = true,
                AvailableFrom = DateTime.Now.AddDays(4),
                AvailableUntil = DateTime.Now.AddDays(12),
                Subject = testing,
                AuthorId = teacher2.Id,
                TestStudentGroups =
                {
                    new TestStudentGroup { StudentGroupId = groupIpz41.Id },
                    new TestStudentGroup { StudentGroupId = groupKn31.Id }
                },
                Questions =
                {
                    Question("Що таке тест-кейс?", 1, false,
                        ("Опис кроків, умов і очікуваного результату перевірки", true),
                        ("Файл стилів", false),
                        ("Клас для підключення до бази", false),
                        ("Назва ролі користувача", false)),

                    Question("Що таке баг-репорт?", 1, false,
                        ("Опис знайденої помилки з умовами її відтворення", true),
                        ("Форма створення користувача", false),
                        ("Міграція бази даних", false),
                        ("Метод контролера", false)),

                    Question("Які дані зазвичай містить баг-репорт?", 2, true,
                        ("Опис помилки", true),
                        ("Кроки відтворення", true),
                        ("Фактичний і очікуваний результат", true),
                        ("Колір кнопки в Bootstrap", false)),

                    Question("Що таке регресійне тестування?", 1, false,
                        ("Перевірка, що нові зміни не зламали старий функціонал", true),
                        ("Тестування лише дизайну", false),
                        ("Створення нової бази даних", false),
                        ("Очищення журналу дій", false)),

                    Question("Що таке smoke testing?", 1, false,
                        ("Швидка перевірка основних функцій системи", true),
                        ("Повне тестування кожного рядка коду", false),
                        ("Тестування тільки SQL-запитів", false),
                        ("Перевірка кольору фону", false)),

                    Question("Що таке автоматизоване тестування?", 1, false,
                        ("Виконання перевірок за допомогою скриптів або інструментів", true),
                        ("Ручне натискання кнопок без сценарію", false),
                        ("Створення макета сайту", false),
                        ("Заповнення журналу вручну", false)),

                    Question("Який інструмент можна використовувати для E2E-тестування вебзастосунків?", 1, false,
                        ("Playwright", true),
                        ("Entity Framework", false),
                        ("SQL Server Management Studio", false),
                        ("NuGet Package Manager", false)),

                    Question("Які рівні тестування можуть застосовуватись у проєкті?", 2, true,
                        ("Модульне тестування", true),
                        ("Інтеграційне тестування", true),
                        ("Системне тестування", true),
                        ("Кольорове тестування", false))
                }
            },

            new Test
            {
                Title = "Авторизація, ролі та безпека в ASP.NET Core",
                Description = "Тест про Identity, ролі користувачів, обмеження доступу, автентифікацію та базові поняття безпеки.",
                TimeLimitMinutes = 28,
                MaxAttempts = 1,
                IsPublished = true,
                AvailableFrom = DateTime.Now.AddDays(-12),
                AvailableUntil = DateTime.Now.AddDays(-1),
                Subject = security,
                AuthorId = teacher3.Id,
                Questions =
                {
                    Question("Що таке автентифікація?", 1, false,
                        ("Перевірка особи користувача", true),
                        ("Надання прав після входу", false),
                        ("Створення таблиці в БД", false),
                        ("Сортування списку тестів", false)),

                    Question("Що таке авторизація?", 1, false,
                        ("Перевірка прав доступу користувача", true),
                        ("Введення логіна і пароля", false),
                        ("Створення Razor View", false),
                        ("Оновлення Bootstrap", false)),

                    Question("Для чого використовуються ролі в системі?", 1, false,
                        ("Для розмежування доступу між типами користувачів", true),
                        ("Для створення тестових питань", false),
                        ("Для підключення CSS", false),
                        ("Для запуску міграцій", false)),

                    Question("Які ролі можуть бути в освітній системі?", 2, true,
                        ("Admin", true),
                        ("Teacher", true),
                        ("Student", true),
                        ("Table", false)),

                    Question("Що робить [Authorize(Roles = \"Teacher\")]", 1, false,
                        ("Дозволяє доступ лише користувачам з роллю Teacher", true),
                        ("Створює роль Teacher у базі", false),
                        ("Видаляє всіх студентів", false),
                        ("Показує всі таблиці БД", false)),

                    Question("Чому паролі не можна зберігати у відкритому вигляді?", 1, false,
                        ("Бо при витоку бази даних вони стануть доступними зловмисникам", true),
                        ("Бо HTML не підтримує текст", false),
                        ("Бо SQL не підтримує рядки", false),
                        ("Бо CSS не може їх відобразити", false)),

                    Question("Що таке HTTPS?", 1, false,
                        ("Захищений протокол передавання даних у вебі", true),
                        ("Тип ViewModel", false),
                        ("Назва міграції", false),
                        ("Метод контролера", false)),

                    Question("Які дії допомагають підвищити безпеку вебзастосунку?", 2, true,
                        ("Перевірка прав доступу", true),
                        ("Валідація вхідних даних", true),
                        ("Використання HTTPS", true),
                        ("Збереження пароля у звичайному тексті", false))
                }
            },

            new Test
            {
                Title = "ООП у C#: класи, інтерфейси та спадкування",
                Description = "Тест перевіряє базові принципи ООП, використання класів, об'єктів, інтерфейсів і поліморфізму.",
                TimeLimitMinutes = 22,
                MaxAttempts = 3,
                IsPublished = true,
                AvailableFrom = DateTime.Now.AddDays(-3),
                AvailableUntil = DateTime.Now.AddDays(4),
                Subject = oop,
                AuthorId = teacher1.Id,
                Questions =
                {
                    Question("Що таке клас у C#?", 1, false,
                        ("Шаблон для створення об'єктів", true),
                        ("Готовий запис у таблиці", false),
                        ("Файл стилів", false),
                        ("Команда SQL", false)),

                    Question("Що таке об'єкт?", 1, false,
                        ("Конкретний екземпляр класу", true),
                        ("Назва бази даних", false),
                        ("Тип міграції", false),
                        ("HTML-форма", false)),

                    Question("Що таке інкапсуляція?", 1, false,
                        ("Приховування внутрішньої реалізації та контроль доступу до даних", true),
                        ("Видалення всіх методів", false),
                        ("Зміна кольору сторінки", false),
                        ("Збереження даних без класів", false)),

                    Question("Що таке спадкування?", 1, false,
                        ("Можливість створити новий клас на основі існуючого", true),
                        ("Автоматичне створення бази", false),
                        ("Підключення Bootstrap", false),
                        ("Сортування таблиці", false)),

                    Question("Що таке поліморфізм?", 1, false,
                        ("Можливість використовувати один інтерфейс для різних реалізацій", true),
                        ("Один файл у проєкті", false),
                        ("Одна таблиця в базі", false),
                        ("Один користувач у системі", false)),

                    Question("Які модифікатори доступу існують у C#?", 2, true,
                        ("public", true),
                        ("private", true),
                        ("protected", true),
                        ("database", false)),

                    Question("Для чого потрібен конструктор?", 1, false,
                        ("Для ініціалізації об'єкта під час створення", true),
                        ("Для запуску CSS", false),
                        ("Для створення ролі користувача", false),
                        ("Для очищення ViewBag", false)),

                    Question("Що може містити клас?", 2, true,
                        ("Поля", true),
                        ("Властивості", true),
                        ("Методи", true),
                        ("SQL Server Management Studio", false))
                }
            }
        };

        db.Tests.AddRange(tests);

        db.EduEvents.AddRange(
            new EduEvent
            {
                Title = "Консультація з дипломного проєкту",
                Description = "Обговорення структури дипломного проєкту, ролей користувачів, моделей бази даних і демонстраційного сценарію.",
                Location = "Аудиторія 304",
                StartAt = DateTime.Now.AddDays(3).Date.AddHours(14),
                EndAt = DateTime.Now.AddDays(3).Date.AddHours(16),
                RegistrationAvailableFrom = DateTime.Now.AddDays(-2),
                RegistrationAvailableUntil = DateTime.Now.AddDays(2),
                Capacity = 20,
                IsRegistrationOpen = true,
                CreatedById = teacher1.Id
            },
            new EduEvent
            {
                Title = "Онлайн-консультація з ASP.NET Core",
                Description = "Жива консультація по ASP.NET Core MVC, Entity Framework Core, ролях користувачів та системі тестування.",
                Location = "Google Meet",
                StartAt = DateTime.Now.AddHours(-1),
                EndAt = DateTime.Now.AddHours(5),
                RegistrationAvailableFrom = DateTime.Now.AddDays(-2),
                RegistrationAvailableUntil = DateTime.Now.AddHours(1),
                Capacity = 40,
                IsRegistrationOpen = true,
                CreatedById = teacher1.Id
            },
            new EduEvent
            {
                Title = "Практикум з Entity Framework Core",
                Description = "Розбір міграцій, зв'язків між сутностями, помилок під час Update-Database і створення початкових даних.",
                Location = "Комп'ютерна лабораторія 212",
                StartAt = DateTime.Now.AddDays(5).Date.AddHours(10),
                EndAt = DateTime.Now.AddDays(5).Date.AddHours(12),
                RegistrationAvailableFrom = DateTime.Now.AddDays(2),
                RegistrationAvailableUntil = DateTime.Now.AddDays(7),
                Capacity = 18,
                IsRegistrationOpen = true,
                CreatedById = teacher1.Id
            },
            new EduEvent
            {
                Title = "Захист лабораторних робіт з КПЗ",
                Description = "Індивідуальна перевірка лабораторних робіт, відповідей на контрольні питання та демонстрація працездатності програм.",
                Location = "Аудиторія 118",
                StartAt = DateTime.Now.AddDays(-2).Date.AddHours(12),
                EndAt = DateTime.Now.AddDays(-2).Date.AddHours(14),
                RegistrationAvailableFrom = DateTime.Now.AddDays(-10),
                RegistrationAvailableUntil = DateTime.Now.AddDays(-3),
                Capacity = 25,
                IsRegistrationOpen = true,
                CreatedById = teacher2.Id
            },
            new EduEvent
            {
                Title = "Майстер-клас з Playwright",
                Description = "Написання E2E-тестів, запуск тестів у браузері, створення HTML-звіту та аналіз помилок.",
                Location = "Онлайн, Google Meet",
                StartAt = DateTime.Now.AddDays(10).Date.AddHours(16),
                EndAt = DateTime.Now.AddDays(10).Date.AddHours(18),
                RegistrationAvailableFrom = DateTime.Now.AddDays(-1),
                RegistrationAvailableUntil = DateTime.Now.AddDays(8),
                Capacity = 30,
                IsRegistrationOpen = true,
                CreatedById = teacher2.Id
            },
            new EduEvent
            {
                Title = "Семінар з інформаційної безпеки",
                Description = "Розгляд автентифікації, авторизації, ролей користувачів, шифрування та базових ризиків вебзастосунків.",
                Location = "Аудиторія 401",
                StartAt = DateTime.Now.AddDays(12).Date.AddHours(13),
                EndAt = DateTime.Now.AddDays(12).Date.AddHours(15),
                RegistrationAvailableFrom = DateTime.Now.AddDays(4),
                RegistrationAvailableUntil = DateTime.Now.AddDays(11),
                Capacity = 22,
                IsRegistrationOpen = true,
                CreatedById = teacher3.Id
            }
        );

        await db.SaveChangesAsync();

        var allTests = await db.Tests.Include(t => t.Questions).ToListAsync();

        var students = new List<ApplicationUser>
        {
            student1,
            student2,
            student3,
            student4,
            student5,
            student6,
            student7,
            student8
        };

        var random = new Random(12);

        foreach (var test in allTests)
        {
            var selectedStudents = students
                .OrderBy(_ => random.Next())
                .Take(random.Next(4, 8))
                .ToList();

            foreach (var student in selectedStudents)
            {
                var maxScore = test.Questions.Sum(q => q.Points);
                var score = random.Next(0, maxScore + 1);
                var finishedAt = DateTime.Now.AddDays(-random.Next(1, 25)).AddHours(-random.Next(1, 8));
                var startedAt = finishedAt.AddMinutes(-random.Next(8, Math.Max(9, test.TimeLimitMinutes)));

                db.TestAttempts.Add(new TestAttempt
                {
                    TestId = test.Id,
                    StudentId = student.Id,
                    Score = score,
                    MaxScore = maxScore,
                    StartedAt = startedAt,
                    FinishedAt = finishedAt
                });
            }
        }

        var events = await db.EduEvents.ToListAsync();

        foreach (var eduEvent in events)
        {
            var registeredStudents = students
                .OrderBy(_ => random.Next())
                .Take(random.Next(3, 7))
                .ToList();

            foreach (var student in registeredStudents)
            {
                db.EventRegistrations.Add(new EventRegistration
                {
                    EduEventId = eduEvent.Id,
                    StudentId = student.Id,
                    RegisteredAt = DateTime.Now.AddDays(-random.Next(1, 10)),
                    Status = RegistrationStatus.Confirmed
                });
            }
        }

        db.AuditLogs.AddRange(
            new AuditLog { UserId = superAdmin.Id, Action = "Створено початкові ролі системи", CreatedAt = DateTime.Now.AddDays(-15) },
            new AuditLog { UserId = admin.Id, Action = "Додано навчальні дисципліни та демонстраційних користувачів", CreatedAt = DateTime.Now.AddDays(-12) },
            new AuditLog { UserId = teacher1.Id, Action = "Опубліковано тести з ASP.NET Core MVC та Entity Framework Core", CreatedAt = DateTime.Now.AddDays(-9) },
            new AuditLog { UserId = teacher2.Id, Action = "Підготовлено тести з SOLID і тестування програмного забезпечення", CreatedAt = DateTime.Now.AddDays(-7) },
            new AuditLog { UserId = teacher3.Id, Action = "Створено тестові матеріали з баз даних та інформаційної безпеки", CreatedAt = DateTime.Now.AddDays(-5) }
        );

        await db.SaveChangesAsync();
    }

    private static Question Question(string text, int points, bool allowMultipleAnswers, params (string Text, bool IsCorrect)[] options)
    {
        var question = new Question
        {
            Text = text,
            Points = points,
            AllowMultipleAnswers = allowMultipleAnswers
        };

        foreach (var option in options)
        {
            question.AnswerOptions.Add(new AnswerOption
            {
                Text = option.Text,
                IsCorrect = option.IsCorrect
            });
        }

        return question;
    }

    private static async Task<ApplicationUser> CreateUser(UserManager<ApplicationUser> userManager, string email, string fullName, string role)
    {
        var user = await userManager.FindByEmailAsync(email);

        if (user == null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = fullName,
                EmailConfirmed = true
            };

            await userManager.CreateAsync(user, "123456");
        }

        if (!await userManager.IsInRoleAsync(user, role))
            await userManager.AddToRoleAsync(user, role);

        return user;
    }
}
