using System;
using System.Collections.Generic;
using BepInEx;
using UnhollowerBaseLib;
using UnhollowerRuntimeLib;
using HarmonyLib;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Input = BepInEx.IL2CPP.UnityEngine.Input;
using OwlchemyVR.Teleportation;
using System.IO;
using System.Linq;
using TMPro;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Archipelago.MultiClient.Net.Helpers;
using Newtonsoft.Json;
using Archipelago.MultiClient.Net.Packets;

namespace APVacationSim
{
    public class Main : MonoBehaviour
    {
        static GameObject beachGate = null;
        static GameObject forestGate = null;
        static GameObject mountainGate = null;

        static GameObject beachTP = null;
        static GameObject forestTP = null;
        static GameObject mountainTP = null;

        static bool beachUnlocked = false;
        static bool forestUnlocked = false;
        static bool mountainUnlocked = false;

        static bool cameraUnlocked = false;

        static int currentCheck = -1;
        static bool canSetup = false;

        static int diveAreaMemories = 0;
        static int hikingTrailMemories = 0;
        static int overlookMemories = 0;
        static int finalMemories = 0;

        static GameObject diveAreaScanner;
        static GameObject hikingTrailScanner;
        static GameObject overlookScanner;
        static GameObject finalScanner;

        static int completedBeachMemoriesCount = 0;
        static int completedForestMemoriesCount = 0;
        static int completedMountainMemoriesCount = 0;

        static GameObject watchText;

        static ArchipelagoSession APSession;

        static GameObject globalGoalManager;

        static Dictionary<string, VSIMLocation> locations;

        static LocationManager locationManager;

        static bool goalSent = false;

        public Main(IntPtr ptr) : base(ptr)
        {
            BepInExLoader.log.LogMessage("[Archipelago] Entered Constructor");
        }

        // Harmony Patch's must be static!
        [HarmonyPrefix]
        public static void Awake()
        {
            BepInExLoader.log.LogMessage("[Archipelago] I'm Awake!");

            using (StreamReader file = File.OpenText(@"BepInEx\plugins\APVacationSim\APLogin.json"))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                JObject o2 = (JObject)JToken.ReadFrom(reader);

                APSession = ArchipelagoSessionFactory.CreateSession(o2.GetValue("port").ToString());

                APSession.Items.ItemReceived += ItemHandler.HandleNewItem;

                ResetBeachMemories();
                ResetForestMemories();
                ResetMountainMemories();

                Connect(o2.GetValue("port").ToString(), o2.GetValue("slot_name").ToString(), o2.GetValue("password").ToString());
            }
        }

        [HarmonyPostfix]
        public static void Start()
        {
            BepInExLoader.log.LogMessage("[Archipelago] I'm Starting Up...");  
        }

