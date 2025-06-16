using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIBQ.DataBase;
using SIBQ.Models;
using System.Security.Claims;

namespace SIBQ.Controllers
{
    public class SimuladoController : Controller
    {
        private readonly ContextDb _context;

        public SimuladoController(ContextDb context)
        {
            _context = context;
        }

        #region Métodos Privados

        private string GetUsuarioId() => User.FindFirstValue(ClaimTypes.NameIdentifier);

        private async Task<bool> IsProfessorAsync(string usuarioId)
        {
            return await _context.Professores.AnyAsync(p => p.Id == usuarioId);
        }

        private IQueryable<Simulado> FiltrarSimuladosPorUsuario(IQueryable<Simulado> query, string usuarioId, string dataCriacaoStr, string dataExpiracaoStr)
        {
            if (DateTime.TryParse(dataCriacaoStr, out DateTime dataCriacao))
            {
                query = query.Where(s => s.DataCriacao.Date >= dataCriacao.Date);
            }

            if (DateTime.TryParse(dataExpiracaoStr, out DateTime dataExpiracao))
            {
                query = query.Where(s => s.DataExpiracao.Date <= dataExpiracao.Date);
            }

            query = query.Where(s => s.CriadorId == usuarioId);

            return query;
        }

        private async Task<List<Simulado>> AdicionarSimuladosPorTurmasAlunoAsync(List<Simulado> simulados, string usuarioId)
        {
            var turmasAluno = await _context.Alunos_Turmas.Where(p => p.AlunoId == usuarioId).ToListAsync();
            var todosSimulados = await _context.Simulados.ToListAsync();

            foreach (var turmaAluno in turmasAluno)
            {
                var simuladoRelacionado = todosSimulados.FirstOrDefault(s => s.Turmas.Contains(turmaAluno.TurmaId));
                if (simuladoRelacionado != null && !simulados.Any(s => s.Id == simuladoRelacionado.Id))
                {
                    simulados.Add(simuladoRelacionado);
                }
            }

            return simulados;
        }

        private async Task AtualizarStatusSimuladoAlunoAsync(List<Simulado> simulados, string usuarioId)
        {
            foreach (var simulado in simulados)
            {
                simulado.FoiRealizado = await _context.Simulados_Alunos
                    .AnyAsync(sa => sa.SimuladoId == simulado.Id && sa.AlunoId == usuarioId);
            }
        }

        private async Task AtualizarStatusSimuladoProfessorAsync(List<Simulado> simulados)
        {
            foreach (var simulado in simulados)
            {
                var turmas = simulado.Turmas;

                if (turmas.Count == 0)
                {
                    simulado.FoiRealizado = false;
                    continue;
                }

                bool todosFizeram = true;

                foreach (var turmaId in turmas)
                {
                    var alunos = await _context.Alunos_Turmas
                        .Where(at => at.TurmaId == turmaId)
                        .Select(at => at.AlunoId)
                        .ToListAsync();

                    foreach (var alunoId in alunos)
                    {
                        bool fez = await _context.Simulados_Alunos
                            .AnyAsync(sa => sa.SimuladoId == simulado.Id && sa.AlunoId == alunoId);
                        if (!fez)
                        {
                            todosFizeram = false;
                            break;
                        }
                    }

                    if (!todosFizeram)
                        break;
                }

                simulado.FoiRealizado = todosFizeram;
            }
        }

