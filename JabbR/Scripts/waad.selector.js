(function ($, window) {
    'use strict';

    window.waadSelector = function (namespace, realm) {
        var script = document.createElement('script');
        script.src = 'https://' + namespace + '.accesscontrol.windows.net/v2/metadata/IdentityProviders.js?protocol=wsfederation&realm=' + realm + '&context=' + escape(document.location.hash) + '&request_id=&version=1.0&callback=waadIdentityProvidersLoaded';
        document.getElementsByTagName('head')[0].appendChild(script);
    };

    window.waadIdentityProvidersLoaded = function (identityProviders) {
        var selector = $('#waad-idp-selector').tmpl({ IdentityProviders: identityProviders });
        selector.appendTo($("#popups"));
        selector.modal();
    };
})(jQuery, window);