/*
 * This module written by Claw. For more details, please visit
 * http://forum.kerbalspaceprogram.com/threads/97285
 * 
 * This mod is covered under the CC-BY-NC-SA license. See the readme.txt for more details.
 * (https://creativecommons.org/licenses/by-nc-sa/4.0/)
 * 
 *
 * ModuleJettisonFix - Written for KSP v1.00
 * 
 * - Reactivates jettison shrouds for parts inside of aero shells and cargo bays.
 *   When shrouds are missing, some parts appear to float.
 * 
 * Change Log:
 * - v01.00 (26 Apr 15) Initial Release
 * 
 */

using UnityEngine;
using KSP;

namespace ClawKSP
{

    public class MJFix : PartModule
    {

        // Insert Tweakable
        [KSPField(guiName = "Fairing", isPersistant = true, guiActiveEditor = true)]
        [UI_Toggle(enabledText = "Removed", disabledText = "Normal", scene = UI_Scene.Editor)]
        public bool isJettisoned = false;

        [KSPField(isPersistant = true)]
        private bool noLongerShielded = false;

        ModuleJettison JettisonModule;

        public void Start ()
        {
            JettisonModule = (ModuleJettison) part.Modules["ModuleJettison"];
            Debug.Log("MJFix.Start(): v01.00");
        }

        public void LateUpdate()
        {
            if (HighLogic.LoadedScene == GameScenes.EDITOR || HighLogic.LoadedScene == GameScenes.FLIGHT)
            {
                if (part.ShieldedFromAirstream == true)
                {
                    Fields["isJettisoned"].guiActiveEditor = true;
                    JettisonModule.isJettisoned = isJettisoned;
                    JettisonModule.jettisonTransform.gameObject.SetActive(!isJettisoned);
                    
                }
                else
                {
                    Fields["isJettisoned"].guiActiveEditor = false;
                }
            }
            else if (HighLogic.LoadedScene == GameScenes.FLIGHT)
            {
                Fields["isJettisoned"].guiActiveEditor = false;
                if (part.ShieldedFromAirstream == true && noLongerShielded == false)
                {
                    JettisonModule.isJettisoned = isJettisoned;
                    JettisonModule.jettisonTransform.gameObject.SetActive(!isJettisoned);
                }
                else
                {
                    noLongerShielded = true;
                }
            }
        }

    }
}
