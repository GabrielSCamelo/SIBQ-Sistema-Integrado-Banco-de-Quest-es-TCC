app.controller('SimuladoController', function ($scope, $http) {
    // Dados e filtros
    $scope.simulados = [];
    $scope.filtro = {
        DataCriacao: '',
        DataExpiracao: ''
    };

    // Configurações de paginação
    $scope.paginaAtual = 1;
    $scope.itensPorPagina = 10;
    $scope.maxPaginasVisiveis = 10;
    $scope.totalPaginas = 1;

    // Retorna os simulados da página atual
    $scope.simuladosPaginados = function () {
        const inicio = ($scope.paginaAtual - 1) * $scope.itensPorPagina;
        return $scope.simulados.slice(inicio, inicio + $scope.itensPorPagina);
    };

    // Muda para uma página válida
    $scope.mudarPagina = function (novaPagina) {
        if (novaPagina >= 1 && novaPagina <= $scope.totalPaginas) {
            $scope.paginaAtual = novaPagina;
        }
    };

    // Calcula as páginas visíveis para paginação (janela deslizante)
    $scope.paginasVisiveis = function () {
        const total = $scope.totalPaginas;
        const max = $scope.maxPaginasVisiveis;
        let atual = $scope.paginaAtual;

        const paginas = [];

        if (total <= max) {
            for (let i = 1; i <= total; i++) {
                paginas.push(i);
            }
        } else {
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

    // Faz a requisição para buscar simulados com filtro
    $scope.pesquisarSimulados = function () {
        $http.post('PesquisarSimulados', $scope.filtro)
            .then(function (response) {
                $scope.simulados = response.data || [];
                $scope.totalPaginas = Math.max(1, Math.ceil($scope.simulados.length / $scope.itensPorPagina));
                $scope.paginaAtual = 1;
            })
            .catch(function (error) {
                console.error('Erro ao buscar simulados', error);
                $scope.simulados = [];
                $scope.totalPaginas = 1;
                $scope.paginaAtual = 1;
            });
    };

    // Exclui o simulado após confirmação
    $scope.excluirSimulado = function (id) {
        if (confirm('Certeza que deseja excluir esse simulado?')) {
            window.location.href = '/Simulado/ExcluirSimulado?id=' + encodeURIComponent(id);
        }
    };

    // Controle para modal de confirmação de início
    $scope.simuladoIdSelecionado = null;

    $scope.confirmarInicio = function (simuladoId) {
        $scope.simuladoIdSelecionado = simuladoId;
        const modalEl = document.getElementById('modalConfirmacaoSimulado');
        if (modalEl) {
            const modal = new bootstrap.Modal(modalEl);
            modal.show();
        }
    };

    // Inicializa carregando os simulados
    $scope.pesquisarSimulados();
});