namespace Flowers_ADC_Series
{
    using System;
    using LeagueSharp;
    using LeagueSharp.Common;

    internal class Program
    {
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
            Me = ObjectManager.Player;

            Menu = new Menu("Flowers' " + Me.ChampionName, "Flowers' " + Me.ChampionName, true);

            var OrbMenu = Menu.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            Orbwalker = new Orbwalking.Orbwalker(OrbMenu);

            switch (Me.ChampionName)
            {
                case "Caitlyn":
                    var caitlyn = new Pluging.Caitlyn();
                    break;
            }

            Menu.AddToMainMenu();
        }
    }
}
