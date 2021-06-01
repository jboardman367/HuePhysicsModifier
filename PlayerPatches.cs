using HarmonyLib;
using Fabric;
using System;
using System.Runtime;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityModManagerNet;
using InControl;
using System.Reflection;


namespace HuePhysicsModifier
{
	[HarmonyPatch(typeof(PlayerNew), "FixedUpdate")]
	public static class PlayerNew_FixedUpdate_Patch
	{
		//private methods
		static readonly MethodInfo m_Method = AccessTools.Method(typeof(PlayerNew), "lineCastHighestHit");
		static readonly FastInvokeHandler lineCastHighestHitDelegate = MethodInvoker.GetHandler(m_Method);

		static readonly MethodInfo m_Method1 = AccessTools.Method(typeof(PlayerNew), "DisableAnimatorBools");
		static readonly FastInvokeHandler DisableAnimatorBoolsDelegate = MethodInvoker.GetHandler(m_Method1);




		//prefix to replace the real one
		[HarmonyPrefix]
		static bool ReplaceingPrefix(
			PlayerNew __instance, MethodBase __originalMethod, ref Rigidbody2D ___rigidbody2D, ref float ___gravityForce, ref bool ___isOnStairs, ref bool ___isGrounded, ref float ___axisJump,
			ref float ___axisVertical, ref float ___jumpThroughAxisY, ref RaycastHit2D ___highestHit, ref bool ___playerJumpLaunchReset, ref Ladder ___ladder, ref bool ___jumpPressedInAir,
			ref float ___jumpPressedInAirTimer, ref float ___timeFromJumpPressedInAirToJump, ref float ___circleColliderHeight, ref bool ___justLanded, ref float ___axisHorizontal,
			ref float ___speed, ref float ___floorAngle, ref bool ___dropClimbReset, ref bool ___climbReset, ref bool ___jumpReset, ref bool ___reachedTopOfLadder, ref float ___ladderOrientTimer,
			ref float ___climbSpeed, ref bool ___steepSlopeLeft, ref bool ___steepSlopeRight, ref Pushable ___lastHitPushable, ref float ___circleColliderWidth, ref float ___axisAction,
			ref float ___pushTimer, ref bool ___pushLockInTrigger, ref bool ___push, ref bool ___pull, ref float ___pushSpeed, ref PlayerNew.PlayerState ___lastPlayerState,
			ref float ___isNotGroundedTimer, ref float ___timeAfterFallPlayerCanJump, ref float ___lastAxisJump, ref bool ___forceJump, ref float ___jumpForce, ref Vector2 ___platformLastPosition,
			ref Pushable ___lastStandPushable, ref float ___pushableLastPositionX, ref Color ___feetColour
		)
		{
			if (!Main.enabled)
			{
				return true;
			}

			try
			{
				Traverse trvBase = Traverse.Create(__instance);
				GameObject base_gameObject = (GameObject)trvBase.Property("gameObject").GetValue();
				Transform base_transform = (Transform)trvBase.Property("transform").GetValue();


				if (__instance.playerState == PlayerNew.PlayerState.isDead)
				{
					return false;
				}
				if (__instance.playerState == PlayerNew.PlayerState.isSleeping)
				{
					return false;
				}
				float num = 0f;
				float num2 = ___rigidbody2D.velocity.y;
				float num3 = ___rigidbody2D.position.x;
				float y = ___rigidbody2D.position.y;
				___gravityForce = 0.4f;
				Vector2 vector = new Vector2(__instance.circleCollider.bounds.center.x, __instance.circleCollider.bounds.center.y + 0.5f);
				Vector2 vector2 = new Vector2(__instance.circleCollider.bounds.center.x, __instance.circleCollider.bounds.max.y + 0.5f);
				Debug.DrawLine(vector, vector2, Color.magenta);
				RaycastHit2D hit = default(RaycastHit2D);
				RaycastHit2D[] array = Physics2D.LinecastAll(vector, vector2, __instance.headHitLayerMask);
				foreach (RaycastHit2D raycastHit2D in array)
				{
					JumpThroughPlatformNew component = raycastHit2D.collider.GetComponent<JumpThroughPlatformNew>();
					if (!raycastHit2D.collider.isTrigger && !component)
					{
						hit = raycastHit2D;
						break;
					}
				}
				__instance.animator.SetFloat("VelocityX", Mathf.Abs(num));
				__instance.animator.SetFloat("VelocityY", num2);
				if (__instance.jumpThroughTrigger)
				{
					__instance.jumpThroughTrigger = false;
				}
				___isOnStairs = false;
				Transform component2 = __instance.ragdoll.GetComponent<Transform>();
				if (__instance.playerState != PlayerNew.PlayerState.isFalling && ___isGrounded && ___axisJump > 0f && __instance.jumpThrough && ___axisVertical < ___jumpThroughAxisY)
				{
					__instance.jumpThrough.PassThrough(true);
					EventManager.Instance.PostEvent("Anim/Jump", base_gameObject);
					__instance.jumpThroughTrigger = true;
					Main.dbljRelease = false;
				}
				___highestHit = (RaycastHit2D)lineCastHighestHitDelegate(__instance);
				if (___highestHit && num2 <= 0f)
				{
					___highestHit.collider.gameObject.SendMessage("PlayerLandedOn", SendMessageOptions.DontRequireReceiver);
				}
				if (___highestHit.collider && ___highestHit.collider.tag == "Floating")
				{
					num2 = 0f;
				}
				else if (___highestHit.collider)
				{
					BouncyBlock component3 = ___highestHit.collider.GetComponent<BouncyBlock>();
					JumpThroughPlatformNew component4 = ___highestHit.collider.GetComponent<JumpThroughPlatformNew>();
					Rigidbody2D component5 = ___highestHit.collider.GetComponent<Rigidbody2D>();
					Pushable component6 = ___highestHit.collider.GetComponent<Pushable>();
					bool flag = ___highestHit.collider.tag == "Stairs";
					MoveBetweenPoints component7 = ___highestHit.collider.GetComponent<MoveBetweenPoints>();
					if (___playerJumpLaunchReset && !component3 && (!component6 || (component6 && !component6.isFalling) || ___highestHit.collider.tag == "FloatingObj") && ((__instance.playerState != PlayerNew.PlayerState.isClimbing && (num2 <= 0f || flag || component7 || (component5 && component5.velocity.y > 0.1f))) || (___ladder && ___ladder.ladderTopCollider.bounds.center.y > base_transform.position.y && ___axisVertical < 0f)))
					{
						___isGrounded = true;
						if (!___jumpPressedInAir || ___jumpPressedInAirTimer <= ___timeFromJumpPressedInAirToJump)
						{
						}
						___jumpPressedInAir = false;
						if (___highestHit.transform.tag != "NoPlayerSnap")
						{
							y = ___highestHit.point.y + ___circleColliderHeight / 2f;
							if (component5)
							{
								num2 = component5.velocity.y;
							}
							else
							{
								num2 = 0f;
							}
						}
					}
				}
				else
				{
					___isGrounded = false;
				}
				if (___highestHit && ___highestHit.collider)
				{
					ConveyerBelt component8 = ___highestHit.collider.GetComponent<ConveyerBelt>();
					if (component8 && ___isGrounded)
					{
						__instance.environmentalVelocity = Vector2.right * component8.speed / 3.05f;
					}
					else
					{
						__instance.environmentalVelocity = Vector2.zero;
					}
				}
				if (__instance.isEnabled && !__instance.isPaused)
				{
					___justLanded = false;
					if ((__instance.playerState == PlayerNew.PlayerState.isFalling & ___isGrounded) || (__instance.playerState == PlayerNew.PlayerState.isJumping & ___isGrounded))
					{
						EventManager.Instance.PostEvent("Anim/Land", base_gameObject);
						___justLanded = true;
						__instance.animator.SetBool("Jumping", false);
						__instance.animator.SetBool("Idle", true);
					}
					if (___isGrounded)
					{
						num = Main.wind.x/(1f+2f*Main.settings.groundFrictScale) + Mathf.Lerp(___rigidbody2D.velocity.x - Main.wind.x / (1f + 6f * Main.settings.groundFrictScale), ___axisHorizontal * ___speed * 1.2f * Mathf.Pow(0.7f, Main.settings.airFrictScale - 1), Time.deltaTime * 15f * Main.settings.groundFrictScale);
						if (___highestHit.transform.tag == "NoPlayerSnap" && ___axisHorizontal == 0f)
						{
							num = 0f;
						}
					}
					else
					{
						if (Main.settings.airFrictScale > 1)
						{
							num = Main.wind.x + Mathf.Lerp(___rigidbody2D.velocity.x - Main.wind.x, ___axisHorizontal * ___speed * 1.2f * Mathf.Pow(0.5f, Main.settings.airFrictScale - 1), Time.deltaTime * 10f);
						}
						else
						{
							num = Main.wind.x + Mathf.Lerp(___rigidbody2D.velocity.x - Main.wind.x, ___axisHorizontal * ___speed * 1.2f, Time.deltaTime * 10f * Main.settings.airFrictScale);
						}
					}
					if (___floorAngle > 20f)
					{
						___isOnStairs = true;
					}
					if (Mathf.Abs(___axisVertical) < 0.5f)
					{
						___dropClimbReset = true;
					}
					if ((!___climbReset && __instance.playerState != PlayerNew.PlayerState.isClimbing) || (!___climbReset && num2 < 0f))
					{
						if (Mathf.Abs(___axisVertical) < 0.5f && ___isGrounded)
						{
							___climbReset = true;
							___dropClimbReset = true;
						}
						if (__instance.playerState == PlayerNew.PlayerState.isFalling && ___axisVertical > 0.5f && num2 <= 0f && !hit)
						{
							___climbReset = true;
							___dropClimbReset = false;
						}
					}
					if (___ladder)
					{
						if (__instance.playerState == PlayerNew.PlayerState.isClimbing && ___axisJump > 0f && ___jumpReset && !ColourWheelTrigger.instance.maskActive)
						{
							if (___axisVertical >= -0.5f)
							{
								if (!hit)
								{
									__instance.forceJump = true;
									Main.dbljRelease = false;
								}
							}
							else
							{
								__instance.playerState = PlayerNew.PlayerState.isFalling;
								Main.dbljRelease = false;
							}
						}
						if (___ladder.isAboveLadder && ___axisVertical < -0.5f)
						{
							___ladder.LadderTopTrigger(true);
						}
						if (!___ladder.isAboveLadder && ___climbReset && Mathf.Abs(___axisVertical) > 0.5f && __instance.playerState != PlayerNew.PlayerState.isClimbing)
						{
							if (___axisVertical <= 0f || ___ladder.ladderTopCollider.transform.position.y >= base_transform.position.y)
							{
								if (!__instance.jumpThrough || !__instance.jumpThrough.colouredObjectBelow || __instance.jumpThrough.colouredObjectBelow.isHidden)
								{
									if (!___ladder || ___ladder.ladderTopCollider.transform.position.y >= base_transform.position.y || !___ladder.jumpThroughPlatform || !___ladder.jumpThroughPlatform.colouredObjectBelow || ___ladder.jumpThroughPlatform.colouredObjectBelow.isHidden)
									{
										if (__instance.jumpThrough)
										{
											__instance.jumpThrough.PassThrough(true);
										}
										__instance.playerState = PlayerNew.PlayerState.isClimbing;
										___isGrounded = false;
										___climbReset = false;
									}
								}
							}
						}
						if (___ladder.isAboveLadder && !__instance.jumpThrough && ___axisVertical > 0f)
						{
							___reachedTopOfLadder = true;
						}
						else
						{
							___reachedTopOfLadder = false;
						}
						if (base_transform.position.y > ___ladder.ladderTopCollider.bounds.min.y && ___axisVertical > 0f)
						{
							___reachedTopOfLadder = true;
						}
					}
					if (__instance.playerState == PlayerNew.PlayerState.isClimbing)
					{
						___gravityForce = 0f;
					}
					if (((!___ladder && __instance.playerState == PlayerNew.PlayerState.isClimbing) || (___ladder && ___ladder.isAboveLadder && __instance.jumpThrough)) && __instance.playerState != PlayerNew.PlayerState.isPushing && __instance.playerState != PlayerNew.PlayerState.isPushIdle && __instance.playerState != PlayerNew.PlayerState.isPushLockIn)
					{
						if (___isGrounded)
						{
							__instance.playerState = PlayerNew.PlayerState.isRunning;
						}
						else if (!___highestHit)
						{
							__instance.playerState = PlayerNew.PlayerState.isFalling;
						}
					}
					if (!___ladder && __instance.jumpThrough)
					{
						__instance.jumpThrough.PassThrough(false);
					}
					if (___ladder)
					{
						Collider2D component9 = ___ladder.GetComponent<Collider2D>();
						Collider2D ladderTopCollider = ___ladder.ladderTopCollider;
						if (__instance.playerState == PlayerNew.PlayerState.isClimbing && ___highestHit && ((___axisVertical > 0f && ladderTopCollider.bounds.center.y < base_transform.position.y) || (___axisVertical < 0f && ladderTopCollider.bounds.center.y > base_transform.position.y)))
						{
							JumpThroughPlatformNew component10 = ___highestHit.collider.GetComponent<JumpThroughPlatformNew>();
							DisableAnimatorBoolsDelegate(__instance);
							__instance.animator.SetBool("Idle", true);
							__instance.playerState = PlayerNew.PlayerState.isIdle;
							if (component10)
							{
								component10.PassThrough(false);
							}
						}
					}
					if (__instance.playerState == PlayerNew.PlayerState.isClimbing && ___ladder)
					{
						if (___ladderOrientTimer < 1f)
						{
							___ladderOrientTimer += Time.deltaTime;
						}
						else
						{
							___ladderOrientTimer = 1f;
						}
						num = 0f;
						num3 = Mathf.Lerp(___rigidbody2D.position.x, ___ladder.transform.position.x, ___ladderOrientTimer);
						num2 = ___axisVertical * ___climbSpeed;
						if (num2 > 0f && hit)
						{
							num2 = 0f;
						}
						if (___reachedTopOfLadder && !___ladder.dropOffTop)
						{
							num2 = 0f;
						}
						AnimatorStateInfo currentAnimatorStateInfo = __instance.animator.GetCurrentAnimatorStateInfo(0);
						DisableAnimatorBoolsDelegate(__instance);
						__instance.animator.SetBool("Climbing", true);
					}
					else
					{
						___ladderOrientTimer = 0f;
					}
					if (__instance.playerState != PlayerNew.PlayerState.isFalling || hit)
					{
					}
					if (__instance.playerState != PlayerNew.PlayerState.isClimbing && __instance.playerState != PlayerNew.PlayerState.isPushLockIn)
					{
						if (___axisHorizontal < 0f)
						{
							__instance.puppet2D.flip = true;
						}
						if (___axisHorizontal > 0f)
						{
							__instance.puppet2D.flip = false;
						}
					}
					if ((___axisHorizontal < 0f && ___steepSlopeLeft) || ___axisHorizontal <= 0f || ___steepSlopeRight)
					{
					}
					if (___isGrounded)
					{
						if (___axisHorizontal != 0f)
						{
							DisableAnimatorBoolsDelegate(__instance);
							__instance.animator.SetBool("Running", true);
							__instance.playerState = PlayerNew.PlayerState.isRunning;
						}
						else if (__instance.playerState != PlayerNew.PlayerState.isJumping)
						{
							DisableAnimatorBoolsDelegate(__instance);
							__instance.animator.SetBool("Idle", true);
							__instance.playerState = PlayerNew.PlayerState.isIdle;
						}
					}
					if (___lastHitPushable)
					{
						___lastHitPushable.SetPushTarget(false);
					}
					if (___isGrounded)
					{
						Vector2 vector3 = base_transform.position;
						Vector2 vector4 = base_transform.position + __instance.animator.transform.right * ___circleColliderWidth;
						RaycastHit2D[] array3 = Physics2D.LinecastAll(vector3, vector4, __instance.floorLayerMask);
						RaycastHit2D raycastHit2D2 = default(RaycastHit2D);
						float num4 = float.PositiveInfinity;
						foreach (RaycastHit2D raycastHit2D3 in array3)
						{
							if (Vector2.Distance(base_transform.position, raycastHit2D3.point) < num4 && raycastHit2D3 && !raycastHit2D3.collider.isTrigger)
							{
								raycastHit2D2 = raycastHit2D3;
								num4 = Vector2.Distance(base_transform.position, raycastHit2D3.point);
							}
						}
						Debug.DrawLine(vector3, vector4, Color.green);
						if (raycastHit2D2.collider && !raycastHit2D2.collider.isTrigger)
						{
							___lastHitPushable = __instance.hitPushable;
							__instance.hitPushable = raycastHit2D2.collider.GetComponent<Pushable>();
							if (__instance.hitPushable && __instance.hitPushable.enabled && !__instance.hitPushable.IsFloating())
							{
								Rigidbody2D component12 = __instance.hitPushable.GetComponent<Rigidbody2D>();
								__instance.hitPushable.SetPushTarget(true);
								DisableAnimatorBoolsDelegate(__instance);
								__instance.animator.SetBool("PushIdle", true);
								if (___axisAction > 0f)
								{
									__instance.playerState = PlayerNew.PlayerState.isPushLockIn;
								}
								___pushTimer += Time.deltaTime;
								if (__instance.playerState != PlayerNew.PlayerState.isPushLockIn && !___pushLockInTrigger)
								{
									___pushLockInTrigger = true;
								}
								if (num > 0.1f)
								{
									if (__instance.hitPushable.transform.position.x > base_transform.position.x)
									{
										___push = true;
									}
									else
									{
										___push = false;
									}
								}
								else if (num < -0.1f)
								{
									if (__instance.hitPushable.transform.position.x < base_transform.position.x)
									{
										___push = true;
									}
									else
									{
										___push = false;
									}
								}
								if (Mathf.Abs(num) > 0.1f)
								{
									DisableAnimatorBoolsDelegate(__instance);
									__instance.animator.SetBool("Pushing", true);
									if (__instance.playerState != PlayerNew.PlayerState.isPushLockIn)
									{
										__instance.playerState = PlayerNew.PlayerState.isPushing;
									}
									Pushable component13 = ___highestHit.collider.GetComponent<Pushable>();
									if (component13)
									{
										Rigidbody2D component14 = component13.GetComponent<Rigidbody2D>();
										component13.isUnderCrate = true;
									}
									if (!___push)
									{
										___pull = true;
										__instance.animator.SetFloat("Pull", Mathf.Clamp(__instance.animator.GetFloat("Pull") + Time.deltaTime * 5f, 0f, 1f));
									}
									else
									{
										___pull = false;
										__instance.animator.SetFloat("Pull", Mathf.Clamp(__instance.animator.GetFloat("Pull") - Time.deltaTime * 5f, 0f, 1f));
									}
								}
								float num5 = ___rigidbody2D.position.x + (__instance.animator.transform.right * ___circleColliderWidth / 2f).x + __instance.animator.transform.right.x * (component12.GetComponent<Collider2D>().bounds.size.x / 2f + 0.05f);
								float y2 = component12.position.y;
								if (((num5 > component12.position.x && num > 0f) || (num5 < component12.position.x && num < 0f)) && !__instance.hitPushable.isOnConveyer && __instance.hitPushable.isOnGround && ___pull)
								{
									float y3 = component12.velocity.y;
									component12.MovePosition(new Vector2(num5, y2));
								}
								num = Mathf.Clamp(num, -___pushSpeed, ___pushSpeed);
							}
						}
					}
					if (__instance.playerState != PlayerNew.PlayerState.isPushLockIn)
					{
						___pushLockInTrigger = false;
						___pushTimer = 0f;
					}
					if ((__instance.playerState == PlayerNew.PlayerState.isPushLockIn || __instance.playerState == PlayerNew.PlayerState.isPushing || __instance.playerState == PlayerNew.PlayerState.isPushIdle || __instance.playerState == PlayerNew.PlayerState.isIdle) && ___pull)
					{
						component2.localPosition = new Vector2(Mathf.SmoothStep(component2.localPosition.x, __instance.animator.transform.right.x * 0.08f, Time.deltaTime * 10f), component2.localPosition.y);
					}
					else
					{
						component2.localPosition = new Vector2(Mathf.SmoothStep(component2.localPosition.x, 0f, Time.deltaTime * 10f), component2.localPosition.y);
					}
					if (!___isGrounded && __instance.playerState != PlayerNew.PlayerState.isClimbing && ___rigidbody2D.velocity.y < 0f)
					{
						DisableAnimatorBoolsDelegate(__instance);
						__instance.animator.SetBool("Falling", true);
						__instance.playerState = PlayerNew.PlayerState.isFalling;
					}
					if (__instance.playerState != PlayerNew.PlayerState.isFalling || ___lastPlayerState != PlayerNew.PlayerState.isFalling)
					{
					}
					if (___isNotGroundedTimer >= ___timeAfterFallPlayerCanJump && __instance.playerState == PlayerNew.PlayerState.isFalling && ___lastPlayerState != PlayerNew.PlayerState.isJumping)
					{
						___jumpReset = false;
					}
					if (___isGrounded || __instance.playerState == PlayerNew.PlayerState.isClimbing)
					{
						if (___lastAxisJump == 0f)
						{
							___jumpReset = true;
							Main.dbljReset = Main.settings.maxJumps;
							Main.dbljRelease = false;
						}
					}
					else if (___axisJump == 1f && __instance.playerState != PlayerNew.PlayerState.isJumping)
					{
						___jumpPressedInAir = true;
					}
					if ((___isGrounded || ___isNotGroundedTimer < ___timeAfterFallPlayerCanJump) && ___axisJump > 0f && ___jumpReset && !___forceJump && !ColourWheelTrigger.instance.maskActive)
					{
						___isGrounded = false;
						___jumpReset = false;
						__instance.forceJump = true;
					}
					if (!(___isGrounded || ___isNotGroundedTimer < ___timeAfterFallPlayerCanJump) && ___axisJump > 0f && Main.dbljReset > 0 && Main.dbljRelease && !Main.forceDblj && !ColourWheelTrigger.instance.maskActive)
					{
						Main.forceDblj = true;
					}
					if (!(___isGrounded || ___isNotGroundedTimer < ___timeAfterFallPlayerCanJump) && ___lastAxisJump == 0f && ___axisJump == 0f)
						Main.dbljRelease = true;

					if (__instance.forceJump)
					{
						___jumpReset = false;
						__instance.forceJump = false;
						Main.dbljReset = Main.settings.maxJumps;
						Main.dbljRelease = false;
						num2 = 1f * ___jumpForce + Mathf.Clamp(num2 / 2f, 0f, float.PositiveInfinity);
						DisableAnimatorBoolsDelegate(__instance);
						__instance.animator.SetBool("Jumping", true);
						__instance.playerState = PlayerNew.PlayerState.isJumping;
						EventManager.Instance.PostEvent("Anim/Jump", base_gameObject);
						___playerJumpLaunchReset = false;
					}
					if (Main.forceDblj)
					{
						Main.dbljReset--;
						Main.forceDblj = false;
						Main.dbljRelease = false;
						num2 = 1f * ___jumpForce;
						DisableAnimatorBoolsDelegate(__instance);
						__instance.animator.SetBool("Jumping", true);
						__instance.playerState = PlayerNew.PlayerState.isJumping;
						EventManager.Instance.PostEvent("Anim/Jump", base_gameObject);
						___playerJumpLaunchReset = false;
					}
				}
				if (__instance.platform)
				{
					Rigidbody2D component15 = __instance.platform.GetComponent<Rigidbody2D>();
					if (___platformLastPosition == Vector2.zero || __instance.platformLast != __instance.platform)
					{
						___platformLastPosition = component15.position;
					}
					num3 += (component15.position - ___platformLastPosition).x;
					___platformLastPosition = component15.position;
					__instance.platformLast = __instance.platform;
				}
				else
				{
					___platformLastPosition = Vector2.zero;
				}
				___rigidbody2D.mass = 1f;
				bool flag2 = true;
				if (___highestHit.collider)
				{
					Rigidbody2D component16 = ___highestHit.collider.GetComponent<Rigidbody2D>();
					if (component16)
					{
						Pushable component17 = component16.GetComponent<Pushable>();
						if (__instance.playerState != PlayerNew.PlayerState.isPushing && __instance.playerState != PlayerNew.PlayerState.isPushIdle && __instance.playerState != PlayerNew.PlayerState.isPushLockIn)
						{
							___rigidbody2D.mass = 0.1f;
							flag2 = false;
							if (___lastStandPushable != component17)
							{
								___pushableLastPositionX = 0f;
							}
							if (___pushableLastPositionX != 0f)
							{
								float num7 = component17.transform.position.x - ___pushableLastPositionX;
								num3 += num7;
							}
							if (component17)
							{
								___pushableLastPositionX = component17.transform.position.x;
								___lastStandPushable = component17;
							}
							else
							{
								___pushableLastPositionX = 0f;
								___lastStandPushable = null;
							}
						}
					}
				}
				if (___highestHit)
				{
					Thwomp component18 = ___highestHit.collider.GetComponent<Thwomp>();
					if (component18 && component18.currentState == Thwomp.State.Rising && hit)
					{
						__instance.Kill();
					}
				}
				if (flag2)
				{
					___pushableLastPositionX = 0f;
				}
				num2 -= Mathf.Lerp(___gravityForce, 0f, Mathf.Clamp(num2, -10f * Mathf.Sqrt(Main.settings.airFrictScale), 0f) / -10f);
				if (!__instance.isEnabled)
				{
					num = 0f;
				}
				___rigidbody2D.position = new Vector2(num3, y);
				___rigidbody2D.velocity = new Vector2(num, num2) + __instance.environmentalVelocity;
				___rigidbody2D.velocity += !___isGrounded ? Main.wind : new Vector2(Main.wind.x/(1f+2*Main.settings.groundFrictScale),Main.wind.y);
				if (___isGrounded)
				{
					__instance.feetDust.emissionRate = Mathf.Abs(___rigidbody2D.velocity.x) * 5f;
					Renderer component19 = __instance.feetDust.GetComponent<Renderer>();
					__instance.feetDust.startColor = Color.black;
					component19.sortingLayerName = "Level";
					if (___feetColour != Color.white && ___feetColour != Color.black)
					{
						__instance.feetDust.startColor = ___feetColour;
						component19.sortingLayerName = "Colours";
					}
					PhysicsMaterial2D physicsMaterial2D = __instance.GetStandingMaterial();
					if (physicsMaterial2D && (physicsMaterial2D.name == "Snow" || physicsMaterial2D.name == "Water"))
					{
						__instance.feetDust.startColor = Color.white;
						component19.sortingLayerName = "Level";
					}
				}
				else
				{
					__instance.feetDust.emissionRate = 0f;
				}
				if (___justLanded)
				{
				}
				__instance.lastVelocity = ___rigidbody2D.velocity;
				___lastPlayerState = __instance.playerState;

				return false;
			}
			catch (Exception e)
			{
				Main.mod.Logger.Error(e.ToString());
				return true;
			}
		}
	}
}