using UnityEngine;
using System;
using System.Collections;
using System.Linq;

public class SphereCast {

	public static Vector3[] GetSphereDirections(int numDirections)
	{
		var pts = new Vector3[numDirections];
		var inc = Mathf.PI * (3 - Mathf.Sqrt(5));
		var off = 2f / numDirections;
		
		foreach (var k in Enumerable.Range(0, numDirections))
		{
			var y = k * off - 1 + (off / 2);
			var r = Mathf.Sqrt(1 - y * y);
			var phi = k * inc;
			var x = (float)(Mathf.Cos(phi) * r);
			var z = (float)(Mathf.Sin(phi) * r);
			pts[k] = new Vector3(x, y, z);
		}
		
		return pts;
	}

}
