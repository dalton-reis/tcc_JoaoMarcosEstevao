using UnityEngine;
using MLAgents;

public class DogAgent : Agent
{
    [Tooltip("How fast the agent moves forward")]
    public float moveSpeed = 5f;

    [Tooltip("How fast the agent turns")]
    public float turnSpeed = 180f;

    private DogAcademy dogAcademy;

    new private Rigidbody rigidbody;
    private float contactRadius = 0f;

    Animator animator;

    private int score = 0;

    /// Initial setup, called when the agent is enabled
    public override void InitializeAgent()
    {
        base.InitializeAgent();
        dogAcademy = FindObjectOfType<DogAcademy>();
        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if (true)
        {
            AddReward(1f);
            score++;
        }
        else
        {
            AddReward(-1f);
        }

        if (score == 5)
        {
            Done();
        }
    }

    /// Perform actions based on a vector of numbers
    /// <param name="vectorAction">The list of actions to take</param>
    public override void AgentAction(float[] vectorAction)
    {
        // Convert the first action to forward movement
        float forwardAmount = vectorAction[0];

        // Convert the second action to turning left or right
        float turnAmount = 0f;
        if (vectorAction[1] == 1f)
        {
            turnAmount = -1f;
        }
        else if (vectorAction[1] == 2f)
        {
            turnAmount = 1f;
        }

        if (forwardAmount > 0)
            animator.SetInteger("condition", 1);
        else
            animator.SetInteger("condition", 0);

        // Apply movement
        rigidbody.MovePosition(transform.position + transform.forward * forwardAmount * moveSpeed * Time.fixedDeltaTime);
        transform.Rotate(transform.up * turnAmount * turnSpeed * Time.fixedDeltaTime);

        // Apply a tiny negative reward every step to encourage action
        AddReward(-1f / agentParameters.maxStep);
    }

    /// Read inputs from the keyboard and convert them to a list of actions.
    /// This is called only when the player wants to control the agent and has set
    /// Behavior Type to "Heuristic Only" in the Behavior Parameters inspector.
    /// <returns>A vectorAction array of floats that will be passed into <see cref="AgentAction(float[])"/></returns>
    public override float[] Heuristic()
    {
        float forwardAction = 0f;
        float turnAction = 0f;
        if (Input.GetKey(KeyCode.W))
        {
            // move forward
            forwardAction = 1f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            // turn left
            turnAction = 1f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            // turn right
            turnAction = 2f;
        }

        // Put the actions into an array and return
        return new float[] { forwardAction, turnAction };
    }

    /// Reset the agent and area
    public override void AgentReset()
    {
        score = 0;
       // contactRadius = dogAcademy.ContactRadius;
    }

    /// Collect all non-Raycast observations
    public override void CollectObservations()
    {
        // Distance to the baby (1 float = 1 value)
        //AddVectorObs(Vector3.Distance(goodBall.transform.position, transform.position));

        // Direction to baby (1 Vector3 = 3 values)
        //AddVectorObs((goodBall.transform.position - transform.position).normalized);

        // Direction penguin is facing (1 Vector3 = 3 values)
        AddVectorObs(transform.forward);

        // 1 + 3 + 3 = 7 total values
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("good_ball"))
        {
            AddReward(0.1f);
        }
        else if (collision.transform.CompareTag("bad_ball"))
        {
            AddReward(-0.1f);
        }
    }
}