using UnityEngine;
//This is an edited version
// based from : http://noobtuts.com/unity/2d-pong-game

public class Ball : MonoBehaviour
{
    public float speed = 30; //determine how fast the ball will go
    public Rigidbody2D ball; // allow velocity and physics to be applied to the ball
    public Vector2 startingPosition; //store the startingposition of the ball which will be utilize at [PongTrainingArea]
    public PongAgent lastHit; //lastHit will store the latest agent that hit the ball


    //Call before start() and called immediately
    void Awake()
    {
        // Initial Velocity
        startingPosition = transform.position;
        ball = GetComponent<Rigidbody2D>();


    }




    public void setRandomVelocity()
    {
        int dir = (Random.Range(0, 2) == 0) ? 1 : -1;
        /**
         if Random.Range(0,2) is equal to 0 then return 1
        else if it is anything else other than 0 it will return -1
        **/

        ball.velocity = (Vector3.right + Vector3.up) * speed / 2 * dir;
        //Muplying the dir if negative will inverts the direction it started allowing it to start with a random direction



    }

    float hitFactor(Vector2 ballPos, Vector2 racketPos,
                    float racketHeight)
    {
        // ascii art:
        // ||  1 <- at the top of the racket
        // ||
        // ||  0 <- at the middle of the racket
        // ||
        // || -1 <- at the bottom of the racket
        return (ballPos.y - racketPos.y) / racketHeight;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        // Note: 'col' holds the collision information. If the
        // Ball collided with a racket, then:
        //   col.gameObject is the racket
        //   col.transform.position is the racket's position
        //   col.collider is the racket's collider

        // Hit the left Racket?
        if (col.gameObject.CompareTag("RacketLeft"))
        {
            // Calculate hit Factor
            lastHit = col.gameObject.GetComponent<PongAgent>();
            float y = hitFactor(transform.position,
                                col.transform.position,
                                col.collider.bounds.size.y);

            // Calculate direction, make length=1 via .normalized
            Vector2 dir = new Vector2(1, y).normalized;

            // Set Velocity with dir * speed
            GetComponent<Rigidbody2D>().velocity = dir * speed;
        }

        // Hit the right Racket?
        // I used tags instead of name since we need to have multiple rackets
        if (col.gameObject.CompareTag("RacketRight"))
        {
            // Calculate hit Factor
            lastHit = col.gameObject.GetComponent<PongAgent>();
            float y = hitFactor(transform.position,
                                col.transform.position,
                                col.collider.bounds.size.y);

            // Calculate direction, make length=1 via .normalized
            Vector2 dir = new Vector2(-1, y).normalized;

            // Set Velocity with dir * speed
            GetComponent<Rigidbody2D>().velocity = dir * speed;
        }
        //Check if the ball have gone outside the bounds
        if (col.gameObject.CompareTag("Boundaries"))
        {
            ball.transform.position = startingPosition;
            //Return back to the center of the game area
        }

        if (col.gameObject.CompareTag(lastHit.goal.tag)) //If the agent score a goal
        {
            if (lastHit != null)
            {

                lastHit.AddReward(10f); //Give the agent reward
                lastHit.EndEpisode();
                //End the current episode which will call onEpisodeEnd in the agent code
            }
            else
            {
                transform.position = startingPosition;
                //If there is no last hit move to starting position
            }
        }
        if (col.gameObject.CompareTag(lastHit.adversaries.goal.tag))
        {
            if (lastHit != null)
            {

                lastHit.AddReward(-3f); //if the agent opponent hit the ball, give the agent a penalty
                // by storing their opponent in the agent code, it allow reward and penalty to be added to the different player.

                lastHit.EndEpisode(); //End the current episode
            }
            else
            {
                transform.position = startingPosition;
            }
        }
    }
}
