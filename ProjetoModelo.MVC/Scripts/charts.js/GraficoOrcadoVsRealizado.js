function incluirMsgDadosInsuficientes()
{
    $('#divGraficoOrcadoVsRealizado').html('<div class="text-center text-primary">Não há informações suficientes para gerar o gráfico</div>');
}

function gerarGraficoOrcadoVsRealizado(arrayDeCategoriasJson, arrayDeCoresJson, arrayDeSaldosRealizadosJson, arrayDeOrcamentosJson)
{
    if (arrayDeCategoriasJson == "" || arrayDeCategoriasJson == null)
    {
        incluirMsgDadosInsuficientes();
        return;
    }

    var arrayDeCategorias = JSON.parse(arrayDeCategoriasJson);
    var arrayDeCores = JSON.parse(arrayDeCoresJson);
    var arrayDeSaldosRealizados = JSON.parse(arrayDeSaldosRealizadosJson);
    var arrayDeOrcamentos = JSON.parse(arrayDeOrcamentosJson);

    if (arrayDeCategorias.length == 0)
    {
        incluirMsgDadosInsuficientes();
        return;
    }

    var data = {
        labels: arrayDeCategorias,
        datasets: [
            {
                label: "Realizado",
                backgroundColor: "rgba(179,181,198,0.2)",
                borderColor: "rgba(179,181,198,1)",
                pointBackgroundColor: "rgba(179,181,198,1)",
                pointBorderColor: "#fff",
                pointHoverBackgroundColor: "#fff",
                pointHoverBorderColor: "rgba(179,181,198,1)",
                data: arrayDeSaldosRealizados
            },
            {
                label: "Orçado",
                backgroundColor: "rgba(255,99,132,0.2)",
                borderColor: "rgba(255,99,132,1)",
                pointBackgroundColor: "rgba(255,99,132,1)",
                pointBorderColor: "#fff",
                pointHoverBackgroundColor: "#fff",
                pointHoverBorderColor: "rgba(255,99,132,1)",
                data: arrayDeOrcamentos
            }
        ]
    }
    
    var ctx = document.getElementById("graficoOrcadoVsRealizado");
    var myBarChart = new Chart(ctx, {
        type: 'radar',
        data: data
    });
}
