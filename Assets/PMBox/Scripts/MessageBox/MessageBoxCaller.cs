using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Pashmak.MessageBox
{
    public class MessageBoxCaller : MonoBehaviour
    {
        // variable________________________________________________________________
        [SerializeField] private UnityEvent m_onShowMessage = new UnityEvent();


        // property________________________________________________________________
        public UnityEvent OnShowMessage
        {
            get
            {
                if (m_onShowMessage == null)
                    m_onShowMessage = new UnityEvent();
                return m_onShowMessage;
            }
            private set => m_onShowMessage = value;
        }


        // function________________________________________________________________
        public void ShowMessage(string profileId)
        {
            OnShowMessage.Invoke();
            MessageBoxManager.ShowMessage(profileId);
        }
    }
}