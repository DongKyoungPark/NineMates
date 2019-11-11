using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
대기 플레이어가 조작하지 않는 상태 불꽃이 부드럽게 타올라가고 있는 상태
   

점프 오브젝트 위에서 도약하여 좌, 우 방향으로 포물선을 그리며 지정된 좌표로 날아가는 상태  불꽃이 한쪽으로 휘어서 속도감을 느낄 수 있다.

   
    오브젝트 예시 : 트램폴린의 외형을 베이스로 맵 컨셉에 맞춰 변환.트램폴린 이파리, 현대식 트램폴린, 구름 트램폴린 등
탑승  비행퍼즐 오브젝트 위에 엎드려서 오브젝트를 붙잡은 채 완만한 곡선을 그리며 지정된 좌표로 날아가는 상태

    점프와 비슷하게 불꽃이 한쪽을 휘지만 엎드려서 오브젝트르 붙잡은 외형이 차이점이다.
    오브젝트 예시 : 맵에 흔히 보이고 성냥이를 찾을 때 사용했던 오브젝트 재사용.커다란 이파리, 커다란 구름 등


걷기  엉덩이를 좌우로 흔들면서 쫑쫑거리며 걷는 상태 (지금 사용하는 리소스 그대로 사용)
    순간이동 오브젝트에서도 사용

매달리기    점프 애니메이션 뒤에 연달아 나오는 경우가 빈번할 것으로 예상
    줄이 타오르면서 불이 옮겨 붙어 공중에서 이동하는 상태
    (손을 뻗거나 줄을 붙잡고 있는 형태도 괜춘)

    오브젝트 예시 : 불이 붙을 수 있고 줄 형태로 되어있는 무언가.로프, 덩굴, 사다리, 밧줄 등
착지 매달리기 애니메이션 뒤에 연달아 나오는 경우가 빈번할 것으로 예상

    바닥과 충돌하면서 가로로 길게 눌리고 퍼졌다가 탄성있게 본래 외형으로 돌아오는 형태
    쓰임 예시 : 날아가기, 로프에서 내려오기 등
손내밀기(나눔) 서브캐릭터 주변으로 다가오면 서브캐릭터는[물음표 말풍선] 을 표시한다.
    플레이어 캐릭터는 [느낌표 말풍선]을 표시하고 손을 뻗는다. 둘의 손이 닿는 즉시 서브 캐릭터의 머리 위에서 하늘을 향해

    빛이 올라간다. 악수를 멈추고 서브캐릭터가 플레이어 캐릭터의 뒤에 쫄랑쫄랑 따라온다.						

    [아기성냥이]는 불이 붙은 캐릭터로 전환되고[큰 성냥이] 의 뒤에서 같은 이동속도로 함께 이동
감정표현    말풍선 속 간단한 이모지 띄움
탈진  비를 만나거나 물에 빠져서 눅눅해지고 빛을 잃은 상태

    물에 젖으면 탈진 애니메이션을 호출하고 다시 빛을 얻은 상태로 돌입

    탈진 애니메이션 : [큰 성냥이]의 주변으로 작은 성냥이들이 옹기종기 모여들고 함께[큰 성냥이] 에게 불을 붙여주는 내용
독립  큰 성냥이가 탈진을 겪은 바로 다음 이어서 독립 애니메이션 호출

    의도 : 작은 성냥이들도 나눔을 실천하러 간다는 것을 암시, 나눔은 일회성에서 끝나지 않는다는 것을 느낄 수 있도록

    독립 애니메이션 : [작은 성냥이] 중 하나가 [큰 성냥이]로 성장하고 인사하며 먼저 맵의 한쪽으로 사라짐


*/

public enum CharacterState
{
    None,
    Idle,
    Idle_Hang,
    Jump,
    Ride,
    Run,
    Run_Hang,
    Hang,
    Fly,
    Land,
    Share,
    Exhaustion,
	Push,
	Pick,
}

public enum CharacterType
{
    Player,
    Follower,
    Puzzle // 이 부분은 논의 필요
}

public class CharacterBehaviour
{
    //private enum 
    private string characterId;
    public CharacterState State {  get { return state; } }
    private CharacterState state;
    private bool isSleeper;

    public CharacterBehaviourData Data { get { return data; } }

    private CharacterBehaviourData data;    
    

    public CharacterBehaviour(string characterID, CharacterState state, bool isSleeper, CharacterBehaviourData data)
    {
        this.characterId = characterID;
        this.state = state;
        this.isSleeper = isSleeper;
        this.data = data;
    }

    public virtual void OnStateExit(AnimatorStateInfo stateInfo)
    {

    }

    public virtual void OnStateEnter(AnimatorStateInfo stateInfo)
    {
        if (data == null)
            return;

        //Debug.Log("OnstateEnter " + data.testOnStateEnter);
    }

    public virtual float GetSpeed(float horizontalMove)
    {
        if (data == null)
            return 0.0f;

        if (data.isPlayable == false)
            return 0.0f;

        float speed = isSleeper == true ? data.speed * 0.2f : data.speed;

        horizontalMove = speed * horizontalMove;

        return Mathf.Clamp(horizontalMove, -1 * data.maxSpeed, data.maxSpeed);
    }
}
