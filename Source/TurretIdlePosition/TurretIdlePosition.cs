using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace TurretIdlePosition;

[StaticConstructorOnStartup]
public static class TurretIdlePosition
{
    public const float FullCircle = 360f;
    public static TurretIdlePositionGameComponent turretIdlePositionGameComponent;

    public static readonly Texture OffIcon = ContentFinder<Texture2D>.Get("UI/LimitOff");
    public static readonly Texture OnIcon = ContentFinder<Texture2D>.Get("UI/LimitOn");

    public static readonly Material arcMaterial =
        SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.6f, 1f, 0.6f, 0.6f));

    public static readonly List<Type> AllPossibleTurrets = [];

    static TurretIdlePosition()
    {
        new Harmony("Mlie.TurretIdlePosition").PatchAll(Assembly.GetExecutingAssembly());
    }

    public static bool IsAllowedRotation(Building_Turret parentTurret, TurretTop turretTop, float originalRotation,
        out float minValue, out float maxValue)
    {
        minValue = 0;
        maxValue = 0;
        if (turretIdlePositionGameComponent == null)
        {
            return true;
        }

        if (!turretIdlePositionGameComponent.TryGetTurretIdlePosition(parentTurret, out var tuple))
        {
            return true;
        }


        minValue = (tuple.Item1 - tuple.Item2 + FullCircle) % FullCircle;
        maxValue = (tuple.Item1 + tuple.Item2) % FullCircle;

        if (minValue > maxValue)
        {
            if (turretTop.CurRotation > minValue || turretTop.CurRotation < maxValue ||
                Math.Max(minValue - turretTop.CurRotation, turretTop.CurRotation - maxValue) >
                Math.Max(minValue - originalRotation, originalRotation - maxValue))
            {
                return true;
            }
        }
        else
        {
            if (turretTop.CurRotation > minValue && turretTop.CurRotation < maxValue || Math.Min(
                    (turretTop.CurRotation - minValue + FullCircle) % FullCircle,
                    (maxValue - turretTop.CurRotation + FullCircle) % FullCircle) < Math.Min(
                    (originalRotation - minValue + FullCircle) % FullCircle,
                    (maxValue - originalRotation + FullCircle) % FullCircle))
            {
                return true;
            }
        }

        return false;
    }
}