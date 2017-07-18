function LancamentosController($scope)  {
	$scope.Lancamentos = getAllLancamentos();
	
	$scope.addLancamento = function (Lancamento)
	{
		$scope.lancamentos.push(Lancamento);
		var lancamentosStr = JSON.stringify($scope.lancamentos);
		localStorage.setItem('lancamentos', lancamentosStr);
		$location.url('#lancamentos');
	}
}

function getAllLancamentos()
{
	var lancamentos = "";	
	try{
		lancamentos = JSON.parse(localStorage.getItem('lancamentos'));
		
		if(lancamentos.length > 0)
			return lancamentos.sort(compareDataVencimento);
		else
			return [];
	}
	catch(Exception)
	{
		return [];
	}
}

function compareDataVencimento(a,b) {
  if (a.DataVencimento < b.DataVencimento)
    return -1;
  if (a.DataVencimento > b.DataVencimento)
    return 1;
  return 0;
}