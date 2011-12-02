(function ($, window, ui) {
    "use strict";

    window.addContentProvider = function (provider) {
        var elements = $("#content-providers-list ul");

        // Grab any elements we need to process.
        var $provider = provider;
        var element = $("#provider_" + provider.Name.replace(" ", "_"));
        if (element.length == 0) {
            {
                // Process the template, and add it in to the div.
                $('#content-provider-template').tmpl(provider).appendTo(elements);
                $("#provider_cb_" + provider.Name.replace(" ", "_")).change(function (e) {
                    updateUiToReflectToggleState();
                });
            }
        }
        $(".content-providers-list").fadeIn();
        updateUiToReflectToggleState();
    }
})(jQuery, window, chat.ui);

function updateUiToReflectToggleState() {
    $(".content-provider-toggle").each(function (i, el) {
        console.log($(this));
        if (this.checked) {
            $("." + $(this).attr("data-name").replace(" ", "_")).show();
        }
        else {
            $("." + $(this).attr("data-name").replace(" ", "_")).hide();
        }
    });
}