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
                backgroundColor: "rgba(0,128,0,0.1)",
                borderColor: "rgba(0,128,0,1)",
                pointBackgroundColor: "rgba(0,128,0,1)",
                pointBorderColor: "#fff",
                pointHoverBackgroundColor: "#fff",
                pointHoverBorderColor: "rgba(0,128,0,1)",
                data: arrayDeSaldosRealizados
            },
            {
                label: "Orçado",
                backgroundColor: "rgba(255,69,0,0.2)",
                borderColor: "rgba(255,69,0,1)",
                pointBackgroundColor: "rgba(255,69,0,1)",
                pointBorderColor: "#fff",
                pointHoverBackgroundColor: "#fff",
                pointHoverBorderColor: "rgba(255,69,0,1)",
                data: arrayDeOrcamentos
            }
        ]
    }

    var ctxDesktop = document.getElementById("GraficoOrcadoVsRealizado_Desktop");
    var myBarChart = new Chart(ctxDesktop, {
        type: 'radar',
        data: data
    });
    
    var ctxMobile = document.getElementById("GraficoOrcadoVsRealizado_Mobile");
    var myBarChart = new Chart(ctxMobile, {
        type: 'radar',
        data: data
    });
}
