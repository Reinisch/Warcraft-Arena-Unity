using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using ElleFramework.Database;
using ElleRealTime.Shared.DBEntities.PlayersInfo;
using ElleRealTimeBaseDAO.Interfaces;

namespace ElleRealTime.Core.BO.World
{
    public class Players
    {
        public PlayerInfo[] GetPlayerInfo(PlayersInfoFilter filter)
        {
            return GetPlayerInfo(filter, null);
        }

        internal PlayerInfo[] GetPlayerInfo(PlayersInfoFilter filter, DbTransaction trans)
        {
            IPlayers dao = DAOFactory.Create<IPlayers>();

            return dao.GetPlayersInfo(filter, trans);
        }

        public void SavePlayerInfo(PlayerInfo playerInfo)
        {
            IPlayers dao = DAOFactory.Create<IPlayers>();
            var filter = new PlayersInfoFilter {AccountID = playerInfo.AccountID};

            //Check if we already have a record saved.
            PlayerInfo[] result = GetPlayerInfo(filter, null);

            if (result != null)
            {
                //If exists, update.
                if (result.Length > 0)
                {
                    dao.UpdatePlayerInfo(playerInfo, null);
                }
                else //Insert new record.
                {
                    dao.InsertPlayerInfo(playerInfo, null);
                }
            }
            else
            {
                dao.InsertPlayerInfo(playerInfo, null);
            }
        }
    }
}
