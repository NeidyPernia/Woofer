﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.ComponentSystems;
using EntityComponentSystem.Events;
using GameBase;
using GameInterfaces.Input;
using GameInterfaces.Input.GamePad;
using WooferGame.Input;
using WooferGame.Systems.Physics;

namespace WooferGame.Systems.Movement
{
    [ComponentSystem("player_controller")]
    class PlayerMovement : ComponentSystem
    {
        public PlayerMovement()
        {
            Watching = new string[] { Component.IdentifierOf<PlayerMovementComponent>() };
            Listening = new string[] { Event.IdentifierOf<CollisionEvent>() };
            InputProcessing = true;
            TickProcessing = true;
        }

        public override void Input()
        {
            IInputMap inputMap = Woofer.Controller.InputManager.ActiveInputMap;

            foreach(PlayerMovementComponent pmc in WatchedComponents)
            {
                Physical rb = pmc.Owner.Components.Get<Physical>();

                ButtonState jumpButton = inputMap.Jump;

                if (jumpButton.IsPressed()) pmc.Jump.RegisterPressed();
                else pmc.Jump.RegisterUnpressed();

                if (pmc.OnGround)
                {
                    if(jumpButton.IsPressed() && pmc.Jump.Execute())
                    {
                        rb.Velocity.Y = pmc.JumpSpeed;
                    }
                }

                double xMovement = inputMap.Movement.X * pmc.CurrentSpeed;
                double xMovementCap = pmc.CurrentMaxSpeed;

                if(xMovement > 0)
                {
                    if(rb.Velocity.X <= xMovementCap)
                    {
                        rb.Velocity.X = Math.Min(rb.Velocity.X + xMovement, xMovementCap);
                    }
                } else if(xMovement < 0)
                {
                    if (rb.Velocity.X >= -xMovementCap)
                    {
                        rb.Velocity.X = Math.Max(rb.Velocity.X + xMovement, -xMovementCap);
                    }
                }
            }
        }

        public override void EventFired(object sender, Event re)
        {
            if(re is CollisionEvent)
            {
                CollisionEvent e = re as CollisionEvent;

                if(e.Victim.Components.Has<PlayerMovementComponent>() && e.Normal.Y > 0)
                {
                    PlayerMovementComponent pmc = e.Victim.Components.Get<PlayerMovementComponent>();
                    pmc.OnGround = true;
                }
            }
        }

        public override void Tick()
        {
            foreach(PlayerMovementComponent pmc in WatchedComponents)
            {
                pmc.OnGround = false;
                pmc.Jump.Tick();
            }
        }
    }
}
