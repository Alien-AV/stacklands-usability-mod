using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace StacklandsUsabilityMod.ShiftDrag
{
    class ShiftDrag
    {
        [HarmonyPatch(typeof(WorldManager))]
        [HarmonyPatch("Update")]
        public static class AddCallToFindParentIfHoldingShift
        {
            enum State { Init, AfterGetMouseButtonDown, AfterLoadHoveredDraggable, AfterInjection };


            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                State currentState = State.Init;
                FileLog.Debug("Running transpiler:");

                var f_HoveredDraggable = AccessTools.Field(typeof(WorldManager), nameof(WorldManager.HoveredDraggable));
                var m_GetMouseButtonDown = AccessTools.Method(typeof(Input), nameof(Input.GetMouseButtonDown));

                foreach (var instruction in instructions)
                {
                    yield return instruction;
                    FileLog.Debug($"[{currentState}] {instruction}");
                    if (currentState == State.Init && instruction.Calls(m_GetMouseButtonDown)) { currentState = State.AfterGetMouseButtonDown; }
                    if (currentState == State.AfterGetMouseButtonDown && instruction.LoadsField(f_HoveredDraggable)) { currentState = State.AfterLoadHoveredDraggable; }
                    if (currentState == State.AfterLoadHoveredDraggable && instruction.Branches(out var label))
                    {
                        var newInstructions = new[]{
                                                    new CodeInstruction(OpCodes.Ldarg_0),
                                                    CodeInstruction.LoadField(typeof(WorldManager), nameof(WorldManager.HoveredDraggable), true),
                                                    CodeInstruction.Call(typeof(ShiftDrag), nameof(FindParentIfHoldingShift))
                        };

                        foreach (var newInstruction in newInstructions)
                        {
                            FileLog.Debug($"[Injecting] {newInstruction}");
                            yield return newInstruction;
                        }

                        currentState = State.AfterInjection;
                    }
                }
            }
        }


        public static void FindParentIfHoldingShift(ref Draggable hoveredDraggable)
        {
            GameCard gameCard;
            if (Input.GetKey(KeyCode.LeftShift) && (gameCard = (hoveredDraggable as GameCard)) != null)
            {
                while (gameCard.Parent != null && gameCard.Parent.CanBeDragged())
                {
                    gameCard = gameCard.Parent;
                }
                hoveredDraggable = gameCard;
            }
        }
    }
}
