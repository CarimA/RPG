-- event conditions

function Flag(f, s)
  return function()
    return _Flag(f, (s ~= false))
  end
end

function Var(v, e, o)
  return function()
    return _Var(v, e, o)
  end
end

-- event trigger

function Trigger(g, d)
  return function()
    return _Trigger(g, d)
  end
end

-- general helpers for coroutine-centric functions (the _ variants are implemented in-engine)

function Move(g, t, s)
  while _Move(g, t, s) do
    coroutine.yield()
  end
end
                
function Say(n, d)
  while _Say(n, d) do
    coroutine.yield()
  end
end

function Wait(t)
  while t > 0 do
    coroutine.yield()
    t = t - GetDeltaTime()
  end
end