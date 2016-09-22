namespace Flowers_Riven
{
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;
    using Color = System.Drawing.Color;

    internal class Drawings
    {
        public static HpBarDraw DrawHpBar = new HpBarDraw();

        public static void Init()
        {
            Drawing.OnDraw += delegate
            {
                if (Program.Menu.Item("drawingW", true).GetValue<bool>() && Program.W.IsReady())
                {
                    Render.Circle.DrawCircle(Program.Me.Position, Program.W.Range, Color.FromArgb(3, 136, 253));
                }

                if (Program.Menu.Item("BrustMaxRange", true).GetValue<bool>() && Program.Me.Level >= 6 && Program.R.IsReady())
                {
                    if (Program.E.IsReady() && Program.Flash != SpellSlot.Unknown && Program.Flash.IsReady())
                        Render.Circle.DrawCircle(Program.Me.Position, 465 + Program.E.Range, Color.FromArgb(253, 3, 3));
                }

                if (Program.Menu.Item("BrustMinRange", true).GetValue<bool>() && Program.Me.Level >= 6 && Program.R.IsReady())
                {
                    if (Program.E.IsReady() && Program.Flash != SpellSlot.Unknown && Program.Flash.IsReady())
                        Render.Circle.DrawCircle(Program.Me.Position, Program.E.Range + Program.Me.BoundingRadius, Color.FromArgb(243, 253, 3));
                }

                if (Program.Menu.Item("DrawDamage", true).GetValue<bool>())
                {
                    foreach (var e in ObjectManager.Get<Obj_AI_Hero>().Where(e => e.IsValidTarget() && !e.IsZombie))
                    {
                        DrawHpBar.Unit = e;
                        DrawHpBar.DrawDmg((float)GetComboDamage(e), new ColorBGRA(255, 204, 0, 170));
                    }
                }

                if (Program.Menu.Item("QuickHarassRange", true).GetValue<bool>())
                {
                    Render.Circle.DrawCircle(Program.Me.Position, Program.E.Range + Program.Me.BoundingRadius, Color.FromArgb(237, 7, 246));
                }

                if (Program.Menu.Item("ShowR1", true).GetValue<bool>())
                {
                    var text = "";
                    if (Program.Menu.Item("R1Combo", true).GetValue<KeyBind>().Active)
                        text = "Enable";
                    if (!Program.Menu.Item("R1Combo", true).GetValue<KeyBind>().Active)
                        text = "Off";
                    Drawing.DrawText(Program.Me.HPBarPosition.X + 30, Program.Me.HPBarPosition.Y - 40, Color.Red, "Use R1: ");
                    Drawing.DrawText(Program.Me.HPBarPosition.X + 90, Program.Me.HPBarPosition.Y - 40, Color.FromArgb(238, 242, 7), text);
                    Program.Menu.Item("R1Combo", true).Permashow();
                }

                if (Program.Menu.Item("ShowBurst", true).GetValue<bool>())
                {
                    foreach (var e in HeroManager.Enemies.Where(e => e.IsValidTarget()))
                    {
                        var text = "";
                        var text2 = "";
                        var Mepos = Drawing.WorldToScreen(Program.Me.Position);
                        var hero = TargetSelector.GetSelectedTarget();

                        if (hero == null)
                        {
                            text = "Lock Target Is Null!";
                        }

                        if (hero != null)
                        {
                            text = "Lock Target is : " + hero.ChampionName;
                            text2 = "Can Flash : " + Program.CanFlash;
                        }

                        if (Program.Menu.Item("BurstFlash", true).GetValue<bool>() && Program.Flash != SpellSlot.Unknown && Program.Flash.IsReady() && e.Distance(Program.Me.ServerPosition) <= 800 && e.Distance(Program.Me.ServerPosition) >= Program.E.Range + Program.Me.AttackRange + 85)
                        {
                            Program.CanFlash = true;
                        }
                        else
                        {
                            Program.CanFlash = false;
                        }

                        Drawing.DrawText(Mepos[0] - 20, Mepos[1], Color.Red, text);
                        Drawing.DrawText(Mepos[0] - 20, Mepos[1] + 14, Color.GreenYellow, text2);
                    }
                }
            };
        }

        private static double GetComboDamage(Obj_AI_Hero e)
        {
            //Thanks Asuvril
            double passive = 0;

            if (Program.Me.Level == 18)
                passive = 0.5;
            else if (Program.Me.Level >= 15)
                passive = 0.45;
            else if (Program.Me.Level >= 12)
                passive = 0.4;
            else if (Program.Me.Level >= 9)
                passive = 0.35;
            else if (Program.Me.Level >= 6)
                passive = 0.3;
            else if (Program.Me.Level >= 3)
                passive = 0.25;
            else
                passive = 0.2;
            double damage = 0;

            if (Program.Q.IsReady())
            {
                var qhan = 3 - Program.QStack;
                damage += Program.Q.GetDamage(e) * qhan + Program.Me.GetAutoAttackDamage(e) * qhan * (1 + passive);
            }

            if (Program.W.IsReady())
                damage += Program.W.GetDamage(e);

            if (Program.R.IsReady())
                if (Program.Me.HasBuff("RivenFengShuiEngine"))
                    damage += Program.Me.CalcDamage(e, Damage.DamageType.Physical, (new double[] { 80, 120, 160 }[Program.R.Level - 1] + 0.6 * Program.Me.FlatPhysicalDamageMod) * (1 + (e.MaxHealth - e.Health) / e.MaxHealth > 0.75 ? 0.75 : (e.MaxHealth - e.Health) / e.MaxHealth) * 8 / 3);

            return damage;
        }
    }
}