$(function() {
    $('.data-table').each(function() {
        var currentTable = this;
        $('thead', this).append($('thead tr', this).clone(true));
        $('thead tr:eq(1) th', this).each(function(i) {
            var title = $(this).text();
            $(this).html('<input type="text" placeholder="поиск" />');

            $('input', this).on('keyup',
                function(e) {
                    var inputValue = this.value;
                    if (e.keyCode === 13 && table.column(i).search() !== inputValue) {
                        var isRegex = inputValue.includes("+");
                        if (isRegex) {
                            inputValue = inputValue.replace("+", "|");
                        }
                        table.column(i).search(inputValue, isRegex, false).draw();
                        sumFunction(currentTable);
                    }
                });
        });
        
        var table = $(this).DataTable({
            paging: false,
            colReorder: true,
            orderCellsTop: true,
            fixedHeader: true
        });
    });
    $('input.datetime-input').datetimepicker();

    $(document).click(function (e) {
        var player = $('.music')[0];
        player.play();
    });
});