#define AGORA_RTC


#define AGORA_VOICE

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

#if AGORA_RTC
using Agora.Rtc;
using io.agora.rtc.demo;
#else
using io.agora.rtm.demo;
#endif

using System;

namespace Agora_RTC_Plugin.API_Example
{
    public class Home : MonoBehaviour
    {
        public InputField AppIdInupt;
        public InputField ChannelInput;
        public InputField TokenInput;

        public AppIdInput AppInputConfig;
        public GameObject CasePanel;
        public GameObject CaseScrollerView;

        public GameObject EventSystem;

        private string _playSceneName = "";



        private string[] _rtcNameList = {
#if AGORA_RTC
//#if AGORA_NUMBER_UID
            "BasicAudioCallScene",
            
            "AudioCallRouteScene",
            "AudioMixingScene",
            "AudioSpectrumScene",
            "ChannelMediaRelayScene",
            
            "CustomCaptureAudioScene",
            
            "CustomRenderAudioScene",
            "DeviceManagerScene",
            
            "JoinChannelVideoTokenScene",
            "JoinChannelWithUserAccountScene",
            "MediaPlayerScene",
            "MediaPlayerOpenWithMediaSourceScene",
            "MediaRecorderScene",
            "MetadataScene",
            "MusicPlayerScene",
            "PluginScene",
            "ProcessAudioRawDataScene",
            
            
            
            
            
            
            "SetEncryptionScene",
            
            "SpatialAudioWithMediaPlayerScene",
            "SpatialAudioWithUsers",
            "StartDirectCdnStreamingScene",
            "StartLocalVideoTranscoderScene",
            "StartRhythmPlayerScene",
            "StartRtmpStreamWithTranscodingScene",
            "StreamMessageScene",
            
            
            "VoiceChangerScene",
            
//#endif
//#if AGORA_STRING_UID
//            "BasicAudioCallSceneS",
//            
//            "AudioMixingSceneS",
//            "AudioSpectrumSceneS",
//            "ChannelMediaRelaySceneS",
//            
//            "CustomCaptureAudioSceneS",
//            
//            "CustomRenderAudioSceneS",
//            "DeviceManagerSceneS",
//            
//            "JoinChannelVideoTokenSceneS",
//            "MediaPlayerSceneS",
//            "MediaPlayerOpenWithMediaSourceS",
//            "MediaRecorderSceneS",
//            "MetadataSceneS",
//            "MusicPlayerSceneS",
//            "PluginSceneS",
//            "ProcessAudioRawDataSceneS",
//            
//            
//            
//            
//            
//            "SetEncryptionSceneS",
//            
//            "SpatialAudioWithMediaPlayerSceneS",
//            "SpatialAudioWithUsersS",
//            "StartDirectCdnStreamingSceneS",
//            "StartLocalVideoTranscoderSceneS",
//            "StartRhythmPlayerSceneS",
//            "StartRtmpStreamWithTranscodingSceneS",
//            "StreamMessageSceneS",
//            
//            
//            "VoiceChangerSceneS",
//            
//#endif
#endif
        };

        private string[] _rtmNameList = {
#if AGORA_RTM
            "RtmClientScene",
            "RtmStreamChannelScene",
            "RtmLockScene",
            "RtmPresenceScene",
            "RtmStorageScene"
#endif
        };

        private void Awake()
        {
#if AGORA_RTC
            PermissionHelper.RequestMicrophontPermission();
            PermissionHelper.RequestCameraPermission();
#endif

            GameObject content = GameObject.Find("Content");
            var contentRectTrans = content.GetComponent<RectTransform>();

            for (int i = 0; i < _rtcNameList.Length; i++)
            {
                var go = Instantiate(CasePanel, content.transform);
                var name = go.transform.Find("Text").gameObject.GetComponent<Text>();
                name.text = _rtcNameList[i];
                var button = go.transform.Find("Button").gameObject.GetComponent<Button>();
                button.onClick.AddListener(OnJoinSceneClicked);
                button.onClick.AddListener(SetScolllerActive);
            }

            for (int i = 0; i < _rtmNameList.Length; i++)
            {
                var go = Instantiate(CasePanel, content.transform);
                var name = go.transform.Find("Text").gameObject.GetComponent<Text>();
                name.text = _rtmNameList[i];
                var button = go.transform.Find("Button").gameObject.GetComponent<Button>();
                button.onClick.AddListener(OnJoinSceneClicked);
                button.onClick.AddListener(SetScolllerActive);
            }


            if (this.AppInputConfig)
            {
                this.AppIdInupt.text = this.AppInputConfig.appID;
                this.ChannelInput.text = this.AppInputConfig.channelName;
                this.TokenInput.text = this.AppInputConfig.token;
            }

        }

        // Start is called before the first frame update
        private void Start()
        {


        }

        // Update is called once per frame
        private void Update()
        {


        }

        private void OnApplicationQuit()
        {
            Debug.Log("OnApplicationQuit");
        }

        public void OnLeaveButtonClicked()
        {
            StartCoroutine(UnloadSceneAsync());
            CaseScrollerView.SetActive(true);
        }

        public IEnumerator UnloadSceneAsync()
        {
            if (this._playSceneName != "")
            {
                AsyncOperation async = SceneManager.UnloadSceneAsync(_playSceneName);
                yield return async;
                EventSystem.gameObject.SetActive(true);
            }
        }

        [System.Serializable] //직열화
        public class TokenData
        {
            public string token;
        }
        public void OnJoinSceneClicked()
        {
            //수정
            DataBase.Instance.SendMessageApi("{\"channel\":\"" + this.AppInputConfig.channelName + "\",\"uid\":" + DataBase.Instance.stu_local_code + "}", "Token", (Success, request) => {

                this.AppInputConfig.appID = this.AppIdInupt.text;
                this.AppInputConfig.channelName = this.ChannelInput.text;

                TokenData tokenData = JsonUtility.FromJson<TokenData>(request);
                Debug.Log("Token: " + tokenData.token);
                this.AppInputConfig.token = tokenData.token;

                var button = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
                var sceneName = button.transform.parent.Find("Text").gameObject.GetComponent<Text>().text;

                EventSystem.gameObject.SetActive(false);

                SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
                this._playSceneName = sceneName;
            });

        }

        public void SetScolllerActive()
        {
            CaseScrollerView.SetActive(false);
        }
    }
}
