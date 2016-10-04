namespace Flowers_ADC_Series.Pluging
{
    using System;
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;
    using Color = System.Drawing.Color;
    using Orbwalking = Orbwalking;
    using static Common;

    internal class Draven
    {
        private static Spell Q;
        private static Spell W;
        private static Spell E;
        private static Spell R;

        private static readonly Menu Menu = Program.Championmenu;
        private static readonly Obj_AI_Hero Me = Program.Me;
        private static readonly Orbwalking.Orbwalker Orbwalker = Program.Orbwalker;

        private static HpBarDraw HpBarDraw = new HpBarDraw();

        public Draven()
        {
            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 950);
            R = new Spell(SpellSlot.R, 3000f);

            E.SetSkillshot(0.25f, 100, 1400, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.4f, 160, 2000, false, SkillshotType.SkillshotLine);


        }
    }
}
