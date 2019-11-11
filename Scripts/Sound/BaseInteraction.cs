using UnityEngine;
using System.Collections;

/// <summary>
/// BaseInteraction - 인터랙션 기본 동작을 정의한 클래스. 인터랙션이 필요한 컴포넌트의 부모 클래스.
/// </summary>
public class BaseInteraction : MonoBehaviour
{

    // 기본 값 세팅.
    public EventStartType eventStartType = EventStartType.NONE;
    public bool repeatExecution = false;
    public bool deactivateAfter = true;
    public float maxMouseDistance = 3;
    public string keyToPress = "";
    public bool keyPressInTrigger = false;

    //public int dropID = 0;
    //public bool consumeDrop = false;

    // variable check
    public AIConditionNeeded needed = AIConditionNeeded.ALL;
    public bool autoDestroyOnVariables = true;
    public string[] variableKey = new string[0];
    public string[] variableValue = new string[0];
    public bool[] checkType = new bool[0];
    // number variables
    public string[] numberVarKey = new string[0];
    public float[] numberVarValue = new float[0];
    public bool[] numberCheckType = new bool[0];
    public ValueCheck[] numberValueCheck = new ValueCheck[0];

    // varialbe set
    public string[] setVariableKey = new string[0];
    public string[] setVariableValue = new string[0];
    // number variables
    public string[] setNumberVarKey = new string[0];
    public float[] setNumberVarValue = new float[0];
    public SimpleOperator[] setNumberOperator = new SimpleOperator[0];

    // ingame
    private bool _isInTrigger = false;
    private GameObject _checkGameObject = null;


