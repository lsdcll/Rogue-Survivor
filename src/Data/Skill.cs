﻿using System;
using System.Collections.Generic;

namespace RogueSurvivor.Data
{
    [Serializable]
    class Skill
    {
        int m_ID;
        int m_Level;

        public int ID
        {
            get { return m_ID; }
        }

        public int Level
        {
            get { return m_Level; }
            set { m_Level = value; }
        }

        public Skill(int id)
        {
            m_ID = id;
        }
        public Skill(int id, int level)
        {
            m_ID = id;
            m_Level = level;
        }
    }

    [Serializable]
    class SkillTable
    {
        Dictionary<int, Skill> m_Table;   // allocated only if needed (some actors have 0 skills)

        /// <summary>
        /// Get all skills null if no skills.
        /// </summary>
        public IEnumerable<Skill> Skills
        {
            get
            {
                if (m_Table == null)
                    return null;

                return m_Table.Values;
            }
        }

        /// <summary>
        /// List all non-zero skills ids as an array; null if no skills.
        /// </summary>
        public int[] SkillsList
        {
            get
            {
                if (m_Table == null)
                    return null;

                int[] array = new int[CountSkills];
                int i = 0;
                foreach (Skill s in m_Table.Values)
                {
                    array[i++] = s.ID;
                }

                return array;
            }
        }

        /// <summary>
        /// Count non-zero skills.
        /// </summary>
        public int CountSkills
        {
            get
            {
                if (m_Table == null)
                    return 0;

                return m_Table.Values.Count;
            }
        }

        public int CountTotalSkillLevels
        {
            get
            {
                int sum = 0;
                foreach (Skill s in m_Table.Values)
                    sum += s.Level;
                return sum;
            }
        }

        public SkillTable()
        {
        }

        public SkillTable(IEnumerable<Skill> startingSkills)
        {
            if (startingSkills == null)
                throw new ArgumentNullException("startingSkills");

            foreach (Skill sk in startingSkills)
                AddSkill(sk);
        }

        public Skill GetSkill(int id)
        {
            if (m_Table == null)
                return null;

            Skill sk;
            if (m_Table.TryGetValue(id, out sk))
                return sk;
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>0 for a missing skill</returns>
        public int GetSkillLevel(int id)
        {
            Skill sk = GetSkill(id);
            if (sk == null)
                return 0;
            return sk.Level;
        }

        public void AddSkill(Skill sk)
        {
            if (m_Table == null)
                m_Table = new Dictionary<int, Skill>(3);

            if (m_Table.ContainsKey(sk.ID))
            {
                UpdateSkill(sk.ID, sk.Level);
                return;
            }
            if (m_Table.ContainsValue(sk))
                throw new ArgumentException("skill already in table");

            m_Table.Add(sk.ID, sk);
        }
        public void UpdateSkill(int id, int level)
        {
            m_Table[id].Level = level;
        }

        public void AddOrIncreaseSkill(int id)
        {
            if (m_Table == null)
                m_Table = new Dictionary<int, Skill>(3);

            Skill sk = GetSkill(id);
            if (sk == null)
            {
                sk = new Skill(id);
                m_Table.Add(id, sk);
            }

            ++sk.Level;
        }

        public void DecOrRemoveSkill(int id)
        {
            if (m_Table == null) return;

            Skill sk = GetSkill(id);
            if (sk == null) return;
            if (--sk.Level <= 0)
            {
                m_Table.Remove(id);
                if (m_Table.Count == 0)
                    m_Table = null;
            }
        }
      

    }
}
