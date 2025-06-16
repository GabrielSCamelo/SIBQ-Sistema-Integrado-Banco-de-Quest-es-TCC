app.controller('simuladoFController', ['$scope', '$timeout', function ($scope, $timeout) {
    const porPagina = 5;

    // Estado da paginação
    $scope.porPagina = porPagina;
    $scope.paginaAtual = 0;

    // Supondo que essa lista venha da variável global window.simuladoData
    $scope.questoesOrdem = window.simuladoData.questoesOrdem;

    // Controla quais questões estão visíveis no template, calculado via filtro no ng-repeat
    $scope.questoesVisiveis = function (index) {
        const start = $scope.paginaAtual * porPagina;
        const end = start + porPagina;
        return index >= start && index < end;
    };

    $scope.proximaPagina = function () {
        const totalQuestoes = $scope.questoesOrdem.length;
        if (($scope.paginaAtual + 1) * porPagina < totalQuestoes) {
            $scope.paginaAtual++;
        }
    };

    $scope.voltarPagina = function () {
        if ($scope.paginaAtual > 0) {
            $scope.paginaAtual--;
        }
    };

    // Gera o resumo de questões respondidas para exibir na confirmação
    $scope.getResumoConfirmacao = function () {
        const radios = document.querySelectorAll("input[type='radio']");
        const questoesRespondidas = {};

        radios.forEach(r => {
            const match = r.name.match(/\d+/);
            if (!match) return;
            const id = match[0];
            if (!questoesRespondidas[id]) questoesRespondidas[id] = false;
            if (r.checked) questoesRespondidas[id] = true;
        });

        const total = $scope.questoesOrdem.length;
        const metade = Math.ceil(total / 2);

        // Cria array para ng-repeat: cada item representa uma linha com duas colunas de questões
        const linhas = [];

        for (let i = 0; i < metade; i++) {
            const idEsquerda = $scope.questoesOrdem[i];
            const idDireita = $scope.questoesOrdem[i + metade];

            linhas.push({
                esquerda: {
                    numero: i + 1,
                    respondida: questoesRespondidas[idEsquerda] === true
                },
                direita: idDireita !== undefined ? {
                    numero: i + 1 + metade,
                    respondida: questoesRespondidas[idDireita] === true
                } : null
            });
        }

        return linhas;
    };

    $scope.abrirConfirmacao = function () {
        $scope.linhasConfirmacao = $scope.getResumoConfirmacao();

        const modalEl = document.getElementById('modalConfirmacao');
        if (modalEl) {
            const modal = new bootstrap.Modal(modalEl);
            modal.show();
        }
    };

    $scope.enviarFormulario = function () {
        const form = document.querySelector("form");
        if (form) form.submit();
    };

    // Inicialização Angular-friendly
    $timeout(() => {
        $scope.paginaAtual = 0;
    }, 0);
}]);