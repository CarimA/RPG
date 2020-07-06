Subscribe {
  Triggers = { Events.GameStart },
  RunOnce = true,
  Event = function ()
    PushScene('controller')
  end
}

tick = 0

Subscribe {
  Triggers = { Trigger(Events.InteractAreaEnter, 'example_event') },
  Event = function ()
    tick = GetTotalTime()
  end
}

Subscribe {
  Triggers = { Trigger(Events.InteractAreaExit, 'example_event') },
  Event = function ()
    tick = GetTotalTime() - tick
    Lock()
    Wait(3)
    Say('debugger', 'it took {# Yellow}' .. tick .. ' ticks{/#} to walk through.')
    Move(Player(), Vector2(0, 0), 100)
    Unlock()
  end
}