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
	
	/// <summary>
	/// 레벨업 이후 주변 적 넉백 시 사용할 메소드
	/// </summary>
	public void KnockBack();
}