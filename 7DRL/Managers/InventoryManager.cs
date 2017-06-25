using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DRL.Managers
{
    public class InventoryManager
    {
        public Inventory playerInv;
        public Weapon equipedWeapon;
        public Armor equipedArmor;
        public Ring equipedRing;
        public Amulet equipedAmulet;

        public List<Utils.Point> lootItems;
        private int lootAmount;

        public InventoryManager(int lootAmount)
        {
            playerInv = new Inventory();

            lootItems = new List<Utils.Point>();
            this.lootAmount = lootAmount;

            RegenLoot();
            Game.g.onUpdate.Add(Update);
        }

        public void Update()
        {
            Draw();
        }

        public void Draw()
        {
            for (int i = 0; i < lootItems.Count; i++)
            {
                if(lootItems[i].x != -10)
                {
                    Game.g.world[lootItems[i].x, lootItems[i].y].color = ConsoleColor.DarkYellow;
                    Game.g.world[lootItems[i].x, lootItems[i].y].Visual = 'L';
                    Game.g.world[lootItems[i].x, lootItems[i].y].collideable = false;
                }
            }
        }

        public void SpawnLoot(int xPos, int yPos)
        {
            lootItems.Add(new Utils.Point(xPos, yPos));
        }

        public void RegenLoot()
        {
            lootItems.Clear();
            for (int i = 0; i < lootAmount; i++)
            {
                lootItems.Add(Utils.Point.getRandomPointInWorld());
            }
        }

        public string getItem(int num)
        {
            if (playerInv.items.Count > num)
            {
                return playerInv.items[num].ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        public void AddLootItem(int level, int lootPosX, int lootPosY)
        {
            for (int i = 0; i < lootItems.Count; i++)
            {
                if(lootItems[i].x == lootPosX && lootItems[i].y == lootPosY)
                {
                    lootItems[i].x = -10;
                    lootItems[i].y = -10;
                }
            }

            ItemType temp = Util.RandomEnumValue<ItemType>();

            if (temp == ItemType.Gold)
            {
                int goldAmount = Game.g.rng.Next(10, 10 + (int)(level * 3.653));
                playerInv.currentGoldAmount += goldAmount;
            }
            else if (temp == ItemType.Weapon)
            {
                playerInv.addItem(Weapon.GenerateWeapon(level));
            }
            else if (temp == ItemType.Armor)
            {
                playerInv.addItem(Armor.GenerateArmor(level));
            }
            else if (temp == ItemType.Ring)
            {
                playerInv.addItem(Ring.GenerateRing(level));
            }
            else if (temp == ItemType.Amulet)
            {
                playerInv.addItem(Amulet.GenerateAmulet(level));
            }
            else if (temp == ItemType.Potion)
            {
                playerInv.addItem(Potion.GeneratePotion(level));
            }
            //Add for new ItemTypes
        }

        public bool EquipWeapon(int itemLoc)
        {
            if (playerInv.items[itemLoc] is Weapon)
            {
                if (equipedWeapon == null)
                {
                    equipedWeapon = (Weapon)playerInv.items[itemLoc];
                    playerInv.removeItem(itemLoc, 1);
                    equipedWeapon.OnEquip();
                }
                else
                {
                    playerInv.addItem(equipedWeapon);
                    equipedWeapon.OnUnequip();
                    equipedWeapon = (Weapon)playerInv.items[itemLoc];
                    equipedWeapon.OnEquip();
                    playerInv.removeItem(itemLoc, 1);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool EquipArmor(int itemLoc)
        {
            if (playerInv.items[itemLoc] is Armor)
            {
                if (equipedArmor == null)
                {
                    equipedArmor = (Armor)playerInv.items[itemLoc];
                    playerInv.removeItem(itemLoc, 1);
                    equipedArmor.OnEquip();
                }
                else
                {
                    playerInv.addItem(equipedArmor);
                    equipedArmor.OnUnequip();
                    equipedArmor = (Armor)playerInv.items[itemLoc];
                    equipedArmor.OnEquip();
                    playerInv.removeItem(itemLoc, 1);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool EquipRing(int itemLoc)
        {
            if (playerInv.items[itemLoc] is Ring)
            {
                if (equipedRing == null)
                {
                    equipedRing = (Ring)playerInv.items[itemLoc];
                    playerInv.removeItem(itemLoc, 1);
                    equipedRing.OnEquip();
                }
                else
                {
                    playerInv.addItem(equipedRing);
                    equipedRing.OnUnequip();
                    equipedRing = (Ring)playerInv.items[itemLoc];
                    equipedRing.OnEquip();
                    playerInv.removeItem(itemLoc, 1);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool EquipAmulet(int itemLoc)
        {
            if (playerInv.items[itemLoc] is Amulet)
            {
                if (equipedAmulet == null)
                {
                    equipedAmulet = (Amulet)playerInv.items[itemLoc];
                    playerInv.removeItem(itemLoc, 1);
                    equipedAmulet.OnEquip();
                }
                else
                {
                    playerInv.addItem(equipedAmulet);
                    equipedAmulet.OnUnequip();
                    equipedAmulet = (Amulet)playerInv.items[itemLoc];
                    equipedAmulet.OnEquip();
                    playerInv.removeItem(itemLoc, 1);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool UsePotion(int itemLoc)
        {
            if (playerInv.items[itemLoc] is Potion)
            {
                ((Potion)playerInv.items[itemLoc]).OnEquip();
                playerInv.removeItem(itemLoc, 1);
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public class Inventory
    {
        public List<Item> items;

        public int currentGoldAmount;

        float currentWeight;
        float MaxWeight;

        public Inventory()
        {
            items = new List<Item>();
            currentGoldAmount = 0;

            updateWeight();
        }

        public void addItem(Item item)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].name == item.name)
                {
                    if (items[i].currentStackSize + item.currentStackSize <= items[i].maxStackSize)
                    {
                        items[i].currentStackSize += item.currentStackSize;
                        updateWeight();
                        return;
                    }
                    else
                    {
                        int itemsOver = items[i].currentStackSize + item.currentStackSize - items[i].maxStackSize;
                        items[i].currentStackSize = items[i].maxStackSize;
                        item.currentStackSize = itemsOver;
                        items.Add(item);
                        items.Last().inventoryPos = items.Count;
                        updateWeight();
                        return;
                    }
                }
            }

            items.Add(item);
            items.Last().inventoryPos = items.Count;
            updateWeight();
        }

        public void removeItem(int loc, int amount)
        {
            if (items[loc].currentStackSize - amount < 0)
            {
                items[loc].currentStackSize -= amount;
            }
            else
            {
                items.RemoveAt(loc);
            }

            updateWeight();
        }

        public void CleanInv()
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].currentStackSize <= 0)
                {
                    removeItem(i, 10000);
                }
            }
        }

        private void updateWeight()
        {
            MaxWeight = Game.g.pcStats.carryWeight;

            currentWeight = 0;
            for (int i = 0; i < items.Count; i++)
            {
                currentWeight += items[i].weight * items[i].currentStackSize;
            }

            if (currentWeight > MaxWeight)
            {
                Game.g.pcStats.isEncumbered = true;
            }
            else
            {
                Game.g.pcStats.isEncumbered = false;
            }

        }
    }

    public abstract class Item
    {
        public char texture;
        public bool isOnGround;
        public int xPos;
        public int yPos;

        public int inventoryPos;
        public string name;
        public float weight;
        public int value;

        public int maxStackSize;
        public int currentStackSize;

        public override string ToString()
        {
            return (name + " " + weight + "lb");
        }

        public abstract void OnEquip();
        public abstract void OnUnequip();

    }

    public class Weapon : Item
    {
        public int damage;

        public int strBuff;
        public int dexBuff;
        public int ConBuff;

        public Weapon(WeaponType w, WeaponEffectType e, int level)
        {
            int strBuff = 0;
            int dexBuff = 0;
            int ConBuff = 0;

            texture = 'W';

            if(e == WeaponEffectType.none)
            {
                name = w.ToString();
            }
            else if(level < 2)
            {
                name = w.ToString() + " of " + e.ToString();
            }
            else
            {
                name = w.ToString() + " of " + e.ToString() + " +" + level;
            }

            maxStackSize = 1;
            currentStackSize = 1;

            value = 0;

            if(w == WeaponType.Dagger)
            {
                damage = 5 * level;
                dexBuff += 1 * level;
                weight = 2f;
            }
            else if (w == WeaponType.Sword)
            {
                damage = 10 * level;
                weight = 3f;
            }
            else if (w == WeaponType.Axe)
            {
                damage = 15 * level;
                dexBuff -= 1 * level;
                weight = 5f;
            }

            if (e == WeaponEffectType.strength)
            {
                strBuff += 1 * level;
            }
            else if (e == WeaponEffectType.speed)
            {
                dexBuff += 1 * level;
            }
            else if (e == WeaponEffectType.poison)
            {
                damage += 5 * level;
            }
            else if (e == WeaponEffectType.hardening)
            {
                ConBuff += 1 * level;
            }
        }


        public override void OnEquip()
        {
            Game.g.pcStats.weaponDamage = damage;
            Game.g.pcStats.str += strBuff;
            Game.g.pcStats.dex += dexBuff;
            Game.g.pcStats.con += ConBuff;

            Game.g.pcStats.RegenStats();
        }

        public override void OnUnequip()
        {
            Game.g.pcStats.weaponDamage = 0;
            Game.g.pcStats.str -= strBuff;
            Game.g.pcStats.dex -= dexBuff;
            Game.g.pcStats.con -= ConBuff;

            Game.g.pcStats.RegenStats();
        }

        public static Weapon GenerateWeapon(int level)
        {
           return new Weapon(Util.RandomEnumValue<WeaponType>(), Util.RandomEnumValue<WeaponEffectType>(), level);
        }
    }

    public class Potion : Item
    {
        int HealthIncrease;
        int StaminaIncrease;
        int ManaIncrease;

        public Potion(PotionType p, int level)
        {
            texture = 'P';

            maxStackSize = 5;
            currentStackSize = 1;

            if (level < 2)
            {
                name = "Potion of " + p.ToString();
            }
            else
            {
                name = "Potion of " + p.ToString() + " +" + level;
            }

            if (p == PotionType.Healing)
            {
                HealthIncrease += 100 * level;
            }
            else if(p == PotionType.Mana)
            {
                ManaIncrease += 100 * level;
            }
            else if (p == PotionType.Vigor)
            {
                StaminaIncrease += 100 * level;
            }
        }

        public override void OnEquip()
        {
            Game.g.pcStats.currentHealth += HealthIncrease;
            Game.g.pcStats.currentMana += ManaIncrease;
            Game.g.pcStats.currentStamina += StaminaIncrease;
        }

        public override void OnUnequip()
        {            
        }

        public static Potion GeneratePotion(int level)
        {
            return new Potion(Util.RandomEnumValue<PotionType>(),  level);
        }
    }

    public class Armor : Item
    {
        public int damageReduct;

        public int strBuff;
        public int dexBuff;
        public int ConBuff;

        public Armor(MaterialType m, ArmorEffectType e, int level)
        {
            int strBuff = 0;
            int dexBuff = 0;
            int ConBuff = 0;

            texture = 'A';

            if (e == ArmorEffectType.none)
            {
                name = m.ToString() + " Armor";
            }
            else if (level < 2)
            {
                name = m.ToString() + " Armor of " + e.ToString();
            }
            else
            {
                name = m.ToString() + " Armor of " + e.ToString() + " +" + level;
            }

            maxStackSize = 1;
            currentStackSize = 1;

            value = 0;

            if(m == MaterialType.Leather)
            {
                damageReduct = 5 * level;
            }
            else if(m == MaterialType.Iron)
            {
                damageReduct = 10 * level;
            }
            else if(m == MaterialType.Steel)
            {
                damageReduct = 15 * level;
            }

            if (e == ArmorEffectType.strength)
            {
                strBuff += 1 * level;
            }
            else if (e == ArmorEffectType.speed)
            {
                dexBuff += 1 * level;
            }
            else if (e == ArmorEffectType.hardening)
            {
                ConBuff += 1 * level;
            }
        }

        public override void OnEquip()
        {
            Game.g.pcStats.damageReduction = damageReduct;
            Game.g.pcStats.str += strBuff;
            Game.g.pcStats.dex += dexBuff;
            Game.g.pcStats.con += ConBuff;

            Game.g.pcStats.RegenStats();
        }

        public override void OnUnequip()
        {
            Game.g.pcStats.damageReduction = 0;
            Game.g.pcStats.str -= strBuff;
            Game.g.pcStats.dex -= dexBuff;
            Game.g.pcStats.con -= ConBuff;

            Game.g.pcStats.RegenStats();
        }

        public static Armor GenerateArmor(int level)
        {
            return new Armor(Util.RandomEnumValue<MaterialType>(), Util.RandomEnumValue<ArmorEffectType>(), level);
        }
    }

    public class Ring : Item
    {
        public int damage;

        public int strBuff;
        public int dexBuff;
        public int ConBuff;
        public int WisBuff;
        public int IntelBuff;

        private int effectMulitplier;

        public Ring(RingMaterialType r, JewelleryType e, int level)
        {
            int strBuff = 0;
            int dexBuff = 0;
            int ConBuff = 0;

            texture = 'R';

            if (level < 2)
            {
                name = r.ToString() + " Ring of " + e.ToString();
            }
            else
            {
                name = r.ToString() + " Ring of " + e.ToString() + " +" + level;
            }

            maxStackSize = 1;
            currentStackSize = 1;

            value = 0;

            if (r == RingMaterialType.Copper)
            {
                effectMulitplier = 1;
                weight = 0.3f;
            }
            else if (r == RingMaterialType.Silver)
            {
                effectMulitplier = 2;
                weight = 0.3f;
            }
            else if (r == RingMaterialType.Gold)
            {
                effectMulitplier = 3;
                weight = 0.3f;
            }

            if (e == JewelleryType.Str)
            {
                strBuff += (1 + effectMulitplier) * level;
            }
            else if (e == JewelleryType.Dex)
            {
                dexBuff += (1 + effectMulitplier) * level;
            }
            else if (e == JewelleryType.Con)
            {
                ConBuff += (1 + effectMulitplier) * level;
            }
            else if (e == JewelleryType.Wis)
            {
                WisBuff += (1 + effectMulitplier) * level;
            }
            else if (e == JewelleryType.Int)
            {
                IntelBuff += (1 + effectMulitplier) * level;
            }
        }
        
        public override void OnEquip()
        {
            Game.g.pcStats.weaponDamage = damage;
            Game.g.pcStats.str += strBuff;
            Game.g.pcStats.dex += dexBuff;
            Game.g.pcStats.con += ConBuff;
            Game.g.pcStats.wis += WisBuff;
            Game.g.pcStats.intel += IntelBuff;

            Game.g.pcStats.RegenStats();
        }

        public override void OnUnequip()
        {
            Game.g.pcStats.weaponDamage = 0;
            Game.g.pcStats.str -= strBuff;
            Game.g.pcStats.dex -= dexBuff;
            Game.g.pcStats.con -= ConBuff;
            Game.g.pcStats.wis -= WisBuff;
            Game.g.pcStats.intel -= IntelBuff;

            Game.g.pcStats.RegenStats();
        }

        public static Ring GenerateRing(int level)
        {
            return new Ring(Util.RandomEnumValue<RingMaterialType>(), Util.RandomEnumValue<JewelleryType>(), level);
        }
    }

    public class Amulet : Item
    {
        public int damage;

        public int strBuff;
        public int dexBuff;
        public int ConBuff;
        public int WisBuff;
        public int IntelBuff;

        private int effectMulitplier;

        public Amulet(RingMaterialType r, JewelleryType e, int level)
        {
            int strBuff = 0;
            int dexBuff = 0;
            int ConBuff = 0;

            texture = 'a';

            if (level < 2)
            {
                name = r.ToString() + " Amulet of " + e.ToString();
            }
            else
            {
                name = r.ToString() + " Amulet of " + e.ToString() + " +" + level;
            }

            maxStackSize = 1;
            currentStackSize = 1;

            value = 0;

            if (r == RingMaterialType.Copper)
            {
                effectMulitplier = 1;
                weight = 0.3f;
            }
            else if (r == RingMaterialType.Silver)
            {
                effectMulitplier = 2;
                weight = 0.3f;
            }
            else if (r == RingMaterialType.Gold)
            {
                effectMulitplier = 3;
                weight = 0.3f;
            }

            if (e == JewelleryType.Str)
            {
                strBuff += (3 + effectMulitplier) * level;
            }
            else if (e == JewelleryType.Dex)
            {
                dexBuff += (3 + effectMulitplier) * level;
            }
            else if (e == JewelleryType.Con)
            {
                ConBuff += (3 + effectMulitplier) * level;
            }
            else if (e == JewelleryType.Wis)
            {
                WisBuff += (1 + effectMulitplier) * level;
            }
            else if (e == JewelleryType.Int)
            {
                IntelBuff += (1 + effectMulitplier) * level;
            }
        }

        public override void OnEquip()
        {
            Game.g.pcStats.weaponDamage = damage;
            Game.g.pcStats.str += strBuff;
            Game.g.pcStats.dex += dexBuff;
            Game.g.pcStats.con += ConBuff;
            Game.g.pcStats.wis += WisBuff;
            Game.g.pcStats.intel += IntelBuff;

            Game.g.pcStats.RegenStats();
        }

        public override void OnUnequip()
        {
            Game.g.pcStats.weaponDamage = 0;
            Game.g.pcStats.str -= strBuff;
            Game.g.pcStats.dex -= dexBuff;
            Game.g.pcStats.con -= ConBuff;
            Game.g.pcStats.wis -= WisBuff;
            Game.g.pcStats.intel -= IntelBuff;

            Game.g.pcStats.RegenStats();
        }

        public static Amulet GenerateAmulet(int level)
        {
            return new Amulet(Util.RandomEnumValue<RingMaterialType>(), Util.RandomEnumValue<JewelleryType>(), level);
        }
    }

    public enum WeaponEffectType
    {
        none,
        strength,
        speed,
        poison,
        hardening,
    }

    public enum ArmorEffectType
    {
        none,
        strength,
        speed,
        hardening,
    }

    public enum PotionType
    {
        Healing,
        Vigor,
        Mana,
    }

    public enum JewelleryType
    {
        Str,
        Dex,
        Con,
        Wis,
        Int
    }

    public enum ItemType
    {
        Gold,
        Weapon,
        Armor,
        Potion,
        Ring,
        Amulet
    }

    public enum WeaponType
    {
        Sword,
        Dagger,
        Axe
    }

    public enum MaterialType
    {
        Leather,
        Iron,
        Steel
    }

    public enum RingMaterialType
    {
        Copper,
        Gold,
        Silver
    }
}
