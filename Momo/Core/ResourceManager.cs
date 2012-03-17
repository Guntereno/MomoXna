using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace Momo.Core
{
    public class ResourceManager
    {
        #region Fields

        ContentManager mContent;
        private Dictionary<Type, Dictionary<string, object>> mResources = new Dictionary<Type,Dictionary<string,object>>();

        private static ResourceManager msInstance = null;
        public static ResourceManager Instance { get { return msInstance; } }

        #endregion


        #region Constructor

        public ResourceManager(ContentManager content)
        {
            if (msInstance != null)
                throw new System.Exception("Attempt to instantiate Singleton twice!");
            msInstance = this;

            mContent = content;
        }

        #endregion


        #region Public Interface

        public T Get<T>(string name)
        {
            Type type = typeof(T);
            if(!mResources.Keys.Contains(type))
            {
                mResources[type] = new Dictionary<string,object>();
            }

            if(!mResources[type].Keys.Contains(name))
            {
                mResources[type][name] = mContent.Load<T>(name);
            }

            return (T)mResources[type][name];
        }

        #endregion
    }
}
