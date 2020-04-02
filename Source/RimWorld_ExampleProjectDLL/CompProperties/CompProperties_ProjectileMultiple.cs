using System;
using System.Collections.Generic;
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
    class CompProperties_ProjectileMultiple : CompProperties
    {
        

        public CompProperties_ProjectileMultiple()
        {
            this.compClass = typeof(AAA.CompProjectileMultiple);
        }
        public int pellets = 1;
        public float forsedScatterRadius = 0.0f;
        public float scatterRadiusAt10tilesAway = 0.0f;
    }
}
