using HarmonyLib;
using RimWorld;
using Verse;

namespace TurretIdlePosition;

[HarmonyPatch(typeof(Building_TurretGun), nameof(Building_TurretGun.Tick))]
public static class Building_TurretGun_Tick
{
    public static void Postfix(Building_TurretGun __instance, TurretTop ___top)
    {
        if (!__instance.IsHashIntervalTick(GenTicks.TickRareInterval))
        {
            return;
        }

        var mannable = __instance.GetComp<CompMannable>();
        if (mannable == null || mannable.MannedNow)
        {
            return;
        }

        if (TurretIdlePosition.IsAllowedRotation(__instance, ___top, ___top.CurRotation, out var minValue,
                out var maxValue))
        {
            return;
        }


        if (minValue > maxValue)
        {
            maxValue += TurretIdlePosition.FullCircle;
        }

        ___top.CurRotation = Rand.Range(minValue, maxValue) % TurretIdlePosition.FullCircle;
    }
}