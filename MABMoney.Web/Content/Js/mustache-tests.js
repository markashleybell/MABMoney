/// <reference path="mustache.d.ts" />
var view = { title: "Joe", calc: function () {
        return 2 + 4;
    } };

var template = "{{title}} spends {{calc}}";
var output = Mustache.render(template, view);

var template2 = "{{title}} spends {{calc}}";
Mustache.parse(template2, null);
var output2 = Mustache.render(template2, view);

var person;
var template = "<h1>{{firstName}} {{lastName}}</h1>Blog: {{blogURL}}";
var html = Mustache.to_html(template, person);
//# sourceMappingURL=mustache-tests.js.map
