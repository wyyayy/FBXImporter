using UnityEngine;
using System.Collections;

public class TestImporter : MonoBehaviour
{
	public string[] modelPaths;

	void Start() {
		for (int i = 0; i < modelPaths.Length; ++i) {
			string path = modelPaths [i];
			if (!string.IsNullOrEmpty (path)) {
				path = Application.dataPath + "/Model/" + path + ".fbx";
				GameObject instance = ModelImporter.Importer.Import (path);
			}
		}
	}
}