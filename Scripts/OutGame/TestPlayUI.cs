using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TestPlayUI : MonoBehaviour
{
    public static TestPlayUI Instance;
    public TMPro.TextMeshProUGUI _presstheBtn;
    public Image _fade;
    public Image _logo;

    private void Awake()
    {
        Instance = this;
        GameStartReady();
    }


    [ContextMenu("SSS")]
    public void GameStartReady()
    {
        StartCoroutine("CoGameStartReady");
    }
    IEnumerator CoGameStartReady()
    {
        _directionGo.gameObject.SetActive(false);
        group.alpha = 0;
        _fade.color = new Color(0f, 0f, 0f, 1f);
        _logo.gameObject.SetActive(false);
        _presstheBtn.gameObject.SetActive(false);
        yield return group.DOFade(1f, 1f).WaitForCompletion(false);        
        yield return _fade.DOFade(0f, 1f).WaitForCompletion(false);
        _logo.gameObject.SetActive(true);
        _logo.color = new Color(1f, 1f, 1f, 0f);
        yield return _logo.DOFade(1f, 1.2f).WaitForCompletion(false);
    }
    public void GameStart()
    {
    }

    bool _isStart = false;
    Transform _tr;
    private void Update()
    {
        if (!_isStart  && Input.GetKeyDown(KeyCode.Mouse0))
        {
            group.DOFade(0f, 1f).WaitForCompletion(false);
            _logo.gameObject.SetActive(false);
            _isStart = true;
        }


        if (null != _tr)
        {
            if (225f < _tr.localPosition.x)
            {
                GameEndDirect();
            }
        }
        else
        {
            _tr = GameObject.FindGameObjectWithTag("Player").GetComponent<Character>().transform;
        }
    }


    public CanvasGroup group;
    public GameObject _directionGo;
    [ContextMenu("SSS222")]
    public void GameEndDirect()
    {
        StartCoroutine("CoGameEndDirect");
    }
    IEnumerator CoGameEndDirect()
    {
        group.alpha = 0;
        RectTransform tr = _directionGo.GetComponent<RectTransform>();
        tr.transform.position = new Vector3(2042f, 272f, 0f);
        _directionGo.SetActive(true);
        yield return group.DOFade(1f, 1f).WaitForCompletion(false);
        yield return tr.DOMoveX(315,  8f).WaitForCompletion(false); 
        yield return _fade.DOFade(1f, 1f).WaitForCompletion(false);
        GameEnd();
    }

    public void GameEnd()
    {
        GameManager.Instance.RestartLevel();
        GameStartReady();
    }
}
