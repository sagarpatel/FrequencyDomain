/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2014.                                   *
* Leap Motion proprietary. Licensed under Apache 2.0                           *
* Available at http://www.apache.org/licenses/LICENSE-2.0.html                 *
\******************************************************************************/

using UnityEngine;
using System.Collections.Generic;
using Leap;

public class HandController : MonoBehaviour {

  // Reference distance from thumb base to pinky base in mm.
  protected const float MODEL_PALM_WIDTH = 85.0f;

  public bool separateLeftRight = false;
  public HandModel leftGraphicsModel;
  public HandModel leftPhysicsModel;
  public HandModel rightGraphicsModel;
  public HandModel rightPhysicsModel;

  private Controller leap_controller_;
  private Dictionary<int, HandModel> graphics_hands_;
  private Dictionary<int, HandModel> physics_hands_;

  void Start() {
    leap_controller_ = new Controller();
    graphics_hands_ = new Dictionary<int, HandModel>();
    physics_hands_ = new Dictionary<int, HandModel>();

    if (leap_controller_ == null) {
      Debug.LogWarning(
          "Cannot connect to controller. Make sure you have Leap Motion v2.0+ installed");
    }
  }

  private void IgnoreHandCollisions(HandModel hand) {
    // Ignores hand collisions with immovable objects.
    Collider[] colliders = gameObject.GetComponentsInChildren<Collider>();
    Collider[] hand_colliders = hand.GetComponentsInChildren<Collider>();

    for (int i = 0; i < colliders.Length; ++i) {
      for (int h = 0; h < hand_colliders.Length; ++h) {
        if (colliders[i].rigidbody == null)
          Physics.IgnoreCollision(colliders[i], hand_colliders[h]);
      }
    }
  }

  HandModel CreateHand(HandModel model) {
    HandModel hand_model = Instantiate(model, transform.position, transform.rotation)
                           as HandModel;
    hand_model.gameObject.SetActive(true);
    IgnoreHandCollisions(hand_model);
    return hand_model;
  }

  private void UpdateModels(Dictionary<int, HandModel> all_hands, HandList leap_hands,
                            HandModel left_model, HandModel right_model) {
    List<int> ids_to_check = new List<int>(all_hands.Keys);

    // Go through all the active hands and update them.
    int num_hands = leap_hands.Count;
    for (int h = 0; h < num_hands; ++h) {
      Hand leap_hand = leap_hands[h];
      
      // Only create or update if the hand is enabled.
      if ((leap_hand.IsLeft && left_model != null) ||
          (leap_hand.IsRight && right_model != null)) {

        ids_to_check.Remove(leap_hand.Id);

        // Create the hand and initialized it if it doesn't exist yet.
        if (!all_hands.ContainsKey(leap_hand.Id)) {
          HandModel model = leap_hand.IsLeft? left_model : right_model;
          HandModel new_hand = CreateHand(model);
          new_hand.SetLeapHand(leap_hand);
          new_hand.SetController(this);
          new_hand.InitHand();
          all_hands[leap_hand.Id] = new_hand;
        }

        // Make sure we update the Leap Hand reference.
        HandModel hand_model = all_hands[leap_hand.Id];
        hand_model.SetLeapHand(leap_hand);

        // Set scaling based on reference hand.
        float hand_scale = leap_hand.PalmWidth / MODEL_PALM_WIDTH;
        hand_model.transform.localScale = hand_scale * transform.localScale;

        hand_model.UpdateHand();
      }
    }

    // Destroy all hands with defunct IDs.
    for (int i = 0; i < ids_to_check.Count; ++i) {
      Destroy(all_hands[ids_to_check[i]].gameObject);
      all_hands.Remove(ids_to_check[i]);
    }
  }

  void Update() {
    if (leap_controller_ == null)
      return;

    Frame frame = leap_controller_.Frame();
    UpdateModels(graphics_hands_, frame.Hands, leftGraphicsModel, rightGraphicsModel);
  }

  void FixedUpdate() {
    if (leap_controller_ == null)
      return;

    Frame frame = leap_controller_.Frame();
    UpdateModels(physics_hands_, frame.Hands, leftPhysicsModel, rightPhysicsModel);
  }
}
