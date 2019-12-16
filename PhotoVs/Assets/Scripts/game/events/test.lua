events['example_event'] = {
	on_enter = function ()
		log.info('enter')
		lock_mov_while(function ()
			dialogue('test name', 'hello world!') -- get_text('intro'))
		end)
	end,

	on_exit = function ()
		camera.shake(2, 0.5)
		log.info('exit')
	end,

	on_action = function ()
		log.info('action')
	end,

	on_stand = function ()
		log.info('stand')
	end,

	on_walk = function ()
		log.info('walk')
	end,

	on_run = function ()
		log.info('run')
	end
}