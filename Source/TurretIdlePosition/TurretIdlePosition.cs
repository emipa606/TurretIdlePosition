using System;
using System.Collections.Generic;
using System.Linq;
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
    private static readonly Mesh[] pieOne = new Mesh[361];
    private static readonly Mesh[] pieTwo = new Mesh[361];
    private static readonly Mesh[] pieThree = new Mesh[361];
    private static readonly Mesh[] pieFour = new Mesh[361];
    private static readonly Mesh[] pieFive = new Mesh[361];
    public static TurretIdlePositionGameComponent TurretIdlePositionGameComponent;
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
            pieOne[i] = makePieMesh(i, 1f);
            pieTwo[i] = makePieMesh(i, 2f);
            pieThree[i] = makePieMesh(i, 3f);
            pieFour[i] = makePieMesh(i, 4f);
            pieFive[i] = makePieMesh(i, 5f);
        }
    }

    public static bool IsAllowedRotation(Building_Turret parentTurret, float currentRotation, float originalRotation,
        out float minValue, out float maxValue)
    {
        minValue = 0;
        maxValue = 0;
        if (TurretIdlePositionGameComponent == null)
        {
            return true;
        }

        if (!TurretIdlePositionGameComponent.TryGetTurretIdlePosition(parentTurret, out var tuple))
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
                return pieOne;
            case <= 2:
                return pieTwo;
            case <= 3:
                return pieThree;
            case <= 4:
                return pieFour;
            default:
                return pieFive;
        }
    }

    private static Mesh makePieMesh(int degreesWide, float length)
    {
        var vertsList = new List<Vector2> { new(0f, 0f) };
        for (var d = 0; d < degreesWide; d++)
        {
            var angle = d / 180f * Mathf.PI;
            var result = new Vector2(0f, 0f)
            {
                x = (float)(length * 0.550000011920929 * Math.Cos(angle)),
                y = (float)(length * 0.550000011920929 * Math.Sin(angle))
            };
            vertsList.Add(result);
        }

        var verts3D = new Vector3[vertsList.Count];
        for (var i = 0; i < verts3D.Length; i++)
        {
            verts3D[i] = new Vector3(vertsList[i].x, 0f, vertsList[i].y);
        }

        var tr = new Triangulator(verts3D.ToList());
        var indices = tr.Triangulate();
        var mesh = new Mesh
        {
            name = "MakePieMesh()",
            vertices = verts3D,
            uv = new Vector2[vertsList.Count],
            triangles = indices.ToArray()
        };
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }
}