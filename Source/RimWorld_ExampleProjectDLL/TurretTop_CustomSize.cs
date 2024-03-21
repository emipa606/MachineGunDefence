using UnityEngine;
using Verse;
// Always needed
//using VerseBase;         // Material/Graphics handling functions are found here
// RimWorld universal objects are here (like 'Building')
//using Verse.AI;          // Needed when you do something with the AI
//using Verse.Sound;       // Needed when you do something with Sound
//using Verse.Noise;       // Needed when you do something with Noises
// RimWorld specific functions are found here (like 'Building_Battery')
//using RimWorld.Planet;   // RimWorld specific functions for world creation
//using RimWorld.SquadAI;  // RimWorld specific functions for squad brains 

namespace AAA;

public class TurretTop_CustomSize(Building_TurretGunCustom ParentTurret)
{
    private const float IdleTurnDegreesPerTick = 0.26f;

    private const int IdleTurnDuration = 140;

    private const int IdleTurnIntervalMin = 150;

    private const int IdleTurnIntervalMax = 350;

    private float curRotationInt;

    private bool idleTurnClockwise;

    private int idleTurnTicksLeft;

    private int ticksUntilIdleTurn;

    private float CurRotation
    {
        get => curRotationInt;
        set
        {
            curRotationInt = value;
            if (curRotationInt > 360.0)
            {
                curRotationInt -= 360f;
            }

            if (curRotationInt < 0.0)
            {
                curRotationInt += 360f;
            }
        }
    }

    public void TurretTopTick()
    {
        var currentTarget = ParentTurret.CurrentTarget;
        if (currentTarget.IsValid)
        {
            CurRotation = (currentTarget.Cell.ToVector3Shifted() - ParentTurret.DrawPos).AngleFlat();
            ticksUntilIdleTurn = Rand.RangeInclusive(IdleTurnIntervalMin, IdleTurnIntervalMax);
            return;
        }

        if (ticksUntilIdleTurn > 0)
        {
            ticksUntilIdleTurn--;
            if (ticksUntilIdleTurn != 0)
            {
                return;
            }

            idleTurnClockwise = Rand.Value < 0.5;

            idleTurnTicksLeft = IdleTurnDuration;
        }
        else
        {
            if (idleTurnClockwise)
            {
                CurRotation += IdleTurnDegreesPerTick;
            }
            else
            {
                CurRotation -= IdleTurnDegreesPerTick;
            }

            idleTurnTicksLeft--;
            if (idleTurnTicksLeft <= 0)
            {
                ticksUntilIdleTurn = Rand.RangeInclusive(IdleTurnIntervalMin, IdleTurnIntervalMax);
            }
        }
    }

    public void DrawTurret()
    {
        var matrix = default(Matrix4x4);
        matrix.SetTRS(ParentTurret.DrawPos + Altitudes.AltIncVect, CurRotation.ToQuat(),
            ParentTurret.TopSizeComp?.Props.topSize ?? Vector3.one);
        Graphics.DrawMesh(MeshPool.plane20, matrix, ParentTurret.def.building.turretTopMat, 0);
    }
}