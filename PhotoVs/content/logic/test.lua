Subscribe(Events.GameStart, function ()
    PushScene('controller')
end, RunOnce)

tick = 0

Subscribe(Events.InteractAreaEnter, 'example_event', function ()
    tick = GetTotalTime()
end)

Subscribe(Events.InteractAreaExit, 'example_event', function ()
    tick = GetTotalTime() - tick
    Lock()
    Wait(3)
    Say('debugger', 'it took {# Yellow}' .. tick .. ' ticks{/#} to walk through.')
    Move(Player(), Vector2(0, 0), 100)
    Unlock()
end)