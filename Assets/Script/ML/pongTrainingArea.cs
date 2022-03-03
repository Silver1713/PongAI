using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; //Import TMP Pro in Unity
// based from : http://noobtuts.com/unity/2d-pong-game

// This is a script for the training area for the agent
// This manages the training environment
// The environment is like a playing field
public class pongTrainingArea : MonoBehaviour
{
    public GameObject spawner; // The spawn position of the spawn point of the ball
    public List<PongAgent> players; // The list of agents (players) in the training area
    public TextMeshPro mesh1;public TextMeshPro mesh2; // Rewards indicator for visualization
    public GameObject ball; 


    public Ball ballConfig; // This is the ball class

    /// <summary>
    /// This resets the current area
    /// to the starting state
    /// </summary>
    public void resetArea()
    {
        if (ball != null) //If ball is not null
        {
            ball.transform.position = ballConfig.startingPosition; //Move ball to starting position
            ballConfig.setRandomVelocity(); //Allow random direction to be set
           
        } 
        resetPlayers(); // Reset the players paddles position
        // Reset player function - reccomended to finish the initialize for the pong agent before
        //doing the resetPlayer
    }

    public void resetPlayers() 
    {

        //Using foreach loop access all agents in players and reset it to its starting position
        players.ForEach(agent => agent.transform.position = agent.startingPosition);

        /**
        foreach (var agent in players)
        {
            agent.transform.position = agent.startingPosition;
        }
        **/
        /**
        for (int i = 0; i < players.Count; i++)
        {
            PongAgent agent = players[i];

            agent.transform.position = agent.startingPosition;
        }
        **/
    }

    //Call at the first frame after awake
    private void Start()
    {
        resetArea();
    }
    //Call once per frame
    private void Update()
    {
        //Display rewards for both agents
        mesh1.text = players[0].GetCumulativeReward().ToString();
        mesh2.text = players[1].GetCumulativeReward().ToString();
    }


}
