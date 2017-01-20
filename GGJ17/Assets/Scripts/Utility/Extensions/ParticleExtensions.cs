using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System;

namespace ExtensionMethods {
	public static class ParticleExtensions {
		public static void Play(this ParticleSystem[] list) {
			foreach(var system in list) {
				system.Play();
			}
		}

		public static void Stop(this ParticleSystem[] list) {
			foreach (var system in list) {
				system.Stop();
			}
		}

		public static void Pause(this ParticleSystem[] list) {
			foreach (var system in list) {
				system.Pause();
			}
		}
	}
}
