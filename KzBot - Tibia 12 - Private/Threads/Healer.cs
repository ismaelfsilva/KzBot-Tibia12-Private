using KzBot.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KzBot.Threads
{
    public static class Healer
    {
        public static System.Threading.Timer Thread = new System.Threading.Timer(HealerThread, null, Timeout.Infinite, Timeout.Infinite);
        public static bool energyRingEquipped = false;

        private static void HealerThread(object? state)
        {
            Thread.Change(Timeout.Infinite, Timeout.Infinite);
            try
            {
                if (!Globals.Config.GeneralStatus || !Globals.Config.HealerStatus || Globals.Process == null || Globals.Process.HasExited || !Objects.Player.isLoggedIn)
                    return;

                int playerHpPc = Objects.Player.Creature.HealthPc;
                int playerHp = Objects.Player.Health;

                int playerMp = Objects.Player.Mana;
                double playerMpOnePc = Objects.Player.ManaMax / 100;
                int playerMpPc = (int)Math.Round(playerMp / playerMpOnePc);

                int playerLevel = Objects.Player.Level;

                foreach (HealRule rule in Globals.Config.Healer)
                {
                    bool takeOutItem = false;

                    if (Globals.AccountId != -1 && (Globals.Accounts.List[Globals.AccountId].Vocation != Vocation.None && rule.Vocation != Vocation.None) && Globals.Accounts.List[Globals.AccountId].Vocation != rule.Vocation)
                        continue;

                    if (DateTime.Now < rule.LastUse.AddMilliseconds(rule.Delay))
                        continue;

                    if (rule.Type == HealType.EnergyRing)
                        energyRingEquipped = Objects.Player.isManaShielded;

                    // Check Hp % or Flat
                    if (rule.HpMax <= 100 && (playerHpPc < rule.HpMin || playerHpPc > rule.HpMax))
                    {
                        if ((rule.Type != HealType.SSA || !Objects.ClientData.hasAmulet) && (rule.Type != HealType.EnergyRing || !energyRingEquipped))
                            continue;
                        else
                            takeOutItem = true;
                    }
                    else if (rule.HpMax > 100 && (playerHp < rule.HpMin || playerHp > rule.HpMax))
                    {
                        if ((rule.Type != HealType.SSA || !Objects.ClientData.hasAmulet) && (rule.Type != HealType.EnergyRing || !energyRingEquipped))
                            continue;
                        else
                            takeOutItem = true;
                    }

                    // Check Mp % or Flat
                    if (rule.MpMax <= 100 && (playerMpPc < rule.MpMin || playerMpPc > rule.MpMax))
                    {
                        if ((rule.Type != HealType.SSA || !Objects.ClientData.hasAmulet) && (rule.Type != HealType.EnergyRing || !energyRingEquipped))
                            continue;
                        else
                            takeOutItem = true;
                    }
                    else if (rule.MpMax > 100 && (playerMp < rule.MpMin || playerMp > rule.MpMax))
                    {
                        if ((rule.Type != HealType.SSA || !Objects.ClientData.hasAmulet) && (rule.Type != HealType.EnergyRing || !energyRingEquipped))
                            continue;
                        else
                            takeOutItem = true;
                    }

                    // Only use Items if doesn't have Targeting Cooldown
                    //if (rule.Type == HealType.Item && Globals.Config.TargetingStatus && Objects.Client.hasCooldown(CooldownGroup.Attack))
                    //    continue;
                    if (rule.Type == HealType.Spell && !Objects.Client.hasCooldown(CooldownGroup.Heal))
                        continue;

                    if (!takeOutItem && rule.Type == HealType.SSA && Objects.ClientData.hasAmulet)
                        continue;

                    if (!takeOutItem && rule.Type == HealType.EnergyRing && energyRingEquipped)
                        continue;

                    if (rule.Type == HealType.Item && Globals.Config.Targeting.Exists(r => r.Type == TargetType.Item) && Objects.Client.HasAttackCooldown && (DateTime.Now - Threads.Targeting.lastTargetSkillTime).TotalMilliseconds <= 500)
                        continue;

                    if (rule.Level > 0 && playerLevel < rule.Level)
                        continue;

                    Keyboard.PressKey(rule.Key);
                    rule.LastUse = DateTime.Now;
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (Globals.Config.GeneralStatus && Globals.Config.HealerStatus)
                    Thread.Change(100, Timeout.Infinite);
            }
        }
    }
}
