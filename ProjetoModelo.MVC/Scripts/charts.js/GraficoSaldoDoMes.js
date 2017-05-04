
function gerarGraficoSaldoDoMes(arrayDeDias, arrayDeReceitas, arrayDeDespesas, arrayDeSaldos)
{
    var arrayDeDiasRazor = arrayDeDias;
    var arrayDeDiasJs = arrayDeDiasRazor.split(',');

    var arrayDeReceitasRazor = arrayDeReceitas;
    var arrayDeReceitasJs = arrayDeReceitasRazor.split(',');

    var arrayDeDespesasRazor = arrayDeDespesas;
    var arrayDeDespesasJs = arrayDeDespesasRazor.split(',');

    var arrayDeSaldosRazor = arrayDeSaldos;
    var arrayDeSaldosJs = arrayDeSaldosRazor.split(',');

    var ctx = document.getElementById("graficoSaldoDoMes");
    var myChart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: arrayDeDiasJs,
            datasets: [
            {
                label: 'Receita',
                data: arrayDeReceitasJs,
                backgroundColor: 'rgba(0, 153, 13, 0.2)',
                borderColor: 'rgba(0, 153, 135, 1)',
                borderWidth: 1
            },
            {
                label: 'Despesa',
                data: arrayDeDespesasJs,
                backgroundColor: 'rgba(219, 0, 0, 0.2)',
                borderColor: 'rgba(219, 0, 0, 1)',
                borderWidth: 1
            },
            {
                label: 'Saldo',
                data: arrayDeSaldosJs,
                backgroundColor: 'rgba(102, 176, 255, 0.7)',
                borderColor: 'rgba(102, 176, 255, 1)',
                borderWidth: 1
            }
            ]
        },
        options: {
            scales: {
                yAxes: [{
                    ticks: {
                        beginAtZero: true
                    }
                }]
            },
            title: {
                display: false,
                text: 'Saldo por dia'
            }
        }
    });
}