using UnityEngine;

public class CurvedProjectile : Projectile
{
	[Space(10)]
	[SerializeField] float curveIntensity = 1f;
	[SerializeField] float curveFrequency = 1f;
	static bool isLeft = false;
	float time = 0.0f;
	int flag = 1;
	public override void MoveProjectile()
	{
		base.MoveProjectile();
		// 곡선 이동 (Sin 곡선 기반)
		float curveOffset = Mathf.Sin(time * curveFrequency) * curveIntensity;
		Vector3 curve = transform.right * (isLeft ? -curveOffset : curveOffset);
		isLeft = !isLeft;
		// 이동 
		transform.position += curve * Time.deltaTime;

		// 시간 업데이트
		time += Time.deltaTime * flag;
		flag = time > 1.0f ? -1 : time < 0.0f ? 1 : flag;
	}
}