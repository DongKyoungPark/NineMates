using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CreateAssetMenu(fileName ="BehaviourScriptable", menuName= "NineMates/Character")]
public class CharacterBehaviourData : ScriptableObject
{
    [Tooltip("해당 Behaviour 에 Sleeper(발화전) State가 있는가요?")]
    public bool isSleeper = false;

    [HideInInspector]
    public float speed = 100;
    [HideInInspector]
    public float maxSpeed = 120;

    [Tooltip("해당 Behaviour 일때 조작이 가능한지?.")]
    public bool isPlayable = true;

    [Tooltip("해당 Behaviour가 사용하는 에니메이션 클립을 넣어주세요.")]
    public AnimationClip motion;

    [HideInInspector]
    public AnimationClip sleeperMotion;

    public string testOnStateEnter;

    // 땅으로 Transition이 전이될 수 있는가?
    public bool isLandable = false;

    // 공중으로 Transition 될 수 있는가?
    public bool isFlyable = false;

    // 임의로 게임 내에서 animation 시간을 제어할 수 있는가?
    public bool isLockable = true;

    /*
    [Tooltip("해당 Behaviour가 끝나고 나서 다음으로 넘어갈 State가 있는가?")]
    public bool hasNextState = false;
    [HideInInspector]
    public CharacterState nextState;
    */
}

#if UNITY_EDITOR
[CustomEditor(typeof(CharacterBehaviourData))]
public class CharacterBehaviourData_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // for other non-HideInInspector fields

        CharacterBehaviourData script = (CharacterBehaviourData)target;

        EditorGUILayout.Separator();

        if(script.isPlayable)
        {
            script.speed = EditorGUILayout.FloatField("Speed", script.speed);
            script.maxSpeed = EditorGUILayout.FloatField("Max Speed", script.maxSpeed);
        }

        if (script.isSleeper)
        {
            script.sleeperMotion = EditorGUILayout.ObjectField("Sleeper Motion", script.sleeperMotion, typeof(AnimationClip), true) as AnimationClip;
        }

        /*
        if(script.hasNextState)
        {
            script.nextState = (CharacterState)EditorGUILayout.EnumPopup("Next State", script.nextState);
        }
        */
    }
}
#endif

