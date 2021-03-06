﻿var MABMoney = (function ($, Mustache, window, undefined) {
    // Name of the cookie to use to store payment calc data
    var _cookieKey = null,

    // Application base path
    _basePath = null,

    // Container for cached UI element selectors
    _ui = {
        globalAccountSelector: null,
        accountSelector: null,
        mobileTabSelect: null,
        allTabs: null,
        tableBody: null,
        currentBalance: null,
        paymentAmount: null,
        interestRate: null,
        minPaymentPercentage: null
    },

    // Container for cached template HTML
    _templates = {
        paymentCalcRow: null,
        budgetCategoryEditForm: null,
        budgetCategoryDeleteForm: null
    },

    // Allow setup of options before init() is called
    _setOptions = function (options) {
        _cookieKey = options.cookieKey;
        _basePath = options.basePath;
    },

    // Calculate APR
    _aprCalc = function (apr) {
        apr = apr * 1 / 100;
        return (Math.pow((1 + apr), (1 / 12)) - 1) * 100;
    },

    // Build repayments table for card/loan repayment calculator
    _buildPaymentCalculatorTable = function (currentBalance, paymentAmount, interestRate, minPaymentPercentage) {
        // Set up variables
        var months = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'],
            balance = currentBalance, 
            html = [],
            maxRows = 100,
            row = 0,
            d = new Date();

        // Set date to the first of next month
        d.setDate(1);
        d.setMonth(d.getMonth() + 1);

        while (balance > 0 && row < maxRows) {
            // Work out how much interest will be paid
            var interestAmount = ((_aprCalc(interestRate) / 100) * balance),
                // Work out what the balance will be at the end of the month
                balanceAtMonthEnd = (balance - paymentAmount) + interestAmount;

            // If the balance is now negative, just set it to zero
            if (balanceAtMonthEnd < 0) {
                balanceAtMonthEnd = 0;
            }

            var model = {
                month: months[d.getMonth()],
                year: d.getFullYear(),
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
            // Increment the row counter
            row++;
        }

        _ui.tableBody.empty().append(html.join(''));
    },

    _init = function () {
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
        $('#net-worth').on('click', function (e) {
            e.preventDefault();
        }).popover({
            title: 'Accounts',
            placement: 'bottom',
            html: true,
            content: $('#net-worth-content').html(),
            container: 'body'
        });

        // Auto-submit global account selector form when dropdown option is changed
        _ui.globalAccountSelector.on('change', function () {
            this.form.submit();
        });

        // Reload transaction index when account is changed
        $('#Transactions_AccountID').on('change', function () {
            window.location = _basePath + 'Transactions/Index/' + $(this).val();
        });

        // Wire in mobile tab replacement dropdown
        _ui.mobileTabSelect.on('change', function () {
            _ui.allTabs.removeClass('active');
            $(_ui.mobileTabSelect.val()).addClass('active');
        });

        // Set up typeahead inputs (using Bootstrap 2.3.2 typeahead wrapped for BS3)
        $('.typeahead').typeahead({
            minlength: 1,
            items: 15,
            source: function (query, process) {
                $.ajax({
                    url: _basePath + 'Accounts/GetTransactionDescriptionHistory',
                    data: { query: query, id: (_ui.globalAccountSelector.length) ? _ui.globalAccountSelector.val() : _ui.accountSelector.val() },
                    type: 'POST'
                }).done(function (data) {
                    process(data);
                });
            }
        });

        // Add a confirmation to any delete forms
        $('form.delete-form').on('submit', function () {
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
        $(window).on('resize', function () {
            $('input.help, select.help, textarea.help, #net-worth').popover('hide');
        });

        // Add a new budget category form row
        $('#add-category-button').on('click', function () {
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
        $('form').on('click', '.cat-delete', function (e) {
            e.preventDefault();
            var control = $(this).parent().parent();
            var categoryId = this.hash.substring(4);
            var index = this.id.substring(3);
            control.replaceWith(Mustache.render(_templates.budgetCategoryDeleteForm, { INDEX: index, ID: categoryId }));
        });

        // Remove zero placeholder in amount fields on input focus
        // and replace it on blur if field is empty
        $('input[name$="Amount"]').on('focus', function () {
            var field = $(this);
            if (parseFloat(field.val()) === 0) {
                field.val('');
            }
        }).on('blur', function () {
            var field = $(this);
            if ($.trim(field.val()) === '' || parseFloat(field.val()) === 0) {
                field.val('0');
            }
        });
    };

    return {
        setOptions: _setOptions,
        init: _init
    };

})(jQuery, Mustache, window, undefined);

$(function () {

    MABMoney.init();
    
});