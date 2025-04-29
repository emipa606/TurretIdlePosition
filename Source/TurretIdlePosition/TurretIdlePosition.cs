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
    public static readonly Mesh[] PieOne = new Mesh[361];
    public static readonly Mesh[] PieTwo = new Mesh[361];
    public static readonly Mesh[] PieThree = new Mesh[361];
    public static readonly Mesh[] PieFour = new Mesh[361];
    public static readonly Mesh[] PieFive = new Mesh[361];
    public static TurretIdlePositionGameComponent turretIdlePositionGameComponent;
    public static readonly HashSet<ThingDef> SelfRotatingTurrets = [];

    public static readonly Texture OffIcon = ContentFinder<Texture2D>.Get("UI/LimitOff");
    public static readonly Texture OnIcon = ContentFinder<Texture2D>.Get("UI/LimitOn");

    public static readonly Material arcMaterial =
        SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.6f, 1f, 0.6f, 0.6f));

    public static readonly List<Type> AllPossibleTurrets = [];

    static TurretIdlePosition()
    {
        new Harmony("Mlie.TurretIdlePosition").PatchAll(Assembly.GetExecutingAssembly());
        for (var i = 0; i < 361; i++)
        {
            PieOne[i] = MakePieMesh(i, 1f);
            PieTwo[i] = MakePieMesh(i, 2f);
            PieThree[i] = MakePieMesh(i, 3f);
            PieFour[i] = MakePieMesh(i, 4f);
            PieFive[i] = MakePieMesh(i, 5f);
        }
    }

    public static bool IsAllowedRotation(Building_Turret parentTurret, float currentRotation, float originalRotation,
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
            if (currentRotation > minValue || currentRotation < maxValue ||
                Math.Max(minValue - currentRotation, currentRotation - maxValue) >
                Math.Max(minValue - originalRotation, originalRotation - maxValue))
            {
                return true;
            }
        }
        else
        {
            if (currentRotation > minValue && currentRotation < maxValue || Math.Min(
                    (currentRotation - minValue + FullCircle) % FullCircle,
                    (maxValue - currentRotation + FullCircle) % FullCircle) < Math.Min(
                    (originalRotation - minValue + FullCircle) % FullCircle,
                    (maxValue - originalRotation + FullCircle) % FullCircle))
            {
                return true;
            }
        }

        return false;
    }

    public static Mesh[] GetMeshFromTurret(Building turret)
    {
        switch (turret.def.size.x)
        {
            case <= 1:
                return PieOne;
            case <= 2:
                return PieTwo;
            case <= 3:
                return PieThree;
            case <= 4:
                return PieFour;
            default:
                return PieFive;
        }
    }

    public static Mesh MakePieMesh(int degreesWide, float length)
    {
        var VertsList = new List<Vector2> { new Vector2(0f, 0f) };
        for (var d = 0; d < degreesWide; d++)
        {
            var angle = d / 180f * Mathf.PI;
            var result = new Vector2(0f, 0f)
            {
                x = (float)(length * 0.550000011920929 * Math.Cos(angle)),
                y = (float)(length * 0.550000011920929 * Math.Sin(angle))
            };
            VertsList.Add(result);
        }

        var Verts3D = new Vector3[VertsList.Count];
        for (var i = 0; i < Verts3D.Length; i++)
        {
            Verts3D[i] = new Vector3(VertsList[i].x, 0f, VertsList[i].y);
        }

        var tr = new Triangulator(VertsList.ToArray());
        var indices = tr.Triangulate();
        var mesh = new Mesh
        {
            name = "MakePieMesh()",
            vertices = Verts3D,
            uv = new Vector2[VertsList.Count],
            triangles = indices
        };
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }
}