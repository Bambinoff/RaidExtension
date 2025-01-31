﻿// ******************************************************************
//       /\ /|       @file       RaidStrategyWorkerPoaching.cs
//       \ V/        @brief      袭击策略 偷猎
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-17 19:01:36
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace SR.ModRimWorld.RaidExtension
{
    [UsedImplicitly]
    public class RaidStrategyWorkerPoaching : RaidStrategyWorker
    {
        public Pawn TempAnimal { get; set; } //目标动物

        /// <summary>
        /// 创建集群AI工作
        /// </summary>
        /// <param name="parms"></param>
        /// <param name="map"></param>
        /// <param name="pawns"></param>
        /// <param name="raidSeed"></param>
        /// <returns></returns>
        protected override LordJob MakeLordJob(IncidentParms parms, Map map, List<Pawn> pawns, int raidSeed)
        {
            var siegePositionFrom =
                RCellFinder.FindSiegePositionFrom_NewTemp(parms.spawnCenter.IsValid ? parms.spawnCenter : pawns[0].PositionHeld,
                    map);
            return new LordJobPoaching(siegePositionFrom, TempAnimal);
        }

        /// <summary>
        /// 生成角色
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        public override List<Pawn> SpawnThreats(IncidentParms parms)
        {
            var pawnGroupMakerParms =
                IncidentParmsUtility.GetDefaultPawnGroupMakerParms(PawnGroupKindDefOf.Combat, parms);
            var pawnList = PawnGroupMakerUtility.GeneratePawns(pawnGroupMakerParms).ToList();
            if (pawnList.Count == 0)
            {
                return pawnList;
            }

            parms.raidArrivalMode.Worker.Arrive(pawnList, parms);
            return pawnList;
        }
    }
}