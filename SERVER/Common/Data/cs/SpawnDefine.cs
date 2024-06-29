//
// Auto Generated Code By excel2json
// https://neil3d.gitee.io/coding/excel2json.html
// 1. 每个 Sheet 形成一个 Struct 定义, Sheet 的名称作为 Struct 的名称
// 2. 表格约定：第一行是变量名称，第二行是变量类型

// Generate From SpawnDefine.xlsx

public class SpawnDefine
{
	public int ID; // ID
	public int MapId; // 地图ID
	public string Pos; // 刷怪位置
	public string Dir; // 刷怪方向
	public int UnitID; // 单位类型
	public int Level; // 单位等级
	public int Period; // 刷新周期（秒）
	public string killRewardList; // 击杀奖励
	public float WalkRange; // 巡逻范围
	public float ChaseRange; // 追击范围
	public float AttackRange; // 攻击范围
}


// End of Auto Generated Code
