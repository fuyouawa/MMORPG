using Common.Proto.Player;
using System.Collections;
using System.Collections.Generic;
using ThirdPersonCamera;
using Tool;
using UnityEngine;


public class NetRunner : MonoBehaviour
{
    private async void Start()
    {
        var request = new EnterGameRequest
        {
            CharacterId = 1,
        };
        NetClient.Session.Send(request);
        var response = await NetClient.Session.ReceiveAsync<EnterGameResponse>();

        SceneHelper.Invoke(() =>
        {
            var player = Spawn(
                Resources.Load<GameObject>("Prefabs/Character/Player/DogPBR"),
                response.Character.Entity.Position.ToVector3(),
                Quaternion.Euler(response.Character.Entity.Direction.ToVector3()));

            var cameraController = Camera.main.GetComponent<CameraController>();
            cameraController.InitFromTarget(player.transform);
        });
    }


    public GameObject Spawn(GameObject obj, Vector3 position, Quaternion rotation)
    {
        var inst = Instantiate(obj, position, rotation);
        return inst;
    }
}
