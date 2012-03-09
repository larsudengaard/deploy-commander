 (function ($) {
	$.endpoint = {
		connectionId: 0,
		pendingSubscriptions: [],
		handlers: {},
		retryDelay: 50
	};

	$.endpoint.start = function () {
		$.ajax('/poll/negotiate',
			{
				type: "POST",
				success: function (connectionId) {
					$.endpoint.connectionId = connectionId;
					setTimeout($.endpoint.connect, 0);

					var subscription;
					while (subscription = $.endpoint.pendingSubscriptions.pop()) {
						$.endpoint.subscribe.apply($.endpoint, subscription);
					}
				},
				error: function () {
					throw new Error('Could not negotiate');
				}
			});
	};

	$.endpoint.connect = function () {
		if ($.endpoint.connectionId == 0)
			throw new Error('Not negotiated.');

		$.ajax('/poll/connect',
			{
				type: "POST",
				data: {
					connectionId: $.endpoint.connectionId
				},
				success: function (messages) {
					$.endpoint.connect();

					for (var j = 0; j < messages.length; j++) {
						var message = messages[j];
						var handlers = $.endpoint.handlers[message.channel + '_' + message.group];
						if (typeof handlers !== 'undefined') {
							for (var i = 0; i < handlers.length; i++) {
								handlers[i](message);
							}
						}
					}
				},
				error: function () {
					setTimeout(function() {
						$.endpoint.connect();
					}, $.endpoint.retryDelay);
				}
			});
	};

	$.endpoint.subscribe = function (channel, group, handler) {
		if ($.endpoint.connectionId == 0) {
			$.endpoint.pendingSubscriptions.push(arguments);
			return;
		}

		$.ajax('/poll/subscribe',
			{
				type: "POST",
				data: {
					connectionId: $.endpoint.connectionId,
					channel: channel,
					group: group
				},
				success: function () {
					var handlers = $.endpoint.handlers[channel + '_' + group];
					if (typeof handlers === 'undefined') {
						handlers = $.endpoint.handlers[channel + '_' + group] = [];
					}

					handlers.push(handler);
				},
				error: function () {
					throw new Error('Could not subscribe to channel ' + channel);
				}
			});
	};
})(jQuery);
