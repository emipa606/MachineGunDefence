using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;         // Always needed
//using VerseBase;         // Material/Graphics handling functions are found here
using Verse;               // RimWorld universal objects are here (like 'Building')
//using Verse.AI;          // Needed when you do something with the AI
//using Verse.Sound;       // Needed when you do something with Sound
//using Verse.Noise;       // Needed when you do something with Noises
using RimWorld;            // RimWorld specific functions are found here (like 'Building_Battery')
//using RimWorld.Planet;   // RimWorld specific functions for world creation
//using RimWorld.SquadAI;  // RimWorld specific functions for squad brains 

namespace AAA
{
    public class TurretTop_CustomSize
    {
        // Token: 0x1700000D RID: 13
        // (get) Token: 0x06000028 RID: 40 RVA: 0x00002816 File Offset: 0x00000A16
        // (set) Token: 0x06000029 RID: 41 RVA: 0x00002820 File Offset: 0x00000A20
        private float CurRotation
        {
            get
            {
                return this.curRotationInt;
            }
            set
            {
                this.curRotationInt = value;
                if ((double)this.curRotationInt > 360.0)
                {
                    this.curRotationInt -= 360f;
                }
                if ((double)this.curRotationInt < 0.0)
                {
                    this.curRotationInt += 360f;
                }
            }
        }

        // Token: 0x0600002A RID: 42 RVA: 0x0000287C File Offset: 0x00000A7C
        public TurretTop_CustomSize(Building_TurretGunCustom ParentTurret)
        {
            this.parentTurret = ParentTurret;
        }

        // Token: 0x0600002B RID: 43 RVA: 0x0000288C File Offset: 0x00000A8C
        public void TurretTopTick()
        {
            LocalTargetInfo currentTarget = this.parentTurret.CurrentTarget;
            if (currentTarget.IsValid)
            {
                this.CurRotation = (currentTarget.Cell.ToVector3Shifted() - this.parentTurret.DrawPos).AngleFlat();
                this.ticksUntilIdleTurn = Rand.RangeInclusive(150, 350);
                return;
            }
            if (this.ticksUntilIdleTurn > 0)
            {
                this.ticksUntilIdleTurn--;
                if (this.ticksUntilIdleTurn == 0)
                {
                    if ((double)Rand.Value < 0.5)
                    {
                        this.idleTurnClockwise = true;
                    }
                    else
                    {
                        this.idleTurnClockwise = false;
                    }
                    this.idleTurnTicksLeft = 140;
                    return;
                }
            }
            else
            {
                if (this.idleTurnClockwise)
                {
                    this.CurRotation += 0.26f;
                }
                else
                {
                    this.CurRotation -= 0.26f;
                }
                this.idleTurnTicksLeft--;
                if (this.idleTurnTicksLeft <= 0)
                {
                    this.ticksUntilIdleTurn = Rand.RangeInclusive(150, 350);
                }
            }
        }

        // Token: 0x0600002C RID: 44 RVA: 0x0000299C File Offset: 0x00000B9C
        public void DrawTurret()
        {
            Matrix4x4 matrix = default(Matrix4x4);
            matrix.SetTRS(this.parentTurret.DrawPos + Altitudes.AltIncVect, this.CurRotation.ToQuat(), (this.parentTurret.TopSizeComp == null) ? Vector3.one : this.parentTurret.TopSizeComp.Props.topSize);
            Graphics.DrawMesh(MeshPool.plane20, matrix, this.parentTurret.def.building.turretTopMat, 0);
        }

        // Token: 0x04000019 RID: 25
        private Building_TurretGunCustom parentTurret;

        // Token: 0x0400001A RID: 26
        private float curRotationInt;

        // Token: 0x0400001B RID: 27
        private int ticksUntilIdleTurn;

        // Token: 0x0400001C RID: 28
        private int idleTurnTicksLeft;

        // Token: 0x0400001D RID: 29
        private bool idleTurnClockwise;

        // Token: 0x0400001E RID: 30
        private const float IdleTurnDegreesPerTick = 0.26f;

        // Token: 0x0400001F RID: 31
        private const int IdleTurnDuration = 140;

        // Token: 0x04000020 RID: 32
        private const int IdleTurnIntervalMin = 150;

        // Token: 0x04000021 RID: 33
        private const int IdleTurnIntervalMax = 350;
        
    }
}
