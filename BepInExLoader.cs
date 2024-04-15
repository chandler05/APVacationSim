using BepInEx;
using UnhollowerRuntimeLib;
using HarmonyLib;
using UnityEngine;
using Object = UnityEngine.Object;
using OwlchemyVR.Teleportation;

namespace APVacationSim
{
    [BepInPlugin(GUID, MODNAME, VERSION)]
    public class BepInExLoader : BepInEx.IL2CPP.BasePlugin
    {
        public const string
            MODNAME = "APVacationSim",
            AUTHOR = "chandler",
            GUID = "com." + AUTHOR + "." + MODNAME,
            VERSION = "0.2.0";

        public static BepInEx.Logging.ManualLogSource log;

        public BepInExLoader()
        {
            log = Log;
        }

        public override void Load()
        {
            log.LogMessage("[Archipelago] Registering ArchipelagoComponent in Il2Cpp");

            try
            {
                // Register our custom Types in Il2Cpp
                ClassInjector.RegisterTypeInIl2Cpp<Main>();

                var go = new GameObject("ArchipelagoObject");
                go.AddComponent<Main>();
                Object.DontDestroyOnLoad(go);
            }
            catch
            {
                log.LogError("[Archipelago] FAILED to Register Il2Cpp Type: ArchipelagoComponent!");
            }

            try
            {
                var harmony = new Harmony("chandler.APVactionSim.il2cpp");

                // Awake
                var originalAwake = AccessTools.Method(typeof(SteamManager), "Awake");
                log.LogMessage("[Archipelago] Harmony - Original Method: " + originalAwake.DeclaringType.Name + "." + originalAwake.Name);
                var postAwake = AccessTools.Method(typeof(Main), "Awake");
                log.LogMessage("[Archipelago] Harmony - Postfix Method: " + postAwake.DeclaringType.Name + "." + postAwake.Name);
                harmony.Patch(originalAwake, prefix: new HarmonyMethod(postAwake));

                // Start
                var originalStart = AccessTools.Method(typeof(MenuBackpackManager), "Start");
                log.LogMessage("[Archipelago] Harmony - Original Method: " + originalStart.DeclaringType.Name + "." + originalStart.Name);
                var postStart = AccessTools.Method(typeof(Main), "Start");
                log.LogMessage("[Archipelago] Harmony - Postfix Method: " + postStart.DeclaringType.Name + "." + postStart.Name);
                harmony.Patch(originalStart, postfix: new HarmonyMethod(postStart));

                // Update
                var originalUpdate = AccessTools.Method(typeof(SteamManager), "Update");
                log.LogMessage("[Archipelago] Harmony - Original Method: " + originalUpdate.DeclaringType.Name + "." + originalUpdate.Name);
                var postUpdate = AccessTools.Method(typeof(Main), "Update");
                log.LogMessage("[Archipelago] Harmony - Postfix Method: " + postUpdate.DeclaringType.Name + "." + postUpdate.Name);
                harmony.Patch(originalUpdate, postfix: new HarmonyMethod(postUpdate));
                log.LogMessage("[Archipelago] Harmony - Runtime Patch's Applied");

                // OnGUI
                /*var originalOnGUI = AccessTools.Method(typeof(ExampleColorReceiver), "OnGUI");
                log.LogMessage("[Archipelago] Harmony - Original Method: " + originalOnGUI.DeclaringType.Name + "." + originalOnGUI.Name);
                var postOnGUI = AccessTools.Method(typeof(Main), "OnGUI");
                log.LogMessage("[Archipelago] Harmony - Postfix Method: " + postOnGUI.DeclaringType.Name + "." + postOnGUI.Name);
                harmony.Patch(originalOnGUI, postfix: new HarmonyMethod(postOnGUI));
                log.LogMessage("[Archipelago] Harmony - Runtime Patch's Applied");*/

                var ogGates = AccessTools.Method(typeof(ResortGateController), "OnEnable");
                log.LogMessage("[Archipelago] Harmony - Original Method: " + ogGates.DeclaringType.Name + "." + ogGates.Name);
                var updatedGates = AccessTools.Method(typeof(Main), "SetupGates");
                log.LogMessage("[Archipelago] Harmony - Prefix Method: " + updatedGates.DeclaringType.Name + "." + updatedGates.Name);
                harmony.Patch(ogGates, prefix: new HarmonyMethod(updatedGates));
                log.LogMessage("[Archipelago] Harmony - Runtime Patch's Applied");

                var ogGates2 = AccessTools.Method(typeof(ResortGateController), "TeleportConnection_OnHighlightBegan");
                log.LogMessage("[Archipelago] Harmony - Original Method: " + ogGates2.DeclaringType.Name + "." + ogGates2.Name);
                var updatedGates2 = AccessTools.Method(typeof(Main), "TeleportCheck");
                log.LogMessage("[Archipelago] Harmony - Prefix Method: " + updatedGates2.DeclaringType.Name + "." + updatedGates2.Name);
                harmony.Patch(ogGates2, prefix: new HarmonyMethod(updatedGates2));
                log.LogMessage("[Archipelago] Harmony - Runtime Patch's Applied");

                var TPZones = AccessTools.Method(typeof(TeleportZone), "Awake");
                log.LogMessage("[Archipelago] Harmony - Original Method: " + TPZones.DeclaringType.Name + "." + TPZones.Name);
                var newTPZones = AccessTools.Method(typeof(Main), "SetupGateTP");
                log.LogMessage("[Archipelago] Harmony - Prefix Method: " + newTPZones.DeclaringType.Name + "." + newTPZones.Name);
                harmony.Patch(TPZones, prefix: new HarmonyMethod(newTPZones));
                log.LogMessage("[Archipelago] Harmony - Runtime Patch's Applied");

                var ogGoal = AccessTools.Method(typeof(GoalManager), "IsGoalCompleted");
                log.LogMessage("[Archipelago] Harmony - Original Method: " + ogGoal.DeclaringType.Name + "." + ogGoal.Name);
                var updatedGoal = AccessTools.Method(typeof(Main), "GoalCompletion");
                log.LogMessage("[Archipelago] Harmony - Prefix Method: " + updatedGoal.DeclaringType.Name + "." + updatedGoal.Name);
                harmony.Patch(ogGoal, prefix: new HarmonyMethod(updatedGoal));
                log.LogMessage("[Archipelago] Harmony - Runtime Patch's Applied");

                var watchCounter = AccessTools.Method(typeof(VacationWatchController), "GetMemoriesCompleteInSceneGroup");
                log.LogMessage("[Archipelago] Harmony - Original Method: " + watchCounter.DeclaringType.Name + "." + watchCounter.Name);
                var watchCounterNew = AccessTools.Method(typeof(Main), "MemoryCount");
                log.LogMessage("[Archipelago] Harmony - Prefix Method: " + watchCounterNew.DeclaringType.Name + "." + watchCounterNew.Name);
                harmony.Patch(watchCounter, prefix: new HarmonyMethod(watchCounterNew));
                log.LogMessage("[Archipelago] Harmony - Runtime Patch's Applied");

                var globalWatchCounter = AccessTools.Method(typeof(VacationWatchController), "GetGlobalMemoriesComplete");
                log.LogMessage("[Archipelago] Harmony - Original Method: " + globalWatchCounter.DeclaringType.Name + "." + globalWatchCounter.Name);
                var globalWatchCounterNew = AccessTools.Method(typeof(Main), "GlobalMemoryCount");
                log.LogMessage("[Archipelago] Harmony - Prefix Method: " + globalWatchCounterNew.DeclaringType.Name + "." + globalWatchCounterNew.Name);
                harmony.Patch(globalWatchCounter, prefix: new HarmonyMethod(globalWatchCounterNew));
                log.LogMessage("[Archipelago] Harmony - Runtime Patch's Applied");

                var checkMemories = AccessTools.Method(typeof(VacationWatchManager), "HasGlobalMemories");
                log.LogMessage("[Archipelago] Harmony - Original Method: " + checkMemories.DeclaringType.Name + "." + checkMemories.Name);
                var checkMemoriesNew = AccessTools.Method(typeof(Main), "MemoryChecker");
                log.LogMessage("[Archipelago] Harmony - Prefix Method: " + checkMemoriesNew.DeclaringType.Name + "." + checkMemoriesNew.Name);
                harmony.Patch(checkMemories, prefix: new HarmonyMethod(checkMemoriesNew));
                log.LogMessage("[Archipelago] Harmony - Runtime Patch's Applied");

                var boardCounter = AccessTools.Method(typeof(ProductivityBoard.GoalPercentageDisplay), "Refresh");
                log.LogMessage("[Archipelago] Harmony - Original Method: " + boardCounter.DeclaringType.Name + "." + boardCounter.Name);
                var boardCounterNew = AccessTools.Method(typeof(Main), "BoardText");
                log.LogMessage("[Archipelago] Harmony - Prefix Method: " + boardCounterNew.DeclaringType.Name + "." + boardCounterNew.Name);
                harmony.Patch(boardCounter, prefix: new HarmonyMethod(boardCounterNew));
                log.LogMessage("[Archipelago] Harmony - Runtime Patch's Applied");

                var boardCounter2 = AccessTools.Method(typeof(ProductivityBoard), "OnEnable");
                log.LogMessage("[Archipelago] Harmony - Original Method: " + boardCounter2.DeclaringType.Name + "." + boardCounter2.Name);
                var boardCounterNew2 = AccessTools.Method(typeof(Main), "Refresh");
                log.LogMessage("[Archipelago] Harmony - Prefix Method: " + boardCounterNew2.DeclaringType.Name + "." + boardCounterNew2.Name);
                harmony.Patch(boardCounter2, postfix: new HarmonyMethod(boardCounterNew2));
                log.LogMessage("[Archipelago] Harmony - Runtime Patch's Applied");

                var ogTrophy = AccessTools.Method(typeof(MemoryTrophy), "CameraPreCull");
                log.LogMessage("[Archipelago] Harmony - Original Method: " + ogTrophy.DeclaringType.Name + "." + ogTrophy.Name);
                var updatedTrophy = AccessTools.Method(typeof(Main), "ChangeMemoryTrophy");
                log.LogMessage("[Archipelago] Harmony - Prefix Method: " + updatedTrophy.DeclaringType.Name + "." + updatedTrophy.Name);
                harmony.Patch(ogTrophy, postfix: new HarmonyMethod(updatedTrophy));
                log.LogMessage("[Archipelago] Harmony - Runtime Patch's Applied");

                // MemoryTrophy - GoalData - Name is Game ID
                // MemoryTrophy - GoalData - Friendly Name is in-game
                // MemoryTrophy - Grabbable - Seems to GIve Memory

                // ResortGateController - Controls front gates to each area

                // GoalManager - Deals with most of the Memory and Goal interactions

                // CURRENT IDEA - Turn off Memory Grants for Goals until triggered (WORKS)
                // Pull list of locations from Archipelago, store in list based on location
                // When you recieve a memory from the Archipelago server, find the first complete location from the list and mark it as memory giving
            }
            catch
            {
                log.LogError("[Archipelago] Harmony - FAILED to Apply Patch's!");
            }
        }
    }
}
