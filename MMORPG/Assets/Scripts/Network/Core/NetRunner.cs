using Common.Proto.Player;
using Common.Proto.Space;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThirdPersonCamera;
using Tool;
using UnityEngine;


public class NetRunner : MonoBehaviour, IController
{
    NetPlayer player;

    public IArchitecture GetArchitecture()
    {
        return GameApp.Interface;
    }

    private async void Start()
    {
        var request = new EnterGameRequest
        {
            CharacterId = 1,
        };
        SceneHelper.BeginSpinnerBox(new());
        NetClient.Session.Send(request);
        var response = await NetClient.Session.ReceiveAsync<EnterGameResponse>();
        SceneHelper.EndSpinnerBox();

        player = new NetPlayer(response.Character.Entity.EntityId,
            response.Character.Entity.Position.ToVector3(),
            Quaternion.Euler(response.Character.Entity.Direction.ToVector3()),
            true);
        this.SendCommand(new CharacterEnterCommand(player));

        Task.Run(OnEntityEnterResponse);
        Task.Run(OnEntitySyncResponse);
        Task.Run(SendEntitySyncRequest);
    }

    async void SendEntitySyncRequest()
    {
        while (true)
        {
            var request = new EntitySyncRequest
            {
                EntitySync = new()
                {
                    Entity = new()
                    {
                        EntityId = player.EntityId,
                        Position = player.Position.ToNetVector3(),
                        Direction = player.Rotation.eulerAngles.ToNetVector3()
                    }
                }
            };
            NetClient.Session.Send(request);
            await Task.Delay(100);
        }
    }

    async void OnEntityEnterResponse()
    {
        while (true)
        {
            var response = await NetClient.Session.ReceiveAsync<EntityEnterResponse>();
            foreach (var entity in response.EntityList)
            {
                var player = new NetPlayer(entity.EntityId,
                    entity.Position.ToVector3(),
                    Quaternion.Euler(entity.Direction.ToVector3()),
                    false);
                this.SendCommand(new CharacterEnterCommand(player));
            }
        }
    }

    async void OnEntitySyncResponse()
    {
        while (true)
        {
            var response = await NetClient.Session.ReceiveAsync<EntitySyncResponse>();
            var entity = response.EntitySync.Entity;
            this.SendCommand(
                new CharacterPositionChangeCommand(
                    entity.EntityId,
                    entity.Position.ToVector3(),
                    Quaternion.Euler(entity.Direction.ToVector3())
                )
            );
        }
    }

    async void OnEntityLeaveResponse()
    {
        while (true)
        {
            var response = await NetClient.Session.ReceiveAsync<EntityLeaveResponse>();
        }
    }
}
