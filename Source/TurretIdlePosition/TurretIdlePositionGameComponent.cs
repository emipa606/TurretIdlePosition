using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace TurretIdlePosition;

public class TurretIdlePositionGameComponent : GameComponent
{
    private Dictionary<Building_Turret, string> turretIdlePositions = new Dictionary<Building_Turret, string>();

    private List<Building_Turret> turretIdlePositionsKeys = [];
    private List<string> turretIdlePositionsValues = [];

    public TurretIdlePositionGameComponent(Game game)
    {
        TurretIdlePosition.turretIdlePositionGameComponent = this;
    }

    public void AddTurretIdlePosition(Building_Turret turret, float rotation, float deviation)
    {
        turretIdlePositions[turret] = $"{rotation}|{deviation}";
    }

    public void RemoveTurretIdlePosition(Building_Turret turret)
    {
        turretIdlePositions.Remove(turret);
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