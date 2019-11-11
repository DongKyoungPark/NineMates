using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    public enum State { None, Intro, Title, Game, GameMenu }
    public static State CurState = State.None;
    private static Scene CurScene;
    public string _sceneName = null;
    public GameObject soundManager;
    private void Awake()
    {
        _intro_parent.SetActive(true);
        _title_parent.SetActive(false);
        _exitBtnGo.SetActive(false);
        _exitMenuGo.SetActive(false);
        _loading.SetActive(false);
        _pressButton.SetActive(false);

        Button btn = _exitBtnGo.GetComponent<Button>();
        btn.onClick.AddListener(() =>
        {
            _exitMenuGo.SetActive(true);
            CurState = State.GameMenu;
        });

        Button btn1 = _continueBtn.GetComponent<Button>();
        btn1.onClick.AddListener(() =>
        {
            _exitMenuGo.SetActive(false);
            CurState = State.Game;
        });

        Button btn2 = _exitBtn.GetComponent<Button>();
        btn2.onClick.AddListener(() =>
        {
            StartCoroutine("CoReStart");
        });
        StartGame();
    }

    private void Start()
    {
        if (soundManager == null)
        {
            Vector2 initPos = new Vector2(0, 0);
            soundManager = Instantiate(Resources.Load("Prefabs/SoundManager"), initPos, Quaternion.identity) as GameObject;
            //SoundManager.instance.PlayBGM(SoundManager.instance.playBGMSoundName[0]);
            //SoundManager.instance.audioSourceBgm.volume = 0.7f;
        }
    }

    Coroutine coChangeScene;
    void ActiveScene(string sceneName, System.Action completeAction)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            return;
        }
        if (null != coChangeScene)
        {
            StopCoroutine(coChangeScene);
        }
        coChangeScene = StartCoroutine(CoSceneChange(sceneName, completeAction));
    }
    IEnumerator CoSceneChange(string sceneName, System.Action completeAction)
    {
        if (false == string.IsNullOrEmpty(CurScene.name))
        {
            yield return SceneManager.UnloadSceneAsync(CurScene);
        }
        yield return SceneManager.LoadSceneAsync(sceneName, new LoadSceneParameters(LoadSceneMode.Additive));
        if (null != completeAction)
        {
            _loading.SetActive(true);
            _loadingSprite.fillAmount = 0.1f;
            yield return new WaitForSeconds(0.2f);
            _loadingSprite.fillAmount = 0.3f;
            yield return new WaitForSeconds(0.11f);
            _loadingSprite.fillAmount = 0.4f;
            yield return new WaitForSeconds(0.2f);
            _loadingSprite.fillAmount = 0.7f;
            yield return new WaitForSeconds(0.11f);
            _loadingSprite.fillAmount = 0.9f;
            yield return new WaitForSeconds(0.66f);
            _loadingSprite.fillAmount = 1f;
            yield return new WaitForSeconds(0.5f);
            _loading.SetActive(false);

            completeAction();
        }
    }

    public GameObject _intro_parent;
    public GameObject _title_parent;

    [ContextMenu("SetUI")]
    public void SetUI()
    {
        //GameObject.Find("Canvas").GetComponentInChildren<;
        //_intro_parent = GetGo("intro_parent");
        //_fade = GameObject.Find("Fade").GetComponent<SpriteRenderer>();
    }

    GameObject GetGo(string name)
    {
        GameObject result = GameObject.Find(name);
        if (null != result)
        {
            result.SetActive(false);
        }
        return result;
    }

    Transform GetTr(string name)
    {
        Transform result = null;
        GameObject go = GameObject.Find(name);
        if (null != go)
        {
            result = go.transform;
            go.SetActive(false);
        }
        return result;
    }

    Coroutine _coStart;
    public Image _mates;
    public Image _char;
    public Image _char2;

    public TMPro.TextMeshProUGUI _titleLable;
    public TMPro.TextMeshProUGUI _teamNameLabel;
    public SpriteRenderer _fade;

    public GameObject _loading;
    public Image _loadingSprite;
    public GameObject _pressButton;
    public TMPro.TextMeshProUGUI _pressButtonLable;

    public RectTransform _bg;
    public Image _bgsprite;

    [ContextMenu("StartGame")]
    public void StartGame()
    {
        if (null != _coStart)
        {
            CurState = State.Intro;
            StopCoroutine("CoStart");
        }
        _coStart = StartCoroutine("CoStart");
    }
    IEnumerator CoStart()
    {
        _intro_parent.SetActive(true);
        _teamNameLabel.color = new Color(1f, 1f, 1f, 0f);
        _char.color = new Color(1f, 1f, 1f, 0f);
        _char2.color = new Color(1f, 1f, 1f, 0f);
        _mates.color = new Color(1f, 1f, 1f, 0f);
        _bg.transform.localPosition = new Vector3(996f, 0f, 0f);
        _bgsprite.color = new Color(1f, 1f, 1f, 0f);
        yield return new WaitForSeconds(0.4f);
        yield return _mates.DOFade(1f, 1.2f).WaitForCompletion(false);
        yield return new WaitForSeconds(1f);
        yield return _mates.DOFade(0f, 0.4f).WaitForCompletion(false);
        yield return _teamNameLabel.DOFade(1f, 0.3f).WaitForCompletion(false);
        yield return new WaitForSeconds(1.2f);
        yield return _teamNameLabel.DOFade(0f, 0.7f).WaitForCompletion(false);
        _titleLable.color = new Color(1f, 1f, 1f, 0f);
        _title_parent.SetActive(true);
        _intro_parent.SetActive(false);
        yield return _titleLable.DOFade(1f, 1f).WaitForCompletion(false);
        yield return _char.DOFade(1f, 0.8f).WaitForCompletion(false);
        yield return _char2.DOFade(1f, 0.4f).WaitForCompletion(false);
        yield return _titleLable.DOColor(new Color(0.9f, 0.4f, 0.4f), 1f).WaitForCompletion(false);

        _bgsprite.DOFade(1f, 0.4f).WaitForCompletion(false);
        _bg.DOMove(new Vector3(-4548f, 0f, 0f), 100f);
        _pressButton.SetActive(true);
        _pressButtonLable.DOFade(1f, 1f).CompletedLoops();

        _isUpdateCheck = true;
        CurState = State.Title;
        yield break;
    }
    IEnumerator CoReStart()
    {
        if (CurScene.name.Contains("Zone"))
        {
            _titleLable.color = new Color(1f, 1f, 1f, 0f);
            _title_parent.SetActive(true);
            _intro_parent.SetActive(false);
            yield return SceneManager.UnloadSceneAsync(CurScene);
            yield return _titleLable.DOFade(1f, 1f).WaitForCompletion(false);

            _loading.SetActive(true);
            _loadingSprite.fillAmount = 0.1f;
            yield return new WaitForSeconds(0.2f);
            _loadingSprite.fillAmount = 0.3f;
            yield return new WaitForSeconds(0.11f);
            _loadingSprite.fillAmount = 0.7f;
            yield return new WaitForSeconds(0.11f);
            _loadingSprite.fillAmount = 1f;
            yield return new WaitForSeconds(0.5f);
            _loading.SetActive(false);

            _isUpdateCheck = true;
            CurState = State.Title;
        }
        yield break;
    }

    bool _isUpdateCheck = false;

    public GameObject _exitBtnGo;
    public GameObject _exitMenuGo;

    public Button _continueBtn;
    public Button _exitBtn;

    private void Update()
    {
        if (_isUpdateCheck)
        {
            if (CurState == State.Title && Input.GetKeyDown(KeyCode.Mouse0))
            {
                //SoundManager.instance.PlayBGM(SoundManager.instance.playBGMSoundName[1]);
                //SoundManager.instance.audioSourceBgm.volume = 0.7f;

                _isUpdateCheck = false;

                ActiveScene(_sceneName, () =>
                {
                    _isUpdateCheck = false;
                    _title_parent.SetActive(false);
                    CurState = State.Game;
                    _exitBtnGo.SetActive(true);
                    CurScene = SceneManager.GetSceneByName(_sceneName);
                });
            }
        }
    }

    //TweenerCore<Color, Color, ColorOptions> StartFade(SpriteRenderer target, float value, float duration)
    //{
    //    return target.DOFade(value, duration);
    //}
}
