namespace xSaliceResurrected.Mid
{
    using System;
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;
    using Managers;
    using Persistence;
    using Utilities;
    using Color = System.Drawing.Color;

    internal class Orianna : Champion
    {
        //ball manager
        private bool _isBallMoving;
        private Vector3 _currentBallPosition;
        private Vector3 _allyDraw;
        private int _ballStatus;

        public Orianna()
        {
            SpellManager.Q = new Spell(SpellSlot.Q, 825);
            SpellManager.W = new Spell(SpellSlot.W);
            SpellManager.E = new Spell(SpellSlot.E, 1100);
            SpellManager.R = new Spell(SpellSlot.R);

            SpellManager.Q.SetSkillshot(0.25f, 80, 1300, false, SkillshotType.SkillshotCircle);
            SpellManager.W.SetSkillshot(0f, 250, float.MaxValue, false, SkillshotType.SkillshotCircle);
            SpellManager.E.SetSkillshot(0.25f, 145, 1700, false, SkillshotType.SkillshotLine);
            SpellManager.R.SetSkillshot(0.60f, 350, float.MaxValue, false, SkillshotType.SkillshotCircle);

            SpellManager.SpellList.Add(Q);
            SpellManager.SpellList.Add(W);
            SpellManager.SpellList.Add(E);
            SpellManager.SpellList.Add(R);

            var key = new Menu("Keys", "Keys");
            {
                key.AddItem(new MenuItem("FarmT", "Harass (toggle)!", true).SetValue(new KeyBind("N".ToCharArray()[0], KeyBindType.Toggle)));
                Menu.AddSubMenu(key);
            }

            //Spell Menu
            var spellMenu = new Menu("SpellMenu", "SpellMenu");
            {
                //W
                var wMenu = new Menu("WSpell", "WSpell");
                {
                    wMenu.AddItem(new MenuItem("autoW", "Use W if hit", true).SetValue(new Slider(2, 1, 5)));
                    spellMenu.AddSubMenu(wMenu);
                }

                //E
                var eMenu = new Menu("ESpell", "ESpell");
                {
                    eMenu.AddItem(new MenuItem("saveEMana", "Do not E To save Mana for Q+W", true).SetValue(true));
                    eMenu.AddItem(new MenuItem("UseEDmg", "Use E to Dmg", true).SetValue(true));
                    eMenu.AddSubMenu(new Menu("E Ally Inc Spell", "shield"));
                    eMenu.SubMenu("shield").AddItem(new MenuItem("eAllyIfHP", "If HP < %", true).SetValue(new Slider(40)));
                    foreach (Obj_AI_Hero ally in ObjectManager.Get<Obj_AI_Hero>().Where(ally => ally.IsAlly))
                        eMenu.SubMenu("shield").AddItem(new MenuItem("shield" + ally.CharData.BaseSkinName, ally.CharData.BaseSkinName, true).SetValue(true));

                    spellMenu.AddSubMenu(eMenu);
                }
                //R
                var rMenu = new Menu("RSpell", "RSpell");
                {
                    rMenu.AddItem(new MenuItem("autoR", "Use R if hit (Global check)", true).SetValue(new Slider(3, 1, 5)));
                    rMenu.AddItem(new MenuItem("blockR", "Block R if no enemy", true).SetValue(true));
                    rMenu.AddItem(new MenuItem("overK", "OverKill Check", true).SetValue(true));
                    rMenu.AddItem(new MenuItem("killR", "Use R only if it hits multiple target", true).SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Toggle)));

                    rMenu.AddSubMenu(new Menu("Auto use R on", "intR"));
                    rMenu.SubMenu("intR").AddItem(new MenuItem("AdditonalTargets", "Require Addition targets", true).SetValue(new Slider(1, 0, 4)));
                    foreach (Obj_AI_Hero enemy in ObjectManager.Get<Obj_AI_Hero>().Where(enemy => enemy.Team != Player.Team))
                        rMenu.SubMenu("intR").AddItem(new MenuItem("intR" + enemy.CharData.BaseSkinName, enemy.CharData.BaseSkinName, true).SetValue(false));

                    spellMenu.AddSubMenu(rMenu);
                }
                Menu.AddSubMenu(spellMenu);
            }

