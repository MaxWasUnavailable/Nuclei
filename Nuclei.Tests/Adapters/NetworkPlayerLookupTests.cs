using System;
using FluentAssertions;
using Mirage;
using Moq;
using Nuclei.Adapters.Players;

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.

namespace Nuclei.Tests.Adapters;

/// <summary>
///     Tests for <see cref="NetworkPlayerLookup" />.
/// </summary>
public sealed class NetworkPlayerLookupTests
{
    [Test]
    public void FromNetworkPlayer_WithNoIdentity_ReturnsDefaultValues()
    {
        var networkPlayer = new Mock<INetworkPlayer>();
        networkPlayer.SetupGet(p => p.Identity).Returns((NetworkIdentity?)null);

        var lookup = new NetworkPlayerLookup();
        var info = lookup.FromNetworkPlayer(networkPlayer.Object);

        info.Name.Should().BeNull();
        info.SteamId.Should().Be(0UL);
    }

    [Test]
    public void FromNetworkPlayer_WithNullArgument_Throws()
    {
        var lookup = new NetworkPlayerLookup();

        var act = () => lookup.FromNetworkPlayer(null!);

        act.Should().Throw<ArgumentNullException>();
    }
}
