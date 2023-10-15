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

    private bool isStacked = false;
    private float timeStacked = 0f;
    private float maxTimeStacked = 1.5f;

    private bool isTransparent = false;
    private float timeTransparent = 0f;
    private float maxTimeTransparent = 1f;

    private float speed = 1f;

    void Start()
    {
        path = new Stack<(int x, int y)>();
        rb2D = GetComponent<Rigidbody2D>();
        GetTarget((int)(transform.position.x - 0.5f), (int)(transform.position.y - 0.5f));
        GetNextStep();
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

        if (isStacked)
        {
            timeStacked += Time.deltaTime;
            if (timeStacked >= maxTimeStacked)
            {
                EnableTransparent();
            }
        }
        else if (isTransparent)
        {
            timeTransparent += Time.deltaTime;
            if (timeTransparent >= maxTimeTransparent)
            {
                DisableTransparent();
            }
        }

        velocity = GetVelocity(nextStep);
        rb2D.MovePosition(rb2D.position + velocity * Time.fixedDeltaTime * 4);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag(World._tag))
        {
            isStacked = true;
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag(World._tag))
        {
            isStacked = false;
        }
    }

    private void EnableTransparent()
    {
        isStacked = false;
        isTransparent = true;
        timeStacked = 0f;
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void DisableTransparent()
    {
        isTransparent = false;
        GetComponent<Collider2D>().isTrigger = false;
        timeTransparent = 0f;
    }

    private void GetNextStep()
    {
        nextPoint = path.Pop();
        nextStep = new Vector2(nextPoint.x + 0.5f, nextPoint.y + 0.5f);
    }

    private Vector2 GetVelocity(Vector2 target)
    {
        Vector2 currentPosition = GetComponent<Rigidbody2D>().position;
        Vector2 direction = target - currentPosition;
        direction.Normalize();
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
            target.GetComponent<SpriteRenderer>().color = Color.yellow;
            path = World.FindShortestPath((currentX, currentY), (xCord, yCord));
        }
    }
}
