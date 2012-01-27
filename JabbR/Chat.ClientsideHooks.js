var interceptor = (function ($) {
    'use strict';
    var plugins = [];

    plugins.push({
        description: "alert - type /alert [message] to show a javascript alert box",
        filter: /(^\/alert )(.+$)/gi,
        func: function (input, match) {
            alert(match[0][2]);
            return null;
        }
    });

    plugins.push({
        description: "rungist - type /rungist [gistnumber] to run a file named jabbr.js from that gist",
        filter: /(^\/rungist )(\w+$)/gi,
        func: function (input, match) {
            runGist(match[0][2]);
            return null;
        }
    });

    plugins.push({
        description: "mark - type /mark [some text] to highlight all text containing [some text]",
        filter: /(^\/mark )(.+$)/gi,
        func: function (input, match) {
            var words = match[0][2];
            var regex = new RegExp(words, 'gi');
            $('.message > .middle').each(function () {
                var txt = $(this).html();
                if (txt && $(this).text() == txt) {
                    txt = txt.replace(regex, function (matched) {
                        return "<span class=\"highlite\" style=\"background-color:Yellow\">" + matched + "</span>";
                    });
                    $(this).html(txt);
                }
            });
            return null;
        }
    });

    plugins.push({
        description: "unmark - type /unmark to remove all text highlights",
        filter: /(^\/unmark)/gi,
        func: function (input, match) {
            $(".highlite").replaceWith(function () {
                return $(this).html();
            });
            return null;
        }
    });

    plugins.push({
        filter: /\/help/gi,
        func: function (input, match) {
            $("ul.messages.current").append("<li><div class='list-header'><div class='content'>Client script help</div></div></li>");
            for(var i in plugins) {
                var txt = plugins[i].description;
                if (txt)
                    $("ul.messages.current").append("<li><div class='list-item'><div class='content'>"+ txt +"</div></div></li>");
            }
            return input;
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

    function runGist(gistid, filename) {
        filename = filename || "jabbr.js"
        var url = 'http://raw.github.com/gist/' + gistid + '/' + filename;
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
        interceptMessage: interceptMessage,
        runGist: runGist
    };
})(jQuery);