        private async Task<Dictionary<string, List<QuestaoDesempenho>>> ObterDesempenhoPorTurmaAsync(Simulado simulado, int simuladoId)
        {
            var alunosSimulado = await _context.Simulados_Alunos
                .Where(sa => sa.SimuladoId == simuladoId)
                .Include(sa => sa.Aluno)
                    .ThenInclude(a => a.Turmas)
                        .ThenInclude(at => at.Turma)
                .Include(sa => sa.Respostas)
                .ToListAsync();

            var desempenhoPorTurma = new Dictionary<string, List<QuestaoDesempenho>>();

            var alunosPorTurma = alunosSimulado
                .SelectMany(sa => sa.Aluno.Turmas.Select(t => new { TurmaNome = t.Turma.Disciplina.Nome + " " + t.Turma.Periodo, SimuladoAluno = sa }))
                .GroupBy(x => x.TurmaNome);

            foreach (var grupo in alunosPorTurma)
            {
                var desempenho = new List<QuestaoDesempenho>();

                foreach (var sq in simulado.simulado_Questaos.OrderBy(q => q.Questao.Id))
                {
                    int questaoId = sq.Questao.Id;
                    int acertos = 0, total = 0;

                    foreach (var item in grupo)
                    {
                        var resposta = item.SimuladoAluno.Respostas.FirstOrDefault(r => r.QuestaoId == questaoId);
                        if (resposta != null)
                        {
                            total++;
                            if (resposta.Resposta == sq.Questao.OpcaoCorretaIndex)
                                acertos++;
                        }
                    }

                    desempenho.Add(new QuestaoDesempenho
                    {
                        QuestaoNumero = sq.Questao.Id,
                        Acertos = acertos,
                        Total = total
                    });
                }

                desempenhoPorTurma[grupo.Key] = desempenho;
            }

            return desempenhoPorTurma;
        }

        #endregion

        #region Actions Públicas

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PesquisarSimulados([FromBody] SimuladoViewModel model)
        {
            var usuarioId = GetUsuarioId();
            var isProfessor = await IsProfessorAsync(usuarioId);

            var query = _context.Simulados.AsQueryable();
            query = FiltrarSimuladosPorUsuario(query, usuarioId, model.DataCriacao, model.DataExpiracao);

            var simulados = await query.ToListAsync();

            if (!isProfessor)
            {
                simulados = await AdicionarSimuladosPorTurmasAlunoAsync(simulados, usuarioId);
                await AtualizarStatusSimuladoAlunoAsync(simulados, usuarioId);
            }
            else
            {
                await AtualizarStatusSimuladoProfessorAsync(simulados);
            }

            var result = simulados.Select(s => new
            {
                id = s.Id,
                criadorId = s.CriadorId,
                idUsuario = usuarioId,
                titulo = s.Titulo,
                ativo = s.Ativo,
                dataCriacao = s.DataCriacao,
                dataExpiracao = s.DataExpiracao,
                foiRealizado = s.FoiRealizado,
            }).ToList();

            var ordenado = result.OrderBy(p => p.criadorId != usuarioId && p.foiRealizado == false && p.ativo);

            return Json(ordenado);
        }

