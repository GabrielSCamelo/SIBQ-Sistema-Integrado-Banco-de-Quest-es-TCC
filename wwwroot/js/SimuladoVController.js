app.controller('SimuladoVController', ['$scope', '$timeout', function ($scope, $timeout) {

    $scope.isProfessor = window.isProfessor;
    $scope.totalQuestoes = window.simuladoData?.questoesOrdem?.length || 0;
    $scope.desempenhoData = window.desempenhoData || {};
    $scope.mediaPorTurma = window.mediaPorTurma || {};

    // Paginação
    $scope.paginaAtual = 0;
    $scope.porPagina = 5;

    $scope.isQuestaoVisivel = function (index) {
        let start = $scope.paginaAtual * $scope.porPagina;
        let end = start + $scope.porPagina;
        return index >= start && index < end;
    };

    $scope.proximaPagina = function () {
        if (($scope.paginaAtual + 1) * $scope.porPagina < $scope.totalQuestoes) {
            $scope.paginaAtual++;
        }
    };

    $scope.voltarPagina = function () {
        if ($scope.paginaAtual > 0) {
            $scope.paginaAtual--;
        }
    };

    $scope.turmas = Object.keys($scope.desempenhoData);
    $scope.turmaSelecionada = null;
    $scope.dadosDesempenho = [];
    $scope.totalAlunosSelecionado = 0;

    let grafico = null;

    function criarGrafico(labels, dados, cores) {
        const ctx = document.getElementById("graficoDesempenho").getContext("2d");

        if (grafico) grafico.destroy();

        grafico = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: labels,
                datasets: [{
                    label: '% de alunos com ≥ 60% de acertos',
                    data: dados,
                    backgroundColor: cores
                }]
            },
            options: {
                scales: {
                    y: {
                        beginAtZero: true,
                        max: 100,
                        ticks: {
                            callback: function (value) { return value + '%'; }
                        }
                    }
                },
                plugins: {
                    legend: { display: false }
                }
            }
        });
    }

    // Função só para gráfico geral, mostra TODAS as turmas SEM filtro de seleção
    $scope.atualizarGraficoGeral = function () {
        const labels = [];
        const valores = [];
        const cores = [];

        for (const [turma, dadosTurma] of Object.entries($scope.mediaPorTurma)) {
            const total = dadosTurma.TotalAlunos || 1;
            const acima60 = dadosTurma.AcimaDe60 || 0;
            const pct = (acima60 / total) * 100;

            labels.push(`${turma} (${total} Alunos)`);
            valores.push(pct.toFixed(1));

            cores.push(
                pct < 30 ? "#e74c3c" :
                    pct < 70 ? "#f1c40f" :
                        "#27ae60"
            );
        }

        criarGrafico(labels, valores, cores);
    };

    // Função que atualiza a tabela por turma selecionada e também atualiza o total de alunos da turma no cabeçalho
    $scope.atualizarTabelaDesempenho = function () {
        if (!$scope.turmaSelecionada) {
            $scope.dadosDesempenho = [];
            $scope.totalAlunosSelecionado = 0;
            return;
        }

        const dados = $scope.desempenhoData[$scope.turmaSelecionada] || [];

        $scope.dadosDesempenho = dados
            .filter(q => q.Acertos > 0 || q.Total > 0)
            .map(q => {
                const pct = q.Total > 0 ? (q.Acertos / q.Total * 100) : 0;
                return {
                    QuestaoNumero: q.QuestaoNumero,
                    Acertos: q.Acertos,
                    Total: q.Total,
                    PctAcerto: pct.toFixed(1)
                };
            });

        $scope.totalAlunosSelecionado = $scope.mediaPorTurma[$scope.turmaSelecionada]?.TotalAlunos || 0;
    };

    angular.element(document).ready(function () {
        // Cria o gráfico geral logo que a página abre
        $scope.atualizarGraficoGeral();

        if ($scope.turmas.length > 0) {
            $scope.turmaSelecionada = $scope.turmas[0];
            $scope.atualizarTabelaDesempenho();
        }
    });
}]);