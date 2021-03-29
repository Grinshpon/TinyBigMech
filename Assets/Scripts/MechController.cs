using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechController : MonoBehaviour
{
  public UnityEngine.AI.NavMeshAgent agent;
  Transform agenttf;

  void Start()
  {
    agenttf = agent.GetComponent<Transform>();
  }

  void Update()
  {
    agent.Move(agenttf.forward * Time.deltaTime * agent.speed);
  }
}
