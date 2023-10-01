using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Routing : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject target;
    Stack<(int x, int y)> path;
    (int x, int y) position;

    void Start()
    {
        position = (1, 1);
        path = new Stack<(int x, int y)>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (path.Count == 0)
            {
                GetTarget();
            }
            else
            {
                position = path.Pop();
                Debug.Log($"NextStep: x-{position.x} ; y-{position.y}");
                transform.position = new Vector2(position.x + 0.5f, position.y + 0.5f);
                //transform.Translate(new Vector2(position.x + 0.5f, position.y + 0.5f));
            }
        }
    }

    private void GetTarget() 
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
            path = World.FindShortestPath(position, (xCord, yCord));
        }
    }
}