        [HttpGet]
        public IActionResult ListaSimulados()
        {
            var usuarioId = GetUsuarioId();
            var isProfessor = _context.Professores.Any(p => p.Id == usuarioId);

            ViewBag.IsProfessor = isProfessor;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> VSimulado(int id)
        {
            var usuarioId = GetUsuarioId();
            var isProfessor = _context.Professores.Any(p => p.Id == usuarioId);

            ViewBag.IsProfessor = isProfessor;

            var simulado = await _context.Simulados
                .Include(s => s.simulado_Questaos)
                    .ThenInclude(sq => sq.Questao).ThenInclude(sq => sq.Disciplina)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (simulado == null) return NotFound();

            if (isProfessor)
            {
                ViewBag.DesempenhoPorTurma = await ObterDesempenhoPorTurmaAsync(simulado, id);
            }
            else
            {
                var respostas = await _context.Simulados_Alunos
                    .Include(p => p.Respostas)
                    .FirstOrDefaultAsync(p => p.AlunoId == usuarioId);
                simulado.simulado_Alunos.Add(respostas);
            }

            return View(simulado);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalvarRespostas(SimuladoViewModel model)
        {
            var usuarioId = GetUsuarioId();

            var simuladoAluno = new Simulado_Aluno
            {
                SimuladoId = model.Simulado.Id,
                AlunoId = usuarioId,
                Respostas = new List<Simulado_Aluno_Resposta>()
            };

            if (model.Respostas != null)
            {
                foreach (var resposta in model.Respostas)
                {
                    if (int.TryParse(resposta.Key, out int questaoId))
                    {
                        simuladoAluno.Respostas.Add(new Simulado_Aluno_Resposta
                        {
                            QuestaoId = questaoId,
                            Resposta = resposta.Value
                        });
                    }
                }
            }

            _context.Add(simuladoAluno);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Simulado Finalizado com sucesso!";
            return RedirectToAction("ListaSimulados", "Simulado");
        }

        [HttpGet]
        public async Task<IActionResult> FSimulado(int id)
        {
            var simulado = new SimuladoViewModel
            {
                Simulado = await _context.Simulados
                    .Include(s => s.simulado_Questaos)
                        .ThenInclude(sq => sq.Questao)
                    .FirstOrDefaultAsync(s => s.Id == id)
            };

            if (simulado == null) return NotFound();

            return View(simulado);
        }

        [HttpGet]
        public async Task<IActionResult> GetDetailsAsync([FromQuery] int? id)
        {
            if (id == null) return NotFound();

            var simulado = await _context.Simulados.FindAsync(id);

            if (simulado == null) return NotFound();

            return Ok(simulado);
        }

        [HttpGet]
        public async Task<IActionResult> ExcluirSimulado(int id)
        {
            var simulado = await _context.Simulados.FindAsync(id);

            if (simulado == null)
            {
                TempData["Warning"] = "ERRO: não foi encontrado o simulado.";
            }
            else
            {
                var algumAlunoFez = await _context.Simulados_Alunos.AnyAsync(sa => sa.SimuladoId == id);
                if (algumAlunoFez)
                {
                    TempData["Warning"] = "ERRO: Um Aluno Já fez o Simulado.";
                }
                else
                {
                    _context.Simulados.Remove(simulado);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Simulado Excluido com Sucesso";
                }
            }

            return RedirectToAction("ListaSimulados");
        }

        [HttpGet]
        public async Task<IActionResult> CriarSimulado(CriarSimuladoViewModel model)
        {
            var usuarioId = GetUsuarioId();
            var isProfessor = _context.Professores.Any(p => p.Id == usuarioId);
            ViewBag.IsProfessor = isProfessor;

            model.DisciplinasDisponiveis = await _context.Disciplinas.ToListAsync();
            model.TurmasDisponiveis = await _context.Turmas.Where(p => p.ProfessorId == usuarioId).ToListAsync();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CadastrarSimulado(CriarSimuladoViewModel model)
        {
            if (model.QuantidadeQuestoes < 5 || model.QuantidadeQuestoes > 25)
            {
                TempData["Warning"] = "ERRO: a quantidade de Questões tem q estar entre 5 - 25";
                return RedirectToAction("CriarSimulado");
            }

            var questoes = await _context.Questoes
                .Where(q => model.DisciplinasSelecionadas.Contains(q.Disciplina.Id))
                .OrderBy(_ => Guid.NewGuid())
                .Take(model.QuantidadeQuestoes)
                .ToListAsync();

            if (questoes.Count < model.QuantidadeQuestoes)
            {
                TempData["Warning"] = "ERRO: Banco de dados não tem questões suficiente.";
                return RedirectToAction("CriarSimulado");
            }

            var usuarioId = GetUsuarioId();
            var isProfessor = _context.Professores.Any(p => p.Id == usuarioId);
            ViewBag.IsProfessor = isProfessor;

            model.Simulado.Titulo = model.Titulo;
            model.Simulado.QuantidadeQuestoes = model.QuantidadeQuestoes;
            model.Simulado.disciplinas = model.DisciplinasSelecionadas;
            model.Simulado.CriadorId = usuarioId;
            model.Simulado.CriadoPorAluno = !isProfessor;

            if (isProfessor)
            {
                model.Simulado.Turmas = model.TurmasSelecionadas;
            }

            _context.Simulados.Add(model.Simulado);
            await _context.SaveChangesAsync();

            foreach (var questao in questoes)
            {
                _context.simulado_Questaos.Add(new Simulado_Questao
                {
                    SimuladoId = model.Simulado.Id,
                    QuestaoId = questao.Id
                });
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Simulado criado com sucesso!";
            return RedirectToAction("ListaSimulados");
        }

        #endregion
    }

}