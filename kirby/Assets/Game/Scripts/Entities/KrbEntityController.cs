using UnityEngine;
using System.Collections;

public class KrbEntityController : EntityController
{
    public override Player CreatePlayer(PlayerData data, Vector2Int coords)
    {
        BaseEntityDependencies deps = new BaseEntityDependencies()
        {
            ParentNode = null,
            EntityController = this,
            MapController = _mapController,
            GameEvents = _gameEvents,
            Coords = coords
        };
        _player = Create<KrbPlayer>(_entityCreationData.PlayerPrefab, data, deps);
        return _player;
    }

    public override Monster CreateMonster(MonsterData data, Vector2Int coords)
    {
        BaseEntityDependencies deps = new BaseEntityDependencies()
        {
            ParentNode = null,
            EntityController = this,
            Coords = coords,
            MapController = _mapController
        };
        var monster = Create<KrbMonster>(_entityCreationData.MonsterPrefab, data, deps);
        return monster;
    }
}
