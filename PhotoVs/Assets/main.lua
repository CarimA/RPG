Coroutine = import('coroutines.lua')


function new_game()

function update()
  Coroutines:resume_all(dt)
end



------------------ coroutine.lua

local co = function()
  local this = {}
  local _coroutines = {}

  function this:spawn(func)
    local co = coroutine.create(func)
    coroutine.resume(co, dt)
    this:add(co)
  end

  function this:add(co)
    table.insert(_coroutines, co)
  end

  function this:remove(index)
    table.remove(_coroutines, index)
  end

  function this:resume(index, dt)
    local co = _coroutines[index]
    if (coroutine.status(co) == 'dead') then
      this:remove(index)
    else
      local ok, err = coroutine.resume(co, dt)
      if not ok then
        error("Error in coroutine: " .. err)
      end
    end
  end

  function this:resume_all(dt)
    for index, item in ipairs(_coroutines) do
      this:resume(index, dt)
    end
  end

  function this:wait(seconds)
    local total = seconds
    while total >= 0 do
      local time = coroutine.yield()
      total = total - time
    end
  end

  function this:enable_letterbox()
    local max_duration = 0.25
    local duration = max_duration
    local target_y = ((1080 / 3) - 274) / 2
    while duration >= 0 do
      local time = coroutine.yield()
      duration = duration - time
      local tween = target_y * (1 - (duration / max_duration))
      set_letterbox(tween)
    end
    set_letterbox(target_y)
  end

  function this:disable_letterbox()
    local max_duration = 0.25
    local duration = max_duration
    local target_y = ((1080 / 3) - 274) / 2
    while duration >= 0 do
      local time = coroutine.yield()
      duration = duration - time
      local tween = target_y * (duration / max_duration)
      set_letterbox(tween)
    end
    set_letterbox(0)
  end

  function this:letterbox(fn)
    this:enable_letterbox()
    fn()
    this:disable_letterbox()
  end

  function this:get_text_input(title, max_length, default_text)
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

  function this:dialogue(name, message)
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
  
  return this
end

return co()


---- action.lua

local _ = {}

EXAMPLE_FUNC = Action {

}

return _