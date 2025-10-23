using System.Collections.Generic;
using Mirage;
using NuclearOption.Networking;
using Nuclei.Helpers;

namespace Nuclei.Features;

/// <summary>
///     Manages server messaging. Not to be confused with chat messages: for functionality surrounding that, see
///     <see cref="ChatService" />.
/// </summary>
public static class MessageService
{
    /// <summary>
    ///     Sends a message to all clients.
    /// </summary>
    /// <param name="message">The message to send. </param>
    /// <param name="authenticatedOnly"> Whether to send the message only to authenticated clients. </param>
    /// <param name="excludeLocalPlayer"> Whether to exclude the local player from receiving the message. </param>
    /// <param name="channel"> The channel to send the message on. </param>
    /// <typeparam name="T">
    ///     The type of the message to send. Likely only supports messages with the
    ///     <see cref="NetworkMessageAttribute" />.
    /// </typeparam>
    public static void SendToAll<T>(T message, bool authenticatedOnly = false, bool excludeLocalPlayer = false, Channel channel = Channel.Reliable)
    {
        Globals.NetworkManagerNuclearOptionInstance.Server.SendToAll(message, authenticatedOnly, excludeLocalPlayer, channel);
    }

    /// <summary>
    ///     Sends a message to multiple clients.
    /// </summary>
    /// <param name="players"> The players to send the message to. </param>
    /// <param name="message"> The message to send. </param>
    /// <param name="excludeLocalPlayer"> Whether to exclude the local player from receiving the message. </param>
    /// <param name="channel"> The channel to send the message on. </param>
    /// <typeparam name="T">
    ///     The type of the message to send. Likely only supports messages with the
    ///     <see cref="NetworkMessageAttribute" />.
    /// </typeparam>
    public static void SendToMany<T>(IEnumerable<INetworkPlayer> players, T message, bool excludeLocalPlayer = false, Channel channel = Channel.Reliable)
    {
        Globals.NetworkManagerNuclearOptionInstance.Server.SendToMany(players, message, excludeLocalPlayer, channel);
    }

    /// <summary>
    ///     Sends a host ended message to all clients.
    /// </summary>
    public static void SendHostEndedMessage()
    {
        SendToAll(new HostEndedMessage
        {
            HostName = Globals.NetworkManagerNuclearOptionInstance.name
        }, excludeLocalPlayer: true);
    }
}