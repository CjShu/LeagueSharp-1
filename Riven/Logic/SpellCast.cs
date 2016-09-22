namespace Flowers_Riven
{
    using LeagueSharp;
    using LeagueSharp.Common;

    internal class SpellCast
    {
        public static void Init()
        {
            Obj_AI_Base.OnProcessSpellCast += delegate (Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
            {
                if (sender.IsMe)
                {
                    if (Args.SData.Name.Contains("RivenTriCleave"))
                    {
                        Program.CanQ = false;
                    }
                }
            };

            Obj_AI_Base.OnPlayAnimation += delegate (Obj_AI_Base sender, GameObjectPlayAnimationEventArgs Args)
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
            };
        }
    }
}