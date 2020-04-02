using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
namespace AAA
{
    public static class AAA_GenLeavingAll
    {
        // Token: 0x06003120 RID: 12576 RVA: 0x0016FD37 File Offset: 0x0016E137
        public static void AAA_DoLeavingsFor(Thing diedThing, Map map, DestroyMode mode)
        {
            AAA_GenLeavingAll.AAA_DoLeavingsFor(diedThing, map, mode, diedThing.OccupiedRect());
        }

        // Token: 0x06003121 RID: 12577 RVA: 0x0016FD48 File Offset: 0x0016E148
        public static void AAA_DoLeavingsFor(Thing diedThing, Map map, DestroyMode mode, CellRect leavingsRect)
        {
            if ((Current.ProgramState != ProgramState.Playing && mode != DestroyMode.Refund) || mode == DestroyMode.Vanish)
            {
                return;
            }
           
            ThingOwner<Thing> thingOwner = new ThingOwner<Thing>();
            if (mode == DestroyMode.KillFinalize && diedThing.def.killedLeavings != null)
            {
                for (int k = 0; k < diedThing.def.killedLeavings.Count; k++)
                {
                    Thing thing = ThingMaker.MakeThing(diedThing.def.killedLeavings[k].thingDef, null);
                    thing.stackCount = diedThing.def.killedLeavings[k].count;
                    thingOwner.TryAdd(thing, true);
                }
            }
            if (AAA_GenLeavingAll.AAA_CanBuildingLeaveResources(diedThing, mode))
            {
                RimWorld.Frame frame = diedThing as RimWorld.Frame;
                if (frame != null)
                {
                    for (int l = frame.resourceContainer.Count - 1; l >= 0; l--)
                    {
                        int num = AAA_GenLeavingAll.AAA_GetBuildingResourcesLeaveCalculator(diedThing, mode)(frame.resourceContainer[l].stackCount);
                        if (num > 0)
                        {
                            frame.resourceContainer.TryTransferToContainer(frame.resourceContainer[l], thingOwner, num, true);
                        }
                    }
                    frame.resourceContainer.ClearAndDestroyContents(DestroyMode.Vanish);
                }
                else
                {
                    List<ThingDefCountClass> list = diedThing.AAA_CostListAdjusted();
                  
                    for (int m = 0; m < list.Count; m++)
                    {
                        ThingDefCountClass thingCountClass = list[m];
                        int num2 = AAA_GenLeavingAll.AAA_GetBuildingResourcesLeaveCalculator(diedThing, mode)(thingCountClass.count);
                        if (num2 > 0 && mode == DestroyMode.KillFinalize && thingCountClass.thingDef.slagDef != null)
                        {
                            int count = thingCountClass.thingDef.slagDef.smeltProducts.First((ThingDefCountClass pro) => pro.thingDef == RimWorld.ThingDefOf.Steel).count;
                            int num3 = num2 / 2 / 8;
                            for (int n = 0; n < num3; n++)
                            {
                               thingOwner.TryAdd(ThingMaker.MakeThing(thingCountClass.thingDef.slagDef, null), true);
                            }
                            num2 -= num3 * count;
                        }
                        if (num2 > 0)
                        {
                          
                                Thing thing2 = ThingMaker.MakeThing(thingCountClass.thingDef, null);
                               thing2.stackCount = num2;
                                thingOwner.TryAdd(thing2, true);
                        }
                    }
                }
            }
            List<IntVec3> list2 = leavingsRect.Cells.InRandomOrder(null).ToList<IntVec3>();
            int num4 = 0;
            while (thingOwner.Count > 0)
            {
                if (mode == DestroyMode.KillFinalize && !map.areaManager.Home[list2[num4]])
                {
                   // thingOwner[0].SetForbidden(true, false);
                }
                Thing thing3;
                if (!thingOwner.TryDrop(thingOwner[0], list2[num4], map, ThingPlaceMode.Near, out thing3, null))
                {
                    Log.Warning(string.Concat(new object[]
                    {
                        "Failed to place all leavings for destroyed thing ",
                        diedThing,
                        " at ",
                        leavingsRect.CenterCell
                    }));
                    return;
                }
                num4++;
                if (num4 >= list2.Count)
                {
                    num4 = 0;
                }
            }
        }

      

        // Token: 0x06003123 RID: 12579 RVA: 0x001701B4 File Offset: 0x0016E5B4
        public static bool AAA_CanBuildingLeaveResources(Thing diedThing, DestroyMode mode)
        {
            if (!(diedThing is Building))
            {
                return false;
            }
            if (mode == DestroyMode.KillFinalize && !diedThing.def.leaveResourcesWhenKilled)
            {
                return false;
            }
            switch (mode)
            {
                case DestroyMode.Vanish:
                    return false;
                case DestroyMode.KillFinalize:
                    return true;
                case DestroyMode.Deconstruct:
                    return diedThing.def.resourcesFractionWhenDeconstructed != 0f;
                case DestroyMode.FailConstruction:
                    return true;
                case DestroyMode.Cancel:
                    return true;
                case DestroyMode.Refund:
                    return true;
                default:
                    throw new ArgumentException("Unknown destroy mode " + mode);
            }
        }

