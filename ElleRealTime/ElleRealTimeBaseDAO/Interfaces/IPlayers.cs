using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using ElleRealTime.Shared.DBEntities.PlayersInfo;

namespace ElleRealTimeBaseDAO.Interfaces
{
    public interface IPlayers : ITransactions
    {
        void UpdatePlayerInfo(PlayerInfo playerInfo, DbTransaction trans);
        void InsertPlayerInfo(PlayerInfo playerInfo, DbTransaction trans);
        PlayerInfo[] GetPlayersInfo(PlayersInfoFilter filter, DbTransaction trans);
    }
}
