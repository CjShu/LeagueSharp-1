namespace xSaliceResurrected.Managers
{
    using LeagueSharp;
    using LeagueSharp.Common;

    public static class ManaManager
    {
        public static void AddManaManagertoMenu(Menu myMenu, string source, int standard)
        {
            myMenu.AddItem(new MenuItem(source + "_Manamanager", "Mana Manager", true).SetValue(new Slider(standard)));
        }

        public static bool FullManaCast()
        {
            return ObjectManager.Player.Mana >= SpellManager.QSpell.ManaCost + SpellManager.WSpell.ManaCost + SpellManager.ESpell.ManaCost + SpellManager.RSpell.ManaCost;
        }

        public static bool HasMana(string source)
        {
            return ObjectManager.Player.ManaPercent > Champion.Menu.Item(source + "_Manamanager", true).GetValue<Slider>().Value;
        }
    }
}
