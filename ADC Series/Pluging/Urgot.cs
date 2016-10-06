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

    internal class Urgot
    {
        private static Spell Q;
        private static Spell Q1;
        private static Spell W;
        private static Spell E;
        private static Spell R;

        private static readonly Menu Menu = Program.Championmenu;
        private static readonly Obj_AI_Hero Me = Program.Me;
        private static readonly Orbwalking.Orbwalker Orbwalker = Program.Orbwalker;

        private static HpBarDraw HpBarDraw = new HpBarDraw();

        public Urgot()
        {
            Q = new Spell(SpellSlot.Q, 980f);
            Q1 = new Spell(SpellSlot.Q, 1200f);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 900f);
            R = new Spell(SpellSlot.R, 850f);

            Q.SetSkillshot(0.25f, 60f, 1600f, true, SkillshotType.SkillshotLine);
            Q1.SetSkillshot(0.25f, 60f, 1600f, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.25f, 120f, 1500f, false, SkillshotType.SkillshotCircle);
        }
    }
}
