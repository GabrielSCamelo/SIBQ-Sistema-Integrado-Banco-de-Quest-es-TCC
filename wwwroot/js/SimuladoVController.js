app.controller('SimuladoVController', ['$scope', '$timeout', function ($scope, $timeout) {
    const vm = this;

    // Dados globais setados no window pelo backend
    vm.isProfessor = window.isProfessor;
    vm.totalQuestoes = window.totalQuestoes;
    vm.desempenhoData = window.desempenhoData || {};

    // Paginação
    vm.paginaAtual = 0;
    vm.porPagina = 5;

    vm.isQuestaoVisivel = function (index) {
        const start = vm.paginaAtual * vm.porPagina;
        const end = start + vm.porPagina;
        return index >= start && index < end;
    };

    vm.proximaPagina = function () {
        if ((vm.paginaAtual + 1) * vm.porPagina < vm.totalQuestoes) {
            vm.paginaAtual++;
        }
    };

    vm.voltarPagina = function () {
        if (vm.paginaAtual > 0) {
            vm.paginaAtual--;
        }
    };

    // Relatório desempenho
    vm.turmas = Object.keys(vm.desempenhoData);
    vm.turmaSelecionada = vm.turmas.length ? vm.turmas[0] : null;
    vm.dadosDesempenho = [];

    let grafico = null;

    function criarGrafico(labels, percentuais, cores) {
        const ctx = document.getElementById("graficoDesempenho").getContext("2d");

        if (grafico) grafico.destroy();

        grafico = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: labels,
                datasets: [{
                    label: '% de Acertos',
                    data: percentuais,
                    backgroundColor: cores
                }]
            },
            options: {
                scales: {
                    y: {
                        min: 0,
                        max: 100,
                        ticks: {
                            callback: function (value) { return value + '%' }
                        }
                    }
                },
                plugins: {
                    legend: { display: false }
                }
            }
        });
    }

    vm.atualizarTabelaDesempenho = function () {
        if (!vm.turmaSelecionada) {
            vm.dadosDesempenho = [];
            if (grafico) grafico.destroy();
            return;
        }

        const dados = vm.desempenhoData[vm.turmaSelecionada] || [];
        vm.dadosDesempenho = dados.map(q => {
            const pct = q.Total > 0 ? (q.Acertos / q.Total * 100) : 0;
            return {
                QuestaoNumero: q.QuestaoNumero,
                Acertos: q.Acertos,
                Total: q.Total,
                PctAcerto: pct.toFixed(1)
            };
        });

        const labels = vm.dadosDesempenho.map(q => "Q" + q.QuestaoNumero);
        const percentuais = vm.dadosDesempenho.map(q => +q.PctAcerto);
        const cores = percentuais.map(pct => {
            if (pct < 30) return "#e74c3c";       // vermelho
            else if (pct < 70) return "#f1c40f";  // amarelo
            else return "#27ae60";                 // verde
        });

        // Usa $timeout para garantir atualização após render do DOM
        $timeout(() => criarGrafico(labels, percentuais, cores), 0);
    };

    // Inicializa após DOM pronto
    angular.element(document).ready(function () {
        if (vm.turmaSelecionada) {
            vm.atualizarTabelaDesempenho();
        }
    });
}]);
