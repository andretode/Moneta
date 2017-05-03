
function gerarGraficoSaldoPorCategoria(arrayDeCategoriasJson, arrayDeCoresJson, arrayDeSaldosJson)
{
    var arrayDeCategorias = JSON.parse(arrayDeCategoriasJson);
    var arrayDeCores = JSON.parse(arrayDeCoresJson);
    var arrayDeSaldos = JSON.parse(arrayDeSaldosJson);

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
                display: true,
                text: 'Despesas por Categoria'
        }
    }

    var ctx = document.getElementById("graficoSaldoPorCategoria");
    var myDoughnutChart = new Chart(ctx, {
        type: 'doughnut',
        data: data,
        options: options
    });
}