using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChallengeKit;
using System;
using NM;

public class Character : MonoBehaviour
{
    // 이걸로 이제 캐릭터 별 구분이 가능하도록 만들것이다.
    [SerializeField]
    private string characterID;

    private CharacterState currentState = CharacterState.Idle;
    private Dictionary<int, CharacterBehaviour> behaviours;
    private int currentHash;

    public CharacterBehaviour CurrentBehaviour {  get { return behaviours[currentHash]; } }

    private const string sleeperPostfix = "_sleeper";
   
    private bool isSleeper = true;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private Rigidbody2D rigidbody;
    public Rigidbody2D RigidBody { get { return rigidbody; } }

    [SerializeField]
    private CharacterController2D controller;

    private BaseInput input;

    [SerializeField]
    private bool isPlayer;

    public bool IsPlayer {  get { return isPlayer; } }

    private float currentSpeed = 0.0f;
    private float requestedDuration = 0.0f;

    public void PreEnter(InputManager inputManager)
    {
        PreEnter_Input(inputManager);
        PreEnter_CharacterController();
    }

    private void PreEnter_CharacterController()
    {
        behaviours = new Dictionary<int, CharacterBehaviour>();

        // ScriptableObject만으로 케릭터 병렬 완성을 가정하자.
        AnimatorOverrideController newController = new AnimatorOverrideController
        {
            name = characterID + "_controller",
            runtimeAnimatorController = animator.runtimeAnimatorController
        };

        // 여기서 이제 컨트롤러 네이밍 규칙이 정해진다.
        // 1. 기본은 전부 소문자
        // 2. 불붙지 않은 state는 뒤에 _sleeper 가 붙음.
        // 3. 고유 케릭터 string 전달로 능력치 세분화 가능하도록 만들자.

        foreach (CharacterState state in Enum.GetValues(typeof(CharacterState)))
        {
            if (state == CharacterState.None)
                continue;

            string stateString = state.ToString().ToLower();
            int id = Animator.StringToHash(stateString);

            // 만약 스크립트별로 별개의 엑션이 필요하다면 
            // new CharacterBehaviour 대신 상속버전으로 넣어 줄 수 있다.

            // todo : 나중에 리소스 메니져 통하게 만들기.
            string path = string.Format("ScriptableObjects/Behaviour/{0}_{1}", characterID, state);
            CharacterBehaviourData data = (CharacterBehaviourData)Resources.Load(path);

            if (data == null)
                continue;

            if (behaviours.ContainsKey(id) == false)
            {
                behaviours.Add(id, new CharacterBehaviour(characterID, state, false, data));
                newController[stateString] = data.motion;
            }

            if (data.isSleeper == false)
                continue;

            stateString += sleeperPostfix;
            int sleeperId = Animator.StringToHash(stateString);

            if (behaviours.ContainsKey(sleeperId) == false)
            {
                behaviours.Add(sleeperId, new CharacterBehaviour(characterID, state, true, data));
                newController[stateString] = data.sleeperMotion;
            }
        }
        animator.runtimeAnimatorController = newController;
        animator.SetBool("IsSleeper", isSleeper);
    }

    private void PreEnter_Input(InputManager inputManager)
    {
        if (input == null)
        {
            if (isPlayer)
            {
                input = gameObject.AddComponent<PlayerInput>();

                if (inputManager != null)
                {
                    inputManager.SetPlayerInput((PlayerInput)input);
                    
                }
                else
                {
                    // Deprecated
                    GameManager.Instance.inputManager.SetPlayerInput((PlayerInput)input);
                }
            }
            else
            {
                input = gameObject.AddComponent<AIInput>();
            }

            input.PreEnter(characterID, UpdateSpeed);
        }
    }
    private void UpdateSpeed(float horizontalMove)
    {
        if (behaviours.ContainsKey(currentHash) == false)
            return;

        currentSpeed = CurrentBehaviour.GetSpeed(horizontalMove);
        animator.SetFloat("Speed", Mathf.Abs(currentSpeed));
    }

    private int GetBehaviourHash(CharacterState state)
    {
        string stateString = state.ToString().ToLower();

        if (isSleeper)
        {
            stateString += sleeperPostfix;
        }

        return Animator.StringToHash(stateString);
    }

    // 왜 테스트 점프냐면 해당 점프 구현 로직을 controller 부터  덜어낼려고... follower 동작 테스트 대상 코드로 임시로 사용
    public void TestJump()
    {
        GoToState(CharacterState.Jump, 0.4f);
        controller.Move(currentSpeed * Time.fixedDeltaTime, false, true);
        Debug.Log("Du Jump!");
    }

