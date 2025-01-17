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
		// � �̵� (Sin � ���)
		float curveOffset = Mathf.Sin(time * curveFrequency) * curveIntensity;
		Vector3 curve = transform.right * (isLeft ? -curveOffset : curveOffset);
		isLeft = !isLeft;
		// �̵� 
		transform.position += curve * Time.deltaTime;

		// �ð� ������Ʈ
		time += Time.deltaTime * flag;
		flag = time > 1.0f ? -1 : time < 0.0f ? 1 : flag;
	}
}