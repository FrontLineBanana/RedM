using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using static CitizenFX.Core.Native.API;

// This project contains various emotes for a RedM server. Do emotes by typing /e [emotename], cancel by typing just /e

// TODO: Get emotes working, add more emotes, make separate dictionary class, change /e to cancel emote and play new emote upon new command

// Current main issue: The emote is not actually playing on the ped. (Either player ped is wrong or Hash)

namespace Emotes
{
    public class Client : BaseScript
    {
        //Vars
        bool isInEmote = false;
        string conditions = "";

        public void RegisterEventHandler(string name, Delegate action)
        {
            try
            {
                EventHandlers[name] += action;
            }
            catch (Exception ex)
            {
                CitizenFX.Core.Debug.WriteLine("Error trying to register event");
            }
        }

        public Client()
        {
            CitizenFX.Core.Debug.WriteLine("Client for Emotes started");
            StartResource();

            //Sets emoteList
            SetEmoteNames();
        }

        private void StartResource()
        {
            CitizenFX.Core.Debug.WriteLine("/e registered.");

            RegisterCommand("e", new Action<int, List<object>, string>((source, args, raw) =>
            {
                //Sets a string from what was typed after /e
                conditions = Convert.ToString(args[0]);
                CitizenFX.Core.Debug.WriteLine("Conditions set.");
                CitizenFX.Core.Debug.WriteLine("conditions = " + conditions + ".");

                //Sets emote or clears emote if one is used and there is no space
                if (!isInEmote && args.Count() > 0)
                {
                    //Compares the conditions with the library and plays the emote if available
                    PlayEmote(conditions);
                    CitizenFX.Core.Debug.WriteLine("Emote played");
                }
                else if (isInEmote && conditions == "")
                {
                    CancelEmote();
                    CitizenFX.Core.Debug.WriteLine("Emote canceled");
                }
                else if (isInEmote && conditions != "")
                {
                    CitizenFX.Core.Debug.WriteLine("Emote canceled");
                    CancelEmote();
                    PlayEmote(conditions);
                    return;
                }
                else
                {
                    TriggerEvent("chat:addMessage", new
                    {
                        color = new[] { 255, 0, 0 },
                        args = new[] { "Emote not found." }
                    });
                    CitizenFX.Core.Debug.WriteLine("Emote not found or bug because u dumb. :)");
                    return;
                }
            }), false);
        }

        void PlayEmote(string emoteName)
        {

            var player = GetPlayerPed(-1);
            CitizenFX.Core.Debug.WriteLine("PlayerPed is: " + player.ToString());

            if (EmotesList.ContainsValue(conditions))
            {
                Function.Call(Hash._TASK_START_SCENARIO_IN_PLACE, player, emoteName, -1, false, false, false, 1.0, false);
                CitizenFX.Core.Debug.WriteLine("Playing emote: " + emoteName);
            }

            isInEmote = true;
        }

        void CancelEmote()
        {
            var player = GetPlayerPed(-1);

            Function.Call(Hash.CLEAR_PED_TASKS, player);

            isInEmote = false;
        }

        void SetEmoteNames()
        {
            for (int i = 0; i < emoteHash.Length; i++)
            {
                //Add Hashes and Custom Names
                EmotesList.Add(emoteHash[i], customName[i]);
            }

            CitizenFX.Core.Debug.WriteLine("List set.");

            for (int i = 0; i < emoteHash.Length; i++)
            {
                //Add IDs to the list
                EmotesList.Add(i.ToString(), customName[i]);
            }

            CitizenFX.Core.Debug.WriteLine("IDs set.");
        }

        //Dictionary for holding the values and keys
        Dictionary<string, string> EmotesList = new Dictionary<string, string>();

        //List for Native Keys of emotes, in order with customName
        private string[] emoteHash =
        {
            "WORLD_HUMAN_SMOKE_CIGAR",
            "WORLD_HUMAN_SMOKE",
            "WORLD_HUMAN_TRUMPET"
        };


        //List of emote names to type in chat, in order with emoteHash
        private string[] customName =
        {
            "cigar",
            "smoke",
            "trumpet"
        };
    }
}