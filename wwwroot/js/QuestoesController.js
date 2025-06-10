app.controller('QuestoesController', ['$scope', '$http', function ($scope, $http) {
    $scope.filtro = { titulo: '', disciplina: '' };
    $scope.questoes = [];
    $scope.paginaAtual = 1;
    $scope.itensPorPagina = 5;
    $scope.maxPaginasVisiveis = 10;

    $scope.questoesPaginadas = function () {
        let inicio = ($scope.paginaAtual - 1) * $scope.itensPorPagina;
        return $scope.questoes.slice(inicio, inicio + $scope.itensPorPagina);
    };

    $scope.totalPaginas = 1;

    $scope.mudarPagina = function (novaPagina) {
        if (novaPagina >= 1 && novaPagina <= $scope.totalPaginas) {
            $scope.paginaAtual = novaPagina;
        }
    };

    $scope.paginasVisiveis = function () {
        let total = $scope.totalPaginas;
        let max = $scope.maxPaginasVisiveis;
        let atual = $scope.paginaAtual;

        let paginas = [];

        if (total <= max) {
            // Mostra todas as páginas se total for menor que max
            for (let i = 1; i <= total; i++) {
                paginas.push(i);
            }
        } else {
            // Calcular janela deslizante
            let start = atual - Math.floor(max / 2);
            if (start < 1) start = 1;

            let end = start + max - 1;
            if (end > total) {
                end = total;
                start = end - max + 1;
            }

            for (let i = start; i <= end; i++) {
                paginas.push(i);
            }
        }
        return paginas;
    };


    $scope.carregarQuestoes = function () {
        $http.get('/Questaos/Pesquisar', {
            params: {
                titulo: $scope.filtro.titulo,
                disciplina: $scope.filtro.disciplina
            }
        }).then(function (response) {
            $scope.questoes = response.data;
            $scope.totalPaginas = Math.ceil($scope.questoes.length / $scope.itensPorPagina);
            $scope.paginaAtual = 1;
        });
    };

    $scope.carregarQuestoes();
}]);
