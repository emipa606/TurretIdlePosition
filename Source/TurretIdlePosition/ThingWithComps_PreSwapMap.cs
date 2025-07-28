using HarmonyLib;
using Verse;

namespace TurretIdlePosition;

[HarmonyPatch(typeof(ThingWithComps), nameof(ThingWithComps.PreSwapMap))]
public static class ThingWithComps_PreSwapMap
{
    public static bool HasCalculated;

    public static void Postfix()
    {
        if (HasCalculated)
        {
            return;
        }

        HasCalculated = true;
        ThingWithComps_PostSwapMap.HasCalculated = false;
        var positionGameComponent = TurretIdlePosition.TurretIdlePositionGameComponent;
        positionGameComponent?.ThingPreMapSwap();
    }
}