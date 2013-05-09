var Stings = {};

(function ($, window) {
    var validStings = { 
        "rimshot": {
            audio: 'http://rimshot-bookmarklet.googlecode.com/svn/trunk/rimshot.',
            emoji: ':tongue:'
        },
        'loser': {
            audio: 'http://rimshot-bookmarklet.googlecode.com/svn/trunk/loser.',
            emoji: ':thumbsdown:'
        }
    };

    Stings.getStings = function () {
        var stings = [];
        for (var key in validStings) {
            stings.push(key + '^');
        }
        return stings;
    };

    Stings.Parser = function () {
        this.parse = function (content) {
            return parseStings(content);
        };

        this.transformToHtml = transformToHtml;

        function parseStings(content) {
            // No aliases at this stage so just return the given content
            return content;
        }

        function transformToHtml(content, emojiParser, isNewMessage) {
            return content.replace(/\^([a-z0-9\+\-_]+)\^/g, function (str, match) {
                var sting = validStings[match];
                if (sting) {
                    var emoji = emojiParser(sting.emoji);
                    if (isNewMessage) {
                        var audioElement = '<audio autoplay="true">' +
                            '<source src="' + sting.audio + 'wav" type="audio/wav" >' +
                            '<source src="' + sting.audio + 'ogg" type="audio/ogg" >' +
                            '</audio>';
                        return audioElement + emoji;
                    } else {
                        return emoji;
                    }
                }
                return content;
            });
        }
    };
})(jQuery, window);