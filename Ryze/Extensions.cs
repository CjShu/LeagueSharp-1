namespace Flowers_Ryze
{
    using LeagueSharp.Common;

    public static class Extensions
    {
        public static bool GetBool(this Menu Menu, string MenuItemName)
        {
            return Menu.Item("NightMoon." + MenuItemName, true).GetValue<bool>();
        }

        public static bool GetKey(this Menu Menu, string MenuItemName)
        {
            return Menu.Item("NightMoon." + MenuItemName, true).GetValue<KeyBind>().Active;
        }

        public static int GetSlider(this Menu Menu, string MenuItemName)
        {
            return Menu.Item("NightMoon." + MenuItemName, true).GetValue<Slider>().Value;
        }

        public static int GetList(this Menu Menu, string MenuItemName)
        {
            return Menu.Item("NightMoon." + MenuItemName, true).GetValue<StringList>().SelectedIndex;
        }

        public static System.Drawing.Color GetColor(this Menu Menu, string MenuItemName)
        {
            return Menu.Item("NightMoon." + MenuItemName, true).GetValue<Circle>().Color;
        }

        public static bool GetDraw(this Menu Menu, string MenuItemName)
        {
            return Menu.Item("NightMoon." + MenuItemName, true).GetValue<Circle>().Active;
        }
    }
}
