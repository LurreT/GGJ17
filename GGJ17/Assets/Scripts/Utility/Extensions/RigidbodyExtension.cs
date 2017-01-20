using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System;

namespace ExtensionMethods {

    public static class RigidbodyExtension {
        /// <summary>
        /// Disables/Reenables the rigidbody.
        /// Deacting will disable the gravity and collision detection, as well as resetting the linear and angular velocity.
        /// Reactivating will do the opposite, except will not affect the velocity.
        /// </summary>
        /// <param name="body"></param>
        /// <param name="state">True to activate, False to deactivate</param>
        public static void SetEnabled(this Rigidbody body, bool state) {
            body.useGravity = state;
            body.detectCollisions = state;
			body.isKinematic = !state;
			body.interpolation = state ? RigidbodyInterpolation.Interpolate : RigidbodyInterpolation.None;

            if (state == false) {
                body.velocity = Vector3.zero;
                body.angularVelocity = Vector3.zero;
            }
        }

        public static bool IsEnabled(this Rigidbody rbody) {
            return rbody.detectCollisions && rbody.useGravity;
        }
    }
}