var urlGetContas = "http://localhost:8003/api/Contas/Get";
var app = angular.module('Moneta', ["ngRoute"]);

app.config(function($routeProvider) {
    $routeProvider
		.otherwise({
        templateUrl : "index.html"
    });
});

app.controller("LancamentosController", function ($scope, $http)  {
	$scope.Lancamentos = getAllLancamentos();
	
	$http.get(urlGetContas).then(function(response) {
			$scope.Contas = response.data;
	});
	
	$scope.novoLancamento = function ()
	{
		window.location = 'index.html#novo-lancamento';
	}
	
	$scope.addLancamento = function (Lancamento)
	{
		$scope.Lancamentos.push(Lancamento);
		var lancamentosStr = JSON.stringify($scope.Lancamentos);
		localStorage.setItem('lancamentos', lancamentosStr);
		window.location = 'index.html';
	}
	
	$scope.editLancamento = function (Lancamento)
	{
		if (Lancamento.Tipo == "despesa")
			Lancamento.Valor *= -1;
		
		$scope.Lancamento = Lancamento;
		window.location = 'index.html#edit-lancamento';
	}
	
	$scope.salvarLancamento = function() {
		salvarLancamentos($scope.Lancamentos);
	}
	
	$scope.deleteLancamento = function (id)
	{
		if (confirm("Deseja realmente remover?"))
		{
			$scope.Lancamentos = $.grep($scope.Lancamentos, function(value) {
				return value.Id != id;
			});
			salvarLancamentos($scope.Lancamentos);
		}
	}
});

function salvarLancamentos(lancamentos) {
	var lancamentosStr = JSON.stringify(lancamentos);
	localStorage.setItem('lancamentos', lancamentosStr);
	window.location = 'index.html';
}

function getAllLancamentos()
{
	var lancamentos = "";	
	try{
		lancamentos = JSON.parse(localStorage.getItem('lancamentos'));
		
		if(lancamentos.length > 0)
			return lancamentos;
		else
			return [];
	}
	catch(Exception)
	{
		return [];
	}
}