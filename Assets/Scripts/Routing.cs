using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine;

public class Routing : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject target;
    Rigidbody2D rb2D;

    Stack<(int x, int y)> path;
    (int x, int y) nextPoint;
    Vector2 nextStep;
    Vector2 velocity;

    private Vector2 previousPosition;
    private int counter = 0;
    private float threshold = 0.01f;  

    void Start()
    {
        path = new Stack<(int x, int y)>();
        rb2D = GetComponent<Rigidbody2D>();
        GetTarget((int)(transform.position.x - 0.5f), (int)(transform.position.y - 0.5f));
        GetNextStep();
        previousPosition = rb2D.position;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        if (Vector2.Distance(nextStep, transform.position) < 0.1f)
        {
            if (path.Count == 0)
            {
                GetTarget(nextPoint.x, nextPoint.y);
            }
            GetNextStep();
        }

        Vector2 currentPosition = GetComponent<Rigidbody2D>().position;
        if (Mathf.Abs(currentPosition.x - previousPosition.x) < threshold &&
            Mathf.Abs(currentPosition.y - previousPosition.y) < threshold)
        {
            counter++;
        }
        else
        {
            counter = 0;
        }
        if (counter > 50)
        {
            Debug.Log("Object has been stacked for some time.");
            nextStep.x += Random.Range(-0.4f, +0.4f);
            nextStep.y += Random.Range(-0.4f, +0.4f);
        }
        if (counter > 250)
        {
            Debug.Log("Object has been stacked for a long time.");
            nextStep.x += Random.Range(-0.4f, +0.4f);
            nextStep.y += Random.Range(-0.4f, +0.4f);
        }
        velocity = GetVelocity(nextStep);
        previousPosition = currentPosition;
        rb2D.MovePosition(rb2D.position + velocity * Time.fixedDeltaTime * 4);
    }

    private void GetNextStep()
    {
        nextPoint = path.Pop();
        //Debug.Log($"Next point: x-{nextPoint.x} ; y-{nextPoint.y}");
        nextStep = new Vector2(nextPoint.x + 0.5f, nextPoint.y + 0.5f);
    }

    private Vector2 GetVelocity(Vector2 target)
    {
        Vector2 currentPosition = GetComponent<Rigidbody2D>().position;
        Vector2 direction = target - currentPosition;
        direction.Normalize();
        float speed = 1f; // Change this value to adjust the speed of movement
        return direction * speed;
    }

    private void GetTarget(int currentX, int currentY) 
    {
        if (target != null)
        {
            Destroy(target);
        }
        target = null;
        while (target == null) 
        {
            int xCord = Random.Range(1, World._maxX);
            int yCord = Random.Range(1, World._maxY);
            if (!World.grid[xCord, yCord]) 
            {
                continue;
            }
            target = World.Build(1, 1, xCord, yCord, World._circleSprite);
            //Debug.Log($"Target: x-{xCord} ; y-{yCord}");
            target.GetComponent<SpriteRenderer>().color = Color.yellow;
            path = World.FindShortestPath((currentX, currentY), (xCord, yCord));
        }
    }
}
