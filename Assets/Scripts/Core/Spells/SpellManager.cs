using System.Collections.Generic;
using Zenject;

namespace Core
{
    public class SpellManager
    {
        [Inject]
        private UnitManager unitManager;

        private readonly List<Spell> activeSpells = new();
        private readonly List<Spell> spellsToRemove = new();
        private readonly List<Spell> spellsToAdd = new();

        private bool IsProcessing { get; set; }

        [Inject]
        private void Setup()
        {
            unitManager.EventEntityDetach += OnEntityDetach;

        }

        internal void Dispose()
        {
            unitManager.EventEntityDetach -= OnEntityDetach;

            activeSpells.ForEach(spell => spell.Dispose());
            spellsToRemove.ForEach(spell => spell.Dispose());
            spellsToAdd.ForEach(spell => spell.Dispose());

            activeSpells.Clear();
            spellsToRemove.Clear();
            spellsToAdd.Clear();
        }

        internal void DoUpdate(int deltaTime)
        {
            bool wasProcessing = IsProcessing;
            IsProcessing = true;

            foreach (var spell in activeSpells)
                spell.DoUpdate(deltaTime);

            IsProcessing = wasProcessing;

            for (int i = spellsToRemove.Count - 1; i >= 0; i--)
            {
                spellsToRemove[i].SpellState = SpellState.Active;
                Remove(spellsToRemove[i]);
            }

            for (int i = spellsToAdd.Count - 1; i >= 0; i--)
                Add(spellsToAdd[i]);

            spellsToRemove.Clear();
            spellsToAdd.Clear();
        }

        internal void Add(Spell spell)
        {
            if (IsProcessing)
            {
                spell.SpellState = SpellState.Adding;
                spellsToAdd.Add(spell);
            }
            else
            {
                spell.SpellState = SpellState.Active;
                activeSpells.Add(spell);
            }
        }

        internal void Remove(Spell spell)
        {
            if (spell.SpellState == SpellState.Disposed || spell.SpellState == SpellState.Removing)
                return;

            if (spell.SpellState == SpellState.Adding)
                spellsToAdd.Remove(spell);

            if (spell.SpellState == SpellState.Active)
            {
                if (IsProcessing)
                {
                    spellsToRemove.Add(spell);
                    spell.SpellState = SpellState.Removing;
                    return;
                }

                activeSpells.Remove(spell);
            }

            spell.Dispose();
        }

        private void OnEntityDetach(Unit unit)
        {
            bool wasProcessing = IsProcessing;
            IsProcessing = true;

            for (int i = activeSpells.Count - 1; i >= 0; i--)
                activeSpells[i].HandleUnitDetach(unit);

            for(int i = spellsToAdd.Count - 1; i >= 0; i--)
                spellsToAdd[i].HandleUnitDetach(unit);

            IsProcessing = wasProcessing;
        }
    }
}
