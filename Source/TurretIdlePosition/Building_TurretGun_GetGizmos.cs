using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace TurretIdlePosition;

[HarmonyPatch]
public static class Building_Turret_GetGizmos
{
    private static IEnumerable<MethodBase> TargetMethods()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assembly in assemblies)
        {
            foreach (var type in assembly.GetTypes().Where(type =>
                         type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(Building_Turret))))
            {
                var getGizmoMethod = type.GetMethod("GetGizmos");
                if (getGizmoMethod == null)
                {
                    continue;
                }

                TurretIdlePosition.AllPossibleTurrets.Add(type);
                yield return getGizmoMethod;
            }
        }
    }

    public static void Postfix(Building_Turret __instance, ref IEnumerable<Gizmo> __result)
    {
        var positionGameComponent = TurretIdlePosition.turretIdlePositionGameComponent;
        if (positionGameComponent == null)
        {
            return;
        }

        if (!__instance.IsEverThreat)
        {
            return;
        }

        if (__instance.GetComp<CompInteractable>() != null)
        {
            return;
        }

        if (__instance.Faction is not { IsPlayer: true })
        {
            return;
        }

        var icon = TurretIdlePosition.OffIcon;
        string defaultDesc = "TIP.SetLimitOff".Translate();
        if (positionGameComponent.TryGetTurretIdlePosition(__instance, out var values))
        {
            icon = TurretIdlePosition.OnIcon;
            defaultDesc = "TIP.SetLimitOn".Translate(values.Item1, values.Item2);
        }

        __result = __result.Append(new Command_Action
        {
            defaultLabel = "TIP.SetLimit".Translate(),
            defaultDesc = defaultDesc,
            icon = icon,
            action = () => Find.WindowStack.Add(new Dialog_TurretIdlePosition(__instance))
        });
    }
}