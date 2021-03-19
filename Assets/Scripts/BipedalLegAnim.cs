﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BipedalLegAnim : MonoBehaviour
{
  const float R = 1.57079633f;

  public Transform mech;
  public Transform leftFoot;
  public Transform rightFoot;
  public Transform leftLookahead;
  public Transform rightLookahead;
  public float distThreshold;
  public float speed;
  public float stepHeight;
  public float heightFromGround = 0.5267162f;
  public float epsilon = 0.01f;
  public LayerMask groundLayer;

  bool leftMoving = false;
  bool rightMoving = false;

  Vector3 leftOrigin;
  Vector3 rightOrigin;

  void Start()
  {
    leftOrigin = leftLookahead.localPosition;
    leftOrigin.y += 2;
    rightOrigin = rightLookahead.localPosition;
    rightOrigin.y += 2;
  }

  void Update()
  {
    Vector3 root = mech.position;
    if(!leftMoving)
    {
      if (Physics.Raycast(root+leftOrigin, -Vector3.up, out RaycastHit info, 10, groundLayer))
      {
        leftLookahead.position = info.point + new Vector3(0f,heightFromGround,0f);
      }
      leftMoving = HandlePos(leftFoot, leftLookahead, LeftPlaced);
    }
    if(!rightMoving)
    {
      if (Physics.Raycast(root+rightOrigin, -Vector3.up, out RaycastHit info, 10, groundLayer))
      {
        rightLookahead.position = info.point + new Vector3(0f,heightFromGround,0f);
      }
      rightMoving = HandlePos(rightFoot, rightLookahead, RightPlaced);
    }
  }

  bool HandlePos(Transform foot, Transform lookahead, Action callback)
  {
    Vector3 footPos = foot.position;
    Vector3 laPos = lookahead.position;
    if (Vector3.Distance(footPos, laPos) >= distThreshold)
    {
      StartCoroutine(MoveFoot(foot,lookahead, callback));
      return true;
    }
    return false;
  }

  void LeftPlaced()
  {
    leftMoving = false;
  }

  void RightPlaced()
  {
    rightMoving = false;
  }

  IEnumerator MoveFoot(Transform foot, Transform lookahead, Action callback)
  {
    Vector3 footPos = foot.position;
    Vector3 laPos = lookahead.position;
    Vector3 prevFootPos = footPos;
    float dist = Vector3.Distance(footPos, laPos);
    float t = 0f;
    while(dist > epsilon) {
      t = Mathf.Clamp(t+Time.deltaTime * speed, 0f, R); //todo: set speed according to mech's speed
      float lerp = Mathf.Sin(t);
      float r = Vector3.Distance(prevFootPos, laPos);
      footPos = Vector3.Lerp(prevFootPos, laPos, lerp);
      footPos.y += Mathf.Sin(lerp*Mathf.PI) * stepHeight;
      foot.position = footPos;

      yield return null;
      laPos = lookahead.position;
      dist = Vector3.Distance(footPos, laPos);
    }

    callback();
    yield return null;
  }
}
