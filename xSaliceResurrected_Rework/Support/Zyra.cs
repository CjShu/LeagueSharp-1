using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using xSaliceResurrected.Managers;
using xSaliceResurrected.Utilities;
using Color = System.Drawing.Color;

namespace xSaliceResurrected.Support
{
    class Zyra : Champion
    {
        public Zyra()
        {
            LoadSpell();
            LoadMenu();
        }

        private void LoadSpell()
        {
            SpellManager.P = new Spell(SpellSlot.Q, 1470);
            SpellManager.Q = new Spell(SpellSlot.Q, 800);
            SpellManager.W = new Spell(SpellSlot.W, 825);
            SpellManager.E = new Spell(SpellSlot.E, 875);
            SpellManager.R = new Spell(SpellSlot.R, 700);

            SpellManager.P.SetSkillshot(0.5f, 70f, 1400f, false, SkillshotType.SkillshotLine);
            SpellManager.Q.SetSkillshot(0.8f, 60f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            SpellManager.E.SetSkillshot(0.25f, 70f, 1400f, false, SkillshotType.SkillshotLine);
            SpellManager.R.SetSkillshot(0.1f, 400f, float.MaxValue, false, SkillshotType.SkillshotCircle);
        }

        private void LoadMenu()
        {
            //key
            var key = new Menu("Key", "Key");
            {
                key.AddItem(new MenuItem("Orbwalk", "Combo!", true).SetValue(new KeyBind(32, KeyBindType.Press)));
                key.AddItem(new MenuItem("Farm", "Harass!", true).SetValue(new KeyBind("C".ToCharArray()[0], KeyBindType.Press)));
                key.AddItem(new MenuItem("FarmT", "Harass (toggle)!", true).SetValue(new KeyBind("N".ToCharArray()[0], KeyBindType.Toggle)));
                key.AddItem(new MenuItem("LaneClear", "Farm!", true).SetValue(new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));
                key.AddItem(new MenuItem("Escape", "Escape with E", true).SetValue(new KeyBind("Z".ToCharArray()[0], KeyBindType.Press)));
                //add to menu
                Menu.AddSubMenu(key);
            }

            //Combo menu:
            var combo = new Menu("Combo", "Combo");
            {
                combo.AddItem(new MenuItem("UseQCombo", "Use Q", true).SetValue(true));
                combo.AddItem(new MenuItem("UseWCombo", "Use W", true).SetValue(true));
                combo.AddItem(new MenuItem("UseECombo", "Use E", true).SetValue(true));
                combo.AddItem(new MenuItem("UseRCombo", "Use R", true).SetValue(true));
                //add to menu
                Menu.AddSubMenu(combo);
            }
            //Harass menu:
            var harass = new Menu("Harass", "Harass");
            {
                harass.AddItem(new MenuItem("UseQHarass", "Use Q", true).SetValue(true));
                harass.AddItem(new MenuItem("UseWHarass", "Use W", true).SetValue(false));
                harass.AddItem(new MenuItem("UseEHarass", "Use E", true).SetValue(true));
                harass.AddItem(new MenuItem("disableAA", "Disable AA", true).SetValue(true));
                ManaManager.AddManaManagertoMenu(harass, "Harass", 30);
                //add to menu
                Menu.AddSubMenu(harass);
            }
            //Farming menu:
            var farm = new Menu("Farm", "Farm");
            {
                farm.AddItem(new MenuItem("UseQFarm", "Use Q", true).SetValue(false));
                farm.AddItem(new MenuItem("UseWFarm", "Use W", true).SetValue(false));
                farm.AddItem(new MenuItem("UseEFarm", "Use E", true).SetValue(false));
                ManaManager.AddManaManagertoMenu(farm, "LaneClear", 30);
                //add to menu
                Menu.AddSubMenu(farm);
            }

            //Misc Menu:
            var misc = new Menu("Misc", "Misc");
            {
                misc.AddSubMenu(AoeSpellManager.AddHitChanceMenuCombo(true, false, true, true));
                misc.AddItem(new MenuItem("E_GapCloser", "Use E for GapCloser", true).SetValue(true));
                misc.AddItem(new MenuItem("smartKS", "Smart KS", true).SetValue(true));
                misc.AddItem(new MenuItem("Auto_Bloom", "Auto bloom Plant if Enemy near", true).SetValue(true));
                //add to menu
                Menu.AddSubMenu(misc);
            }

            //Drawings menu:
            var drawing = new Menu("Drawings", "Drawings");
            {
                drawing.AddItem(new MenuItem("Draw_Disabled", "Disable All", true).SetValue(false));
                drawing.AddItem(new MenuItem("Draw_Q", "Draw Q", true).SetValue(true));
                drawing.AddItem(new MenuItem("Draw_W", "Draw Q", true).SetValue(true));
                drawing.AddItem(new MenuItem("Draw_E", "Draw E", true).SetValue(true));
                drawing.AddItem(new MenuItem("Draw_R", "Draw R", true).SetValue(true));

                MenuItem drawComboDamageMenu = new MenuItem("Draw_ComboDamage", "Draw Combo Damage", true).SetValue(true);
                MenuItem drawFill = new MenuItem("Draw_Fill", "Draw Combo Damage Fill", true).SetValue(new Circle(true, Color.FromArgb(90, 255, 169, 4)));
                drawing.AddItem(drawComboDamageMenu);
                drawing.AddItem(drawFill);
                DamageIndicator.DamageToUnit = GetComboDamage;
                DamageIndicator.Enabled = drawComboDamageMenu.GetValue<bool>();
                DamageIndicator.Fill = drawFill.GetValue<Circle>().Active;
                DamageIndicator.FillColor = drawFill.GetValue<Circle>().Color;
                drawComboDamageMenu.ValueChanged +=
                    delegate(object sender, OnValueChangeEventArgs eventArgs)
                    {
                        DamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
                    };
                drawFill.ValueChanged +=
                    delegate(object sender, OnValueChangeEventArgs eventArgs)
                    {
                        DamageIndicator.Fill = eventArgs.GetNewValue<Circle>().Active;
                        DamageIndicator.FillColor = eventArgs.GetNewValue<Circle>().Color;
                    };
                Menu.AddSubMenu(drawing);
            }

            var customMenu = new Menu("Custom Perma Show", "Custom Perma Show");
            {
                var myCust = new CustomPermaMenu();
                customMenu.AddItem(new MenuItem("custMenu", "Move Menu", true).SetValue(new KeyBind("L".ToCharArray()[0], KeyBindType.Press)));
                customMenu.AddItem(new MenuItem("enableCustMenu", "Enabled", true).SetValue(true));
                customMenu.AddItem(myCust.AddToMenu("Combo Active: ", "Orbwalk"));
                customMenu.AddItem(myCust.AddToMenu("Harass Active: ", "Farm"));
                customMenu.AddItem(myCust.AddToMenu("Harass(T) Active: ", "FarmT"));
                customMenu.AddItem(myCust.AddToMenu("Laneclear Active: ", "LaneClear"));
                customMenu.AddItem(myCust.AddToMenu("Escape Active: ", "Escape"));
                Menu.AddSubMenu(customMenu);
            }
        }

        private float GetComboDamage(Obj_AI_Base enemy)
        {
            double damage = 0d;

            if (Q.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.Q) * 2;

            if (E.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.E);

            if (R.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.R) * 3;

            damage = ItemManager.CalcDamage(enemy, damage);

            return (float)damage;
        }

        private void Combo()
        {
            UseSpells(Menu.Item("UseQCombo", true).GetValue<bool>(), Menu.Item("UseWCombo", true).GetValue<bool>(),
                Menu.Item("UseECombo", true).GetValue<bool>(), Menu.Item("UseRCombo", true).GetValue<bool>(), "Combo");
        }

        private void Harass()
        {
            UseSpells(Menu.Item("UseQHarass", true).GetValue<bool>(), Menu.Item("UseWHarass", true).GetValue<bool>(),
                Menu.Item("UseEHarass", true).GetValue<bool>(), false, "Harass");
        }

        private void UseSpells(bool useQ, bool useW, bool useE, bool useR, string source)
        {
            if (source == "Harass" && !ManaManager.HasMana("Harass"))
                return;

            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);

            if (target == null)
                return;

            float dmg = GetComboDamage(target);

            if (useW)
            {
                if (useE)
                {
                    var pred = E.GetPrediction(target, true);
                    if (pred.Hitchance >= HitChance.VeryHigh&& E.IsReady())
                    {
                        E.Cast(target);
                        Cast_W(pred.CastPosition);
                    }
                }

                //items
                if (source == "Combo")
                {
                    var itemTarget = TargetSelector.GetTarget(750, TargetSelector.DamageType.Physical);
                    if (itemTarget != null)
                    {
                        ItemManager.Target = itemTarget;

                        //see if killable
                        if (dmg > itemTarget.Health - 50)
                            ItemManager.KillableTarget = true;

                        ItemManager.UseTargetted = true;
                    }
                }

                if (useQ)
                {
                    var pred = Q.GetPrediction(target);
                    if (pred.Hitchance >= HitChance.VeryHigh && pred.CastPosition.Distance(Player.ServerPosition) < Q.Range)
                    {
                        Q.Cast(pred.CastPosition);
                        Cast_W(pred.CastPosition);
                        return;
                    }
                }
            }
            else
            {
                if (useQ)
                    SpellCastManager.CastBasicSkillShot(Q, Q.Range, TargetSelector.DamageType.Magical, HitChance.VeryHigh);

                if (useE)
                    SpellCastManager.CastBasicSkillShot(E, E.Range, TargetSelector.DamageType.Magical, HitChance.VeryHigh);
            }

            if (useR)
                Cast_R();
        }

