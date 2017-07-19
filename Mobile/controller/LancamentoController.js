var app = angular.module('Moneta', ["ngRoute"]);

app.config(function($routeProvider) {
    $routeProvider
		.otherwise({
        templateUrl : "index.html"
    });
});

/*
app.controller("editLancamentoController", function ($scope) {
		$scope.Lancamento = Lancamento;
});
*/

app.controller("LancamentosController", function ($scope)  {
	$scope.Lancamentos = getAllLancamentos();
	
	$scope.novoLancamento = function ()
	{
		$scope.Contas = ["Andre BB", "Andre Carteira"];
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
		$scope.Lancamento = Lancamento;
		$scope.Contas = ["Andre BB", "Andre Carteira"];
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