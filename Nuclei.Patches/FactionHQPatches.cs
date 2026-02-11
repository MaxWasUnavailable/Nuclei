using HarmonyLib;
using Mirage;
using NuclearOption;
using Nuclei.Abstractions.NO.Airbases;
using Nuclei.Abstractions.NO.Factions;
using Nuclei.Abstractions.NO.Positions;
using Nuclei.Abstractions.NO.Units;
using Nuclei.Abstractions.NO.Zones;
using Nuclei.Adapters.Units;
using Nuclei.Events.Events;

// ReSharper disable InconsistentNaming

namespace Nuclei.Patches;

[HarmonyPatch(typeof(FactionHQ))]
[HarmonyPriority(Priority.First)]
[HarmonyWrapSafe]
internal static class FactionHQPatches
{
    private static IUnitLookup UnitLookup { get; } = new UnitLookup();

    [HarmonyPrefix]
    [HarmonyPatch(nameof(FactionHQ.RegisterExclusionZone))]
    private static void RegisterExclusionZonePrefix(FactionHQ __instance, ExclusionZone exclusionZone)
    {
        FactionEvents.OnPreExclusionZoneRegistered(new FactionExclusionZoneEvent(ToFactionInfo(__instance), ToZoneInfo(exclusionZone)));
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(FactionHQ.RegisterExclusionZone))]
    private static void RegisterExclusionZonePostfix(FactionHQ __instance, ExclusionZone exclusionZone)
    {
        FactionEvents.OnPostExclusionZoneRegistered(new FactionExclusionZoneEvent(ToFactionInfo(__instance), ToZoneInfo(exclusionZone)));
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(FactionHQ.OnPlayersChange))]
    private static void OnPlayersChangePrefix(FactionHQ __instance)
    {
        FactionEvents.OnPreFactionRosterChanged(new FactionRosterEvent(ToFactionInfo(__instance)));
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(FactionHQ.OnPlayersChange))]
    private static void OnPlayersChangePostfix(FactionHQ __instance)
    {
        FactionEvents.OnPostFactionRosterChanged(new FactionRosterEvent(ToFactionInfo(__instance)));
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(FactionHQ.OnAirbaseAdded))]
    private static void OnAirbaseAddedPrefix(FactionHQ __instance, int index, NetworkBehaviorSyncvar<Airbase> value)
    {
        var airbase = value.Value;
        FactionEvents.OnPreAirbaseAdded(new FactionAirbaseEvent(ToFactionInfo(__instance), ToAirbaseInfo(airbase)));
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(FactionHQ.OnAirbaseAdded))]
    private static void OnAirbaseAddedPostfix(FactionHQ __instance, int index, NetworkBehaviorSyncvar<Airbase> value)
    {
        var airbase = value.Value;
        FactionEvents.OnPostAirbaseAdded(new FactionAirbaseEvent(ToFactionInfo(__instance), ToAirbaseInfo(airbase)));
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(FactionHQ.OnAirbaseRemoved))]
    private static void OnAirbaseRemovedPrefix(FactionHQ __instance, int index, NetworkBehaviorSyncvar<Airbase> value)
    {
        var airbase = value.Value;
        FactionEvents.OnPreAirbaseRemoved(new FactionAirbaseEvent(ToFactionInfo(__instance), ToAirbaseInfo(airbase)));
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(FactionHQ.OnAirbaseRemoved))]
    private static void OnAirbaseRemovedPostfix(FactionHQ __instance, int index, NetworkBehaviorSyncvar<Airbase> value)
    {
        var airbase = value.Value;
        FactionEvents.OnPostAirbaseRemoved(new FactionAirbaseEvent(ToFactionInfo(__instance), ToAirbaseInfo(airbase)));
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(FactionHQ.SetTrackingState))]
    private static void SetTrackingStatePrefix(FactionHQ __instance, PersistentID id, GlobalPosition lastKnownPosition, float lastSpottedTime, ref bool __state)
    {
        __state = !__instance.trackingDatabase.ContainsKey(id);
        if (__state)
            UnitEvents.OnPreUnitTracked(new UnitTrackingEvent(ToFactionInfo(__instance), UnitLookup.FromPersistentId(id), ToGlobalPosition(lastKnownPosition)));
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(FactionHQ.SetTrackingState))]
    private static void SetTrackingStatePostfix(FactionHQ __instance, PersistentID id, bool __state)
    {
        if (__state && __instance.trackingDatabase.ContainsKey(id))
            UnitEvents.OnPostUnitTracked(new UnitTrackingEvent(ToFactionInfo(__instance), UnitLookup.FromPersistentId(id), null));
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(FactionHQ.DeregisterTrackedUnit))]
    private static void DeregisterTrackedUnitPrefix(FactionHQ __instance, Unit unit, ref bool __state)
    {
        if (!unit)
        {
            __state = false;
            return;
        }

        var id = unit.persistentID;
        __state = __instance.trackingDatabase.ContainsKey(id);
        if (__state)
            UnitEvents.OnPreUnitUntracked(new UnitTrackingEvent(ToFactionInfo(__instance), UnitLookup.FromPersistentId(id), null));
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(FactionHQ.DeregisterTrackedUnit))]
    private static void DeregisterTrackedUnitPostfix(FactionHQ __instance, Unit unit, bool __state)
    {
        if (!__state || !unit)
            return;

        var id = unit.persistentID;
        if (!__instance.trackingDatabase.ContainsKey(id))
            UnitEvents.OnPostUnitUntracked(new UnitTrackingEvent(ToFactionInfo(__instance), UnitLookup.FromPersistentId(id), null));
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(FactionHQ.HQ_OnUnitChangeFaction))]
    private static void HQOnUnitChangeFactionPrefix(FactionHQ __instance, Unit unit, ref bool __state)
    {
        __state = unit && unit.NetworkHQ == __instance && __instance.trackingDatabase.ContainsKey(unit.persistentID);
        if (__state)
            UnitEvents.OnPreUnitFactionChanged(new UnitFactionChangedEvent(ToFactionInfo(__instance), UnitLookup.FromPersistentId(unit.persistentID)));
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(FactionHQ.HQ_OnUnitChangeFaction))]
    private static void HQOnUnitChangeFactionPostfix(FactionHQ __instance, Unit unit, bool __state)
    {
        if (!__state || !unit)
            return;

        UnitEvents.OnPostUnitFactionChanged(new UnitFactionChangedEvent(ToFactionInfo(__instance), UnitLookup.FromPersistentId(unit.persistentID)));
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(FactionHQ.RegisterFactionUnit))]
    private static void RegisterFactionUnitPrefix(FactionHQ __instance, Unit unit)
    {
        if (!unit)
            return;

        UnitEvents.OnPreUnitRegistered(new FactionUnitEvent(ToFactionInfo(__instance), ToUnitInfo(unit)));
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(FactionHQ.RegisterFactionUnit))]
    private static void RegisterFactionUnitPostfix(FactionHQ __instance, Unit unit)
    {
        if (!unit)
            return;

        UnitEvents.OnPostUnitRegistered(new FactionUnitEvent(ToFactionInfo(__instance), ToUnitInfo(unit)));
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(FactionHQ.RemoveFactionUnit))]
    private static void RemoveFactionUnitPrefix(FactionHQ __instance, Unit unit)
    {
        if (!unit)
            return;

        UnitEvents.OnPreUnitRemoved(new FactionUnitEvent(ToFactionInfo(__instance), ToUnitInfo(unit)));
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(FactionHQ.RemoveFactionUnit))]
    private static void RemoveFactionUnitPostfix(FactionHQ __instance, Unit unit)
    {
        if (!unit)
            return;

        UnitEvents.OnPostUnitRemoved(new FactionUnitEvent(ToFactionInfo(__instance), ToUnitInfo(unit)));
    }

    private static IFactionInfo ToFactionInfo(FactionHQ hq)
    {
        var name = hq.faction?.factionExtendedName ?? hq.faction?.factionName ?? "Unknown";
        return new FactionInfo(name);
    }

    private static IAirbaseInfo ToAirbaseInfo(Airbase airbase)
    {
        var name = airbase.SavedAirbase?.DisplayName ?? airbase.name ?? "Unknown";
        return new AirbaseInfo(name);
    }

    private static IUnitInfo ToUnitInfo(Unit unit)
    {
        var id = unit.persistentID.ToString() ?? string.Empty;
        var name = unit.unitName ?? unit.name ?? "Unknown";
        return new UnitInfo(id, name);
    }


    private static IExclusionZoneInfo ToZoneInfo(ExclusionZone zone)
    {
        return new ExclusionZoneInfo(zone.sourceId.ToString());
    }

    private static IGlobalPosition ToGlobalPosition(GlobalPosition position)
    {
        return new GlobalPositionInfo(position.x, position.y, position.z);
    }
}