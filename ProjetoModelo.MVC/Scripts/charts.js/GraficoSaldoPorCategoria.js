function incluirMsgDadosInsuficientes()
{
    $('#divGraficoSaldoPorCategoria').html('<div class="text-center text-primary">Não há informações suficientes para gerar o gráfico</div>');
}

function gerarGraficoSaldoPorCategoria(arrayDeCategoriasJson, arrayDeCoresJson, arrayDeSaldosJson)
{
    if (arrayDeCategoriasJson == "" || arrayDeCategoriasJson == null)
    {
        incluirMsgDadosInsuficientes();
        return;
    }

    var arrayDeCategorias = JSON.parse(arrayDeCategoriasJson);
    var arrayDeCores = JSON.parse(arrayDeCoresJson);
    var arrayDeSaldos = JSON.parse(arrayDeSaldosJson);

    if (arrayDeCategorias.length == 0)
    {
        incluirMsgDadosInsuficientes();
        return;
    }

    var data = {
        labels: arrayDeCategorias,
        datasets: [
            {
                data: arrayDeSaldos,
                backgroundColor: arrayDeCores,
                hoverBackgroundColor: arrayDeCores
            }]
    };

    var options = {
        title: {
            display: false,
            text: 'Despesas por Categoria',
            responsive: true
        },
        tooltips: {
			callbacks: {
				label: function(tooltipItem, data) {
				    var allData = data.datasets[tooltipItem.datasetIndex].data;
					var tooltipLabel = data.labels[tooltipItem.index];
					var tooltipData = allData[tooltipItem.index];
					var strValor = 'R$ ' + tooltipData.toFixed(2);
					strValor = strValor.replace('.', ',');
					var total = 0;
					for (var i in allData) {
						total += allData[i];
					}
					var tooltipPercentage = Math.round((tooltipData / total) * 100);
					return tooltipLabel + ': ' + strValor + ' (' + tooltipPercentage + '%)';
				}
            }
        }
    }

    limpasCanvas();            

    var ctxDesktop = document.getElementById("GraficoSaldoPorCategoria_Desktop");
    var myDoughnutChart = new Chart(ctxDesktop, {
        type: 'doughnut',
        data: data,
        options: options
    });

    var ctxMobile = document.getElementById("GraficoSaldoPorCategoria_Mobile");
    var myDoughnutChart = new Chart(ctxMobile, {
        type: 'doughnut',
        data: data,
        options: options
    });
}

// Faz o tratamento da remoção dos canvas para não sobrepor e manter 
// dois gráficos quando este é atualizado via Ajax.
function limpasCanvas()
{
    if ($('#GraficoSaldoPorCategoria_Mobile').length) {
        $('#GraficoSaldoPorCategoria_Mobile').remove();
        $('#GraficoSaldoPorCategoria_Desktop').remove();
    }
    $('#divGraficoSaldoPorCategoria')
        .html('<canvas class="visible-xs visible-sm" id="GraficoSaldoPorCategoria_Mobile" height="350"></canvas>' +
            '<canvas class="visible-md visible-lg" id="GraficoSaldoPorCategoria_Desktop"></canvas>');
}