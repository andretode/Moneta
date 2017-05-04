﻿
function gerarGraficoSaldoPorCategoria(arrayDeCategoriasJson, arrayDeCoresJson, arrayDeSaldosJson)
{
    var arrayDeCategorias = JSON.parse(arrayDeCategoriasJson);
    var arrayDeCores = JSON.parse(arrayDeCoresJson);
    var arrayDeSaldos = JSON.parse(arrayDeSaldosJson);

    if (arrayDeCategorias.length != 0)
    {
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
            }
        }

        var ctxDesktop = document.getElementById("GraficoSaldoPorCategoria-Desktop");
        var myDoughnutChart = new Chart(ctxDesktop, {
            type: 'doughnut',
            data: data,
            options: options
        });

        var ctxMobile = document.getElementById("GraficoSaldoPorCategoria-Mobile");
        var myDoughnutChart = new Chart(ctxMobile, {
            type: 'doughnut',
            data: data,
            options: options
        });
    }
}