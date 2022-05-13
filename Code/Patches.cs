using System;
using MelonLoader;
using HarmonyLib;
using UnityEngine;
using System.Collections.Generic; // List
using System.Linq; // Contains
using UnityEngine.SceneManagement;

namespace DiyBySantana
{

    public class ChangeLayerOfGear:MelonMod
    {
        public static GameObject ChangeLayerGO; /////////// Game object

        /////////// Layer 0==Default, 17==Gear, 18==Container, 19==InteractiveProp .
        // 00. Default, 01. TransparentFX, 02. Ignore Raycast, 03. 3, 04. Water, 05. UI, 
        // 06. 6, 07. 7, 08. Ground, 09. TerrainObject, 10. TriggerNPC, 11. Buildings, 
        // 12. InteractivePropNoCollideGear, 13. ParticleKiller, 14. Player, 15. NoCollidePlayer, 
        // 16. NPC, 17. Gear, 18. Container, 19. InteractiveProp, 20. Corpse, 21. Trigger, 
        // 22. SoundEmitter, 23. Weapon, 24. InteractivePropNoCollidePlayer, 
        // 25. TriggerIgnoreRaycast, 26. CharacterControllerCollideOnly, 27. NPCBones, 
        // 28. GroundNoNavmesh, 29. InspectGear, 30. TriggerCollideCapsule, 31. TriggerReverb

        public static int layerNotInteraction = 28;
        public static int layerGear = 17;
        public static int changedLayerNum = layerGear;
        public static int changedLayerChild0Num = 0; 

        static string[] targetGearNameArray = { 
            "GEAR_DiyABoard2x1",
            "GEAR_DiyABoard6x1",
            "GEAR_DiyABoard4x2" 
        };

        /*
         * by. better bases by Xpazeman
         * 
        internal static List<GameObject> GetRootObjects()
        {
            List<GameObject> rootObj = new List<GameObject>();

            for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
            {
                Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);

                GameObject[] sceneObj = scene.GetRootGameObjects();

                foreach (GameObject obj in sceneObj)
                {
                    rootObj.Add(obj);
                }
            }

            return rootObj;
        }
        */

        // ------------------------------------------------------------------------------------------------------------
        // Change Diy boards layer, Used by Implementations_OnSceneWasInitialized and Settings_OnConfirm
        //------------------------------------------------------------------------------------------------------------
        public static void DiyChangeLayerNum()
        {
            //GetRootObjects();
            GameObject findTargetGO = new GameObject() ;

            // ----- Find 
            for (int i=0; i< targetGearNameArray.Length; i++){ 
                findTargetGO = GameObject.Find(targetGearNameArray[i]);
                if (findTargetGO != null){
                    break; 
                }
            }
            if (findTargetGO == null){  
                //MelonLogger.Msg(ConsoleColor.DarkYellow,"DIY : findTargetGO is not exist at this scene.");
                return;
            }

            Transform gearsTrf = findTargetGO.transform.parent;
            
            if (gearsTrf == null){
                MelonLogger.Msg(" ---------- DIY gearsTrf null");
                return;
            }

            int gearChildCount = gearsTrf.childCount;
            //MelonLogger.Msg(" ---------- DIY child count "+gearChildCount);

            for (int i = 0; i < gearChildCount; i++)
            {
                for (int targetGearNameArrayNum = 0; targetGearNameArrayNum < targetGearNameArray.Length; targetGearNameArrayNum++) {
                    if (gearsTrf.GetChild(i).name == targetGearNameArray[targetGearNameArrayNum])
                    {
                        //MelonLogger.Msg("---------- DIY - " + gearsTrf.GetChild(i).name);

                        if (Settings.options.diyBoardsBoolVal == true)
                        {   changedLayerNum = layerGear;        }
                        else
                        {   changedLayerNum = layerNotInteraction;  }

                        gearsTrf.GetChild(i).gameObject.layer = changedLayerNum;

                        int childcount = gearsTrf.GetChild(i).gameObject.GetComponentInChildren<Transform>().childCount;
                        if (childcount > 0)
                        {
                            for (int j = 0; j < childcount; j++)
                            {
                                gearsTrf.GetChild(i).gameObject.transform.GetChild(j).gameObject.layer = changedLayerChild0Num;
                            }
                        }
                    }
                }
            }
        }

        // ------------------------------------------------------------------------------------------------------------
        // Drop
        //------------------------------------------------------------------------------------------------------------

        [HarmonyPatch(typeof(GearItem), "Drop")]
        internal class ChangeLayerOfGearDrop
        {
            private static void Postfix(GearItem __instance)
            {

                /////// MelonLogger.Msg(" ================ GearItem Drop = " + __instance.gameObject.name);

                if (targetGearNameArray.Contains(__instance.gameObject.name))
                {
                    /////// MelonLogger.Msg(" -------------------- Contains == True "+ __instance.gameObject.name);
                }
                else
                {
                    /////// MelonLogger.Msg(" -------------------- Contains == False, Return " + __instance.gameObject.name);
                    return; // Return
                }

                
                if (Settings.options.diyBoardsBoolVal == true)
                {
                    changedLayerNum = layerGear;
                }
                else
                {
                    changedLayerNum = layerNotInteraction;
                }

                //MelonLogger.Msg(" ================ Layer = " + changedLayerNum);
                __instance.gameObject.layer = changedLayerNum;

                int childcount = __instance.gameObject.GetComponentInChildren<Transform>().childCount;
                if (childcount == 0) return;

                for (int i = 0; i < childcount; i++)
                {
                    __instance.gameObject.transform.GetChild(i).gameObject.layer = changedLayerChild0Num;
                    //MelonLogger.Msg(" -------------------- Child num " + __instance.gameObject.GetComponentInChildren<Transform>().childCount);
                }

            }
        }

