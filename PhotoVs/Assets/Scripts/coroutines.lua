scripts = {}
scripts['example_func'] = {}

scripts['example_func']['on_enter'] = function ()
  lock_movement()
  enable_letterbox()
  play_bgm("main1.ogg")

  local val = get_input("What's your name?")
  _(val, "<red>Blah</red> <sin 5>blah blah blah blah blah blah <sin 4 2><blue>blah</blue></sin> blah blah</sin> blah blah blah blah blah <sin 5>blah blah blah blah</sin> more filler crap blah blah blah blah blah blah")

  unlock_movement()

  set_flag("name", val)
  wait(4)
  save()

  log(val)
end

scripts['example_func']['on_exit'] = function ()
  disable_letterbox()
  play_bgm("outlaws lullaby.mp3")
  stop_bgm()
end

--scripts['example_func']['on_action'] = function ()
  --get_input("title", 10, "")
  --log("was there something to do?")  
  --wait(0.25)
  --log("oh")
--end


