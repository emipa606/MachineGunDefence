using UnityEngine;
using Verse;

namespace AAA
{
    // Token: 0x0200000B RID: 11
    public class CompProperties_TurretTopSize : CompProperties
    {
        // Token: 0x04000013 RID: 19
        public Vector3 topSize = Vector3.one;

        // Token: 0x0600001E RID: 30 RVA: 0x0000259D File Offset: 0x0000079D
        public CompProperties_TurretTopSize()
        {
            compClass = typeof(CompTurretTopSize);
        }
    }
}