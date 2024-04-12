using Common.Proto.Player;
using Tool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Emm();
    }

    async void Emm()
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
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