            //Combo menu:
            var combo = new Menu("Combo", "Combo");
            {
                combo.AddItem(new MenuItem("UseQCombo", "Use Q", true).SetValue(true));
                combo.AddItem(new MenuItem("UseWCombo", "Use W", true).SetValue(true));
                combo.AddItem(new MenuItem("UseECombo", "Use E", true).SetValue(true));
                combo.AddItem(new MenuItem("UseRCombo", "Use R", true).SetValue(true));
                combo.AddItem(new MenuItem("autoRCombo", "Use R if hit", true).SetValue(new Slider(2, 1, 5)));
                Menu.AddSubMenu(combo);
            }
            //Harass menu:
            var harass = new Menu("Harass", "Harass");
            {
                harass.AddItem(new MenuItem("UseQHarass", "Use Q", true).SetValue(true));
                harass.AddItem(new MenuItem("UseWHarass", "Use W", true).SetValue(false));
                harass.AddItem(new MenuItem("UseEHarass", "Use E", true).SetValue(true));
                ManaManager.AddManaManagertoMenu(harass, "Harass", 30);
                Menu.AddSubMenu(harass);
            }
            //Farming menu:
            var farm = new Menu("Farm", "Farm");
            {
                farm.AddItem(new MenuItem("UseQFarm", "Use Q", true).SetValue(false));
                farm.AddItem(new MenuItem("UseWFarm", "Use W", true).SetValue(false));
                farm.AddItem(new MenuItem("qFarm", "Only Q/W if > minion", true).SetValue(new Slider(3, 0, 5)));
                ManaManager.AddManaManagertoMenu(farm, "Farm", 50);
                Menu.AddSubMenu(farm);
            }

