using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MelonLoader;
using HarmonyLib;
using UnityEngine;

namespace DiyBySantana
{

    internal class DiyChangeColor : MelonMod
    {
        public static void ChangeWood(string diyGroup) 
        {
            ////// MelonLogger.Msg(" ++++++++++ DIY ChangeWood Start ++++++++++++++++++++++++++++++ ");

            string[] gearArray;
            byte colR;
            byte colG;
            byte colB;
            byte colA;
            
            string[] diyBoardArray ={
                "GEAR_DiyABoard2x1",
                "GEAR_DiyABoard4x2",
                "GEAR_DiyABoard6x1"
            };

            string[] diyFurnitureArray ={
                "GEAR_DiyCofTableA",
                "GEAR_DiyCratesB",
                "GEAR_DiyDrTableB",
                "GEAR_DiyGunRackA",
                "GEAR_DiyWallShelfA",
                "GEAR_DiyWchairA",
                "GEAR_DiyWchairB",
                "GEAR_DiyWshelfA",
                "GEAR_DiyWStoolA"
            };

            string[] diyTableWareArray = {
                "GEAR_DiyTwCupA",
                "GEAR_DiyTwCuttingBoardA",
                "GEAR_DiyTwDishBL",
                "GEAR_DiyTwDishBO",
                "GEAR_DiyTwDishBowlA",
                "GEAR_DiyTwDishBowlB",
                "GEAR_DiyTwDishPlateA",
                "GEAR_DiyTwFork",
                "GEAR_DiyTwSpatulaSpoon",
                "GEAR_DiyTwSpoon"
            };

            switch (diyGroup) 
            {
                case "Board":
                    gearArray = diyBoardArray;
                    colR = (byte)Settings.options.colorBR;
                    colG = (byte)Settings.options.colorBG;
                    colB = (byte)Settings.options.colorBB;
                    colA = 255;
                    break;
                case "Furniture":
                    gearArray = diyFurnitureArray;
                    colR = (byte)Settings.options.colorFR;
                    colG = (byte)Settings.options.colorFG;
                    colB = (byte)Settings.options.colorFB;
                    colA = 255; 
                    break;
                case "Tableware":
                    gearArray = diyTableWareArray;
                    colR = (byte)Settings.options.colorTR;
                    colG = (byte)Settings.options.colorTG;
                    colB = (byte)Settings.options.colorTB;
                    colA = 255;
                    break;
                default:
                    gearArray = new string[]{ "error" };
                    colR = 255;
                    colG = 255;
                    colB = 0;
                    colA = 255;
                    MelonLogger.Msg(" ++++++++++ DIY diyGroup error");
                    break;

            }



            for (int i = 0; i < gearArray.Length; i++)
            {
                if (Resources.Load(gearArray[i]) == null) 
                {
                    MelonLogger.Msg(" ++++++++++ DIY gearArray load null , Array " + i);
                }
                else
                {
                    if (Resources.Load(gearArray[i]).TryCast<GameObject>().transform.childCount==0) 
                    {
                        if (Resources.Load(gearArray[i]).TryCast<GameObject>().GetComponent<MeshRenderer>() == null) 
                        {
                            MelonLogger.Msg(" ++++++++++ DIY gearArray MeshRenderer null , Array " + i);
                        }
                        else
                        {
                            Resources.Load(gearArray[i]).TryCast<GameObject>().GetComponent<MeshRenderer>().material.color = new Color32(colR, colG, colB, colA);
                        }
                    }
                    else
                    {
                        for (int j = 0; j < Resources.Load(gearArray[i]).TryCast<GameObject>().transform.childCount; j++)
                        { 
                            if (Resources.Load(gearArray[i]).TryCast<GameObject>().transform.GetChild(j).GetComponent<MeshRenderer>() == null) 
                            {
                                MelonLogger.Msg(" ++++++++++ DIY gearArray MeshRenderer null , Array " + i +", Child "+j);
                            }
                            else 
                            { 
                                Resources.Load(gearArray[i]).TryCast<GameObject>().transform.GetChild(j).GetComponent<MeshRenderer>().material.color = new Color32(colR, colG, colB, colA); 
                            }
                        }
                    }
                    
                }
            }

                
            
        }

    }
}
