using HarmonyLib;
using Mirage;
using NuclearOption.Networking;
using Nuclei.Abstractions.NO.Airbases;
using Nuclei.Abstractions.NO.Factions;
using Nuclei.Abstractions.NO.Players;
using Nuclei.Adapters.Players;
using Nuclei.Adapters.Units;
using Nuclei.Events.Events;

// ReSharper disable InconsistentNaming

namespace Nuclei.Patches.Patches;

[HarmonyPatch(typeof(MessageManager))]
[HarmonyPriority(Priority.First)]
[HarmonyWrapSafe]
internal static class MessageManagerPatches
{
    private static INetworkPlayerLookup Lookup { get; } = new NetworkPlayerLookup();
    private static IUnitLookup UnitLookup { get; } = new UnitLookup();

    [HarmonyPrefix]
    [HarmonyPatch(nameof(MessageManager.JoinMessage))]
    private static void JoinMessagePrefix(Player joinedPlayer, ref PlayerAnnouncementEvent __state)
    {
        __state = new PlayerAnnouncementEvent(ToPlayerInfo(joinedPlayer));
        AnnouncementEvents.OnPrePlayerJoinedAnnouncement(__state);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(MessageManager.JoinMessage))]
    private static void JoinMessagePostfix(Player joinedPlayer, PlayerAnnouncementEvent __state)
    {
        AnnouncementEvents.OnPostPlayerJoinedAnnouncement(__state);
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(MessageManager.DisconnectedMessage))]
    private static void DisconnectedMessagePrefix(Player player, ref PlayerAnnouncementEvent __state)
    {
        __state = new PlayerAnnouncementEvent(ToPlayerInfo(player));
        AnnouncementEvents.OnPrePlayerDisconnectedAnnouncement(__state);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(MessageManager.DisconnectedMessage))]
    private static void DisconnectedMessagePostfix(Player player, PlayerAnnouncementEvent __state)
    {
        AnnouncementEvents.OnPostPlayerDisconnectedAnnouncement(__state);
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(MessageManager.RpcPlayerJoinFactionMessage))]
    private static void PlayerJoinFactionPrefix(Player player, FactionHQ hq, ref PlayerFactionAnnouncementEvent __state)
    {
        __state = new PlayerFactionAnnouncementEvent(ToPlayerInfo(player), ToFactionInfo(hq));
        AnnouncementEvents.OnPrePlayerJoinedFactionAnnouncement(__state);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(MessageManager.RpcPlayerJoinFactionMessage))]
    private static void PlayerJoinFactionPostfix(Player player, FactionHQ hq, PlayerFactionAnnouncementEvent __state)
    {
        AnnouncementEvents.OnPostPlayerJoinedFactionAnnouncement(__state);
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(MessageManager.RpcAllHQMessage))]
    private static void RpcAllHqPrefix(string message, ref TextAnnouncementEvent __state)
    {
        __state = new TextAnnouncementEvent(message);
        AnnouncementEvents.OnPreHqBroadcastAnnouncement(__state);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(MessageManager.RpcAllHQMessage))]
    private static void RpcAllHqPostfix(string message, TextAnnouncementEvent __state)
    {
        AnnouncementEvents.OnPostHqBroadcastAnnouncement(__state);
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(MessageManager.RpcHQMessage))]
    private static void RpcHqPrefix(FactionHQ hq, string message, ref HqTextAnnouncementEvent __state)
    {
        __state = new HqTextAnnouncementEvent(ToFactionInfo(hq), message);
        AnnouncementEvents.OnPreHqAnnouncement(__state);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(MessageManager.RpcHQMessage))]
    private static void RpcHqPostfix(FactionHQ hq, string message, HqTextAnnouncementEvent __state)
    {
        AnnouncementEvents.OnPostHqAnnouncement(__state);
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(MessageManager.TargetCreditMessage))]
    private static void TargetCreditMessagePrefix(INetworkPlayer player, PersistentID killedId, float creditAwarded, FactionHQ.RewardType actionType, ref CreditAwardedEvent __state)
    {
        var recipient = Lookup.FromNetworkPlayer(player);
        var target = UnitLookup.FromPersistentId(killedId);
        __state = new CreditAwardedEvent(recipient, target, creditAwarded, actionType.ToString());
        CombatEvents.OnPreCreditAwarded(__state);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(MessageManager.TargetCreditMessage))]
    private static void TargetCreditMessagePostfix(INetworkPlayer player, PersistentID killedId, float creditAwarded, FactionHQ.RewardType actionType, CreditAwardedEvent __state)
    {
        CombatEvents.OnPostCreditAwarded(__state);
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(MessageManager.RpcBombFailMessage))]
    private static void BombFailPrefix(PersistentID bombId, float gForce, ref BombFailureEvent __state)
    {
        __state = new BombFailureEvent(UnitLookup.FromPersistentId(bombId), gForce);
        CombatEvents.OnPreBombFailed(__state);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(MessageManager.RpcBombFailMessage))]
    private static void BombFailPostfix(PersistentID bombId, float gForce, BombFailureEvent __state)
    {
        CombatEvents.OnPostBombFailed(__state);
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(MessageManager.RpcKillMessage))]
    private static void KillMessagePrefix(PersistentID killerId, PersistentID killedId, KillType killedType, ref KillFeedEvent __state)
    {
        __state = new KillFeedEvent(UnitLookup.FromPersistentId(killerId), UnitLookup.FromPersistentId(killedId), killedType.ToString());
        CombatEvents.OnPreKillFeed(__state);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(MessageManager.RpcKillMessage))]
    private static void KillMessagePostfix(PersistentID killerId, PersistentID killedId, KillType killedType, KillFeedEvent __state)
    {
        CombatEvents.OnPostKillFeed(__state);
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(MessageManager.RpcPilotCaptureMessage))]
    private static void PilotCapturePrefix(PersistentID id, bool rescued, ref PilotCaptureEvent __state)
    {
        __state = new PilotCaptureEvent(UnitLookup.FromPersistentId(id), rescued);
        CombatEvents.OnPrePilotCaptured(__state);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(MessageManager.RpcPilotCaptureMessage))]
    private static void PilotCapturePostfix(PersistentID id, bool rescued, PilotCaptureEvent __state)
    {
        CombatEvents.OnPostPilotCaptured(__state);
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(MessageManager.RpcRepairMessage))]
    private static void RepairPrefix(PersistentID id, ref RepairEvent __state)
    {
        __state = new RepairEvent(UnitLookup.FromPersistentId(id));
        CombatEvents.OnPreRepairReported(__state);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(MessageManager.RpcRepairMessage))]
    private static void RepairPostfix(PersistentID id, RepairEvent __state)
    {
        CombatEvents.OnPostRepairReported(__state);
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(MessageManager.RpcWarheadDestroyedMessage))]
    private static void WarheadDestroyedPrefix(Airbase airbase, FactionHQ hq, int number, ref WarheadDestroyedEvent __state)
    {
        __state = new WarheadDestroyedEvent(ToAirbaseInfo(airbase), ToFactionInfo(hq), number);
        CombatEvents.OnPreWarheadDestroyed(__state);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(MessageManager.RpcWarheadDestroyedMessage))]
    private static void WarheadDestroyedPostfix(Airbase airbase, FactionHQ hq, int number, WarheadDestroyedEvent __state)
    {
        CombatEvents.OnPostWarheadDestroyed(__state);
    }

    // TODO: centralize converter methods like this in a single location

    private static IPlayerInfo ToPlayerInfo(Player player)
    {
        var name = player?.GetNameOrCensored() ?? player?.name ?? "Unknown";
        return new PlayerInfo(name, 0UL);
    }

    private static IFactionInfo ToFactionInfo(FactionHQ hq)
    {
        var name = hq?.faction?.factionExtendedName ?? hq?.faction?.factionName ?? "Unknown";
        return new FactionInfo(name);
    }

    private static IAirbaseInfo ToAirbaseInfo(Airbase airbase)
    {
        var name = airbase?.SavedAirbase?.DisplayName ?? airbase?.name ?? "Unknown";
        return new AirbaseInfo(name);
    }
}