            //intiator list:
            var initator = new Menu("Initiator", "Initiator");
            {
                foreach (Obj_AI_Hero hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsAlly))
                {
                    foreach (Initiator intiator in Initiator.InitatorList)
                    {
                        if (intiator.HeroName == hero.CharData.BaseSkinName)
                        {
                            initator.AddItem(new MenuItem(intiator.SpellName, intiator.SpellName, true)).SetValue(false);
                        }
                    }
                }
                Menu.AddSubMenu(initator);
            }

            //Misc Menu:
            var misc = new Menu("Misc", "Misc");
            {
                misc.AddItem(new MenuItem("UseInt", "Use R to Interrupt", true).SetValue(true));
            }


            //Drawings menu:
            var drawing = new Menu("Drawings", "Drawings");
            {
                drawing.AddItem(new MenuItem("QRange", "Q range", true).SetValue(new Circle(false, Color.FromArgb(100, 255, 0, 255))));
                drawing.AddItem(new MenuItem("WRange", "W range", true).SetValue(new Circle(true, Color.FromArgb(100, 255, 0, 255))));
                drawing.AddItem(new MenuItem("ERange", "E range", true).SetValue(new Circle(false, Color.FromArgb(100, 255, 0, 255))));
                drawing.AddItem(new MenuItem("RRange", "R range", true).SetValue(new Circle(false, Color.FromArgb(100, 255, 0, 255))));

                MenuItem drawComboDamageMenu = new MenuItem("Draw_ComboDamage", "Draw Combo Damage", true).SetValue(true);
                MenuItem drawFill = new MenuItem("Draw_Fill", "Draw Combo Damage Fill", true).SetValue(new Circle(true, Color.FromArgb(90, 255, 169, 4)));
                drawing.AddItem(drawComboDamageMenu);
                drawing.AddItem(drawFill);
                DamageIndicator.DamageToUnit = GetComboDamage;
                DamageIndicator.Enabled = drawComboDamageMenu.GetValue<bool>();
                DamageIndicator.Fill = drawFill.GetValue<Circle>().Active;
                DamageIndicator.FillColor = drawFill.GetValue<Circle>().Color;
                drawComboDamageMenu.ValueChanged +=
                    delegate (object sender, OnValueChangeEventArgs eventArgs)
                    {
                        DamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
                    };
                drawFill.ValueChanged +=
                    delegate (object sender, OnValueChangeEventArgs eventArgs)
                    {
                        DamageIndicator.Fill = eventArgs.GetNewValue<Circle>().Active;
                        DamageIndicator.FillColor = eventArgs.GetNewValue<Circle>().Color;
                    };

                //add to menu
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
                customMenu.AddItem(myCust.AddToMenu("LastHit Active: ", "LastHit"));
                customMenu.AddItem(myCust.AddToMenu("Escape Active: ", "Flee"));
                customMenu.AddItem(myCust.AddToMenu("R Multi Only: ", "killR"));
                Menu.AddSubMenu(customMenu);
            }
        }

        private float GetComboDamage(Obj_AI_Base enemy)
        {
            var damage = 0d;

            //if (Q.IsReady())
            damage += Player.GetSpellDamage(enemy, SpellSlot.Q) * 1.5;

            if (W.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.W);

            if (E.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.E);

            if (R.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.R) - 25;

            damage = ItemManager.CalcDamage(enemy, damage);

            return (float)damage;
        }

        private void Combo()
        {
            //Orbwalker.SetAttacks(!(Q.IsReady()));
            UseSpells(Menu.Item("UseQCombo", true).GetValue<bool>(), Menu.Item("UseWCombo", true).GetValue<bool>(),
                Menu.Item("UseECombo", true).GetValue<bool>(), Menu.Item("UseRCombo", true).GetValue<bool>(), "Combo");
        }
        private void Harass()
        {
            UseSpells(Menu.Item("UseQHarass", true).GetValue<bool>(), Menu.Item("UseWHarass", true).GetValue<bool>(),
                Menu.Item("UseEHarass", true).GetValue<bool>(), false, "Harass");
        }

        private void UseSpells(bool useQ, bool useW, bool useE, bool useR, String source)
        {
            if (source == "Harass" && !ManaManager.HasMana("Harass"))
                return;

            var range = E.IsReady() ? E.Range : Q.Range;
            var target = TargetSelector.GetTarget(range, TargetSelector.DamageType.Magical);

            if (useQ && Q.IsReady())
            {
                CastQ(target, source);
            }

            if (_isBallMoving)
                return;

            if (useW && target != null && W.IsReady())
            {
                CastW(target);
            }

            //items
            if (source == "Combo")
            {
                var itemTarget = TargetSelector.GetTarget(750, TargetSelector.DamageType.Physical);
                if (itemTarget != null)
                {
                    var dmg = GetComboDamage(itemTarget);
                    ItemManager.Target = itemTarget;

                    //see if killable
                    if (dmg > itemTarget.Health - 50)
                        ItemManager.KillableTarget = true;

                    ItemManager.UseTargetted = true;
                }
            }

            if (useE && target != null && E.IsReady())
            {
                CastE(target);
            }

            if (useR && target != null && R.IsReady())
            {
                if (Menu.Item("intR" + target.CharData.BaseSkinName, true) != null)
                {
                    foreach (
                        var enemy in
                            ObjectManager.Get<Obj_AI_Hero>()
                                .Where(x => Player.Distance(x.Position) < 1500 && x.IsValidTarget() && x.IsEnemy && !x.IsDead))
                    {
                        if (!enemy.IsDead && Menu.Item("intR" + enemy.CharData.BaseSkinName, true).GetValue<bool>())
                        {
                            CastR(enemy, true);
                            return;
                        }
                    }
                }

                if (!(Menu.Item("killR", true).GetValue<KeyBind>().Active)) //check if multi
                {
                    if (Menu.Item("overK", true).GetValue<bool>() &&
                        Player.GetSpellDamage(target, SpellSlot.Q) + Player.GetSpellDamage(target, SpellSlot.W) >= target.Health)
                    {
                        return;
                    }
                    if (GetComboDamage(target) >= target.Health - 100 && !target.IsZombie)
                        CastR(target);
                }
            }
        }

        private void CastW(Obj_AI_Base target)
        {
            if (_isBallMoving) return;

            var prediction = Util.GetPCircle(_currentBallPosition, W, target, true);

            if (W.IsReady() && prediction.UnitPosition.Distance(_currentBallPosition) < W.Width)
            {
                W.Cast();
            }

        }

        private void CastR(Obj_AI_Base target, bool checkAdditional = false)
        {
            if (_isBallMoving) return;

            var prediction = Util.GetPCircle(_currentBallPosition, R, target, true);

            if (R.IsReady() && prediction.UnitPosition.Distance(_currentBallPosition) <= R.Width)
            {
                if (checkAdditional)
                {
                    var add = Menu.Item("AdditonalTargets", true).GetValue<Slider>().Value + 1;

                    if (CountR() >= add)
                        R.Cast();
                }
                else
                {
                    R.Cast();
                }
            }
        }

        private void CastE(Obj_AI_Base target)
        {
            if (_isBallMoving) return;

            if (Menu.Item("saveEMana", true).GetValue<bool>() && Player.Mana - ESpell.ManaCost < QSpell.ManaCost + WSpell.ManaCost)
                return;

            var etarget = Player;

            switch (_ballStatus)
            {
                case 0:
                    if (target != null)
                    {
                        var travelTime = target.Distance(Player.ServerPosition) / Q.Speed;
                        var minTravelTime = 10000f;

                        foreach (
                            Obj_AI_Hero ally in
                                ObjectManager.Get<Obj_AI_Hero>()
                                    .Where(x => x.IsAlly && Player.Distance(x.ServerPosition) <= E.Range && !x.IsMe))
                        { 
                            //dmg enemy with E
                            if (Menu.Item("UseEDmg", true).GetValue<bool>())
                            {
                                var prediction3 = Util.GetP(Player.ServerPosition, E, target, true);
                                var obj = Util.VectorPointProjectionOnLineSegment(Player.ServerPosition.To2D(),
                                    ally.ServerPosition.To2D(), prediction3.UnitPosition.To2D());
                                var isOnseg = (bool)obj[2];
                                var pointLine = (Vector2)obj[1];

                                if (E.IsReady() && isOnseg &&
                                    prediction3.UnitPosition.Distance(pointLine.To3D()) < E.Width)
                                {
                                    //Console.WriteLine("Dmg 1");
                                    E.CastOnUnit(ally);
                                    return;
                                }
                            }

                            var allyRange = target.Distance(ally.ServerPosition) / Q.Speed +
                                                ally.Distance(Player.ServerPosition) / E.Speed;
                            if (allyRange < minTravelTime)
                            {
                                etarget = ally;
                                minTravelTime = allyRange;
                            }
                        }

                        if (minTravelTime < travelTime && Player.Distance(etarget.ServerPosition) <= E.Range &&
                            E.IsReady())
                        {
                            E.CastOnUnit(etarget);
                        }
                    }
                    break;
                case 1:
                    //dmg enemy with E
                    if (Menu.Item("UseEDmg", true).GetValue<bool>())
                    {
                        var prediction = Util.GetP(_currentBallPosition, E, target, true);
                        var obj = Util.VectorPointProjectionOnLineSegment(_currentBallPosition.To2D(),
                            Player.ServerPosition.To2D(), prediction.UnitPosition.To2D());
                        var isOnseg = (bool)obj[2];
                        var pointLine = (Vector2)obj[1];

                        if (E.IsReady() && isOnseg && prediction.UnitPosition.Distance(pointLine.To3D()) < E.Width)
                        {
                            //Console.WriteLine("Dmg 2");
                            E.CastOnUnit(Player);
                            return;
                        }
                    }

                    var travelTime2 = target.Distance(_currentBallPosition) / Q.Speed;
                    var minTravelTime2 = target.Distance(Player.ServerPosition) / Q.Speed +
                                            Player.Distance(_currentBallPosition) / E.Speed;

                    if (minTravelTime2 < travelTime2 && target.Distance(Player.ServerPosition) <= Q.Range + Q.Width &&
                        E.IsReady())
                    {
                        E.CastOnUnit(Player);
                    }

                    break;
                case 2:
                    var travelTime3 = target.Distance(_currentBallPosition) / Q.Speed;
                    var minTravelTime3 = 10000f;

                    foreach (
                        var ally in
                            ObjectManager.Get<Obj_AI_Hero>()
                                .Where(x => x.IsAlly && Player.Distance(x.ServerPosition) <= E.Range && !x.IsMe))
                    {
                        //dmg enemy with E
                        if (Menu.Item("UseEDmg", true).GetValue<bool>())
                        {
                            var prediction2 = Util.GetP(_currentBallPosition, E, target, true);
                            var obj = Util.VectorPointProjectionOnLineSegment(_currentBallPosition.To2D(),
                                ally.ServerPosition.To2D(), prediction2.UnitPosition.To2D());
                            var isOnseg = (bool)obj[2];
                            var pointLine = (Vector2)obj[1];

                            if (E.IsReady() && isOnseg &&
                                prediction2.UnitPosition.Distance(pointLine.To3D()) < E.Width)
                            {
                                E.CastOnUnit(ally);
                                return;
                            }
                        }

                        var allyRange2 = target.Distance(ally.ServerPosition) / Q.Speed +
                                            ally.Distance(_currentBallPosition) / E.Speed;

                        if (allyRange2 < minTravelTime3)
                        {
                            etarget = ally;
                            minTravelTime3 = allyRange2;
                        }
                    }

                    if (minTravelTime3 < travelTime3 && Player.Distance(etarget.ServerPosition) <= E.Range &&
                        E.IsReady())
                    {
                        E.CastOnUnit(etarget);
                    }

                    break;
            }
        }

        private void CastQ(Obj_AI_Base target, String source)
        {
            if (_isBallMoving || !target.IsValidTarget(Q.Range)) return;

            var prediction = Util.GetP(_currentBallPosition, Q, target,  true);

            if (Q.IsReady() && prediction.Hitchance >= HitChance.VeryHigh && Player.Distance(target.Position) <= Q.Range)
            {
                Q.Cast(prediction.CastPosition);
            }
        }

        private void CheckWMec()
        {
            if (!W.IsReady() || _isBallMoving)
                return;

            var minHit = Menu.Item("autoW", true).GetValue<Slider>().Value;

            var hit = (from x in ObjectManager.Get<Obj_AI_Hero>().Where(champ => champ.IsValidTarget(1500) && champ.IsVisible && !champ.IsZombie)
                       select Util.GetPCircle(_currentBallPosition, W, x, true)).Count(prediction => W.IsReady() && prediction.UnitPosition.Distance(_currentBallPosition) < W.Width);

            if (hit >= minHit && W.IsReady())
                W.Cast();
        }

        private void CheckRMec()
        {
            if (!R.IsReady() || _isBallMoving)
                return;

            var minHit = Menu.Item("autoRCombo", true).GetValue<Slider>().Value;

            var hit = (from x in ObjectManager.Get<Obj_AI_Hero>().Where(champ => champ.IsValidTarget(1500) && champ.IsVisible && !champ.IsZombie)
                       select Util.GetPCircle(_currentBallPosition, R, x, true)).Count(prediction => R.IsReady() && prediction.UnitPosition.Distance(_currentBallPosition) < R.Width);

            if (hit >= minHit && R.IsReady())
                R.Cast();
        }

        private void CheckRMecGlobal()
        {
            if (!R.IsReady() || _isBallMoving)
                return;

            var minHit = Menu.Item("autoR", true).GetValue<Slider>().Value;

            var hit = (from x in ObjectManager.Get<Obj_AI_Hero>().Where(champ => champ.IsValidTarget(1500) && champ.IsVisible && !champ.IsZombie)
                       select Util.GetPCircle(_currentBallPosition, R, x, true)).Count(prediction => R.IsReady() && prediction.UnitPosition.Distance(_currentBallPosition) < R.Width);


            if (hit >= minHit && R.IsReady())
                R.Cast();
        }

        private int CountR()
        {
            if (!R.IsReady())
                return 0;

            return (from enemy in ObjectManager.Get<Obj_AI_Hero>().Where(champ => champ.IsValidTarget(1500) && champ.IsVisible && !champ.IsZombie)
                    select Util.GetPCircle(_currentBallPosition, R, enemy, true)).Count(prediction => R.IsReady() && prediction.UnitPosition.Distance(_currentBallPosition) <= R.Width);
        }

        private void LastHit()
        {
            if (!OrbwalkManager.CanMove(40)) return;

            var allMinions = MinionManager.GetMinions(Player.ServerPosition, Q.Range);

            if (Q.IsReady())
            {
                foreach (var minion in allMinions)
                {
                    if (minion.IsValidTarget() &&
                        HealthPrediction.GetHealthPrediction(minion, (int)(Player.Distance(minion.Position) * 1000 / 1400)) <
                        Player.GetSpellDamage(minion, SpellSlot.Q) - 10)
                    {
                        var prediction = Util.GetP(_currentBallPosition, Q, minion, true);

                        if (prediction.Hitchance >= HitChance.High && Q.IsReady())
                            Q.Cast(prediction.CastPosition);
                    }
                }
            }
        }

        private void Farm()
        {
            if (!OrbwalkManager.CanMove(40)) return;

            if (!ManaManager.HasMana("Farm"))
                return;

            var allMinionsQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition,
                Q.Range + Q.Width);
            var allMinionsW = MinionManager.GetMinions(ObjectManager.Player.ServerPosition,
                Q.Range + Q.Width);

            var useQ = Menu.Item("UseQFarm", true).GetValue<bool>();
            var useW = Menu.Item("UseWFarm", true).GetValue<bool>();
            var min = Menu.Item("qFarm", true).GetValue<Slider>().Value;

            if (useQ && Q.IsReady())
            {
                Q.From = _currentBallPosition;

                var pred = Q.GetCircularFarmLocation(allMinionsQ, Q.Width + 15);

                if (pred.MinionsHit >= min)
                    Q.Cast(pred.Position);
            }

            var hit = 0;
            if (useW && W.IsReady())
            {
                hit += allMinionsW.Count(enemy => enemy.Distance(_currentBallPosition) < W.Width);

                if (hit >= min && W.IsReady())
                    W.Cast();
            }
        }

        private void Escape()
        {
            OrbwalkManager.Orbwalk(null, Game.CursorPos);

            if (_ballStatus == 0 && W.IsReady())
                W.Cast();
            else if (E.IsReady() && _ballStatus != 0)
                E.CastOnUnit(Player);
        }

        protected override void Game_OnGameUpdate(EventArgs args)
        {
            //check if player is dead
            if (Player.IsDead) return;

            OnGainBuff();
            CheckRMecGlobal();
            CheckWMec();

            if (Menu.Item("FarmT", true).GetValue<KeyBind>().Active)
                Harass();

            switch (Orbwalker.ActiveMode)
            {
                case xSaliceResurrected.Orbwalking.OrbwalkingMode.Combo:
                    CheckRMec();
                    Combo();
                    break;
                case xSaliceResurrected.Orbwalking.OrbwalkingMode.Mixed:
                    break;
                case xSaliceResurrected.Orbwalking.OrbwalkingMode.LastHit:
                    LastHit();
                    break;
                case xSaliceResurrected.Orbwalking.OrbwalkingMode.LaneClear:
                    Farm();
                    break;
                case xSaliceResurrected.Orbwalking.OrbwalkingMode.Freeze:
                    break;
                case xSaliceResurrected.Orbwalking.OrbwalkingMode.CustomMode:
                    break;
                case xSaliceResurrected.Orbwalking.OrbwalkingMode.None:
                    break;
                case xSaliceResurrected.Orbwalking.OrbwalkingMode.Flee:
                    Escape();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnGainBuff()
        {
            if (Player.HasBuff("OrianaGhostSelf"))
            {
                _ballStatus = 0;
                _currentBallPosition = Player.ServerPosition;
                _isBallMoving = false;
                return;
            }

            foreach (Obj_AI_Hero ally in
                ObjectManager.Get<Obj_AI_Hero>()
                    .Where(ally => ally.IsAlly && !ally.IsDead && ally.HasBuff("orianaghost")))
            {
                _ballStatus = 2;
                _currentBallPosition = ally.ServerPosition;
                _allyDraw = ally.Position;
                _isBallMoving = false;
                return;
            }

            _ballStatus = 1;
        }

        protected override void Drawing_OnDraw(EventArgs args)
        {
            foreach (var spell in SpellList)
            {
                var menuItem = Menu.Item(spell.Slot + "Range", true).GetValue<Circle>();
                if ((spell.Slot == SpellSlot.R && menuItem.Active) || (spell.Slot == SpellSlot.W && menuItem.Active))
                {
                    if (_ballStatus == 0)
                        Render.Circle.DrawCircle(Player.Position, spell.Width, spell.IsReady() ? Color.Aqua : Color.Red);
                    else if (_ballStatus == 2)
                        Render.Circle.DrawCircle(_allyDraw, spell.Width, spell.IsReady() ? Color.Aqua : Color.Red);
                    else
                        Render.Circle.DrawCircle(_currentBallPosition, spell.Width, spell.IsReady() ? Color.Aqua : Color.Red);
                }
                else if (menuItem.Active)
                    Render.Circle.DrawCircle(Player.Position, spell.Range, spell.IsReady() ? Color.Aqua : Color.Red);
            }
        }

        protected override void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base unit, GameObjectProcessSpellCastEventArgs args)
        {
            //Shield Ally
            if (!Menu.Item("saveEMana", true).GetValue<bool>() || Player.Mana - ESpell.ManaCost >= QSpell.ManaCost + WSpell.ManaCost)
            {
                if (unit.IsEnemy && unit.Type == GameObjectType.obj_AI_Hero && E.IsReady())
                {
                    foreach (
                        var ally in
                            ObjectManager.Get<Obj_AI_Hero>()
                                .Where(
                                    x =>
                                        Player.Distance(x.Position) < E.Range && Player.Distance(unit.Position) < 1500 &&
                                        x.IsAlly && !x.IsDead).OrderBy(x => x.Distance(args.End)))
                    {
                        if (Menu.Item("shield" + ally.CharData.BaseSkinName, true) != null)
                        {
                            if (Menu.Item("shield" + ally.CharData.BaseSkinName, true).GetValue<bool>())
                            {
                                int hp = Menu.Item("eAllyIfHP", true).GetValue<Slider>().Value;

                                if (ally.Distance(args.End) < 500 && ally.HealthPercent <= hp)
                                {
                                    //Game.PrintChat("shielding");
                                    E.CastOnUnit(ally);
                                    _isBallMoving = true;
                                    return;
                                }
                            }
                        }
                    }
                }
            }

            //intiator
            if (unit.IsAlly)
            {
                if (Initiator.InitatorList.Where(spell => args.SData.Name == spell.SDataName).Where(spell => Menu.Item(spell.SpellName, true).GetValue<bool>()).Any(spell => E.IsReady() && Player.Distance(unit.Position) < E.Range))
                {
                    E.CastOnUnit(unit);
                    _isBallMoving = true;
                    return;
                }
            }

            if (!unit.IsMe) return;

            var castedSlot = ObjectManager.Player.GetSpellSlot(args.SData.Name);

            if (castedSlot == SpellSlot.Q)
            {
                _isBallMoving = true;
                Utility.DelayAction.Add(
                    (int)Math.Max(1, 1000 * (args.End.Distance(_currentBallPosition) - Game.Ping - 0.1) / Q.Speed), () =>
                    {
                        _currentBallPosition = args.End;
                        _ballStatus = 1;
                        _isBallMoving = false;
                        //Game.PrintChat("Stopped");
                    });
            }
        }

        protected override void Interrupter_OnPosibleToInterrupt(Obj_AI_Hero unit, Interrupter2.InterruptableTargetEventArgs spell)
        {
            if (!Menu.Item("UseInt", true).GetValue<bool>() || _isBallMoving) return;

            if (Player.Distance(unit.Position) < R.Width)
            {
                CastR(unit);
            }
            else
            {
                CastQ(unit, "Combo");
            }
        }

        protected override void SpellbookOnOnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (args.Slot != SpellSlot.R)
                return;

            if (_isBallMoving)
                args.Process = false;

            if (CountR() == 0 && Menu.Item("blockR", true).GetValue<bool>())
            {
                //Block packet if enemies hit is 0
                args.Process = false;
            }
        }
    }
}
