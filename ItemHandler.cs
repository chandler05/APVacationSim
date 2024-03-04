using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APVacationSim
{
    public class ItemHandler
    {
        public static void HandleNewItem(ReceivedItemsHelper item)
        {
            if (item.PeekItemName() == "Camera")
            {
                //Debug.Log("Unlock 1");
                Main.EnableCamera();
            }
            if (item.PeekItemName() == "Vacation Beach Gate Unlock")
            {
                //Debug.Log("Unlock 2");
                Main.UnlockBeach();
            }
            if (item.PeekItemName() == "Vacation Forest Gate Unlock")
            {
                //Debug.Log("Unlock 3");
                Main.UnlockForest();
            }
            if (item.PeekItemName() == "Vacation Mountain Gate Unlock")
            {
                //Debug.Log("Unlock 4");
                Main.UnlockMountain();
            }
            if (item.PeekItemName() == "Memory (Vacation Beach)")
            {
                //Debug.Log("Unlock 5");
                Main.AddBeachMemory();
            }
            if (item.PeekItemName() == "Memory (Vacation Forest)")
            {
                //Debug.Log("Unlock 6");
                Main.AddForestMemory();
            }
            if (item.PeekItemName() == "Memory (Vacation Mountain)")
            {
                //Debug.Log("Unlock 7");
                Main.AddMountainMemory();
            }

            item.DequeueItem();
        }
    }
}
