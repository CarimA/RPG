# Game Events

Photeus handles logic by loading external scripts from `content\logic`, `data\mods` and internal scripts from any class that inherits `Plugin` in the game assembly. It does this with reflection at runtime. Scripts that produce runtime errors will not be loaded.

The base class `Plugin` provides access to all services and all assemblies loaded by the game (as a result, scripts are **not** sandboxed, caution should be taken when putting scripts from unknown sources into `data\mods`). It also provides a number of helper methods listed under the **Event Commands** section.

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

The Trigger attribute will cause the engine to subscribe to the event provided in "Event ID".

More than one trigger can be used for a method. 

See the **Triggers** section for a list of constants for events that triggers can subscribe to.

### `[AutoRunOverworld]` (not implemented yet)

A trigger that runs every frame when on the overworld.

### `[AutoRunBattle]` (not implemented yet)

A trigger that runs every frame when in a battle. Can be used for stuff like checking game over conditions.

### `[FlagCondition("Flag ID", Boolean Value)]`

The FlagCondition attribute will cause the engine to add a conditional to the event subscription that checks whether the player's save data has a flag named "Flag ID" and whether it matches the boolean value you specify.

More than one condition can be used for a method.

### `[VariableCondition("Variable ID", Equality, Value)]`

The VariableCondition attribute will cause the engine to add a conditional to the event subscription that checks whether the player's save data has a flag named "Variable ID" and whether it matches the (IComparable) object you specify in Value, using the equality parameter.

Equality is an enumerable that can be one of six values:

- **Equality.Equals**: checks if the value returned from the flag is equal to the value specified.
- **Equality.GreaterThan**: checks if the value returned from the flag is greater  than the value specified.
- **Equality.GreaterThanOrEquals**: checks if the value returned from the flag is greater than or equal to the value specified.
- **Equality.LessThan**: checks if the value returned from the flag is less than the value specified.
- **Equality.LessThanOrEquals**: checks if the value returned from the flag is less than or equal to the value specified.
- **Equality.Not**: checks if the value is not equal to the value specified.

More than one condition can be used for a method.

### `[StateCondition("Screen Name")]` (not implemented yet)

The StateCondition attribute will cause the engine to add a conditional to the event subscription that checks whether the game is on the specified state.

More than one condition can be used for a method.

[TimePhaseCondition(TimePhase)]

- TimePhase.MidNight
- TimePhase.LateNight
- TimePhase.Sunrise
- TimePhase.EarlyMorning
- TimePhase.LateMorning
- TimePhase.Noon
- TimePhase.EarlyAfternoon
- TimePhase.LateAfternoon
- TimePhase.EarlyEvening
- TimePhase.LateEvening
- TimePhase.EarlyNight

[DayCondtion(Day)]

- Day.Monday
- Day.Tuesday
- Day.Wednesday
- Day.Thursday
- Day.Friday
- Day.Saturday
- Day.Sunday

## Triggers

### ***About Dynamic Triggers***

Some triggers support the ability to amend the constant to specify a specific instance of the trigger to listen to, for example, *INTERACT_AREA_ENTER* can be modified to *INTERACT_AREA_ENTER:example_event* which will subscribe to an event that is only raised specifically when the player enters the *example_event* boundary.

### `GAME_START`

Raised when the game starts

### `SAVE_LOADED` (not implemented yet)

Raised when the player loads a save file in the main menu. Provides the index of the save file that was loaded via SaveEventArgs.

### `SAVE_CREATED` (not implemented yet)

Raised when the player creates a new save file in the main menu. Provides the index of the save file that was created via SaveEventArgs.

### `SAVE_CHANGED` (not implemented yet)

Raised when the player saves their game. Provides the index of the save file that was changed via SaveEventArgs.

### `STEP_TAKEN` (not implemented yet)

Raised when a game object takes a step (corresponding to their animations). Provides the game object that took a step via GameObjectEventArgs.

### `MOVEMENT_STATE_CHANGED` (not implemented yet)

Raised when the player changes between walking, running and standing, with both the old and new states reported. Provides the player's game object and movement states via MovementEventArgs.

### `ENTERED_ZONE` (not implemented yet)

Raised when the player enters a new zone. It is a dynamic trigger that can listen to a specific zone being entered. Provides the zone the player has just entered via ZoneEventArgs.

### `EXITED_ZONE` (not implemented yet)

Raised when the player exits a zone. It is a dynamic trigger that can listen to a specific zone being exited. Provides the zone the player has just exited via ZoneEventArgs.

### `ZONE_CHANGED` (not implemented yet)

Raised when the player changes zone. Provides the old zone and new zone via ZoneChangesEventArgs.

### `ENTERED_MAP` (not implemented yet)

Raised when the player enters a new map. It is a dynamic trigger that can listen to a specific map being entered. Provides the map the player has just entered via MapEventArgs.

### `EXITED_MAP` (not implemented yet)

Raised when the player exits a map. It is a dynamic trigger that can listen to a specific map being exited. Provides the map the player has just exited via ZoneEventArgs.

### `MAP_CHANGED` (not implemented yet)

Raised when the player changes map. Provides the old map and new map via MapChangesEventArgs.

### `INTERACT_ACTION` (dynamic)

Raised when the player presses their interact key/button while standing on an interactable zone. Provides the player game object and interactable zone game object via InteractEventArgs.

### `INTERACT_AREA_ENTER` (dynamic)

