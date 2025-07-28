using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace TurretIdlePosition;

public class TurretIdlePositionGameComponent : GameComponent
{
    private Rot4 originalShipRotation;
    private Dictionary<Building_Turret, string> turretIdlePositions = new();
    private List<Building_Turret> turretIdlePositionsKeys = [];
    private List<string> turretIdlePositionsValues = [];

    public TurretIdlePositionGameComponent(Game game)
    {
        TurretIdlePosition.TurretIdlePositionGameComponent = this;
    }

    public void AddTurretIdlePosition(Building_Turret turret, float rotation, float deviation)
    {
        turretIdlePositions[turret] = $"{rotation}|{deviation}";
    }

    public void RemoveTurretIdlePosition(Building_Turret turret)
    {
        turretIdlePositions.Remove(turret);
    }

    public void ThingPreMapSwap()
    {
        originalShipRotation = Find.CurrentGravship.Rotation;
    }

    public void ThingPostMapSwap()
    {
        var newShipRotation = Find.CurrentGravship.Rotation;
        if (originalShipRotation == newShipRotation)
        {
            return;
        }


        foreach (var thing in Find.CurrentGravship.Things)
        {
            if (thing is not Building_Turret turret)
            {
                continue;
            }

            if (!turretIdlePositions.TryGetValue(turret, out var valueString))
            {
                return;
            }

            var turretRotation = float.Parse(valueString.Split('|')[0]);
            var turretDeviation = float.Parse(valueString.Split('|')[1]);
            var rotationDifference = newShipRotation.AsAngle - originalShipRotation.AsAngle;
            turretRotation = (turretRotation + rotationDifference) % 360;
            if (turretRotation < 0)
            {
                turretRotation += 360;
            }

            turretIdlePositions[turret] = $"{turretRotation}|{turretDeviation}";
        }
    }

    public bool TryGetTurretIdlePosition(Building_Turret turret, out Tuple<float, float> values)
    {
        values = null;
        if (!turretIdlePositions.ContainsKey(turret))
        {
            return false;
        }

        var valueString = turretIdlePositions.GetValueOrDefault(turret);
        values = new Tuple<float, float>(float.Parse(valueString.Split('|')[0]),
            float.Parse(valueString.Split('|')[1]));
        return true;
    }

    public override void ExposeData()
    {
        base.ExposeData();

        if (Scribe.mode == LoadSaveMode.Saving)
        {
            var newDict = new Dictionary<Building_Turret, string>();
            foreach (var turretIdlePosition in turretIdlePositions)
            {
                if (turretIdlePosition.Key == null || turretIdlePosition.Key.Discarded)
                {
                    continue;
                }

                newDict[turretIdlePosition.Key] = turretIdlePosition.Value;
            }

            turretIdlePositions = newDict;
        }

        Scribe_Collections.Look(ref turretIdlePositions, "turretIdlePositions",
            LookMode.Reference, LookMode.Value, ref turretIdlePositionsKeys, ref turretIdlePositionsValues);
    }
}