# Scripting & Game Events

Photeus handles logic by loading external Lua scripts from `content\logic` and `data\mods`. Scripts that produce runtime errors will not be loaded.

While the API that is exposed to the scripts is sandboxed, the scripts still have access to the full Lua API and so, caution should be taken when using scripts taken from unknown sources.

Scripts are almost entirely event driven, with each method having *conditions* and *triggers* to determine where and when the events will run. The host engine has an underlying event queue in which all subscriptions are automatically forwarded to. When an event is raised, if the conditions to run the method is met, the method will run.

# Writing & Distributing Your Own Mods

Scripts loaded from `data\mods` have access to exactly the same API as scripts loaded from `content\logic`, and can do exactly the same things (to the point that you *could* put your mods in `content\logic`, but please don't do that).

All scripts/data you write must be packaged into a single zip file and can dropped directly into `data\mods` for ease-of-installation and distribution. 

All zips must contain a `metadata.json` file which implements the following schema:

```json
{
  "name": "My Mod Name",
  "version": 1
}
```

Any zips that do not contain a `metadata.json` will not be loaded by the game.

At runtime, any non-script files inside the zip file will get extracted to `content\modfiles\{mod name}`, and when the assetloader loads data, it will check both the original location and all available mods, taking precedence with the file in `content\modfiles`.

If any mods cause conflicts by trying to override the same data, neither mod will be loaded and the player will be alerted of the conflict.

By default, all mods placed into `data\mods` are loaded, this is behaviour that I hope to change in the future through use of an in-game mod manager.

# General Code Style Stuff

(TODO) 

All methods and classes exposed to the scipting system use Pascal Case. 

Use 2 spaces for indenting.

# The Subscribe Method

The `Subscribe` method is the primary means of defining functionality within Photeus. It accepts a single table as an argument, and looks something like this:

```lua
Subscribe {
  Triggers = { 
    GameEvents.GameStart,
    Trigger(GameEvents.GameStart, 'hello')
  },
  Conditions = { 
    FlagCondition('this_flag', true),
    VarCondition('this_var', Equality.Equals, 100)
  },
  RunOnce = true,
  Event = function ()

  end
}
```

## Triggers

The `Triggers` key is a table treated as an array containing `GameEvents` enum values (see the **Game Events** section for a list of all enums) and/or any of the following method calls.

(todo)

|Method|Parameters|Description|Implemented?|
|-|-|-|:-:|
|`Trigger(gameEvent, delimiter)`|||✅|
|`AutoRunOverworld()`|||❌|
|`AutoRunBattle()`|||❌|

## Conditions

More than one condition can be defined for an event. All conditions need to be met before an event is raised.


|Method|Parameters|Description|Implemented?
|-|-|-|:-:|
|`FlagCondition(flag, value)`|**flag - string**: the name of the flag to check.<br><br>**value - string:** the value the flag must match with.|Adds a conditional check to the event subscription that checks whether the player's save data has a named flag and whether it matches the boolean value you specify.|✅|
|`VarCondtion(variable, equality, value)`|**variable - string:** the name of the variable to check.<br><br>**equality - Equality:** the equality comparison check that should be used.<br><br>**value - object:** the value the variable must match with.|Adds a conditional check to the event subscription that checks whether the player's save data has a named flag and whether it matches the (IComparable implementing) object you specify using the equality check specified in the Equality parameter.|✅|
|`StateCondition(state)`|**state - string:** the name of the state to check.|Adds a conditional check to the event subscription that checks whether the game is on the specified screen state.|❌|
|`TimePhaseCondition(timePhase)`|**timePhase - TimePhase:** the time phase to compare against.|Adds a conditional check to the event subscription that checks whether the in-game time matches the specified time phase.|❌|
|`DayCondition(day)`|**day - Day:** the day to compare against.|Adds a conditional check to the event subscription that checks whether the in-game day matches the specified day.|❌|

### Equality Enum Values
|Equality|Description|
|-|-|
|Equality.Equals|Flag value is equal to the value specified.|
|Equality.GreaterThan|Flag value is greater than the value specified.|
|Equality.GreaterThanOrEquals|Flag value is greater than or equal to the value specified.|
|Equality.LessThan|Flag value is less than the value specified.|
|Equality.LessThanOrEquals|Flag value is less than or equal to the value specified.|
|Equality.Not|Flag value is not equal to the value specified.|

### TimePhase Enum Values
|TimePhase|Description|
|-|-|
|TimePhase.Midnight|Between 23:00 and 01:00|
|TimePhase.EarlyMorning|Between 01:00 and 05:00|
|TimePhase.LateMorning|Between 05:00 and 11:00|
|TimePhase.Noon|Between 11:00 and 13:00|
|TimePhase.Afternoon|Between 13:00 and 17:00|
|TimePhase.EarlyEvening|Between 17:00 and 20:00|
|TimePhase.LateEvening|Between 20:00 and 23:00|

### Day Enum Values

|Day|Description|
|-|-|
|Day.Monday|Monday (do I really need to specify this.)|
|Day.Tuesday|Tuesday|
|Day.Wednesday|Wednesday|
|Day.Thursday|Thursday|
|Day.Friday|Friday|
|Day.Saturday|Saturday|
|Day.Sunday|Sunday|

## RunOnce

This is an optional key set to `false` by default. It can be either a `boolean` or a `string`. By setting it to `true`, that makes it so that the event will run once and then any triggers associated with it are automatiaclly unsubscribed. By setting it to a `string`, it becomes true by default but will also cause the given string to be set as a boolean flag in the player data, which will prevent the event from ever running again after the player saves their game.

```lua
Subscribe {
  Triggers = { GameEvents.GameStart },
  RunOnce = 'NeverRunAgain',
  Event = function ()
    -- some code here
  end
}
```

could be seen as shorthand for:

```lua
Subscribe {
  Triggers = { GameEvents.GameStart },
  Conditions = { FlagCondition('NeverRunAgain', false) }
  RunOnce = true,
  Event = function ()
    -- some code here
    SetFlag('NeverRunAgain', true)
  end
}
```

## Event

The function that actually runs when the event is raised. Can be provided with an optional variable that implements `GameEventArgs`.

# Game Events

## About Dynamic Triggers
Some game events will also raise a more detailed event that uses other data as a key to subscribe to more specific events. 

For example, `InteractAreaEnter` is an event that will raise for every instance of when the player enters a map interact zone boundary, but if you enter a map interact boundary named `example_event`, then the event `InteractAreaEnter:example_event` is also raised. 

This allows you to be more specific about which particular actions you may want to listen to without overwhelming the event notification system.

*(After all, it'd probably start slowing things down if *every* single map interact boundary entry event was raised and each of them checked against what they really wanted to listen for, right?)*

## List of all Game Events

|GameEvent|Type|Description of Event|GameEventArgs|Implemented?|
|-|-|-|-|:-:|
|GameEvent.GameStart|State|The game starts.|None|✅|
|GameEvent.ScreenPushed *(dynamic)*|State|The game has pushed a new state to the screen stack.|StateEventArgs|❌|
|GameEvent.ScreenPopped *(dynamic)*|State|The game has popped a state from the screen stack.|StateEventArgs|❌|
|GameEvent.ScreenChanged|State|The game has changed the state in the screen stack.|StateChangedEventArgs|❌|
|GameEvent.SaveLoaded|Save|Player loads a save file in the main menu.|SaveEventArgs|❌|
|GameEvent.SaveCreated|Save|Player creates a new save file in the main menu.|SaveEventArgs|❌|
|GameEvent.SaveChanged|Save|Player saves their game.|SaveEventArgs|❌|
|GameEvent.StepTaken|Movement|A game object takes a step corresponding to their animation.|GameObjectEventArgs|❌|
|GameEvent.MovementStateChanged|Movement|Player changes state between walking, running and standing.|MovementEventArgs|❌|
|GameEvent.EnteredZone *(dynamic)*|Zone|Player enters a new zone.|ZoneEventArgs|❌|
|GameEvent.ExitedZone *(dynamic)*|Zone|Player exits a zone.|ZoneEventArgs|❌|
|GameEvent.ZoneChanged|Zone|Player changes zone.|ZoneChangedEventArgs|❌|
|GameEvent.EnteredMap *(dynamic)*|Map|Player enters a new map.|MapEventArgs|❌|
|GameEvent.ExitedMap *(dynamic)*|Map|Player exits a map.|MapEventArgs|❌|
|GameEvents.MapChanged|Map|Player changes map.|MapChangedEventArgs|❌|
|GameEvents.InteractAction *(dynamic)*|Interaction|Player presses the interact action while standing on an interactible zone.|InteractEventArgs|✅|
|GameEvents.InteractAreaEnter *(dynamic)*|Interaction|Player has entered an interactible zone.|InteractEventArgs|✅|
|GameEvents.InteractAreaExit *(dynamic)*|Interaction|Player has exited an interactible zone.|InteractEventArgs|✅|
|GameEvents.InteractAreaStand *(dynamic)*|Interaction|Player is standing on an interactible zone.|InteractEventArgs|✅|
|GameEvents.InteractAreaWalk *(dynamic)*|Interaction|Player is walking on an interactible zone.|InteractEventArgs|✅|
|GameEvents.InteractAreaRun *(dynamic)*|Interaction|Player is running on an interactible zone.|InteractEventArgs|✅|
|GameEvents.Collision|Interaction|Player has collided with a solid object.|GameObjectEventArgs|✅|
|GameEvents.InputActionPressed|Input|Player has pressed an action.|InputEventArgs|❌|
|GameEvents.InputActionReleased|Input|Player has released an action.|InputEventArgs|❌|
|GameEvents.AssetLoaded *(dynamic)*|Assets|The game has loaded an asset into memory.|AssetEventArgs|❌|
|GameEvents.AssetUnloaded *(dynamic)*|Assets|The game has released an asset from memory.|AssetEventArgs|❌|
|GameEvents.AudioBgmPlayed *(dynamic)*|Audio|The game has started playing a new BGM track.|AudioEventArgs|❌|
|GameEvents.AudioBgmStopped|Audio|The game has stopped playing a BGM track.|None|❌|
|GameEvents.AudioSfxPlayed *(dynamic)*|Audio|The game has started playing a new sound effect.|AudioEventArgs|❌|
|GameEvents.DialogueNextCharacter|Dialogue|The game has moved on to the next character in dialogue.|DialogueEventArgs|❌|
|GameEvents.DialogueNextPage|Dialogue|The game has moved on to the next page in dialogue.|DialogueEventArgs|❌|
|GameEvents.DialogueFinished|Dialogue|The game has finished the current dialogue.|DialogueEventArgs|❌|
|GameEvents.TimePhaseChanged|DateTime|The time phase of the day has changed.|TimeEventArgs|❌|
|GameEvents.DayChanged|DateTime|The day of the week has changed.|DayEventArgs|❌|


## List of GameEventArg Classes 
(todo)

# Scripting API

|Method|Parameters|Description|Implemented?|
|-|-|-|:-:|
|`Wait(time)`|**time - number:** the length of time to halt the script in seconds.|Halts the script for a specified amount of time.|✅|




### `Spawn(IEnumerator) : IEnumerator`

### `Dialogue(string, string) : Dialogue`

### `LockMovement(Func<IEnumerator>) : IEnumerator`

### `Move(IGameObject, Vector2, float) : IEnumerator`

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



---
---
--- 

# general tasks todo list because i'm too lazy to put it elsewhere
- Refactor the Day/Night system a class:
  - handle the timing, allow getting/setting the time (convert from 24 hour units to 0 to 1 scale and back), setting the time scale (rate at which time flows) getting the time phase, and allow enabling/disabling time flow (and tie to event commands + raise events per hour)
  - Implement day of the week!
  - handle loading the LUT textures for interpolating and provide a method to retrieve it
  - Decide on how long a day should be in-game (don't forget that City is 60* faster) (1 hour in albion, 1 min in city?)
- Graphics Pass continues:
  - Lighting/Shadow, figure out an elegant solution, maybe copy what Graveyard Keeper does with its fake lighting/shadows?
  - Add map-specific and zone-specific colour grading support (maybe support a Plugin Command which can change the global LUT?)
  - Wind deforming shader (create a duplicated texture with a mask determining how strongly a texel is affected?)
    - could perhaps create a _materials, _normals and _height texture with different colour channels used for different things
  - Falling particle leaves
  - Shader for water
    - Refactor alllll of the code into a filter
      - Combine the shaders into one with different passes
      - Keep displacement as its own shader/filter, reuse with wind stuff.
    - Redesign the overworld renderer to accept a list of filters?
  - Wind trails
  - Fog? Maybe use the depth buffer?
  - Footstep dust (are we finally gonna make a player sprite? :ooo)
  - Fish shadows in water
- Plugin System:
  - Implement missing attributes (triggers/conditions), events and commands
  - Make every IEnumerator function return a wrapped Coroutine instead (and change docs to reflect that)
- Input System:
  - Refactor STakeScreenshot into a plugin
  - Refactor SHandleFullscreen into a plugin
- Generic:
  - Update the triggers to reflect the enums rather than constants
  - Look at what existing logic can be refactored into a plugin
  - Redo how screens handle game objects and systems?
  - Change every `(float)gameTime.ELapsedTime.TotalSeconds` to use the GetElapsedSeconds() extension method.
  - Go through all constructors and fix them to only require Service (when any of them request for something that Service provides) and create private fields which hold what they ask for (and handle that in the constructor)
  - Remove all Service private fields
  - Add a debug keybind for ScriptHost.Reset
  - Rewrite this doc to reflect Lua Way(TM)


  -- sub({ trigger(Events.GameStart, 'example_event'), trigger(Events.GameStart, 'example_2'), Events.GameStart, tell_them_to_use_a_controller)

Maybe have a list of filters that run in map properties?

Overworld can take a list of properties which chain into each other

Filter can have an inherited class MaterialFilter which also passes in the material map

Rearrange tileset once and for all (max 256pix width, 1024 height)

- get rid of the shadowed trees
- sort by type, leave a 2 tile gap between each section