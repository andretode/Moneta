$(function () { // will trigger when the document is ready
	$(".datepicker").datepicker({
		dateFormat: 'dd/mm/yy',
		dayNames: ['Domingo', 'Segunda', 'Terça', 'Quarta', 'Quinta', 'Sexta', 'S&aacute;bado'],
		dayNamesMin: ['D', 'S', 'T', 'Q', 'Q', 'S', 'S', 'D'],
		dayNamesShort: ['Dom', 'Seg', 'Ter', 'Qua', 'Qui', 'Sex', 'Sáb', 'Dom'],
		monthNames: ['Janeiro', 'Fevereiro', 'Mar&ccedil;o', 'Abril', 'Maio', 'Junho', 'Julho', 'Agosto', 'Setembro', 'Outubro', 'Novembro', 'Dezembro'],
		monthNamesShort: ['Jan', 'Fev', 'Mar', 'Abr', 'Mai', 'Jun', 'Jul', 'Ago', 'Set', 'Out', 'Nov', 'Dez'],
		nextText: 'Pr&oacute;ximo',
		prevText: 'Anterior'
	});
});

function trocarConta() {
    $.post("/Home/TrocarConta",
        { ContaIdFiltro: $('#ContaIdFiltro').val() },
        function (data) {
            if (data.status === "Ok") {
                location.reload()
            }
            else {
                alert("Erro ao trocar de conta!");
            }
        }
    );
}