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

    internal class Caitlyn
    {
        private static Spell Q;
        private static Spell W;
        private static Spell E;
        private static Spell R;

        private static int LastQTime;
        private static int LastWTime;

        private static readonly Menu Menu = Program.Championmenu;
        private static readonly Obj_AI_Hero Me = Program.Me;
        private static readonly Orbwalking.Orbwalker Orbwalker = Program.Orbwalker;

        private static HpBarDraw HpBarDraw = new HpBarDraw();

        public Caitlyn()
        {
            Q = new Spell(SpellSlot.Q, 1250f);
            W = new Spell(SpellSlot.W, 800f);
            E = new Spell(SpellSlot.E, 750f);
            R = new Spell(SpellSlot.R, 2000f);

            Q.SetSkillshot(0.50f, 50f, 2000f, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.25f, 60f, 1600f, true, SkillshotType.SkillshotLine);

            var ComboMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            {
                ComboMenu.AddItem(new MenuItem("ComboQ", "Use Q", true).SetValue(true));
                ComboMenu.AddItem(
                    new MenuItem("ComboQRange", "Use Q| Min Range >= x", true).SetValue(new Slider(700, 500, 1250)));
                ComboMenu.AddItem(new MenuItem("ComboW", "Use W", true).SetValue(true));
                ComboMenu.AddItem(
                    new MenuItem("ComboWCount", "Use W| Min Count >= x", true).SetValue(new Slider(1, 1, 3)));
                ComboMenu.AddItem(new MenuItem("ComboE", "Use E", true).SetValue(true));

            }

            var HarassMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));
            {
                HarassMenu.AddItem(new MenuItem("", "", true));
                HarassMenu.AddItem(new MenuItem("", "", true));
            }

            var KillStealMenu = Menu.AddSubMenu(new Menu("KillSteal", "KillSteal"));
            {
                KillStealMenu.AddItem(new MenuItem("KillStealQ", "Use Q", true).SetValue(true));
            }

            var FleeMenu = Menu.AddSubMenu(new Menu("Flee", "Flee"));
            {
                FleeMenu.AddItem(new MenuItem("FleeE", "Use E", true).SetValue(true));
            }

            var RMenu = Menu.AddSubMenu(new Menu("R Menu", "R Menu"));
            {
                RMenu.AddItem(new MenuItem("SemiR", "Semi-manual R Key", true).SetValue(new KeyBind('T', KeyBindType.Press)));
            }

            var MiscMenu = Menu.AddSubMenu(new Menu("Misc", "Misc"));
            {
                MiscMenu.AddItem(
                    new MenuItem("EQKey", "One Key EQ target", true).SetValue(new KeyBind('G', KeyBindType.Press)));
            }

            var DrawMenu = Menu.AddSubMenu(new Menu("Drawings", "Drawings"));
            {
                DrawMenu.AddItem(new MenuItem("DrawQ", "Draw Q Range", true).SetValue(false));
                DrawMenu.AddItem(new MenuItem("DrawW", "Draw W Range", true).SetValue(false));
                DrawMenu.AddItem(new MenuItem("DrawE", "Draw E Range", true).SetValue(false));
                DrawMenu.AddItem(new MenuItem("DrawR", "Draw R Range", true).SetValue(false));
                DrawMenu.AddItem(new MenuItem("DrawRMin", "Draw R Range(MinMap)", true).SetValue(false));
                DrawMenu.AddItem(new MenuItem("DrawDamage", "Draw ComboDamage", true).SetValue(true));
            }

            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
            Drawing.OnEndScene += OnEndScene;
        }

        private void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (!sender.IsMe)
                return;

            if (Args.Slot == SpellSlot.Q)
            {
                LastQTime = Utils.TickCount;
            }

            if (Args.Slot == SpellSlot.W)
            {
                LastWTime = Utils.TickCount;
            }
        }

        private void OnUpdate(EventArgs Args)
        {
            R.Range = 500 * R.Level + 1500;

            if (Me.IsDead || Me.IsRecalling())
            {
                return;
            }

            KillSteal();

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    LaneClear();
                    JungleClear();
                    break;
                case Orbwalking.OrbwalkingMode.Flee:
                    Flee();
                    break;
                case Orbwalking.OrbwalkingMode.None:
                    if (Menu.Item("EQKey", true).GetValue<KeyBind>().Active)
                    {
                        OneKeyEQ();
                    }
                    break;
            }
        }

        private void KillSteal()
        {
            if (Menu.Item("KillStealQ", true).GetValue<bool>() && Q.IsReady())
            {
                foreach (
                    var target in HeroManager.Enemies.Where(x => x.IsValidTarget(Q.Range) && CheckTargetSureCanKill(x)))
                {
                    Q.CastTo(target);
                }
            }
        }

        private void Combo()
        {
            var target = TargetSelector.GetSelectedTarget() ??
                         TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);

            if (CheckTarget(target, R.Range))
            {
                if (Menu.Item("ComboE", true).GetValue<bool>() && E.IsReady() && target.IsValidTarget(700) &&
                    E.GetPrediction(target).CollisionObjects.Count == 0 && E.CanCast(target))
                {
                    E.Cast(target);
                }

                if (Menu.Item("ComboQ", true).GetValue<bool>() && Q.IsReady() && target.IsValidTarget(Q.Range) &&
                    target.DistanceToPlayer() >= Menu.Item("ComboQRange", true).GetValue<Slider>().Value)
                {
                    Q.CastTo(target);
                }

                if (Menu.Item("ComboW", true).GetValue<bool>() && W.IsReady() && target.IsValidTarget(W.Range) &&
                    W.Instance.Ammo >= Menu.Item("ComboWCount", true).GetValue<Slider>().Value)
                {
                    if (LastWTime - Utils.TickCount > 1500)
                    {
                        if (target.IsFacing(Me))
                        {
                            if (target.IsMelee && target.DistanceToPlayer() < 250)
                            {
                                W.Cast(Me);
                            }
                            else if (target.IsValidTarget(W.Range))
                            {
                                W.CastTo(target);
                            }
                        }
                        else
                        {
                            var Pos = W.GetPrediction(target).CastPosition +
                                      Vector3.Normalize(target.ServerPosition - Me.ServerPosition) * 150;

                            W.Cast(Pos);
                        }
                    }
                }
            }
        }

        private void Harass()
        {

        }

        private void LaneClear()
        {

        }

        private void JungleClear()
        {

        }

        private void Flee()
        {
            if (Menu.Item("FleeE", true).GetValue<bool>() && E.IsReady())
            {
                E.Cast(Me.Position - (Game.CursorPos - Me.Position));
            }
        }

        private void OneKeyEQ()
        {
            Orbwalking.MoveTo(Game.CursorPos);

            if (E.IsReady() && Q.IsReady())
            {
                var target = TargetSelector.GetSelectedTarget() ??
                    TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);

                if (CheckTarget(target, E.Range))
                {
                    if (target.Health >= Q.GetDamage(target) + E.GetDamage(target) &&
                        E.GetPrediction(target).CollisionObjects.Count == 0 && E.CanCast(target))
                    {
                        E.Cast(target);
                        Q.CastTo(target);
                    }
                }
            }
        }

        private void OnDraw(EventArgs Args)
        {
            if (!Me.IsDead && !MenuGUI.IsShopOpen && !MenuGUI.IsChatOpen && !MenuGUI.IsScoreboardOpen)
            {
                if (Menu.Item("DrawQ", true).GetValue<bool>() && Q.IsReady())
                {
                    Render.Circle.DrawCircle(Me.Position, Q.Range, Color.Green, 1);
                }

                if (Menu.Item("DrawW", true).GetValue<bool>() && W.Level > 0)
                {
                    Render.Circle.DrawCircle(Me.Position, W.Range, Color.FromArgb(9, 253, 242), 1);
                }

                if (Menu.Item("DrawE", true).GetValue<bool>() && E.IsReady())
                {
                    Render.Circle.DrawCircle(Me.Position, E.Range, Color.FromArgb(188, 6, 248), 1);
                }

                if (Menu.Item("DrawR", true).GetValue<bool>() && R.IsReady())
                {
                    Render.Circle.DrawCircle(Me.Position, R.Range, Color.FromArgb(19, 130, 234), 1);
                }

                if (Menu.Item("DrawDamage", true).GetValue<bool>())
                {
                    foreach (
                        var x in ObjectManager.Get<Obj_AI_Hero>().Where(e => e.IsValidTarget() && !e.IsDead && !e.IsZombie))
                    {
                        HpBarDraw.Unit = x;
                        HpBarDraw.DrawDmg((float)ComboDamage(x), new ColorBGRA(255, 204, 0, 170));
                    }
                }
            }
        }

        private void OnEndScene(EventArgs Args)
        {
            if (!Me.IsDead && !MenuGUI.IsShopOpen && !MenuGUI.IsChatOpen && !MenuGUI.IsScoreboardOpen)
            {
#pragma warning disable 618
                if (Menu.Item("DrawRMin", true).GetValue<bool>() && R.IsReady())
                    Utility.DrawCircle(Me.Position, R.Range, Color.FromArgb(14, 194, 255), 1, 30, true);
#pragma warning restore 618
            }
        }
    }
}
