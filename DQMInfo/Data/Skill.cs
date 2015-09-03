using System;
using System.Collections.Generic;

namespace DQMInfo.Data
{
    public class Skill : IOutputable
    {
        public String Name;
        public int MP;
        public int MinLV;
        public int MinHP;
        public int MinATK;
        public int MinDEF;
        public int MinAGL;
        public int MinINT;
        public String Description;

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

        public void Output()
        {
			System.Console.WriteLine("Skill: {0}", this.Name);
			System.Console.WriteLine("MP Used: {0}", this.MP == -1 ? "ALL" : this.MP.ToString());
			System.Console.WriteLine("Prerequisites to learn:");
			System.Console.WriteLine("\tLevel Required: >{0}", this.MinLV);
			System.Console.WriteLine("\tHP Required: >{0}", this.MinHP);
			System.Console.WriteLine("\tAttack Required: >{0}", this.MinATK);
			System.Console.WriteLine("\tDefense Required: >{0}", this.MinDEF);
			System.Console.WriteLine("\tAgility Required: >{0}", this.MinAGL);
			System.Console.WriteLine("\tIntelligence Required: >{0}", this.MinINT);
			System.Console.WriteLine("Description:");
			System.Console.WriteLine("\t{0}", this.Description);
        }

		public static List<String> OutputMultiple(List<Skill> outputList)
		{
			List<String> ret = new List<String>();
			ret.Add
				(
					String.Format
						(
							"{0,9} {1,3} {2,3} {3,3} {4,3} {5,3} {6,3} {7,3} {8}",
							"SkillName",
							"MP",
							"LVL",
							"HP",
							"ATK",
							"DEF",
							"AGL",
							"INT",
							"Description"
						)
				);

			foreach(var thisOutput in outputList)
			{
				ret.Add
				(
					String.Format
					(
						"{0,9} {1,3} {2,3} {3,3} {4,3} {5,3} {6,3} {7,3} {8}",
						thisOutput.Name,
						thisOutput.MP == -1 ? "ALL" : thisOutput.MP.ToString(),
						thisOutput.MinLV,
						thisOutput.MinHP,
						thisOutput.MinATK,
						thisOutput.MinDEF,
						thisOutput.MinAGL,
						thisOutput.MinINT,
						thisOutput.Description
					)
				);
			}

			return ret;
		}
    }
}

