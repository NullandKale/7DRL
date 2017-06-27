namespace _7DRL.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

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
        Amulet,
        Tome
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

    public enum TomeEffect
    {
        Healing,
        FireStorm,
        Vigor,
    }

    public class InventoryManager
    {
        public Inventory playerInv;
        public Weapon equipedWeapon;
        public Armor equipedArmor;
        public Ring equipedRing;
        public Amulet equipedAmulet;
        public Tome equipedTome;

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
                if (lootItems[i].X != -10)
                {
                    Game.g.world[lootItems[i].X, lootItems[i].Y].Color = ConsoleColor.DarkYellow;
                    Game.g.world[lootItems[i].X, lootItems[i].Y].Visual = 'L';
                    Game.g.world[lootItems[i].X, lootItems[i].Y].Collideable = false;
                }
            }

            if (Game.input.IsKeyFalling(OpenTK.Input.Key.B))
            {
                Buy(Game.g.pcStats.level);
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
                lootItems.Add(Utils.Point.GetRandomPointInWorld());
            }
        }

        public string GetItem(int num)
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

        public void Buy(int level)
        {
            if (playerInv.currentGoldAmount >= 100)
            {
                playerInv.currentGoldAmount -= 100;

                ItemType temp = Util.Choose(
                            new ItemType[] { ItemType.Gold, ItemType.Weapon, ItemType.Armor, ItemType.Potion, ItemType.Ring, ItemType.Amulet, ItemType.Tome },
                            new float[] { 0.0f, 0.15f, 0.15f, 0.3f, 0.15f, 0.15f, 0.10f }, Game.g.rng);

                level = Util.Choose(new int[] { level, level + 1 }, new float[] { 0.95f, 0.05f }, Game.g.rng);

                if (temp == ItemType.Weapon)
                {
                    playerInv.AddItem(Weapon.GenerateWeapon(level));
                }
                else if (temp == ItemType.Armor)
                {
                    playerInv.AddItem(Armor.GenerateArmor(level));
                }
                else if (temp == ItemType.Ring)
                {
                    playerInv.AddItem(Ring.GenerateRing(level));
                }
                else if (temp == ItemType.Amulet)
                {
                    playerInv.AddItem(Amulet.GenerateAmulet(level));
                }
                else if (temp == ItemType.Potion)
                {
                    playerInv.AddItem(Potion.GeneratePotion(level));
                }
                else if (temp == ItemType.Tome)
                {
                    playerInv.AddItem(Tome.GenerateTome(level));
                }
            }
        }

        public void AddLootItem(int level, int lootPosX, int lootPosY)
        {
            for (int i = 0; i < lootItems.Count; i++)
            {
                if (lootItems[i].X == lootPosX && lootItems[i].Y == lootPosY)
                {
                    lootItems[i].X = -10;
                    lootItems[i].Y = -10;
                }
            }

            ItemType temp = Util.Choose(
                new ItemType[] { ItemType.Gold, ItemType.Weapon, ItemType.Armor, ItemType.Potion, ItemType.Ring, ItemType.Amulet, ItemType.Tome },
                new float[] { 0.2f, 0.15f, 0.15f, 0.2f, 0.15f, 0.1f, 0.05f }, Game.g.rng);

            level = Util.Choose(new int[] { level, level + 1 }, new float[] { 0.95f, 0.05f }, Game.g.rng);

            int goldAmount = 0;
            bool gotGold = false;

            if (temp == ItemType.Gold)
            {
                goldAmount = Game.g.rng.Next(10, 10 + (int)(level * 3.653));
                playerInv.currentGoldAmount += goldAmount;
                gotGold = true;
            }
            else if (temp == ItemType.Weapon)
            {
                playerInv.AddItem(Weapon.GenerateWeapon(level));
            }
            else if (temp == ItemType.Armor)
            {
                playerInv.AddItem(Armor.GenerateArmor(level));
            }
            else if (temp == ItemType.Ring)
            {
                playerInv.AddItem(Ring.GenerateRing(level));
            }
            else if (temp == ItemType.Amulet)
            {
                playerInv.AddItem(Amulet.GenerateAmulet(level));
            }
            else if (temp == ItemType.Potion)
            {
                playerInv.AddItem(Potion.GeneratePotion(level));
            }
            else if (temp == ItemType.Tome)
            {
                playerInv.AddItem(Tome.GenerateTome(level));
            }
            
            if (!gotGold)
            {
                Game.g.LogCombat("Picked up " + playerInv[playerInv.Count - 1]);
            }
            else
            {
                Game.g.LogCombat("Picked up " + goldAmount + " gold");
            }
        }

        public bool EquipWeapon(int itemLoc)
        {
            if (playerInv.items[itemLoc] is Weapon)
            {
                if (equipedWeapon == null)
                {
                    equipedWeapon = (Weapon)playerInv.items[itemLoc];
                    playerInv.RemoveItem(itemLoc, 1);
                    equipedWeapon.OnEquip();
                }
                else
                {
                    playerInv.AddItem(equipedWeapon);
                    equipedWeapon.OnUnequip();
                    equipedWeapon = (Weapon)playerInv.items[itemLoc];
                    equipedWeapon.OnEquip();
                    playerInv.RemoveItem(itemLoc, 1);
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
                    playerInv.RemoveItem(itemLoc, 1);
                    equipedArmor.OnEquip();
                }
                else
                {
                    playerInv.AddItem(equipedArmor);
                    equipedArmor.OnUnequip();
                    equipedArmor = (Armor)playerInv.items[itemLoc];
                    equipedArmor.OnEquip();
                    playerInv.RemoveItem(itemLoc, 1);
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
                    playerInv.RemoveItem(itemLoc, 1);
                    equipedRing.OnEquip();
                }
                else
                {
                    playerInv.AddItem(equipedRing);
                    equipedRing.OnUnequip();
                    equipedRing = (Ring)playerInv.items[itemLoc];
                    equipedRing.OnEquip();
                    playerInv.RemoveItem(itemLoc, 1);
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public void Sell(int itemLoc)
        {
            if (itemLoc < playerInv.items.Count)
            {
                playerInv.currentGoldAmount += playerInv.items[itemLoc].value;
                playerInv.RemoveItem(itemLoc, 1);
            }
        }

        public bool EquipAmulet(int itemLoc)
        {
            if (playerInv.items[itemLoc] is Amulet)
            {
                if (equipedAmulet == null)
                {
                    equipedAmulet = (Amulet)playerInv.items[itemLoc];
                    playerInv.RemoveItem(itemLoc, 1);
                    equipedAmulet.OnEquip();
                }
                else
                {
                    playerInv.AddItem(equipedAmulet);
                    equipedAmulet.OnUnequip();
                    equipedAmulet = (Amulet)playerInv.items[itemLoc];
                    equipedAmulet.OnEquip();
                    playerInv.RemoveItem(itemLoc, 1);
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool EquipTome(int itemLoc)
        {
            if (playerInv.items[itemLoc] is Tome)
            {
                if (equipedTome == null)
                {
                    equipedTome = (Tome)playerInv.items[itemLoc];
                    playerInv.RemoveItem(itemLoc, 1);
                    equipedTome.OnEquip();
                }
                else
                {
                    playerInv.AddItem(equipedTome);
                    equipedTome.OnUnequip();
                    equipedTome = (Tome)playerInv.items[itemLoc];
                    equipedTome.OnEquip();
                    playerInv.RemoveItem(itemLoc, 1);
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool UseTome()
        {
            if (equipedTome != null)
            {
                equipedTome.Use();
                return true;
            }

            return false;
        }

        public bool UsePotion(int itemLoc)
        {
            if (playerInv.items[itemLoc] is Potion)
            {
                ((Potion)playerInv.items[itemLoc]).OnEquip();
                playerInv.RemoveItem(itemLoc, 1);
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

        public float currentWeight;
        public float MaxWeight;

        public Inventory()
        {
            items = new List<Item>();
            currentGoldAmount = 0;

            UpdateWeight();
        }

        public int Count
        {
            get
            {
                return items.Count;
            }
        }

        public Item this[int index]
        {
            get
            {
                return items[index];
            }
        }

        public void AddItem(Item item)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].name == item.name)
                {
                    if (items[i].currentStackSize + item.currentStackSize <= items[i].maxStackSize)
                    {
                        items[i].currentStackSize += item.currentStackSize;
                        UpdateWeight();
                        return;
                    }
                    else
                    {
                        int itemsOver = items[i].currentStackSize + item.currentStackSize - items[i].maxStackSize;
                        items[i].currentStackSize = items[i].maxStackSize;
                        item.currentStackSize = itemsOver;
                        items.Add(item);
                        items.Last().inventoryPos = items.Count;
                        UpdateWeight();
                        return;
                    }
                }
            }

            items.Add(item);
            items.Last().inventoryPos = items.Count;
            UpdateWeight();
        }

        public void RemoveItem(int loc, int amount)
        {
            if (items[loc].currentStackSize - amount < 0)
            {
                items[loc].currentStackSize -= amount;
            }
            else
            {
                items.RemoveAt(loc);
            }

            UpdateWeight();
        }

        public void CleanInv()
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].currentStackSize <= 0)
                {
                    RemoveItem(i, 10000);
                }
            }
        }

        private void UpdateWeight()
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
            return name + " " + weight + "lb";
        }

        public abstract void OnEquip();

        public abstract void OnUnequip();
    }

    public class Tome : Item
    {
        public int healAmount;
        public int vigorAmount;
        public int fireballDamage;
        public int manacost;

        private TomeEffect effect;

        public Tome(TomeEffect t, int level)
        {
            weight = 1;

            if (level < 2)
            {
                name = "Tome of " + t.ToString();
            }
            else
            {
                name = "Tome of " + t.ToString() + " +" + level;
            }

            value = 10 * level;
            maxStackSize = 1;
            currentStackSize = 1;
            effect = t;

            if (t == TomeEffect.FireStorm)
            {
                fireballDamage = (10 + Game.g.pcStats.intel) * level;
                manacost = 50;
            }
            else if (t == TomeEffect.Healing)
            {
                healAmount = (10 + Game.g.pcStats.intel) * level;
                manacost = (int)(healAmount * .75f);
            }
            else if (t == TomeEffect.Vigor)
            {
                vigorAmount = (10 + Game.g.pcStats.intel) * level;
                manacost = (int)(vigorAmount * .75f);
            }
        }

        public static Tome GenerateTome(int level)
        {
            return new Tome(Util.RandomEnumValue<TomeEffect>(), level);
        }

        public override void OnEquip()
        {
        }

        public void Use()
        {
            if (effect == TomeEffect.Healing)
            {
                if (Game.g.pcStats.currentMana - manacost >= 0)
                {
                    Game.g.pcStats.Heal(healAmount);
                    Game.g.pcStats.currentMana -= manacost;
                }
            }
            else if (effect == TomeEffect.FireStorm)
            {
                if (Game.g.pcStats.currentMana - manacost >= 0)
                {
                    for (int i = 0; i < Game.g.enemy.Length; i++)
                    {
                        if (Utils.Point.SquareDist(Game.g.player.pos, Game.g.enemy[i].pos) < 25)
                        {
                            Game.g.enemyAI[i].GetHurt(Game.g.enemy[i]);
                        }
                    }

                    Game.g.pcStats.currentMana -= manacost;
                }
            }
            else if (effect == TomeEffect.Vigor)
            {
                if (Game.g.pcStats.currentMana - manacost >= 0)
                {
                    Game.g.pcStats.RegenStam(healAmount);
                    Game.g.pcStats.currentMana -= manacost;
                }
            }
        }

        public override void OnUnequip()
        {
        }
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

            if (e == WeaponEffectType.none)
            {
                name = w.ToString();
            }
            else if (level < 2)
            {
                name = w.ToString() + " of " + e.ToString();
            }
            else
            {
                name = w.ToString() + " of " + e.ToString() + " +" + level;
            }

            maxStackSize = 1;
            currentStackSize = 1;

            weight = 10;
            value = 10 * level;

            if (w == WeaponType.Dagger)
            {
                damage = 5 * level;
                dexBuff += 1 * level;
                weight -= 5;
                value -= 5 * level;
            }
            else if (w == WeaponType.Sword)
            {
                damage = 10 * level;
            }
            else if (w == WeaponType.Axe)
            {
                damage = 15 * level;
                dexBuff -= 1 * level;
                weight += 5;
                value += 5 * level;
            }

            if (e == WeaponEffectType.strength)
            {
                strBuff += 1 * level;
                value += 5 * level;
            }
            else if (e == WeaponEffectType.speed)
            {
                dexBuff += 1 * level;
                value += 5 * level;
            }
            else if (e == WeaponEffectType.poison)
            {
                damage += 5 * level;
                value += 5 * level;
            }
            else if (e == WeaponEffectType.hardening)
            {
                ConBuff += 1 * level;
                value += 5 * level;
            }
        }

        public static Weapon GenerateWeapon(int level)
        {
            return new Weapon(Util.RandomEnumValue<WeaponType>(), Util.RandomEnumValue<WeaponEffectType>(), level);
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
    }

    public class Potion : Item
    {
        private int HealthIncrease;
        private int StaminaIncrease;
        private int ManaIncrease;

        public Potion(PotionType p, int level)
        {
            texture = 'P';

            maxStackSize = 5;
            currentStackSize = 1;

            value = 10 * level;
            weight = 2;

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
            else if (p == PotionType.Mana)
            {
                ManaIncrease += 100 * level;
            }
            else if (p == PotionType.Vigor)
            {
                StaminaIncrease += 100 * level;
            }
        }

        public static Potion GeneratePotion(int level)
        {
            return new Potion(Util.RandomEnumValue<PotionType>(), level);
        }

        public override void OnEquip()
        {
            if (Game.g.pcStats.currentHealth + HealthIncrease > Game.g.pcStats.maxHealth)
            {
                Game.g.pcStats.currentHealth = Game.g.pcStats.maxHealth;
            }
            else
            {
                Game.g.pcStats.currentHealth += HealthIncrease;
            }

            if (Game.g.pcStats.currentMana + ManaIncrease > Game.g.pcStats.maxMana)
            {
                Game.g.pcStats.currentMana = Game.g.pcStats.maxMana;
            }
            else
            {
                Game.g.pcStats.currentMana += ManaIncrease;
            }

            if (Game.g.pcStats.currentStamina + StaminaIncrease > Game.g.pcStats.maxStamina)
            {
                Game.g.pcStats.currentStamina = Game.g.pcStats.maxStamina;
            }
            else
            {
                Game.g.pcStats.currentStamina += StaminaIncrease;
            }
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

            value = 10 * level;
            weight = 20;

            if (m == MaterialType.Leather)
            {
                damageReduct = 5 * level;
                value -= 5 * level;
                weight -= 5;
            }
            else if (m == MaterialType.Iron)
            {
                damageReduct = 10 * level;
            }
            else if (m == MaterialType.Steel)
            {
                damageReduct = 15 * level;
                value += 5 * level;
                weight += 10;
            }

            if (e == ArmorEffectType.strength)
            {
                strBuff += 1 * level;
                value += 5 * level;
            }
            else if (e == ArmorEffectType.speed)
            {
                dexBuff += 1 * level;
                value += 5 * level;
            }
            else if (e == ArmorEffectType.hardening)
            {
                ConBuff += 1 * level;
                value += 5 * level;
            }
        }

        public static Armor GenerateArmor(int level)
        {
            return new Armor(Util.RandomEnumValue<MaterialType>(), Util.RandomEnumValue<ArmorEffectType>(), level);
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

            value = 20;

            if (r == RingMaterialType.Copper)
            {
                effectMulitplier = 1;
                weight = 0.3f;
                value -= 10 * level;
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
                value += 10 * level;
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

        public static Ring GenerateRing(int level)
        {
            return new Ring(Util.RandomEnumValue<RingMaterialType>(), Util.RandomEnumValue<JewelleryType>(), level);
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

            value = 25;

            if (r == RingMaterialType.Copper)
            {
                effectMulitplier = 1;
                weight = 0.3f;
                value -= 15 * level;
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
                value += 15 * level;
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

        public static Amulet GenerateAmulet(int level)
        {
            return new Amulet(Util.RandomEnumValue<RingMaterialType>(), Util.RandomEnumValue<JewelleryType>(), level);
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
    }
}
