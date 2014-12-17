interface MABMoneyOptions {
    cookieKey: string;
}

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
}

interface MABMoneyTemplates {
    paymentCalcRow: string;
    budgetCategoryEditForm: string;
    budgetCategoryDeleteForm: string;
}

interface MABMoneyPaymentCalculatorRowModel {
    month: string;
    monthStart: string;
    minPayment: string;
    payment: string;
    interest: string;
    monthEnd: string;
}

var MABMoney = (function ($, window, undefined) {
    // Name of the cookie to use to store payment calc data
    var _cookieKey: string = null;

    // Container for cached UI element selectors
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

    // Container for cached template HTML
    var _templates: MABMoneyTemplates = {
        paymentCalcRow: null,
        budgetCategoryEditForm: null,
        budgetCategoryDeleteForm: null
    };

    // Allow setup of options before init() is called
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
        // Set date to the first of next month
        var d = new Date();
        d.setDate(1);
        d.setMonth(d.getMonth() + 1);

        var months = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'];
        var balance = currentBalance, html = [];

        while (balance > 0) {
            // Work out how much interest will be paid
            var interestAmount: number = (_aprCalc(interestRate) / 100) * balance;
            // Work out what the balance will be at the end of the month
            var balanceAtMonthEnd: number = (balance - paymentAmount) + interestAmount;

            // If the balance is now negative, just set it to zero
            if (balanceAtMonthEnd < 0) { balanceAtMonthEnd = 0; }

            var model: MABMoneyPaymentCalculatorRowModel = {
                month: months[d.getMonth()],
                monthStart: balance.toFixed(2).toString(),
                minPayment: ((minPaymentPercentage / 100) * balance).toFixed(2).toString(),
                payment: ((paymentAmount > balance) ? balance : paymentAmount).toFixed(2).toString(),
                interest: interestAmount.toFixed(2).toString(),
                monthEnd: balanceAtMonthEnd.toFixed(2).toString()
            };

            // Update the balance for the next iteration
            balance = balanceAtMonthEnd;
            // Add the rendered table row to the array
            html.push(Mustache.render(_templates.paymentCalcRow, model));
            // Increment the month
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

        // Cache template HTML
        _templates.paymentCalcRow = $('#tmpl-payment-calc-row').html();
        _templates.budgetCategoryEditForm = $('#tmpl-budget-category-edit-form').html();
        _templates.budgetCategoryDeleteForm = $('#tmpl-budget-category-delete-form').html();

        // Parse and cache compiled templates
        Mustache.parse(_templates.paymentCalcRow); 
        Mustache.parse(_templates.budgetCategoryEditForm); 
        Mustache.parse(_templates.budgetCategoryDeleteForm); 

        // Set up date picker inputs
        $('.date-picker').datepicker({
            format: 'dd/mm/yyyy',
            autoclose: true,
            weekStart: 1
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

        // Reload transaction index when account is changed
        $('#Transactions_AccountID').on('change', function (evt) {
            window.location = '/Transactions/Index/' + $(this).val();
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

        // Initially populate the payment calculator table
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
            // Store the minimum payment amount in a cookie
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

        // Add a new budget category form row
        $('#add-category-button').on('click', function (evt) {
            // Grab the last category input that currently exists in the page
            var lastInput = $('input[id^=Categories_]:last');
            // Get the current index from the input's name attribute
            var match = /\[(\d+)\]/.exec(lastInput.attr('name'));
            // Increment the index
            var index = parseInt(match[1], 10) + 1;
            // Add a new input row after the last input
            lastInput.parent().parent().after(Mustache.render(_templates.budgetCategoryEditForm, { INDEX: index }));
        });

        // Remove a budget category form row and replace it with the deletion data
        $('form').on('click', '.cat-delete', function (evt) {
            evt.preventDefault();
            var control = $(this).parent().parent();
            var categoryId = this.hash.substring(4);
            var index = this.id.substring(3);
            control.replaceWith(Mustache.render(_templates.budgetCategoryDeleteForm, { INDEX: index, ID: categoryId }));
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