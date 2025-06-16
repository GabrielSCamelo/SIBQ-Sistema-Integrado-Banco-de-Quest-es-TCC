var app = angular.module('SIBQ', []);

app.run(['$http', function ($http) {
    var token = document.querySelector('meta[name="csrf-token"]');
    if (token) {
        $http.defaults.headers.common['RequestVerificationToken'] = token.getAttribute('content');
    }
}]);

app.controller('Main', ['$scope', '$http', function ($scope, $http) {
    $scope.currentUser = localStorage.getItem('currentUser') || null;

    $scope.logout = function () {
        $http.post('/Logout')
            .then(function () {
                $scope.currentUser = null;
                localStorage.removeItem('currentUser');
                window.location.href = '/Home/Login'; // redireciona para login após logout
            })
            .catch(function (error) {
                console.error('Erro no logout:', error);
            });
    };
}]);