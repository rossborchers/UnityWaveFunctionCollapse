using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WaveFunctionCollapseManager))]
[CanEditMultipleObjects]
public class WaveFunctionCollapseManagerEditor : Editor
{
	public override void OnInspectorGUI()
	{
		if (GUILayout.Button("Generate"))
		{
			((WaveFunctionCollapseManager) target).GenerateAsync();
		}

		DrawDefaultInspector();
	}
}
