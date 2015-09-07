using System;
using System.Collections.Generic;
using System.Linq;

using DQMInfo.Output;

namespace DQMInfo.Data
{
	public class Monster : IData, IBreedable
	{
		public Family MemberOf;
		public String Name;

		public String SearchName { get { return this.Name; } }
		public String BreedName { get { return this.Name; } }

		public int MaxLV;
		public int GrowthRate;
		public int BaseHP;
		public int BaseMP;
		public int BaseATK;
		public int BaseDEF;
		public int BaseAGL;
		public int BaseINT;
		public List<Skill> LearnedSkills;

		public bool isFinal { get { return false; } }

		public Monster()
		{
		}

		public Monster
			(
				Family inMemberOf,
				String inName,
				int inMaxLV,
				int inGrowthRate,
				int inBaseHP,
				int inBaseMP,
				int inBaseATK,
				int inBaseDEF,
				int inBaseAGL,
				int inBaseINT,
				Skill inSkill1,
				Skill inSkill2,
				Skill inSkill3
			)
		{
			this.MemberOf = inMemberOf;
			this.Name = inName;
			this.MaxLV = inMaxLV;
			this.GrowthRate = inGrowthRate;
			this.BaseHP = inBaseHP;
			this.BaseMP = inBaseMP;
			this.BaseATK = inBaseATK;
			this.BaseDEF = inBaseDEF;
			this.BaseAGL = inBaseAGL;
			this.BaseINT = inBaseINT;
			this.LearnedSkills = new List<Skill>();
			this.LearnedSkills.Add(inSkill1);
			this.LearnedSkills.Add(inSkill2);
			this.LearnedSkills.Add(inSkill3);
		}

		public static List<Monster> BuildMonsterList(List<String[]> ParsedCSV, List<Skill> SkillList, List<Family> FamilyList)
		{
			List<Monster> ret = new List<Monster>();

			foreach(String[] thisMonster in ParsedCSV)
			{
				if (thisMonster.Length != 13)
					throw new Exception();
				ret.Add
				(
					new Monster
					(
						FamilyList.AsQueryable().Where(x => x.Name == thisMonster[0]).First(),
						thisMonster[1],
						Int32.Parse(thisMonster[2]),
						Int32.Parse(thisMonster[3]),
						Int32.Parse(thisMonster[4]),
						Int32.Parse(thisMonster[5]),
						Int32.Parse(thisMonster[6]),
						Int32.Parse(thisMonster[7]),
						Int32.Parse(thisMonster[8]),
						Int32.Parse(thisMonster[9]),
						SkillList.AsQueryable().Where(x => x.Name == thisMonster[10]).First(),
						SkillList.AsQueryable().Where(x => x.Name == thisMonster[11]).First(),
						SkillList.AsQueryable().Where(x => x.Name == thisMonster[12]).First()
					)
				);
			}

			return ret;
		}

		public List<String> OutputSingle()
		{
			List<String> ret = new List<String>();
			ret.Add(String.Format("Monster: {0}", this.Name));
			ret.Add(String.Format("Family: {0}", this.MemberOf.Name));
			ret.Add(String.Format("Max Level: {0}", this.MaxLV));
			ret.Add(String.Format("Growth Rate (0-31): {0}", this.GrowthRate));
			ret.Add(String.Format("Base HP: {0}", this.BaseHP));
			ret.Add(String.Format("Base MP: {0}", this.BaseMP));
			ret.Add(String.Format("Base Attack: {0}", this.BaseATK));
			ret.Add(String.Format("Base Defense: {0}", this.BaseDEF));
			ret.Add(String.Format("Base Agility: {0}", this.BaseAGL));
			ret.Add(String.Format("Base Intelligence: {0}", this.BaseINT));
			ret.Add(String.Format("Learned Skills: "));
			foreach(String thisLine in OutputData<Skill>.OutputMultiple(LearnedSkills))
			{
				ret.Add(String.Format("\t{0}", thisLine));
			}

			return ret;
		}

		public String OutputMultipleHeader()
		{
			return String.Format
				(
					"{0,9} {1,11} {2,5} {3,2} {4,3} {5,3} {6,3} {8,3} {9,3} {10,3} {11,31}", 
					"Monster", 
					"Family", 
					"MaxLV", 
					"GR", 
					"HP", 
					"MP", 
					"ATK", 
					"DEF", 
					"AGL", 
					"INT", 
					"Learned Skills"
				);
		}

		public String OutputMultipleLine()
		{
			return String.Format
				(
					"{0,9} {1,11} {2,5} {3,2} {4,3} {5,3} {6,3} {8,3} {9,3} {10,3} {11,31}", 
					this.Name, 
					this.MemberOf.Name, 
					this.MaxLV, 
					this.GrowthRate, 
					this.BaseHP, 
					this.BaseMP, 
					this.BaseATK, 
					this.BaseDEF, 
					this.BaseAGL, 
					this.BaseINT, 
					String.Join(", ", this.LearnedSkills.Select(x => x.Name).ToList())
				);
		}
	}
}

