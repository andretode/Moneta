var urlGetContas = "http://localhost:8003/api/Contas/Get";
var urlGetLancamentosDoMes = "http://localhost:8003/api/Lancamento/Get";
var urlAddAllLancamentos = "http://localhost:8003/api/Lancamento";
var app = angular.module('Moneta', ["ngRoute"]);

app.config(function($routeProvider) {
    $routeProvider
		.otherwise({
        templateUrl : "index.html"
    });
});

app.controller("LancamentosController", function ($scope, $http, $filter)  {    
    $http.get(urlGetLancamentosDoMes + "?lancamentosJson={ContaIdFiltro:'335615a8-0332-4901-a245-3121021cae96',MesAnoCompetencia: '2017-07-01'}")
    .success(function(data) {
        $scope.Lancamentos = getAllLancamentos();
        
        //Remove os lançamentos repetidos
        var lancamentosNaoRepetidos = data.LancamentosDoMes;
        $.each($scope.Lancamentos, function(index, ll){
            //Remove Nao-Fake
            lancamentosNaoRepetidos = $.grep(lancamentosNaoRepetidos, function(ldm){ 
                 return ldm.LancamentoId != ll.LancamentoId;
            });
            //Remove Fake
            lancamentosNaoRepetidos = $.grep(lancamentosNaoRepetidos, function(ldm){ 
                 return ldm.IdDaParcelaNaSerie != ll.IdDaParcelaNaSerie;
            });
        });
        $scope.Lancamentos = $scope.Lancamentos.concat(lancamentosNaoRepetidos);
    })
    .error(function(data, status) {
        alert("Ocorreu um erro ao buscar os lançamentos nas nuvens. Tente novamente mais tarde.");
        $scope.Lancamentos = getAllLancamentos();
    });
	
	$http.get(urlGetContas).then(function(response) {
        $scope.Contas = response.data;
	});
	
	$scope.novoLancamento = function ()
	{
		window.location = 'index.html#novo-lancamento';
	};
	
	$scope.addLancamento = function (Lancamento)
	{
        Lancamento.Sincronizado = false;
        Lancamento.CategoriaId = '59ef6a93-65dd-4afc-be81-5f53166cc272';
		$scope.Lancamentos.push(Lancamento);
		var lancamentosStr = JSON.stringify($scope.Lancamentos);
		localStorage.setItem('lancamentos', lancamentosStr);
		window.location = 'index.html';
	};
	
	$scope.editLancamento = function (Lancamento)
	{
		if (Lancamento.Tipo == "despesa")
			Lancamento.Valor *= -1;
		
		$scope.Lancamento = Lancamento;
		window.location = 'index.html#edit-lancamento';
	};
	
	$scope.salvarLancamento = function() {
		salvarLancamentos($scope.Lancamentos);
	};
	
	$scope.deleteLancamento = function (lancamento)
	{
		if (confirm("Deseja realmente remover?"))
		{
            if(lancamento.Sincronizado)
            {
                $http.post(
                    "http://localhost:8003/api/Lancamento/Remove",
                    JSON.stringify(lancamento)
                )
                .success(function(data) {
                    //Remove localmente o lançamento 
                    $scope.Lancamentos = $.grep($scope.Lancamentos, function(value) {
                        return value.LancamentoId != lancamento.LancamentoId;
                    });
                    salvarLancamentos($scope.Lancamentos);
                    
                    location.reload();
                })
                .error(function(data, status) {
                    alert("Ocorreu um erro ao sincronizar. Tente novamente mais tarde.");
                });
            }
            else{
                $scope.Lancamentos = $.grep($scope.Lancamentos, function(value) {
                    return value.LancamentoId != lancamento.LancamentoId;
                });
                salvarLancamentos($scope.Lancamentos);
            }            

		}
	};
    
    $scope.sincronizarLancamentos = function ()
    {
        var lancamentosNaoSincronizados = $filter('filter')($scope.Lancamentos, {Sincronizado:false});
        if(lancamentosNaoSincronizados.length > 0)
        {
            $http.post(
                "http://localhost:8003/api/Lancamento/Post",
                JSON.stringify(lancamentosNaoSincronizados)
            )
            .success(function(data) {
                //Remove os lançamentos locais que foram sincronizados com sucesso
                $scope.Lancamentos = $.grep($scope.Lancamentos, function(l){ 
                     return l.Sincronizado == true;
                });
                salvarLancamentos($scope.Lancamentos);
                
                location.reload();
                //window.location = 'index.html';
            })
            .error(function(data, status) {
                alert("Ocorreu um erro ao sincronizar. Tente novamente mais tarde.");
            });
        }
        else{
            alert("Não há lançamentos para sincronizar.");
        }
    };
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