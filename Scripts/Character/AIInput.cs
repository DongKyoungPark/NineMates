using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BTAI;

public class AIInput : BaseInput
{
    private AIBrain brain;
    private Root ai = BT.Root();

    private Character character;
    private Animator animator;

    private float currentTime;

    private Character player;

    public override void PreEnter(string characterID, Action<float> updateSpeed)
    {
        base.PreEnter(characterID, updateSpeed);

        string path = string.Format("ScriptableObjects/AIBrain/{0}", characterID);
        brain = (AIBrain)Resources.Load(path);

        character = GetComponent<Character>();
        animator = GetComponent<Animator>();

        player = CharacterManager.Instance.player.GetComponent<Character>();

        currentTime = 0;

        ai.OpenBranch
        (
            // 1) Sleeper Part. 일종의 대기 상태.
            BT.While(character.IsSleeper).OpenBranch
            (
                BT.Wait(brain.tickRate)
            ),

            // 2) MainAction - 이 부분만 AIBrain 로 옮길 수 있을지도?
            // BT도 너무 난잡하게 제어하지 말고, IsAlive 부분만 제어가능하게 만들면 된다.
            // 사실 이정도 단순함이면 BT 말고 그냥 로직짜도 될듯... 이건 다른 프로그래머와 상의 해 보도록 하자.
            BT.While(character.IsAlive).OpenBranch
            (
                BT.RandomSequence(new int[] { 1, 6/*, 2*/ }).OpenBranch
                (
                    BT.Call(DoTeleportToPlayer),
                    BT.Call(TrackPlayer)/*,
                    BT.Root().OpenBranch
                    (
                        BT.Call(character.TestJump),
                        BT.Wait(0.5f)
                    )
                    */
                )
            ),

            BT.Wait(0.2f),
            BT.Call(LeaveWorld)
        );
        // MainAction End

    }

    private bool NeedTeleport()
    {
        return Mathf.Abs((player.transform.position - transform.position).sqrMagnitude) > brain.teleportDistance * brain.teleportDistance;
    }

    public float accel = 1f;
    private void TrackPlayer()
    {
        Vector3 dirVector = player.transform.position - transform.position;
        float dis = Vector3.Distance(player.transform.position, transform.position);
        if ( brain.awayDistFromPlayer > dirVector.sqrMagnitude)
        {
            HorizontalMove = 0.0f;
        }
        else
        {
            // todo : 날아다닐 수 있으면 이런식으로 horiznotalMove만 넣는 형식으로 만들지 말 것.
            HorizontalMove = dirVector.normalized.x + dirVector.normalized.x * (accel * dis);
        }
    }

    private void DoTeleportToPlayer()
    {
        if (NeedTeleport() == false)
            return;

        transform.position = player.transform.position;
        EffectManager.Instance.OnEffect("StarPopup", transform.position);
    }

    private void LeaveWorld()
    {
        Debug.Log("Im Leaving the World!!!!");
    }

    private void Update()
    {
        // todo : GameTime 구현부에 종속되도록 
        currentTime += Time.deltaTime;

        if (currentTime < brain.tickRate)
            return;

        currentTime = 0;
        ai.Tick();
    }
}
