using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[CanEditMultipleObjects, CustomEditor(typeof(Cannon), false)]
public class e_Cannon : Editor {

	private bool move;
	private Vector2 mouse;
	private RaycastHit lastHit;
	private Material mat2;

	void OnEnable() {
		mat2 = new Material(Shader.Find("Sprites/Default"));
		mat2.color = new Color(1f, 0f, 0f, 0.1f);
		EditorUtility.SetDirty(target);
	}

	void OnDisable() {
		DestroyImmediate(mat2);
	}

	public override bool RequiresConstantRepaint() {
		return true;
	}
	void OnSceneGUI() {

		if (!move) return;

		Event e = Event.current;

		int controlID = GUIUtility.GetControlID(FocusType.Passive);
		switch (e.GetTypeForControl(controlID)) {
			case EventType.MouseDown:
				if (e.button == 1) {
					move = false;
					GUIUtility.hotControl = 0;
					e.Use();
				} else if (e.button == 0) {
					GUIUtility.hotControl = controlID;
					e.Use();
				}
				break;

			case EventType.MouseUp:
				if (e.button != 0) break;

				// Move em
				Undo.RecordObject(target, "Change cannon target");
				(target as Cannon).target = lastHit.point;
				Repaint();

				move = false;
				GUIUtility.hotControl = 0;
				e.Use();
				break;

			case EventType.MouseMove:
				mouse = Event.current.mousePosition;
				HandleUtility.Repaint();
				break;

			case EventType.KeyDown:
				if (e.keyCode == KeyCode.Escape && move) {
					// Abort
					move = false;
					GUIUtility.hotControl = 0;
					e.Use();
				}
				break;

		}

		if (Camera.current && e.type == EventType.Repaint) {
			Ray ray = HandleUtility.GUIPointToWorldRay(mouse);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, Camera.current.farClipPlane, Physics.DefaultRaycastLayers)) {
				Handles.color = new Color(0, 1, 1);
				Handles.ArrowCap(controlID, hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal), 1);
				Handles.CircleCap(controlID, hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal), 1);

				#region Draw meshes
				foreach (var filter in hit.collider.GetComponentsInChildren<MeshFilter>()) {
					mat2.SetPass(0);
					Handles.color = Color.magenta;
					Graphics.DrawMeshNow(filter.sharedMesh, Matrix4x4.TRS(filter.transform.position, filter.transform.rotation, filter.transform.lossyScale));
				}
				#endregion

				#region Draw box colliders
				foreach (var box in hit.collider.GetComponentsInChildren<BoxCollider>()) {
					Handles.color = Color.red;

					Vector3 pos = box.transform.position;
					Vector3 half = box.size / 2;

					Handles.DrawLine(pos + box.transform.TransformVector(box.center + new Vector3(half.x, half.y, half.z)),
						pos + box.transform.TransformVector(box.center + new Vector3(half.x, half.y, -half.z)));

					Handles.DrawLine(pos + box.transform.TransformVector(box.center + new Vector3(half.x, half.y, -half.z)),
						pos + box.transform.TransformVector(box.center + new Vector3(-half.x, half.y, -half.z)));

					Handles.DrawLine(pos + box.transform.TransformVector(box.center + new Vector3(-half.x, half.y, -half.z)),
						pos + box.transform.TransformVector(box.center + new Vector3(-half.x, half.y, half.z)));

					Handles.DrawLine(pos + box.transform.TransformVector(box.center + new Vector3(-half.x, half.y, half.z)),
						pos + box.transform.TransformVector(box.center + new Vector3(half.x, half.y, half.z)));


					Handles.DrawLine(pos + box.transform.TransformVector(box.center + new Vector3(half.x, half.y, half.z)),
						pos + box.transform.TransformVector(box.center + new Vector3(half.x, -half.y, half.z)));

					Handles.DrawLine(pos + box.transform.TransformVector(box.center + new Vector3(half.x, half.y, -half.z)),
						pos + box.transform.TransformVector(box.center + new Vector3(half.x, -half.y, -half.z)));

					Handles.DrawLine(pos + box.transform.TransformVector(box.center + new Vector3(-half.x, half.y, half.z)),
						pos + box.transform.TransformVector(box.center + new Vector3(-half.x, -half.y, half.z)));

					Handles.DrawLine(pos + box.transform.TransformVector(box.center + new Vector3(-half.x, half.y, -half.z)),
						pos + box.transform.TransformVector(box.center + new Vector3(-half.x, -half.y, -half.z)));


					Handles.DrawLine(pos + box.transform.TransformVector(box.center + new Vector3(half.x, -half.y, half.z)),
						pos + box.transform.TransformVector(box.center + new Vector3(half.x, -half.y, -half.z)));

					Handles.DrawLine(pos + box.transform.TransformVector(box.center + new Vector3(half.x, -half.y, -half.z)),
						pos + box.transform.TransformVector(box.center + new Vector3(-half.x, -half.y, -half.z)));

					Handles.DrawLine(pos + box.transform.TransformVector(box.center + new Vector3(-half.x, -half.y, -half.z)),
						pos + box.transform.TransformVector(box.center + new Vector3(-half.x, -half.y, half.z)));

					Handles.DrawLine(pos + box.transform.TransformVector(box.center + new Vector3(-half.x, -half.y, half.z)),
						pos + box.transform.TransformVector(box.center + new Vector3(half.x, -half.y, half.z)));
				}
				#endregion

				Handles.Label(hit.point + Vector3.up * HandleUtility.GetHandleSize(hit.point) / 2, "Change cannon target\nLMB to move\nRMB to abort");
				lastHit = hit;
			}
		}
	}

	public override void OnInspectorGUI() {

		DrawDefaultInspector();

		serializedObject.Update();
		var prop = serializedObject.FindProperty("target");
		prop.vector3Value = EditorGUILayout.Vector3Field(prop.displayName, prop.vector3Value);
		EditorGUILayout.Space();

		if (Selection.transforms.Length > 0) {
			GUI.enabled = !move;
			if (GUILayout.Button(move ? "Moving..." : "Change cannon target")) {
				move = true;
			}
			GUI.enabled = true;
		}
		serializedObject.ApplyModifiedProperties();
	}

}
