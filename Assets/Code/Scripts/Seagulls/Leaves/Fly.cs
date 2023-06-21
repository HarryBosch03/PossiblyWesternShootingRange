﻿using ShootingRangeGame.AI.BehaviourTrees.Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ShootingRangeGame.Seagulls.Leaves
{
    public class Fly : Leaf<SeagullBrain>
    {
        private float launchForce = 2.0f;
        private float minFlyForce = 5.0f;
        private float maxFlyForce = 12.0f;
        private float flySpeed = 2.0f;

        private float targetHeight;
        private int state;

        public override string Name => $"{base.Name} Fly";

        protected override void OnStart(BehaviourTree tree)
        {
            Target.ShiftLookDirection(10.0f);
            targetHeight = Target.Seagull.transform.position.y + Random.value * 4.0f + 1.0f;
            Target.Seagull.rigidbody.AddForce(Vector3.up * launchForce, ForceMode.VelocityChange);
            state = 0;
        }

        public override BehaviourTree.AbandonResponse RespondToAbandonRequest() => BehaviourTree.AbandonResponse.CannotAbandon;

        protected override BehaviourTree.Result OnExecute(BehaviourTree tree)
        {
            var transform = Target.Seagull.transform;

            Target.Seagull.MoveVector = Target.Seagull.LookDirection * flySpeed;

            switch (state)
            {
                case 0:
                    var height = transform.position.y;
                    if (height > targetHeight)
                    {
                        state++;
                        break;
                    }

                    Target.Animation = "Flap";
                    var flyForce = Mathf.Lerp(maxFlyForce, minFlyForce, height / targetHeight);
                    Target.Seagull.rigidbody.AddForce(Vector3.up * flyForce, ForceMode.Acceleration);
                    break;
                case 1:
                    Target.Animation = "Glide";
                    if (Target.Seagull.Grounded) state++;
                    break;
                default:
                    return BehaviourTree.Result.Success;
            }

            return BehaviourTree.Result.Pending;
        }
    }
}