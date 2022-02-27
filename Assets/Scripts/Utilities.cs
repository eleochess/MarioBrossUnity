using UnityEngine;

public class Utilities {

	public static Vector2 GetVector2 (Vector3 pa_vector3) {
		return new Vector2 (pa_vector3.x, pa_vector3.y);
	}

	public static GameObject GetPlayerObject() {
		return GameObject.FindWithTag(StringNaming.Tag_Player);
	}
}
