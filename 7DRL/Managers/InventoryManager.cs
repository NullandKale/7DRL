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

        public Utils.Point[] lootItems;

        public InventoryManager(int lootAmount)
        {
            playerInv = new Inventory();
            lootItems = new Utils.Point[lootAmount];

            RegenLoot();
            Game.g.onUpdate.Add(Update);
        }

        public void Update()
        {
            Draw();
        }

        public void Draw()
        {
            for (int i = 0; i < lootItems.Length; i++)
            {
                if(lootItems[i].x != -10)
                {
                    Game.g.world[lootItems[i].x, lootItems[i].y].Visual = 'L';
                    Game.g.world[lootItems[i].x, lootItems[i].y].collideable = false;
                }
            }
        }

        public void RegenLoot()
        {
            for (int i = 0; i < lootItems.Length; i++)
            {
                lootItems[i] = Utils.Point.getRandomPointInWorld();
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
            for (int i = 0; i < lootItems.Length; i++)
            {
                if(lootItems[i].x == lootPosX && lootItems[i].y == lootPosY)
                {
                    lootItems[i].x = -10;
                    lootItems[i].y = -10;
                }
            }


            ItemType temp = Util.RandomEnumValue<ItemType>();

            if(temp == ItemType.Gold)
            {
                int goldAmount = Game.g.rng.Next(10, 10 + (int)(level * 3.653));
                playerInv.currentGoldAmount += goldAmount;
            }
            else if(temp == ItemType.Weapon)
            {
                playerInv.addItem(Weapon.GenerateWeapon(level));
            }
            else if(temp == ItemType.Armor)
            {
                playerInv.addItem(Armor.GenerateArmor(level));
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
            return (name + " " + weight + "lb " + value + " g");
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

        public Weapon(WeaponType w, EffectType e, int level)
        {
            int strBuff = 0;
            int dexBuff = 0;
            int ConBuff = 0;

            texture = 'W';

            if(e == EffectType.none)
            {
                name = w.ToString();
            }
            else
            {
                name = w.ToString() + " of " + e.ToString();
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

            if (e == EffectType.strength)
            {
                strBuff += 1 * level;
            }
            else if (e == EffectType.speed)
            {
                dexBuff += 1 * level;
            }
            else if (e == EffectType.poison)
            {
                damage += 5 * level;
            }
            else if (e == EffectType.hardening)
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
           return new Weapon(Util.RandomEnumValue<WeaponType>(), Util.RandomEnumValue<EffectType>(), level);
        }
    }

    public class Potion : Item
    {
        int HealthIncrease;
        int StaminaIncrease;
        int ManaIncrease;

        public Potion(PotionType p, int level)
        {

        }

        public override void OnEquip()
        {
            
        }

        public override void OnUnequip()
        {
            
        }
    }

    public class Armor : Item
    {
        public int damageReduct;

        public int strBuff;
        public int dexBuff;
        public int ConBuff;

        public Armor(MaterialType m, EffectType e, int level)
        {
            int strBuff = 0;
            int dexBuff = 0;
            int ConBuff = 0;

            texture = 'A';

            if (e == EffectType.none)
            {
                name = m.ToString() + " Armor";
            }
            else
            {
                name = m.ToString() + " Armor of " + e.ToString();
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

            if (e == EffectType.strength)
            {
                strBuff += 1 * level;
            }
            else if (e == EffectType.speed)
            {
                dexBuff += 1 * level;
            }
            else if (e == EffectType.poison)
            {
                dexBuff += 1 * level;
            }
            else if (e == EffectType.hardening)
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
            return new Armor(Util.RandomEnumValue<MaterialType>(), Util.RandomEnumValue<EffectType>(), level);
        }
    }

    public enum EffectType
    {
        none,
        strength,
        speed,
        poison,
        hardening,
    }

    public enum PotionType
    {
        Healing,
        Vigor,
        Mana,
    }

    public enum ItemType
    {
        Gold,
        Weapon,
        Armor,
        Potion,
        //Ring,
        //Amulet,
        //Ingredient
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
}
