/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              WeaponStatGenerator
Description:        Handles stat generation for weapons
Date Created:       10/03/2022
Author:             Maxime Dubois
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    10/03/2022
        - [Max] Created Base Class
    12/03/2022
        - [Max] Added overloaded BaseDamageTierCheck method

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LootTiers;

namespace StatGenerator
{
    class WeaponInfo
    {
        public CharacterBase Owner { get; set; }
        public float BaseDamage { get; set; }
        public bool IsOwnerPlayer { get; set; }
    }

    static class WeaponStatGenerator
    {

        const int MIN_COMMON_DMG = 2;
        const int MAX_COMMON_DMG = 5;
        const int MIN_RARE_DMG = 5;
        const int MAX_RARE_DMG = 8;
        const int MIN_HEROIC_DMG = 8;
        const int MAX_HEROIC_DMG = 11;
        const int MIN_MYTHIC_DMG = 11;
        const int MAX_MYTHIC_DMG = 14;
        const int GODLIKE_DMG = 10;
        const int PLAYER_NONE_DMG = 1;
        const int ENEMY_NONE_DMG = 8;

        public static WeaponInfo GenerateWeaponInfo(LootTier rarity, Transform weaponTransform)
        {
            WeaponInfo weaponInfo = new WeaponInfo();

            weaponInfo.Owner = SetWeaponOwner(weaponTransform);

            if (weaponInfo.Owner != null)
            {
                weaponInfo.IsOwnerPlayer = weaponInfo.Owner.gameObject.tag == "Player" ? true : false;
                weaponInfo.BaseDamage = BaseDamageTierCheck(rarity, weaponInfo.Owner, weaponInfo.IsOwnerPlayer);
            }
            else
            {
                weaponInfo.BaseDamage = BaseDamageTierCheck(rarity);
            }

            return weaponInfo;
        }

        static CharacterBase SetWeaponOwner(Transform weaponTransform)
        {
            // Get Weapon Owner
            if (weaponTransform.parent != null)
            {
                // Get Owner from Parent
                CharacterBase chara = weaponTransform.GetComponentInParent<CharacterBase>();

                // If Owner exists
                if (chara != null)
                {
                    // Return Owner
                    return chara;
                }
            }
            return null;
        }

        static float BaseDamageTierCheck(LootTier rarity, CharacterBase owner, bool isOwnerPlayer)
        {
            // Assign Base Damage Range based on Weapon's Loot Tier. Godlike Weapons are assigned a set value.
            switch (rarity)
            {
                case LootTier.Common:
                    return (float)Random.Range(MIN_COMMON_DMG, MAX_COMMON_DMG);

                case LootTier.Rare:
                    return (float)Random.Range(MIN_RARE_DMG, MAX_RARE_DMG);

                case LootTier.Heroic:
                    return (float)Random.Range(MIN_HEROIC_DMG, MAX_HEROIC_DMG);

                case LootTier.Mythic:
                    return (float)Random.Range(MIN_MYTHIC_DMG, MAX_MYTHIC_DMG);

                case LootTier.Godlike:
                    return (float)GODLIKE_DMG;

                case LootTier.None:
                    // If Owner exists and is Player
                    if (owner != null && isOwnerPlayer)
                    {
                        return (float)PLAYER_NONE_DMG;
                    }
                    else if (owner != null && !isOwnerPlayer)// Owner = Enemy
                    {
                        return (float)ENEMY_NONE_DMG;
                    }
                    else
                    {
                        return (float)PLAYER_NONE_DMG;
                    }

                default:
                    Debug.LogError("WeaponStatGenerator.BaseDamageTierCheck->No recognized rarity");
                    return -1;
            }
        }

        static float BaseDamageTierCheck(LootTier rarity)
        {
            // Assign Base Damage Range based on Weapon's Loot Tier. Godlike Weapons are assigned a set value.
            switch (rarity)
            {
                case LootTier.Common:
                    return (float)Random.Range(MIN_COMMON_DMG, MAX_COMMON_DMG);

                case LootTier.Rare:
                    return (float)Random.Range(MIN_RARE_DMG, MAX_RARE_DMG);

                case LootTier.Heroic:
                    return (float)Random.Range(MIN_HEROIC_DMG, MAX_HEROIC_DMG);

                case LootTier.Mythic:
                    return (float)Random.Range(MIN_MYTHIC_DMG, MAX_MYTHIC_DMG);

                case LootTier.Godlike:
                    return (float)GODLIKE_DMG;
                default:
                    Debug.LogError("WeaponStatGenerator.BaseDamageTierCheck->No recognized rarity");
                    return -1;
            }
        }
    }  
}
