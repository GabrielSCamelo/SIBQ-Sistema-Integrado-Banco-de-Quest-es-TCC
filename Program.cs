using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SIBQ.DataBase;
using SIBQ.Service;

var builder = WebApplication.CreateBuilder(args);

// Adicione serviços ao contêiner.
builder.Services.AddControllersWithViews();

// Adicione serviços de páginas Razor (necessário para registrar todos os serviços necessários)
builder.Services.AddRazorPages();

builder.Services.Configure<FormOptions>(options =>
{
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartBodyLengthLimit = int.MaxValue;
    options.MemoryBufferThreshold = int.MaxValue;
});


// Configurar o banco de dados
builder.Services.AddDbContext<ContextDb>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(10, 4, 32))));

// Configurar serviços de identidade
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
})
.AddEntityFrameworkStores<ContextDb>()
.AddDefaultTokenProviders();

var app = builder.Build();

// Chame o método para criar roles
using (var scope = app.Services.CreateScope())

{
    var services = scope.ServiceProvider;
    try
    {
        // Criar roles no banco
        await Roles_DB.RoleInitializer.CreateRolesAsync(services);

        // Obter o contexto e o UserManager do serviço
        var context = services.GetRequiredService<ContextDb>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

        // Chama o método para gerar dados fictícios
        await DadosFicticiosSeeder.SeedAsync(context, userManager);
    }
    catch (Exception ex)
    {
        // Trate as exceções de acordo
        Console.WriteLine($"Erro ao criar roles ou dados fictícios: {ex.Message}");
    }
}

// Configure o pipeline de solicitação HTTP.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Adiciona o middleware de autenticação
app.UseAuthorization();
app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Login}");

app.Run();