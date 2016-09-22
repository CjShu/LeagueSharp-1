namespace Flowers_Riven
{
    using LeagueSharp;
    using LeagueSharp.Common;

    internal class Others
    {
        public static void Init()
        {
            AntiGapcloser.OnEnemyGapcloser += delegate (ActiveGapcloser gapcloser)
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
            };

            Interrupter2.OnInterruptableTarget += delegate (Obj_AI_Hero sender, Interrupter2.InterruptableTargetEventArgs Args)
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
            };
        }
    }
}