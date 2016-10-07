namespace Common_Orbwalker
{
    using System;
    using LeagueSharp.Common;

    internal class Program
    {
        private static Menu Menu;
        private static Orbwalking.Orbwalker Orbwalker;

        private static void Main(string[] Args)
        {
            CustomEvents.Game.OnGameLoad += OnGameLoad;
        }

        private static void OnGameLoad(EventArgs Args)
        {
            Menu = new Menu("Common Orbwalker", "Common Orbwalker", true);

            var orbMenu = Menu.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            Orbwalker = new Orbwalking.Orbwalker(orbMenu);

            Menu.AddToMainMenu();
        }
    }
}
