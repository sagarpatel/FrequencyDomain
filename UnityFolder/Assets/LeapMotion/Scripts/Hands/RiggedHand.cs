/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2014.                                   *
* Leap Motion proprietary. Licensed under Apache 2.0                           *
* Available at http://www.apache.org/licenses/LICENSE-2.0.html                 *
\******************************************************************************/

using UnityEngine;
using System.Collections;
using Leap;

public class RiggedHand : HandModel {

  public override void InitHand() {
    UpdateHand();
  }

  public override void UpdateHand() {
    Transform arm = GetArm();
    arm.position = GetPalmPosition();
    arm.rotation = GetPalmRotation();

    for (int i = 0; i < fingers.Length; ++i) {
      if (fingers[i] != null)
        fingers[i].UpdateFinger();
    }
  }

  Transform GetArm() {
    return transform.Find("Root").Find("Arm");
  }

  protected Vector3 GetPalmPosition() {
    return GetController().transform.TransformPoint(GetLeapHand().PalmPosition.ToUnityScaled());
  }

  protected Quaternion GetPalmRotation() {
    return GetController().transform.rotation * GetLeapHand().Basis.Rotation();
  }
}
