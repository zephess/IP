using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollow : MonoBehaviour
{
    [SerializeField]
    private Transform playerHead;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = playerHead.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
