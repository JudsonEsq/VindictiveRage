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

        public static string name = "Vindictive Rage";
        ItemIndex rageID = (ItemIndex)ItemLib.ItemLib.GetItemId(name);

        void Start()
        {
            IL.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
        }
        void CharacterBody_RecalculateStats(ILContext il)
        {
            int locNumber = 0;
            ILCursor grrsor = new ILCursor(il);
            grrsor.GotoNext(
                x => x.MatchLdloc(out locNumber),
                x => x.MatchCallvirt<CharacterBody>("set_damage")
                );
            grrsor.Emit(OpCodes.Ldloc, locNumber);
            grrsor.Emit(OpCodes.Ldarg, 0);
            grrsor.EmitDelegate<Func<float, RoR2.CharacterBody, float>>(
                (currentMultiplier, self) =>
                {
                    if (self.inventory)
                        currentMultiplier += (self.inventory.GetItemCount(rageID) * (1-(self.healthComponent.shield + self.healthComponent.health)/self.healthComponent.fullCombinedHealth) * 2);
                    return currentMultiplier;
                }
                );
            grrsor.Emit(OpCodes.Stloc, locNumber);
            Debug.Log(il);
            Debug.Log(locNumber);
            
        }

        //Cheaty Cheaty dev code
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.F2))
            {
                var transform = PlayerCharacterMasterController.instances[0].master.GetBodyObject().transform;
                PickupDropletController.CreatePickupDroplet(new PickupIndex(rageID), transform.position, transform.forward * 20f);
            }
        }

        //Create the fields for various aesthetic aspects of Rage.
        public static UnityEngine.AssetBundle daisy;
        public static UnityEngine.GameObject model;
        public static UnityEngine.Object icon;
        private static ItemDisplayRule[] _itemDisplayRules;

        //Declares that the following method is an item.
        [Item(ItemAttribute.ItemType.Item)]
        public static ItemLib.CustomItem HurtMePlenty()
        {
            ItemDef HurtMePlenty = new ItemDef
            {
                tier = ItemTier.Tier3,
                pickupModelPath = "",
                pickupIconPath = "",
                nameToken = name,
                pickupToken = "This pain will make me stronger.",
                descriptionToken = "At low health, go completely nutters."
            };

            return new ItemLib.CustomItem(HurtMePlenty, null, icon, _itemDisplayRules);

        }

    }
}
