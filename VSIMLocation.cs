﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APVacationSim
{
    public class VSIMLocation
    {
        public readonly string name;
        public readonly long ap_id;
        public readonly string item_name;
        public readonly string player_name;
        public readonly string in_game_id;

        public VSIMLocation(string name, long ap_id, string item_name, string player_name, string in_game_id)
        {
            this.name = name;
            this.ap_id = ap_id;
            this.item_name = item_name;
            this.player_name = player_name;
            this.in_game_id = in_game_id;
        }
    }

    public class RawVSIMLocation
    {
        public readonly string in_game_id;
        public readonly string name;

        public RawVSIMLocation(string in_game_id, string name)
        {
            this.in_game_id = in_game_id;
            this.name = name;
        }
    }
}
