using Common.Proto.Player;
using Tool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using Cinemachine;

public class Test : MonoBehaviour
{
    public GameObject _heroCamera;

    // Start is called before the first frame update
    async void Start()
    {
        var request = new EnterGameRequest
        {
            CharacterId = 1,
        };
        NetClient.Session.Send(request);
        var response = await NetClient.Session.ReceiveAsync<EnterGameResponse>();

        SceneManager.Instance.Invoke(() =>
        {
            var prefabs = Resources.Load<GameObject>("Prefabs/DogPBR");
            var hero = Instantiate(prefabs);
            hero.transform.position = response.Character.Entity.Position.ToVector3();
            var direction = response.Character.Entity.Direction.ToVector3();
            hero.transform.rotation = Quaternion.Euler(direction.x, direction.y, direction.z);

            var camera = _heroCamera.GetComponent<CinemachineFreeLook>();
            camera.Follow = hero.transform;
            camera.LookAt = hero.transform;
            
            var script = hero.AddComponent<PlayerMove>();
            script.Player = hero;
            script.MoveSpeed = 5;
            script.PlayerCamera = camera;
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
