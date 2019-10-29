using System.Collections.Generic;
using Common;
using Core;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Spell Tooltip Info Container", menuName = "Game Data/Containers/Spell Tooltip Info", order = 1)]
    public class SpellTooltipInfoContainer : ScriptableUniqueInfoContainer<SpellTooltipInfo>
    {
        [SerializeField, UsedImplicitly] private List<SpellTooltipInfo> tooltipInfos;

        private readonly Dictionary<SpellInfo, SpellTooltipInfo> tooltipInfoBySpell = new Dictionary<SpellInfo, SpellTooltipInfo>();
        private readonly Dictionary<int, SpellTooltipInfo> tooltipInfoBySpellId = new Dictionary<int, SpellTooltipInfo>();

        protected override List<SpellTooltipInfo> Items => tooltipInfos;

        public IReadOnlyDictionary<SpellInfo, SpellTooltipInfo> TooltipInfoBySpell => tooltipInfoBySpell;
        public IReadOnlyDictionary<int, SpellTooltipInfo> TooltipInfoBySpellId => tooltipInfoBySpellId;

        public override void Register()
        {
            base.Register();

            foreach (SpellTooltipInfo tooltipInfo in tooltipInfos)
            {
                tooltipInfoBySpell.Add(tooltipInfo.SpellInfo, tooltipInfo);
                tooltipInfoBySpellId.Add(tooltipInfo.SpellInfo.Id, tooltipInfo);
            }
        }

        public override void Unregister()
        {
            tooltipInfoBySpell.Clear();
            tooltipInfoBySpellId.Clear();

            base.Unregister();
        }
    }
}
