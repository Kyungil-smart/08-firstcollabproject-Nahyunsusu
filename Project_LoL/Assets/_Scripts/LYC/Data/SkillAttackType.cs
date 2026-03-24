using System;

[Flags]
public enum AttackType
{
	None = 0,
	Melee = 1 << 0, // 1  - 근접
	Ranged = 1 << 1, // 2  - 원거리
	Projectile = 1 << 2, // 4  - 투사
	Area = 1 << 3, // 8  - 범위
	Wide = 1 << 4, // 16 - 광범위
	Spread = 1 << 5, // 32 - 산탄
}