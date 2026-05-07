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
            if (!await roleManager.RoleExistsAsync(role)) await roleManager.CreateAsync(new IdentityRole(role));

        await CreateUser(userManager, "superadmin@edu.local", "Суперадмін системи", "SuperAdmin");
        await CreateUser(userManager, "admin@edu.local", "Адміністратор коледжу", "Admin");
        var teacher = await CreateUser(userManager, "teacher@edu.local", "Олена Шевчук", "Teacher");
        await CreateUser(userManager, "student@edu.local", "Марія Студент", "Student");

        if (!await db.Subjects.AnyAsync())
        {
            var subject = new Subject { Name = "Конструювання програмного забезпечення", Description = "Навчальна дисципліна для демонстраційного модуля тестування." };
            var test = new Test { Title = "Основи ASP.NET Core MVC", Description = "Короткий тест для перевірки базових знань MVC та EF Core.", TimeLimitMinutes = 15, Subject = subject, AuthorId = teacher.Id, IsPublished = true };
            test.Questions.Add(new Question { Text = "Що відповідає за роботу з даними у патерні MVC?", Points = 1, AnswerOptions = { new AnswerOption { Text = "Model", IsCorrect = true }, new AnswerOption { Text = "View" }, new AnswerOption { Text = "Layout" } } });
            test.Questions.Add(new Question { Text = "Які можливості надає Entity Framework Core?", AllowMultipleAnswers = true, Points = 2, AnswerOptions = { new AnswerOption { Text = "Роботу з базою через об'єкти", IsCorrect = true }, new AnswerOption { Text = "Міграції бази даних", IsCorrect = true }, new AnswerOption { Text = "Компіляцію CSS" } } });
            db.Tests.Add(test);
            db.EduEvents.Add(new EduEvent { Title = "Консультація з дипломного проєкту", Description = "Зустріч для обговорення структури, функціоналу та демонстраційних сценаріїв проєкту.", Location = "Аудиторія 304", StartAt = DateTime.Today.AddDays(3).AddHours(14), Capacity = 20, CreatedById = teacher.Id });
            await db.SaveChangesAsync();
        }
    }

    private static async Task<ApplicationUser> CreateUser(UserManager<ApplicationUser> userManager, string email, string fullName, string role)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            user = new ApplicationUser { UserName = email, Email = email, FullName = fullName, EmailConfirmed = true };
            await userManager.CreateAsync(user, "123456");
        }
        if (!await userManager.IsInRoleAsync(user, role)) await userManager.AddToRoleAsync(user, role);
        return user;
    }
}
