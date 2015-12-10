﻿using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Animator))]
[RequireComponent (typeof(PlayerInput))]
[RequireComponent (typeof(PlayerMovement))]
public class AnimationWrapper : MonoBehaviour 
{
	private Animator animator;
	private PlayerInput input;
	private PlayerMovement movement;

	//input last values
	private float lastMoveV = 0.0f;
	private float lastMoveH = 0.0f;
	private float oldMoveV = 0.0f;
	private float oldMoveH = 0.0f;
	private float moveHTimer = 0.0f;
	private float moveVTimer = 0.0f;

	public float smoothTime = 0.5f;

	void Start () 
	{
		animator = GetComponent<Animator>();
		input = GetComponent<PlayerInput>();
		movement = GetComponent<PlayerMovement>();
	}
	
	void Update () 
	{
		//connect to PlayerInput
		InputData inputData = input.GetInputData();
		float moveV = inputData.motionV * (inputData.run + 1.0f);
		float moveH = inputData.motionH * (inputData.run + 1.0f);

		float smoothMoveV = Smooth(moveV, ref lastMoveV, ref oldMoveV, ref moveVTimer);
		float smoothMoveH = Smooth(moveH, ref lastMoveH, ref oldMoveH, ref moveHTimer);
		animator.SetFloat("moveV", smoothMoveV);
		animator.SetFloat("moveH", smoothMoveH);
		animator.SetBool("isMoving", (moveV != 0.0f || moveH != 0.0f));

		//connect to PlayerProperties
		PlayerProperties properties = movement.GetProperties();
		animator.SetBool("isJumping", (properties.jumpTimer > 0.0f));
		animator.SetBool("isGrounded", properties.isGrounded);
	}

	private float Smooth(float move, ref float lastMove, ref float oldMove, ref float moveTimer)
	{
		//setup smooth timer
		if (lastMove != move)
		{
			moveTimer = smoothTime;
			oldMove = lastMove;
		}
		else
		{
			moveTimer = Mathf.Max (0.0f, moveTimer - Time.deltaTime);
		}
		lastMove = move;

		//perform smoothing
		float smoothMove;
		if (smoothTime <= 0.0f)
		{
			smoothMove = move;
		}
		else
		{
			smoothMove = Mathf.Lerp(move, oldMove, moveTimer / smoothTime);
		}

		return smoothMove;
	}
}