namespace Flowers_Riven
{
    using System.Linq;
    using Common;
    using LeagueSharp;
    using LeagueSharp.Common;

    internal class DoCast
    {
        internal static void Init()
        {
            Obj_AI_Base.OnDoCast += OnDoCast;
        }

        private static void OnDoCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (!sender.IsMe || !Orbwalking.IsAutoAttack(Args.SData.Name))
            {
                return;
            }

            if (Args.Target == null)
            {
                return;
            }

            Program.QTarget = (Obj_AI_Base)Args.Target;

            if (Program.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                var target = Args.Target as Obj_AI_Hero;

                if (target != null && !target.IsDead && !target.IsZombie)
                {
                    Program.CastItem(true, true);

                    if (Program.Q.IsReady())
                    {
                        Program.CastQ(target);
                    }
                    else if (Program.W.IsReady() && target.IsValidTarget(Program.W.Range))
                    {
                        Program.W.Cast(target.Position);
                    }
                    else if (Program.E.IsReady() && target.DistanceToPlayer() > 
                        Orbwalking.GetRealAutoAttackRange(Program.Me) &&
                             target.IsValidTarget(Program.E.Range))
                    {
                        Program.E.Cast(target.Position);
                    }
                }
            }

            if (Program.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Brust)
            {
                var target = Args.Target as Obj_AI_Hero;

                if (target != null && !target.IsDead && !target.IsZombie)
                {
                    Program.CastItem(true, true);

                    if (Program.Q.IsReady())
                    {
                        Program.CastQ(target);
                    }
                    else if (Program.W.IsReady() && target.IsValidTarget(Program.W.Range))
                    {
                        Program.W.Cast(target.Position);
                    }
                    else if (Program.E.IsReady() && target.DistanceToPlayer() > 
                        Orbwalking.GetRealAutoAttackRange(Program.Me) &&
                             target.IsValidTarget(Program.E.Range))
                    {
                        Program.E.Cast(target.Position);
                    }
                }
            }

            if (Program.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.QuickHarass)
            {
                var target = Args.Target as Obj_AI_Hero;

                if (target != null && !target.IsDead && !target.IsZombie)
                {
                    Program.CastItem(true);

                    if (Program.Q.IsReady() && Program.QStack != 2 &&
                        Program.Menu.Item("HarassQ", true).GetValue<bool>())
                    {
                        Program.CastQ(target);
                    }
                }
            }

            if (Program.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                var target = Args.Target as Obj_AI_Hero;

                if (target != null && !target.IsDead && !target.IsZombie)
                {
                    Program.CastItem(true);

                    if (Program.Q.IsReady() && Program.Menu.Item("HarassQ", true).GetValue<bool>())
                    {
                        Program.CastQ(target);
                    }
                }
            }

            if (Program.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                LaneClear(Args);
                JungleClear(Args);
            }
        }

        private static void LaneClear(GameObjectProcessSpellCastEventArgs Args)
        {
            if (Program.Menu.Item("LaneClearQ", true).GetValue<bool>() && Program.Q.IsReady())
            {
                if (Args.Target.Type == GameObjectType.obj_AI_Turret || Args.Target.Type == GameObjectType.obj_Turret)
                {
                    if (Program.Q.IsReady() && !Args.Target.IsDead)
                    {
                        Program.CastQ((Obj_AI_Base) Args.Target);
                    }
                }
                else
                {
                    var minion = Args.Target as Obj_AI_Minion;
                    var minions = MinionManager.GetMinions(Program.E.Range + Program.Me.AttackRange);

                    if (minion != null)
                    {
                        Program.CastItem(true);

                        if (minions.Count >= 2)
                        {
                            Program.CastQ(minion);
                        }
                    }
                }
            }
        }

        private static void JungleClear(GameObjectProcessSpellCastEventArgs Args)
        {
            if (Args.Target is Obj_AI_Minion)
            {
                var mobs = MinionManager.GetMinions(Program.E.Range + Program.Me.AttackRange, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
                var mob = mobs.FirstOrDefault();

                if (mob != null)
                {
                    Program.CastItem(true);

                    if (Program.Menu.Item("JungleClearQ", true).GetValue<bool>() && Program.Q.IsReady())
                    {
                        Program.CastQ(mob);
                    }
                    else if (Program.Menu.Item("JungleClearW", true).GetValue<bool>() && Program.W.IsReady() && mob.IsValidTarget(Program.W.Range))
                    {
                        Program.W.Cast(mob.Position);
                    }
                    else if (Program.Menu.Item("JungleClearE", true).GetValue<bool>() && Program.E.IsReady())
                    {
                        if (mob.HasBuffOfType(BuffType.Stun) && !Program.W.IsReady())
                        {
                            Program.E.Cast(mob.Position);
                        }
                        else if (!mob.HasBuffOfType(BuffType.Stun))
                        {
                            Program.E.Cast(mob.Position);
                        }
                    }
                }
            }
        }
    }
}