        // Token: 0x06003124 RID: 12580 RVA: 0x00170240 File Offset: 0x0016E640
        public static Func<int, int> AAA_GetBuildingResourcesLeaveCalculator(Thing diedThing, DestroyMode mode)
        {
            if (!AAA_GenLeavingAll.AAA_CanBuildingLeaveResources(diedThing, mode))
            {
                return (int count) => 0;
            }
            switch (mode)
            {
                case DestroyMode.Vanish:
                    return (int count) => 0;
                case DestroyMode.KillFinalize:
                    return (int count) => GenMath.RoundRandom((float)count /* 0.5f*/);
                case DestroyMode.Deconstruct:
                    return (int count) => GenMath.RoundRandom((float)count /** diedThing.def.resourcesFractionWhenDeconstructed*/);
                case DestroyMode.FailConstruction:
                    return (int count) => GenMath.RoundRandom((float)count /* 0.5f*/);
                case DestroyMode.Cancel:
                    return (int count) => GenMath.RoundRandom((float)count * 1f);
                case DestroyMode.Refund:
                    return (int count) => count;
                default:
                    throw new ArgumentException("Unknown destroy mode " + mode);
            }
        }

        // Token: 0x06003125 RID: 12581 RVA: 0x00170364 File Offset: 0x0016E764
        public static void DropFilthDueToDamage(Thing t, float damageDealt)
        {
            if (!t.def.useHitPoints || !t.Spawned || t.def.filthLeaving == null)
            {
                return;
            }
            CellRect cellRect = t.OccupiedRect().ExpandedBy(1);
            AAA_GenLeavingAll.tmpCellsCandidates.Clear();
            foreach (IntVec3 intVec in cellRect)
            {
                if (intVec.InBounds(t.Map) && intVec.Walkable(t.Map))
                {
                    AAA_GenLeavingAll.tmpCellsCandidates.Add(intVec);
                }
            }
            if (!AAA_GenLeavingAll.tmpCellsCandidates.Any<IntVec3>())
            {
                return;
            }
            int num = GenMath.RoundRandom(damageDealt * Mathf.Min(0.0166666675f, 1f / ((float)t.MaxHitPoints / 10f)));
            for (int i = 0; i < num; i++)
            {
                RimWorld.FilthMaker.TryMakeFilth(AAA_GenLeavingAll.tmpCellsCandidates.RandomElement<IntVec3>(), t.Map, t.def.filthLeaving, 1);
            }
        }

        // Token: 0x04002064 RID: 8292
        private const float LeaveFraction_Kill = 1.0f;

        // Token: 0x04002065 RID: 8293
        private const float LeaveFraction_Cancel = 1.0f;

        // Token: 0x04002066 RID: 8294
        public const float LeaveFraction_DeconstructDefault = 1.0f;

        // Token: 0x04002067 RID: 8295
        private const float LeaveFraction_FailConstruction = 1.0f;

        // Token: 0x04002068 RID: 8296
        private static List<IntVec3> tmpCellsCandidates = new List<IntVec3>();
    }
    // Token: 0x02000882 RID: 2178











    public static class AAA_CostListCalculator
    {
        // Token: 0x06003064 RID: 12388 RVA: 0x0016C45D File Offset: 0x0016A85D
        public static void Reset()
        {
            AAA_CostListCalculator.cachedCosts.Clear();
        }

        // Token: 0x06003065 RID: 12389 RVA: 0x0016C469 File Offset: 0x0016A869
        public static List<ThingDefCountClass> AAA_CostListAdjusted(this Thing thing)
        {
            return thing.def.AAA_CostListAdjusted(thing.Stuff, true);
        }

