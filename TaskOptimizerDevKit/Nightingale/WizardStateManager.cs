/* ===========================================================
 * WizardStateManager.cs -- Project Nightingale 
 * 
 * 
 * 
 * To the extent possible under law, Aragorn Wyvernzora has 
 * waived all copyright and related or neighboring rights to 
 * WizardStateManager.cs. 
 * 
 * This work is published from: United States.
 * 
 * ==========================================================
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Nightingale
{
    class WizardStateManager
    {
        Dictionary<String, WizardState> m_states = new Dictionary<string, WizardState>();
        WizardState currentState = null;

        public void AddState(WizardState state)
        {
            if (!m_states.ContainsKey(state.Name))
            {
                state.StateManager = this;
                m_states.Add(state.Name, state);
                state.Visible = false;
                state.Enabled = false;
            }
            else
                throw new InvalidOperationException();
        }

        public void SetCurrentState(String name)
        {
            if (currentState != null && name == currentState.Name)
                return;
            if (!m_states.ContainsKey(name))
                throw new KeyNotFoundException();

            WizardState newState = m_states[name];
            newState.ActivateState();
            if (currentState != null)
                currentState.DeactivateState();
            currentState = newState;
        }

        public Panel CurrentState
        {
            get { return currentState; }
        }
    }
}
