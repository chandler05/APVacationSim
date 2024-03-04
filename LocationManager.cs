using Archipelago.MultiClient.Net;
using OwlchemyVR;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace APVacationSim
{
    public class LocationManager
    {
        public Dictionary<GoalData, bool> goalDatas = new Dictionary<GoalData, bool>();
        public Dictionary<string, VSIMLocation> allLocations;
        public GoalManager manager;
        public readonly ArchipelagoSession session;
        public LocationManager(ArchipelagoSession session) 
        {
            this.session = session;
        }

        public void RegisterLocations(Dictionary<string, VSIMLocation> locations, GoalManager goalManager)
        {
            foreach (GoalData data in goalManager.goals)
            {
                foreach (VSIMLocation location in locations.Values)
                {
                    if (location.in_game_id == data.name)
                    {
                        goalDatas.Add(data, false);
                        data.friendlyName = location.item_name + " for " + location.player_name;
                        data.memoryBackgroundColor = Color.white;
                    }
                }
            }
            allLocations = locations;
            manager = goalManager;
        }

        public async void CheckLocations()
        {
            Dictionary<GoalData, bool> successful = new Dictionary<GoalData, bool>();
            foreach(KeyValuePair<GoalData, bool> location in goalDatas)
            {
                if (!location.Value)
                {
                    if (SceneManager.GetActiveScene().name != "TravelMuseum" && manager != null && GameObject.Find("Coconut_SaveManager").GetComponent<SaveStateManager>().saveStateIndex != -1 && manager.IsGoalCompleted(location.Key))
                    {
                        //Debug.Log("HELP " + location.Key.name);
                        foreach (KeyValuePair<string, VSIMLocation> apLocation in allLocations)
                        {
                            if (apLocation.Value.in_game_id == location.Key.name)
                            {
                                //Debug.Log("Location Completed - " + apLocation.Key);
                                await Task.Run(() => session.Locations.CompleteLocationChecks(apLocation.Value.ap_id));
                                successful.Add(location.Key, true);
                                break;
                            }
                        }
                    } else
                    {
                        successful.Add(location.Key, location.Value);
                    }
                } else
                {
                    successful.Add(location.Key, location.Value);
                }
            }
            goalDatas = successful;
        }
    }
}
