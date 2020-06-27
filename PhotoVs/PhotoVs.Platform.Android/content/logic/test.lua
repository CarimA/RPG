sub(Events.GameStart, function ()
    push_scene('controller')
end, run_once)

tick = 0

sub(Events.InteractAreaEnter, 'example_event', function ()
    tick = get_time()
end)

sub(Events.InteractAreaExit, 'example_event', function ()
    tick -= get_time()
    lock()
    say('debugger', 'it took {# Yellow}' + tick + ' ticks{/#} to walk through.')
    move($('Player'), vec2.new(0, 0), 100)
    unlock()
end)


to implement:
    move
    vec2