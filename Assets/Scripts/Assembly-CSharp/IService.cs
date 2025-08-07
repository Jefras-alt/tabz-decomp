public interface IService
{
	void Initialize();

	void Update();

	void LateUpdate();

	void FixedUpdate();

	void Destroy();

	float UpdateTime();

	float LateUpdateTime();

	float FixedUpdateTime();
}
