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

        public  static DateTime lastTargetSkillTime = DateTime.MinValue;

        private static void TargetThread(object? state)
        {
            Target.Change(Timeout.Infinite, Timeout.Infinite);
            try
            {
                if (!Globals.Config.GeneralStatus || !Globals.Config.TargetingStatus || Globals.Process == null || Globals.Process.HasExited || !Objects.Player.isLoggedIn)
                    return;

                // Auto Target Area

                List<Creature> creatures = Battlelist.getCreaturesOnScreen().FindAll(cr => cr.Type == CreatureType.Monster && cr.HealthPc > 0);

                if (creatures.Count <= 0)
                    return;

                Objects.Client.targetNear();

                if (Globals.Config.follow_Target && Objects.Player.isAttacking)
                {
                    Creature creatureToFollow = creatures.Find(c => c.Id == Player.TargetId);
                    if (creatureToFollow != null)
                        Objects.Player.Goto(creatureToFollow.Position);
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (Globals.Config.GeneralStatus && Globals.Config.TargetingStatus)
                    Target.Change(500, Timeout.Infinite);
            }
        }
        private static void SpellCasterThread(object? state)
        {
            SpellCaster.Change(Timeout.Infinite, Timeout.Infinite);
            try
            {
                if (!Globals.Config.GeneralStatus || !Globals.Config.TargetingStatus || Globals.Process == null || Globals.Process.HasExited)
                    return;

                // Spell Caster Area

                bool hasAttackCooldown = Objects.Client.hasCooldown(CooldownGroup.Attack);

                List<Creature> creatures = Battlelist.getCreaturesOnScreen().FindAll(cr => cr.Type == CreatureType.Monster && cr.HealthPc > 0);
                Position playerPos = Player.Position;

                int playerMana = Objects.Player.Mana;
                int playerLevel = Objects.Player.Level;
                Creature target = creatures.Find(c => c.Id == Player.TargetId);
                Position targetPos = target !=null ? target.Position : new Position() { X = 0, Y = 0, Z = 0};

                bool didCastSkill = false;

                foreach (TargetRule rule in Globals.Config.Targeting)
                {
                    if (rule.Type == TargetType.Support && !Objects.Client.hasCooldown(CooldownGroup.Support))
                        continue;
                    else if (!hasAttackCooldown)
                        continue;

                    if (rule.Delay > 0 && (DateTime.Now - rule.LastUse).TotalMilliseconds <= rule.Delay)
                        continue;

                    if (Globals.ComboStatus && Globals.Config.ignore_Creature_Count_on_Combo)
                    { }
                    else if (rule.PlayerOnCenter && creatures.Where(c => c.Position.distanceTo(playerPos) <= rule.Range).Count() < rule.CreatureCount)
                        continue;
                    else if (!rule.PlayerOnCenter && creatures.Where(c => c.Position.distanceTo(targetPos) <= rule.Range).Count() < rule.CreatureCount)
                        continue;

                    if (rule.Mana > playerMana)
                        continue;

                    if (rule.Level > 0 && playerLevel < rule.Level)
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
                if (Globals.Config.GeneralStatus && Globals.Config.TargetingStatus)
                    SpellCaster.Change(100, Timeout.Infinite);
            }
        }
    }
}
