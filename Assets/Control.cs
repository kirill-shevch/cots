using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control : MonoBehaviour
{
    private float speed;
    private GameObject camera;

    // Start is called before the first frame update
    void Start()
    {
        camera = GameObject.FindGameObjectsWithTag("MainCamera")[0];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        camera.transform.Translate(new Vector2(moveX, moveY));
    }
}
