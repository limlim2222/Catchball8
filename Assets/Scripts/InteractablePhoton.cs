//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: This object will get hover events and can be attached to the hands
//
//=============================================================================

using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;


namespace Valve.VR.InteractionSystem
{
    public class InteractablePhoton : Interactable
    {
        private PhotonView photonView; 
        void Start()
        {
            photonView = GetComponent<PhotonView>();
        }

        protected override void OnHandHoverBegin(Hand hand)
        {
            photonView.RequestOwnership();
            base.OnHandHoverBegin(hand);
        }

        protected override void OnAttachedToHand(Hand hand)
        {
            photonView.RequestOwnership();
            base.OnAttachedToHand(hand);
        }

    }
}
