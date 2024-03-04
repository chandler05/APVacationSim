using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APVacationSim
{
    public class VSIMLocation
    {
        public readonly long ap_id;
        public readonly string item_name;
        public readonly string player_name;
        public readonly int type;
        public readonly string in_game_id;

        public VSIMLocation(long ap_id, string item_name, string player_name, int type, string in_game_id)
        {
            this.ap_id = ap_id;
            this.item_name = item_name;
            this.player_name = player_name;
            this.type = type;
            this.in_game_id = in_game_id;
        }
    }
}
