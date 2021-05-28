using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Base
{
    public sealed class InputManager : AbstractService<IInputManager>, IInputManager
    {
        private List<IInputSource> inputSources = new List<IInputSource>();
        private List<IInputSource> inputSourceOverrides = new List<IInputSource>();

        [Inject] private IInputSourceFactory inputSourceFactory;

        public override void Start()
        {
            base.Start();
            inputSources.Add(inputSourceFactory.Spawn());
        }

        public IInputSource InputForPlayer(int playerIndex)
        {
            if (playerIndex >= 0) {
                if (playerIndex < inputSourceOverrides.Count && inputSourceOverrides[playerIndex] != null)
                    return inputSourceOverrides[playerIndex];
                if (playerIndex < inputSources.Count && inputSources[playerIndex] != null)
                    return inputSources[playerIndex];
            }

            return DummyInputSource.Instance;
        }

        public bool InputOverridenForPlayer(int playerIndex)
        {
            return (playerIndex >= 0
                 && playerIndex < inputSourceOverrides.Count
                 && inputSourceOverrides[playerIndex] != null);
        }

        public void OverrideInputForPlayer(int playerIndex, IInputSource overrideSource)
        {
            DebugOnly.Check(playerIndex >= 0, "Invalid player index.");

            if (overrideSource == null) {
                if (playerIndex < inputSourceOverrides.Count)
                    inputSourceOverrides[playerIndex] = null;
                return;
            }

            DebugOnly.Check(!InputOverridenForPlayer(playerIndex),
                "Attempted to install multiple overrides for player input.");

            while (playerIndex >= inputSourceOverrides.Count)
                inputSourceOverrides.Add(null);

            inputSourceOverrides[playerIndex] = overrideSource;
        }
    }
}
