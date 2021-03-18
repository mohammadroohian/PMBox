using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Pashmak.MessageBox
{
    [RequireComponent(typeof(AudioSource))]
    public class MessageBoxManager : MonoBehaviour
    {
        // variable________________________________________________________________
        private static MessageBoxManager m_instance;
        private static Queue<MessageBoxSkin> m_skins = new Queue<MessageBoxSkin>();
        private static AudioSource m_audioSource;
        private Dictionary<string, int> m_messagesDic = new Dictionary<string, int>();

        [SerializeField] private bool m_allowSendMessage = true;
        [SerializeField] AlignmentSections m_alignmentSections;
        [SerializeField] private MessageBoxProfileCollection m_profileCollection = null;
        private Dictionary<string, MessageBoxProfile> m_profileCollectionDict = null;

        public UnityEvent m_onShowMessage;
        public UnityEvent m_onHideMessage;


        // property________________________________________________________________
        public static MessageBoxManager Instance { get => m_instance; private set => m_instance = value; }
        public bool AllowSendMessage { get => m_allowSendMessage; }
        public static AudioSource AudioSource { get => MessageBoxManager.m_audioSource; set => MessageBoxManager.m_audioSource = value; }
        public AlignmentSections AlignmentSections { get => m_alignmentSections; set => m_alignmentSections = value; }


        // monoBehaviour___________________________________________________________
        void Awake()
        {
            // singleton
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            //  AudioSource
            m_audioSource = GetComponent<AudioSource>();

            // create dict
            m_profileCollectionDict = new Dictionary<string, MessageBoxProfile>();
            foreach (MessageBoxProfile profile in m_profileCollection.MessageBoxProfiles)
            {
                m_profileCollectionDict.Add(profile.ProfileID, profile);
            }
        }


        // function________________________________________________________________
        public static MessageBoxSkin ShowMessage(string profileID, string title, string description, Sprite picture, Dictionary<string, string> textDict = null, Dictionary<string, Sprite> imageDict = null)
        {
            if (!Instance.AllowSendMessage) return null;

            // text
            if (textDict == null && (title != null || description != null))
                textDict = new Dictionary<string, string>();
            if (title != null)
                textDict.Add(MessageBoxSkin.TITLE_ID, title);
            if (description != null)
                textDict.Add(MessageBoxSkin.DESCRIPTION_ID, description);

            // pic
            if (imageDict == null && picture != null)
            {
                imageDict = new Dictionary<string, Sprite>();
                imageDict.Add(MessageBoxSkin.PICTURE_ID, picture);
            }

            return ShowMessage(profileID, textDict, imageDict);
        }
        public static MessageBoxSkin ShowMessage(string profileID, Dictionary<string, string> textDict = null, Dictionary<string, Sprite> imageDict = null)
        {
            if (!Instance.AllowSendMessage) return null;

            MessageBoxSkin tmpMsgSkin = InstantiateMessageBox(profileID);//Instantiate MessageBox.
            tmpMsgSkin.SetContent(textDict, imageDict);
            tmpMsgSkin.Show();
            return tmpMsgSkin;
        }
        public static void ShowMessage(string profileID)
        {
            ShowMessage(profileID, null, null);
        }

        public void NoneStaticShowMessage(string profileID)
        {
            if (!Instance.AllowSendMessage) return;

            ShowMessage(profileID);
        }

        private static MessageBoxProfile GetProfile(string profileID)
        {
            string cleaned_key = CleanIDText(profileID);
            if (Instance.m_profileCollectionDict.ContainsKey(cleaned_key))
                return Instance.m_profileCollectionDict[cleaned_key];
            Debug.LogErrorFormat("there is no mach for message {0}!", cleaned_key);
            return null;
        }
        private static string CleanIDText(string profileID)
        {
            return profileID.ToLower().Trim();
        }
        private static RectTransform GetAlignmentPanel(Alignment alignment)
        {
            switch (alignment)
            {
                case Alignment.UpperLeft:
                    return Instance.AlignmentSections.m_upperLeftPanel;
                case Alignment.UpperCenter:
                    return Instance.AlignmentSections.m_upperCenterPanel;
                case Alignment.UpperRight:
                    return Instance.AlignmentSections.m_upperRightPanel;
                case Alignment.MiddleLeft:
                    return Instance.AlignmentSections.m_middleLeftPanel;
                case Alignment.MiddleCenter:
                    return Instance.AlignmentSections.m_middleCenterPanel;
                case Alignment.MiddleRight:
                    return Instance.AlignmentSections.m_middleRightPanel;
                case Alignment.LowerLeft:
                    return Instance.AlignmentSections.m_lowerLeftPanel;
                case Alignment.LowerCenter:
                    return Instance.AlignmentSections.m_lowerCenterPanel;
                case Alignment.LowerRight:
                    return Instance.AlignmentSections.m_lowerRightPanel;
                default:
                    return Instance.AlignmentSections.m_upperLeftPanel;
            }
        }
        private static MessageBoxSkin InstantiateMessageBox(string profileID)
        {
            MessageBoxProfile tmpProfile = GetProfile(profileID);
            RectTransform tmpParent = GetAlignmentPanel(tmpProfile.Alignment);

            // Instantiate.
            GameObject tmpObj = Instantiate
                (
                tmpProfile.Skin.gameObject,
                Vector3.zero,
                tmpParent.rotation
                ) as GameObject;
            tmpObj.transform.SetParent(tmpParent, false);

            MessageBoxSkin tmpSkin = tmpObj.GetComponent<MessageBoxSkin>();
            m_skins.Enqueue(tmpSkin); //Add to queue.
            Instance.m_onShowMessage.Invoke();
            tmpSkin.SetProfile(tmpProfile);
            return tmpSkin;
        }
    }
}