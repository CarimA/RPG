-- waits for the specified amount of seconds before proceeding
function wait(seconds)
  local total = seconds
  while total >= 0 do
    local time = coroutine.yield()
    total = total - time
  end
end

letterbox_y = 0

-- turns on black bars then proceeds
function enable_letterbox()
  local max_duration = 0.25
  local duration = max_duration
  local speed = 60
  local target_y = ((1080 / 3) - 274) / 2
  while duration >= 0 do
    local time = coroutine.yield()
    duration = duration - time
    local tween = target_y * (1 - (duration / max_duration))
    letterbox_y = tween
  end
  letterbox_y = target_y
end

-- turns off black bars then proceeds
function disable_letterbox()
  local max_duration = 0.25
  local duration = max_duration
  local speed = 60
  local target_y = ((1080 / 3) - 274) / 2
  while duration >= 0 do
    local time = coroutine.yield()
    duration = duration - time
    local tween = target_y * (duration / max_duration)
    letterbox_y = tween
  end
  letterbox_y = 0
end

-- brings up text input screen and waits until the player submits something then proceeds
function get_input(title, max_length, default_text)
  if title == nil then
    title = ''
  end

  if max_length == nil then
    max_length = 14
  end

  if default_text == nil then
    default_text = ''
  end

  push_get_input_screen(title, max_length, default_text)
  while text_input == nil do
	  coroutine.yield()
  end
  local temp = text_input
  pop_screen()
  text_input = nil
  return temp
end

function _(name, message)
  if name == nil then
    name = ''
  end

  if message == nil then
    return
  end

  push_show_dialogue_screen(name, message)
  while dialogue_complete == nil do
    coroutine.yield()
  end
  dialogue_complete = nil
  pop_screen()
end