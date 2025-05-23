using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static RobotAgent;
using static DroneAgent;
using static EnvManager;

public class EpisodeManager : MonoBehaviour
{
    public RobotAgent[] robotAgents;
    //public DroneAgent[] droneAgents;
    public EnvManager env;
    public bool episodeInProgress = false;
    public int episodeCounter = 0;

    public void EndAllEpisodes()
    {
        if (episodeInProgress) return;

        episodeInProgress = true;
        foreach (RobotAgent agent in robotAgents)
        {
            agent.EndEpisode();
        }
        //foreach (DroneAgent agent in droneAgents)
        //{
        //    agent.EndEpisode();
        //}
        Debug.Log("All episodes ended by EpisodeManager!");

        //환경 초기화
        env.initEnv();
        episodeInProgress = false;
    }

    public void EpisodeCounter() //수확 성공으로 인한 에피소드 종료를 세기 위한 함수
    {
        episodeCounter++;
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
