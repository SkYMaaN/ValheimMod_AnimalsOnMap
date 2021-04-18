using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace ValheimMod_AnimalsOnMap
{
    [BepInPlugin("SkYMaN.ValheimMod_ShowAnimalsOnMap", "ShowAnimalsOnMap", "1.0.1")]
    [BepInProcess("valheim.exe")]
    public class Valheim_AnimalsOnMapClass : BaseUnityPlugin
    {
        private readonly Harmony harmony = new Harmony("SkYMaN.ValheimMod_ShowAnimalsOnMap");
        void Awake()
        {
            Debug.Log("Start mod loading - ValheimMod_ShowAnimalsOnMap");
            harmony.PatchAll();
            Debug.Log("Finish mod loading - ValheimMod_ShowAnimalsOnMap");
        }
        void OnDestroy()
        {
            harmony.UnpatchSelf();
        }
        static List<Minimap.PinData> savedPins = new List<Minimap.PinData>();
        [HarmonyPatch(typeof(Minimap), nameof(Minimap.OnMapMiddleClick))]
        class DeleteAnimalsOnMap_Patch
        {
            static void Prefix(ref Minimap ___m_instance, ref List<Minimap.PinData> ___m_pins)
            {
                if (savedPins.Count > 0)
                {
                    for (int i = 0; i < ___m_pins.Count; i++)
                    {
                        for (int j = 0; j < savedPins.Count; j++)
                        {
                            if (___m_pins[i].m_pos == savedPins[j].m_pos)
                            {
                                ___m_instance.RemovePin(___m_pins[i]);
                            }
                        }
                    }

                }              
            }
        }
        [HarmonyPatch(typeof(Minimap), nameof(Minimap.OnMapMiddleClick))]
        class AddAnimalsOnMap_Patch
        {
            static void Postfix(ref Minimap ___m_instance, ref List<Minimap.PinData> ___m_pins)
            {
                foreach (Character obj in Character.GetAllCharacters())
                {
                    if (obj.name == "Boar(Clone)" || obj.name == "Deer(Clone)")
                    {
                        Minimap.PinData pinData = new Minimap.PinData();
                        pinData.m_pos = obj.GetCenterPoint();
                        pinData.m_name = obj.GetHoverName();
                        pinData.m_save = false;
                        pinData.m_checked = false;
                        pinData.m_type = Minimap.PinType.Icon3;
                        ___m_instance.AddPin(obj.GetCenterPoint(), Minimap.PinType.Icon3, obj.GetHoverName(), false, false);
                        savedPins.Add(pinData);
                    }
                }
            }
        }
    }
}