        [HarmonyPostfix]
        public static void Update()
        {
            //BepInExLoader.log.LogMessage("[Archipelago] I'm Updating, disable this message after testing.");

            if (beachTP != null && !beachUnlocked)
            {
                beachTP.GetComponent<TeleportZone>().enabled = false;
            } else if (beachTP != null && beachUnlocked)
            {
                beachTP.GetComponent<TeleportZone>().enabled = true;
            }

            if (forestTP != null && !forestUnlocked)
            {
                forestTP.GetComponent<TeleportZone>().enabled = false;
            }
            else if (forestTP != null && forestUnlocked)
            {
                forestTP.GetComponent<TeleportZone>().enabled = true;
            }

            if (mountainTP != null && !mountainUnlocked)
            {
                mountainTP.GetComponent<TeleportZone>().enabled = false;
            }
            else if (mountainTP != null && mountainUnlocked)
            {
                mountainTP.GetComponent<TeleportZone>().enabled = true;
            }

            if (diveAreaScanner == null)
            {
                GameObject search = GameObject.Find("WristbandScanner_DiveSiteAccess");
                if (search != null) {
                    diveAreaScanner = search;
                }
            } else
            {
                if (diveAreaScanner.GetComponent<WristbandScannerController>().scannerUnlock.requiredLocalMemories != diveAreaMemories)
                {
                    diveAreaScanner.GetComponent<WristbandScannerController>().scannerUnlock.requiredLocalMemories = diveAreaMemories;
                    diveAreaScanner.GetComponent<WristbandScannerController>().memoryCountLabel.text = diveAreaMemories.ToString();
                    diveAreaScanner.GetComponent<WristbandScannerController>().failVOClips = null;
                    diveAreaScanner.GetComponent<WristbandScannerController>().failClipVOIntro = null;
                }
            }

            if (hikingTrailScanner == null)
            {
                GameObject search = GameObject.Find("Wristband_HikeAccess");
                if (search != null)
                {
                    hikingTrailScanner = search;
                }
            }
            else
            {
                if (hikingTrailScanner.GetComponent<WristbandScannerController>().scannerUnlock.requiredLocalMemories != hikingTrailMemories)
                {
                    hikingTrailScanner.GetComponent<WristbandScannerController>().scannerUnlock.requiredLocalMemories = hikingTrailMemories;
                    hikingTrailScanner.GetComponent<WristbandScannerController>().memoryCountLabel.text = hikingTrailMemories.ToString();
                    hikingTrailScanner.GetComponent<WristbandScannerController>().failVOClips = null;
                    hikingTrailScanner.GetComponent<WristbandScannerController>().failClipVOIntro = null;
                }
            }

            if (overlookScanner == null)
            {
                GameObject search = GameObject.Find("WristbandScanner_OverlookAccess");
                if (search != null)
                {
                    overlookScanner = search;
                }
            }
            else
            {
                if (overlookScanner.GetComponent<WristbandScannerController>().scannerUnlock.requiredLocalMemories != overlookMemories)
                {
                    overlookScanner.GetComponent<WristbandScannerController>().scannerUnlock.requiredLocalMemories = overlookMemories;
                    overlookScanner.GetComponent<WristbandScannerController>().memoryCountLabel.text = overlookMemories.ToString();
                    overlookScanner.GetComponent<WristbandScannerController>().failVOClips = null;
                    overlookScanner.GetComponent<WristbandScannerController>().failClipVOIntro = null;
                }
            }

            if (finalScanner == null)
            {
                GameObject search = GameObject.Find("WristbandScanner_Finale/WristbandScannerContainer/WristbandScanner_Finale");
                if (search != null)
                {
                    finalScanner = search;
                }
            }
            else
            {
                if (finalScanner.GetComponent<WristbandScannerController>().scannerUnlock.requiredLocalMemories != finalMemories)
                {
                    finalScanner.GetComponent<WristbandScannerController>().scannerUnlock.requiredLocalMemories = finalMemories;
                    finalScanner.GetComponent<WristbandScannerController>().memoryCountLabel.text = finalMemories.ToString();
                    finalScanner.GetComponent<WristbandScannerController>().failVOClips = null;
                    finalScanner.GetComponent<WristbandScannerController>().failClipVOIntro = null;
                }
            }

            if (!goalSent && GameObject.Find("Coconut_PartyManager") != null && GameObject.Find("Coconut_PartyManager").GetComponent<PartyManager>().isPartyActive)
            {
                SendGoal();
                goalSent = true;
            }

            if (watchText == null)
            {
                try
                {
                    GameObject search = GameObject.Find("Pulse/TopFraction");
                    if (search != null)
                    {
                        watchText = search;
                    }
                }
                catch { }
            }
            else
            {
                GameObject beach = GameObject.Find("Pulse/BeachOn");
                GameObject forest = GameObject.Find("Pulse/ForestOn");
                GameObject mountain = GameObject.Find("Pulse/MountainOn");
                if (beach != null && forest != null && mountain != null)
                {
                    watchText.GetComponent<TextMeshPro>().text = (completedBeachMemoriesCount + completedForestMemoriesCount + completedMountainMemoriesCount).ToString();
                } else if (beach != null)
                {
                    watchText.GetComponent<TextMeshPro>().text = completedBeachMemoriesCount.ToString();
                } else if (forest != null)
                {
                    watchText.GetComponent<TextMeshPro>().text = completedForestMemoriesCount.ToString();
                } else if (mountain != null)
                {
                    watchText.GetComponent<TextMeshPro>().text = completedMountainMemoriesCount.ToString();
                }
            }

            if (globalGoalManager == null || locations == null)
            {
                GameObject search = GameObject.Find("Coconut_GoalManager");
                if (search != null && locations != null)
                {
                    globalGoalManager = search;
                    locationManager = new LocationManager(APSession);
                    locationManager.RegisterLocations(locations, globalGoalManager.GetComponent<GoalManager>());
                }
            } else
            {
                locationManager.CheckLocations();
            }
        }

        public static void EnableCamera()
        {
            cameraUnlocked = true;
        }

        public static void AddBeachMemory()
        {
            completedBeachMemoriesCount++;
        }

        public static void AddForestMemory()
        {
            completedForestMemoriesCount++;
        }

        public static void AddMountainMemory()
        {
            completedMountainMemoriesCount++;
        }

        public static void ResetBeachMemories()
        {
            completedBeachMemoriesCount = 0;
        }

        public static void ResetForestMemories()
        {
            completedForestMemoriesCount = 0;
        }

