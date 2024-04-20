//
// Auto Generated Code By excel2json
// https://neil3d.gitee.io/coding/excel2json.html
// 1. 每个 Sheet 形成一个 Struct 定义, Sheet 的名称作为 Struct 的名称
// 2. 表格约定：第一行是变量名称，第二行是变量类型

// Generate From SkillDefine.xlsx

public class SkillDefine
{
	public int ID; // 编号
	public int TID; // 单位类型
	public int Code; // 技能码
	public string Name; // 技能名称
	public string Description; // 技能描述
	public int Level; // 技能等级
	public int MaxLevel; // 技能上限
	public string Type; // 类别
	public string Icon; // 技能图标
	public string TargetType; // 目标类型
	public float CD; // 冷却时间
	public int SpellRange; // 施法距离
	public float IntonateTime; // 施法前摇
	public int Cost; // 魔法消耗
	public string Anim1; // 前摇动作
	public string Anim2; // 激活动作
	public int ReqLevel; // 等级要求
	public bool IsMissile; // 是否投射物
	public string Missile; // 投射物
	public int MissileSpeed; // 投射速度
	public string HitArt; // 击中效果
	public int Area; // 影响区域
	public float Duration; // 持续时间
	public float Interval; // 伤害间隔
	public float[] HitDelay; // 命中延迟
	public int[] BUFF; // 附加效果
	public float AD; // 物理攻击
	public float AP; // 法术攻击
	public float ADC; // 物攻加成
	public float APC; // 法攻加成
}


// End of Auto Generated Code
