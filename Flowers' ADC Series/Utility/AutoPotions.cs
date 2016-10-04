namespace Flowers_ADC_Series.Utility
{
    using System;
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;

    internal class AutoPotions
    {
        private static readonly Menu Menu = Program.Menu;
        private static readonly Obj_AI_Hero Me = Program.Me;

        public AutoPotions()
        {
            var enableActivator = false;

            if (Menu.GetMenu("Activator", "activator") == null &&
                Menu.GetMenu("ElUtilitySuite", "ElUtilitySuite") == null &&
                Menu.GetMenu("MActivator", "masterActivator") == null)
            {
                enableActivator = true;
            }
            else
            {
                enableActivator = false;
            }

            var PotionsMenu = Menu.AddSubMenu(new Menu("Auto Potions", "Auto Potions"));
            {
                PotionsMenu.AddItem(new MenuItem("EnablePosition", "Enabled", true).SetValue(enableActivator));
                PotionsMenu.AddItem(new MenuItem("PositionHp", "When Player HealthPercent <= %", true).SetValue(new Slider(35)));
            }

            Game.OnUpdate += OnUpdate;
        }

        private void OnUpdate(EventArgs Args)
        {
            if (Me.IsDead)
            {
                return;
            }

            if (Me.InFountain() ||
                Me.Buffs.Any(x => x.Name.ToLower().Contains("recall") || !x.Name.ToLower().Contains("teleport")))
            {
                return;
            }

            if (Menu.Item("EnablePosition", true).GetValue<bool>() &&
               Me.HealthPercent <= Menu.Item("PositionHp").GetValue<Slider>().Value)
            {
                if (Me.Buffs.Any(x => x.Name.Equals("ItemCrystalFlask", StringComparison.OrdinalIgnoreCase) ||
                                      x.Name.Equals("ItemCrystalFlaskJungle", StringComparison.OrdinalIgnoreCase) ||
                                      x.Name.Equals("ItemDarkCrystalFlask", StringComparison.OrdinalIgnoreCase) ||
                                      x.Name.Equals("RegenerationPotion", StringComparison.OrdinalIgnoreCase) ||
                                      x.Name.Equals("ItemMiniRegenPotion", StringComparison.OrdinalIgnoreCase) ||
                                      x.Name.Equals("ItemMiniRegenPotion", StringComparison.OrdinalIgnoreCase)))
                {
                    return;
                }

                if (Items.HasItem(2003)) //Health_Potion 
                {
                    Items.UseItem(2003);
                }

                if (Items.HasItem(2009)) //Total_Biscuit_of_Rejuvenation 
                {
                    Items.UseItem(2009);
                }

                if (Items.HasItem(2010)) //Total_Biscuit_of_Rejuvenation2 
                {
                    Items.UseItem(2010);
                }

                if (Items.HasItem(2031)) //Refillable_Potion 
                {
                    Items.UseItem(2031);
                }

                if (Items.HasItem(2032)) //Hunters_Potion 
                {
                    Items.UseItem(2032);
                }

                if (Items.HasItem(2033)) //Corrupting_Potion 
                {
                    Items.UseItem(2033);
                }
            }
        }
    }
}
