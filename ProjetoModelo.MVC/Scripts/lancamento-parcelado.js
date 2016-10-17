
function mostrarLancamentoParcelado() {
    $('#divLancamentoParcelado').removeClass("hidden");
    $('#botaoRepetir').addClass("hidden");
}

function selecionouLancamentosFixos()
{
    $('#divNumeroParcelas').addClass("hidden");
    $('#divPeriodicidade').removeClass("hidden");
}

function selecionouLancamentosParcelados() {
    $('#divNumeroParcelas').removeClass("hidden");
    $('#divPeriodicidade').removeClass("hidden");
}

function validarLancamentoParcelado()
{
    var fixoChecked = $('#repetir_fixo').is(':checked');
    var parceladoChecked = $('#repetir_parcelado').is(':checked');
    if(parceladoChecked) {
        if (!informarNumParcelas()) {
            return false;
        }
        else {
            if (!informarPeriodicidade()) {
                return false;
            }
        }
    }
    else if (fixoChecked) {
        if (!informarPeriodicidade()) {
            return false;
        }
    }
    $('#formLancamento').submit();
}

function informarNumParcelas()
{
    var numParcelas = $('#LancamentoParcelado_NumeroParcelas').val();
    if (numParcelas == '' || numParcelas < 2) {
        alert("O n° de parcelas deve ser maior que 1");
        $('#LancamentoParcelado_NumeroParcelas').focus();
        return false;
    }
    return true;
}

function informarPeriodicidade()
{
    var periodicidade = $('#LancamentoParcelado_Periodicidade').val();
    if (periodicidade == 0) {
        alert("Informe a periodicidade");
        $('#LancamentoParcelado_Periodicidade').focus();
        return false;
    }
    return true;
}