        // Token: 0x06003066 RID: 12390 RVA: 0x0016C480 File Offset: 0x0016A880
        public static List<ThingDefCountClass> AAA_CostListAdjusted(this BuildableDef entDef, ThingDef stuff, bool errorOnNullStuff = true)
        {
            AAA_CostListCalculator.CostListPair key = new AAA_CostListCalculator.CostListPair(entDef, stuff);
            List<ThingDefCountClass> list;
            if (!AAA_CostListCalculator.cachedCosts.TryGetValue(key, out list))
            {
                list = new List<ThingDefCountClass>();
                int num = 0;
                if (entDef.MadeFromStuff)
                {
                    if (errorOnNullStuff && stuff == null)
                    {
                        Log.Error("Cannot get AdjustedCostList for " + entDef + " with null Stuff.");
                        return null;
                    }
                    if (stuff != null)
                    {
                        num = Mathf.RoundToInt((float)entDef.costStuffCount / stuff.VolumePerUnit);
                        if (num < 1)
                        {
                            num = 1;
                        }
                    }
                    else
                    {
                        num = entDef.costStuffCount;
                    }
                }
                else if (stuff != null)
                {
                    Log.Error(string.Concat(new object[]
                    {
                        "Got AdjustedCostList for ",
                        entDef,
                        " with stuff ",
                        stuff,
                        " but is not MadeFromStuff."
                    }));
                }
                bool flag = false;
                if (entDef.costList != null)
                {
                    for (int i = 0; i < entDef.costList.Count; i++)
                    {
                        ThingDefCountClass thingDefCountClass = entDef.costList[i];

                        if (thingDefCountClass.thingDef == stuff)
                        {//-----------------stuff---------------------------------
                            list.Add(new ThingDefCountClass(thingDefCountClass.thingDef, thingDefCountClass.count + num));
                            flag = true;
                        }
                        else
                        {
                            float count2 = thingDefCountClass.count;
                            //--------------------------------------------------------
                            //             leave  one crate
                            //----------------------------------------------------------
                            thingDefCountClass.count = GenMath.RoundRandom(count2);
                            list.Add(thingDefCountClass);
                        }
                    }
                }
                if (!flag && num > 0)
                {
                    list.Add(new ThingDefCountClass(stuff, num));
                }
                AAA_CostListCalculator.cachedCosts.Add(key, list);
            }
            return list;
        }

        // Token: 0x04001AB8 RID: 6840
        private static Dictionary<AAA_CostListCalculator.CostListPair, List<ThingDefCountClass>> cachedCosts = new Dictionary<AAA_CostListCalculator.CostListPair, List<ThingDefCountClass>>(AAA_CostListCalculator.FastCostListPairComparer.Instance);

        // Token: 0x02000883 RID: 2179
        private struct CostListPair : IEquatable<AAA_CostListCalculator.CostListPair>
        {
            // Token: 0x06003068 RID: 12392 RVA: 0x0016C5F3 File Offset: 0x0016A9F3
            public CostListPair(BuildableDef buildable, ThingDef stuff)
            {
                this.buildable = buildable;
                this.stuff = stuff;
            }

            // Token: 0x06003069 RID: 12393 RVA: 0x0016C604 File Offset: 0x0016AA04
            public override int GetHashCode()
            {
                int seed = 0;
                seed = Gen.HashCombine<BuildableDef>(seed, this.buildable);
                return Gen.HashCombine<ThingDef>(seed, this.stuff);
            }

            // Token: 0x0600306A RID: 12394 RVA: 0x0016C62E File Offset: 0x0016AA2E
            public override bool Equals(object obj)
            {
                return obj is AAA_CostListCalculator.CostListPair && this.Equals((AAA_CostListCalculator.CostListPair)obj);
            }

            // Token: 0x0600306B RID: 12395 RVA: 0x0016C649 File Offset: 0x0016AA49
            public bool Equals(AAA_CostListCalculator.CostListPair other)
            {
                return this == other;
            }

            // Token: 0x0600306C RID: 12396 RVA: 0x0016C657 File Offset: 0x0016AA57
            public static bool operator ==(AAA_CostListCalculator.CostListPair lhs, AAA_CostListCalculator.CostListPair rhs)
            {
                return lhs.buildable == rhs.buildable && lhs.stuff == rhs.stuff;
            }

            // Token: 0x0600306D RID: 12397 RVA: 0x0016C67F File Offset: 0x0016AA7F
            public static bool operator !=(AAA_CostListCalculator.CostListPair lhs, AAA_CostListCalculator.CostListPair rhs)
            {
                return !(lhs == rhs);
            }

            // Token: 0x04001AB9 RID: 6841
            public BuildableDef buildable;

            // Token: 0x04001ABA RID: 6842
            public ThingDef stuff;
        }

        // Token: 0x02000884 RID: 2180
        private class FastCostListPairComparer : IEqualityComparer<AAA_CostListCalculator.CostListPair>
        {
            // Token: 0x0600306F RID: 12399 RVA: 0x0016C693 File Offset: 0x0016AA93
            public bool Equals(AAA_CostListCalculator.CostListPair x, AAA_CostListCalculator.CostListPair y)
            {
                return x == y;
            }

            // Token: 0x06003070 RID: 12400 RVA: 0x0016C69C File Offset: 0x0016AA9C
            public int GetHashCode(AAA_CostListCalculator.CostListPair obj)
            {
                return obj.GetHashCode();
            }

            // Token: 0x04001ABB RID: 6843
            public static readonly AAA_CostListCalculator.FastCostListPairComparer Instance = new AAA_CostListCalculator.FastCostListPairComparer();
        }
    }
}
