﻿namespace PhotoVs.Models.FSM
{
    public interface IState
    {
        bool IsBlocking { get; set; }

        void Enter(params object[] args);
        void Exit();

        void Resume();

        void Suspend();
    }
}