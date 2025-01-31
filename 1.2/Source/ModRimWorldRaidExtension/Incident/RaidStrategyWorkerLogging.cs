﻿// ******************************************************************
//       /\ /|       @file       RaidStrategyWorkerLogging.cs
//       \ V/        @brief      袭击策略 伐木
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-17 19:05:27
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using System.Collections.Generic;
using JetBrains.Annotations;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace SR.ModRimWorld.RaidExtension
{
    [UsedImplicitly]
    public class RaidStrategyWorkerLogging : RaidStrategyWorker
    {
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
            return new LordJobLogging(siegePositionFrom);
        }
    }
}