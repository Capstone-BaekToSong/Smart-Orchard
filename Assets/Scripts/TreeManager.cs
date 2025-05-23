using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static RendererToggle;
using static EnvManager;

public class TreeManager : MonoBehaviour
{
    // 트리의 위치정보 갖고 있어야돼

    public enum TreeState { Normal, FruitBearing, Rot } //나무들 상태(나중에 pest, wither 추가!)

    public TreeState state = TreeState.Normal;
    public float HP;

    public RendererToggle rendererToggle;
    public EnvManager env;

    public float resetHP = 50f; //과일, 해충 이후 나무 초기화HP값

    public float maxHp = 100f; //나무 HP 최대값
    // public int pestDamageRate = 2; //해충으로 인해 감소하는 체력값
    public int fruitStepCounter = 0; //과일이 익으면 제한된 스텝안에 수확해야하는데, 익고 난 뒤에 카운팅 되는 변수
    public int fruitTimeoutStep = 3000; //20스텝 안에 수확해야함
                                        //public int treeWitherHP = 20;
    public int rotStepCounter = 0;
    public int rotTimeoutStep = 3000;


    public void Initialize()
    {
        HP = Random.Range(30f, 70f); //나무 HP값 설정
        state = TreeState.Normal; //나무에 별 일 없는 상태(뭔 일이 일어날 수 있는 상태)
        fruitStepCounter = 0; //수확 카운터
        rendererToggle.initializeRenderers(); //Empty tree 상태로 renderer 변경
        env = FindObjectOfType<EnvManager>();
    }

    public void updateHP()
    {
        switch (state) //나무에 벌어질 수 있는 일들
        {
            case TreeState.Normal: //나무에 뭔 일이 벌어질 수 있는 상황
                if (HP < maxHp)
                {
                    HP = HP + 0.02f;
                }

                //if (HP < 80 && Random.value < 0.05f) //체력이 80미만이면 5%확률로 해충 발생
                //{
                //    state = TreeState.Pest; //해충 생긴 나무로 인식
                //    //해충 생겼다는거 시각화하는 함수
                //}

                if (HP >= 80f && Random.value < 0.005f) //체력이 80이상이면 20%확률로 과일이 익음
                {
                    state = TreeState.FruitBearing; //과일 익은 나무로 인식(트리거)
                    rendererToggle.makeFullTree(); //과일 익었다는거 시각화하는 함수
                    env.RipenCounter();
                    fruitStepCounter = 0; //수확 카운터 초기화
                }
                break;

            case TreeState.FruitBearing: //익은 나무가 존재할 때
                fruitStepCounter++;
                if (fruitStepCounter >= fruitTimeoutStep)
                {
                    state = TreeState.Rot;
                    rendererToggle.makeRottenTree(); //과일이 썩었다는거 시각화하는 함수
                }
                break;

            case TreeState.Rot:
                rotStepCounter++;
                if (rotStepCounter >= rotTimeoutStep)
                {
                    ResetTree();
                    state = TreeState.Normal;
                }
                break;

                //case TreeState.Pest: //해충 발생시
                //    HP -= pestDamageRate; //HP가 줄어드는 로직 실행

                //    if (HP <= treeWitherHP) //체력이 줄어들다 30이하가 되면
                //    {
                //        state = TreeState.Wither;
                //    }
                //    break;

                //case TreeState.Wither:
                //    ResetTree();
                //    state = TreeState.Normal;
                //    break;

                //             //Rot나 Wither 시 한 step 쉬었다 다시 나무 초기화
        }
    }

    public void Harvest() //수확 로직
    {
        //수확 성공 시각화
        ResetTree();
    }

    //public void PesticideSpray() //방사로직
    //{
    //    //방제 성공 시각화
    //    ResetTree();
    //}

    public bool IsHarvestable() //과일이 익은 나무인식
    {
        return state == TreeState.FruitBearing;
    }

    //public bool IsPestActive() //해충발생한 나무인식
    //{
    //    return state == TreeState.Pest;
    //}

    public bool IsFruitRot() //열매 제때 수확 못했는지
    {
        return state == TreeState.Rot;
    }

    //public bool IsTreeWither() //방제 제때 못했는지
    //{
    //    return state == TreeState.Wither;
    //}

    private void ResetTree() //나무리셋로직
    {
        HP = resetHP; //HP 초기화
        state = TreeState.Normal; //정상상태
        rendererToggle.initializeRenderers();
        fruitStepCounter = 0;
        rotStepCounter = 0;
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
