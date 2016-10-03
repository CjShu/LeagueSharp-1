namespace Flowers_Jinx
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using LeagueSharp;
    using LeagueSharp.Common;

    internal class Program
    {
        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;
        public static Menu Menu;
        public static Obj_AI_Hero Me;
        public static Orbwalking.Orbwalker Orbwalker;
        public static HpBarDraw HpBarDraw = new HpBarDraw();

        private static void Main(string[] Args)
        {
            CustomEvents.Game.OnGameLoad += OnGameLoad;
        }

        private static void OnGameLoad(EventArgs Args)
        {
            if (ObjectManager.Player.ChampionName != "Jinx")
            {
                return;
            }

            Me = ObjectManager.Player;

            Menu = new Menu("Flowers' Jinx", "Flowers' Jinx", true);

            var OrbMenu = Menu.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            Orbwalker = new Orbwalking.Orbwalker(OrbMenu);

            var ComboMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            {
                ComboMenu.AddItem(new MenuItem("ComboQ", "Q Mode:", true).SetValue(new StringList(new[] {"Simple", "Logic"}, 1)));
            }

            Menu.AddToMainMenu();
        }
    }
}
