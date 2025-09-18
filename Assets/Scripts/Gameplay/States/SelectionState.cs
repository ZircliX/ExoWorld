using System;
using OverBang.GameName.Core;
using OverBang.GameName.Core.Characters;

namespace OverBang.GameName.Gameplay.States
{
    public class SelectionState : IGameState
    {
        private readonly ICharacterSelectionService selectionService;
        private readonly ICharacterSpawnService spawnService;
        private readonly Action<IGameState> stateTransition;

        public SelectionState(
            ICharacterSelectionService selectionService,
            ICharacterSpawnService spawnService,
            Action<IGameState> stateTransition)
        {
            this.selectionService = selectionService;
            this.spawnService = spawnService;
            this.stateTransition = stateTransition;
        }

        public void Enter()
        {
            selectionService.StartCharacterSelection(OnCharacterSelected);
        }

        private void OnCharacterSelected(CharacterData character)
        {
            spawnService.SpawnCharacter(character);
            stateTransition(new HubState(stateTransition));
        }

        public void Exit() { }
    }
}