namespace Flowers_Riven
{
    using LeagueSharp.Common;

    internal class InitMenu
    {
        public static void Init()
        {
            Program.Menu = new Menu("Flowers' Riven", "Flowers' Riven", true);

            Program.Menu.AddSubMenu(new Menu("Orbwalking", "Orbwalker.Menu"));
            Program.Orbwalker = new Orbwalking.Orbwalker(Program.Menu.SubMenu("Orbwalker.Menu"));

            var ComboMenu = Program.Menu.AddSubMenu(new Menu("Combo", "Combo"));
            {
                ComboMenu.AddItem(new MenuItem("ComboW", "Use W", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("ComboE", "Use E Mode: ", true).SetValue(new StringList(new[] { "To Target", "To Mouse", "Off" })));
                ComboMenu.AddItem(new MenuItem("ComboR", "Use R", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("R1Combo", "Use R1", true).SetValue(new KeyBind('L', KeyBindType.Toggle, true)));
                ComboMenu.AddItem(new MenuItem("R2Mode", "Use R2 Mode: ", true).SetValue(new StringList(new[] { "Killable", "Max Damage", "First Cast", "Off" }, 1)));
                ComboMenu.AddItem(new MenuItem("Brust Setting", "Brust Setting"));
                ComboMenu.AddItem(new MenuItem("BurstFlash", "Use Flash", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("BurstIgnite", "Use Ignite", true).SetValue(true));
            }

            var HarassMenu = Program.Menu.AddSubMenu(new Menu("Harass", "Harass"));
            {
                HarassMenu.AddItem(new MenuItem("HarassQ", "Use Q", true).SetValue(true));
                HarassMenu.AddItem(new MenuItem("HarassW", "Use W", true).SetValue(true));
                HarassMenu.AddItem(new MenuItem("HarassE", "Use E", true).SetValue(true));
            }

            var LaneClearMenu = Program.Menu.AddSubMenu(new Menu("LaneClear", "LaneClear"));
            {
                LaneClearMenu.AddItem(new MenuItem("LaneClearQ", "Use Q", true).SetValue(true));
                LaneClearMenu.AddItem(new MenuItem("LaneClearW", "Use W", true).SetValue(true));
            }

            var JungleClearMenu = Program.Menu.AddSubMenu(new Menu("JungleClear", "JungleClear"));
            {
                JungleClearMenu.AddItem(new MenuItem("JungleClearQ", "Use Q", true).SetValue(true));
                JungleClearMenu.AddItem(new MenuItem("JungleClearW", "Use W", true).SetValue(true));
                JungleClearMenu.AddItem(new MenuItem("JungleClearE", "Use E", true).SetValue(true));
            }

            var KillStealMenu = Program.Menu.AddSubMenu(new Menu("KillSteal", "KillSteal"));
            {
                KillStealMenu.AddItem(new MenuItem("KillStealW", "Use W", true).SetValue(true));
                KillStealMenu.AddItem(new MenuItem("KillStealE", "Use E", true).SetValue(true));
                KillStealMenu.AddItem(new MenuItem("KillStealR", "Use R", true).SetValue(true));
            }

            var MiscMenu = Program.Menu.AddSubMenu(new Menu("Misc", "Misc"));
            {
                MiscMenu.AddItem(new MenuItem("Q Setting", "Q Setting"));
                MiscMenu.AddItem(new MenuItem("KeepQALive", "Keep Q alive", true).SetValue(true));
                MiscMenu.AddItem(new MenuItem("Dance", "Dance Emote in QA", true).SetValue(false));
                MiscMenu.AddItem(new MenuItem("DC", "Dance Delay", true).SetValue(new Slider(100, 0, 200)));
                MiscMenu.AddItem(new MenuItem("W Setting", "W Setting"));
                MiscMenu.AddItem(new MenuItem("AntiGapCloserW", "AntiGapCloser", true).SetValue(true));
                MiscMenu.AddItem(new MenuItem("InterruptTargetW", "Interrupt Danger Spell", true).SetValue(true));
            }

            var EvadeMenu = Program.Menu.AddSubMenu(new Menu("Evade", "Evade"));
            {
                Evade.Program.InjectEvade();
            }

            var SkinMenu = Program.Menu.AddSubMenu(new Menu("SkinChance", "SkinChance"));
            {
                SkinMenu.AddItem(new MenuItem("EnableSkin", "Enabled", true).SetValue(false)).ValueChanged += Program.EnbaleSkin;
                SkinMenu.AddItem(new MenuItem("SelectSkin", "Select Skin: ", true).SetValue(new StringList(new[] { "Classic", "Redeemed Riven", "Crimson Elite Riven", "Battle Bunny Riven", "Championship Riven", "Dragonblade Riven", "Arcade Riven" })));
            }

            var DrawMenu = Program.Menu.AddSubMenu(new Menu("Draw", "Drawings"));
            {
                DrawMenu.AddItem(new MenuItem("drawingW", "W Range", true).SetValue(false));
                DrawMenu.AddItem(new MenuItem("BrustMinRange", "Burst Min Range", true).SetValue(true));
                DrawMenu.AddItem(new MenuItem("BrustMaxRange", "Burst Max Range", true).SetValue(true));
                DrawMenu.AddItem(new MenuItem("QuickHarassRange", "Quick Harass Range", true).SetValue(true));
                DrawMenu.AddItem(new MenuItem("DrawDamage", "Draw Combo Damage", true).SetValue(true));
                DrawMenu.AddItem(new MenuItem("ShowR1", "Show R1 Status", true).SetValue(true));
                DrawMenu.AddItem(new MenuItem("ShowBurst", "Show Burst Status", true).SetValue(true));
            }

            Program.Menu.AddItem(new MenuItem("Credit", "Credit : NightMoon"));

            Program.Menu.AddToMainMenu();
        }
    }
}
