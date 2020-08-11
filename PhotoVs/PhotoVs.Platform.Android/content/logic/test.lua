Subscribe {
  Triggers = { 'GameStart' },
  RunOnce = true,
  Event = function ()
    --PushScene('controller')
    --Notify(Events.InteractAreaExit, 'example_event')
  end
}

tick = 0

Subscribe {
  Triggers = { 'InteractAreaEnter:example_event' },
  Event = function ()
    tick = GetTotalTime()
  end
}

Subscribe {
  Triggers = { 'InteractAreaExit:example_event' },
  Event = function ()
    tick = GetTotalTime() - tick
    --Lock()
    --Wait(3)
    --Say('debugger', 'it took {# Yellow}' .. tick .. ' ticks{/#} to walk through.')
    --Move(Player(), Vector2(0, 0), 100)
    --Unlock()
  end
}