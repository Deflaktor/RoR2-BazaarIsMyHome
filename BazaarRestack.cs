﻿using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace BazaarIsMyHome
{
    public class BazaarRestack : BazaarBase
    {
        AsyncOperationHandle<InteractableSpawnCard> iscShrineRestack;
        public override void Init()
        {
            iscShrineRestack = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/ShrineRestack/iscShrineRestack.asset");
        }

        public override void Hook()
        {
            On.RoR2.PurchaseInteraction.ScaleCost += PurchaseInteraction_ScaleCost;
        }

        public override void SetupBazaar()
        {
            SpawnShrineRestack();
        }

        private void PurchaseInteraction_ScaleCost(On.RoR2.PurchaseInteraction.orig_ScaleCost orig, PurchaseInteraction self, float scalar)
        {
            if (ModConfig.EnableMod.Value)
            {
                if (self.name.StartsWith("ShrineRestack"))
                {
                    if (ModConfig.EnableShrineRestack.Value)
                    {
                        scalar = (float)ModConfig.ShrineRestackScalar.Value;
                    }
                }
            }
            orig(self, scalar);
        }

        private void SpawnShrineRestack()
        {
            if (ModConfig.EnableShrineRestack.Value)
            {
                // 跌序
                SpawnCard spawnCard = iscShrineRestack.WaitForCompletion();
                GameObject shrinerestackOne = spawnCard.DoSpawn(new Vector3(-130f, -24f, -40f), Quaternion.identity, new DirectorSpawnRequest(spawnCard, DirectPlacement, Run.instance.runRNG)).spawnedInstance;
                shrinerestackOne.transform.eulerAngles = new Vector3(0.0f, 220f, 0.0f);
                shrinerestackOne.GetComponent<ShrineRestackBehavior>().maxPurchaseCount = ModConfig.ShrineRestackMaxCount.Value;
                shrinerestackOne.GetComponent<PurchaseInteraction>().cost = ModConfig.ShrineRestackCost.Value;
                shrinerestackOne.GetComponent<PurchaseInteraction>().Networkcost = ModConfig.ShrineRestackCost.Value;
                if (ModConfig.PenaltyCoefficient_Temp != 1)
                {
                    shrinerestackOne.GetComponent<PurchaseInteraction>().cost *= ModConfig.PenaltyCoefficient_Temp;
                    shrinerestackOne.GetComponent<ShrineRestackBehavior>().costMultiplierPerPurchase = ModConfig.PenaltyCoefficient_Temp;
                }
            }
        }
    }
}
