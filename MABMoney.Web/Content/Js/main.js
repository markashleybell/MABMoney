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

    $('#account-selector').on('change', function (evt) {
        this.form.submit();
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

});