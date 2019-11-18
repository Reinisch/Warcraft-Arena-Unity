using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using ElleRealTime.Shared.DBEntities.PlayersInfo;

namespace ElleRealTimeBaseDAO.Base.World
{
    public static class Players
    {
        public static void UpdatePlayerInfo(ElleRealTimeDbDAO dao, PlayerInfo playerInfo, DbTransaction trans)
        {
            Hashtable prms = new Hashtable
            {
                { $"@{nameof(PlayerInfo.AccountID)}", playerInfo.AccountID },
                { $"@{nameof(PlayerInfo.PosX)}", playerInfo.PosX },
                { $"@{nameof(PlayerInfo.PosY)}", playerInfo.PosY },
                { $"@{nameof(PlayerInfo.PosZ)}", playerInfo.PosZ },
                { $"@{nameof(PlayerInfo.RotX)}", playerInfo.RotX },
                { $"@{nameof(PlayerInfo.RotY)}", playerInfo.RotY },
                { $"@{nameof(PlayerInfo.RotZ)}", playerInfo.RotZ },
            };

            dao.ExecuteNonQuery("UPDATE players_info " +
                                $"SET PosX = @{nameof(PlayerInfo.PosX)}, " +
                                $"    PosY = @{nameof(PlayerInfo.PosY)}, " +
                                $"    PosZ = @{nameof(PlayerInfo.PosZ)}, " +
                                $"    RotX = @{nameof(PlayerInfo.RotX)}, " +
                                $"    RotY = @{nameof(PlayerInfo.RotY)}, " +
                                $"    RotZ = @{nameof(PlayerInfo.RotZ)} " +
                                $"WHERE AccountID = @{nameof(PlayerInfo.AccountID)}", prms, trans);
        }

        public static void InsertPlayerInfo(ElleRealTimeDbDAO dao, PlayerInfo playerInfo, DbTransaction trans)
        {
            Hashtable prms = new Hashtable
            {
                { $"@{nameof(PlayerInfo.AccountID)}", playerInfo.AccountID },
                { $"@{nameof(PlayerInfo.PosX)}", playerInfo.PosX },
                { $"@{nameof(PlayerInfo.PosY)}", playerInfo.PosY },
                { $"@{nameof(PlayerInfo.PosZ)}", playerInfo.PosZ },
                { $"@{nameof(PlayerInfo.RotX)}", playerInfo.RotX },
                { $"@{nameof(PlayerInfo.RotY)}", playerInfo.RotY },
                { $"@{nameof(PlayerInfo.RotZ)}", playerInfo.RotZ },
            };

            dao.ExecuteNonQuery("INSERT INTO players_info( AccountID, PosX, PosY, PosZ, RotX, RotY, RotZ ) VALUES ( " +
                            $" @{nameof(PlayerInfo.AccountID)}, " +
                            $" @{nameof(PlayerInfo.PosX)}, " +
                            $" @{nameof(PlayerInfo.PosY)}, " +
                            $" @{nameof(PlayerInfo.PosZ)}, " +
                            $" @{nameof(PlayerInfo.RotX)}, " +
                            $" @{nameof(PlayerInfo.RotY)}, " +
                            $" @{nameof(PlayerInfo.RotZ)} " +
                            "); ", prms, trans);
        }

        public static string GetBaseQueryPlayersInfo(PlayersInfoFilter filter, Hashtable prms)
        {
            return "SELECT PI.* " +
                   "FROM players_info PI " +
                   filter.WhereCondition(prms);
        }
    }
}
