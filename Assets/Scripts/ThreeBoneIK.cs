using UnityEngine;

public class ThreeBoneIK : MonoBehaviour
{
  public Transform mech;
  public Transform root, stifle, hock, foot, target;
  float lenUpper, lenMiddle, lenLower, lenMax;
  public bool dynamicRatio;
  [Range(0,1)]
  public float mRatio = 0.5f;

  //Quaternion initFootRotation;
  Vector3 initFootRotation;
  float rootY;

  public bool leftLeg = false;

  void Awake()
  {
    Init();
  }

  void Update()
  {
    UpdateBones();
    //foot.rotation = mech.rotation * initFootRotation;
    //foot.rotation = Quaternion.Euler(mech.TransformDirection(initFootRotation));
    //foot.RotateAround(foot.position,mech.right,20*Time.deltaTime);
  }

  public void UpdateBones()
  {
    Vector3 rootPos = root.position, targetPos = target.position;
    float targetLength = Vector3.Distance(rootPos, targetPos);
    Vector3 dir = targetPos-rootPos;
    if (targetLength >= lenMax)
    {
      // extend fully
      float theta = Vector3.Angle(-Vector3.up, dir);
      if (dir.z < 0f) theta *= -1f;
      if (leftLeg) theta *= -1f;
      root.localRotation = Quaternion.Euler(new Vector3(0f,rootY,theta));
      stifle.localRotation = Quaternion.identity;
      hock.localRotation = Quaternion.identity;
    }
    else
    {
      float ratio = (lenUpper + lenMiddle * mRatio) / lenMax; //(lenUpper + lenMiddle + lenLower);

      // Law of Cosines - SSS: cos(C) = (a^2 + b^2 - c^2) / 2ab
      float a = lenUpper, b = lenMiddle * mRatio, c = targetLength * ratio;
      float n = Mathf.Clamp((Mathf.Pow(a,2) + Mathf.Pow(b,2) - Mathf.Pow(c,2)) / (2*a*b),-1f,1f);
      float n1 = Mathf.Clamp((Mathf.Pow(a,2) + Mathf.Pow(c,2) - Mathf.Pow(b,2)) / (2*a*c),-1f,1f);
      //angle for upper-mid
      float angleA = Mathf.Rad2Deg * (Mathf.Acos(n)-Mathf.PI);

      //angle for torso-upper
      float thetaC = Vector3.Angle(-Vector3.up, dir);
      if (dir.z < 0f) thetaC *= -1f;
      float angleC = thetaC + (Mathf.Rad2Deg * (Mathf.Acos(n1)));

      a = lenLower;
      b = lenMiddle * (1f-mRatio);
      c = targetLength * (1f-ratio);
      n = Mathf.Clamp((Mathf.Pow(a,2) + Mathf.Pow(b,2) - Mathf.Pow(c,2)) / (2*a*b),-1f,1f);
      //angle for mid-lower
      float angleB = Mathf.Rad2Deg * (Mathf.PI - Mathf.Acos(n));

      if (leftLeg)
      {
        angleA *= -1f;
        angleB *= -1f;
        angleC *= -1f;
      }
      root.localRotation = Quaternion.Euler(new Vector3(0f,rootY,angleC));
      stifle.localRotation = Quaternion.Euler(new Vector3(0f,0f,angleA));
      hock.localRotation = Quaternion.Euler(new Vector3(0f,0f,angleB));
    }
  }

  public void Init()
  {
    //prevHockRotation = hock.rotation.eulerAngles;
    float fr = mech.InverseTransformDirection(foot.rotation.eulerAngles).y;
    Debug.Log("------------------------");
    //Debug.Log(mech.TransformDirection(foot.rotation.eulerAngles));
    //Debug.Log(mech.InverseTransformDirection(foot.rotation.eulerAngles));
    //Debug.Log(Quaternion.Angle(mech.rotation, foot.rotation));
    Vector3 a = mech.InverseTransformDirection(foot.rotation.eulerAngles);
    Vector3 b = mech.TransformDirection(a);
    Debug.Log(a);
    Debug.Log(foot.rotation.eulerAngles);
    Debug.Log(b);
    //Debug.Log(Quaternion.FromToRotation(mech.rotation.eulerAngles,foot.rotation.eulerAngles).eulerAngles);
    //float y = Quaternion.FromToRotation(mech.rotation.eulerAngles,foot.rotation.eulerAngles).eulerAngles.y;
    //initFootRotation = Quaternion.Euler(new Vector3(0,fr,0));
    initFootRotation = mech.InverseTransformDirection(foot.rotation.eulerAngles);
    rootY = root.localRotation.eulerAngles.y;
    lenUpper = Vector3.Distance(root.position,stifle.position);
    lenMiddle = Vector3.Distance(stifle.position, hock.position);
    lenLower = Vector3.Distance(hock.position, foot.position);
    lenMax = lenUpper + lenMiddle + lenLower;
    UpdateRatio();
  }

  public void UpdateRatio()
  {
    if (dynamicRatio)
    {
      mRatio = lenUpper / (lenUpper+lenLower);
    }
  }
}
