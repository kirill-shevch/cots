using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Routing : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject target;
    Rigidbody2D rb2D;

    Stack<(int x, int y)> path;
    (int x, int y) nextPoint;
    Vector2 nextStep;
    Vector2 velocity;

    void Start()
    {
        path = new Stack<(int x, int y)>();
        rb2D = GetComponent<Rigidbody2D>();
        GetTarget(1, 1);
        velocity = GetVelocity();
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
            rb2D.velocity = Vector2.zero;
            velocity = GetVelocity();
        }

        rb2D.MovePosition(rb2D.position + velocity * Time.fixedDeltaTime * 4);
    }

    private Vector2 GetVelocity()
    {
        float diagonalModifyer = 1;
        nextPoint = path.Pop();
        nextStep = new Vector2(nextPoint.x + 0.5f, nextPoint.y + 0.5f);
        if (Mathf.Abs(nextStep.x - transform.position.x) > 0.1 && Mathf.Abs(nextStep.y - transform.position.y) > 0.1)
        {
            diagonalModifyer = 0.7071f;
        }
        return new Vector2((nextStep.x - transform.position.x) * diagonalModifyer, (nextStep.y - transform.position.y) * diagonalModifyer);
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
            target = World.Build(1, 1, xCord, yCord);
            Debug.Log($"Target: x-{xCord} ; y-{yCord}");
            target.GetComponent<SpriteRenderer>().color = Color.yellow;
            path = World.FindShortestPath((currentX, currentY), (xCord, yCord));
        }
    }
}
