using System;
using System.Collections.Generic;
using System.Linq;

namespace DQMInfo.Data
{
	public class Monster : IOutputable, IBreedable
	{
		public Family MemberOf;
		public String Name;
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

		public void Output()
		{
			System.Console.WriteLine("Monster: {0}", this.Name);
			System.Console.WriteLine("Family: {0}", this.MemberOf.Name);
			System.Console.WriteLine("Max Level: {0}", this.MaxLV);
			System.Console.WriteLine("Growth Rate (0-31): {0}", this.GrowthRate);
			System.Console.WriteLine("Base HP: {0}", this.BaseHP);
			System.Console.WriteLine("Base MP: {0}", this.BaseMP);
			System.Console.WriteLine("Base Attack: {0}", this.BaseATK);
			System.Console.WriteLine("Base Defense: {0}", this.BaseDEF);
			System.Console.WriteLine("Base Agility: {0}", this.BaseAGL);
			System.Console.WriteLine("Base Intelligence: {0}", this.BaseINT);
			System.Console.WriteLine("Learned Skills: ");
			foreach(String thisLine in Skill.OutputMultiple(LearnedSkills))
			{
				System.Console.WriteLine("\t{0}", thisLine);
			}
		}
	}
}

