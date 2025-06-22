using HarmonyLib;
using RimWorld;
using Verse;

namespace TurretIdlePosition;

[HarmonyPatch(typeof(Building_TurretGun), "Tick")]
public static class Building_TurretGun_Tick
{
    public static void Postfix(Building_TurretGun __instance, TurretTop ___top)
    {
        if (!__instance.IsHashIntervalTick(GenTicks.TickRareInterval))
        {
            return;
        }

        if (__instance.CurrentTarget != null)
        {
            return;
        }

        if (__instance.GetComp<CompMannable>()?.MannedNow == true)
        {
            return;
        }

        if (TurretIdlePosition.SelfRotatingTurrets.Contains(__instance.def))
        {
            return;
        }

        if (TurretIdlePosition.IsAllowedRotation(__instance, ___top.CurRotation, ___top.CurRotation, out var minValue,
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