        // ------------------------------------------------------------------------------------------------------------
        // Inspect
        // ------------------------------------------------------------------------------------------------------------

        // ProcessInspectablePickupItem
        // InteractiveObjectsProcessInteraction
        // EnterInspectGearMode - error
        // ExitInspectGearMode --- Use it!

        [HarmonyPatch(typeof(PlayerManager), "ExitInspectGearMode")]
        internal class ChangeLayerOfGearInspect
        {

            private static void Postfix(PlayerManager __instance)
            {
                /////// MelonLogger.Msg(" ================ ExitInspectGearMode Postfix = ");

                if (__instance.m_Gear == null)
                {
                    /////// MelonLogger.Msg(" ~~~~~~~~~~~~~~~~~~~~ ExitInspectGearMode Postfix = m_Gear NULL, Return");
                    return; // Return
                }

                if (targetGearNameArray.Contains(__instance.m_Gear.gameObject.name))
                {
                    /////// MelonLogger.Msg(" ~~~~~~~~~~~~~~~~~~~~ Contains == True " + __instance.m_Gear.gameObject.name);
                }
                else
                {
                    /////// MelonLogger.Msg(" ~~~~~~~~~~~~~~~~~~~~ Contains == False, Return " + __instance.m_Gear.gameObject.name);
                    return; // Return
                }

                /////// MelonLogger.Msg(" ~~~~~~~~~~~~~~~~~~~~ ExitInspectGearMode Postfix = " + __instance.m_Gear.gameObject.name);

                // ExitInspectGearMode prefix/postfix .

                if (Settings.options.diyBoardsBoolVal == true)
                {
                    changedLayerNum = layerGear;
                }
                else
                {
                    changedLayerNum = layerNotInteraction;
                }

                //MelonLogger.Msg(" ================ Layer = " + changedLayerNum);
                __instance.m_Gear.gameObject.layer = changedLayerNum;

                
                int childcount = __instance.m_Gear.gameObject.GetComponentInChildren<Transform>().childCount;
                if (childcount == 0) return;
                /////// MelonLogger.Msg(" -------------------- ExitInspectGearMode Posftix = childcount = " + childcount);
                for (int i = 0; i < childcount; i++)
                {
                    __instance.m_Gear.gameObject.transform.GetChild(i).gameObject.layer = changedLayerChild0Num;
                }

                __instance.m_Gear = null; //========== if it is not null, ExitMeshPlacement error occurs.

                /////// MelonLogger.Msg(" ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ ");

            }

        }

        // ------------------------------------------------------------------------------------------------------------
        // Placement
        // ------------------------------------------------------------------------------------------------------------

        [HarmonyPatch(typeof(PlayerManager), "ExitMeshPlacement")]
        internal class ChangeLayerOfGearPlacement
        {

            // Prefix ------------------------------------------------------------------------------------------------------------

            private static void Prefix(PlayerManager __instance)
            {
                /////// MelonLogger.Msg(" ================ PlayerManager ExitMeshPlacement =11Prefix=  ");
                ChangeLayerGO = null;

                if (__instance.m_ObjectToPlace == null) //m_ObjectToPlaceGearItem
                {
                    /////// MelonLogger.Msg(" -------------------- PlayerManager ExitMeshPlacement Prefix m_ObjectToPlace =NULL=, Return  ");
                    return; // Return
                }

                if (targetGearNameArray.Contains(__instance.m_ObjectToPlace.name))
                {
                    /////// MelonLogger.Msg(" -------------------- Contains == True " + __instance.m_ObjectToPlace.name);
                }
                else
                {
                    /////// MelonLogger.Msg(" -------------------- Contains == False, Return " + __instance.m_ObjectToPlace.name);
                    return; // Return
                }

                /////// MelonLogger.Msg(" -------------------- PlayerManager ExitMeshPlacement Prefix m_ObjectToPlace =NOT NULL= " + __instance.m_ObjectToPlace.name);

                ChangeLayerGO = __instance.m_ObjectToPlace.TryCast<GameObject>();

            }


            // Postfix ------------------------------------------------------------------------------------------------------------

            private static void Postfix(PlayerManager __instance)
            {
                /////// MelonLogger.Msg(" ================ PlayerManager ExitMeshPlacement =22Postfix=  ");

                if (ChangeLayerGO == null) 
                {
                    /////// MelonLogger.Msg(" -------------------- PlayerManager ExitMeshPlacement Postfix ChangeLayerGO =NULL=, Return  ");
                    return; // Return
                }

                /////// MelonLogger.Msg(" -------------------- PlayerManager ExitMeshPlacement Postfix m_ObjectToPlace ======== " + ChangeLayerGO.name);

                if (Settings.options.diyBoardsBoolVal == true)
                {
                    changedLayerNum = layerGear;
                }
                else
                {
                    changedLayerNum = layerNotInteraction;
                }

                //MelonLogger.Msg(" ================ Layer = " + changedLayerNum);
                ChangeLayerGO.layer = changedLayerNum;

                int childcount = ChangeLayerGO.GetComponentInChildren<Transform>().childCount;
                if (childcount == 0) return;

                for (int i = 0; i < childcount; i++)
                {
                    ChangeLayerGO.transform.GetChild(i).gameObject.layer = changedLayerChild0Num;
                }
                  
            }

        }
    }


}
