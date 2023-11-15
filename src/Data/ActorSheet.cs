using System;
using System.Collections.Generic;
using System.Linq;

namespace RogueSurvivor.Data
{
    [Serializable]
    class ActorSheet
    {
        SkillTable m_SkillTable = new SkillTable();
        SkillTable m_SharedSkillTable = new SkillTable();
        SkillTable m_CombinedSkillTable = new SkillTable();

        [NonSerialized]
        public static readonly ActorSheet BLANK = new ActorSheet(0, 0, 0, 0, 0, Attack.BLANK, Defence.BLANK, 0, 0, 0, 0);

        public int BaseHitPoints { get; private set; }
        public int BaseStaminaPoints { get; private set; }
        public int BaseFoodPoints { get; private set; }
        public int BaseSleepPoints { get; private set; }
        public int BaseSanity { get; private set; }
        public Attack UnarmedAttack { get; private set; }
        public Defence BaseDefence { get; private set; }
        public int BaseViewRange { get; private set; }
        public int BaseAudioRange { get; private set; }
        public float BaseSmellRating { get; private set; }
        public int BaseInventoryCapacity { get; private set; }

        public SkillTable SkillTable
        {
            get { return m_SkillTable; }
            set { m_SkillTable = value; }
        }
        public SkillTable SharedSkillTable
        {
            get { return m_SharedSkillTable; }
            set { m_SharedSkillTable = value; }
        }

        public SkillTable CombinedSkillTable
        {
            get { return m_CombinedSkillTable; }
        }

        public ActorSheet(int baseHitPoints, int baseStaminaPoints,
            int baseFoodPoints, int baseSleepPoints, int baseSanity,
            Attack unarmedAttack, Defence baseDefence,
            int baseViewRange, int baseAudioRange, int smellRating,
            int inventoryCapacity)
        {
            this.BaseHitPoints = baseHitPoints;
            this.BaseStaminaPoints = baseStaminaPoints;
            this.BaseFoodPoints = baseFoodPoints;
            this.BaseSleepPoints = baseSleepPoints;
            this.BaseSanity = baseSanity;
            this.UnarmedAttack = unarmedAttack;
            this.BaseDefence = baseDefence;
            this.BaseViewRange = baseViewRange;
            this.BaseAudioRange = baseAudioRange;
            this.BaseSmellRating = smellRating / 100.0f;
            this.BaseInventoryCapacity = inventoryCapacity;
        }

        public ActorSheet(ActorSheet copyFrom)
        {
            if (copyFrom == null)
                throw new ArgumentNullException("copyFrom");

            this.BaseHitPoints = copyFrom.BaseHitPoints;
            this.BaseStaminaPoints = copyFrom.BaseStaminaPoints;
            this.BaseFoodPoints = copyFrom.BaseFoodPoints;
            this.BaseSleepPoints = copyFrom.BaseSleepPoints;
            this.BaseSanity = copyFrom.BaseSanity;
            this.UnarmedAttack = copyFrom.UnarmedAttack;
            this.BaseDefence = copyFrom.BaseDefence;
            this.BaseViewRange = copyFrom.BaseViewRange;
            this.BaseAudioRange = copyFrom.BaseAudioRange;
            this.BaseSmellRating = copyFrom.BaseSmellRating;
            this.BaseInventoryCapacity = copyFrom.BaseInventoryCapacity;

            if (copyFrom.SkillTable.Skills != null)
                m_SkillTable = new SkillTable(copyFrom.SkillTable.Skills);
            if (copyFrom.SharedSkillTable.Skills != null)
                m_SharedSkillTable = new SkillTable(copyFrom.SharedSkillTable.Skills);
        }

        public void CalculateCombinedSkillTable()
        {
            m_CombinedSkillTable = new SkillTable();

            //add all shared skills to new table
            int[] SharedSkillsList = m_SharedSkillTable.SkillsList;
            int[] BaseSkillsList = m_SkillTable.SkillsList;
            if(SharedSkillsList != null)
            {
                for(int i = 0; i < SharedSkillsList.Length; i++)
                {
                    int id = SharedSkillsList[i];
                    m_CombinedSkillTable.AddSkill(new Skill(id, m_SharedSkillTable.GetSkillLevel(id)));
                }
               
            }
            //add base skills to new table if not yet present: SHARED SKILLS WILL ALWAYS HAVE A HIGHER LEVEL THAN BASE
            if (BaseSkillsList != null)
            {
                for(int i = 0; i < BaseSkillsList.Length; i++)
                {
                    int id = BaseSkillsList[i];
                    //if combined skill table contains data
                    if(m_CombinedSkillTable.SkillsList != null)
                    {
                        //if skill hasnt been added yet, add it : Duplicate skills may exist, but shared level will always be higher, so duplicates can be ignored
                        if (!m_CombinedSkillTable.SkillsList.Contains(id)) m_CombinedSkillTable.AddSkill(new Skill(id, m_SkillTable.GetSkillLevel(id)));
                    }
                    else m_CombinedSkillTable.AddSkill(new Skill(id, m_SkillTable.GetSkillLevel(id)));

                }
            }
            
            
        }
        
        public void LearnSharedSkills(SkillTable leaderSkills)
        {
            //Upkeep reset shared skills
            m_SharedSkillTable = new SkillTable();

            foreach (Skill leaderSkill in leaderSkills.Skills)
            {
                Skill actorSkill = new Skill(leaderSkill.ID, m_SkillTable.GetSkillLevel(leaderSkill.ID));

                //Skill only shareable if present in Gameplay.Skills.PARTY_SHARE_SKILLS 
                //AND
                //The leader has a higher level in that skill
                //Otherwise, no skill share
                if (Gameplay.Skills.PARTY_SHARE_SKILLS.Contains((Gameplay.Skills.IDs)leaderSkill.ID)
                    && leaderSkill.Level > actorSkill.Level)
                {
                    //Calculate shared skill level as average between leader and follower
                    actorSkill.Level = (int)Math.Ceiling(((double)(leaderSkill.Level + actorSkill.Level) / 2));

                    //add the skill to the actors shared skill table
                    m_SharedSkillTable.AddSkill(actorSkill);

                }
            }
            CalculateCombinedSkillTable();
        }
        public void ForgetSharedSkills()
        {
            m_SharedSkillTable = new SkillTable();
            CalculateCombinedSkillTable();
        }
    }
}
