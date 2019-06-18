using System.Collections.Generic;

namespace Core
{
    internal class SpellManager
    {
        internal static int SpellAliveCount;

        private readonly WorldManager worldManager;
        private readonly List<Spell> activeSpells = new List<Spell>();
        private readonly List<Spell> spellsToRemove = new List<Spell>();
        private readonly List<Spell> spellsToAdd = new List<Spell>();

        private bool IsProcessing { get; set; }

        internal SpellManager(WorldManager worldManager)
        {
            this.worldManager = worldManager;
        }

        internal void Dispose()
        {
            activeSpells.ForEach(spell => spell.Dispose());
            spellsToRemove.ForEach(spell => spell.Dispose());
            spellsToAdd.ForEach(spell => spell.Dispose());

            activeSpells.Clear();
            spellsToRemove.Clear();
            spellsToAdd.Clear();
        }

        internal void DoUpdate(int deltaTime)
        {
            IsProcessing = true;

            foreach (var spell in activeSpells)
                spell.DoUpdate(deltaTime);

            IsProcessing = false;

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
    }
}
