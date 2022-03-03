using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

// based from : http://noobtuts.com/unity/2d-pong-game
// This is the main component of the ml-agent code
//it is inherited from the agent class
// it allow the computer to control the agent

public class PongAgent : Agent
{
    Rigidbody2D paddleBody; //the paddle rigidbody
    [Header("For agents")]

    public Vector3 startingPosition; // The paddle starting position
    private GameObject ball; //The ball in the game

    [Tooltip("AI Goal")] public GameObject goal; //The agent goal that it is defending
    [Tooltip("AI Adversary")] public PongAgent adversaries; // The agent's opponent

    public pongTrainingArea area; // The current training area


    [Header("configuration")]
    [Tooltip("Paddle movement speed")]public float speed = 30;

    [Header("Action Inputs")]
    [Tooltip("The movement to apply")] public float movementAmount;
    /// -1 ,0 ,1
    /// 0 : 0
    /// 1 : 1
    /// 2 : -1


    public override void Initialize()
    {
        
        
        //Initialize is similar to  the Start() method, 
        //it is called when the ml-agents have been started
        base.Initialize();
        paddleBody = transform.GetComponent<Rigidbody2D>();
        startingPosition = transform.position; //Store the starting position of the paddle
        ball = area.ball; // get the ball object from the training area
        
    }

    //Handles inputs by the ml-agents
    //ml-agent at start will randomly put in different values
    public override void OnActionReceived(ActionBuffers actions)
    {
        //Actions : Can be by player or NN
        // For the actions it uses unsigned integer
        /**
         *  If movement amount is 0 mean the paddle won't move.
         *  If it is 1 mean move up.
         *  if it is 2 means move down.
         *  The movement amount controls the movement while the actions is managed 
         *  by the code. 
         *  You can defined what the movement amount is with the different ml-agents inputs
         */
        movementAmount = 0; // -1, 0 , 1
        if (actions.DiscreteActions[0] == 1f)
        {
            movementAmount = 1f;

        } else if (actions.DiscreteActions[0] == 2f)
        {
            movementAmount = -1f;
        }

        // This is to encourage actions by increasing penalty per steps
        // Since ML-Agent involve through rewards and penalty
        if (MaxStep >= 0)
        {
            AddReward(-0.1f / MaxStep);
        }
        //Allow the paddle to move
        //using the moveAmount as an modifier
        paddleBody.MovePosition((Vector2)transform.position   + (new Vector2(0, movementAmount) * speed )* Time.fixedDeltaTime );
    }

    //Heuristic is like an manual switch from auto pilot
    // This determine the manual inputs and what it does
    /**
     * W to move up
     * S to move down
     */
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        int movementInput = 0;
        if (Input.GetKey(KeyCode.W))
        {
            movementInput = 1;
        } else if (Input.GetKey(KeyCode.S))
        {
            movementInput = 2;
        }

        actionsOut.DiscreteActions.Array[0] = movementInput;
    }
    //OnCollisionEnter detect if a collider has entered the agent collider
    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("PongBall"))
        {
            AddReward(1f);
            adversaries.AddReward(-15f);

        }
        
    }
    //Called when endEpisode is called.
    //Reset everything to it starting state
    public override void OnEpisodeBegin()
    {
        base.OnEpisodeBegin();
        area.resetArea();
    }


    public override void CollectObservations(VectorSensor sensor)
    {
        base.CollectObservations(sensor);

        sensor.AddObservation(Vector2.Distance(transform.position, ball.transform.position)); //1 Observation [1 float value]
        sensor.AddObservation((ball.transform.position - transform.position).normalized); //3 Observations [3 float values]
        sensor.AddObservation((adversaries.transform.position - transform.position).normalized);//3 Observations [3 float values]

        // 7 Observations [7 values]
        //Vectors is considered 2 or 3 observation depending if it is 2d or 3d
        /**
         * Since Vector3D has 3 values (X,Y,Z), each is considered one observation.
         * For Vector2D (X, Y) has 2 values and is considered 2 observations.
         * If mix with 2D and 3D if will usually be 3 observations since the return result will usually be
         * Vector3D (X,Y,0)
         */

    }




}
