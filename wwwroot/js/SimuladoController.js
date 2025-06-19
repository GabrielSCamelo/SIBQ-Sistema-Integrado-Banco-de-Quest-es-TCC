app.controller('SimuladoController', ['$scope', '$http', '$sce', function ($scope, $http, $sce) {
    // Dados e filtros
    $scope.simulados = [];
    $scope.filtro = {
        DataCriacao: '',
        DataExpiracao: ''
    };

    // Paginação
    $scope.paginaAtual = 1;
    $scope.itensPorPagina = 10;
    $scope.maxPaginasVisiveis = 10;
    $scope.totalPaginas = 1;

    // Modal de mensagem
    $scope.modalTitulo = '';
    $scope.modalMensagem = '';

    $scope.mostrarModalMensagem = function (titulo, mensagem) {
        $scope.modalTitulo = titulo || 'Aviso';
        $scope.modalMensagem = $sce.trustAsHtml(mensagem || '');
        var modalEl = document.getElementById('modalMensagem');
        var modal = bootstrap.Modal.getOrCreateInstance(modalEl);
        modal.show();
    };

    // Funções de paginação
    $scope.simuladosPaginados = function () {
        const inicio = ($scope.paginaAtual - 1) * $scope.itensPorPagina;
        return $scope.simulados.slice(inicio, inicio + $scope.itensPorPagina);
    };

    $scope.mudarPagina = function (novaPagina) {
        if (novaPagina >= 1 && novaPagina <= $scope.totalPaginas) {
            $scope.paginaAtual = novaPagina;
        }
    };

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

    // Busca simulados com filtro
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
                $scope.mostrarModalMensagem('Erro', 'Não foi possível buscar os simulados. Tente novamente mais tarde.');
            });
    };

    // Exclui o simulado após confirmação via modal
    $scope.simuladoParaExcluir = null;

    $scope.confirmarExclusao = function (simulado) {
        $scope.simuladoParaExcluir = simulado;
        $scope.mostrarModalMensagem(
            'Confirmação',
            `Tem certeza que deseja excluir o simulado <strong>${simulado.titulo}</strong>? <br><br>
            <div class="modal-footer">
             <button type="button" class="btn btn-danger me-2" ng-click="excluirSimuladoConfirmado()">Sim, excluir</button>
             <button type="button" class="btn btn-primary" data-bs-dismiss="modal">Cancelar</button>
             </div>`

        );
    };

    // Confirmar exclusão, chamado pelo botão do modal
    $scope.excluirSimuladoConfirmado = function () {
        if (!$scope.simuladoParaExcluir) return;
        const id = $scope.simuladoParaExcluir.id;
        // Fechar modal manualmente
        var modalEl = document.getElementById('modalMensagem');
        var modal = bootstrap.Modal.getInstance(modalEl);
        if (modal) modal.hide();

        window.location.href = '/Simulado/ExcluirSimulado?id=' + encodeURIComponent(id);
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

    // Inicializa
    $scope.pesquisarSimulados();
}]);