namespace Core
{
    public static class PlayerHelper
    {
        public const int PlayerCustomDisplaySize = 3;
        public const int PlayerMaxSkills = 128;
        public const int PlayerExploredZonesSize = 256;
        public const int QuestsCompletedBitsSize = 1000; // Size of client completed quests bit map
        public const int MaxQuestCounts = 24;
        public const int MaxQuestOffset = 16;
        public const int InventorySlotBag0 = 255;
        public const int VisibleItemEntryOffset = 0;
        public const int VisibleItemEnchantmentOffset = 1;
        public const int MaxEquipmentSetIndex = 20;
        public const int MaxPlayerSummonDelay = 2 * TimeHelper.Minute;  // Player summoning auto-decline time (in secs)
        public const int BattlePetSpeciesMaxID = 1986;

        public const int MaxTalentTiers = 7;
        public const int MaxTalentColumns = 3;

        public const int MaxQuestLogSize = 25;
        public const int QuestItemDropCount = 4;
        public const int QuestRewardChoicesCount = 6;
        public const int QuestRewardItemCount = 4;
        public const int QuestDeplinkCount = 10;
        public const int QuestRewardReputationsCount = 5;
        public const int QuestEmoteCount = 4;
        public const int QuestRewardCurrencyCount = 4;
        public const int QuestRewardDisplaySpellCount = 3;

        public const int MaxRunes = 5;
        public const int MaxRechargingRunes = 5;
        public const int MaxDrunken = 4;
        public const int KnownTitlesSize = 6;
        public const int MaxTitleIndex = KnownTitlesSize * 64;

        public const int MaxTimers = 3;
        public const int MaxPlayedTimeIndex = 2;
        public const int DisabledMirrorTimer = -1;

        public const int MaxDeclinedNameCases = 5;
        public const int SocialFriendLimit = 50;
        public const int SocialIgnoreLimit = 50;

        public const int MaxActionButtons = 132;
        public const long MaxActionButtonActionValue = 0xFFFFFFFF;
        public const long MaxMoneyAmount = long.MaxValue;

        public const int MailBodyItemTemplate = 8383;
        public const int MaxMailItems = 12;

        public static ulong ActionButtonAction(ulong x)
        {
            return x & 0x00000000FFFFFFFF;
        }

        public static ulong ActionButtonType(ulong x)
        {
            return (x & 0xFFFFFFFF00000000) >> 56;
        }
    }
}