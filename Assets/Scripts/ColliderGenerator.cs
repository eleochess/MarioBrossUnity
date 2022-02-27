using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ColliderGenerator : MonoBehaviour {

	public class Vector2Connection {
		public Vector2 vector2_point;
		public Vector2 vector2_point_real;
		public bool is_start;
		public bool is_connected = false;
	}

	public bool pu_redoCollider = false;

	PolygonCollider2D poly2D;
	float blockSize = 0, blockSizeReal = 0, min_x = 0, max_x = 0, min_y = 0, max_y = 0, width_blocks = 0, heigth_blocks = 0;

	void Start () {
		ColliderGeneration ();
	}

	void FixedUpdate() {
		if (pu_redoCollider) {
			Destroy (GetComponent<PolygonCollider2D> ());
			ColliderGeneration ();
			pu_redoCollider = false;
		}
	}

	void ColliderGeneration() {
		blockSize = 0; blockSizeReal = 0; min_x = 0; max_x = 0; min_y = 0; max_y = 0; width_blocks = 0; heigth_blocks = 0;
		List<Transform> list_tr = new List<Transform>();

		bool first_loop = true;
		Transform loop_transform;
		BoxCollider2D loop_boxCollider2D;

		foreach (Transform tr in transform) {
			loop_transform = tr;
			if (tr.tag == StringNaming.Tag_Hitbox) {
				loop_boxCollider2D = loop_transform.GetComponent<BoxCollider2D>();
				loop_boxCollider2D.isTrigger = true;
			}

			if (first_loop) {
				poly2D = null;
				poly2D = gameObject.AddComponent<PolygonCollider2D>();
				blockSizeReal = loop_transform.GetComponent<SpriteRenderer>().bounds.size.x;
				blockSize = RoundDecimalNumber(blockSizeReal, 2);
				min_x = tr.localPosition.x;
				max_x = tr.localPosition.x;
				min_y = tr.localPosition.y;
				max_y = tr.localPosition.y;
				first_loop = false;
			}

			list_tr.Add(tr);

			if (tr.localPosition.x < min_x) min_x = tr.localPosition.x;
			if (tr.localPosition.x > max_x) max_x = tr.localPosition.x;
			if (tr.localPosition.y < min_y) min_y = tr.localPosition.y;
			if (tr.localPosition.y > max_y) max_y = tr.localPosition.y;
		}

		if (first_loop) {
			Debug.Log("No child gameobjects found.");
			return;
		}

		//PolygonCollider2D poly2D = GetComponent<PolygonCollider2D>();

		// +1 to take the last block, +1 to analize the one more empty block in case there's a vector in the polygonCollider
		width_blocks = Mathf.Round((max_x - min_x) / blockSize + 2);
		heigth_blocks = Mathf.Round((max_y - min_y) / blockSize + 2);

		float curr_x = min_x, curr_y = max_y, curr_x_real = curr_x, curr_y_real = curr_y;
		bool exist_block = false;

		List<bool> oneUpLevelTopBlocks = new List<bool>(), generatingTopBlocks = new List<bool>();
		Transform temp_tr;

		List<List<Vector2Connection>> list_paths = new List<List<Vector2Connection>>();
		List<Vector2Connection> vector2_line = new List<Vector2Connection>(), vector2_line_temp = new List<Vector2Connection>(), vector2_line_found = new List<Vector2Connection>(), vector2_line_combinable = new List<Vector2Connection>();
		Vector2Connection vector2_start = new Vector2Connection(), vector2_end = new Vector2Connection(), vector2_found = new Vector2Connection();

		#region GETTING ONLY THE CORRECT VECTORS
		//Building imaginary square
		bool upLeft = false, upRight = false, downLeft = false, downRight = false;

		for (int i = 0; i < heigth_blocks; i++) {
			curr_y_real = max_y - (blockSize * (float)i);
			curr_y = RoundDecimalNumber(curr_y_real, 2);
			for (int j = 0; j < width_blocks; j++) {
				curr_x_real = min_x + (blockSize * (float)j);
				curr_x = RoundDecimalNumber(curr_x_real, 2);

				exist_block = list_tr.Where(u => RoundDecimalNumber(u.localPosition.x, 2) == curr_x && RoundDecimalNumber(u.localPosition.y, 2) == curr_y).Any();
				generatingTopBlocks.Add(exist_block);

				downRight = exist_block;

				if (oneUpLevelTopBlocks.Any()) {
					upRight = oneUpLevelTopBlocks[j];
					if (j > 0) {
						upLeft = oneUpLevelTopBlocks[j - 1];
					}
				}

				if (j > 0) {
					downLeft = generatingTopBlocks[j - 1];
				}

				#region COMBINACION DE CONDICIONALES PARA LOS CUADRANTES <= Mexican detected

				if (!upLeft && !upRight && !downLeft && downRight) {
					// 00
					// 01
					vector2_start = new Vector2Connection() { vector2_point = new Vector2() { x = curr_x, y = curr_y }, vector2_point_real = new Vector2() { x = curr_x_real, y = curr_y_real }, is_start = true };
				}
				else if (!upLeft && !upRight && downLeft && !downRight) {
					// 00
					// 10
					vector2_end = new Vector2Connection() { vector2_point = new Vector2() { x = curr_x, y = curr_y }, vector2_point_real = new Vector2() { x = curr_x_real, y = curr_y_real }, is_start = false };
					List<Vector2Connection> vector2_line_temporal = new List<Vector2Connection>();
					vector2_line_temporal.Add(vector2_start); vector2_line_temporal.Add(vector2_end);
					list_paths.Add(vector2_line_temporal);
				}
				else if (!upLeft && upRight && !downLeft && !downRight) {
					// 01
					// 00
					vector2_start = new Vector2Connection() { vector2_point = new Vector2() { x = curr_x, y = curr_y }, vector2_point_real = new Vector2() { x = curr_x_real, y = curr_y_real }, is_start = true };
					foreach (List<Vector2Connection> line in list_paths) {
						vector2_found = line.Where(l => !l.is_connected && l.vector2_point.x == curr_x).FirstOrDefault();
						if (vector2_found != null) {
							vector2_found.is_connected = true;
							vector2_start.is_connected = true;
							vector2_line_temp.Add(vector2_start); vector2_line_temp.AddRange(line);
							line.Clear();
							line.AddRange(vector2_line_temp);
							vector2_line_found = line;
							break;
						}
					}
				}
				else if (upLeft && !upRight && !downLeft && !downRight) {
					// 10
					// 00
					vector2_end = new Vector2Connection() { vector2_point = new Vector2() { x = curr_x, y = curr_y }, vector2_point_real = new Vector2() { x = curr_x_real, y = curr_y_real }, is_start = false };
					if (vector2_line_found != null) {
						vector2_line_temp.Add(vector2_end); vector2_line_temp.AddRange(vector2_line_found);
						int index = list_paths.FindIndex(l => l == vector2_line_found);
						list_paths[index] = vector2_line_temp;
					}
				}
				else if (!upLeft && upRight && downLeft && downRight) {
					// 01
					// 11
					vector2_end = new Vector2Connection() { vector2_point = new Vector2() { x = curr_x, y = curr_y }, vector2_point_real = new Vector2() { x = curr_x_real, y = curr_y_real }, is_start = false };
					if (vector2_start.is_connected) {
						foreach (List<Vector2Connection> line in list_paths) {
							vector2_found = line.Where(l => !l.is_connected && l.vector2_point.x == curr_x).FirstOrDefault();
							if (vector2_found != null) {
								vector2_found.is_connected = true;
								vector2_end.is_connected = true;
								vector2_line_combinable = line;
								break;
							}
						}
						vector2_line_temp = vector2_line_found;
						vector2_line_temp.Add(vector2_end);
						vector2_line_temp.AddRange(vector2_line_combinable);

						int index = list_paths.FindIndex(l => l == vector2_line_found);
						list_paths[index] = vector2_line_temp;

						index = list_paths.FindIndex(l => l == vector2_line_combinable);
						list_paths.RemoveAt(index);
					}
					else {
						foreach (List<Vector2Connection> line in list_paths) {
							vector2_found = line.Where(l => !l.is_connected && l.vector2_point.x == curr_x).FirstOrDefault();
							if (vector2_found != null) {
								vector2_found.is_connected = true;
								vector2_end.is_connected = true;

								vector2_line_temp = new List<Vector2Connection>();
								foreach (Vector2Connection v in line) {
									vector2_line_temp.Add(v);
								}

								line.Clear();
								line.Add(vector2_start);
								line.Add(vector2_end);
								line.AddRange(vector2_line_temp);
								break;
							}
						}
					}
				}
				else if (upLeft && !upRight && downLeft && downRight) {
					// 10
					// 11
					vector2_start = new Vector2Connection() { vector2_point = new Vector2() { x = curr_x, y = curr_y }, vector2_point_real = new Vector2() { x = curr_x_real, y = curr_y_real }, is_start = true };
					foreach (List<Vector2Connection> line in list_paths) {
						vector2_found = line.Where(l => !l.is_connected && l.vector2_point.x == curr_x).FirstOrDefault();
						if (vector2_found != null) {
							vector2_found.is_connected = true;
							vector2_start.is_connected = true;
							line.Add(vector2_start);
							vector2_line_found = line;
							break;
						}
					}
				}
				else if (upLeft && upRight && downLeft && !downRight) {
					// 11
					// 10
					vector2_start = new Vector2Connection() { vector2_point = new Vector2() { x = curr_x, y = curr_y }, vector2_point_real = new Vector2() { x = curr_x_real, y = curr_y_real }, is_start = true };
				}
				else if (upLeft && upRight && !downLeft && downRight) {
					// 11
					// 01
					vector2_end = new Vector2Connection() { vector2_point = new Vector2() { x = curr_x, y = curr_y }, vector2_point_real = new Vector2() { x = curr_x_real, y = curr_y_real }, is_start = false };
					vector2_line.Add(vector2_start); vector2_line.Add(vector2_end);
					list_paths.Add(vector2_line);
				}

				#endregion

				upLeft = false;
				upRight = false;
				downLeft = false;
				downRight = false;
				vector2_line_temp = new List<Vector2Connection>();
			}
			oneUpLevelTopBlocks = generatingTopBlocks;
			generatingTopBlocks = new List<bool>();
		}
		#endregion

		List<Vector2> list_vector2 = new List<Vector2>();
		poly2D.pathCount = list_paths.Count;
		for (int i = 0; i < list_paths.Count; i++) {
			list_vector2 = new List<Vector2>();
			foreach (Vector2Connection vector2 in list_paths[i]) {
				list_vector2.Add(vector2.vector2_point_real);
			}
			poly2D.SetPath(i, list_vector2.ToArray());
		}

		// INGORING COLLIDER
		foreach (Transform tr in list_tr) {
			Physics2D.IgnoreCollision (tr.GetComponent<BoxCollider2D> (), poly2D);
		}
	}

	float RoundDecimalNumber(float number, float decimalLimit) {
		float tempNumber = Mathf.Pow(10, decimalLimit);
		return Mathf.Round(number * tempNumber) / tempNumber;
	}
}
