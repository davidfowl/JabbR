var interceptor = (function ($) {
    'use strict';
    var plugins = [];

    plugins.push({
        filter: /(^\/alert )(.+$)/gi,
        func: function (input, match) {
            alert(match[0][2]);
            return "";
        }
    });

    plugins.push({
        filter: /(^\/gistplugin )(\w+$)/gi,
        func: function (input, match) {
            injectGists(match[0][2]);
            return "";
        }
    });

    function interceptMessage(msg) {
        for (var i in plugins) {
            var matches = [];
            var match;
            while (match = plugins[i].filter.exec(msg))
                matches.push(match);
            if (matches.length) {
                msg = plugins[i].func(msg, matches);
            }
        }
        return msg;
    }

    function injectGists(gistid) {
        var url = 'http://gist.github.com/' + gistid + '.js?file=plugins.js';
        url = 'http://raw.github.com/gist/' + gistid + '/plugins.js';
        $.ajax({
            type: "GET",
            url: url,
            dataType: "jsonp",
            success: function (data) {
                eval(data);
            }
        });
    }

    return {
        plugins: plugins,
        interceptMessage: interceptMessage
    };
})(jQuery);