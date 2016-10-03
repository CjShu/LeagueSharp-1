namespace Flowers_ADC_Series
{
    using System;
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;

    internal class Program
    {
        public static int SkinID;
        public static bool EnableActivator = true;
        public static Menu Menu;
        public static Menu Championmenu;
        public static Obj_AI_Hero Me;
        public static Orbwalking.Orbwalker Orbwalker;

        public static string[] DangerSpellName = new[]
        {
            "KatarinaR", "GalioIdolOfDurand", "GragasE", "Crowstorm", "BandageToss", "LissandraE",
            "AbsoluteZero", "AlZaharNetherGrasp", "FallenOne", "PantheonRJump", "CaitlynAceintheHole",
            "MissFortuneBulletTime", "InfiniteDuress", "ThreshQ", "RocketGrab", "AatroxQ", "AkaliShadowDance",
            "Headbutt", "DianaTeleport", "AlZaharNetherGrasp", "JaxLeapStrike", "KatarinaE", "KhazixE",
            "LeonaZenithBlade",
            "MaokaiTrunkLine", "MonkeyKingNimbus", "PantheonW", "PoppyHeroicCharge", "ShenShadowDash",
            "SejuaniArcticAssault", "RenektonSliceAndDice", "Slash", "XenZhaoSweep", "RocketJump"
        };

        private static string[] SkinName
        {
            get
            {
                switch (ObjectManager.Player.ChampionName)
                {
                    case "Ashe":
                        return new[]
                        {
                            "Classic", "Freljord Ashe", "Sherwood Forest Ashe",
                            "Woad Ashe", "Queen Ashe", "Amethyst Ashe", "Heartseeker Ashe",
                            "Marauder Ashe", "Project Ashe"
                        };
                    case "Caitlyn":
                        return new[]
                        {
                            "Classic", "Resistance Caitlyn", "Sheriff Caitlyn",
                            "Safari Caitlyn", "Arctic Warfare Caitlyn", "Officer Caitlyn",
                            "Headhunter Caitlyn", "Lunar Wraith Caitlyn"
                        };
                    case "Corki":
                        return new[]
                        {
                            "Classic", "UFO Corki", "Ice Toboggan Corki", "Red Baron Corki",
                            "Hot Rod Corki", "Urfrider Corki", "Dragonwing Corki", "Fnatic Corki"
                        };
                    case "Draven":
                        return new[]
                        {
                            "Classic", "Soul Reaver Draven", "Gladiator Draven", "Primetime Draven",
                            "Pool Party Draven", "Beast Hunter Draven", "Draven Draven"
                        };
                    case "Ezreal":
                        return new[]
                        {
                            "Classic", "Nottingham Ezreal", "Striker Ezreal", "Frosted Ezreal",
                            "Explorer Ezreal", "Pulsefire Ezreal", "TPA Ezreal", "Debonair Ezreal",
                            "Ace of Spades Ezreal"
                        };
                    case "Garves":
                        return new[]
                        {
                            "Classic", "Hired Gun Graves", "Jailbreak Graves", "Mafia Graves", "Riot Graves",
                            "Pool Party Graves", "Cutthroat Graves"
                        };
                    case "Jhin":
                        return new[]
                        {
                            "Classic", "High Noon Jhin"
                        };
                    case "Jinx":
                        return new[]
                        {
                            "Classic", "Mafia Jinx", "Firecracker Jinx", "Slayer Jinx"
                        };
                    case "Kalista":
                        return new[]
                        {
                            "Classic", "Blood Moon Kalista", "Championship Kalista"
                        };
                    case "KogMaw":
                        return new[]
                        {
                            "Classic", "Caterpillar Kog'Maw", "Sonoran Kog'Maw", "Monarch Kog'Maw",
                            "Reindeer Kog'Maw", "Lion Dance Kog'Maw", "Deep Sea Kog'Maw",
                            "Jurassic Kog'Maw", "Battlecast Kog'Maw"
                        };
                    case "Lucian":
                        return new[]
                        {
                            "Classic", "Hired Gun Lucian", "Striker Lucian", "PROJECT: Lucian"
                        };
                    case "MissFortune":
                        return new[]
                        {
                            "Classic", "Cowgirl Miss Fortune", "Waterloo Miss Fortune",
                            "Secret Agent Miss Fortune", "Candy Cane Miss Fortune", "Road Warrior Miss Fortune",
                            "Mafia Miss Fortune", "Arcade Miss Fortune", "Captain Fortune"
                        };
                    case "Quinn":
                        return new[]
                        {
                            "Classic", "Phoenix Quinn", "Woad Scout Quinn", "Corsair Quinn"
                        };
                    case "Sivir":
                        return new[]
                        {
                            "Classic", "Warrior Princess Sivir", "Spectacular Sivir",
                            "Huntress Sivir", "Bandit Sivir", "PAX Sivir", "Snowstorm Sivir",
                            "Warden Sivir", "Victorious Sivir"
                        };
                    case "Tristana":
                        return new[]
                        {
                            "Classic", "Riot Girl Tristana", "Earnest Elf Tristana",
                            "Firefighter Tristana", "Guerilla Tristana", "Buccaneer Tristana",
                            "Rocket Girl Tristana", "Dragon Trainer Tristana"
                        };
                    case "Twitch":
                        return new[]
                        {
                            "Classic", "Kingpin Twitch", "Whistler Village Twitch", "Medieval Twitch",
                            "Gangster Twitch", "Vandal Twitch", "Pickpocket Twitch", "SSW Twitch"
                        };
                    case "Urgot":
                        return new[]
                        {
                            "Classic", "Giant Enemy Crabgot", "Butcher Urgot", "Battlecast Urgot"
                        };
                    case "Varus":
                        return new[]
                        {
                            "Classic", "Blight Crystal Varus", "Arclight Varus", "Arctic Ops Varus",
                            "Heartseeker Varus", "Varus Swiftbolt", "Dark Star Varus"
                        };
                    case "Vayne":
                        return new[]
                        {
                            "Classic", "Vindicator Vayne", "Aristocrat Vayne", "Dragonslayer Vayne",
                            "Heartseeker Vayne", "SKT T1 Vayne", "Arclight Vayne"
                        };
                    default:
                        return new[] { "Classic", "1", "2", "3", "4", "5", "6", "7" };
                }
            }
        }

        private static void Main(string[] Args)
        {
            CustomEvents.Game.OnGameLoad += OnGameLoad;
        }

        private static void OnGameLoad(EventArgs Args)
        {
            Me = ObjectManager.Player;

            SkinID = Me.BaseSkinId;

            if (Menu.GetMenu("Activator", "activator") == null &&
                Menu.GetMenu("ElUtilitySuite", "ElUtilitySuite") == null &&
                Menu.GetMenu("MActivator", "masterActivator") == null)
            {
                EnableActivator = false;
            }
            else
            {
                EnableActivator = true;
            }

            Menu = new Menu("Flowers' " + Me.ChampionName, "Flowers' " + Me.ChampionName, true);

            var OrbMenu = Menu.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            Orbwalker = new Orbwalking.Orbwalker(OrbMenu);

            var PredMenu = Menu.AddSubMenu(new Menu("Prediction", "Prediction"));
            {
                PredMenu.AddItem(new MenuItem("SelectPred", "Select Prediction: ", true).SetValue(new StringList(new[]
                {
                    "Common Prediction", "OKTW Prediction", "SDK Prediction", "SPrediction(Need F5 Reload)",
                    "xcsoft AIO Prediction"
                }, 1)));
                PredMenu.AddItem(
                    new MenuItem("SetHitchance", "HitChance: ", true).SetValue(
                        new StringList(new[] { "VeryHigh", "High", "Medium", "Low" })));
                PredMenu.AddItem(new MenuItem("AboutCommonPred", "Common Prediction -> LeagueSharp.Commmon Prediction"));
                PredMenu.AddItem(new MenuItem("AboutOKTWPred", "OKTW Prediction -> Sebby' Prediction"));
                PredMenu.AddItem(new MenuItem("AboutSDKPred", "SDK Prediction -> LeagueSharp.SDKEx Prediction"));
                PredMenu.AddItem(new MenuItem("AboutSPred", "SPrediction -> Shine' Prediction"));
                PredMenu.AddItem(new MenuItem("AboutxcsoftAIOPred", "xcsoft AIO Prediction -> xcsoft ALL In One Prediction"));
            }

            var AutoLevelMenu = Menu.AddSubMenu(new Menu("Auto Levels", "Auto Levels"));
            {
                AutoLevelMenu.AddItem(new MenuItem("LevelsEnable", "Enabled", true).SetValue(false));
                AutoLevelMenu.AddItem(new MenuItem("LevelsAutoR", "Auto Level R", true).SetValue(true));
                AutoLevelMenu.AddItem(new MenuItem("LevelsDelay", "Auto Level Delays", true).SetValue(new Slider(700, 0, 2000)));
                AutoLevelMenu.AddItem(new MenuItem("LevelsLevels", "When Player Level >= Enable!", true).SetValue(new Slider(3, 1, 6)));
                AutoLevelMenu.AddItem(
                    new MenuItem("LevelsMode", "Mode: ", true).SetValue(
                        new StringList(new[]
                            {"Q -> W -> E", "Q -> E -> W", "W -> Q -> E", "W -> E -> Q", "E -> Q -> W", "E -> W -> Q"})));
            }

            var PotionsMenu = Menu.AddSubMenu(new Menu("Auto Potions", "Auto Potions"));
            {
                PotionsMenu.AddItem(new MenuItem("EnablePosition", "Enabled", true).SetValue(EnableActivator));
                PotionsMenu.AddItem(new MenuItem("PositionHp", "When Player HealthPercent <= %", true).SetValue(new Slider(35)));
            }

            var SkinMenu = Menu.AddSubMenu(new Menu("Skin Chance", "Skin Chance"));
            {
                SkinMenu.AddItem(new MenuItem("EnableSkin", "Enabled", true).SetValue(false))
                    .DontSave().ValueChanged += EnbaleSkin;
                SkinMenu.AddItem(
                    new MenuItem("SelectSkin", "Select Skin: ", true).SetValue(
                        new StringList(SkinName)));
            }

            Championmenu = Menu.AddSubMenu(new Menu("Pluging: " + Me.ChampionName, "Pluging: " + Me.ChampionName));
            {
                switch (Me.ChampionName)
                {
                    case "Ashe":
                        var ashe = new Pluging.Ashe();
                        Game.PrintChat("Flowers' " + Me.ChampionName + " Load Succeed! Credit: NightMoon");
                        break;
                    case "Caitlyn":
                        var caitlyn = new Pluging.Caitlyn();
                        Game.PrintChat("Flowers' " + Me.ChampionName + " Load Succeed! Credit: NightMoon");
                        break;
                    case "Corki":
                        var corki = new Pluging.Corki();
                        Game.PrintChat("Flowers' " + Me.ChampionName + " Load Succeed! Credit: NightMoon");
                        break;
                    case "Draven":
                        var draven = new Pluging.Draven();
                        Game.PrintChat("Flowers' " + Me.ChampionName + " Load Succeed! Credit: NightMoon");
                        break;
                    case "Ezreal":
                        var ezreal = new Pluging.Ezreal();
                        Game.PrintChat("Flowers' " + Me.ChampionName + " Load Succeed! Credit: NightMoon");
                        break;
                    case "Garves":
                        var garves = new Pluging.Garves();
                        Game.PrintChat("Flowers' " + Me.ChampionName + " Load Succeed! Credit: NightMoon");
                        break;
                    case "Jhin":
                        var jhin = new Pluging.Jhin();
                        Game.PrintChat("Flowers' " + Me.ChampionName + " Load Succeed! Credit: NightMoon");
                        break;
                    case "Jinx":
                        var jinx = new Pluging.Jinx();
                        Game.PrintChat("Flowers' " + Me.ChampionName + " Load Succeed! Credit: NightMoon");
                        break;
                    case "Kalista":
                        var kalista = new Pluging.Kalista();
                        Game.PrintChat("Flowers' " + Me.ChampionName + " Load Succeed! Credit: NightMoon");
                        break;
                    case "KogMaw":
                        var kogMaw = new Pluging.KogMaw();
                        Game.PrintChat("Flowers' " + Me.ChampionName + " Load Succeed! Credit: NightMoon");
                        break;
                    case "Lucian":
                        var lucian = new Pluging.Lucian();
                        Game.PrintChat("Flowers' " + Me.ChampionName + " Load Succeed! Credit: NightMoon");
                        break;
                    case "MissFortune":
                        var missFortune = new Pluging.MissFortune();
                        Game.PrintChat("Flowers' " + Me.ChampionName + " Load Succeed! Credit: NightMoon");
                        break;
                    case "Quinn":
                        var quinn = new Pluging.Quinn();
                        Game.PrintChat("Flowers' " + Me.ChampionName + " Load Succeed! Credit: NightMoon");
                        break;
                    case "Sivir":
                        var sivir = new Pluging.Sivir();
                        Game.PrintChat("Flowers' " + Me.ChampionName + " Load Succeed! Credit: NightMoon");
                        break;
                    case "Tristana":
                        var tristana = new Pluging.Tristana();
                        Game.PrintChat("Flowers' " + Me.ChampionName + " Load Succeed! Credit: NightMoon");
                        break;
                    case "Twitch":
                        var twitch = new Pluging.Twitch();
                        Game.PrintChat("Flowers' " + Me.ChampionName + " Load Succeed! Credit: NightMoon");
                        break;
                    case "Urgot":
                        var urgot = new Pluging.Urgot();
                        Game.PrintChat("Flowers' " + Me.ChampionName + " Load Succeed! Credit: NightMoon");
                        break;
                    case "Varus":
                        var varus = new Pluging.Varus();
                        Game.PrintChat("Flowers' " + Me.ChampionName + " Load Succeed! Credit: NightMoon");
                        break;
                    case "Vayne":
                        var vayne = new Pluging.Vayne();
                        Game.PrintChat("Flowers' " + Me.ChampionName + " Load Succeed! Credit: NightMoon");
                        break;
                    default:
                        Menu.AddItem(new MenuItem("Not Support!", Me.ChampionName + ": Not Support!", true));
                        Game.PrintChat("Flowers' " + Me.ChampionName + " Not Support! Credit: NightMoon");
                        break;
                }
            }

            Menu.AddItem(new MenuItem("SpaceBar", "   ", true));
            Menu.AddItem(new MenuItem("Credit", "Credit: NightMoon", true));

            Menu.AddToMainMenu();

            Game.OnUpdate += OnUpdate;
            Obj_AI_Base.OnLevelUp += OnLevelUp;
        }

        private static void EnbaleSkin(object obj, OnValueChangeEventArgs Args)
        {
            if (!Args.GetNewValue<bool>())
            {
                ObjectManager.Player.SetSkin(ObjectManager.Player.ChampionName, SkinID);
            }
        }

        private static void OnUpdate(EventArgs Args)
        {
            if (Me.IsDead)
            {
                return;
            }

            if (Menu.Item("EnableSkin", true).GetValue<bool>())
            {
                ObjectManager.Player.SetSkin(ObjectManager.Player.ChampionName,
                    Menu.Item("SelectSkin", true).GetValue<StringList>().SelectedIndex);
            }

            if (Me.InFountain() ||
                Me.Buffs.Any(x => x.Name.ToLower().Contains("recall") || x.Name.ToLower().Contains("teleport")))
            {
                return;
            }

            if (Menu.Item("EnablePosition", true).GetValue<bool>() &&
                Menu.Item("PositionHp").GetValue<Slider>().Value >= Me.HealthPercent)
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

                if (Items.HasItem(2003) && Items.UseItem(2003)) //Health_Potion 
                {
                    return;
                }

                if (Items.HasItem(2009) && Items.UseItem(2009)) //Total_Biscuit_of_Rejuvenation 
                {
                    return;
                }

                if (Items.HasItem(2010) && Items.UseItem(2010)) //Total_Biscuit_of_Rejuvenation2 
                {
                    return;
                }

                if (Items.HasItem(2031) && Items.UseItem(2031)) //Refillable_Potion 
                {
                    return;
                }

                if (Items.HasItem(2032) && Items.UseItem(2032)) //Hunters_Potion 
                {
                    return;
                }

                if (Items.HasItem(2033)) //Corrupting_Potion 
                {
                    Items.UseItem(2033);
                }
            }
        }


        private static void OnLevelUp(Obj_AI_Base sender, EventArgs Args)
        {
            if (!sender.IsMe || !Menu.Item("LevelsEnable", true).GetValue<bool>())
            {
                return;
            }

            if (Menu.Item("LevelsAutoR", true).GetValue<bool>() && (Me.Level == 6 || Me.Level == 11 || Me.Level == 16))
            {
                Me.Spellbook.LevelSpell(SpellSlot.R);
            }

            if (Me.Level >= Menu.Item("LevelsLevels", true).GetValue<Slider>().Value)
            {
                int Delay = Menu.Item("LevelsDelay", true).GetValue<Slider>().Value;

                if (Me.Level < 3)
                {
                    switch (Menu.Item("LevelsMode", true).GetValue<StringList>().SelectedIndex)
                    {
                        case 0:
                            if (Me.Spellbook.GetSpell(SpellSlot.Q).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.Q);
                            else if (Me.Spellbook.GetSpell(SpellSlot.W).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.W);
                            else if (Me.Spellbook.GetSpell(SpellSlot.E).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.E);
                            break;
                        case 1:
                            if (Me.Spellbook.GetSpell(SpellSlot.Q).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.Q);
                            else if (Me.Spellbook.GetSpell(SpellSlot.E).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.E);
                            else if (Me.Spellbook.GetSpell(SpellSlot.W).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.W);
                            break;
                        case 2:
                            if (Me.Spellbook.GetSpell(SpellSlot.W).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.W);
                            else if (Me.Spellbook.GetSpell(SpellSlot.Q).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.Q);
                            else if (Me.Spellbook.GetSpell(SpellSlot.E).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.E);
                            break;
                        case 3:
                            if (Me.Spellbook.GetSpell(SpellSlot.W).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.W);
                            else if (Me.Spellbook.GetSpell(SpellSlot.E).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.E);
                            else if (Me.Spellbook.GetSpell(SpellSlot.Q).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.Q);
                            break;
                        case 4:
                            if (Me.Spellbook.GetSpell(SpellSlot.E).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.E);
                            else if (Me.Spellbook.GetSpell(SpellSlot.Q).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.Q);
                            else if (Me.Spellbook.GetSpell(SpellSlot.W).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.W);
                            break;
                        case 5:
                            if (Me.Spellbook.GetSpell(SpellSlot.E).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.E);
                            else if (Me.Spellbook.GetSpell(SpellSlot.W).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.W);
                            else if (Me.Spellbook.GetSpell(SpellSlot.Q).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.Q);
                            break;
                    }
                }
                else if (Me.Level > 3)
                {
                    switch (Menu.Item("LevelsMode", true).GetValue<StringList>().SelectedIndex)
                    {
                        case 0:
                            if (Me.Spellbook.GetSpell(SpellSlot.Q).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.Q);
                            else if (Me.Spellbook.GetSpell(SpellSlot.W).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.W);
                            else if (Me.Spellbook.GetSpell(SpellSlot.E).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.E);

                            //Q -> W -> E
                            DelayLevels(Delay, SpellSlot.Q);
                            DelayLevels(Delay + 50, SpellSlot.W);
                            DelayLevels(Delay + 100, SpellSlot.E);
                            break;
                        case 1:
                            if (Me.Spellbook.GetSpell(SpellSlot.Q).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.Q);
                            else if (Me.Spellbook.GetSpell(SpellSlot.E).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.E);
                            else if (Me.Spellbook.GetSpell(SpellSlot.W).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.W);

                            //Q -> E -> W
                            DelayLevels(Delay, SpellSlot.Q);
                            DelayLevels(Delay + 50, SpellSlot.E);
                            DelayLevels(Delay + 100, SpellSlot.W);
                            break;
                        case 2:
                            if (Me.Spellbook.GetSpell(SpellSlot.W).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.W);
                            else if (Me.Spellbook.GetSpell(SpellSlot.Q).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.Q);
                            else if (Me.Spellbook.GetSpell(SpellSlot.E).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.E);

                            //W -> Q -> E
                            DelayLevels(Delay, SpellSlot.W);
                            DelayLevels(Delay + 50, SpellSlot.Q);
                            DelayLevels(Delay + 100, SpellSlot.E);
                            break;
                        case 3:
                            if (Me.Spellbook.GetSpell(SpellSlot.W).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.W);
                            else if (Me.Spellbook.GetSpell(SpellSlot.E).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.E);
                            else if (Me.Spellbook.GetSpell(SpellSlot.Q).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.Q);

                            //W -> E -> Q
                            DelayLevels(Delay, SpellSlot.W);
                            DelayLevels(Delay + 50, SpellSlot.E);
                            DelayLevels(Delay + 100, SpellSlot.Q);
                            break;
                        case 4:
                            if (Me.Spellbook.GetSpell(SpellSlot.E).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.E);
                            else if (Me.Spellbook.GetSpell(SpellSlot.Q).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.Q);
                            else if (Me.Spellbook.GetSpell(SpellSlot.W).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.W);

                            //E -> Q -> W
                            DelayLevels(Delay, SpellSlot.E);
                            DelayLevels(Delay + 50, SpellSlot.Q);
                            DelayLevels(Delay + 100, SpellSlot.W);
                            break;
                        case 5:
                            if (Me.Spellbook.GetSpell(SpellSlot.E).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.E);
                            else if (Me.Spellbook.GetSpell(SpellSlot.W).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.W);
                            else if (Me.Spellbook.GetSpell(SpellSlot.Q).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.Q);

                            //E -> W -> Q
                            DelayLevels(Delay, SpellSlot.E);
                            DelayLevels(Delay + 50, SpellSlot.W);
                            DelayLevels(Delay + 100, SpellSlot.Q);
                            break;
                    }
                }
            }
        }

        private static void DelayLevels(int time, SpellSlot slot)
        {
            Utility.DelayAction.Add(time, () => ObjectManager.Player.Spellbook.LevelSpell(slot));
        }
    }
}
