namespace Flowers_Riven
{
    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;
    using System;

    internal class Program
    {
        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;
        public static SpellSlot Ignite = SpellSlot.Unknown;
        public static SpellSlot Flash = SpellSlot.Unknown;
        public static Menu Menu;
        public static Obj_AI_Hero Me;
        public static bool CanQ;
        public static bool CastR2;
        public static bool CanFlash;
        public static Vector3 FleePosition = Vector3.Zero;
        public static Vector3 TargetPosition = Vector3.Zero;
        public static int QStack;
        public static Orbwalking.Orbwalker Orbwalker;
        public static AttackableUnit QTarget;
        public static int SkinID;

        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnLoad;
        }

        private static void OnLoad(EventArgs args)
        {
            if (ObjectManager.Player.ChampionName != "Riven")
                return;

            Game.PrintChat("<font color='#2848c9'>Flowers Riven</font> --> <font color='#b756c5'>Load! </font> <font size='30'><font color='#d949d4'>Good Luck!</font></font>");

            Me = ObjectManager.Player;

            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W, 270f);
            E = new Spell(SpellSlot.E, 312f);
            R = new Spell(SpellSlot.R, 900f) { MinHitChance = HitChance.High };
            R.SetSkillshot(0.25f, 45f, 1600f, false, SkillshotType.SkillshotCone);

            Ignite = Me.GetSpellSlot("SummonerDot");
            Flash = Me.GetSpellSlot("SummonerFlash");

            SkinID = Me.BaseSkinId;

            InitMenu.Init();
            LoopEvent.Init();
            Drawings.Init();
            SpellCast.Init();
            DoCast.Init();
            Others.Init();
        }

        public static void EnbaleSkin(object obj, OnValueChangeEventArgs Args)
        {
            if (!Args.GetNewValue<bool>())
            {
                ObjectManager.Player.SetSkin(ObjectManager.Player.ChampionName, SkinID);
            }
        }

        public static void CastItem(bool tiamat = false, bool youmuu = false)
        {
            if (tiamat)
            {
                if (Items.HasItem(3077) && Items.CanUseItem(3077))
                {
                    Items.UseItem(3077);
                }

                if (Items.HasItem(3074) && Items.CanUseItem(3074))
                {
                    Items.UseItem(3074);
                }

                if (Items.HasItem(3053) && Items.CanUseItem(3053))
                {
                    Items.UseItem(3053);
                }
            }

            if (youmuu)
            {
                if (Items.HasItem(3142) && Items.CanUseItem(3142))
                {
                    Items.UseItem(3142);
                }
            }
        }

        public static void CastQ(AttackableUnit target)
        {
            CanQ = true;
            QTarget = target;
        }
    }
}