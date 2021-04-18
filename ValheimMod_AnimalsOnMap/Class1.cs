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
        protected static ConfigEntry<bool> configEntry_ShowTamedAnimals;
        protected static ConfigEntry<bool> configEntry_ShowDeer;
        protected static ConfigEntry<bool> configEntry_ShowBoar;
        protected static ConfigEntry<bool> configEntry_ShowNeck;
        void Awake()
        {
            Debug.Log("Start mod loading - ValheimMod_ShowAnimalsOnMap");
            configEntry_ShowTamedAnimals = Config.Bind<bool>("General", "Show Tamed Animals", false, "Show Tamed Animals");
            configEntry_ShowDeer = Config.Bind<bool>("General", "Show Deer", false, "Show Deer");
            configEntry_ShowBoar = Config.Bind<bool>("General", "Show Boar", false, "Show Boar");
            configEntry_ShowNeck = Config.Bind<bool>("General", "Neck", false, "Show Neck");
            harmony.PatchAll();
            Debug.Log("Finish mod loading - ValheimMod_ShowAnimalsOnMap");
        }
        void OnDestroy()
        {
            harmony.UnpatchSelf();
        }
        static List<Minimap.PinData> savedPins = new List<Minimap.PinData>();
        static List<Minimap.PinData> inGamePins = new List<Minimap.PinData>();
        [HarmonyPatch(typeof(Player), nameof(Player.IsTeleporting))]
        class PlayerIsTeleporting_Patch
        {
            static void Prefix()
            {
                if (savedPins.Count > 0)
                {
                    for (int i = 0; i < inGamePins.Count; i++)
                    {
                        for (int j = 0; j < savedPins.Count; j++)
                        {
                            if (inGamePins[i].m_pos == savedPins[j].m_pos)
                            {
                                Minimap.instance.RemovePin(inGamePins[i]);
                            }
                        }
                    }
                }
            }
        }
        [HarmonyPatch(typeof(Minimap), "Awake")]
        class GetInGamePinsArray_Patch
        {
            static void Prefix(ref List<Minimap.PinData> ___m_pins)
            {
                inGamePins = ___m_pins; 
            }
        }
        [HarmonyPatch(typeof(Minimap), "SetMapMode")]
        class DeleteAnimalsOnMap_Patch
        {
            static void Prefix(ref Minimap ___m_instance, ref List<Minimap.PinData> ___m_pins)
            {
                if (___m_instance.m_largeRoot == true)
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
        }
        [HarmonyPatch(typeof(Minimap), "SetMapMode")]
        class AddAnimalsOnMap_Patch
        {
            static void Postfix(ref Minimap ___m_instance, ref List<Minimap.PinData> ___m_pins)
            {
                if (___m_instance.m_largeRoot == true)
                {
                    foreach (Character obj in Character.GetAllCharacters())
                    {
                        if (!obj.IsTamed())
                        {
                            if (obj.name == "Boar(Clone)" && configEntry_ShowBoar.Value == true)
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
                            else if(obj.name == "Deer(Clone)" && configEntry_ShowDeer.Value == true)
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
                            else if(obj.name == "Neck(Clone)" && configEntry_ShowNeck.Value == true)
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
                        if (obj.IsTamed() && configEntry_ShowTamedAnimals.Value)
                        {
                            if (obj.name == "Boar(Clone)")
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
    }
}
