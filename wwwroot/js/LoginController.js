app.controller('LoginController', ['$scope', function ($scope) {
    $scope.showPassword = false;

    $scope.togglePasswordVisibility = function () {
        var passwordInput = document.getElementById('Password');
        $scope.showPassword = !$scope.showPassword;
        if ($scope.showPassword) {
            passwordInput.type = 'text';
        } else {
            passwordInput.type = 'password';
        }
    };
}]);