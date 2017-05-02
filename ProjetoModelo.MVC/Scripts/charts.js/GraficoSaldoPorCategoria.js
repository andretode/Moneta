
function gerarGraficoSaldoPorCategoria(arrayDeCategoriasJson, arrayDeSaldosJson)
{
    var cores = ["#FF6384","#36A2EB","#FFCE56","#AF6384","#A1A2EB","#AFCE56",
                    "#11CE56", "#F6A2EB", "#FFA500", "#FF4500", "#0000FF", "#4B0082",
                    "#00FFFF","#696969","#008B8B","#006400","#FFD700","#A0522D",
                    "#D2691E", "#BC8F8F", "#F5DEB3", "#808000", "#ADFF2F",
                    "#B22222", "#7FFF00", "#FF69B4", "#6495ED", "#000000",
                    "#B0E0E6", "#DAA520"];
    var arrayDeCategorias = JSON.parse(arrayDeCategoriasJson);
    var arrayDeSaldos = JSON.parse(arrayDeSaldosJson);

    var data = {
        labels: arrayDeCategorias,
        datasets: [
            {
                data: arrayDeSaldos,
                backgroundColor: cores,
                hoverBackgroundColor: cores
            }]
    };

    var options = {
        title: {
                display: true,
                text: 'Saldo por Categoria'
        }
    }

    var ctx = document.getElementById("graficoSaldoPorCategoria");
    var myDoughnutChart = new Chart(ctx, {
        type: 'doughnut',
        data: data,
        options: options
    });
}