# Game Events

Photeus handles logic by loading external scripts from `content\logic`, `data\mods` and internal scripts from any class that inherits `Plugin` in the game assembly. It does this via reflection at runtime. Scripts that produce runtime errors will not be loaded.

The base class `Plugin` provides access to all services and all assemblies loaded by the game (as a result, scripts are **not** sandboxed, any caution should be taken when putting scripts from unknown sources into `data\mods`). It also provides a number of helper methods listed under the **Commands** section.

Plugin scripts are entirely event driven, with each method having *Conditions* and *Triggers* to determine when the events run. The engine has an underlying event queue in which all methods that implement *Triggers Attributes* will automatically be subscribed to the event they request. When an event is raised, if the conditions to run the method is met, determined by *Conditions Attributes*, the method will run. 

Every event must have a `[GameEvent]` attribute, or it will not be handled when the game loads.

## Attributes

### `[GameEvent]`

Mandatory, in order for the engine to pick up the method and subscribe it to the relevant events.

### `[RunOnce]`

The RunOnce attribute makes it so that once the event runs once, any triggers associated with the event are unsubscribed. `[RunOnce]` has an optional parameter (used like `[RunOnce("Flag")]`) which persists the event having ran once already and will prevent it from running again after the player saves their game.

`[RunOnce("Flag")]` could be seen as shorthand for:

```csharp
[RunOnce]
[FlagCondition("Flag", false)]
void SomeRandomEvent(IGameEventArgs args) 
{
    // (Actual event code)

    SetFlag("Flag", true);
}
```

### `[Trigger("Event ID")]`

More than one trigger can be used for a method.

### `[AutoRunOverworld]` (not implemented yet)

A trigger that runs every frame when on the overworld.

### `[AutoRunBattle]` (not implemented yet)

A trigger that runs every frame when in a battle. Can be used for stuff like checking game over conditions.

### `[FlagCondition("Flag ID", Boolean Value)]`

More than one condition can be used for a method.

### `[VariableCondition("Variable ID", Equality, Value)]`

More than one condition can be used for a method.

### `[ScreenCondition("Screen Name")]` (not implemented yet)

A condition that checks what the current active screen is

## Triggers

**GAME_START**  
Raised when the game starts

**SAVE_LOADED**: (not implemented yet)


**SAVE_CREATED**: (not implemented yet)


**STEP**: (not implemented yet)


**STARTED_WALKING**: (not implemented yet)


**STOPPED_WALKING**: (not implemented yet)


**STARTED_RUNNING**: (not implemented yet)


**STOPPED_RUNNING**: (not implemented yet)


**STARTED_STANDING**: (not implemented yet)


**STOPPED_STANDING**: (not implemented yet)


**ENTERED_ZONE**: (not implemented yet) (dynamic)


**EXITED_ZONE**: (not implemented yet) (dynamic)


**ENTERED_MAP**: (not implemented yet) (dynamic)


**EXITED_MAP**: (not implemented yet) (dynamic)


**INTERACT_ACTION**: (dynamic)


**INTERACT_AREA_ENTER**: (dynamic)


**INTERACT_AREA_EXIT**: (dynamic)


**INTERACT_AREA_STAND**: (dynamic)


**INTERACT_AREA_WALK**: (dynamic)


**INTERACT_AREA_RUN**: (dynamic)


**COLLISION**:


## Event Commands

### `Spawn(IEnumerator) : IEnumerator`

### `Dialogue(string, string) : Dialogue`

### `LockMovement(Func<IEnumerator>) : IEnumerator`

### `Move(IGameObject, Vector2, float) : IEnumerator`

### `Wait(float) : Wait`

### `GetText(string) : string`

### `PickChoice(string[]) : int` (not implemented yet)

### `PlayerFlag(string, bool?) : bool`

### `PlayerVariable(string, IComparable?) : IComparable`

### `Save()`

### `Notify(string, IGameEventArgs)`

### `Warp(string, Vector2)`

### `Warp(Vector2)`

### `GetGameObjectByName(string) : IGameObject` (not properly implemented yet)

### `GetGameObjectsByTag(string) : IGameObjectCollection` (not properly implemented yet)

### `FadeToBlack() : IEnumerator` (not implemented yet)

### `FadeToColour(Color) : IEnumerator` (not implemented yet)

### `FadeInGame() : IEnumerator` (not implemented yet)

### `ShowBlackBars() : IEnumerator` (not implemented yet)

### `HideBlackBars() : IEnumerator` (not implemented yet)

### `PlayBgm(string)` (not implemented yet)

### `StopBgm()` (not implemented yet)

### `PlaySfx(string)` (not implemented yet)

### `PlaySfx(string, Vector2)` (not implemented yet)

### 



Todo List:
- Implement missing methods
- Make every IEnumerator function return a wrapped Coroutine instead
- Change Input to be a typical class that receives inputs instead of a component
    - Change Input related things to be event driven
- Look at refactoring existing game logic out of the codebase and into plugins
