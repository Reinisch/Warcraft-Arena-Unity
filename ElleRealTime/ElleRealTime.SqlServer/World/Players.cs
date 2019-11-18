using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using ElleRealTime.Shared.DBEntities.PlayersInfo;
using ElleRealTimeBaseDAO;
using ElleRealTimeBaseDAO.Interfaces;

namespace ElleRealTime.SqlServer.World
{
    public class Players : ElleRealTimeDbDAO, IPlayers
    {
        public void InsertPlayerInfo(PlayerInfo playerInfo, DbTransaction trans)
        {
            ElleRealTimeBaseDAO.Base.World.Players.InsertPlayerInfo(this, playerInfo, trans);
        }

        public PlayerInfo[] GetPlayersInfo(PlayersInfoFilter filter, DbTransaction trans)
        {
            Hashtable prms = new Hashtable
            {
                { "@FirstRow", filter.FirstRow },
                { "@LastRow", filter.LastRow },
            };

            return ExecuteViewArray<PlayerInfo>("SELECT * " +
                                          "FROM ( " +
                                          "    SELECT MZ.*, " +
                                          "			 ROW_NUMBER() OVER (" + filter.OrderByCondition() + ") AS rn " +
                                          "    FROM ( " + ElleRealTimeBaseDAO.Base.World.Players.GetBaseQueryPlayersInfo(filter, prms) + " ) MZ " +
                                          ") NumMZ " +
                                          "WHERE rn BETWEEN @FirstRow AND @LastRow",
                prms, trans);
        }

        public void UpdatePlayerInfo(PlayerInfo playerInfo, DbTransaction trans)
        {
            ElleRealTimeBaseDAO.Base.World.Players.UpdatePlayerInfo(this, playerInfo, trans);
        }
    }
}
