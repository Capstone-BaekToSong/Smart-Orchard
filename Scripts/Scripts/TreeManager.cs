using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeManager : MonoBehaviour
{
    public enum TreeState { Normal, FruitBearing, Pest, Rot, Wither} //나무들 상태

    public TreeState state = TreeState.Normal;
    public int HP;
    public int resetHP = 50; //과일, 해충 이후 나무 초기화HP값

    public float hpIncreasePerStep = 1.0f; // 에디터에서 조정 가능한 HP 증가량

    public int maxHp = 100; //나무 HP 최대값
    public int pestDamageRate = 2; //해충으로 인해 감소하는 체력값
    public int fruitStepCounter = 0; //과일이 익으면 제한된 스텝안에 수확해야하는데, 익고 난 뒤에 카운팅 되는 변수
    public int fruitTimeoutStep = 20; //20스텝 안에 수확해야함
    public int treeWitherHP = 20;

    public void Initialize()
    {
        HP = Random.Range(30, 70); //나무 HP값 설정
        state = TreeState.Normal; //나무에 별 일 없는 상태(뭔 일이 일어날 수 있는 상태)
        fruitStepCounter = 0; //수확 카운터
    }

    public void updateHP()
    {
        switch (state) //나무에 벌어질 수 있는 일들
        {
            case TreeState.Normal: //나무에 뭔 일이 벌어질 수 있는 상황
                if (HP < maxHp)  HP++;

                if (HP < 80 && Random.value < 0.05f) //체력이 80미만이면 5%확률로 해충 발생
                {
                    state = TreeState.Pest; //해충 생긴 나무로 인식
                    //해충 생겼다는거 시각화하는 함수
                }

                if (HP >= 80f && Random.value < 0.2f) //체력이 80이상이면 20%확률로 과일이 익음
                {
                    state = TreeState.FruitBearing; //과일 익은 나무로 인식(트리거)
                    //과일 익었다는거 시각화하는 함수
                    fruitStepCounter = 0; //수확 카운터 초기화
                }
                break;

            case TreeState.FruitBearing: //익은 나무가 존재할 때
                fruitStepCounter++;
                if (fruitStepCounter >= fruitTimeoutStep)
                {
                    state = TreeState.Rot;
                    //과일이 썩었다는거 시각화하는 함수
                }
                break;

            case TreeState.Rot:
                ResetTree();
                state = TreeState.Normal;
                break;

            case TreeState.Pest: //해충 발생시
                HP -= pestDamageRate; //HP가 줄어드는 로직 실행

                if (HP <= treeWitherHP) //체력이 줄어들다 30이하가 되면
                {
                    state = TreeState.Wither;
                }
                break;

            case TreeState.Wither:
                ResetTree();
                state = TreeState.Normal;
                break;
            //Rot나 Wither 시 한 step 쉬었다 다시 나무 초기화
        }
    }

    public void Harvest() //수확 로직
    {
        //수확 성공 시각화
        ResetTree();
    }

    public void PesticideSpray() //방사로직
    {
        //방제 성공 시각화
        ResetTree();
    }

    public bool IsHarvestable() //과일이 익은 나무인식
    {
        return state == TreeState.FruitBearing;
    }

    public bool IsPestActive() //해충발생한 나무인식
    {
        return state == TreeState.Pest;
    }

    public bool IsFruitRot() //열매 제때 수확 못했는지
    {
        return state == TreeState.Rot;
    } 

    public bool IsTreeWither() //방제 제때 못했는지
    {
        return state == TreeState.Wither;
    }

    private void ResetTree() //나무리셋로직
    {
        HP = resetHP; //HP 초기화
        state = TreeState.Normal; //정상상태
        fruitStepCounter = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
