using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static RobotAgent;
using static DroneAgent;

public class EpisodeManager : MonoBehaviour
{
    public RobotAgent[] robotAgents;
    public DroneAgent[] droneAgents;

    public void EndAllEpisodes()
    {
        foreach (RobotAgent agent in robotAgents)
        {
            agent.EndEpisode();
        }
        foreach (DroneAgent agent in droneAgents)
        {
            agent.EndEpisode();
        }
        Debug.Log("All episodes ended by EpisodeManager!");
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