Raised when the player enters an interactable zone. Provides the player game object and interactable zone game object via InteractEventArgs.

### `INTERACT_AREA_EXIT` (dynamic)

Raised when the player exits an interactable zone. Provides the player game object and interactable zone game object via InteractEventArgs.

### `INTERACT_AREA_STAND` (dynamic)

Raised while the player is standing on an interactable zone. Provides the player game object and interactable zone game object via InteractEventArgs.

### `INTERACT_AREA_WALK` (dynamic)

Raised while the player is walking on an interactable zone. Provides the player game object and interactable zone game object via InteractEventArgs.

### `INTERACT_AREA_RUN` (dynamic)

Raised while the player is running on an interactable zone. Provides the player game object and interactable zone game object via InteractEventArgs.

### `COLLISION`:

### `INPUT_ACTION_PRESSED` (not implemented yet) (dynamic)

### `INPUT_ACTION_RELEASED` (not implemented yet) (dynamic)

### `ASSET_LOADED` (not implemented yet) (dynamic)

### `ASSET_UNLOADED` (not implemented yet) (dynamic)

### `AUDIO_BGM_PLAYED` (not implemented yet) (dynamic)

### `AUDIO_BGM_STOPPED` (not implemented yet)

### `AUDIO_SFX_PLAYED` (not implemented yet) (dynamic)

### `DIALOGUE_PROGRESSED` (not implemented yet)

Raised with each new character that is added when dialogue runs. Provides the character that was just inserted and the current speaker. Example use: voice sound effects for specific characters.

### `STATE_CHANGED` (not implemented yet) (dynamic)

### `STATE_PUSHED` (not implemented yet) (dynamic)

### `STATE_POPPED` (not implemented yet) (dynamic)

TIME_PHASE_CHANGED

DAY_CHANGED

## EventArg Classes (todo)

### `GameEventArgs`


## Event Commands (todo)

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

EnableMapBgm()

DisableMapBgm()

### `StopBgm()` (not implemented yet)

### `PlaySfx(string)` (not implemented yet)

### `PlaySfx(string, Vector2)` (not implemented yet)

SetTime

SetTimeScale

GetTime

GetTimePhase

SetDay

GetDay

EnableDayCycle

DisableDayCycle

### 

Todo List:
- Refactor the Day/Night system a class:
  - handle the timing, allow getting/setting the time (convert from 24 hour units to 0 to 1 scale and back), setting the time scale (rate at which time flows) getting the time phase, and allow enabling/disabling time flow (and tie to event commands + raise events per hour)
  - Implement day of the week!
  - handle loading the LUT textures for interpolating and provide a method to retrieve it
  - Try and figure out what's causing DirectX to bug out
  - Decide on how long a day should be in-game (don't forget that City is 60* faster) (1 hour in albion, 1 min in city?)
- Refactor (remove?) the renderer classes:
  - Remove CanvasSize (pass it to the Camera instead, see below), VirtualRenderTarget2D and any other uneeded classes
  - Refactor ColorGrading to resemble TextureAverager
    - Rename from ColorGrading to ColorGrader 
  - Change the Camera into a standard class and add a zoom factor which scales to meet the boundary size
    - problem: this means every pixel gets rendered which makes shaders much slower. Maybe still render to a small buffer and do something like get the backbuffer after everything is done instead of using rendertargets?
    - Maybe make a Renderer class which anything can "request" a render buffer (which copies what had been rendered up to that point to a new buffer and then provides a rendertarget it can use) and "relinquish" to re-render what was saved and let everything carry on
  - Change the renderer to be provided a minimum and maximum resolution to be constrained to so that resolutions that are too weird don't cause major issues?
- Graphics Pass continues:
  - Lighting/Shadow, figure out an elegant solution, maybe copy what Graveyard Keeper does with its fake lighting/shadows?
  - Add map-specific and zone-specific colour grading support (maybe support a Plugin Command which can change the global LUT?)
  - Add a *very* soft vignette
  - Wind deforming shader (create a duplicated texture with a mask determining how strongly a texel is affected?)
  - Falling particle leaves
  - Shader for water (https://forums.tigsource.com/index.php?topic=40539.msg1104986#msg1104986)
    - in the map post-processor, maybe find tiles that use the water tiles and copy from tiles above + flip upside-down to be rendered in a special water layer?
- Plugin System:
  - Change the Triggers (EventType.cs) to an enum with an optional parameter for a string so that the dynamic stuff (or events in general) can be less typo prone
  - Implement missing attributes (triggers/conditions), events and commands
  - Make dynamic triggers raise plain events too (e.g. "INTERACT_AREA_ENTER:example" also raises "INTERACT_AREA_ENTER". Maybe do this automatically as part of the event notifier by checking for colons?)
  - Make every IEnumerator function return a wrapped Coroutine instead (and change docs to reflect that)
- Input System:
  - Refactor STakeScreenshot into a plugin
  - Refactor SHandleFullscreen into a plugin
- Generic:
  - Look at what existing logic can be refactored into a plugin
  - Redo how screens handle game objects and systems?
  - Change every `(float)gameTime.ELapsedTime.TotalSeconds` to use the GetElapsedSeconds() extension method.
  - Go through all constructors and fix them to only require Service (when any of them request for something that Service provides) and create private fields which hold what they ask for (and handle that in the constructor)
  - Remove all Service private fields