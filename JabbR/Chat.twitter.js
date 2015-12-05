(function ($, window, ui) {
    "use strict";

    // Prevent multiple global events from being bound
    var twitterRenderBound = false;

    window.addTweet = function (tweet) {
        // Keep track of whether we're near the end, so we can auto-scroll once the tweet is added.
        var elements = null,
            tweetSegment = '/statuses/',
            currentMessages = $('.messages.current'),
            id = tweet.url.substring(tweet.url.indexOf(tweetSegment) + tweetSegment.length);

        // Grab any elements we need to process.
        elements = $('div.tweet_' + id)
        // Strip the classname off, so we don't process this again if someone posts the same tweet.
        .removeClass('tweet_' + id);

        // Process the template, and add it in to the div.
        $('#tweet-template').tmpl(tweet).appendTo(elements);

        //Checking 'twttr' reference because of dynamic Twitter API script
        if (window.twttr && !twitterRenderBound) {
            twitterRenderBound = true;
            twttr.events.bind('rendered', function (event) {
                if (currentMessages.scrollTop() + currentMessages.outerHeight() > currentMessages[0].scrollHeight - $(event.target).outerHeight() - 30)
                {
                    ui.scrollToBottom();
                }
            });
        }
    };

})(window.jQuery, window, chat.ui);
