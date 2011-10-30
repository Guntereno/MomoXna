using System;



namespace Momo.Core.Collision2D
{
    public class CollidableGroupInfo
    {
        private Flags m_groupMembership;
        private Flags m_collidesWithGroups;


        public Flags GroupMembership
        {
            get { return m_groupMembership; }
            set { m_groupMembership = value; }
        }


        public Flags CollidesWithGroups
        {
            get { return m_collidesWithGroups; }
            set { m_collidesWithGroups = value; }
        }


        public bool DoesCollideWithGroup(ref CollidableGroupInfo groupInfo)
        {
            return groupInfo.m_collidesWithGroups.AnyFlagSet(m_groupMembership.Data);
        }
    }
}
