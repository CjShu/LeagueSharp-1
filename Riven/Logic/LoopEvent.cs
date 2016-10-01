namespace Flowers_Riven
{
    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;
    using System;
    using System.Linq;

    internal class LoopEvent
    {
        public static void Init()
        {
            Game.OnUpdate += OnUpdate;
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Program.Me.IsDead)
                return;

            Autobool();
            KeelQLogic();
            KillStealLogic();

            if (Program.Menu.Item("EnableSkin", true).GetValue<bool>())
            {
                ObjectManager.Player.SetSkin(ObjectManager.Player.ChampionName, Program.Menu.Item("SelectSkin", true).GetValue<StringList>().SelectedIndex);
            }

            switch (Program.Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Brust:
                    Brust();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
                case Orbwalking.OrbwalkingMode.QuickHarass:
                    QuickHarass();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    LaneClear();
                    break;
                case Orbwalking.OrbwalkingMode.Flee:
                    FleeLogic();
                    break;
                case Orbwalking.OrbwalkingMode.WallJump:
                    WallJump();
                    break;
            }
        }

        private static void Autobool()
        {
            if (Program.QTarget != null)
            {
                if (Program.CanQ)
                {
                    Program.Q.Cast(((Obj_AI_Base)Program.QTarget).Position);
                }
            }
        }

        private static void KeelQLogic()
        {
            if (Program.Menu.Item("KeepQALive", true).GetValue<bool>() && !Program.Me.UnderTurret(true) &&
                !Program.Me.IsRecalling() && Program.Me.HasBuff("RivenTriCleave"))
            {
                if (Program.Me.GetBuff("RivenTriCleave").EndTime - Game.Time < 0.3)
                {
                    Program.Q.Cast(Game.CursorPos);
                }
            }
        }

        private static void KillStealLogic()
        {
            foreach (var e in HeroManager.Enemies.Where(e => !e.IsZombie && !e.HasBuff("KindredrNoDeathBuff") && 
            !e.HasBuff("Undying Rage") &&
            !e.HasBuff("JudicatorIntervention") && e.IsValidTarget()))
            {
                if (Program.W.IsReady() && Program.Menu.Item("KillStealW", true).GetValue<bool>())
                {
                    if (e.IsValidTarget(Program.W.Range) && Program.Me.GetSpellDamage(e, SpellSlot.W) > e.Health + e.HPRegenRate)
                    {
                        Program.W.Cast();
                    }
                }

                if (Program.R.IsReady() && Program.Menu.Item("KillStealR", true).GetValue<bool>())
                {
                    if (Program.Me.HasBuff("RivenWindScarReady"))
                    {
                        if (Program.E.IsReady() && Program.Menu.Item("KillStealE", true).GetValue<bool>())
                        {
                            if (Program.Me.ServerPosition.CountEnemiesInRange(Program.R.Range + Program.E.Range) < 3 && Program.Me.HealthPercent > 50)
                            {
                                if (Program.Me.GetSpellDamage(e, SpellSlot.R) > e.Health + e.HPRegenRate && e.IsValidTarget(Program.R.Range + Program.E.Range - 100))
                                {
                                    if (Program.E.IsReady())
                                    {
                                        Program.E.Cast(e.Position);
                                    }
                                    else if (!Program.E.IsReady())
                                    {
                                        Program.R.CastIfHitchanceEquals(e, HitChance.High, true);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (Program.Me.GetSpellDamage(e, SpellSlot.R) > e.Health + e.HPRegenRate && e.IsValidTarget(Program.R.Range - 50))
                            {
                                Program.R.CastIfHitchanceEquals(e, HitChance.High, true);
                            }
                        }
                    }
                }
            }
        }

        private static void Combo()
        {
            var t = TargetSelector.GetTarget(900, TargetSelector.DamageType.Physical);

            if (t.IsValidTarget())
            {
                if (Program.Menu.Item("ComboW", true).GetValue<bool>() && Program.W.IsReady() && t.IsValidTarget(Program.W.Range))
                {
                    Program.W.Cast();
                }

                if (Program.Menu.Item("ComboE", true).GetValue<StringList>().SelectedIndex != 2 && Program.E.IsReady())
                {
                    switch (Program.Menu.Item("ComboE", true).GetValue<StringList>().SelectedIndex)
                    {
                        case 0:
                            Program.E.Cast(t.ServerPosition);
                            break;
                        case 1:
                            Program.E.Cast(Game.CursorPos);
                            break;
                    }
                }

                if (Program.Menu.Item("ComboR", true).GetValue<bool>())
                {
                    if (Program.R.IsReady())
                    {
                        switch (Program.R.Instance.Name)
                        {
                            case "RivenFengShuiEngine":
                                if (Program.Menu.Item("R1Combo", true).GetValue<KeyBind>().Active)
                                {
                                    if (t.Distance(Program.Me.ServerPosition) < Program.E.Range + Program.Me.AttackRange && Program.Me.CountEnemiesInRange(500) >= 1 &&
                                        !t.IsDead)
                                    {
                                        Program.R.Cast();
                                    }
                                }
                                break;
                            case "RivenIzunaBlade":
                                if (t.IsValidTarget(850) && !t.IsDead)
                                {
                                    switch (Program.Menu.Item("R2Mode", true).GetValue<StringList>().SelectedIndex)
                                    {
                                        case 0:
                                            if (Program.R.GetDamage(t) > t.Health && t.IsValidTarget(Program.R.Range) && t.Distance(Program.Me.ServerPosition) < 600)
                                            {
                                                Program.R.Cast(t);
                                            }
                                            break;
                                        case 1:
                                            if (t.HealthPercent < 25 && t.Health > Program.R.GetDamage(t) + Program.Me.GetAutoAttackDamage(t) * 2)
                                            {
                                                Program.R.Cast(t);
                                            }
                                            break;
                                        case 2:
                                            if (t.IsValidTarget(Program.R.Range) && t.Distance(Program.Me.ServerPosition) < 600)
                                            {
                                                Program.R.Cast(t);
                                            }
                                            break;
                                    }
                                }
                                break;
                        }
                    }
                }
            }
        }

        private static void Brust()
        {
            var e = TargetSelector.GetSelectedTarget();

            if (e != null && !e.IsDead && e.IsValidTarget() && !e.IsZombie)
            {
                if (Program.R.IsReady())
                {
                    if (Program.Me.HasBuff("RivenFengShuiEngine") & Program.Q.IsReady() && Program.E.IsReady() && Program.W.IsReady() &&
                        e.Distance(Program.Me.ServerPosition) < Program.E.Range + Program.Me.AttackRange + 100)
                    {
                        Program.E.Cast(e.Position);
                    }

                    if (Program.E.IsReady() && e.Distance(Program.Me.ServerPosition) < Program.Me.AttackRange + Program.E.Range + 100)
                    {
                        Program.R.Cast();
                        Program.E.Cast(e.Position);
                    }
                }

                if (Program.W.IsReady() && HeroManager.Enemies.Any(x => x.IsValidTarget(Program.W.Range)))
                {
                    Program.W.Cast();
                }

                if (Program.QStack == 1 || Program.QStack == 2 || e.HealthPercent < 50)
                {
                    if (Program.Me.HasBuff("RivenWindScarReady"))
                    {
                        Program.R.Cast(e);
                    }
                }

                if (Program.Menu.Item("BurstIgnite", true).GetValue<bool>() && Program.Ignite != SpellSlot.Unknown)
                {
                    if (e.HealthPercent < 50)
                    {
                        if (Program.Ignite.IsReady())
                        {
                            Program.Me.Spellbook.CastSpell(Program.Ignite, e);
                        }
                    }
                }

                if (Program.Menu.Item("BurstFlash", true).GetValue<bool>() && Program.Flash != SpellSlot.Unknown)
                {
                    if (Program.Flash.IsReady() && Program.R.IsReady() && Program.R.Instance.Name == "RivenFengShuiEngine" && Program.E.IsReady() &&
                        Program.W.IsReady() && e.Distance(Program.Me.ServerPosition) <= 780 &&
                        e.Distance(Program.Me.ServerPosition) >= Program.E.Range + Program.Me.AttackRange + 85)
                    {
                        Program.R.Cast();
                        Program.E.Cast(e.Position);
                        Utility.DelayAction.Add(150, () => { Program.Me.Spellbook.CastSpell(Program.Flash, e.Position); });
                    }
                }
            }
        }

        private static void Harass()
        {
            if (Program.Menu.Item("HarassW", true).GetValue<bool>() && Program.W.IsReady())
            {
                var t = HeroManager.Enemies.Find(x => x.IsValidTarget(Program.W.Range) && !x.HasBuffOfType(BuffType.SpellShield));

                if (t != null)
                {
                    Program.W.Cast();
                }
            }
        }

        private static void QuickHarass()
        {
            var t = TargetSelector.GetSelectedTarget();

            if (t != null && t.IsValidTarget())
            {
                if (Program.QStack == 2)
                {
                    if (Program.E.IsReady())
                    {
                        Program.E.Cast(Program.Me.ServerPosition + (Program.Me.ServerPosition - t.ServerPosition).Normalized() * Program.E.Range);
                    }

                    if (!Program.E.IsReady())
                    {
                        Program.Q.Cast(Program.Me.ServerPosition + (Program.Me.ServerPosition - t.ServerPosition).Normalized() * Program.E.Range);
                    }
                }

                if (Program.W.IsReady())
                {
                    if (t.IsValidTarget(Program.W.Range) && Program.QStack == 1)
                    {
                        Program.W.Cast();
                    }
                }

                if (Program.Q.IsReady())
                {
                    if (Program.QStack == 0)
                    {
                        if (t.IsValidTarget(Program.Me.AttackRange + Program.Me.BoundingRadius + 150))
                        {
                            Program.CastQ(t);
                        }
                    }
                }
            }
        }

        private static void LaneClear()
        {
            if (Program.Menu.Item("LaneClearW", true).GetValue<bool>())
            {
                var minions = MinionManager.GetMinions(Program.Me.ServerPosition, Program.W.Range);

                if (Program.W.IsReady() && minions.Count >= 3)
                {
                    Program.W.Cast();
                }
            }
        }

        private static void FleeLogic()
        {
            var e = HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(Program.W.Range) && !enemy.HasBuffOfType(BuffType.SpellShield));

            if (Program.W.IsReady() && e.FirstOrDefault().IsValidTarget(Program.W.Range))
            {
                Program.W.Cast();
            }

            if (Program.E.IsReady() && !Program.Me.IsDashing())
            {
                Program.E.Cast(Program.Me.Position.Extend(Game.CursorPos, 300));
            }
            else if (Program.Q.IsReady() && !Program.Me.IsDashing())
            {
                Program.Q.Cast(Game.CursorPos);
            }
        }

        private static void WallJump()
        {
            if (Program.Q.IsReady() && Program.QStack != 2)
            {
                Program.Q.Cast(Game.CursorPos);
            }

            //Thanks Asuvril

            var wallCheck = VectorHelper.GetFirstWallPoint(Program.Me.Position, Game.CursorPos);
            if (wallCheck != null)
            {
                wallCheck = VectorHelper.GetFirstWallPoint((Vector3)wallCheck, Game.CursorPos, 5);
            }

            var movePosition = wallCheck != null ? (Vector3)wallCheck : Game.CursorPos;
            var tempGrid = NavMesh.WorldToGrid(movePosition.X, movePosition.Y);
            Program.FleePosition = NavMesh.GridToWorld((short)tempGrid.X, (short)tempGrid.Y);

            if (wallCheck != null)
            {
                var wallPosition = movePosition;
                var direction = (Game.CursorPos.To2D() - wallPosition.To2D()).Normalized();
                const float maxAngle = 80f;
                const float step = maxAngle / 20;
                var currentAngle = 0f;
                var currentStep = 0f;

                while (true)
                {
                    if (currentStep > maxAngle && currentAngle < 0)
                    {
                        break;
                    }
                    if ((currentAngle == 0 || currentAngle < 0) && currentStep != 0)
                    {
                        currentAngle = (currentStep) * (float)Math.PI / 180;
                        currentStep += step;
                    }
                    else if (currentAngle > 0)
                    {
                        currentAngle = -currentAngle;
                    }

                    Vector3 checkPoint;

                    if (currentStep == 0)
                    {
                        currentStep = step;
                        checkPoint = wallPosition + 300 * direction.To3D();
                    }
                    else
                    {
                        checkPoint = wallPosition + 300 * direction.Rotated(currentAngle).To3D();
                    }
                    if (!checkPoint.ToNavMeshCell().CollFlags.HasFlag(CollisionFlags.Wall) &&
                        !checkPoint.ToNavMeshCell().CollFlags.HasFlag(CollisionFlags.Building))
                    {
                        wallCheck = VectorHelper.GetFirstWallPoint(checkPoint, wallPosition);
                        if (wallCheck != null)
                        {
                            var firstWallPoint = VectorHelper.GetFirstWallPoint((Vector3)wallCheck, wallPosition);

                            if (firstWallPoint != null)
                            {
                                var wallPositionOpposite = (Vector3)firstWallPoint;

                                if (Math.Sqrt(Program.Me.GetPath(wallPositionOpposite).Sum(o => o.To2D().LengthSquared())) - Program.Me.Distance(wallPositionOpposite) > 200)
                                {
                                    if (Program.Me.Distance(wallPositionOpposite, true) < Math.Pow(300 - Program.Me.BoundingRadius / 2, 5) && Program.QStack == 2)
                                    {
                                        Program.TargetPosition = wallPositionOpposite;

                                        if (Program.E.IsReady())
                                        {
                                            Program.E.Cast(Game.CursorPos);
                                        }
                                        else if (!Program.E.IsReady())
                                        {
                                            Program.Q.Cast(Game.CursorPos);
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}