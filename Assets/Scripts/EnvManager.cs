using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TreeManager;

public class EnvManager : MonoBehaviour
{
    public TreeManager[] trees; //나무들 배열

    public List<TreeManager> fruitTrees = new List<TreeManager>(); // 과일이 익은 나무들
    //public List<TreeManager> pestTrees = new List<TreeManager>();  // 해충이      나무들
    
    public float harvestCounter = 0f;
    public float ripenCounter = 0f;

    public void initEnv() //나무들 체력값 초기 배정
    {
        foreach (TreeManager tree in trees) //각 나무들에 대하여(배열 순회)
        {
            tree.Initialize(); //나무들 HP 0~100 사이의 랜덤값 배정
        }
        harvestCounter = 0;
        ripenCounter = 0;
    }

    public void updateEnv()
    {
        fruitTrees.Clear();
    //    pestTrees.Clear();

        foreach (TreeManager tree in trees) //각 나무들에 대하여
        {
            tree.updateHP(); //Hp값 증가

            if (tree.IsHarvestable()) //과일열린 나무가 있으면
                fruitTrees.Add(tree); //다른 코드에서 참조할 수 있게 리스트에 추가

            //if (tree.IsPestActive()) //해충생긴 나무가 있으면
            //    pestTrees.Add(tree); //다른 코드에서 참조할 수 있게 리스트에 추가
        }
    }

    public void HarvestCounter()
    {
        harvestCounter++;
    }
    public void RipenCounter()
    {
        ripenCounter++;
    }
    public float CalculateGlobalReward()
    {
        float globalReward = 0f;
        if (ripenCounter == 0)
        {
            return globalReward;
        }
        globalReward = 10f * (harvestCounter / ripenCounter);
        Debug.Log("Global Reward : " + globalReward);
        return globalReward;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        initEnv();
    }

    // Update is called once per frame
    void Update()
    {
        updateEnv();
    }
}
