using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atomix.Input
{
    /// <summary>
    /// Returns first positive state selection.
    /// </summary>
    public class MutipleInputProvider : IInputProvider
    {
        protected List<IInputProvider> providers;

        public MutipleInputProvider()
        {
            providers = new List<IInputProvider>();
        }

        public void AddProvider(IInputProvider provider)
        {
            providers.Add(provider);
        }

        /// <summary>
        /// Returns first input state with selection or from first provider or null if no provider is registered.
        /// </summary>
        /// <returns></returns>
        public IInputState GetState()
        {
            if (providers.Count == 0)
                return null;

            foreach (var provider in providers)
            {
                IInputState state = provider.GetState();
                if (state == null)
                    continue;

                if (state.IsSelected)
                    return state;
            }

            return providers[0].GetState();
        }
    }
}
