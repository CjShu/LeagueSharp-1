namespace Flowers_Caitlyn
{
    using System;
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;
    using Color = System.Drawing.Color;
    using static Common;

    internal class Program
    {
        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;
        public static Menu Menu;
        public static Obj_AI_Hero Me;
        public static bool CanCastR;
        public static int LastQTime;
        public static int LastWTime;
        public static Orbwalking.Orbwalker Orbwalker;
        public static HpBarDraw HpBarDraw = new HpBarDraw();

        private static void Main(string[] Args)
        {
            CustomEvents.Game.OnGameLoad += OnGameLoad;
        }

        private static void OnGameLoad(EventArgs Args)
        {
            if (ObjectManager.Player.ChampionName != "Caitlyn")
            {
                return;
            }

            Me = ObjectManager.Player;

            Q = new Spell(SpellSlot.Q, 1250f);
            W = new Spell(SpellSlot.W, 800f);
            E = new Spell(SpellSlot.E, 750f);
            R = new Spell(SpellSlot.R, 2000f);

            Q.SetSkillshot(0.50f, 50f, 2000f, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.25f, 60f, 1600f, true, SkillshotType.SkillshotLine);

            Menu = new Menu("Flowers' Caitlyn", "Flowers' Caitlyn", true);

            var OrbMenu = Menu.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            Orbwalker = new Orbwalking.Orbwalker(OrbMenu);

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

            var FleeMenu = Menu.AddSubMenu(new Menu("Flee", "Flee"));
            {
                FleeMenu.AddItem(new MenuItem("FleeE", "Use E", true).SetValue(true));
            }

            var MiscMenu = Menu.AddSubMenu(new Menu("Misc", "Misc"));
            {
                MiscMenu.AddItem(
                    new MenuItem("EQKey", "One Key EQ target", true).SetValue(new KeyBind('T', KeyBindType.Press)));
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

            Menu.AddToMainMenu();

            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
            Drawing.OnEndScene += OnEndScene;
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (sender.IsEnemy && sender is Obj_AI_Turret && Args.Target.IsMe)
            {
                CanCastR = false;
            }
            else
            {
                CanCastR = true;   
            }

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

        private static void OnUpdate(EventArgs args)
        {
            if (Me.IsDead || Me.IsRecalling())
            {
                return;
            }

            R.Range = 500 * (R.Level == 0 ? 1 : R.Level) + 1500;

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
                case Orbwalking.OrbwalkingMode.LastHit:
                    break;
                case Orbwalking.OrbwalkingMode.Freeze:
                    break;
                case Orbwalking.OrbwalkingMode.Flee:
                    Flee();
                    break;
                case Orbwalking.OrbwalkingMode.CustomMode:
                    break;
                case Orbwalking.OrbwalkingMode.None:
                    if (Menu.Item("EQKey", true).GetValue<KeyBind>().Active)
                    {
                        OneKeyEQ();
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void KillSteal()
        {
   
        }

        private static void Combo()
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
                            if (target.IsMelee && target.DistanceToPlayer() < 280)
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
                                      Vector3.Normalize(target.ServerPosition - Me.ServerPosition)*150;

                            W.Cast(Pos);
                        }
                    }
                }
            }
        }

        private static void Harass()
        {
         
        }

        private static void LaneClear()
        {
            
        }

        private static void JungleClear()
        {
           
        }

        private static void Flee()
        {
            if (Menu.Item("FleeE", true).GetValue<bool>() && E.IsReady())
            {
                E.Cast(Me.Position - (Game.CursorPos - Me.Position));
            }
        }

        private static void OneKeyEQ()
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

        private static void OnDraw(EventArgs Args)
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

        private static void OnEndScene(EventArgs Args)
        {
            if (!Me.IsDead && !MenuGUI.IsShopOpen && !MenuGUI.IsChatOpen && !MenuGUI.IsScoreboardOpen)
            {
#pragma warning disable 618
                if (Menu.Item("DrawRMin", true).GetValue<bool>() && R.IsReady())
                    Utility.DrawCircle(Me.Position, R.Range, Color.FromArgb(14, 194, 255), 1, 30, true);
#pragma warning restore 618
            }
        }

        private static double ComboDamage(Obj_AI_Hero target)
        {
            if (target != null && target.IsValidTarget())
            {
                var Damage = 0d;

                Damage += Me.GetAutoAttackDamage(target);

                if (Q.IsReady())
                {
                    Damage += Me.Spellbook.GetSpell(SpellSlot.Q).IsReady() ? Me.GetSpellDamage(target, SpellSlot.Q) : 0d;
                }

                if (E.IsReady())
                {
                    Damage += Me.Spellbook.GetSpell(SpellSlot.E).IsReady() ? Me.GetSpellDamage(target, SpellSlot.E) : 0d;
                }

                if (R.IsReady())
                {
                    Damage += Me.Spellbook.GetSpell(SpellSlot.R).IsReady() ? Me.GetSpellDamage(target, SpellSlot.R) * Menu.Item("RMenuKill", true).GetValue<Slider>().Value : 0d;
                }

                if (target.ChampionName == "Moredkaiser")
                    Damage -= target.Mana;

                // exhaust
                if (Me.HasBuff("SummonerExhaust"))
                    Damage = Damage * 0.6f;

                // blitzcrank passive
                if (target.HasBuff("BlitzcrankManaBarrierCD") && target.HasBuff("ManaBarrier"))
                    Damage -= target.Mana / 2f;

                // kindred r
                if (target.HasBuff("KindredRNoDeathBuff"))
                    Damage = 0;

                // tryndamere r
                if (target.HasBuff("UndyingRage") && target.GetBuff("UndyingRage").EndTime - Game.Time > 0.3)
                    Damage = 0;

                // kayle r
                if (target.HasBuff("JudicatorIntervention"))
                    Damage = 0;

                // zilean r
                if (target.HasBuff("ChronoShift") && target.GetBuff("ChronoShift").EndTime - Game.Time > 0.3)
                    Damage = 0;

                // fiora w
                if (target.HasBuff("FioraW"))
                    Damage = 0;

                return Damage;
            }
            return 0d;
        }
    }
}
