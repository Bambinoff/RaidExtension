// ******************************************************************
//       /\ /|       @file       PawnExtension.cs
//       \ V/        @brief      角色扩展
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-07-11 12:17:00
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace SR.ModRimWorld.RaidExtension
{
    public static class PawnExtension
    {
        /// <summary>
        /// 寻找最近的 满足体型的动物
        /// </summary>
        /// <param name="leader"></param>
        /// <param name="minTargetRequireHealthScale"></param>
        /// <returns></returns>
        public static Pawn FindTargetAnimal(this Pawn leader, float minTargetRequireHealthScale)
        {
            //验证器
            bool SpoilValidator(Thing t) => (t is Pawn animal)
                                            && animal.RaceProps.Animal && !animal.Dead &&
                                            animal.RaceProps.baseHealthScale >= minTargetRequireHealthScale
                                            && leader.CanReserve(t);

            //找队长身边最近的动物
            var targetThing = GenClosest.ClosestThing_Global_Reachable(leader.Position, leader.Map,
                leader.Map.mapPawns.AllPawnsSpawned, PathEndMode.ClosestTouch,
                TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Some), validator: SpoilValidator);

            return (Pawn)targetThing;
        }

        /// <summary>
        /// 寻找最近的成熟的树
        /// </summary>
        /// <param name="pawn"></param>
        /// <returns></returns>
        public static Thing FindTree(this Pawn pawn)
        {
            //验证器 是植物 可以保留 没有燃烧中 角色不是什么树木爱好者 成熟了
            bool SpoilValidator(Thing t) => t is Plant plant && pawn.CanReserve(plant) && !plant.IsBurning() &&
                                            PlantUtility.PawnWillingToCutPlant_Job(t, pawn) && plant.IsTree();

            //寻找身边最近的成熟的树
            var targetPlant = GenClosest.ClosestThing_Global_Reachable(pawn.Position, pawn.Map,
                pawn.Map.spawnedThings, PathEndMode.ClosestTouch,
                TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Some), validator: SpoilValidator);
            return targetPlant;
        }

        /// <summary>
        /// 寻找战利品
        /// </summary>
        /// <param name="seacher"></param>
        /// <param name="root"></param>
        /// <param name="map"></param>
        /// <param name="maxDist"></param>
        /// <param name="disallowed"></param>
        /// <param name="validator"></param>
        /// <returns></returns>
        public static Thing TryFindBestSpoilsToTake(this Pawn seacher, IntVec3 root, Map map, float maxDist,
            ICollection<Thing> disallowed = null, Predicate<Thing> validator = null)
        {
            if (map == null)
            {
                return null;
            }

            if (seacher != null && !seacher.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
            {
                return null;
            }

            //搜索者存在 但无法触碰地图边界 或者 搜索者不存在
            if (seacher != null && !map.reachability.CanReachMapEdge(seacher.Position,
                TraverseParms.For(seacher)) || (seacher == null && !map.reachability.CanReachMapEdge(root,
                TraverseParms.For(TraverseMode.PassDoors))))
            {
                return null;
            }

            //验证器 搜索者不存在 或者搜索者可以预留当前物体 并且没有禁用 并且物体可以被偷 并且物品周围有敌对派系尸体
            bool SpoilValidator(Thing t) => (seacher == null || seacher.CanReserve(t)) &&
                                            (disallowed == null || !disallowed.Contains(t)) &&
                                            (validator == null || validator(t));


            return GenClosest.ClosestThing_Regionwise_ReachablePrioritized(root, map,
                ThingRequest.ForGroup(ThingRequestGroup.HaulableEverOrMinifiable), PathEndMode.ClosestTouch,
                TraverseParms.For(TraverseMode.NoPassClosedDoors), maxDist, SpoilValidator,
                StealAIUtility.GetValue);
        }
    }
}
