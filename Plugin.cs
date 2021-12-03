using DiskCardGame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using APIPlugin;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using GBC;
using HarmonyLib;
using Pixelplacement;
using Sirenix;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.PostProcessing;
using UnityEngine.XR;

namespace cardsmechanics
{




    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public class Plugin : BaseUnityPlugin
    {
        private const string PluginGuid = "kopie.inscryption.cardsmechanics";
        private const string PluginName = "cardsmechanics";
        private const string PluginVersion = "1.0.0";
        internal static ManualLogSource Log;


        
        
        
        public class spellexamplenew : AbilityBehaviour
        {
	        public override Ability Ability
	        {
		        get
		        {
			        return ability;
		        }
	        }

	        public static Ability ability;
	        
	        //SlotTargetedForAttack
	        public override bool RespondsToSlotTargetedForAttack(CardSlot slot, PlayableCard attacker)
	        {
		        return true;
	        }

	        public override IEnumerator OnSlotTargetedForAttack(CardSlot slot, PlayableCard attacker)
	        {
		        yield return base.PreSuccessfulTriggerSequence();
		        yield return new WaitForSeconds(0.2f);
		        yield return slot.Card.TakeDamage(3, attacker);
		        yield break;
	        }
        }
        
        
        
        
        private NewAbility AddAbility1()
        {
	        AbilityInfo info = ScriptableObject.CreateInstance<AbilityInfo>();
	        info.powerLevel = 0;
	        info.rulebookName = "Spell core";
	        info.rulebookDescription = "Core needed to detect if a card is a spell!";
	        info.metaCategories = new List<AbilityMetaCategory> {AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part1Modular};

	        List<DialogueEvent.Line> lines = new List<DialogueEvent.Line>();
	        DialogueEvent.Line line = new DialogueEvent.Line();
	        line.text = "";
	        lines.Add(line);
	        info.abilityLearnedDialogue = new DialogueEvent.LineSet(lines);

	        byte[] imgBytes = System.IO.File.ReadAllBytes(Path.Combine(this.Info.Location.Replace("cardsmechanics.dll",""),"Artwork/spellcore.png"));
	        Texture2D tex = new Texture2D(2,2);
	        tex.LoadImage(imgBytes);

	        NewAbility spellcore = new NewAbility(info,typeof(spellcorenew),tex,AbilityIdentifier.GetAbilityIdentifier(PluginGuid, info.rulebookName));
	        spellcorenew.ability = spellcore.ability;
	        return spellcore;
        }
        
        
        public class spellcorenew : AbilityBehaviour
        {
	        public override Ability Ability
	        {
		        get
		        {
			        return ability;
		        }
	        }

	        public static Ability ability;
        }
        

	    [HarmonyPatch(typeof(BoardManager), "ChooseSlot")]
        public class patchtheslots
        {
            static void Prefix(out BoardManager __state, ref BoardManager __instance)
            {
                __state = __instance;
            }

            
            
            public static IEnumerator Postfix(IEnumerator enumerator, List<CardSlot> validSlots, bool canCancel, BoardManager __state)
            {
	            if (GameObject.Find("CardNearBoardParent").GetComponentInChildren<PlayableCard>().Info.HasAbility(APIPlugin.NewAbility.abilities.Find(ability => ability.id==AbilityIdentifier.GetAbilityIdentifier("kopie.inscryption.cardsmechanics", "Spell core")).ability))
	            {
		            __state.ChoosingSlot = true;
		            Singleton<InteractionCursor>.Instance.ForceCursorType(CursorType.Place);
		            if (!canCancel)
		            {
			            Singleton<ViewManager>.Instance.Controller.SwitchToControlMode(__state.choosingSlotViewMode, false);
		            }
		            Singleton<ViewManager>.Instance.SwitchToView(__state.combatView, false, false);
		            __state.cancelledPlacementWithInput = false;
		            __state.currentValidSlots = validSlots;
		            __state.LastSelectedSlot = null;
		            foreach (CardSlot cardSlot in __state.allSlots.FindAll(slot => slot.Card == null))
		            {
			            cardSlot.SetEnabled(false);
			            cardSlot.ShowState(HighlightedInteractable.State.NonInteractable, false, 0.15f);
		            }
		            __state.SetQueueSlotsEnabled(false);
		            foreach (CardSlot cardSlot2 in validSlots)
		            {
			            cardSlot2.Chooseable = true;
		            }
		            yield return new WaitUntil(() => __state.LastSelectedSlot != null || (canCancel && __state.cancelledPlacementWithInput));
		            if (canCancel && __state.cancelledPlacementWithInput)
		            {
			            Singleton<ViewManager>.Instance.SwitchToView(__state.defaultView, false, false);
		            }
		            foreach (CardSlot cardSlot3 in __state.allSlots.FindAll(slot => slot.Card != null))
		            {
			            cardSlot3.SetEnabled(true);
			            cardSlot3.ShowState(HighlightedInteractable.State.Interactable, false, 0.15f);
		            }
		            __state.SetQueueSlotsEnabled(true);
		            foreach (CardSlot cardSlot4 in validSlots)
		            {
			            cardSlot4.Chooseable = false;
		            }
		            Singleton<ViewManager>.Instance.Controller.SwitchToControlMode(__state.defaultViewMode, false);
		            __state.ChoosingSlot = false;
		            Singleton<InteractionCursor>.Instance.ClearForcedCursorType();
		            yield break;
  
	            }
	            else
	            {
		            __state.ChoosingSlot = true;
		            Singleton<InteractionCursor>.Instance.ForceCursorType(CursorType.Place);
		            if (!canCancel)
		            {
			            Singleton<ViewManager>.Instance.Controller.SwitchToControlMode(__state.choosingSlotViewMode, false);
		            }
		            Singleton<ViewManager>.Instance.SwitchToView(__state.boardView, false, false);
		            __state.cancelledPlacementWithInput = false;
		            __state.currentValidSlots = validSlots;
		            __state.LastSelectedSlot = null;
		            foreach (CardSlot cardSlot in __state.opponentSlots)
		            {
			            cardSlot.SetEnabled(false);
			            cardSlot.ShowState(HighlightedInteractable.State.NonInteractable, false, 0.15f);
		            }
		            __state.SetQueueSlotsEnabled(false);
		            foreach (CardSlot cardSlot2 in validSlots)
		            {
			            cardSlot2.Chooseable = true;
		            }
		            yield return new WaitUntil(() => __state.LastSelectedSlot != null || (canCancel && __state.cancelledPlacementWithInput));
		            if (canCancel && __state.cancelledPlacementWithInput)
		            {
			            Singleton<ViewManager>.Instance.SwitchToView(__state.defaultView, false, false);
		            }
		            foreach (CardSlot cardSlot3 in __state.opponentSlots)
		            {
			            cardSlot3.SetEnabled(true);
			            cardSlot3.ShowState(HighlightedInteractable.State.Interactable, false, 0.15f);
		            }
		            __state.SetQueueSlotsEnabled(true);
		            foreach (CardSlot cardSlot4 in validSlots)
		            {
			            cardSlot4.Chooseable = false;
		            }
		            Singleton<ViewManager>.Instance.Controller.SwitchToControlMode(__state.defaultViewMode, false);
		            __state.ChoosingSlot = false;
		            Singleton<InteractionCursor>.Instance.ClearForcedCursorType();
		            yield break;
	            }

            }
        }
        
        


        
	    [HarmonyPatch(typeof(PlayerHand), "PlayCardOnSlot")]
        public class patchtheenterboard
        {
            static void Prefix(out PlayerHand __state, ref PlayerHand __instance)
            {
                __state = __instance;
            }

            
            
            public static IEnumerator Postfix(IEnumerator enumerator, PlayableCard card, CardSlot slot, PlayerHand __state)
            {
	            
	            if (card.Info.HasAbility(APIPlugin.NewAbility.abilities.Find(ability => ability.id==AbilityIdentifier.GetAbilityIdentifier("kopie.inscryption.cardsmechanics", "Spell core")).ability))
	            {
		            if (__state.CardsInHand.Contains(card))
		            {
			            __state.RemoveCardFromHand(card);
			            if (card.TriggerHandler.RespondsToTrigger(Trigger.PlayFromHand, Array.Empty<object>()))
			            {
				            yield return card.TriggerHandler.OnTrigger(Trigger.PlayFromHand, Array.Empty<object>());
			            }
			            yield return card.TriggerHandler.OnTrigger(Trigger.SlotTargetedForAttack, new object[]
				            {
					            slot,
					            card
				            });
			            card.Anim.PlayDeathAnimation();
		            }
		            yield break;
	            }
	            else
	            {
		            if (__state.CardsInHand.Contains(card))
		            {
			            __state.RemoveCardFromHand(card);
			            if (card.TriggerHandler.RespondsToTrigger(Trigger.PlayFromHand, Array.Empty<object>()))
			            {
				            yield return card.TriggerHandler.OnTrigger(Trigger.PlayFromHand, Array.Empty<object>());
			            }
			            yield return Singleton<BoardManager>.Instance.ResolveCardOnBoard(card, slot, 0.1f, null, true);
		            }
		            yield break;
	            }
	            
            }
        }
        
 
        
        
	    [HarmonyPatch(typeof(PlayerHand), "SelectSlotForCard")]
        public class patchtheplay
        {
            static void Prefix(out PlayerHand __state, ref PlayerHand __instance)
            {
                __state = __instance;
            }

            
            
            public static IEnumerator Postfix(IEnumerator enumerator, PlayableCard card, PlayerHand __state)
            {
	            if (card.Info.HasAbility(APIPlugin.NewAbility.abilities.Find(ability => ability.id==AbilityIdentifier.GetAbilityIdentifier("kopie.inscryption.cardsmechanics", "Spell core")).ability))
	            {
		            __state.CardsInHand.ForEach(delegate(PlayableCard x)
			{
				x.SetEnabled(false);
			});
			yield return new WaitWhile(() => __state.ChoosingSlot);
			__state.OnSelectSlotStartedForCard(card);
			if (Singleton<RuleBookController>.Instance != null)
			{
				Singleton<RuleBookController>.Instance.SetShown(false, true);
			}
			Singleton<BoardManager>.Instance.CancelledSacrifice = false;
			__state.choosingSlotCard = card;
			if (card != null && card.Anim != null)
			{
				card.Anim.SetSelectedToPlay(true);
			}
			Singleton<BoardManager>.Instance.ShowCardNearBoard(card, true);
			if (Singleton<TurnManager>.Instance.SpecialSequencer != null)
			{
				yield return Singleton<TurnManager>.Instance.SpecialSequencer.CardSelectedFromHand(card);
			}
			bool cardWasPlayed = false;
			bool requiresSacrifices = card.Info.BloodCost > 0;
			if (requiresSacrifices)
			{
				List<CardSlot> validSlots = Singleton<BoardManager>.Instance.AllSlotsCopy.FindAll((CardSlot x) => x.Card != null);
				yield return Singleton<BoardManager>.Instance.ChooseSacrificesForCard(validSlots, card);
			}
			if (!Singleton<BoardManager>.Instance.CancelledSacrifice)
			{
				List<CardSlot> validSlots2 = Singleton<BoardManager>.Instance.AllSlotsCopy.FindAll((CardSlot x) => x.Card != null);
				yield return Singleton<BoardManager>.Instance.ChooseSlot(validSlots2, !requiresSacrifices);
				CardSlot lastSelectedSlot = Singleton<BoardManager>.Instance.LastSelectedSlot;
				if (lastSelectedSlot != null)
				{
					cardWasPlayed = true;
					card.Anim.SetSelectedToPlay(false);
					yield return __state.PlayCardOnSlot(card, lastSelectedSlot);
					if (card.Info.BonesCost > 0)
					{
						yield return Singleton<ResourcesManager>.Instance.SpendBones(card.Info.BonesCost);
					}
					if (card.EnergyCost > 0)
					{
						yield return Singleton<ResourcesManager>.Instance.SpendEnergy(card.EnergyCost);
					}
				}
			}
			if (!cardWasPlayed)
			{
				Singleton<BoardManager>.Instance.ShowCardNearBoard(card, false);
			}
			__state.choosingSlotCard = null;
			if (card != null && card.Anim != null)
			{
				card.Anim.SetSelectedToPlay(false);
			}
			__state.CardsInHand.ForEach(delegate(PlayableCard x)
			{
				x.SetEnabled(true);
			});
			yield break;  
	            }
	            else
	            {
		                        __state.CardsInHand.ForEach(delegate(PlayableCard x)
			{
				x.SetEnabled(false);
			});
			yield return new WaitWhile(() => __state.ChoosingSlot);
			__state.OnSelectSlotStartedForCard(card);
			if (Singleton<RuleBookController>.Instance != null)
			{
				Singleton<RuleBookController>.Instance.SetShown(false, true);
			}
			Singleton<BoardManager>.Instance.CancelledSacrifice = false;
			__state.choosingSlotCard = card;
			if (card != null && card.Anim != null)
			{
				card.Anim.SetSelectedToPlay(true);
			}
			Singleton<BoardManager>.Instance.ShowCardNearBoard(card, true);
			if (Singleton<TurnManager>.Instance.SpecialSequencer != null)
			{
				yield return Singleton<TurnManager>.Instance.SpecialSequencer.CardSelectedFromHand(card);
			}
			bool cardWasPlayed = false;
			bool requiresSacrifices = card.Info.BloodCost > 0;
			if (requiresSacrifices)
			{
				List<CardSlot> validSlots = Singleton<BoardManager>.Instance.PlayerSlotsCopy.FindAll((CardSlot x) => x.Card != null);
				yield return Singleton<BoardManager>.Instance.ChooseSacrificesForCard(validSlots, card);
			}
			if (!Singleton<BoardManager>.Instance.CancelledSacrifice)
			{
				List<CardSlot> validSlots2 = Singleton<BoardManager>.Instance.PlayerSlotsCopy.FindAll((CardSlot x) => x.Card == null);
				yield return Singleton<BoardManager>.Instance.ChooseSlot(validSlots2, !requiresSacrifices);
				CardSlot lastSelectedSlot = Singleton<BoardManager>.Instance.LastSelectedSlot;
				if (lastSelectedSlot != null)
				{
					cardWasPlayed = true;
					card.Anim.SetSelectedToPlay(false);
					yield return __state.PlayCardOnSlot(card, lastSelectedSlot);
					if (card.Info.BonesCost > 0)
					{
						yield return Singleton<ResourcesManager>.Instance.SpendBones(card.Info.BonesCost);
					}
					if (card.EnergyCost > 0)
					{
						yield return Singleton<ResourcesManager>.Instance.SpendEnergy(card.EnergyCost);
					}
				}
			}
			if (!cardWasPlayed)
			{
				Singleton<BoardManager>.Instance.ShowCardNearBoard(card, false);
			}
			__state.choosingSlotCard = null;
			if (card != null && card.Anim != null)
			{
				card.Anim.SetSelectedToPlay(false);
			}
			__state.CardsInHand.ForEach(delegate(PlayableCard x)
			{
				x.SetEnabled(true);
			});
			yield break;
	            }

            }
        }
        




        private void Awake()
        {
	        
	        AddAbility1();
	        Harmony harmony = new Harmony(PluginGuid);
            harmony.PatchAll();
            Plugin.Log = base.Logger;
            
            
        }

    }
}