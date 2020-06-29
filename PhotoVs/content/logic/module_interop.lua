function flag(f, s)
  return function()
    return _flag(f, (s ~= false))
  end
end

function var(v, e, o)
  return function()
    return _var(v, e, o)
  end
end

function trigger(g, d)
  return function()
    return _trigger(g, d)
  end
end

function move(g, t, s)
  while _move(g, t, s) do
    coroutine.yield()
  end
end
                
function say(n, d)
  while _say(n, d) do
    coroutine.yield()
  end
end
