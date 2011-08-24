using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Graphics;



namespace Momo.Core.Shaders
{
    public class Shader
    {
        // --------------------------------------------------------------------
        // -- Private Members
        // --------------------------------------------------------------------
        private string m_name;

        private Effect m_effect = null;
        private EffectParameter[] m_managedParameterList = new EffectParameter[(int)ParameterSemantic.Type.kCount];
        private List<EffectParameter> m_unmanagedParameterList = new List<EffectParameter>(10);


        // --------------------------------------------------------------------
        // -- Public Properties
        // --------------------------------------------------------------------
        #region Properties
        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        public Effect Effect
        {
            get { return m_effect; }
            set { m_effect = value; }
        }

        public EffectParameter[] ManagedParameterList
        {
            get { return m_managedParameterList; }
        }

        public List<EffectParameter> UnmanagedParameterList
        {
            get { return m_unmanagedParameterList; }
        }
        #endregion


        // --------------------------------------------------------------------
        // -- Constructor/Deconstructor
        // --------------------------------------------------------------------
        public Shader(Effect effect, string name)
        {
            m_effect = effect;
            m_name = name;

            // Set just the first technique in the file, not really a long term choice, but
            // good enough for now.
            m_effect.CurrentTechnique = m_effect.Techniques[0];

            // Populate the parameter list.
            PopulateParameterLists();
        }


        public void SetOnDevice()
        {
             m_effect.CurrentTechnique.Passes[0].Apply();
        }


        public EffectParameter GetUnmanagedParameter(string semantic)
        {
            int paramCnt = m_unmanagedParameterList.Count;
            for (int i = 0; i < paramCnt; ++i)
            {
                EffectParameter param = m_unmanagedParameterList[i];
                if (param.Semantic != null && param.Semantic == semantic)
                {
                    return param;
                }
            }

            return null;
        }



        // --------------------------------------------------------------------
        // -- Private Methods
        // --------------------------------------------------------------------
        private void PopulateParameterLists()
        {
            int paramCnt = m_effect.Parameters.Count;
            for (int i = 0; i < paramCnt; ++i)
            {
                EffectParameter param = m_effect.Parameters[i];
                if (param.Semantic != null && param.Semantic != "")
                {
                    bool found = false;
                    for (int j = 0; j < (int)ParameterSemantic.Type.kCount; ++j)
                    {
                        if (param.Semantic == ParameterSemantic.GetSemanticName((ParameterSemantic.Type)j))
                        {
                            m_managedParameterList[j] = param;
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                        m_unmanagedParameterList.Add(param);
                }
            }
        }
    }
}