        public static void ResetMountainMemories()
        {
            completedMountainMemoriesCount = 0;
        }

        public static void UnlockBeach()
        {
            beachUnlocked = true;
        }
           
        public static void UnlockForest()
        {
            forestUnlocked = true;
        }
           
        public static void UnlockMountain()
        {
            mountainUnlocked = true;
        }

        private static void Connect(string server, string user, string pass)
        {
            if (pass == "")
            {
                pass = null;
            }

            LoginResult result;

            try
            {
                result = APSession.TryConnectAndLogin("Vacation Simulator", user, Archipelago.MultiClient.Net.Enums.ItemsHandlingFlags.AllItems, new Version(0, 4, 4), password: pass);
                // handle TryConnectAndLogin attempt here and save the returned object to `result`
            }
            catch (Exception e)
            {
                result = new LoginFailure(e.GetBaseException().Message);
            }

            if (!result.Successful)
            {
                LoginFailure failure = (LoginFailure)result;
                string errorMessage = $"Failed to Connect to {server} as {user}:";
                foreach (string error in failure.Errors)
                {
                    errorMessage += $"\n    {error}";
                }
                foreach (ConnectionRefusedError error in failure.ErrorCodes)
                {
                    errorMessage += $"\n    {error}";
                }

                return; // Did not connect, show the user the contents of `errorMessage`
            }

            // Successfully connected, `ArchipelagoSession` (assume statically defined as `session` from now on) can now be used to interact with the server and the returned `LoginSuccessful` contains some useful information about the initial connection (e.g. a copy of the slot data as `loginSuccess.SlotData`)
            var loginSuccess = (LoginSuccessful)result;

            Debug.Log(loginSuccess.ToString());

            PullFromSlotData(loginSuccess);
        }

        private static void PullFromSlotData(LoginSuccessful login)
        {
            locations = ((JObject) login.SlotData["locations"]).ToObject<Dictionary<string, VSIMLocation>>();
            var settings = ((JObject)login.SlotData["settings"]).ToObject<Dictionary<string, int>>();
            if (settings != null)
            {
                diveAreaMemories = settings["beachGate"];
                hikingTrailMemories = settings["forestGate"];
                overlookMemories = settings["mountainGate"];
                finalMemories = settings["finalGate"];
            }
        }

        public static void SendGoal()
        {
            Debug.Log("Sending Goal!");
            var statusUpdatePacket = new StatusUpdatePacket();
            statusUpdatePacket.Status = ArchipelagoClientState.ClientGoal;
            APSession.Socket.SendPacket(statusUpdatePacket);
        }

        [HarmonyPrefix]
        public static bool BoardText(ProductivityBoard.GoalPercentageDisplay __instance)
        {
            if (__instance.label.name == "BeachPercent")
            {
                __instance.label.text = completedBeachMemoriesCount.ToString() + "/22";
            }
            if (__instance.label.name == "ForestPercent")
            {
                __instance.label.text = completedForestMemoriesCount.ToString() + "/23";
            }
            if (__instance.label.name == "MountainPercent")
            {
                __instance.label.text = completedMountainMemoriesCount.ToString() + "/24";
            }
            if (__instance.label.name == "TotalPercent")
            {
                Debug.Log((((completedBeachMemoriesCount + completedForestMemoriesCount + completedMountainMemoriesCount) / 69f) * 100f).ToString());
                __instance.label.text = Mathf.Ceil(((completedBeachMemoriesCount + completedForestMemoriesCount + completedMountainMemoriesCount) / 69f) * 100f).ToString() + "%";
            }
            return false;
        }

        [HarmonyPostfix]
        public static void Refresh(ProductivityBoard __instance)
        {
            foreach (ProductivityBoard.GoalPercentageDisplay display in __instance.displays)
            {
                display.Refresh();
            }
        }

        [HarmonyPrefix]
        public static void TeleportCheck(ResortGateController __instance)
        {
            if (__instance.gameObject.name == "ResortGate_Beach")
            {
                currentCheck = 0;
                beachGate = __instance.gameObject;
            }
            if (__instance.gameObject.name == "ResortGate_Forest")
            {
                currentCheck = 2;
                forestGate = __instance.gameObject;
            }
            if (__instance.gameObject.name == "ResortGate_Mountain")
            {
                currentCheck = 4;
                mountainGate = __instance.gameObject;
            }
        }


