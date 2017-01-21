using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;

[CustomEditor(typeof(CannonCollection))]
public class e_CannonCollection : Editor {

	private CannonCollection script;
	private ReorderableList strikes;

	private void OnEnable() {
		script = target as CannonCollection;

		// Strikes
		strikes = new ReorderableList(serializedObject, serializedObject.FindProperty("strikes"), true, true, true, true);

		strikes.drawHeaderCallback += rect => {
			Rect labelRect = new Rect(rect.x, rect.y, rect.width - 82, rect.height);
			Rect timestampRect = new Rect(labelRect.xMax + 4, rect.y, 50, rect.height);
			Rect flytimeRect = new Rect(timestampRect.xMax + 4, rect.y, 24, rect.height);
			EditorGUI.LabelField(labelRect, strikes.serializedProperty.displayName);
			EditorGUI.LabelField(timestampRect, script.parent.timeByBPM ? "BEAT" : "TIME");
			EditorGUI.LabelField(flytimeRect, "PRE");
		};

		strikes.drawElementCallback += (rect, index, isActive, isFocused) => {
			rect.yMin += 2;
			rect.yMax -= 2;

			var item = strikes.serializedProperty.GetArrayElementAtIndex(index);

			var spawner = item.FindPropertyRelative("spawnerIndex");
			var unit = item.FindPropertyRelative("unitIndex");
			var timestamp = item.FindPropertyRelative("timestamp");
			var flytime = item.FindPropertyRelative("flytime");

			if (flytime.floatValue <= float.Epsilon) flytime.floatValue = 3;

			Rect spawnerRect = new Rect(rect.x, rect.y, (rect.width - 82) * 0.5f - 2, rect.height);
			Rect unitRect = new Rect(spawnerRect.xMax + 4, rect.y, spawnerRect.width, rect.height);
			Rect timestampRect = new Rect(unitRect.xMax + 4, rect.y, 50, rect.height);
			Rect flytimeRect = new Rect(timestampRect.xMax + 4, rect.y, 24, rect.height);
			
			spawner.intValue = DrawPopupThingie(spawnerRect, spawner, script.parent.spawners.ConvertAll<Object>(c=>c));
			unit.intValue = DrawPopupThingie(unitRect, unit, script.parent.units.ConvertAll<Object>(c=>c));
			
			EditorGUI.PropertyField(timestampRect, timestamp, GUIContent.none);
			EditorGUI.PropertyField(flytimeRect, flytime, GUIContent.none);
		};
	}

	private int DrawPopupThingie(Rect rect, SerializedProperty prop, List<Object> basedOf) {

		List<string> display = new List<string>();

		// fill array
		for (int i = 0; i < basedOf.Count; i++) {
			var p = basedOf[i];
			if (p != null)
				if (p is CannonCollection)
					display.Add("[" + i + "] <collections not supported>");
				else
					display.Add("[" + i + "] " + p.name);
			else display.Add("[" + i + "] <null>");
		}

		// check which is selected
		int selected = prop.intValue;
		selected = EditorGUI.Popup(rect, selected, display.ToArray());

		return selected;
	}

	public override void OnInspectorGUI() {
		DrawDefaultInspector();

		serializedObject.Update();
		EditorGUILayout.Space();
		if (script.parent == null) {
			EditorGUILayout.HelpBox("ERROR! No parent is assigned", MessageType.Error);
		} else {
			strikes.DoLayoutList();
			EditorGUILayout.Space();

			if (GUILayout.Button("Sort strikes by TIME")) {

				bool any;
				do {
					any = false;

					for (int i = 0; i < strikes.serializedProperty.arraySize - 1; i++) {
						var a = strikes.serializedProperty.GetArrayElementAtIndex(i);
						var b = strikes.serializedProperty.GetArrayElementAtIndex(i+1);

						if (a.FindPropertyRelative("timestamp").floatValue > b.FindPropertyRelative("timestamp").floatValue) {
							strikes.serializedProperty.MoveArrayElement(i + 1, i);
							any = true;
						}

					}

				} while (any);

			}
		}

		EditorGUILayout.Space();

		serializedObject.ApplyModifiedProperties();
	}

}
