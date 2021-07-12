using Verse; // Always needed
//using VerseBase;         // Material/Graphics handling functions are found here
// RimWorld universal objects are here (like 'Building')
//using Verse.AI;          // Needed when you do something with the AI
//using Verse.Sound;       // Needed when you do something with Sound
//using Verse.Noise;       // Needed when you do something with Noises
// RimWorld specific functions are found here (like 'Building_Battery')
//using RimWorld.Planet;   // RimWorld specific functions for world creation
//using RimWorld.SquadAI;  // RimWorld specific functions for squad brains 

namespace AAA
{
    internal class CompProperties_ProjectileMultiple : CompProperties
    {
        public float forsedScatterRadius = 0.0f;
        public int pellets = 1;
        public float scatterRadiusAt10tilesAway = 0.0f;


        public CompProperties_ProjectileMultiple()
        {
            compClass = typeof(CompProjectileMultiple);
        }
    }
}