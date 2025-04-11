using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace TurretIdlePosition;

[StaticConstructorOnStartup]
public static class TurretIdlePosition
{
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
}