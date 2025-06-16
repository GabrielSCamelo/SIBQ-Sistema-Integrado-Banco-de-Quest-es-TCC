using Microsoft.AspNetCore.Identity;

namespace SIBQ.Service
{
    public class Roles_DB
    {
        public static class Roles
        {
            public const string Aluno = "Aluno";
            public const string Professor = "Professor";
        }

        public static class RoleInitializer
        {
            public static async Task CreateRolesAsync(IServiceProvider serviceProvider)
            {
                using var scope = serviceProvider.CreateScope();
                var services = scope.ServiceProvider;
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                // Defina os nomes das roles
                string[] roleNames = { Roles.Aluno, Roles.Professor };

                foreach (var roleName in roleNames)
                {
                    if (!await roleManager.RoleExistsAsync(roleName))
                    {
                        await roleManager.CreateAsync(new IdentityRole(roleName));
                    }
                }
            }
        }

    }
}
