using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

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

        var acceptableRange = new FloatRange(tuple.Item1 + FullCircle - tuple.Item2,
            tuple.Item1 + FullCircle + tuple.Item2);

        if (acceptableRange.Includes(__instance.CurRotation + FullCircle))
        {
            return;
        }

        var stateDifference = Mathf.Abs(acceptableRange.Average - (__state + FullCircle));
        var curRotationDifference = Mathf.Abs(acceptableRange.Average - (__instance.CurRotation + FullCircle));

        if (stateDifference > curRotationDifference)
        {
            return;
        }

        __instance.CurRotation = __state;
        ___idleTurnClockwise = !___idleTurnClockwise;
        ___idleTurnTicksLeft = 0;
    }
}