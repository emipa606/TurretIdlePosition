using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace TurretIdlePosition;

[HarmonyPatch]
public static class Building_CMCTurretGun_Tick
{
    private static FieldInfo TopField;
    private static PropertyInfo TopRotationProperty;

    public static bool Prepare()
    {
        return ModLister.GetActiveModWithIdentifier("TOT.CeleTech.MKIII") != null;
    }

    public static MethodBase TargetMethod()
    {
        TopField = AccessTools.Field("TOT_DLL_test.Building_CMCTurretGun:turrettop");
        TopRotationProperty = AccessTools.Property("TOT_DLL_test.CMCTurretTop:DestRotation");
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

        var top = TopField.GetValue(__instance);
        if (top == null)
        {
            return;
        }

        var topRotationProperty = TopRotationProperty.GetValue(top);
        if (topRotationProperty == null)
        {
            return;
        }

        if (TurretIdlePosition.IsAllowedRotation(__instance, (float)topRotationProperty, (float)topRotationProperty,
                out var minValue,
                out var maxValue))
        {
            return;
        }

        if (minValue > maxValue)
        {
            maxValue += TurretIdlePosition.FullCircle;
        }

        TopRotationProperty.SetValue(top, Rand.Range(minValue, maxValue) % TurretIdlePosition.FullCircle);
    }
}