    /// <summary>
    /// 조건 변수 추가. 
    /// </summary>
    public void AddVariableCondition()
    {
        variableKey = ArrayHelper.Add("key", variableKey);
        variableValue = ArrayHelper.Add("value", variableValue);
        checkType = ArrayHelper.Add(true, checkType);
    }
    /// <summary>
    /// 조건 변수 삭제.
    /// </summary>
    /// <param name="index"></param>
    public void RemoveVariableCondition(int index)
    {
        variableKey = ArrayHelper.Remove(index, variableKey);
        variableValue = ArrayHelper.Remove(index, variableValue);
        checkType = ArrayHelper.Remove(index, checkType);
    }
    /// <summary>
    /// 조건 상수 추가.
    /// </summary>
    public void AddNumberVariableCondition()
    {
        numberVarKey = ArrayHelper.Add("key", numberVarKey);
        numberVarValue = ArrayHelper.Add(0, numberVarValue);
        numberCheckType = ArrayHelper.Add(true, numberCheckType);
        numberValueCheck = ArrayHelper.Add(ValueCheck.EQUALS, numberValueCheck);
    }
    /// <summary>
    /// 조건 상수 삭제
    /// </summary>
    /// <param name="index"></param>
    public void RemoveNumberVariableCondition(int index)
    {
        numberVarKey = ArrayHelper.Remove(index, numberVarKey);
        numberVarValue = ArrayHelper.Remove(index, numberVarValue);
        numberCheckType = ArrayHelper.Remove(index, numberCheckType);
        numberValueCheck = ArrayHelper.Remove(index, numberValueCheck);
    }
    /// <summary>
    /// 조건 체크.
    /// </summary>
    /// <returns></returns>
    public bool CheckVariables()
    {
        //+ 설정 된 조건이 없으면 검사 안합니다.
        if (variableKey.Length == 0 && numberVarKey.Length == 0)
        {
            return false;
        }
        bool apply = true; // 적용 판단 여부.
        bool any = false; // 조건에 만족한 것이 있는지 판단 여부.
                          //+ 모든 조건중 만족한 것이 있는지 확인한다.
        for (int i = 0; i < variableKey.Length; i++)
        {
            bool check = EventHandler.CheckVariable(variableKey[i], variableValue[i]);

            if ((check && checkType[i]) || (!check && !checkType[i]))
            {
                any = true;
            }
            else if (AIConditionNeeded.ALL.Equals(needed))
            {
                apply = false;
                break;
            }
        }
        //+ 위에 조건이 만족했다면 숫자 값 조건도 만족하는지 확인한다.
        if (apply == true)
        {
            for (int i = 0; i < numberVarKey.Length; i++)
            {
                bool check = EventHandler.CheckNumberVariable(numberVarKey[i], numberVarValue[i], numberValueCheck[i]);

                if ((check && numberCheckType[i]) || (!check && !numberCheckType[i]))   // bool numberCheckType[i]
                {
                    any = true;
                }
                else if (AIConditionNeeded.ALL.Equals(needed))
                {
                    apply = false;
                    break;
                }
            }
        }
        //+ 하나만 만족해도 되지만 만족한 것이 하나도 없었다면 적용하지 않는다.
        if (AIConditionNeeded.ONE.Equals(needed) && !any &&
            (variableKey.Length > 0 || numberVarKey.Length > 0))
        {
            apply = false;
        }
        return apply;
    }
    /// <summary>
    /// AddVariableSet - 조건 추가.
    /// </summary>
    public void AddVariableSet()
    {
        setVariableKey = ArrayHelper.Add("key", setVariableKey);
        setVariableValue = ArrayHelper.Add("value", setVariableValue);
    }
    /// <summary>
    /// RemoveVariableSet - 조건 삭제.
    /// </summary>
    /// <param name="index"></param>
    public void RemoveVariableSet(int index)
    {
        setVariableKey = ArrayHelper.Remove(index, setVariableKey);
        setVariableValue = ArrayHelper.Remove(index, setVariableValue);
    }
    /// <summary>
    /// AddNumberVariableSet - 상수 조건 추가.
    /// </summary>
    public void AddNumberVariableSet()
    {
        setNumberVarKey = ArrayHelper.Add("key", setNumberVarKey);
        setNumberVarValue = ArrayHelper.Add(0, setNumberVarValue);
        setNumberOperator = ArrayHelper.Add(SimpleOperator.ADD, setNumberOperator);
    }
    /// <summary>
    /// RemoveNumberVariableSet - 상수 조건 삭제.
    /// </summary>
    /// <param name="index"></param>
    public void RemoveNumberVariableSet(int index)
    {
        setNumberVarKey = ArrayHelper.Remove(index, setNumberVarKey);
        setNumberVarValue = ArrayHelper.Remove(index, setNumberVarValue);
        setNumberOperator = ArrayHelper.Remove(index, setNumberOperator);
    }
    /// <summary>
    /// SetVariables - 조건 설정. 보통 중간 저장된 조건들을 다시 불러와서 적용시킬때 사용된다.
    /// </summary>
    public void SetVariables()
    {
        for (int i = 0; i < setVariableKey.Length; i++)
        {
            EventHandler.SetVariable(setVariableKey[i], setVariableValue[i]);
        }
        for (int i = 0; i < setNumberVarKey.Length; i++)
        {
            if (SimpleOperator.ADD.Equals(setNumberOperator[i]))
            {
                EventHandler.SetNumberVariable(setNumberVarKey[i], EventHandler.GetNumberVariable(setNumberVarKey[i]) + setNumberVarValue[i]);
            }
            else if (SimpleOperator.SUB.Equals(setNumberOperator[i]))
            {
                EventHandler.SetNumberVariable(setNumberVarKey[i], EventHandler.GetNumberVariable(setNumberVarKey[i]) - setNumberVarValue[i]);
            }
            else if (SimpleOperator.SET.Equals(setNumberOperator[i]))
            {
                EventHandler.SetNumberVariable(setNumberVarKey[i], setNumberVarValue[i]);
            }
        }
    }
    /// <summary>
    /// TouchInteract - 터치용 인터랙트 처리 함수.
    /// </summary>
    public virtual void TouchInteract()
    {

    }
    /// <summary>
    /// CheckTriggerEnter - 트리거가 충돌됐을때를 체크한다.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool CheckTriggerEnter(Collider other)
    {
        //+ 한번 충돌 검출된 오브젝트는 다시 체크하지 않는다.
        bool check = false;
        if (_checkGameObject == other.gameObject)
            return false;

        _isInTrigger = true;
        if (EventStartType.TRIGGER_ENTER.Equals(eventStartType)/* &&  CheckVariables()*/)
        {
            check = true;
        }

        _checkGameObject = other.gameObject;
        return check;
    }
    /// <summary>
    /// CheckTriggerExit- 트리거가 빠져나갔을때 체크.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool CheckTriggerExit(Collider other)
    {
        //+ 한번 충돌 검출된 오브젝트만 체크한다.
        bool check = false;
        if (_checkGameObject != other.gameObject)
            return false;

        _isInTrigger = false;
        if (EventStartType.TRIGGER_EXIT.Equals(eventStartType)/* &&  CheckVariables()*/)
        {
            check = true;
        }

        _checkGameObject = null;
        return check;
    }
    /// <summary>
    /// KeyPress - 키 입력 여부 체크.
    /// </summary>
    /// <returns></returns>
    public bool KeyPress()
    {
        if (keyToPress == string.Empty)
            return false;

        return EventStartType.KEY_PRESS.Equals(eventStartType)// &&  CheckVariables() 
            && Input.GetKeyDown(keyToPress) && (!keyPressInTrigger || _isInTrigger);
    }
    /// <summary>
    /// Interact
    /// </summary>
    /// <returns></returns>
    public virtual bool Interact() { return false; }


}
