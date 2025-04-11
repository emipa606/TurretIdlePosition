using System;
using HarmonyLib;
using RimWorld;

namespace TurretIdlePosition;

[HarmonyPatch(typeof(TurretTop), nameof(TurretTop.TurretTopTick))]
public static class TurretTop_TurretTopTick
{
    private const float FullCircle = 360f;

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

        if (__state == __instance.CurRotation)
        {
            return;
        }

        var positionGameComponent = TurretIdlePosition.turretIdlePositionGameComponent;
        if (positionGameComponent == null)
        {
            return;
        }

        if (!positionGameComponent.TryGetTurretIdlePosition(___parentTurret, out var tuple))
        {
            return;
        }

        var minValue = (tuple.Item1 - tuple.Item2 + FullCircle) % FullCircle;
        var maxValue = (tuple.Item1 + tuple.Item2) % FullCircle;

        if (minValue > maxValue)
        {
            if (__instance.CurRotation > minValue || __instance.CurRotation < maxValue)
            {
                return;
            }

            if (Math.Max(minValue - __instance.CurRotation, __instance.CurRotation - maxValue) >
                Math.Max(minValue - __state, __state - maxValue))
            {
                return;
            }
        }
        else
        {
            if (__instance.CurRotation > minValue && __instance.CurRotation < maxValue)
            {
                return;
            }

            if (Math.Min((__instance.CurRotation - minValue + FullCircle) % FullCircle,
                    (maxValue - __instance.CurRotation + FullCircle) % FullCircle) < Math.Min(
                    (__state - minValue + FullCircle) % FullCircle,
                    (maxValue - __state + FullCircle) % FullCircle))
            {
                return;
            }
        }

        __instance.CurRotation = __state;
        ___idleTurnClockwise = !___idleTurnClockwise;
        ___idleTurnTicksLeft = 0;
    }
}