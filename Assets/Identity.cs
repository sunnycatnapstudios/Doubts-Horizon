using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Identity : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    public Survivor survivor;
    public Survivor GetSurvivor() { return survivor; }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
