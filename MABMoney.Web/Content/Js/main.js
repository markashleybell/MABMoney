$(function () {
    $('.date-picker').datepicker({
        format: 'dd/mm/yyyy',
        autoclose: true
    });

    $('#net-worth').on('click', function (evt) {
        evt.preventDefault();
    }).popover({
        title: 'Accounts',
        placement: 'bottom',
        html: true,
        content: $('#net-worth-content').html()
    });

    var accountSelector = $('.account-selector');

    accountSelector.on('change', function (evt) {
        this.form.submit();
    });

    var mobileTabSelect = $('#mobile-tab-select select:first');
    var allTabs = $('div.tab-content').find('div.tab-pane');

    mobileTabSelect.on('change', function (evt) {
        allTabs.removeClass('active');
        $(mobileTabSelect.val()).addClass('active');
    });

    $('.typeahead').typeahead({
        minlength: 1,
        source: function (query, process) {

            $.ajax({
                url: '/Accounts/GetTransactionDescriptionHistory',
                data: { query: query, id: accountSelector.val() },
                type: 'POST'
            }).done(function (data) {
                console.log(data);
                process(data);
            });

        }
    });


    function aprCalc(apr) {
        apr = apr * 1 / 100;
        return (Math.pow((1 + apr), (1 / 12)) - 1) * 100;
    }

    var tableBody = $('#payment-calculator-table tbody');

    function buildPaymentCalculatorTable(currentBalance, paymentAmount, interestRate, minPaymentPercentage) {

        currentBalance = parseFloat(currentBalance);
        paymentAmount = parseFloat(paymentAmount);
        interestRate = parseFloat(interestRate);
        minPaymentPercentage = parseFloat(minPaymentPercentage);

        var months = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'];

        var d = new Date();

        d.setDate(1);
        d.setMonth(d.getMonth() + 1);

        var balance = currentBalance;

        var html = [];

        while (balance > 0) {

            var interestAmount = (aprCalc(interestRate) / 100) * balance;

            var balanceAtMonthEnd = (balance - paymentAmount) + interestAmount;

            // console.log(aprCalc(interestRate));

            var model = {
                month: months[d.getMonth()],
                monthStart: balance,
                minPayment: (minPaymentPercentage / 100) * balance, 
                payment: (paymentAmount > balance) ? balance : paymentAmount,
                interest: interestAmount,
                monthEnd: (balanceAtMonthEnd > 0) ? balanceAtMonthEnd : 0
            };

            balance = model.monthEnd;

            // console.log(model);

            var row = '<tr>' +
                        '<td>' + model.month + '</td>' +
                        '<td>' + model.monthStart.toFixed(2) + '</td>' +
                        '<td>' + model.interest.toFixed(2) + '</td>' +
                        '<td>' + model.minPayment.toFixed(2) + '</td>' +
                        '<td>' + model.payment.toFixed(2) + '</td>' +
                        '<td>' + model.monthEnd.toFixed(2) + '</td>' +
                      '</tr>';

            html.push(row);

            d.setMonth(d.getMonth() + 1);

        }

        tableBody.empty();

        tableBody.append(html.join(''));
    }

    

    var currentBalance = $('#payment-calculator-currentbalance');
    var paymentAmount = $('#payment-calculator-paymentamount');
    var interestRate = $('#payment-calculator-interestrate');
    var minPaymentPercentage = $('#payment-calculator-minpaymentpercentage');

    buildPaymentCalculatorTable(currentBalance.val(), paymentAmount.val(), interestRate.val(), minPaymentPercentage.val());

    $('#payment-calculator-recalculate').on('click', function (evt) {
        evt.preventDefault();
        buildPaymentCalculatorTable(currentBalance.val(), paymentAmount.val(), interestRate.val(), minPaymentPercentage.val());
    });

    $('input.help, select.help, textarea.help').popover({
        title: 'What\'s this?',
        trigger: 'focus'
    });

    var tmpl = '<div class="control-group">' +
               '    <input class="label-textbox" id="Categories_{{INDEX}}__Name" name="Categories[{{INDEX}}].Name" placeholder="Name" type="text" value="">' +
               '    <div class="controls">' +
               '        <input data-val="true" data-val-number="The field Amount must be a number." data-val-required="The Amount field is required." id="Categories_{{INDEX}}__Amount" name="Categories[{{INDEX}}].Amount" placeholder="Email" type="text" value="0">' +
               '        <a class="cat-delete" id="del{{INDEX}}" href="#del0">Delete</a>' +
               '        <span class="field-validation-valid" data-valmsg-for="Categories[{{INDEX}}].Amount" data-valmsg-replace="true"></span>' +
               '        <input data-val="true" data-val-number="The field Budget_BudgetID must be a number." data-val-required="The Budget_BudgetID field is required." id="Categories_{{INDEX}}__Budget_BudgetID" name="Categories[{{INDEX}}].Budget_BudgetID" type="hidden" value="0">' +
               '        <input data-val="true" data-val-number="The field Category_CategoryID must be a number." data-val-required="The Category_CategoryID field is required." id="Categories_{{INDEX}}__Category_CategoryID" name="Categories[{{INDEX}}].Category_CategoryID" type="hidden" value="0">' +
               '    </div>' +
               '</div>';

    var del = '<div><div>' +
              '<input data-val="true" id="Categories_{{INDEX}}__Delete" name="Categories[{{INDEX}}].Delete" type="hidden" value="true">' +
              '<input data-val="true" id="Categories_{{INDEX}}__Amount" name="Categories[{{INDEX}}].Amount" type="hidden" value="0">' +
              '<input data-val="true" id="Categories_{{INDEX}}__Budget_BudgetID" name="Categories[{{INDEX}}].Budget_BudgetID" type="hidden" value="0">' +
              '<input data-val="true" id="Categories_{{INDEX}}__Category_CategoryID" name="Categories[{{INDEX}}].Category_CategoryID" type="hidden" value="{{ID}}">';
              '</div></div>';

    $('#add-category-button').on('click', function (evt) {
        // Grab the last category input that currently exists in the page
        var lastInput = $('input[id^=Categories_]:last');

        // Get the current index from the input's name attribute
        var match = /\[(\d+)\]/.exec(lastInput.attr('name'));
        // Increment the index
        var id = parseInt(match[1], 10) + 1;
        
        lastInput.parent().parent().after(tmpl.replace(/\{\{INDEX\}\}/g, id));
    });

    $('form').on('click', '.cat-delete', function (evt) {

        evt.preventDefault();
        var control = $(this).parent().parent();
        var categoryId = this.hash.substring(4);
        var index = this.id.substring(3);

        control.replaceWith(del.replace(/\{\{INDEX\}\}/g, index).replace(/\{\{ID\}\}/g, categoryId))

    });

});