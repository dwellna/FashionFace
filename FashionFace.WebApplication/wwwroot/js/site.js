// Write your JavaScript code.
let connection = new signalR.HubConnection('/Update', { transport: signalR.TransportType.LongPolling });
connection.on('update', () =>
{
  $.ajax({
    url: "/home/tweetstatistics",
    type: 'GET',
    async: 'true',
    success: function (response)
    {
      $('#tweetstatistics').html($($.parseHTML(response)).find('#tweetstatistics').html());
    }
  });
});

connection.start();