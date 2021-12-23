using BepInEx;
using System.Collections;
using UnityEngine;

namespace Trainer
{
    [BepInPlugin("TrainerDev", "Trainer", "1.0")]
    public class BepInLoader : BaseUnityPlugin
    {
        public static BepInLoader instance;
        public static BepInEx.Logging.ManualLogSource log;
        private bool startFired = false;

        private void Awake()
        {
            instance = this;
            log = base.Logger;

            this.transform.parent = null;
            DontDestroyOnLoad(this);

            StartCoroutine(this.StartEx()); // Just in case Start() doesn't fire. Some game's don't.
        }

        private void Start()
        {
            if (!startFired)
            {
                TrainerLoader.Init();
                startFired = true;
            }
        }

        IEnumerator StartEx()
        {
            this.Start();
            yield return new WaitForSeconds(5);

            if (!TrainerLoader.initialized) { startFired = false; this.Start(); }
        }
    }
}


