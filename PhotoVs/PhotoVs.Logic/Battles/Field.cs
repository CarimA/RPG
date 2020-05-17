using Microsoft.Xna.Framework;
using PhotoVs.Utils.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PhotoVs.Logic.Battles
{
    public class Field
    {
        private BattlePhoton _activePhoton;
        private Grid<Hazard> _hazards;

        private readonly Grid<BattlePhoton> _photons;

        private readonly Random _random;
        private int _turn;

        private Queue<BattlePhoton> _turnOrder;

        public Field()
        {
            _random = new Random();

            _photons = new Grid<BattlePhoton>();
            _hazards = new Grid<Hazard>();

            _turnOrder = new Queue<BattlePhoton>();
            _turn = 0;
        }

        // raised when a photon is attempting to deal damage.
        // used to apply any modifiers invoked by the photon itself.
        private event EventHandler OnDealDamage;

        // raised when a photon is attempting to take damage.
        // used to apply any modifiers invoked by the photon itself
        // or reflect an attack
        private event EventHandler OnTakeDamage;

        // raised when a photon uses an action. can modify the
        // amount of stamina taken
        private event EventHandler OnDeductStamina;

        // raised when a stat is attempting to increase.
        private event EventHandler OnModifyBoost;

        // raised when a stat is attempting to decrease.
        private event EventHandler OnModifyReduction;

        // raised when a status effect is attempting to be
        // applied. 
        private event EventHandler OnSetStatus;

        public void DetermineTurnOrder()
        {
            // sort by speed and then priority,
            // so that prioritised photons will always go first
            // (but speed still kicks in)
            var photons = _photons
                .ToList()
                .OrderBy(photon => photon.Speed)
                .OrderBy(photon => photon.Priority)
                .ToList();

            // if there's a speed tie, coinflip on who gets to go
            // first
            photons.Sort((a, b) =>
            {
                if (a.Speed.CompareTo(b.Speed) == 0)
                    return _random.NextDouble() < 0.5
                        ? -1
                        : 1;
                return 0;
            });

            // remove anything that currently cannot attack
            photons.RemoveAll(photon => !photon.CanAttack);

            _turnOrder = new Queue<BattlePhoton>(photons);
        }

        public void NextPhoton()
        {
            // todo: litter with coroutines to hang up execution

            // check if there's anybody left in this turn
            if (_turnOrder.Count > 0)
            {
                // call a photon turn finish event for the photon
                // that just finished its turn

                // set the active photon
                _activePhoton = _turnOrder.Dequeue();

                // call a photon turn started event for the photon
                // that just started its turn
            }
            else
            {
                // call a turn finish event

                _turn++;
                DetermineTurnOrder();

                // call a turn start event

                NextPhoton();
            }
        }

        public void Update(GameTime gameTime)
        {
            if (_activePhoton.HasMoved)
                NextPhoton();
        }
    }
}