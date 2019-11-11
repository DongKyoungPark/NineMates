using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

using GooglePlayGames;
using GooglePlayGames.BasicApi;
using DG.Tweening;

namespace NM
{
    public class User : MonoBehaviour, IManager
    {
        const string GUESTKEY = "isGuest";
        const string LEVELKEY = "level";
        const string TRIGGERKEY = "trigger";

        private bool waitingForAuth = false;
        private bool isGuest = false;
        private static bool isLogin = false;

        public struct GameData
        {            
            public int _zoneNumber;
            public int _triggerIndex;
            public bool _isCrash;
            public bool _isGuest;
            // 유저 데이터에 대한 Id(Key) 발급 시스템이 필요하다...
            //public PlayerData _player;
        }

        public struct PlayerData
        {
            public int _playerId;
            public string _playerName;
            public DateTime _createDate;
            public DateTime _lastLoginDate;
            public DateTime _logoutDate;
        }

        private static User _instance;

        public static User Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new User();
                }
                return _instance;
            }
        }

        private static GameData _curPlayData = new GameData();
        public static GameData CurPlayData => _curPlayData;

    public void Init()
        {
            Debug.Log("유저생성");
            if (_instance == null)
            {
                _instance = new User();
            }
            Load();
            // Select the Google Play Games platform as our social platform implementation
#if UNITY_ANDROID

            //PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
            //    .EnableSavedGames()
            //    .RequestEmail()
            //    .RequestServerAuthCode(false)
            //    .RequestIdToken()
            //    .Build();

            //GooglePlayGames.PlayGamesPlatform.InitializeInstance(config);
            GooglePlayGames.PlayGamesPlatform.DebugLogEnabled = true;
            GooglePlayGames.PlayGamesPlatform.Activate();

            Debug.Log("Activation Success");
            // 로그인한다
            //SignIn();
#elif UNITY_IOS

            //GameCenterPlatform.ShowDefaultAchievementCompletionBanner(true);

#endif

#if UNITY_EDITOR

            isLogin = true;
#endif
        }

        public void Release()
        {
            Debug.Log("유저소멸");
        }
        public static GameData GetPlayData()
        {
            Debug.Log("유저 데이터 겟");
            return _curPlayData;
        }
        public static void SetPlayData(int level, int triggerIndex)
        {
            Debug.Log("유저 데이터 셋");
            _curPlayData._zoneNumber = level;
            _curPlayData._triggerIndex = triggerIndex;
        }
		public static void Load()
        {
            Debug.Log("유저 로드");
            _curPlayData._zoneNumber = PlayerPrefs.GetInt(LEVELKEY);
            _curPlayData._triggerIndex = PlayerPrefs.GetInt(TRIGGERKEY);
            Debug.Log($"현재 유저 데이터는 zone:{_curPlayData._zoneNumber } / index: {_curPlayData._triggerIndex}");
        }
        public static void Save()
        {
            Debug.Log("유저 세이브");
            PlayerPrefs.SetInt(LEVELKEY, _curPlayData._zoneNumber);
            PlayerPrefs.SetInt(TRIGGERKEY, _curPlayData._triggerIndex);
        }

        public static bool IsGuest()
        {
            return PlayerPrefs.GetInt(GUESTKEY) == 1;
        }
        public static bool IsSocial()
        {
            return Social.localUser.authenticated;
        }

        public void SignIn(Action<bool> callback)
        {
            Debug.Log("SignIn Start");

//#if UNITY_EDITOR
//            callback(false);
//            //return;
//#endif

            if (Social.localUser.authenticated == false && isLogin == false)
            {
                // false 일때이기 때문에 자동 로그인을 한다 
                // 자동로그인이 안되면 Guest 로그인을 물어본다 
                waitingForAuth = true;



                if (!Social.localUser.authenticated)
                {
                    Social.localUser.Authenticate((bool success, string msg) =>
                    {
                        Debug.Log("Auth Start");
                        // ???  로그인 후 재로그인을 안하려면 사용자 세션을 어떻게 관리?
                        if (success)
                        {
#if UNITY_ANDROID
                            ((GooglePlayGames.PlayGamesPlatform)Social.Active).SetGravityForPopups(Gravity.CENTER_HORIZONTAL);
#endif
                            // 성공                            
                            waitingForAuth = false;
                            isLogin = true;
                            PlayerPrefs.SetInt(GUESTKEY, 0);
                            Debug.Log("로그인 성공");
                        }
                        else
                        {
                            Debug.Log("로그인 실패");
                            Debug.Log($"why?? {msg}");
                            // 실패
                            // Guest 로그인을 한다 또는 그냥 종료시킨다                             

                            // Guest 로그인 하시겠습니까?
                            //isGuest = true;
                            //Debug.Log("Start GuestMode");
                            //button.gameObject.SetActive(true);

                            isGuest = true;
                            if (isGuest)
                            {
                                // main 진입 
                                waitingForAuth = false;

                                PlayerPrefs.SetInt(GUESTKEY, 1);
                                Debug.Log("Guest 로그인 성공");
                                isLogin = true;
                            }
                            //else
                            //{
                            //    waitingForAuth = false;
                            //    // 최종 종료
                            //    Debug.Log("최종 로그인 실패");
                            //    isLogin = false;
                            //}
                        }
                        // 결과는??
                        callback?.Invoke(success);
                    });
                }

            }
            else if (isLogin == true)
            {
                // 현재 로그인 상태를 유지중이기 때문에 바로 Main으로 이동 
                // 자동 로그인된 것과 같다
                Debug.Log("세션 유지 성공");
                //return Social.localUser.authenticated;
            }
            else
            {
                Debug.Log("로그인 실패");
                isLogin = false;
                SignOut();
                //return false;
            }
            // Sign out!
            //((GooglePlayGames.PlayGamesPlatform)Social.Active).SignOut();
        }

        private void _SignIn(Action<bool> callback)
        {
            bool result = false;
#if UNITY_ANDROID

            PlayGamesPlatform.Instance.Authenticate((bool success) =>
            {
                if (success)
                {
                    ((GooglePlayGames.PlayGamesPlatform)Social.Active).SetGravityForPopups(Gravity.TOP);
                    result = success;
                    callback?.Invoke(result);
                }
                else
                {

                }
            });


#elif UNITY_IOS

            if (!Social.localUser.authenticated)
            {
                Social.localUser.Authenticate((suc) =>
                {
                    result = suc;
                    if (null != callback)
                    {
                        callback(result);
                    }
                });
            }
#endif
                       
#if UNITY_EDITOR

            result = true;
            if (null != callback)
            {
                callback(result);
            }
#endif

        }

        public void SignOut()
        {
			//PlayGamesPlatform.Instance.SignOut();
#if UNITY_ANDROID
            ((GooglePlayGames.PlayGamesPlatform)Social.Active).SignOut();
#endif
        }

        public static bool IsLogin()
        {
            Debug.Log($"login Status:{isLogin}");
            return isLogin;
        }

        //public bool IsGuest()
        //{
        //    Debug.Log($"guest Status:{isGuest}");
        //    return isGuest;
        //}

    }

    


}