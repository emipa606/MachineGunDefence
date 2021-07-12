using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace AAA
{
    public static class AAA_GenLeavingAll
    {
        // Token: 0x04002064 RID: 8292
        private const float LeaveFraction_Kill = 1.0f;

        // Token: 0x04002065 RID: 8293
        private const float LeaveFraction_Cancel = 1.0f;

        // Token: 0x04002066 RID: 8294
        public const float LeaveFraction_DeconstructDefault = 1.0f;

        // Token: 0x04002067 RID: 8295
        private const float LeaveFraction_FailConstruction = 1.0f;

        // Token: 0x04002068 RID: 8296
        private static readonly List<IntVec3> tmpCellsCandidates = new List<IntVec3>();

        // Token: 0x06003120 RID: 12576 RVA: 0x0016FD37 File Offset: 0x0016E137
        public static void AAA_DoLeavingsFor(Thing diedThing, Map map, DestroyMode mode)
        {
            AAA_DoLeavingsFor(diedThing, map, mode, diedThing.OccupiedRect());
        }

        // Token: 0x06003121 RID: 12577 RVA: 0x0016FD48 File Offset: 0x0016E148
        public static void AAA_DoLeavingsFor(Thing diedThing, Map map, DestroyMode mode, CellRect leavingsRect)
        {
            if (Current.ProgramState != ProgramState.Playing && mode != DestroyMode.Refund ||
                mode == DestroyMode.Vanish)
            {
                return;
            }

            var thingOwner = new ThingOwner<Thing>();
            if (mode == DestroyMode.KillFinalize && diedThing.def.killedLeavings != null)
            {
                foreach (var thingDefCountClass in diedThing.def.killedLeavings)
                {
                    var thing = ThingMaker.MakeThing(thingDefCountClass.thingDef);
                    thing.stackCount = thingDefCountClass.count;
                    thingOwner.TryAdd(thing);
                }
            }

            if (AAA_CanBuildingLeaveResources(diedThing, mode))
            {
                if (diedThing is Frame frame)
                {
                    for (var l = frame.resourceContainer.Count - 1; l >= 0; l--)
                    {
                        var num = AAA_GetBuildingResourcesLeaveCalculator(diedThing, mode)(frame.resourceContainer[l]
                            .stackCount);
                        if (num > 0)
                        {
                            frame.resourceContainer.TryTransferToContainer(frame.resourceContainer[l], thingOwner, num);
                        }
                    }

                    frame.resourceContainer.ClearAndDestroyContents();
                }
                else
                {
                    var list = diedThing.AAA_CostListAdjusted();

                    foreach (var thingCountClass in list)
                    {
                        var num2 = AAA_GetBuildingResourcesLeaveCalculator(diedThing, mode)(thingCountClass.count);
                        if (num2 > 0 && mode == DestroyMode.KillFinalize && thingCountClass.thingDef.slagDef != null)
                        {
                            var count = thingCountClass.thingDef.slagDef.smeltProducts
                                .First(pro => pro.thingDef == ThingDefOf.Steel).count;
                            var num3 = num2 / 2 / 8;
                            for (var n = 0; n < num3; n++)
                            {
                                thingOwner.TryAdd(ThingMaker.MakeThing(thingCountClass.thingDef.slagDef));
                            }

                            num2 -= num3 * count;
                        }

                        if (num2 <= 0)
                        {
                            continue;
                        }

                        var thing2 = ThingMaker.MakeThing(thingCountClass.thingDef);
                        thing2.stackCount = num2;
                        thingOwner.TryAdd(thing2);
                    }
                }
            }

            var list2 = leavingsRect.Cells.InRandomOrder().ToList();
            var num4 = 0;
            while (thingOwner.Count > 0)
            {
                if (mode == DestroyMode.KillFinalize && !map.areaManager.Home[list2[num4]])
                {
                    // thingOwner[0].SetForbidden(true, false);
                }

                if (!thingOwner.TryDrop(thingOwner[0], list2[num4], map, ThingPlaceMode.Near, out _))
                {
                    Log.Warning(string.Concat("Failed to place all leavings for destroyed thing ", diedThing, " at ",
                        leavingsRect.CenterCell));
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
            if (!AAA_CanBuildingLeaveResources(diedThing, mode))
            {
                return _ => 0;
            }

            switch (mode)
            {
                case DestroyMode.Vanish:
                    return _ => 0;
                case DestroyMode.KillFinalize:
                    return count => GenMath.RoundRandom(count /* 0.5f*/);
                case DestroyMode.Deconstruct:
                    return count => GenMath.RoundRandom(count /* * diedThing.def.resourcesFractionWhenDeconstructed*/);
                case DestroyMode.FailConstruction:
                    return count => GenMath.RoundRandom(count /* 0.5f*/);
                case DestroyMode.Cancel:
                    return count => GenMath.RoundRandom(count * 1f);
                case DestroyMode.Refund:
                    return count => count;
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

            var cellRect = t.OccupiedRect().ExpandedBy(1);
            tmpCellsCandidates.Clear();
            foreach (var intVec in cellRect)
            {
                if (intVec.InBounds(t.Map) && intVec.Walkable(t.Map))
                {
                    tmpCellsCandidates.Add(intVec);
                }
            }

            if (!tmpCellsCandidates.Any())
            {
                return;
            }

            var num = GenMath.RoundRandom(damageDealt * Mathf.Min(0.0166666675f, 1f / (t.MaxHitPoints / 10f)));
            for (var i = 0; i < num; i++)
            {
                FilthMaker.TryMakeFilth(tmpCellsCandidates.RandomElement(), t.Map, t.def.filthLeaving);
            }
        }
    }

    // Token: 0x02000882 RID: 2178
}