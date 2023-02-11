using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class ResourceGenerator : NetworkBehaviour
{
   [SerializeField] private Health health = null;
   [SerializeField] private int resourcesPerInterval = 10;
   [SerializeField] private float interval = 2f;
   [SerializeField] private GameObject particle = default;

   private float timer;
   private RTSPlayer player;

   #region Server

   public override void OnStartServer()
   {
      timer = interval;
      player = connectionToClient.identity.GetComponent<RTSPlayer>();

      health.ServerOnDie += HandleServerOnDie;
      GameOverHandle.ServerOnGameOver += HandleSeverGameOver;
   }

   public override void OnStopServer()
   {
      health.ServerOnDie -= HandleServerOnDie;
      GameOverHandle.ServerOnGameOver -= HandleSeverGameOver;
   }

   [Server]
   private void PlayParticle()
   {
      var particleInstance = Instantiate(particle, transform);
      NetworkServer.Spawn(particleInstance);
      RpcPlayParticle();
   }
   
   [ClientRpc]
   private void RpcPlayParticle()
   {
      if (isServer)
      {
         return;
      }
      Instantiate(particle, transform);
   }
   

   [ServerCallback]
   private void Update()
   {
      timer -= Time.deltaTime;

      if (timer <= 0)
      {
         player.SetResources(player.GetResources() + resourcesPerInterval);
         PlayParticle();
         
         timer += interval;
      }
   }

   private void HandleServerOnDie()
   {
      NetworkServer.Destroy(gameObject);
   }

   private void HandleSeverGameOver()
   {
      enabled = false;
   }

   #endregion
   
}
