
using System.Threading;
using UnityEngine;

namespace Trainer
{
    // Primary Loader Class for Injecting via an Injector Tool (i.e MInjector, MInject, SharpMonoInjector). Also gets refenrced by BepInLoader.

    public class TrainerLoader
    {
        #region[Declarations]

        // Declare our GameObject
        private static GameObject _Load;
        public static GameObject Load { get { return _Load; } set { _Load = value; } }

        public static bool initialized = false;
        
        #endregion

        #region[Loader Methods]

        // Our Loader Method. Must be Static
        public static void Init()
        {
            #region[Create Trainer GameObject]

            // Create a new Gameobject
            TrainerLoader._Load = new GameObject("Trainer");

            #region[Set our GameObject as Root]
            
            Load.transform.parent = null;     // Old Unity
            //Load.transform.SetParent(null); // New Unity
            Transform gameRootObject = Load.transform.root;
            if (gameRootObject != null)
            {
                if (gameRootObject.gameObject != TrainerLoader.Load)
                {
                    gameRootObject.parent = Load.transform; // Old Unity
                    //gameRootObject.SetParent(null);       // New Unity
                }
            }

            #endregion

            // Add Components to our GameObject, we add our Injected menu, etc. Pay attention to load order, it can cause issues.
            TrainerLoader._Load.AddComponent<TrainerMenu>();

            // Tell Unity not to destory our GameObject on scene change
            GameObject.DontDestroyOnLoad(TrainerLoader._Load);

            #endregion

            initialized = true;
        }

        #endregion

    }
}
