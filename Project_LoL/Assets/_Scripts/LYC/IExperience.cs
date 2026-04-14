public interface IExperience
{
	/// <summary>
	/// 플레이어가 레벨업 시 Broadcast할 Event
	/// </summary>
	public event System.Action LevelUp;

	/// <summary>
	/// 플레이어 경험치 추가
	/// </summary>
	/// <param name="exp">경험치 양</param>
	public void AddExperience(int exp);


	public void KnockBack();
}