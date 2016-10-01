namespace Flowers_Riven
{
    using LeagueSharp;
    using LeagueSharp.Common;
    using Common;

    internal class SpellCast
    {
        public static void Init()
        {
            Spellbook.OnCastSpell += OnCastSpell;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Obj_AI_Base.OnPlayAnimation += OnPlayAnimation;
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += OnInterruptableTarget;
        }

        private static void OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs Args)
        {
            if (Program.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                if (Args.Slot == SpellSlot.Q)
                {
                    if (Program.Menu.Item("ComboR", true).GetValue<bool>() && Program.R.IsReady() &&
                        Program.R.Instance.Name == "RivenIzunaBlade")
                    {
                        var t = TargetSelector.GetTarget(900, TargetSelector.DamageType.Physical);

                        if (t.IsValidTarget() && !t.IsDead)
                        {
                            switch (Program.Menu.Item("R2Mode", true).GetValue<StringList>().SelectedIndex)
                            {
                                case 0:
                                    if (Program.R.GetDamage(t) > t.Health && t.IsValidTarget(Program.R.Range) &&
                                        t.DistanceToPlayer() < 600)
                                    {
                                        Program.R.Cast(t);
                                    }
                                    break;
                                case 1:
                                    if (t.HealthPercent < 25 && t.Health > Program.R.GetDamage(t) +
                                        Program.Me.GetAutoAttackDamage(t)*2)
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
                    }
                }
            }
        }

        private static void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Program.Menu.Item("AntiGapCloserW", true).GetValue<bool>())
            {
                if (Program.W.IsReady())
                {
                    if (gapcloser.Sender.IsValidTarget(Program.W.Range))
                    {
                        Program.W.Cast();
                    }
                }
            }
        }

        private static void OnInterruptableTarget(Obj_AI_Hero sender, Interrupter2.InterruptableTargetEventArgs Args)
        {
            if (Program.Menu.Item("InterruptTargetW", true).GetValue<bool>())
            {
                if (Args.DangerLevel >= Interrupter2.DangerLevel.High)
                {
                    if (sender.IsValidTarget(Program.W.Range))
                    {
                        if (Program.W.IsReady())
                        {
                            Program.W.Cast();
                        }
                    }
                }
            }
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            if (Args.SData.Name.Contains("RivenTriCleave"))
            {
                Program.CanQ = false;
            }
        }

        private static void OnPlayAnimation(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs Args)
        {
            if (!sender.IsMe)
                return;

            if (Program.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.None)
                return;

            switch (Args.Animation)
            {
                case "Spell1a":
                    Program.QStack = 1;
                    if (Program.Menu.Item("Dance", true).GetValue<bool>())
                    {
                        Game.SendEmote(Emote.Dance);
                    }
                    Utility.DelayAction.Add(281, () =>
                    {
                        Game.SendEmote(Emote.Dance);
                        Program.Me.IssueOrder(GameObjectOrder.MoveTo, Program.Me.Position.Extend(Game.CursorPos, -10));
                        Orbwalking.ResetAutoAttackTimer();
                    });
                    break;
                case "Spell1b":
                    if (Program.Menu.Item("Dance", true).GetValue<bool>())
                    {
                        Game.SendEmote(Emote.Dance);
                    }
                    Utility.DelayAction.Add(281, () =>
                    {
                        Game.SendEmote(Emote.Dance);
                        Program.Me.IssueOrder(GameObjectOrder.MoveTo, Program.Me.Position.Extend(Game.CursorPos, -10));
                        Orbwalking.ResetAutoAttackTimer();
                    });
                    break;
                case "Spell1c":
                    if (Program.Menu.Item("Dance", true).GetValue<bool>())
                    {
                        Game.SendEmote(Emote.Dance);
                    }
                    Utility.DelayAction.Add(381, () =>
                    {
                        Game.SendEmote(Emote.Dance);
                        Program.Me.IssueOrder(GameObjectOrder.MoveTo, Program.Me.Position.Extend(Game.CursorPos, -10));
                        Orbwalking.ResetAutoAttackTimer();
                    });
                    break;
            }
        }
    }
}