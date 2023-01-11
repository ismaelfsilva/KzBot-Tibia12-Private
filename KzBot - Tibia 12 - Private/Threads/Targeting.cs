using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KzBot.Objects;

namespace KzBot.Threads
{
    public static class Targeting
    {
        public static System.Threading.Timer Target = new System.Threading.Timer(TargetThread, null, Timeout.Infinite, Timeout.Infinite);
        public static System.Threading.Timer SpellCaster = new System.Threading.Timer(SpellCasterThread, null, Timeout.Infinite, Timeout.Infinite);

        public static DateTime lastTargetSkillTime = DateTime.MinValue;
        public static DateTime lastUtitoTime = DateTime.MinValue;

        public static int creaturesToTarget = 0;

        private static void TargetThread(object? state)
        {
            Target.Change(Timeout.Infinite, Timeout.Infinite);
            try
            {
                if (!Globals.ScriptConfig.GeneralStatus || !Globals.ScriptConfig.TargetingStatus || Globals.Process == null || Globals.Process.HasExited || !Objects.Player.isLoggedIn || !Objects.Player.isAlive())
                    return;

                // Auto Target Area

                if (Globals.ScriptConfig.only_target_on_lures && !Globals.ComboStatus)
                    return;

                List<Creature> creatures = Battlelist.getCreaturesOnScreen().FindAll(cr => cr.Type == CreatureType.Monster && cr.HealthPc > 0 && !Globals.ScriptConfig.ignore_List.Contains(cr.Name));
                creaturesToTarget = creatures.Count;

                if (creaturesToTarget <= 0)
                    return;

                Objects.Client.targetNear(creatures);

                if (Globals.ScriptConfig.follow_Target && Objects.Player.isAttacking)
                {
                    Creature creatureToFollow = creatures.Find(c => c.Id == Player.TargetId);
                    if (creatureToFollow != null && creatureToFollow.Position.distanceTo(Objects.Player.Position) > 1)
                        Objects.Player.Goto(creatureToFollow.Position);
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (Globals.ScriptConfig.GeneralStatus && Globals.ScriptConfig.TargetingStatus)
                    Target.Change(500, Timeout.Infinite);
            }
        }
        private static void SpellCasterThread(object? state)
        {
            SpellCaster.Change(Timeout.Infinite, Timeout.Infinite);
            try
            {
                if (!Globals.ScriptConfig.GeneralStatus || !Globals.ScriptConfig.TargetingStatus || Globals.Process == null || Globals.Process.HasExited || !Objects.Player.isLoggedIn || !Objects.Player.isAlive())
                    return;

                // Spell Caster Area

                bool hasAttackCooldown = Objects.Client.hasCooldown(CooldownGroup.Attack);

                List<Creature> creatures = Battlelist.getCreaturesOnScreen().FindAll(cr => cr.Type == CreatureType.Monster && cr.HealthPc > 0 && !Globals.ScriptConfig.ignore_List.Contains(cr.Name));
                Position playerPos = Player.Position;

                int playerMana = Objects.Player.Mana;
                int playerLevel = Objects.Player.Level;
                Creature target = creatures.Find(c => c.Id == Player.TargetId);
                Position targetPos = target !=null ? target.Position : new Position() { X = 0, Y = 0, Z = 0};

                bool didCastSkill = false;

                if (Globals.ScriptConfig.auto_Utito && Globals.ComboStatus && (DateTime.Now - lastUtitoTime).TotalMilliseconds > 10000 && playerLevel >= 60 && playerMana >= 290 && Objects.Client.hasCooldown(CooldownGroup.Support))
                {
                    Keyboard.PressKey((Keys)Properties.Settings.Default.Utito_Key);
                    lastUtitoTime = DateTime.Now;
                }

                foreach (TargetRule rule in Globals.ScriptConfig.Targeting)
                {
                    if (Globals.AccVocation != Vocation.None && rule.Vocation != Vocation.None && Globals.AccVocation != rule.Vocation)
                        continue;

                    if ((rule.Level > 0 && playerLevel < rule.Level) || (rule.MaxLevel > 0 && playerLevel > rule.MaxLevel))
                        continue;

                    if (rule.ComboOnly && !Globals.ComboStatus)
                        continue;

                    if (rule.Type == TargetType.Support && !Objects.Client.hasCooldown(CooldownGroup.Support))
                        continue;
                    else if (!hasAttackCooldown)
                        continue;

                    if (rule.Delay > 0 && (DateTime.Now - rule.LastUse).TotalMilliseconds <= rule.Delay)
                        continue;

                    if (Globals.ComboStatus && Globals.ScriptConfig.ignore_Creature_Count_on_Combo)
                    { }
                    else if (rule.PlayerOnCenter && creatures.Where(c => c.Position.distanceTo(playerPos) <= rule.Range).Count() < rule.CreatureCount)
                        continue;
                    else if (!rule.PlayerOnCenter && creatures.Where(c => c.Position.distanceTo(targetPos) <= rule.Range).Count() < rule.CreatureCount)
                        continue;

                    if (rule.Mana > playerMana)
                        continue;

                    lastTargetSkillTime = DateTime.Now;
                    didCastSkill = true;
                    Keyboard.PressKey(rule.Key);
                    rule.LastUse = DateTime.Now;
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (Globals.ScriptConfig.GeneralStatus && Globals.ScriptConfig.TargetingStatus)
                    SpellCaster.Change(100, Timeout.Infinite);
            }
        }
    }
}
