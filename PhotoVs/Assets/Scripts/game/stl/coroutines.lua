local function Coroutines()
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

  -- helper functions that can be used to halt a script in place based on condition
  function this:wait_until(predicate)
    while not predicate() do
      coroutine.yield()
    end  
  end
  
  function this:wait_while(predicate)
    while predicate() do
      coroutine.yield()
    end
  end

  function this:wait_seconds(secs)
    local time = secs
    while (time > 0) do
      local dt = coroutine.yield()
      time = time - dt
    end
  end

  return this
end

coroutines = Coroutines()
events = {}

function lock_mov_while(func)
  player.lock_movement()
  func()
  player.unlock_movement()
end

function dialogue(name, message)
  if name == nil then
    return
  end

  if message == nil then
    return
  end

  screens.dialogue(name, message)
  while dialogue_complete == nil do
    coroutine.yield()
  end
  dialogue_complete = nil
  screens.pop()
end
