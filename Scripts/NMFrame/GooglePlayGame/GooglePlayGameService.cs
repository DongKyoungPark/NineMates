using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NM.GooglePlay
{
    public interface IGooglePlayGameService
    {
        void Start();
        void SignIn();
        void SignOut();
        void Save();
        void Load();        
    }

    public class GooglePlayGameService
    {
        public static readonly GooglePlayGameService Instance = new GooglePlayGameService();

        private static bool waitingForAuth = false;

        private static bool isGuest = false;

        private static bool isLogin = false;

        public void Start()
        {
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

        public void SignIn(Action<bool> callback)
        {
            Debug.Log("SignIn Start");
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
                            Debug.Log("로그인 성공");                            
                        }
                        else
                        {
                            Debug.Log("로그인 실패");
                            Debug.LogWarning($"why?? {msg}");
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
            else if(isLogin == true)
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

        public void SignOut()
        {
			//PlayGamesPlatform.Instance.SignOut();
#if UNITY_ANDROID
            ((GooglePlayGames.PlayGamesPlatform)Social.Active).SignOut();
#endif
        }

        public bool IsLogin()
        {
            Debug.Log($"login Status:{isLogin}");
            return isLogin;
        }

        public bool IsGuest()
        {
            Debug.Log($"guest Status:{isGuest}");
            return isGuest;
        }

    }
}
