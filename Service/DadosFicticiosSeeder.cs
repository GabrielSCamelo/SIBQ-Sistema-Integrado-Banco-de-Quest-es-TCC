using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SIBQ.DataBase;
using SIBQ.Models;
using static SIBQ.Service.Roles_DB;

public class DadosFicticiosSeeder
{
    public static async Task SeedAsync(ContextDb context, UserManager<IdentityUser> userManager)
    {
        try
        {
            if (
                !await context.Disciplinas.AnyAsync() ||
                !await context.Turmas.AnyAsync() ||
                !await context.Professores.AnyAsync() ||
                !await context.Alunos.AnyAsync()
            )
            {
                // 1. Criar Disciplinas
                var nomesDisciplinas = new[]
                {
                    "Algoritmos e Lógica de Programação", "Estrutura de Dados",
                    "Banco de Dados", "Programação Orientada a Objetos",
                    "Engenharia de Software"
                };

                var disciplinas = new List<Disciplina>();
                foreach (var nome in nomesDisciplinas)
                {
                    var sigla = new string(nome.Where(char.IsLetter).Take(3).ToArray()).ToUpper();
                    if (!await context.Disciplinas.AnyAsync(d => d.Nome == nome))
                    {
                        var disciplina = new Disciplina { Nome = nome, Sigla = sigla };
                        context.Disciplinas.Add(disciplina);
                        disciplinas.Add(disciplina);
                    }
                }
                await context.SaveChangesAsync();

                // 2. Criar Professores
                var professores = new List<Professor>();
                for (int i = 1; i <= 5; i++)
                {
                    var username = $"Professor{i}";
                    if (await userManager.FindByNameAsync(username) == null)
                    {
                        var professor = new Professor
                        {
                            IdFuncional = $"P{i}",
                            UserName = username,
                            Email = $"professor{i}@example.com",
                            EmailConfirmed = true
                        };
                        var result = await userManager.CreateAsync(professor, "Senha123!");
                        if (result.Succeeded)
                        {
                            await userManager.AddToRoleAsync(professor, Roles.Professor);
                            professores.Add(professor);
                        }
                    }
                }
                await context.SaveChangesAsync();

                // 3. Criar Turmas
                var disciplinasSalvas = await context.Disciplinas.ToListAsync();
                var professoresSalvos = await context.Professores.ToListAsync();

                for (int i = 0; i < 5; i++)
                {
                    var disciplina = disciplinasSalvas.ElementAtOrDefault(i);
                    var professor = professoresSalvos.ElementAtOrDefault(i);

                    if (disciplina == null || professor == null) continue;

                    if (!await context.Turmas.AnyAsync(p => p.DisciplinaId == disciplina.Id))
                    {
                        var turma = new Turma
                        {
                            DisciplinaId = disciplina.Id,
                            Periodo = $"2025.{i + 1}",
                            ProfessorId = professor.Id,
                            aluno_Turmas = new List<Aluno_Turma>()
                        };
                        context.Turmas.Add(turma);
                    }
                }
                await context.SaveChangesAsync();

                // 4. Criar Alunos
                var turmasSalvas = await context.Turmas.ToListAsync();

                for (int i = 1; i <= 150; i++)
                {
                    var username = $"Aluno{i}";
                    if (await userManager.FindByNameAsync(username) == null)
                    {
                        var aluno = new Aluno
                        {
                            Matricula = $"A{i}",
                            UserName = username,
                            Email = $"aluno{i}@example.com",
                            EmailConfirmed = true
                        };
                        var result = await userManager.CreateAsync(aluno, "Senha123!");
                        if (result.Succeeded)
                        {
                            await userManager.AddToRoleAsync(aluno, Roles.Aluno);
                            var turma = turmasSalvas[(i - 1) % turmasSalvas.Count];
                            context.Alunos_Turmas.Add(new Aluno_Turma
                            {
                                AlunoId = aluno.Id,
                                TurmaId = turma.Id
                            });
                        }
                    }
                }
                await context.SaveChangesAsync();

                Console.WriteLine("Dados fictícios inseridos com sucesso.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao gerar dados fictícios: {ex.Message}");
        }
    }
}