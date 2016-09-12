namespace xSaliceResurrected
{
    using System;
    using LeagueSharp.Common;

    public class Program
    {
        public static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += LoadReligion;
        }

        public static void LoadReligion(EventArgs args)
        {
            var champs = new Champion(true);
        }
    }
}
