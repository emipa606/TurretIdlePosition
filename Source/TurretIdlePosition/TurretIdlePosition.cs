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

    static TurretIdlePosition()
    {
        new Harmony("Mlie.TurretIdlePosition").PatchAll(Assembly.GetExecutingAssembly());
    }
}