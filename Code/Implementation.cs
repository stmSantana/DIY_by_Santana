using System;
using System.IO;
using System.Linq;
using ModSettings;
using System.Collections.Generic; // List<>
using MelonLoader;
using HarmonyLib;
using UnityEngine;

namespace DiyBySantana
{
    internal class Implementation : MelonMod
    {
        public override void OnApplicationStart()
        {
            Settings.OnLoad();/// ModSettings
            LoggerInstance.Msg($"DIY - Version {BuildInfo.Version}");
        }
        public override void OnSceneWasInitialized(int level, string name)
        {
            DiyChangeColor.ChangeWood("Board");
            DiyChangeColor.ChangeWood("Furniture");
            DiyChangeColor.ChangeWood("Tableware");
            
            ChangeLayerOfGear.DiyChangeLayerNum();
        }

    }

    //---------------------------------------------------------------------------------------
    // Break Down
    // Add nail box if Yield has Reclaimed Wood.
    //---------------------------------------------------------------------------------------

    [HarmonyPatch(typeof(BreakDown), "DoBreakDown")]
    internal class BreakDownGetNail
    {
        private static void Prefix(BreakDown __instance)
        {
            GameObject DiyBreakDownObj = __instance.gameObject;
            string DiyBreakDownObjName = DiyBreakDownObj.name;
            
            int DiyArrayLenght = __instance.m_YieldObject.Length;

            string DiyAddedYield = "GEAR_DiyANailBoxC"; // Yield object --- Nail box
            GameObject DiyAddedYieldObj = Resources.Load(DiyAddedYield).TryCast<GameObject>();
            GearItem DiyAddedYieldGearItem = DiyAddedYieldObj.GetComponent<GearItem>();
            int DiyAddedYieldObjUnit = 1;

            //GameObject DiyYieldObj;
            bool doAddNail = false;

            string DiyRWood = "GEAR_ReclaimedWoodB";
            var NoNailLst = new List<string>() { "Plank" }; // "Metal", "ChairCushion",
            
            //var lst1 = new List<string>() { "Table", "Chair", "Crate", "Palette","Shelf", "Stool", "Bench" };

            //MelonLogger.Msg(" ========== DIY ===");


            // ----------------------------------------------------------------------------------------------------
            // 
            for (int i = 0; i < DiyArrayLenght; i++) // doAddNail=True, if Yield objects contain GEAR_ReclaimedWoodB.
            {
                if (DiyBreakDownObj.GetComponent<BreakDown>().m_YieldObject[i].name == DiyRWood) { 
                    doAddNail = true;
                    if (DiyBreakDownObj.GetComponent<BreakDown>().m_YieldObjectUnits[i] > 6) DiyAddedYieldObjUnit = 2; // If DiyRwood > 6, Yield nail = 2
                }
            }
            if (doAddNail == false) return; // ==================== Return, No relaimed wood

            for (int i = 0; i < NoNailLst.Count; i++)
            {
                if (DiyBreakDownObjName.Contains(NoNailLst[i])) return; // ==================== Return, No nail list
            }
            // 
            // ----------------------------------------------------------------------------------------------------


            // If it does not return, continue this...
            // Increase the length of the array by 1. and add nails.

            GameObject[] tempArrayYo = new GameObject[DiyArrayLenght]; // Yield objects
            int[] tempArrayYoU = new int[DiyArrayLenght];
            DiyBreakDownObj.GetComponent<BreakDown>().m_YieldObject.CopyTo(tempArrayYo, 0);
            DiyBreakDownObj.GetComponent<BreakDown>().m_YieldObjectUnits.CopyTo(tempArrayYoU, 0);
            DiyBreakDownObj.GetComponent<BreakDown>().m_YieldObject = new GameObject[DiyArrayLenght + 1]; // Length + 1
            DiyBreakDownObj.GetComponent<BreakDown>().m_YieldObjectUnits = new int[DiyArrayLenght + 1]; // Length + 1

            for (int i=0; i<DiyArrayLenght; i++)
            {
                DiyBreakDownObj.GetComponent<BreakDown>().m_YieldObject[i] = tempArrayYo[i];
                DiyBreakDownObj.GetComponent<BreakDown>().m_YieldObjectUnits[i] = tempArrayYoU[i];
            }
            DiyBreakDownObj.GetComponent<BreakDown>().m_YieldObject[DiyArrayLenght] = DiyAddedYieldObj;
            DiyBreakDownObj.GetComponent<BreakDown>().m_YieldObjectUnits[DiyArrayLenght] = DiyAddedYieldObjUnit;


            //MelonLogger.Msg(" ========== ");
            
        }
    }



}