        [HarmonyPrefix]
        public static bool SetupGates(ResortGateController __instance)
        {
            if (__instance.gameObject.name == "ResortGate_Beach")
            {
                if (!canSetup)
                {
                    beachGate = __instance.gameObject;
                    __instance.gameObject.SetActive(false);
                    return false;
                }
            }
            if (__instance.gameObject.name == "ResortGate_Forest")
            {
                if (!canSetup)
                {
                    forestGate = __instance.gameObject;
                    __instance.gameObject.SetActive(false);
                    return false;
                }
            }
            if (__instance.gameObject.name == "ResortGate_Mountain")
            {
                if (!canSetup)
                {
                    mountainGate = __instance.gameObject;
                    __instance.gameObject.SetActive(false);
                    return false;
                }
            }
            return true;
        }

        [HarmonyPrefix]
        public static bool SetupGateTP(TeleportZone __instance)
        {
            if (__instance.gameObject.name == "VisitorCenter_Hub_BeachPath")
            {
                beachTP = __instance.gameObject;
            }
            if (__instance.gameObject.name == "VisitorCenter_Hub_ForestPath")
            {
                forestTP = __instance.gameObject;
            }
            if (__instance.gameObject.name == "VisitorCenter_Hub_MountainPath")
            {
                mountainTP = __instance.gameObject;
            }
            return true;
        }

        [HarmonyPrefix]
        public static bool MemoryCount(VacationWatchController __instance, ref string __0, ref int __result)
        {
            if (__0 == "Beach")
            {
                __result = completedBeachMemoriesCount;
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        public static bool GoalCompletion(GoalManager __instance, ref GoalData __0, ref bool __result)
        {
            if (__0.cachedName == "FinishResortIntro")
            {
                __result = true;
                if (beachGate != null)
                {
                    canSetup = true;
                    currentCheck = 0;
                    beachGate.SetActive(true);
                }
                return false;
            }

            if (__0.cachedName == "UnlockBackpack")
            {
                __result = true;
                return false;
            }

            if (__0.cachedName == "UnlockCamera")
            {
                __result = cameraUnlocked;
                return false;
            }

            if (__0.cachedName == "ArriveAtBeach")
            {
                __result = true;
                return false;
            }

            if (__0.cachedName == "FinishBeachIntro")
            {
                __result = true;
                return false;
            }

            if (__0.cachedName == "CompleteCameraTutorial")
            {
                __result = true;
                return false;
            }

            if (__0.cachedName == "ArriveAtForest")
            {
                __result = true;
                return false;
            }

            if (__0.cachedName == "CompleteForestIntro")
            {
                __result = true;
                return false;
            }

            if (__0.cachedName == "ArriveAtMountain")
            {
                __result = true;
                return false;
            }

            if (__0.cachedName == "FinishMountainIntro")
            {
                __result = true;
                return false;
            }

            if (__0.cachedName == "UnlockAllBiomesAccess")
            {
                if (canSetup || currentCheck > -1)
                {
                    currentCheck++;
                    if (currentCheck == 1 || currentCheck == 2)
                    {
                        __result = beachUnlocked;
                        if (currentCheck == 2)
                        {
                            forestGate.SetActive(true);
                        }
                    }
                    if (currentCheck == 3 || currentCheck == 4)
                    {
                        __result = forestUnlocked;
                        if (currentCheck == 4)
                        {
                            mountainGate.SetActive(true);
                        }
                    }
                    if (currentCheck == 5 || currentCheck == 6)
                    {
                        __result = mountainUnlocked;
                        if (currentCheck == 6)
                        {
                            canSetup = false;
                            currentCheck = -1;
                        }
                    }
                    if (!canSetup)
                    {
                        currentCheck = -1;
                    }
                    return false;
                }
                __result = true;
                return false;
            }
            return true;
        }

        private static Sprite LoadItemImage(string path)
        {
            byte[] bytes = File.ReadAllBytes(path);
            var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            ImageConversion.LoadImage(tex, bytes);
            tex.filterMode = FilterMode.Point;

            var size = new Vector2Int(260, 260);
            var rect = new Rect(0, 0, size.x, size.y);
            var pivot = new Vector2(0.5f, 0.5f);
            int pixelsPerUnit = 100;
            var border = Vector4.zero;
            return Sprite.Create(tex, rect, pivot, pixelsPerUnit, 0, SpriteMeshType.Tight, border);
        }

        private static Sprite apLogo = LoadItemImage(Environment.CurrentDirectory + "/BepInEx/plugins/APVacationSim/resources/icon.png");

        [HarmonyPostfix]
        public static void ChangeMemoryTrophy(MemoryTrophy __instance)
        {
            if (apLogo == null)
            {
                apLogo = LoadItemImage(Environment.CurrentDirectory + "/BepInEx/plugins/APVacationSim/resources/icon.png");
            }
            if (__instance.rendererForSprite.sprite != apLogo) {
                __instance.rendererForSprite.sprite = apLogo;
            }
        }
    }
}
