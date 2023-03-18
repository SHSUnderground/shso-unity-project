using System.Collections.Generic;
using UnityEngine;

public interface SplineInterpolator
{
	bool Initialize(List<SplinePoint> ctrlPoints);

	float ArcLength();

	float ArcLength(float t0, float t1);

	Vector3 Derivative(float t);

	float TimeFromDistance(float t1, float s);

	Vector3 GetVector(float t);

	void DrawGizmos();
}