        protected override void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base unit, GameObjectProcessSpellCastEventArgs args)
        {
            if (!unit.IsMe) return;

            SpellSlot castedSlot = ObjectManager.Player.GetSpellSlot(args.SData.Name);

            if (castedSlot == SpellSlot.E || castedSlot == SpellSlot.Q)
            {
                E.LastCastAttemptT = Utils.TickCount;
            }
        }

        private void Cast_W(Vector3 pos)
        {
            if (!W.IsReady() || Player.Distance(pos) > W.Range || W.Instance.Ammo == 0)
                return;

            if (Utils.TickCount - E.LastCastAttemptT > 100 + Game.Ping)
                return;

            if (W.Instance.Ammo == 1)// 1 cast
            {
                Utility.DelayAction.Add(50, () => W.Cast(pos));
            }
            else if (W.Instance.Ammo == 2)// 2 cast
            {
                Utility.DelayAction.Add(50, () => W.Cast(pos));
                Utility.DelayAction.Add(350, () => W.Cast(pos));
            }
        }

        private void Cast_R()
        {
            var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);
            if (target == null)
                return;

            var pred = R.GetPrediction(target, true);

            if (GetComboDamage(target) > target.Health - 150 && pred.Hitchance >= HitChance.High)
            {
                R.Cast(pred.UnitPosition);
            }
        }


        private void Farm()
        {
            if (!ManaManager.HasMana("LaneClear"))
                return;

            List<Obj_AI_Base> allMinionsQ = MinionManager.GetMinions(Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.NotAlly);
            List<Obj_AI_Base> allMinionsE = MinionManager.GetMinions(Player.ServerPosition, E.Range, MinionTypes.All, MinionTeam.NotAlly);

            var useQ = Menu.Item("UseQFarm", true).GetValue<bool>();
            var useW = Menu.Item("UseWFarm", true).GetValue<bool>();
            var useE = Menu.Item("UseEFarm", true).GetValue<bool>();

            if (useQ && allMinionsQ.Count > 0 && Q.IsReady())
            {
                var pred = Q.GetCircularFarmLocation(allMinionsQ);
                Q.Cast(pred.Position);

                if (useW)
                    Cast_W(pred.Position.To3D());
            }
            if (useE && allMinionsE.Count > 0 && E.IsReady())
            {
                var pred = E.GetLineFarmLocation(allMinionsE);
                E.Cast(pred.Position);

                if (useW)
                    Cast_W(pred.Position.To3D());
            }
        }

        private void CheckKs()
        {
            foreach (Obj_AI_Hero target in ObjectManager.Get<Obj_AI_Hero>().Where(x => Player.IsValidTarget(E.Range)).OrderByDescending(GetComboDamage))
            {
                //QEW
                if (Player.Distance(target.ServerPosition) <= Q.Range && Player.GetSpellDamage(target, SpellSlot.Q) + Player.GetSpellDamage(target, SpellSlot.E) > target.Health && Q.IsReady() && E.IsReady())
                {
                    E.Cast(target);
                    Q.Cast(target);
                    W.Cast(Q.GetPrediction(target).CastPosition);
                    return;
                }
                //Q + plants
                if (Player.Distance(target.ServerPosition) <= Q.Range && Player.GetSpellDamage(target, SpellSlot.Q) * 2 > target.Health && Q.IsReady() && W.IsReady())
                {
                    Q.Cast(Q.GetPrediction(target).CastPosition);
                    W.Cast(Q.GetPrediction(target).CastPosition);
                    return;
                }
                //Q
                if (Player.Distance(target.ServerPosition) <= Q.Range && Player.GetSpellDamage(target, SpellSlot.Q) > target.Health && Q.IsReady())
                {
                    Q.Cast(target);
                    return;
                }

                //E
                if (Player.Distance(target.ServerPosition) <= E.Range && Player.GetSpellDamage(target, SpellSlot.E) > target.Health && E.IsReady())
                {
                    E.Cast(target);
                    return;
                }

                //R
                if (Player.Distance(target.ServerPosition) <= R.Range && Player.GetSpellDamage(target, SpellSlot.R) > target.Health && R.IsReady() && Menu.Item("R_KS", true).GetValue<bool>())
                {
                    R.Cast(target);
                    return;
                }
            }
        }

        private void AutoBloom()
        {
            if (!Q.IsReady() || !Menu.Item("Auto_Bloom", true).GetValue<bool>())
                return;

            foreach (Obj_AI_Hero target in ObjectManager.Get<Obj_AI_Hero>().Where(x => Player.IsValidTarget(Q.Range)).OrderByDescending(GetComboDamage))
            {
                foreach (Obj_AI_Minion plants in ObjectManager.Get<Obj_AI_Minion>().Where(x => x.Name == "Zyra" && x.Distance(Player.Position) < Q.Range))
                {
                    var predQ = Q.GetPrediction(target, true);

                    if (Q.IsReady() && plants.Distance(predQ.UnitPosition) < Q.Width)
                        Q.Cast(plants);
                }
            }
        }

        protected override void BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (!Menu.Item("disableAA", true).GetValue<bool>())
                return;

            if ((args.Target is Obj_AI_Minion) && Menu.Item("Farm", true).GetValue<KeyBind>().Active)
                args.Process = false;
        }

        protected override void Game_OnGameUpdate(EventArgs args)
        {
            if (Player.IsDead)
                return;

            if (Player.IsZombie)
            {
                var target = TargetSelector.GetTarget(P.Range, TargetSelector.DamageType.True);
                if (target == null)
                    return;
                var pred = P.GetPrediction(target);
                if (pred.Hitchance >= HitChance.High)
                    Q.Cast(target);
            }

            if (Menu.Item("smartKS", true).GetValue<bool>())
                CheckKs();

            AutoBloom();

            if (Menu.Item("Escape", true).GetValue<KeyBind>().Active && E.IsReady())
            {
                OrbwalkManager.Orbwalk(null, Game.CursorPos);
                foreach (Obj_AI_Hero target in ObjectManager.Get<Obj_AI_Hero>().Where(x => Player.IsValidTarget(E.Range)).OrderBy(x => x.Distance(Player.Position)))
                {
                    E.Cast(target);
                    return;
                }
            }

            if (Menu.Item("Orbwalk", true).GetValue<KeyBind>().Active)
            {
                Combo();
            }
            else
            {
                if (Menu.Item("LaneClear", true).GetValue<KeyBind>().Active)
                    Farm();

                if (Menu.Item("Farm", true).GetValue<KeyBind>().Active)
                    Harass();
                

                if (Menu.Item("FarmT", true).GetValue<KeyBind>().Active)
                    Harass();
            }
        }

        protected override void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!Menu.Item("E_GapCloser", true).GetValue<bool>()) return;

            if (E.IsReady() && gapcloser.Sender.Distance(Player.Position) < 300)
                E.Cast(gapcloser.Sender);
        }

        protected override void Drawing_OnDraw(EventArgs args)
        {
            if (Menu.Item("Draw_Disabled", true).GetValue<bool>())
                return;

            if (Menu.Item("Draw_Q", true).GetValue<bool>())
                if (Q.Level > 0)
                    Render.Circle.DrawCircle(Player.Position, Q.Range, Q.IsReady() ? Color.Green : Color.Red);

            if (Menu.Item("Draw_W", true).GetValue<bool>())
                if (W.Level > 0)
                    Render.Circle.DrawCircle(Player.Position, W.Range, W.IsReady() ? Color.Green : Color.Red);

            if (Menu.Item("Draw_E", true).GetValue<bool>())
                if (E.Level > 0)
                    Render.Circle.DrawCircle(Player.Position, E.Range, E.IsReady() ? Color.Green : Color.Red);

            if (Menu.Item("Draw_R", true).GetValue<bool>())
                if (R.Level > 0)
                    Render.Circle.DrawCircle(Player.Position, R.Range, R.IsReady() ? Color.Green : Color.Red);
        }
    }
}
