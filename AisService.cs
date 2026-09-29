using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace ShipTracker;

/// <summary>
/// Service connecting to AISStream.io, which issues realtime AIS ship data.
/// Establishes a websocket connection and received JSON messages.
////// See full documentation at https://aisstream.io/documentation
/// </summary>
public sealed class AisService : IAsyncDisposable
{
  readonly Uri Endpoint = new("wss://stream.aisstream.io/v0/stream");
  ClientWebSocket? webSocket;
  CancellationTokenSource? receiveLoopCancellationToken;
  Task? receiveLoopTask;

  /// <summary>
  /// Connects to AISStream.io with a subscription to a websocket.
  /// </summary>
  /// <param name="apiKey">Your free AISStream.io API key (available at https://aisstream.io).</param>
  /// <param name="boundingBoxes">One or more [[lat1,lon1],[lat2,lon2]] corner pairs describing the areas to track./// </param>
  /// </summary>
  public async Task ConnectAsync(string apiKey, double[][][] boundingBoxes, string[] messageTypeFilter)
  {
    if (string.IsNullOrWhiteSpace(apiKey))
      throw new ArgumentException($"API key not provided. {Environment.NewLine}" +
                                  $"An API key is required to connect to the AISStream.io service. {Environment.NewLine}" +
                                  $"Go to https://aisstream.io/ to get a free key.");

    webSocket = new ClientWebSocket();
    webSocket.Options.DangerousDeflateOptions = new WebSocketDeflateOptions();

    FireStatusChanged("Connecting to AISStream.io...");
    await webSocket.ConnectAsync(Endpoint, CancellationToken.None).ConfigureAwait(false);

    var subscription = new Dictionary<string, object?>
    {
      ["APIKey"] = apiKey,
      ["BoundingBoxes"] = boundingBoxes,
      ["FilterMessageTypes"] = messageTypeFilter
    };

    var payload = JsonSerializer.SerializeToUtf8Bytes(subscription);

    await webSocket.SendAsync(payload, WebSocketMessageType.Text, endOfMessage: true, CancellationToken.None)
                   .ConfigureAwait(false);

    FireStatusChanged("Server connected");

    receiveLoopCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken.None);
    receiveLoopTask = Task.Run(() => ListenForMessagesAsync(receiveLoopCancellationToken.Token), CancellationToken.None);
  }

  async Task ListenForMessagesAsync(CancellationToken token)
  {
    var buffer = new byte[32 * 1024];
    using var messageStream = new MemoryStream();

    try
    {
      while (webSocket is { State: WebSocketState.Open } && !token.IsCancellationRequested)
      {
        messageStream.SetLength(0);  // initialize stream
        WebSocketReceiveResult result;
        do
        {
          result = await webSocket.ReceiveAsync(buffer, token).ConfigureAwait(false);
          if (result.MessageType == WebSocketMessageType.Close)
          {
            FireStatusChanged($"Server closed the connection ({result.CloseStatusDescription}).");
            return;
          }
          messageStream.Write(buffer, 0, result.Count);
        } while (!result.EndOfMessage);

        messageStream.Position = 0;
        try
        {
          var envelope = JsonSerializer.Deserialize<AisEnvelope>(messageStream);
          if (envelope is null) return;

          FireMessageReceived(envelope);
        }
        catch (JsonException ex)
        {
          FireException(ex);
        }
      }
    }
    catch (OperationCanceledException)
    {
      // not an error: expected when the caller disconnects
    }
    catch (WebSocketException)
    {
      FireException(new Exception("Connection error. The API key may be invalid"));
      FireStatusChanged($"Server disconnected");
    }
    catch (Exception ex)
    {
      FireException(ex);
      FireStatusChanged($"Server disconnected");
    }
  }

  public async Task DisconnectAsync()
  {
    try
    {
      receiveLoopCancellationToken?.Cancel();

      if (webSocket is { State: WebSocketState.Open })
      {
        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client closing", CancellationToken.None)
                       .ConfigureAwait(false);
      }

      if (receiveLoopTask is not null)
        await receiveLoopTask.ConfigureAwait(false);
    }
    catch { }
    finally
    {
      FireStatusChanged("Server disconnected");
    }
  }

  public async ValueTask DisposeAsync()
  {
    await DisconnectAsync().ConfigureAwait(false);
    webSocket?.Dispose();
    receiveLoopCancellationToken?.Dispose();
  }

  #region Events
  public event Action<string>? StatusChanged;
  void FireStatusChanged(string text)
  {
    StatusChanged?.Invoke(text);
  }

  public event Action<AisEnvelope>? MessageReceived;
  void FireMessageReceived(AisEnvelope message)
  {
    MessageReceived?.Invoke(message);
  }

  public event Action<Exception>? Exception;
  void FireException(Exception ex)
  {
    Exception?.Invoke(ex);
  }
  #endregion
}
