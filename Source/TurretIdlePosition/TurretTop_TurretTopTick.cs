using HarmonyLib;
using RimWorld;

namespace TurretIdlePosition;

[HarmonyPatch(typeof(TurretTop), nameof(TurretTop.TurretTopTick))]
public static class TurretTop_TurretTopTick
{
    public static void Prefix(out float __state, TurretTop __instance)
    {
        __state = __instance.CurRotation;
    }

    public static void Postfix(float __state, TurretTop __instance, Building_Turret ___parentTurret,
        ref int ___idleTurnTicksLeft, ref bool ___idleTurnClockwise)
    {
        if (___parentTurret.CurrentTarget.IsValid)
        {
            return;
        }

        var mannable = ___parentTurret.GetComp<CompMannable>();
        if (mannable != null)
        {
            return;
        }

        if (__state == __instance.CurRotation)
        {
            return;
        }

        if (TurretIdlePosition.IsAllowedRotation(___parentTurret, __instance, __state, out _, out _))
        {
            return;
        }

        __instance.CurRotation = __state;
        ___idleTurnClockwise = !___idleTurnClockwise;
        ___idleTurnTicksLeft = 0;
    }
}