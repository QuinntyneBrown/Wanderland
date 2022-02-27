﻿using Orleans;
using Orleans.Runtime;
using Wanderland.Web.Shared;

namespace Wanderland.Web.Server.Grains
{
    public class TileGrain : Grain, ITileGrain
    {
        public IPersistentState<Tile> Tile { get; }

        public ILogger<TileGrain> Logger { get; }

        public TileGrain([PersistentState(Constants.PersistenceKeys.TileStateName, Constants.PersistenceKeys.TileStorageName)] 
            IPersistentState<Tile> tile,
            ILogger<TileGrain> logger)
        {
            Tile = tile;
            Logger = logger;
        }

        async Task ITileGrain.Arrives(IWanderGrain wanderer)
        {
            var wandererName = wanderer.GetPrimaryKeyString();
            if(!Tile.State.WanderersHere.Any(x => x.GetPrimaryKeyString() == wandererName))
            {
                Tile.State.WanderersHere.Add(wanderer);
                Logger.LogInformation($"{wandererName} has wandered into tile {this.GetPrimaryKeyString()}");
            }
        }

        async Task ITileGrain.Leaves(IWanderGrain wanderer)
        {
            var wandererName = wanderer.GetPrimaryKeyString();
            if (Tile.State.WanderersHere.Any(x => x.GetPrimaryKeyString() == wandererName))
            {
                Tile.State.WanderersHere.Remove(wanderer);
                Logger.LogInformation($"{wandererName} has left tile {this.GetPrimaryKeyString()}");
            }
        }

        async Task<Tile> ITileGrain.GetTile()
        {
            return Tile.State;
        }

        async Task ITileGrain.SetTileInfo(Tile tile)
        {
            Tile.State = tile;
        }
    }
}
