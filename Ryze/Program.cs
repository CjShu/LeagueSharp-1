namespace Flowers_Ryze
{
    using LeagueSharp;
    using LeagueSharp.Common;
    using System;
    using System.Linq;

    internal class Program
    {
        private static Spell Q, W, E, R;
        private static SpellSlot Ignite = SpellSlot.Unknown;
        private static Menu Menu;
        private static Obj_AI_Hero Me, combotarget = null;
        private static int SkinID = 0;
        private static float SpellTime;
        private static bool CanShield = false;
        private static Orbwalking.Orbwalker Orbwalker;
        private static HpBarDraw DrawHpBar = new HpBarDraw();

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnLoad;
        }

        private static void OnLoad(EventArgs args)
        {
            if (ObjectManager.Player.ChampionName != "Ryze")
                return;

            Me = ObjectManager.Player;

            Game.PrintChat("<font color='#2848c9'>Flowers' Ryze</font> --> <font color='#b756c5'>Version : 1.0.0.3</font> <font size='30'><font color='#d949d4'>Good Luck!</font></font>");

            SkinID = Me.BaseSkinId;

            LoadSpell();
            LoadMenu();
            LoadEvents();
        }

        private static void LoadSpell()
        {
            Q = new Spell(SpellSlot.Q, 1000f);
            W = new Spell(SpellSlot.W, 600f);
            E = new Spell(SpellSlot.E, 600f);
            R = new Spell(SpellSlot.R, 1500f);

            Q.SetSkillshot(0.25f, 50f, 1700, true, SkillshotType.SkillshotLine);
            Q.DamageType = W.DamageType = E.DamageType = TargetSelector.DamageType.Magical;

            Ignite = Me.GetSpellSlot("SummonerDot");
        }

        private static void LoadMenu()
        {
            Menu = new Menu("Flowers' Ryze", "NightMoon", true);

            Menu.AddSubMenu(new Menu("Orbwalking(走砍设置)", "NightMoon.Orbwalking.Menu"));
            Orbwalker = new Orbwalking.Orbwalker(Menu.SubMenu("NightMoon.Orbwalking.Menu"));

            Menu.AddSubMenu(new Menu("Combo(连招设置)", "NightMoon.Combo.Menu"));
            Menu.SubMenu("NightMoon.Combo.Menu").AddItem(new MenuItem("NightMoon.Q.Combo", "Use Q(使用Q)", true).SetValue(true));
            Menu.SubMenu("NightMoon.Combo.Menu").AddItem(new MenuItem("NightMoon.QNoCol.Combo", "Use Q | Smart No Collision(使用Q | 智能无碰撞模式)", true).SetValue(true));
            Menu.SubMenu("NightMoon.Combo.Menu").AddItem(new MenuItem("NightMoon.W.Combo", "Use W(使用W)", true).SetValue(true));
            Menu.SubMenu("NightMoon.Combo.Menu").AddItem(new MenuItem("NightMoon.E.Combo", "Use E(使用E)", true).SetValue(true));
            Menu.SubMenu("NightMoon.Combo.Menu").AddItem(new MenuItem("NightMoon.Ignite.Combo", "Use Ignite(使用点燃)", true).SetValue(true));
            Menu.SubMenu("NightMoon.Combo.Menu").AddItem(new MenuItem("NightMoon.Shield.Combo", "Active Shield(连招触发护盾)", true).SetValue(true));
            Menu.SubMenu("NightMoon.Combo.Menu").AddItem(new MenuItem("NightMoon.Shield.HP", "Player HealthPercent(玩家最低触发生命值)", true).SetValue(new Slider(60)));
            Menu.SubMenu("NightMoon.Combo.Menu").AddItem(new MenuItem("NightMoon.ShieldKey.Combo", "Or Active Key(按此按键启动)", true).SetValue(new KeyBind('T', KeyBindType.Toggle, true)));

            Menu.AddSubMenu(new Menu("Harass(骚扰设置)", "NightMoon.Harass.Menu"));
            Menu.SubMenu("NightMoon.Harass.Menu").AddItem(new MenuItem("NightMoon.Q.Harass", "Use Q(使用Q)", true).SetValue(true));
            Menu.SubMenu("NightMoon.Harass.Menu").AddItem(new MenuItem("NightMoon.W.Harass", "Use W(使用W)", true).SetValue(false));
            Menu.SubMenu("NightMoon.Harass.Menu").AddItem(new MenuItem("NightMoon.E.Harass", "Use E(使用E)", true).SetValue(true));
            Menu.SubMenu("NightMoon.Harass.Menu").AddItem(new MenuItem("NightMoon.HarassMana", "Min Mana Percent(自身最低蓝量比)", true).SetValue(new Slider(40)));

            Menu.AddSubMenu(new Menu("LaneClear(清线设置)", "NightMoon.LaneClear.Menu"));
            Menu.SubMenu("NightMoon.LaneClear.Menu").AddItem(new MenuItem("NightMoon.Q.LaneClear", "Use Q(使用Q)", true).SetValue(true));
            Menu.SubMenu("NightMoon.LaneClear.Menu").AddItem(new MenuItem("NightMoon.W.LaneClear", "Use W(使用W)", true).SetValue(true));
            Menu.SubMenu("NightMoon.LaneClear.Menu").AddItem(new MenuItem("NightMoon.E.LaneClear", "Use E(使用E)", true).SetValue(true));
            Menu.SubMenu("NightMoon.LaneClear.Menu").AddItem(new MenuItem("NightMoon.EMin.LaneClear", "Use E | Min Hit(使用E|最小命中小兵数)", true).SetValue(new Slider(3, 1, 5)));
            Menu.SubMenu("NightMoon.LaneClear.Menu").AddItem(new MenuItem("NightMoon.LaneClearMana", "Min Mana Percent(自身最低蓝量比)", true).SetValue(new Slider(40)));

            Menu.AddSubMenu(new Menu("JungleClear(清野设置)", "NightMoon.JungleClear.Menu"));
            Menu.SubMenu("NightMoon.JungleClear.Menu").AddItem(new MenuItem("NightMoon.Q.JungleClear", "Use Q(使用Q)", true).SetValue(true));
            Menu.SubMenu("NightMoon.JungleClear.Menu").AddItem(new MenuItem("NightMoon.W.JungleClear", "Use W(使用W)", true).SetValue(true));
            Menu.SubMenu("NightMoon.JungleClear.Menu").AddItem(new MenuItem("NightMoon.E.JungleClear", "Use E(使用E)", true).SetValue(true));
            Menu.SubMenu("NightMoon.JungleClear.Menu").AddItem(new MenuItem("NightMoon.JungleClearMana", "Min Mana Percent(自身最低蓝量比)", true).SetValue(new Slider(40)));

            Menu.AddSubMenu(new Menu("LastHit(补刀设置)", "NightMoon.LastHit.Menu"));
            Menu.SubMenu("NightMoon.LastHit.Menu").AddItem(new MenuItem("NightMoon.Q.LastHit", "Use Q(使用Q)", true).SetValue(true));
            Menu.SubMenu("NightMoon.LastHit.Menu").AddItem(new MenuItem("NightMoon.W.LastHit", "Use W(使用W)", true).SetValue(false));
            Menu.SubMenu("NightMoon.LastHit.Menu").AddItem(new MenuItem("NightMoon.E.LastHit", "Use E(使用E)", true).SetValue(false));
            Menu.SubMenu("NightMoon.LastHit.Menu").AddItem(new MenuItem("NightMoon.LastHitMana", "Min Mana Percent(自身最低蓝量比)", true).SetValue(new Slider(40)));

            Menu.AddSubMenu(new Menu("KillSteal(击杀设置)", "NightMoon.KillSteal.Menu"));
            Menu.SubMenu("NightMoon.KillSteal.Menu").AddItem(new MenuItem("NightMoon.Q.KillSteal", "Use Q(使用Q)", true).SetValue(true));
            Menu.SubMenu("NightMoon.KillSteal.Menu").AddItem(new MenuItem("NightMoon.W.KillSteal", "Use W(使用W)", true).SetValue(true));
            Menu.SubMenu("NightMoon.KillSteal.Menu").AddItem(new MenuItem("NightMoon.E.KillSteal", "Use E(使用E)", true).SetValue(true));
            Menu.SubMenu("NightMoon.KillSteal.Menu").AddItem(new MenuItem("NightMoon.Ignite.KillSteal", "Use Ignite(使用点燃)", true).SetValue(true));

            Menu.AddSubMenu(new Menu("Skin(换肤设置)", "NightMoon.Skin.Menu"));
            Menu.SubMenu("NightMoon.Skin.Menu").AddItem(new MenuItem("NightMoon.Enable.Skin", "Enable(启动)", true).SetValue(false)).DontSave();
            Menu.SubMenu("NightMoon.Skin.Menu").AddItem(new MenuItem("NightMoon.Select.Skin", "Select Skin(选择皮肤)", true).SetValue(new StringList(new[] { "Classic", "Young", "Tribal", "Uncle", "Triumphant", "Professor", "Zombie", "Dark Crystal", "Pirate", "Whitebeard" }))).DontSave();

            Menu.AddSubMenu(new Menu("Misc(杂项设置)", "NightMoon.Misc.Menu"));
            Menu.SubMenu("NightMoon.Misc.Menu").AddItem(new MenuItem("NightMoon.W.AntiGap", "Use W to GapCloser(使用W反突进)", true).SetValue(true));
            Menu.SubMenu("NightMoon.Misc.Menu").AddItem(new MenuItem("NightMoon.W.Interrupt", "Use W to Interrupt Spell(使用W打断技能)", true).SetValue(false));
            Menu.SubMenu("NightMoon.Misc.Menu").AddItem(new MenuItem("NightMoon.Packet.Use", "Use Packet(使用封包)", true).SetValue(true));
            Menu.SubMenu("NightMoon.Misc.Menu").AddItem(new MenuItem("NightMoon.Clear.Use", "Clear Use Spell(清线使用技能)", true).SetValue(new KeyBind('G', KeyBindType.Toggle, true)));
            Menu.SubMenu("NightMoon.Misc.Menu").AddItem(new MenuItem("NightMoon.ComboAA.Disable", "Smart Disable Attack In Combo Mode(连招时智能禁止普攻)", true).SetValue(true));
            Menu.SubMenu("NightMoon.Misc.Menu").AddItem(new MenuItem("NightMoon.ComboAA.DisableLevel", "Disable All Attack In Combo Mode (连招时禁止所有普攻)", true).SetValue(new KeyBind('A', KeyBindType.Toggle)));

            Menu.AddSubMenu(new Menu("Drawings(显示设置)", "NightMoon.Drawings.Menu"));
            Menu.SubMenu("NightMoon.Drawings.Menu").AddItem(new MenuItem("NightMoon.Q.Draw", "Q Range(Q 范围)", true).SetValue(false));
            Menu.SubMenu("NightMoon.Drawings.Menu").AddItem(new MenuItem("NightMoon.W.Draw", "W Range(W 范围)", true).SetValue(false));
            Menu.SubMenu("NightMoon.Drawings.Menu").AddItem(new MenuItem("NightMoon.E.Draw", "E Range(E 范围)", true).SetValue(false));
            Menu.SubMenu("NightMoon.Drawings.Menu").AddItem(new MenuItem("NightMoon.R.Draw", "R Range(R 范围)", true).SetValue(false));
            Menu.SubMenu("NightMoon.Drawings.Menu").AddItem(new MenuItem("NightMoon.MinR.Draw", "R Range(Min Map) (R 范围_小地图现实)", true).SetValue(false));
            Menu.SubMenu("NightMoon.Drawings.Menu").AddItem(new MenuItem("NightMoon.Combo.Draw", "Combo Shield Status(连招触发护盾状态)", true).SetValue(true));
            Menu.SubMenu("NightMoon.Drawings.Menu").AddItem(new MenuItem("NightMoon.Damage.Draw", "Combo Damage(连招伤害显示)", true).SetValue(true));

            Menu.AddToMainMenu();
        }

        private static void LoadEvents()
        {
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Game.OnUpdate += OnUpdate;
            Orbwalking.BeforeAttack += BeforeAttack;
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += OnInterruptableTarget;
            Drawing.OnDraw += OnDraw;
            Drawing.OnEndScene += OnEndScene;
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (sender.IsMe)
            {
                if(Args.Slot == SpellSlot.Q || Args.Slot == SpellSlot.W || Args.Slot == SpellSlot.E)
                {
                    SpellTime = Game.Time;
                }
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            R.Range = R.Level * 1500f;

            if (Menu.GetBool("Shield.Combo") && (Menu.GetKey("ShieldKey.Combo") || Me.HealthPercent <= Menu.GetSlider("Shield.HP")))
            {
                CanShield = true;
            }
            else
            {
                CanShield = false;
            }

            if (Menu.GetBool("QNoCol.Combo") && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && (CanShield || (combotarget != null && combotarget.IsValidTarget(600))))
            {
                Q.Collision = false;
            }
            else
            {
                Q.Collision = true;
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                ComboLogic();
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                HarassLogic();
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear && Menu.GetKey("Clear.Use"))
            {
                LaneLogic();
                JungleLogic();
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LastHit)
            {
                LastHitLogic();
            }

            KillSteal();
            Skin();
        }

        private static void CastSpell(bool useQ, bool useW, bool useE)
        {
            if (combotarget.IsValidTarget() && combotarget.IsHPBarRendered)
            {
                if (!CanShield && Game.Time - SpellTime > 0.3)
                {
                    if (useQ && Q.IsReady() && combotarget.IsValidTarget(Q.Range))
                    {
                        var QPred = Q.GetPrediction(combotarget);

                        if (QPred.Hitchance >= HitChance.High)
                        {
                            Q.Cast(QPred.CastPosition, UsePacket);
                        }
                    }

                    if (useW && W.IsReady() && combotarget.IsValidTarget(W.Range))
                    {
                        W.CastOnUnit(combotarget, UsePacket);
                    }

                    if (useE && E.IsReady() && combotarget.IsValidTarget(E.Range))
                    {
                        E.CastOnUnit(combotarget, UsePacket);
                    }
                }
                else if (CanShield)
                {
                    if (useQ && Q.IsReady() && combotarget.IsValidTarget(Q.Range))
                    {
                        var QPred = Q.GetPrediction(combotarget);

                        if (QPred.Hitchance >= HitChance.High)
                        {
                            Q.Cast(QPred.CastPosition, UsePacket);
                        }
                    }

                    if (useW && W.IsReady() && combotarget.IsValidTarget(W.Range) && !Q.IsReady())
                    {
                        W.CastOnUnit(combotarget, UsePacket);
                    }

                    if (useE && E.IsReady() && combotarget.IsValidTarget(E.Range) && !Q.IsReady())
                    {
                        E.CastOnUnit(combotarget, UsePacket);
                    }
                }
            }
        }

        private static void ComboLogic()
        {
            var target = TargetSelector.GetTarget(Q.Range, Q.DamageType);

            if (target.IsValidTarget() && target.IsHPBarRendered)
            {
                combotarget = target;

                CastSpell(Menu.GetBool("Q.Combo"), Menu.GetBool("W.Combo"), Menu.GetBool("E.Combo"));

                if (Menu.GetBool("Ignite.Combo") && Ignite != SpellSlot.Unknown && Ignite.IsReady() && target.HealthPercent < 20)
                {
                    Me.Spellbook.CastSpell(Ignite, target);
                }
            }
        }

        private static void HarassLogic()
        {
            if (Me.ManaPercent >= Menu.GetSlider("HarassMana"))
            {
                var target = TargetSelector.GetTarget(Q.Range, Q.DamageType);
                var minions = MinionManager.GetMinions(Me.Position, E.Range);
                var min = minions.Where(x => x.IsValidTarget(E.Range) && x.Position.Distance(target.Position) <= 300).FirstOrDefault();

                if (target.IsValidTarget() && target.IsHPBarRendered)
                {
                    if (Menu.GetBool("Q.Harass") && target.IsValidTarget(Q.Range) && !Me.HasBuff("ryzeqiconfullcharge"))
                    {
                        var QPred = Q.GetPrediction(target);

                        if (QPred.Hitchance >= HitChance.High)
                        {
                            Q.Cast(QPred.CastPosition, UsePacket);
                        }

                        if (min != null && min.Health < Q.GetDamage(min) && min.HasBuff("RyzeE"))
                        {
                            Q.Cast(min, UsePacket);
                        }
                    }

                    if (Menu.GetBool("E.Harass"))
                    {
                        if (min != null && min.Health < Q.GetDamage(min) + E.GetDamage(min))
                        {
                            E.CastOnUnit(min, UsePacket);
                        }
                        else if (target.IsValidTarget(E.Range))
                        {
                            E.CastOnUnit(target, UsePacket);
                        }
                    }
                }
            }
        }

        private static void LaneLogic()
        {
            if (Me.ManaPercent >= Menu.GetSlider("LaneClearMana"))
            {
                var minions = MinionManager.GetMinions(Me.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Enemy);

                if (minions.Count() > 0)
                {
                    if (Menu.GetBool("E.LaneClear") && E.IsReady())
                    {
                        foreach (var minE in minions.Where(x => x.IsValidTarget(E.Range) && MinionManager.GetMinions(x.Position, 250).Count() >= Menu.GetSlider("EMin.LaneClear")))
                        {
                            if (minE != null)
                            {
                                E.Cast(minE, UsePacket);
                            }
                        }
                    }

                    foreach (var min in minions.Where(x => x.IsValidTarget(600) && x.HasBuff("RyzeE")))
                    {
                        if (min != null)
                        {
                            if (Menu.GetBool("W.LaneClear") && W.IsReady() && min.Health < W.GetDamage(min))
                            {
                                W.CastOnUnit(min, UsePacket);
                            }
                            else if (Menu.GetBool("Q.LaneClear") && Q.IsReady())
                            {
                                Q.Cast(min, UsePacket);
                            }
                        }
                        else if (min == null)
                        {
                            if (Menu.GetBool("Q.LaneClear") && Q.IsReady())
                            {
                                Q.Cast(minions.FirstOrDefault(), UsePacket);
                            }
                        }
                    }
                }
            }
        }

        private static void JungleLogic()
        {
            if (Me.ManaPercent >= Menu.GetSlider("JungleClearMana"))
            {
                var mobs = MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

                if (mobs.Count() > 0)
                {
                    var mob = mobs.FirstOrDefault();

                    if (Menu.GetBool("Q.JungleClear") && Q.IsReady())
                    {
                        Q.Cast(mob, UsePacket);
                        return;
                    }

                    if (Menu.GetBool("E.JungleClear") && E.IsReady())
                    {
                        E.CastOnUnit(mob, UsePacket);
                        return;
                    }

                    if (Menu.GetBool("W.JungleClear") && W.IsReady())
                    {
                        W.CastOnUnit(mob, UsePacket);
                        return;
                    }
                }
            }
        }

        private static void LastHitLogic()
        {
            if (Me.ManaPercent >= Menu.GetSlider("LastHitMana"))
            {
                var Minions = MinionManager.GetMinions(Me.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Enemy);

                if (Minions.Count() > 0)
                {
                    var min = Minions.FirstOrDefault();

                    if (Menu.GetBool("E.LastHit") && E.IsReady() && min.IsValidTarget(E.Range) && min.Health < E.GetDamage(min))
                    {
                        E.Cast(min, UsePacket);
                        return;
                    }

                    if (Menu.GetBool("W.LastHit") && W.IsReady() && min.IsValidTarget(W.Range) && min.Health < W.GetDamage(min))
                    {
                        W.Cast(min, UsePacket);
                        return;
                    }

                    if (Menu.GetBool("Q.LastHit") && Q.IsReady() && !Me.HasBuff("ryzeqiconfullcharge") && min.IsValidTarget(Q.Range) && min.Health < Q.GetDamage(min))
                    {
                        Q.Cast(min, UsePacket);
                        return;
                    }
                }
            }
        }

        private static void KillSteal()
        {
            foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(Q.Range) && !x.IsDead && !x.IsZombie && x.IsHPBarRendered))
            {
                if (Menu.GetBool("Q.KillSteal") && Q.IsReady() && target.Health < Q.GetDamage(target) && target.IsValidTarget(Q.Range))
                {
                    var QPred = Q.GetPrediction(target);

                    if (QPred.Hitchance >= HitChance.VeryHigh)
                    {
                        Q.Cast(QPred.CastPosition);
                    }
                }

                if (Menu.GetBool("W.KillSteal") && W.IsReady() && target.Health < W.GetDamage(target) && target.IsValidTarget(W.Range))
                {
                    W.CastOnUnit(target, UsePacket);
                }

                if (Menu.GetBool("E.KillSteal") && E.IsReady() && target.Health < E.GetDamage(target) && target.IsValidTarget(E.Range))
                {
                    E.CastOnUnit(target, UsePacket);
                }

                if (Menu.GetBool("Ignite.KillSteal") && Ignite != SpellSlot.Unknown && Ignite.IsReady() && target.Health < Me.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) && target.IsValidTarget(600))
                {
                    Me.Spellbook.CastSpell(Ignite, target);
                }
            }
        }

        private static void Skin()
        {
            if (Menu.GetBool("Enable.Skin"))
            {
                Me.SetSkin(Me.CharData.BaseSkinName, Menu.GetList("Select.Skin"));
            }
            else
            {
                Me.SetSkin(Me.CharData.BaseSkinName, SkinID);
            }
        }

        private static void BeforeAttack(Orbwalking.BeforeAttackEventArgs Args)
        {
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && Args.Target is Obj_AI_Hero && Args.Target.IsValidTarget(600))
            {
                if(Menu.GetBool("ComboAA.Disable") && (W.IsReady() || E.IsReady()))
                {
                    Args.Process = false;
                }

                if (Menu.GetKey("ComboAA.DisableLevel"))
                {
                    Args.Process = false;
                }
            }
        }

        private static void OnEnemyGapcloser(ActiveGapcloser Gapcloser)
        {
            var sender = Gapcloser.Sender;

            if (sender.IsEnemy && Menu.GetBool("W.AntiGap"))
            {
                if (W.IsReady() && sender.IsValidTarget(250))
                {
                    W.CastOnUnit(sender, UsePacket);
                }
            }
        }

        private static void OnInterruptableTarget(Obj_AI_Hero sender, Interrupter2.InterruptableTargetEventArgs Args)
        {
            if (Menu.GetBool("W.Interrupt") && sender.IsEnemy)
            {
                if (Args.DangerLevel >= Interrupter2.DangerLevel.Medium && sender.IsValidTarget(W.Range))
                {
                    W.CastOnUnit(sender, UsePacket);
                }
            }
        }

        private static void OnDraw(EventArgs Args)
        {
            if (Me.IsDead)
                return;

            if (Q.IsReady() && Menu.GetBool("Q.Draw"))
                Render.Circle.DrawCircle(Me.Position, Q.Range, System.Drawing.Color.AliceBlue, 3);

            if (W.IsReady() && Menu.GetBool("W.Draw"))
                Render.Circle.DrawCircle(Me.Position, W.Range, System.Drawing.Color.DarkRed, 3);

            if (E.IsReady() && Menu.GetBool("E.Draw"))
                Render.Circle.DrawCircle(Me.Position, E.Range, System.Drawing.Color.Pink, 3);

            if (R.IsReady() && Menu.GetBool("R.Draw"))
                Render.Circle.DrawCircle(Me.Position, R.Range, System.Drawing.Color.GreenYellow, 3);

            if (Menu.GetBool("Damage.Draw"))
            {
                foreach (var target in HeroManager.Enemies.Where(e => e.IsValidTarget() && !e.IsZombie))
                {
                    HpBarDraw.Unit = target;
                    HpBarDraw.DrawDmg((float)GetComboDamage(target), new SharpDX.ColorBGRA(255, 255, 0, 170));
                }
            }

            if (Menu.GetBool("Combo.Draw"))
            {
                var text = "";
                var x = Drawing.WorldToScreen(Me.Position).X;
                var y = Drawing.WorldToScreen(Me.Position).Y;

                if (CanShield)
                {
                    text = "On";
                }
                else if (!CanShield)
                {
                    text = "Off";
                }

                Drawing.DrawText(x - 60, y + 40, System.Drawing.Color.Red, "Combo Shield : ");
                Drawing.DrawText(x + 50, y + 40, System.Drawing.Color.Yellow, text);
            }
        }

        private static void OnEndScene(EventArgs Args)
        {
            if (Menu.GetBool("MinR.Draw"))
            {
#pragma warning disable 618
                Utility.DrawCircle(Me.Position, R.Range, System.Drawing.Color.White, 1, 30, true);
#pragma warning restore 618
            }
        }

        private static bool UsePacket
        {
            get
            {
                return Menu.GetBool("Packet.Use");
            }
        }

        private static double GetComboDamage(Obj_AI_Hero target)
        {
            var Damage = 0d;

            if (Q.Level > 0 && Q.IsReady())
            {
                Damage += Q.GetDamage(target);
            }

            if (W.Level > 0 && W.IsReady())
            {
                Damage += W.GetDamage(target);
            }

            if (E.Level > 0 && E.IsReady())
            {
                Damage += E.GetDamage(target);
            }

            Damage += Me.GetAutoAttackDamage(target);

            return Damage;
        }
    }
}
