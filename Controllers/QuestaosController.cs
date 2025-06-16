using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIBQ.DataBase;
using SIBQ.Models;
using System.Security.Claims;

namespace SIBQ.Controllers
{
    public class QuestaosController : Controller
    {
        private readonly ContextDb _context;

        public QuestaosController(ContextDb context)
        {
            _context = context;
        }

        private string GetUsuarioId() => User.FindFirstValue(ClaimTypes.NameIdentifier);
        private async Task<List<Disciplina>> ObterDisciplinasAsync() => await _context.Disciplinas.ToListAsync();
        private List<Disciplina> ObterDisciplinas() => _context.Disciplinas.ToList();
        private bool UsuarioEhProfessor(string usuarioId) => _context.Professores.Any(p => p.Id == usuarioId);

        // GET: /Questaos/Questoes
        [HttpGet]
        public IActionResult Questoes(QuestaoViewModel model)
        {
            model.Disciplinas = ObterDisciplinas();
            return View(model);
        }

        // GET: /Questaos/Deletar/5
        [HttpGet]
        public async Task<IActionResult> Deletar(int id)
        {
            var questao = await _context.Questoes.FindAsync(id);
            if (questao == null || id == 0)
            {
                TempData["Warning"] = "ERRO: Questão não encontrada.";
            }
            else
            {
                var sendoUtilizada = await _context.simulado_Questaos.AnyAsync(p => p.QuestaoId == id);
                if (sendoUtilizada)
                {
                    TempData["Warning"] = "ERRO: Simulado está utilizando a Questão";
                }
                else
                {
                    _context.Questoes.Remove(questao);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Questão deletada com sucesso.";
                }
            }

            return RedirectToAction("ListaQuestoes");
        }

        // GET: /Questaos/ListaQuestoes
        [HttpGet]
        public async Task<IActionResult> ListaQuestoes(QuestaoViewModel model)
        {
            var usuarioId = GetUsuarioId();
            ViewBag.IsProfessor = UsuarioEhProfessor(usuarioId);
            model.Disciplinas = await ObterDisciplinasAsync();
            return View(model);
        }

        // GET: /Questaos/Pesquisar?titulo=...&disciplina=...
        [HttpGet]
        public IActionResult Pesquisar(string titulo, int? disciplina)
        {
            var query = _context.Questoes.Include(q => q.Disciplina).AsQueryable();

            if (!string.IsNullOrWhiteSpace(titulo))
                query = query.Where(q => q.Titulo.Contains(titulo));

            if (disciplina.HasValue && disciplina.Value > 0)
                query = query.Where(q => q.DisciplinaId == disciplina.Value);

            var resultado = query.Select(q => new
            {
                id = q.Id,
                titulo = q.Titulo,
                disciplinaNome = q.Disciplina.Nome
            }).ToList();

            return Json(resultado.OrderBy(p => p.titulo));
        }

        // GET: /Questaos/Visualizar/5
        [HttpGet]
        public IActionResult Visualizar(int id)
        {
            var questao = _context.Questoes.Include(q => q.Disciplina).FirstOrDefault(q => q.Id == id);
            if (questao == null) return NotFound();

            var usuarioId = GetUsuarioId();
            ViewBag.IsProfessor = UsuarioEhProfessor(usuarioId);

            return View(questao);
        }

        // GET: /Questaos/Alterar
        [HttpGet]
        public async Task<IActionResult> Alterar(int id)
        {
            var questao = await _context.Questoes.Include(p => p.Disciplina).FirstOrDefaultAsync(p => p.Id == id);
            if (questao == null)
            {
                TempData["Warning"] = "ERRO: Falha ao carregar a questão.";
                return RedirectToAction("Questoes");
            }

            var model = new QuestaoViewModel
            {
                Questao = questao,
                Disciplinas = await ObterDisciplinasAsync()
            };

            return View("Questoes", model);  // Reutilizando a View
        }

        // POST: /Questaos/CadastrarQuestao
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CadastrarQuestao(QuestaoViewModel model)
        {
            if (model == null)
            {
                ModelState.AddModelError("", "Dados inválidos.");
                TempData["Warning"] = "ERRO: Dados inválidos.";
                return RedirectToAction("Questoes");
            }

            var disciplinaExiste = await _context.Disciplinas.FindAsync(model.Questao.DisciplinaId) != null;
            if (!disciplinaExiste)
            {
                ModelState.AddModelError("", "Disciplina inválida.");
                TempData["Warning"] = "ERRO: Disciplina inválida.";
                return RedirectToAction("Questoes");
            }

            if (model.Questao.Id == 0 && !await _context.Questoes.AnyAsync(p => p.Enunciado == model.Questao.Enunciado ||
            p.DisciplinaId == model.DisciplinaId ||
            p.OpcaoA == model.Questao.OpcaoA ||
            p.OpcaoB == model.Questao.OpcaoB ||
            p.OpcaoC == model.Questao.OpcaoC ||
            p.OpcaoD == model.Questao.OpcaoD ||
            p.OpcaoE == model.Questao.OpcaoE ||
            p.Titulo == model.Questao.Titulo ||
            p.OpcaoA == model.Questao.OpcaoA))
            {
                _context.Questoes.Add(model.Questao);
                TempData["Success"] = "Questão cadastrada com sucesso.";
            }
            else
            {
                _context.Questoes.Update(model.Questao);
                TempData["Success"] = "Questão editada com sucesso.";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Questoes");
        }
    }
}