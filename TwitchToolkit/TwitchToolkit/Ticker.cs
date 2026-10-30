using System;
using System.Collections.Concurrent;
using System.Threading;
using RimWorld;
using TwitchToolkit.Store;
using TwitchToolkit.Votes;
using UnityEngine;
using Verse;

namespace TwitchToolkit;

public class Ticker(Game game) : GameComponent
{
    public Timer timer = null;

    public long LastIRCPong = DateTime.Now.ToFileTime();

    public readonly ConcurrentQueue<FiringIncident> FiringIncidents = new();

    public readonly ConcurrentQueue<VoteEvent> VoteEvents = new();

    public readonly ConcurrentQueue<IncidentWorker> Incidents = new();

    public readonly ConcurrentQueue<IncidentHelper> IncidentHelpers = new();

    public readonly ConcurrentQueue<IncidentHelperVariables> IncidentHelperVariables = new();

    private readonly Game _game = game;

    private readonly TwitchToolkit _mod = Toolkit.Mod;

    private int _lastMinute = -1;

    private int _lastCoinReward = -1;

    public DateTime lastEvent = DateTime.Now;

    public bool CreatedByController { get; internal set; }

    public override void GameComponentTick()
    {
        //IL_0191: Unknown result type (might be due to invalid IL or missing erences)
        //IL_0198: Expected O, but got Unknown
        try
        {
            if (_game == null || _mod == null)
            {
                Log.Warning("[TTK]: Ticker is not initialized yet, skipping tick");
                return;
            }

            _mod.Tick();
            int minutes = (int)(_game.Info.RealPlayTimeInteracting / 60f);
            double getTime = (double)Time.time / 60.0;
            int time = Convert.ToInt32(Math.Truncate(getTime));

            if(!PurchaseRequestService.pendingVariablePurchaseRequests.IsEmpty)
            {
                Log.Warning("[TTK]: Processing pending purchase requests");
                Purchase_Handler.ValidatePendingVariablePurchaseRequests();
            }

            if(!PurchaseRequestService.pendingSimplePurchaseRequests.IsEmpty)
            {
                Log.Warning("[TTK]: Processing pending simple purchase requests");
                Purchase_Handler.ValidatePendingSimplePurchaseRequests();
            }

            if (!IncidentHelpers.IsEmpty)
            {
                Log.Warning("[TTK]: Processing variable incident queue");
                while (!IncidentHelpers.IsEmpty)
                {
                    if (IncidentHelpers.TryDequeue(out IncidentHelper incidentHelper2))
                    {
                        if (!(incidentHelper2 is VotingHelper))
                        {
                            Purchase_Handler.QueuePlayerMessage(incidentHelper2.Viewer, incidentHelper2.message);
                        }
                        incidentHelper2.TryExecute();
                    }
                }
                Helper.playerMessages = [];
            }


            while (!IncidentHelperVariables.IsEmpty)
            {
                Log.Warning("[TTK]: Processing variable incident queue - next incident");

                if (IncidentHelperVariables.TryDequeue(out IncidentHelperVariables incidentHelper))
                {
                    Purchase_Handler.QueuePlayerMessage(incidentHelper.Viewer, incidentHelper.message, incidentHelper.storeIncident.variables);
                    incidentHelper.TryExecute();

                    Purchase_Handler.viewerNamesDoingVariableCommands.Remove(incidentHelper.Viewer.username);
                }
                Helper.playerMessages = [];
            }

            if (!Incidents.IsEmpty)
            {
                if (Incidents.TryDequeue(out IncidentWorker incident))
                {
                    IncidentParms incidentParms = new()
                    {
                        target = (IIncidentTarget)(object)Helper.AnyPlayerMap
                    };

                    incident.TryExecute(incidentParms);
                }
            }

            if (FiringIncidents.Count > 0)
            {
                if (FiringIncidents.TryDequeue(out FiringIncident incident))
                {
                    Helper.Log("Firing " + ((Def)incident.def).defName);
                    incident.def.Worker.TryExecute(incident.parms);
                }
            }

            VoteHandler.CheckForQueuedVotes();

            if (_lastCoinReward < 0)
            {
                _lastCoinReward = time;
            }
            else if (ToolkitSettings.EarningCoins && time - _lastCoinReward >= ToolkitSettings.CoinInterval && Viewers.jsonallviewers != null)
            {
                _lastCoinReward = time;
                Viewers.AwardViewersCoins();
            }

            if (_lastMinute < 0)
            {
                _lastMinute = time;
            }
            else if (_lastMinute < time)
            {
                _lastMinute = time;
                Toolkit.JobManager.CheckAllJobs();
                Viewers.RefreshViewers();
            }
        }
        catch (Exception ex)
        {
            Helper.Log("Exception: " + ex.Message + ex.StackTrace);
        }
    }

}
