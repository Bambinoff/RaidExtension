﻿// ******************************************************************
//       /\ /|       @file       IncidentWorkerLogging.cs
//       \ V/        @brief      事件 伐木
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-06-17 19:42:56
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace SR.ModRimWorld.RaidExtension
{
    [UsedImplicitly]
    public class IncidentWorkerLogging : IncidentWorker_RaidEnemy
    {
        private const int MaxThreatPoints = 3000; //袭击点数

        /// <summary>
        /// 是否可以生成事件
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!(parms.target is Map map))
            {
                Log.Error("[SR.ModRimWorld.RaidExtension]target must be a map.");
                return false;
            }

            bool SpoilValidator(Thing t) => t is Plant plant && !plant.IsBurning() && plant.HarvestableNow
                                            && plant.def.plant.IsTree;

            var isTreeExist = map.spawnedThings.Any(SpoilValidator);
            //没有树 无法触发事件
            if (!isTreeExist)
            {
                Log.Warning("[SR.ModRimWorld.RaidExtension]there is no tree.");
                return false;
            }

            //候选派系列表
            var candidateFactionList = CandidateFactions(map).ToList();
            return Enumerable.Any(candidateFactionList, faction => faction.HostileTo(Faction.OfPlayer));
        }

        /// <summary>
        /// 派系能否成为资源组
        /// </summary>
        /// <param name="f"></param>
        /// <param name="map"></param>
        /// <param name="desperate"></param>
        /// <returns></returns>
        protected override bool FactionCanBeGroupSource(Faction f, Map map, bool desperate = false)
        {
            return base.FactionCanBeGroupSource(f, map, desperate) && f.def.humanlikeFaction && !f.Hidden;
        }

        /// <summary>
        /// 袭击点数
        /// </summary>
        /// <param name="parms"></param>
        protected override void ResolveRaidPoints(IncidentParms parms)
        {
            if (parms.points > MaxThreatPoints)
            {
                parms.points = MaxThreatPoints;
            }
        }

        /// <summary>
        /// 解决突袭策略
        /// </summary>
        /// <param name="parms"></param>
        /// <param name="groupKind"></param>
        public override void ResolveRaidStrategy(IncidentParms parms, PawnGroupKindDef groupKind)
        {
            parms.raidStrategy = RaidStrategyDefOf.SrLogging;
        }

        /// <summary>
        /// 获取信件定义
        /// </summary>
        /// <returns></returns>
        protected override LetterDef GetLetterDef()
        {
            return LetterDefOf.ThreatSmall;
        }
    }
}