using System;
using System.Linq;
using System.Threading.Tasks;
using ElleRealTimeStd.Shared.Test.Entities.StreamingHub.Player;
using ElleRealTimeStd.Shared.Test.Interfaces.StreamingHub;
using MagicOnion.Server.Hubs;
using UnityEngine;

namespace ElleRealTime.Tests.Services
{
    public class GamingHub : StreamingHubBase<IGamingHub, IGamingHubReceiver>, IGamingHub
    {
        // this class is instantiated per connected so fields are cache area of connection.
        IGroup room;
        Player self;
        IInMemoryStorage<Player> storage;

        public async Task<Player[]> JoinAsync(string roomName, string userName, Vector3 position, Quaternion rotation)
        {
            Vector3 tmpPos = new Vector3(11.13949f, 4.719501f, -116.671f);
            self = new Player
            {
                Name = userName,
                Position = tmpPos,
                Rotation = rotation
            };

            (room, storage) = await Group.AddAsync(roomName, self);

            Broadcast(room).OnJoin(self);

            Console.WriteLine($"{userName} joined the room \"{roomName}\"");

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

    }
}
