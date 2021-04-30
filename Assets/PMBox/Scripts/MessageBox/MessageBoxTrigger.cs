using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Pashmak.MessageBox
{
    public class MessageBoxTrigger : MonoBehaviour
    {
        // variable________________________________________________________________
        [SerializeField] private bool m_isActive = true;
        [SerializeField] private UnityEvent m_onShowMessage = new UnityEvent();
        [SerializeField] private UnityEvent m_onHideMessage = new UnityEvent();


        // property________________________________________________________________
        public bool IsActive { get => m_isActive; set => m_isActive = value; }
        public UnityEvent OnShowMessage { get => m_onShowMessage; private set => m_onShowMessage = value; }
        public UnityEvent OnHideMessage { get => m_onHideMessage; private set => m_onHideMessage = value; }


        // monoBehaviour___________________________________________________________
        void Start()
        {
            MessageBoxManager.Instance.m_onShowMessage.AddListener(() =>
            {
                if (IsActive)
                    OnShowMessage.Invoke();
            });
            MessageBoxManager.Instance.m_onHideMessage.AddListener(() =>
            {
                if (IsActive)
                    OnHideMessage.Invoke();
            });
        }
    }
}