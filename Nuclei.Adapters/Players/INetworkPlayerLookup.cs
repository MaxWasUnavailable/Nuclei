using Mirage;
using Nuclei.Abstractions.Players;

namespace Nuclei.Adapters.Players;

/// <summary>
///     Adapter for resolving player information from network player objects.
/// </summary>
public interface INetworkPlayerLookup
{
    /// <summary>
    ///     Builds a stable player info snapshot for the given network player.
    /// </summary>
    IPlayerInfo FromNetworkPlayer(INetworkPlayer networkPlayer);
}

