using System;
using System.Linq;
using System.Threading.Tasks;
using ElleRealTime.Core.BO.World;
using ElleRealTime.Shared.DBEntities.PlayersInfo;
using ElleRealTimeStd.Shared.Test.Entities.StreamingHub.Player;
using ElleRealTimeStd.Shared.Test.Interfaces.StreamingHub;
using MagicOnion.Server.Hubs;
using UnityEngine;

namespace ElleRealTime.Tests.Services
{
    public class GamingHub : StreamingHubBase<IGamingHub, IGamingHubReceiver>, IGamingHub
    {
        private static Vector3 DefaultVector3 = new Vector3(11.13949f, 4.719501f, -116.671f);
        private static Quaternion DefaultQuaternion = new Quaternion(0, 0, 0, 0);

        // this class is instantiated per connected so fields are cache area of connection.
        IGroup room;
        Player self;
        IInMemoryStorage<Player> storage;

        public async Task<Player[]> JoinAsync(string roomName, int accountId)
        {
            var bo = new Players();
            var result = bo.GetPlayerInfo(new PlayersInfoFilter {AccountID = accountId});
            if (result != null && result.Length == 1)
            {
                PlayerInfo playerInfo = result[0];
                self = new Player
                {
                    Name = accountId.ToString(), //ToDO: Temp
                    Position = new Vector3(playerInfo.PosX, playerInfo.PosY, playerInfo.PosZ),
                    Rotation = new Quaternion(playerInfo.RotX, playerInfo.RotY, playerInfo.RotZ, 0)
                };
            }
            else
            {
                self = new Player
                {
                    Name = accountId.ToString(), //ToDO: Temp
                    Position = DefaultVector3,
                    Rotation = DefaultQuaternion
                };
            }

            (room, storage) = await Group.AddAsync(roomName, self);

            Broadcast(room).OnJoin(self);

            Console.WriteLine($"{accountId} joined the room \"{roomName}\"");

            return storage.AllValues.ToArray();
        }

        public async Task LeaveAsync()
        {
            await room.RemoveAsync(this.Context);
            Console.WriteLine($"{self.Name} leaves the room.");
            Broadcast(room).OnLeave(self);
        }

        public async Task MoveAsync(Vector3 position, Quaternion rotation)
        {
            self.Position = position;
            self.Rotation = rotation;

            Console.WriteLine($"{self.Name} is moving to: ");
            Console.WriteLine($"Vector3: x={position.x}, y={position.y}, z={position.z} ");
            Console.WriteLine($"Quaternion: x={rotation.x}, y={rotation.y}, z={rotation.z}, w={rotation.w}");

            Broadcast(room).OnMove(self);
        }

        public async Task SendAnimStateAsync(int state)
        {
            Broadcast(room).OnAnimStateChange(self.Name, state);
        }

        public async Task SavePlayerAsync()
        {
            Program.Logger.Info($"Player {self.Name} requested to save his info.");
            var bo = new Players();

            PlayerInfo playerInfo = new PlayerInfo
            {
                PosX = self.Position.x,
                PosY = self.Position.y,
                PosZ = self.Position.z,

                RotX = self.Rotation.x,
                RotY = self.Rotation.y,
                RotZ = self.Rotation.z,

                AccountID = int.Parse(self.Name)
            };

            bo.SavePlayerInfo(playerInfo);

            BroadcastToSelf(room).OnPlayerInfoSaved();
        }
    }
}
