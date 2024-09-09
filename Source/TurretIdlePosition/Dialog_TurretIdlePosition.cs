using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace TurretIdlePosition;

public class Dialog_TurretIdlePosition : Window
{
    private readonly Building_Turret buildingTurret;
    private readonly List<Building_Turret> extraTurrets = [];
    private float deviation;
    private bool limited;
    private float rotation;

    public Dialog_TurretIdlePosition(Building_Turret turret)
    {
        buildingTurret = turret;
        closeOnClickedOutside = true;
        absorbInputAroundWindow = true;
        draggable = true;
        var selectedTurrets = Find.Selector.SelectedObjects.Where(selectedObject =>
            TurretIdlePosition.AllPossibleTurrets.Contains(selectedObject.GetType()) &&
            selectedObject != buildingTurret);

        if (selectedTurrets.Any() == false)
        {
            return;
        }

        foreach (var selectedTurret in selectedTurrets)
        {
            if (selectedTurret is not Building_Turret turretBuilding)
            {
                continue;
            }

            extraTurrets.Add(turretBuilding);
        }
    }

    public override Vector2 InitialSize => new Vector2(300, 150);

    public override void DoWindowContents(Rect inRect)
    {
        var positionGameComponent = TurretIdlePosition.turretIdlePositionGameComponent;
        if (positionGameComponent == null)
        {
            return;
        }

        limited = positionGameComponent.TryGetTurretIdlePosition(buildingTurret, out var tuple);
        var wasLimited = limited;
        var listing_Standard = new Listing_Standard();
        listing_Standard.Begin(inRect);
        listing_Standard.CheckboxLabeled("TIP.LimitArc".Translate(), ref limited);

        if (limited != wasLimited && limited)
        {
            positionGameComponent.AddTurretIdlePosition(buildingTurret, 0, 0);
            listing_Standard.End();
            return;
        }

        if (limited)
        {
            listing_Standard.Label("TIP.Rotation".Translate(tuple.Item1));
            rotation = Widgets.HorizontalSlider(listing_Standard.GetRect(20), tuple.Item1, 0, 359f, false, null,
                null, null, 1);
            listing_Standard.Label("TIP.Deviation".Translate(tuple.Item2));
            deviation = Widgets.HorizontalSlider(listing_Standard.GetRect(20), tuple.Item2, 0, 179f, false, null,
                null, null, 1);
            positionGameComponent.AddTurretIdlePosition(buildingTurret, rotation, deviation);
            if (extraTurrets.Any())
            {
                foreach (var extraTurret in extraTurrets)
                {
                    positionGameComponent.AddTurretIdlePosition(extraTurret, rotation, deviation);
                }
            }
        }
        else
        {
            positionGameComponent.RemoveTurretIdlePosition(buildingTurret);
            if (extraTurrets.Any())
            {
                foreach (var extraTurret in extraTurrets)
                {
                    positionGameComponent.RemoveTurretIdlePosition(extraTurret);
                }
            }
        }

        listing_Standard.End();
    }


    public override void WindowUpdate()
    {
        base.WindowUpdate();
        if (!limited)
        {
            return;
        }

        var degreesWide = (int)Math.Max(deviation, 5);
        var center = buildingTurret.DrawPos + (Quaternion.AngleAxis(rotation, Vector3.up) * Vector3.forward * 0.8f);
        Graphics.DrawMesh(MeshPool.pies[degreesWide], center,
            Quaternion.AngleAxis(rotation + ((float)degreesWide / 2) - 90f, Vector3.up), TurretIdlePosition.arcMaterial,
            0);
        if (!extraTurrets.Any())
        {
            return;
        }

        foreach (var extraTurret in extraTurrets)
        {
            center = extraTurret.DrawPos + (Quaternion.AngleAxis(rotation, Vector3.up) * Vector3.forward * 0.8f);
            Graphics.DrawMesh(MeshPool.pies[degreesWide], center,
                Quaternion.AngleAxis(rotation + ((float)degreesWide / 2) - 90f, Vector3.up),
                TurretIdlePosition.arcMaterial,
                0);
        }
    }
}