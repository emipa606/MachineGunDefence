using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace AAA;

public static class AAA_GenLeavingAll
{
    private static readonly List<IntVec3> tmpCellsCandidates = [];

    public static void AAA_DoLeavingsFor(Thing diedThing, Map map, DestroyMode mode)
    {
        AAA_DoLeavingsFor(diedThing, map, mode, diedThing.OccupiedRect());
    }

    private static void AAA_DoLeavingsFor(Thing diedThing, Map map, DestroyMode mode, CellRect leavingsRect)
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
                Log.Warning(
                    $"Failed to place all leavings for destroyed thing {diedThing} at {leavingsRect.CenterCell}");
                return;
            }

            num4++;
            if (num4 >= list2.Count)
            {
                num4 = 0;
            }
        }
    }


    private static bool AAA_CanBuildingLeaveResources(Thing diedThing, DestroyMode mode)
    {
        if (diedThing is not Building)
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
            case DestroyMode.Cancel:
            case DestroyMode.Refund:
                return true;
            default:
                throw new ArgumentException($"Unknown destroy mode {mode}");
        }
    }

    private static Func<int, int> AAA_GetBuildingResourcesLeaveCalculator(Thing diedThing, DestroyMode mode)
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
                throw new ArgumentException($"Unknown destroy mode {mode}");
        }
    }
}