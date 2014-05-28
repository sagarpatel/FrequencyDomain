/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2014.                                   *
* Leap Motion proprietary. Licensed under Apache 2.0                           *
* Available at http://www.apache.org/licenses/LICENSE-2.0.html                 *
\******************************************************************************/

using UnityEngine;
using System.Collections;
using Leap;

// Interface for all fingers.
public abstract class FingerModel : MonoBehaviour {

  public const int NUM_BONES = 4;
  public const int NUM_JOINTS = 5;

  public Finger.FingerType fingerType = Finger.FingerType.TYPE_INDEX;

  private Hand hand_;
  private Finger finger_;

  HandController controller_;

  public void SetController(HandController controller) {
    controller_ = controller;
  }

  public HandController GetController() {
    return controller_;
  }

  public void SetLeapHand(Hand hand) {
    hand_ = hand;
    finger_ = hand.Fingers[(int)fingerType];
  }

  public Hand GetLeapHand() { return hand_; }
  public Finger GetLeapFinger() { return finger_; }

  public abstract void InitFinger();

  public abstract void UpdateFinger();

  // Returns the location of the given joint on the finger in relation to the controller.
  protected Vector3 GetJointPosition(int joint) {
    Vector3 local_position;
    if (joint >= NUM_BONES)
      local_position = finger_.Bone((Bone.BoneType.TYPE_DISTAL)).NextJoint.ToUnityScaled();
    else
      local_position = finger_.Bone((Bone.BoneType)(joint)).PrevJoint.ToUnityScaled();

    return controller_.transform.TransformPoint(local_position);
  }

  // Returns the center of the given bone on the finger in relation to the controller.
  protected Vector3 GetBonePosition(int bone_type) {
    Bone bone = finger_.Bone((Bone.BoneType)(bone_type));
    Vector3 local_position = 0.5f * (bone.PrevJoint.ToUnityScaled() +
                                     bone.NextJoint.ToUnityScaled());

    return controller_.transform.TransformPoint(local_position);
  }

  // Returns the direction the given bone is facing on the finger in relation to the controller.
  protected Vector3 GetBoneDirection(int bone_type) {
    Vector3 local_direction = finger_.Bone((Bone.BoneType)(bone_type)).Direction.ToUnity();
    return controller_.transform.TransformDirection(local_direction);
  }

  // Returns the rotation quaternion of the given bone in relation to the controller.
  protected Quaternion GetBoneRotation(int bone_type) {
    Quaternion local_rotation = finger_.Bone((Bone.BoneType)(bone_type)).Basis.Rotation();
    return controller_.transform.rotation * local_rotation;
  }
}
