using R2API;
using UnityEngine;
using BepInEx;
using RoR2;
using ItemLib;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;

namespace VindictiveRage
{
    //Tells BepIn r2api is necessary
    [BepInDependency("com.bepis.r2api")]

    [BepInDependency(ItemLibPlugin.ModGuid)]

    [BepInPlugin("dev.JudsonEsq.TooAngryToDie", "Vindictive Rage", "0.0.1")]

    public class RageMod : BaseUnityPlugin
    {
        void Start()
        {
            IL.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
        }
        void CharacterBody_RecalculateStats(ILContext il)
        {
            int locNumber = 0;
            ILCursor grrsor = new ILCursor(il);
            grrsor.GotoNext(
                x => x.MatchLdarg(0),
                x => x.MatchLdloc(out locNumber)
                );
            grrsor.Index += 1;
            grrsor.Emit(OpCodes.Ldloc, locNumber);
            grrsor.Emit(OpCodes.Ldarg, 0);
            grrsor.EmitDelegate<Func<float, RoR2.CharacterBody, float>>(
                (currentMultiplier, self) =>
                {
                    if (self.inventory)
                        currentMultiplier += (self.inventory.GetItemCount((ItemIndex)ItemLib.ItemLib.GetItemId("Vindictive Rage")) * (1-(self.healthComponent.shield + self.healthComponent.health)/self.healthComponent.fullCombinedHealth) * 2);
                    return currentMultiplier;
                }
                );
            grrsor.Emit(OpCodes.Stloc, locNumber);
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.F2))
            {
                var transform = PlayerCharacterMasterController.instances[0].master.GetBodyObject().transform;
                PickupDropletController.CreatePickupDroplet(PickupIndex.Find("ItemIndex.Count"), transform.position, transform.forward * 20f);
            }
        }
        public static UnityEngine.AssetBundle daisy;
        public static UnityEngine.GameObject model;
        public static UnityEngine.Object icon;
        private static ItemDisplayRule[] _itemDisplayRules;

        public static ItemLib.CustomItem HurtMePlenty()
        {
            ItemDef HurtMePlenty = new ItemDef
            {
                tier = ItemTier.Tier3,
                pickupModelPath = "",
                pickupIconPath = "",
                nameToken = "Vindictive Rage",
                pickupToken = "This pain will make me stronger.",
                descriptionToken = "At low health, go completely nutters."
            };

            return new ItemLib.CustomItem(HurtMePlenty, null, icon, _itemDisplayRules);

        }

    }
}
