﻿/*
 * This module written by Claw. For more details, please visit
 * http://forum.kerbalspaceprogram.com/threads/97285
 * 
 * This mod is covered under the CC-BY-NC-SA license. See the readme.txt for more details.
 * (https://creativecommons.org/licenses/by-nc-sa/4.0/)
 * 
 *
 * ModuleProceduralFairingPlus - Written for KSP v1.0
 * 
 * - Fixes some bugs with pulling and replacing fairings.
 * - Activates a tweakable slider for the user to select the number of panels on the fairing.
 * - Activates a tweakable slider to control ejection forces on the panels.
 * 
 * Change Log:
 * 
 * - v00.01  (4 May 15)    Initial Release
 * 
 */

using UnityEngine;
using KSP;
using System.Reflection;

namespace ClawKSP
{
    public class MPFPlus : PartModule
    {

        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = true, guiName = "Panels")]
        [UI_FloatRange(minValue = 1f, maxValue = 8f, stepIncrement = 1f)]
        public float nArcs = 0f;
            
        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = true, guiName = "Ejection Force")]
        [UI_FloatRange(minValue = 0f, maxValue = 200f, stepIncrement = 10f)]
        public float ejectionForce = -1f;

        ModuleProceduralFairing FairingModule;

        private void GetModule()
        {
            if (null == part.Modules) { return; }

            for (int indexModules = 0; indexModules < part.Modules.Count; indexModules++)
            {
                if ("ModuleProceduralFairing" == part.Modules[indexModules].moduleName)
                {
                    FairingModule = (ModuleProceduralFairing)part.Modules[indexModules];
                }
            }
        }  // GetModule


        public void Start ()
        {
            Debug.Log("MPFPlus.OnStart(): v00.01");

            GetModule();

            if (null == FairingModule) { return; }

            if (ejectionForce == -1)
            {
                ejectionForce = FairingModule.ejectionForce;
            }

            if (nArcs == 0)
            {
                nArcs = FairingModule.nArcs;
            }

            GameEvents.onPartRemove.Add(RemovePart);
        }

        public void RemovePart(GameEvents.HostTargetAction<Part, Part> RemovedPart)
        {
            if (null == FairingModule) { return; }

            if (RemovedPart.target == part)
            {
                if (FairingModule.xSections.Count > 0)
                {
                    Debug.LogWarning("Deleting Fairing");
                    FairingModule.DeleteFairing();
                }
                MethodInfo MPFMethod = FairingModule.GetType().GetMethod("DumpInterstage", BindingFlags.NonPublic | BindingFlags.Instance);

                if (MPFMethod != null)
                {
                    MPFMethod.Invoke(FairingModule, new object[] { });
                }
            }
        }

        public void FixedUpdate ()
        {
            if (FairingModule == null)
            {
                part.Modules.Remove(this);
                return;
            }

            if (FairingModule.nArcs != nArcs)
            {
                FairingModule.nArcs = (int) nArcs;

                MethodInfo MPFMethod = FairingModule.GetType().GetMethod("WipeMesh", BindingFlags.NonPublic | BindingFlags.Instance);

                if (MPFMethod != null)
                {
                    MPFMethod.Invoke(FairingModule, new object[] { });
                }

                MPFMethod = FairingModule.GetType().GetMethod("SpawnMeshes", BindingFlags.NonPublic | BindingFlags.Instance);

                if (MPFMethod != null)
                {
                    MPFMethod.Invoke(FairingModule, new object[] { true });
                }
            }

            FairingModule.ejectionForce = ejectionForce;
            
        }

        public void OnDestroy ()
        {
            GameEvents.onPartRemove.Remove(RemovePart);
        }
    }
}
