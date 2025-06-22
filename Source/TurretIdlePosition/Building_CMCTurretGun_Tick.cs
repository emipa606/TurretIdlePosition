using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace TurretIdlePosition;

[HarmonyPatch]
public static class Building_CMCTurretGun_Tick
{
    private static FieldInfo topFieldInfo;
    private static PropertyInfo topRotationPropertyInfo;

    public static bool Prepare()
    {
        return ModLister.GetActiveModWithIdentifier("TOT.CeleTech.MKIII", true) != null;
    }

    public static MethodBase TargetMethod()
    {
        topFieldInfo = AccessTools.Field("TOT_DLL_test.Building_CMCTurretGun:turrettop");
        topRotationPropertyInfo = AccessTools.Property("TOT_DLL_test.CMCTurretTop:DestRotation");
        return AccessTools.Method("TOT_DLL_test.Building_CMCTurretGun:Tick");
    }

    public static void Postfix(Building_TurretGun __instance)
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

        var top = topFieldInfo.GetValue(__instance);
        if (top == null)
        {
            return;
        }

        var rotationProperty = topRotationPropertyInfo.GetValue(top);
        if (rotationProperty == null)
        {
            return;
        }

        if (TurretIdlePosition.IsAllowedRotation(__instance, (float)rotationProperty, (float)rotationProperty,
                out var minValue,
                out var maxValue))
        {
            return;
        }

        if (minValue > maxValue)
        {
            maxValue += TurretIdlePosition.FullCircle;
        }

        topRotationPropertyInfo.SetValue(top, Rand.Range(minValue, maxValue) % TurretIdlePosition.FullCircle);
    }
}