using HarmonyLib;
using Verse;

namespace TurretIdlePosition;

[HarmonyPatch(typeof(ThingWithComps), nameof(ThingWithComps.PostSwapMap))]
public static class ThingWithComps_PostSwapMap
{
    public static bool HasCalculated;

    public static void Postfix()
    {
        if (HasCalculated)
        {
            return;
        }

        HasCalculated = true;
        ThingWithComps_PreSwapMap.HasCalculated = false;
        var positionGameComponent = TurretIdlePosition.TurretIdlePositionGameComponent;
        positionGameComponent?.ThingPostMapSwap();
    }
}