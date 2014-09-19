interface MABMoneyOptions {
    cookieKey: string;
};

interface MABMoneyUIElements {
    globalAccountSelector: JQuery;
    accountSelector: JQuery;
    mobileTabSelect: JQuery;
    allTabs: JQuery;
    tableBody: JQuery;
    currentBalance: JQuery;
    paymentAmount: JQuery;
    interestRate: JQuery;
    minPaymentPercentage: JQuery;
};

interface MABMoneyPaymentCalculatorRowModel {
    month: string;
    monthStart: number;
    minPayment: number;
    payment: number;
    interest: number;
    monthEnd: number;
};

var MABMoney = (function ($, window, undefined) {

    var _cookieKey: string = null; 

    var _ui: MABMoneyUIElements = {
        globalAccountSelector: null,
        accountSelector: null,
        mobileTabSelect: null,
        allTabs: null,
        tableBody: null,
        currentBalance: null,
        paymentAmount: null,
        interestRate: null,
        minPaymentPercentage: null
    };

    var _setOptions = function (options: MABMoneyOptions) {
        _cookieKey = options.cookieKey;
    };

    // Calculate APR
    var _aprCalc = function (apr: number) {
        apr = apr * 1 / 100;
        return (Math.pow((1 + apr), (1 / 12)) - 1) * 100;
    };

    // Build repayments table for card/loan repayment calculator
    var _buildPaymentCalculatorTable = function (currentBalance: number, paymentAmount: number, interestRate: number, minPaymentPercentage: number) {

        var months = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'];

        var d = new Date();

        d.setDate(1);
        d.setMonth(d.getMonth() + 1);

        var balance = currentBalance;

        var html = [];

        while (balance > 0) {

            var interestAmount = (_aprCalc(interestRate) / 100) * balance;

            var balanceAtMonthEnd = (balance - paymentAmount) + interestAmount;

            var model: MABMoneyPaymentCalculatorRowModel = {
                month: months[d.getMonth()],
                monthStart: balance,
                minPayment: (minPaymentPercentage / 100) * balance,
                payment: (paymentAmount > balance) ? balance : paymentAmount,
                interest: interestAmount,
                monthEnd: (balanceAtMonthEnd > 0) ? balanceAtMonthEnd : 0
            };

            balance = model.monthEnd;

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

        _ui.tableBody.empty().append(html.join(''));
    }

    var _init = function () {
        // Cache selectors
        _ui.globalAccountSelector = $('.account-selector');
        _ui.accountSelector = $('.account-dropdown');
        _ui.mobileTabSelect = $('#mobile-tab-select select:first');
        _ui.allTabs = $('div.tab-content').find('div.tab-pane');
        _ui.tableBody = $('#payment-calculator-table tbody');
        _ui.currentBalance = $('#payment-calculator-currentbalance');
        _ui.paymentAmount = $('#payment-calculator-paymentamount');
        _ui.interestRate = $('#payment-calculator-interestrate');
        _ui.minPaymentPercentage = $('#payment-calculator-minpaymentpercentage');

        // Set up date picker inputs
        $('.date-picker').datepicker({
            format: 'dd/mm/yyyy',
            autoclose: true
        });

        // Set up net worth table popover
        $('#net-worth').on('click', function (evt) {
            evt.preventDefault();
        }).popover({
            title: 'Accounts',
            placement: 'bottom',
            html: true,
            content: $('#net-worth-content').html(),
            container: 'body'
        });

        // Auto-submit global account selector form when dropdown option is changed
        _ui.globalAccountSelector.on('change', function (evt) {
            this.form.submit();
        });

        // Wire in mobile tab replacement dropdown
        _ui.mobileTabSelect.on('change', function (evt) {
            _ui.allTabs.removeClass('active');
            $(_ui.mobileTabSelect.val()).addClass('active');
        });

        // Set up typeahead inputs (using Bootstrap 2.3.2 typeahead)
        $('.typeahead').typeahead({
            minlength: 1,
            source: function (query, process) {
                $.ajax({
                    url: '/Accounts/GetTransactionDescriptionHistory',
                    data: { query: query, id: (_ui.globalAccountSelector.length) ? _ui.globalAccountSelector.val() : _ui.accountSelector.val() },
                    type: 'POST'
                }).done(function (data) {
                        process(data);
                    });
            }
        });

        // Add a confirmation to any delete forms
        $('form.delete-form').on('submit', function (evt) {
            return confirm('Are you sure?');
        });

        // Initially populate the paymnt calculator table
        _buildPaymentCalculatorTable(parseFloat(_ui.currentBalance.val()),
                                     parseFloat(_ui.paymentAmount.val()),
                                     parseFloat(_ui.interestRate.val()),
                                     parseFloat(_ui.minPaymentPercentage.val()));

        // Handle the 'Recalculate' button click
        $('#payment-calculator-recalculate').on('click', function (evt) {
            evt.preventDefault();
            var accountId = _ui.globalAccountSelector.val();
            // Store the monthly payment amount in a cookie
            $.cookie(_cookieKey + '_' + accountId + '_DefaultCardPaymentAmount', _ui.paymentAmount.val(), { expires: 365 });
            // Store the interest rate in a cookie
            $.cookie(_cookieKey + '_' + accountId + '_DefaultCardInterestRate', _ui.interestRate.val(), { expires: 365 });
            // Store the interest rate in a cookie
            $.cookie(_cookieKey + '_' + accountId + '_DefaultCardMinimumPayment', _ui.minPaymentPercentage.val(), { expires: 365 });
            // Rebuild the payment table
            _buildPaymentCalculatorTable(parseFloat(_ui.currentBalance.val()),
                                         parseFloat(_ui.paymentAmount.val()),
                                         parseFloat(_ui.interestRate.val()),
                                         parseFloat(_ui.minPaymentPercentage.val()));
        });

        // Add popover hints to .help classed inputs
        $('input.help, select.help, textarea.help').popover({
            title: 'What\'s this?',
            trigger: 'focus',
            container: 'body'
        });

        // Hide any popovers when the window is resized
        $(window).on('resize', function (e) {
            $('input.help, select.help, textarea.help, #net-worth').popover('hide');
        });

        // Form template for new budget category
        var tmpl = '<div class="form-group">' +
            '    <div class="col-md-2">' +
            '        <input class="form-control" id="Categories_{{INDEX}}__Name" name="Categories[{{INDEX}}].Name" placeholder="Name" type="text" value="">' +
            '    </div>' +
            '    <div class="col-md-2">' +
            '        <input class="form-control" data-val="true" data-val-number="The field Amount must be a number." data-val-required="The Amount field is required." id="Categories_{{INDEX}}__Amount" name="Categories[{{INDEX}}].Amount" placeholder="Email" type="text" value="0">' +
            '    </div>' +
            '    <div class="col-md-2">' +
            '        <a class="cat-delete btn btn-default" id="del{{INDEX}}" href="#del0">Delete</a>' +
            '    </div>' +
            '    <div class="col-md-6">' +
            '        <span class="field-validation-valid" data-valmsg-for="Categories[{{INDEX}}].Amount" data-valmsg-replace="true"></span>' +
            '        <input data-val="true" data-val-number="The field Budget_BudgetID must be a number." data-val-required="The Budget_BudgetID field is required." id="Categories_{{INDEX}}__Budget_BudgetID" name="Categories[{{INDEX}}].Budget_BudgetID" type="hidden" value="0">' +
            '        <input data-val="true" data-val-number="The field Category_CategoryID must be a number." data-val-required="The Category_CategoryID field is required." id="Categories_{{INDEX}}__Category_CategoryID" name="Categories[{{INDEX}}].Category_CategoryID" type="hidden" value="0">' +
            '    </div>' +
            '</div>';

        // Form template for a deleted budget category
        var del = '<div><div>' +
            '<input data-val="true" id="Categories_{{INDEX}}__Delete" name="Categories[{{INDEX}}].Delete" type="hidden" value="true">' +
            '<input data-val="true" id="Categories_{{INDEX}}__Amount" name="Categories[{{INDEX}}].Amount" type="hidden" value="0">' +
            '<input data-val="true" id="Categories_{{INDEX}}__Budget_BudgetID" name="Categories[{{INDEX}}].Budget_BudgetID" type="hidden" value="0">' +
            '<input data-val="true" id="Categories_{{INDEX}}__Category_CategoryID" name="Categories[{{INDEX}}].Category_CategoryID" type="hidden" value="{{ID}}">';
        '</div></div>';

        // Add a new budget category form row
        $('#add-category-button').on('click', function (evt) {
            // Grab the last category input that currently exists in the page
            var lastInput = $('input[id^=Categories_]:last');

            // Get the current index from the input's name attribute
            var match = /\[(\d+)\]/.exec(lastInput.attr('name'));
            // Increment the index
            var id = parseInt(match[1], 10) + 1;

            lastInput.parent().parent().after(tmpl.replace(/\{\{INDEX\}\}/g, id.toString()));
        });

        // Remove a budget category form row and replace it with the deletion data
        $('form').on('click', '.cat-delete', function (evt) {

            evt.preventDefault();
            var control = $(this).parent().parent();
            var categoryId = this.hash.substring(4);
            var index = this.id.substring(3);

            control.replaceWith(del.replace(/\{\{INDEX\}\}/g, index).replace(/\{\{ID\}\}/g, categoryId))

        });
    };

    return {
        setOptions: _setOptions,
        init: _init
    };

})(jQuery, window, undefined);

$(function () {

    MABMoney.init();
    
});