    public void OnLand()
    {
        if(CurrentBehaviour.Data.isLandable)
        {
            // 스테이트 초기 진입할 때, 특정 사운드를 넣는것도 가능하겠다. (착지 사운드때문에)
            // 다만 착지하는 곳이 땅 뿐만 아니라 물이나 환경에 따라 달리 나게 한다면, 또한 별개의 시스템이 필요하다.

            // 최초 밧줄잡는데 전환 에니메이션 필요하면 또 여기 넣을 수 있겠지...
            GoToState(animator.GetBool("IsHang") ? CharacterState.Idle_Hang : CharacterState.Land);
        }
    }

    public void OnFly()
    {
        if (CurrentBehaviour.Data.isFlyable)
        {
            // 스테이트 초기 진입할 때, 특정 사운드를 넣는것도 가능하겠다. (착지 사운드때문에)
            // 다만 착지하는 곳이 땅 뿐만 아니라 물이나 환경에 따라 달리 나게 한다면, 또한 별개의 시스템이 필요하다.
            GoToState(CharacterState.Fly);
        }
    }

    public void GoToState(CharacterState state, float duration = 0.0f)
    {
        if (state == CharacterState.None)
            return;

        int key = GetBehaviourHash(state);

        if (behaviours.ContainsKey(key) == false)
            return;

        if (behaviours[key].Data.isLockable == false)
            return;

        animator.Play(key, 0, 0);

        requestedDuration = duration;
        CoManager.Stop(this, 0);
        CoManager.Start(this, AnimLockByDuration(duration), 0);
    }

    private IEnumerator AnimLockByDuration(float duration)
    {
        animator.SetBool("IsLock", true);
        yield return new WaitForSeconds(duration);
        animator.SetBool("IsLock", false);
    }

    public void OnStateEnter(AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (behaviours == null)
            return;

        if(behaviours.ContainsKey(stateInfo.shortNameHash))
        {
            if(requestedDuration != 0.0f && stateInfo.loop == false)
            {
                var clipInfo = animator.GetCurrentAnimatorClipInfo(0);
                float originalDurtaion = clipInfo.Length;
                animator.speed = originalDurtaion / requestedDuration;
                requestedDuration = 0.0f;
            }
            else
            {
                animator.speed = 1.0f;
            }

            currentHash = stateInfo.shortNameHash;
            currentState = behaviours[currentHash].State;
            behaviours[stateInfo.shortNameHash].OnStateEnter(stateInfo);
        }
    }

    public void OnStateExit(AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (behaviours == null)
            return;

        if (behaviours.ContainsKey(stateInfo.shortNameHash))
        {
            behaviours[stateInfo.shortNameHash].OnStateExit(stateInfo);
        }
    }

    void FixedUpdate()
    {
        // Move our character
        controller.Move(currentSpeed * Time.fixedDeltaTime, false, false);
    }

    public void AwakeFromSleep()
    {
		if (isPlayer)
		{
			isSleeper = false;
			animator.SetBool("IsSleeper", isSleeper);
		}
		else
		{
			Invoke("LateAwakeFromSleep", 2.0f);
		}
    }

	public void LateAwakeFromSleep()
	{
		if (isSleeper)
		{
			NMManager.Instance.ScoreUP();
			EffectManager.Instance.OnEffect("StarPopup", transform.position);
		}

		isSleeper = false;
		animator.SetBool("IsSleeper", isSleeper);
	}

	public void SetParameter<T>(string parameter, T value)
    {
        if (animator == null)
            return;

        if(value is bool)
        {
            animator.SetBool(parameter, Convert.ToBoolean(value));
        }
        else
        {
            // etc etc.. 
            Debug.LogWarning("Character::SetParameter Type is not Implemented, " + value.GetType().ToString());
        }
    }

    public bool IsSleeper()
    {
        return isSleeper;
    }

    public bool IsAlive()
    {
        return true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isPlayer)
        {
			if (collision.CompareTag("DeadZone"))
            {
                CharacterManager.Instance.ResetPlayer();
            }
        }

        if (isSleeper)
            return;

        if (collision.CompareTag("Sleeper"))
        {
			if (isPlayer)
			{
				if (collision.transform.parent.GetComponent<Character>().IsSleeper())
					GoToState(CharacterState.Share, 3.1f);
			}

			collision.SendMessageUpwards("AwakeFromSleep", SendMessageOptions.DontRequireReceiver);
        }
    }
}
