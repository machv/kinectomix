using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mach.Xna.Input
{
    /// <summary>
    /// Handles input from multiple input providers.
    /// </summary>
    public class MutipleInputProvider : IInputProvider
    {
        /// <summary>
        /// List of observed <see cref="IInputProvider"/>s.
        /// </summary>
        protected List<IInputProvider> _providers;

        /// <summary>
        /// Initializes new instance of <see cref="MutipleInputProvider"/> class.
        /// </summary>
        public MutipleInputProvider()
        {
            _providers = new List<IInputProvider>();
        }

        /// <summary>
        /// Adds <see cref="IInputProvider"/>.
        /// </summary>
        /// <param name="provider">Provider to add.</param>
        public void AddProvider(IInputProvider provider)
        {
            _providers.Add(provider);
        }

        /// <summary>
        /// Returns first input state with active state from first provider or null if no provider is registered.
        /// </summary>
        /// <returns>Active <see cref="IInputState"/>.</returns>
        public IInputState GetState()
        {
            if (_providers.Count == 0)
                return null;

            List<IInputState> states = new List<IInputState>();

            // Find first with active selection.
            foreach (var provider in _providers)
            {
                IInputState state = provider.GetState();
                if (state == null)
                    continue;

                if (state.IsStateActive)
                    return state;

                states.Add(state);
            }

            // If no active, find first with non-zero position.
            foreach (IInputState state in states)
            {
                if (state == null)
                    continue;

                if (state.X != 0 && state.Y != 0)
                    return state;
            }

            // Fall back to first state.
            return _providers[0].GetState();
        }
    }
}
