/// <reference path="Scripts/jquery-1.7.js" />
/// <reference path="Scripts/toastr.js" />
(function ($) {
    "use strict";

    var toastTimeOut = 10000,
        chromeToast = null;

    var chromePolyfill = {
        ToastStatus: { Allowed: 0, NotConfigured: 1, Blocked: 2 },
        canToast: function () {
            // we can toast if the user hasn't explicitly denied
            return window.webkitNotifications.checkPermission() !== this.ToastStatus.Blocked;
        },
        ensureToast: function (preferences) {
            if (window.webkitNotifications.checkPermission() === this.ToastStatus.NotConfigured) {
                preferences.canToast = false;
            }
        },
        toastMessage: function (message, roomName) {
            if (window.webkitNotifications.checkPermission() !== this.ToastStatus.Allowed) {
                return;
            }

            // Hide any previously displayed toast
            toast.hideToast();

            chromeToast = window.webkitNotifications.createNotification(
                'Content/images/logo32.png',
                message.trimmedName,
                $('<div/>').html(message.message).text());

            chromeToast.ondisplay = function () {
                setTimeout(function () {
                    chromeToast.cancel();
                }, toastTimeOut);
            };

            chromeToast.onclick = function () {
                toast.hideToast();

                // Trigger the focus event
                $(toast).trigger('toast.focus', [roomName]);
            };

            chromeToast.show();
        },
        hideToast: function () {
            if (chromeToast && chromeToast.cancel) {
                chromeToast.cancel();
            }
        },
        enableToast: function (callback) {
            var deferred = $.Deferred();
            // If not configured, request permission
            if (window.webkitNotifications.checkPermission() === this.ToastStatus.NotConfigured) {
                window.webkitNotifications.requestPermission(function () {
                    if (window.webkitNotifications.checkPermission()) {
                        deferred.reject();
                    }
                    else {
                        deferred.resolve();
                    }
                });
            }
            else if (window.webkitNotifications.checkPermission() === this.ToastStatus.Allowed) {
                // If we're allowed then just resolve here
                deferred.resolve();
            }
            else {
                // We don't have permission
                deferred.reject();
            }

            return deferred;
        }
    };

    var toastrPolyfill = {
        canToast: function () {
            return true;
        },
        ensureToast: function (preferences) {
            preferences.canToast = true;
        },
        toastMessage: function (message, roomName) {
            var overrides = {}
            overrides.timeOut = toastTimeOut;
            toastr.info(message.message, message.trimmedName, overrides);
        },
        hideToast: function () {
        },
        enableToast: function (callback) {
            var deferred = $.Deferred();
            deferred.resolve();
            return deferred;
        }
    };

    var toast = window.webkitNotifications ? chromePolyfill : toastrPolyfill;

    if (!window.chat) {
        window.chat = {};
    }
    window.chat.toast = toast;
})(jQuery);