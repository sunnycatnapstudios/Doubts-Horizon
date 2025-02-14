using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _PartyManager : MonoBehaviour
{
    public int partyCount;
    public GameObject partyMemberTemplate;
    public List<RuntimeAnimatorController> partyAnimControllers = new List<RuntimeAnimatorController>();
    
    public void Awake()
    {

    }
}
