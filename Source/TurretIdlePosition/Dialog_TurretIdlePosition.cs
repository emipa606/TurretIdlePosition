using RimWorld;
using UnityEngine;
using Verse;

namespace TurretIdlePosition;

public class Dialog_TurretIdlePosition : Window
{
    private readonly Building_Turret buildingTurret;

    public Dialog_TurretIdlePosition(Building_Turret turret)
    {
        buildingTurret = turret;
        closeOnClickedOutside = true;
        absorbInputAroundWindow = true;
    }

    public override Vector2 InitialSize => new Vector2(300, 150);

    public override void DoWindowContents(Rect inRect)
    {
        var positionGameComponent = TurretIdlePosition.turretIdlePositionGameComponent;
        if (positionGameComponent == null)
        {
            return;
        }

        var limited = positionGameComponent.TryGetTurretIdlePosition(buildingTurret, out var tuple);
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
            var rotation = Widgets.HorizontalSlider(listing_Standard.GetRect(20), tuple.Item1, 0, 359f, false, null,
                null, null, 1);
            listing_Standard.Label("TIP.Deviation".Translate(tuple.Item2));
            var deviation = Widgets.HorizontalSlider(listing_Standard.GetRect(20), tuple.Item2, 0, 179f, false, null,
                null, null, 1);
            positionGameComponent.AddTurretIdlePosition(buildingTurret, rotation, deviation);
        }
        else
        {
            positionGameComponent.RemoveTurretIdlePosition(buildingTurret);
        }

        listing_Standard.End();
    }
}