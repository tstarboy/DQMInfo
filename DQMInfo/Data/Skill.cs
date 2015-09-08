using System;
using System.Collections.Generic;
using System.Linq;

namespace DQMInfo.Data
{
	public class Skill : IData
    {
        public String Name;

		public String SearchName { get { return this.Name; } }

        public int MP;
        public int MinLV;
        public int MinHP;
        public int MinATK;
        public int MinDEF;
        public int MinAGL;
        public int MinINT;
        public String Description;
		public List<Monster> MonstersThatLearn;

        public Skill()
        {
        }

        public Skill
            (
                String inName,
                int inMP,
                int inMinLV,
                int inMinHP,
                int inMinATK,
                int inMinDEF,
                int inMinAGL,
                int inMinINT,
                String inDescription
            )
        {
            this.Name = inName;
            this.MP = inMP;
            this.MinLV = inMinLV;
            this.MinHP = inMinHP;
            this.MinATK = inMinATK;
            this.MinDEF = inMinDEF;
            this.MinAGL = inMinAGL;
            this.MinINT = inMinINT;
            this.Description = inDescription;
        }

        public static List<Skill> BuildSkillList(List<String[]> ParsedCSV)
        {
            List<Skill> ret = new List<Skill>();

            foreach(String[] thisSkill in ParsedCSV)
            {
                if (thisSkill.Length != 9)
                    throw new Exception();
                ret.Add
                (
                    new Skill
                    (
                        thisSkill[0],
                        thisSkill[1] == "ALL" ? -1 : Int32.Parse(thisSkill[1]),
                        Int32.Parse(thisSkill[2]),
                        Int32.Parse(thisSkill[3]),
                        Int32.Parse(thisSkill[4]),
                        Int32.Parse(thisSkill[5]),
                        Int32.Parse(thisSkill[6]),
                        Int32.Parse(thisSkill[7]),
                        thisSkill[8]
                    )
                );
            }

            return ret;
        }

		public static List<Skill> AddMonsterList(List<Skill> SkillList, List<Monster> MonsterList)
		{
			for(int i = 0; i < SkillList.Count; i++)
			{
				SkillList[i].MonstersThatLearn = MonsterList.AsQueryable().Where(x => x.LearnedSkills.Select(y => y.Name).Contains(SkillList[i].Name)).ToList();
			}
			return SkillList;
		}

		public List<String> OutputSingle()
        {
			List<String> ret = new List<String>();
			ret.Add(String.Format("Skill: {0}", this.Name));
			ret.Add(String.Format("MP Used: {0}", this.MP == -1 ? "ALL" : this.MP.ToString()));
			ret.Add(String.Format("Prerequisites to learn:"));
			ret.Add(String.Format("\tLevel Required: >{0}", this.MinLV));
			ret.Add(String.Format("\tHP Required: >{0}", this.MinHP));
			ret.Add(String.Format("\tAttack Required: >{0}", this.MinATK));
			ret.Add(String.Format("\tDefense Required: >{0}", this.MinDEF));
			ret.Add(String.Format("\tAgility Required: >{0}", this.MinAGL));
			ret.Add(String.Format("\tIntelligence Required: >{0}", this.MinINT));
			ret.Add(String.Format("Description:"));
			ret.Add(String.Format("\t{0}", this.Description));
			ret.Add(String.Format("Monsters that learn {0}:", this.Name));
			foreach(Monster thisMonster in MonstersThatLearn)
			{
				ret.Add(String.Format("\t{0}", thisMonster.Name));
			}

			return ret;
        }

		public String OutputMultipleHeader()
		{
			return String.Format
				(
					"{0,9} {1,3} {2,3} {3,3} {4,3} {5,3} {6,3} {7,3} {8, -55}",
					"SkillName",
					"MP",
					"LVL",
					"HP",
					"ATK",
					"DEF",
					"AGL",
					"INT",
					"Description"
				);
		}

		public String OutputMultipleLine()
		{
			return String.Format
				(
					"{0,9} {1,3} {2,3} {3,3} {4,3} {5,3} {6,3} {7,3} {8, -55}",
					this.Name,
					this.MP == -1 ? "ALL" : this.MP.ToString(),
					this.MinLV,
					this.MinHP,
					this.MinATK,
					this.MinDEF,
					this.MinAGL,
					this.MinINT,
					this.Description
				);
		}
    }
}

