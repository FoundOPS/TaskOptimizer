/* ===========================================================
 * WizardState.cs -- Project Nightingale 
 * 
 * To the extent possible under law, Aragorn Wyvernzora has 
 * waived all copyright and related or neighboring rights to 
 * WizardState.cs. 
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
    class WizardState : Panel
    {
        public WizardStateManager StateManager
        { get; set; }

        public new void BringToFront()
        {
            StateManager.SetCurrentState(this.Name);
        }

        public void ActivateState()
        {
            base.BringToFront();
            this.Enabled = true;
            this.Visible = true;

            if (m_onActivate != null)
                m_onActivate(this, new EventArgs());
        }
        public void DeactivateState()
        {
            base.SendToBack();
            this.Visible = false;
            this.Enabled = false;

            if (m_onDeactivated != null)
                m_onDeactivated(this, new EventArgs());
        }

        private EventHandler m_onActivate;
        public event EventHandler OnActivated
        { add { m_onActivate += value; } remove { m_onActivate -= value; } }

        private EventHandler m_onDeactivated;
        public event EventHandler OnDeactivated
        { add { m_onDeactivated += value; } remove { m_onDeactivated -= value; } }
    }
}
