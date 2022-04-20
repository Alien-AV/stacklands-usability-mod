using HarmonyLib;
using UnityEngine;

namespace StacklandsUsabilityMod
{
	//[HarmonyPatch(typeof(WorldManager))]
	//[HarmonyPatch("Update")]
	class ShiftDragPatch
	{
		static bool Prefix(WorldManager __instance)
		{
			NewUpdate(__instance);
			return false;
		}
		private static void NewUpdate(WorldManager __instance)
		{
			var worldManager = Traverse.Create(__instance);
			float b = worldManager.Method("DetermineTargetWorldSize").GetValue<float>();
			__instance.WorldSizeIncrease = Mathf.Lerp(__instance.WorldSizeIncrease, b, Time.deltaTime * 12f);
			Shader.SetGlobalFloat("_WorldSizeIncrease", __instance.WorldSizeIncrease);
			__instance.TopBgElements.localPosition = Vector3.forward * __instance.WorldSizeIncrease;
			__instance.BottomBgElements.localPosition = Vector3.back * __instance.WorldSizeIncrease;
			__instance.LeftBgElements.localPosition = Vector3.left * __instance.WorldSizeIncrease;
			__instance.RightBgElements.localPosition = Vector3.right * __instance.WorldSizeIncrease;
			worldManager.Method("UpdatePhysics");
			worldManager.Method("CheckQueuedAnimations");
			bool flag = false;
			if (__instance.starterPack == null || __instance.starterPack.WasClicked)
			{
				flag = true;
			}
			if (worldManager.Field("currentAnimationRoutine").GetValue<Coroutine>() != null || worldManager.Field("currentAnimation").GetValue() != null)
			{
				flag = false;
			}
			if (flag)
			{
				__instance.MonthTimer += Time.deltaTime * __instance.TimeScale;
			}
			if (__instance.MonthTimer >= __instance.MonthTime)
			{
				__instance.MonthTimer -= __instance.MonthTime;
				__instance.CurrentMonth++;
				worldManager.Method("EndOfMonth");
			}
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			Plane plane = new Plane(Vector3.up, Vector3.zero);
			float d;
			plane.Raycast(ray, out d);
			worldManager.Field("mouseWorldPosition").SetValue(ray.origin + ray.direction * d);
			__instance.HoveredDraggable = null;
			__instance.HoveredInteractable = null;
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				__instance.TogglePause();
			}
			bool flag2 = GameCanvas.instance.PositionIsOverUI(Input.mousePosition);
			if (!flag2)
			{
				int num = Physics.RaycastNonAlloc(ray, worldManager.Field("hits").GetValue<RaycastHit[]>());
				float num2 = float.MaxValue;
				for (int i = 0; i < num; i++)
				{
					RaycastHit raycastHit = worldManager.Field("hits").GetValue<RaycastHit[]>()[i];
					Draggable component = raycastHit.collider.gameObject.GetComponent<Draggable>();
					if (component != null)
					{
						float distance = raycastHit.distance;
						if (distance < num2)
						{
							num2 = distance;
							if (__instance.DraggingDraggable == null)
							{
								__instance.HoveredDraggable = component;
							}
						}
					}
					Interactable component2 = raycastHit.collider.gameObject.GetComponent<Interactable>();
					if (component2 != null)
					{
						__instance.HoveredInteractable = component2;
					}
				}
			}
			if (__instance.HoveredInteractable != null)
			{
				GameScreen.InfoBoxTitle = __instance.HoveredInteractable.name;
				GameScreen.InfoBoxText = __instance.HoveredInteractable.GetTooltipText();
			}
			if (__instance.CanInteract)
			{
				if (Input.GetMouseButtonDown(0) && !flag2)
				{
					if (__instance.HoveredDraggable != null)
					{
						GameCard gameCard;
						if (Input.GetKey(KeyCode.LeftShift) && (gameCard = (__instance.HoveredDraggable as GameCard)) != null)
						{
							while (gameCard.Parent != null && gameCard.Parent.CanBeDragged())
							{
								gameCard = gameCard.Parent;
							}
							__instance.HoveredDraggable = gameCard;
						}
						if (__instance.HoveredDraggable.CanBeDragged())
						{
							__instance.DraggingDraggable = __instance.HoveredDraggable;
							__instance.DraggingDraggable.DragStartPosition = __instance.DraggingDraggable.transform.position;
							worldManager.Field("grabOffset").SetValue(worldManager.Field("mouseWorldPosition").GetValue<Vector3>() - __instance.DraggingDraggable.transform.position);
							__instance.DraggingDraggable.StartDragging();
						}
						else
						{
							__instance.HoveredDraggable.Clicked();
							GameCard gameCard2 = __instance.HoveredDraggable as GameCard;
							if (gameCard2 != null)
							{
								gameCard2.RotWobble(1f);
							}
						}
					}
					else if (__instance.HoveredInteractable != null)
					{
						__instance.HoveredInteractable.Click();
					}
					else
					{
						GameCamera.instance.StartDragging();
					}
				}
				if (Input.GetKeyDown(KeyCode.Backspace) && !flag2 && __instance.HoveredCard != null && __instance.CardCanBeSold(__instance.HoveredCard, true))
				{
					__instance.SellCard(__instance.HoveredCard.transform.position, __instance.HoveredCard.GetRootCard(), 1f, true);
				}
			}
			else if (Input.GetMouseButtonDown(0) && !flag2 && !__instance.InAnimation)
			{
				GameCamera.instance.StartDragging();
			}
			__instance.NearbyCardTarget = null;
			float maxValue = float.MaxValue;
			if (__instance.DraggingDraggable != null)
			{
				__instance.DraggingDraggable.TargetPosition = worldManager.Field("mouseWorldPosition").GetValue<Vector3>() - worldManager.Field("grabOffset").GetValue<Vector3>();
				if (__instance.DraggingCard != null)
				{
					__instance.DraggingCard.Clampieee();
				}
			}
			if (__instance.DraggingCard != null)
			{
				foreach (CardTarget cardTarget in __instance.CardTargets)
				{
					if (cardTarget.CanHaveCard(__instance.DraggingCard))
					{
						float num3 = Vector3.Distance(__instance.DraggingCard.TargetPosition, cardTarget.transform.position);
						if (num3 < __instance.CardTargetSnapDistance && num3 < maxValue)
						{
							__instance.NearbyCardTarget = cardTarget;
							Vector3 targetPosition = __instance.DraggingCard.TargetPosition;
							__instance.DraggingCard.TargetPosition = cardTarget.transform.position;
						}
					}
				}
			}
			if (Application.isEditor && Input.GetKeyDown(KeyCode.R))
			{
				__instance.ClearRoundAndRestart();
			}
			if (!__instance.CanInteract && __instance.DraggingDraggable != null)
			{
				__instance.DraggingDraggable.StopDragging();
				__instance.DraggingDraggable = null;
			}
			if (Input.GetMouseButtonUp(0) || !__instance.CanInteract)
			{
				if (__instance.DraggingCard != null)
				{
					if (__instance.NearbyCardTarget != null)
					{
						__instance.NearbyCardTarget.CardDropped(__instance.DraggingCard);
					}
					else
					{
						__instance.CheckIfCanAddOnStack(__instance.DraggingCard);
					}
				}
				if (__instance.DraggingDraggable != null)
				{
					__instance.DraggingDraggable.StopDragging();
					__instance.DraggingDraggable = null;
				}
			}
			if (__instance.CurrentGameState == WorldManager.GameState.Playing && worldManager.Field("currentAnimationRoutine").GetValue() == null && __instance.starterPack == null && worldManager.Method("CheckVillagersDead").GetValue<bool>())
			{
				__instance.GameOverReason = "All Humans Died";
				__instance.CurrentGameState = WorldManager.GameState.GameOver;
				GameCanvas.instance.SetScreen(GameCanvas.instance.GameOverScreen);
			}
			worldManager.Method("DebugUpdate");
		}

	}
}
