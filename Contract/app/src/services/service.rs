
#![no_std]
#![allow(static_mut_refs)]

use sails_rs::{
    prelude::*,
    gstd::{msg, exec},
    collections::HashMap,
};

pub static mut CHARACTER_STATE: Option<CharacterState> = None;

/// Struct for the medieval character state
#[derive(Debug, Clone, Default)]
pub struct CharacterState {
    pub owner: ActorId,
    pub strength: u128,
    pub speed: u128,
    pub health: u128,
    pub max_health: u128,
    pub actions_log: Vec<ActionRecord>,
}

/// Struct for logging actions
#[derive(Debug, Clone, Encode, Decode, TypeInfo)]
#[codec(crate = sails_rs::scale_codec)]
#[scale_info(crate = sails_rs::scale_info)]
pub struct ActionRecord {
    pub timestamp: u64,
    pub kind: ActionKind,
}

#[derive(Debug, Clone, Encode, Decode, TypeInfo, PartialEq, Eq)]
#[codec(crate = sails_rs::scale_codec)]
#[scale_info(crate = sails_rs::scale_info)]
pub enum ActionKind {
    Run,
    Attack { target: ActorId, amount: u128 },
    RestoreHealth { amount: u128 },
}

impl CharacterState {
    pub fn init(strength: u128, speed: u128, max_health: u128) {
        let sender = msg::source();
        unsafe {
            CHARACTER_STATE = Some(Self {
                owner: sender,
                strength,
                speed,
                health: max_health,
                max_health,
                actions_log: Vec::new(),
            })
        }
    }
    pub fn state_mut() -> &'static mut CharacterState {
        let s = unsafe { CHARACTER_STATE.as_mut() };
        debug_assert!(s.is_some(), "State not initialized");
        unsafe { s.unwrap_unchecked() }
    }
    pub fn state_ref() -> &'static CharacterState {
        let s = unsafe { CHARACTER_STATE.as_ref() };
        debug_assert!(s.is_some(), "State not initialized");
        unsafe { s.unwrap_unchecked() }
    }
}

/// Events emitted by the contract
#[derive(Debug, Encode, Decode, TypeInfo)]
#[codec(crate = sails_rs::scale_codec)]
#[scale_info(crate = sails_rs::scale_info)]
pub enum CharacterEvent {
    Ran { speed: u128 },
    Attacked { target: ActorId, damage: u128 },
    RestoredHealth { amount: u128, health: u128 },
}

#[derive(Debug, Encode, Decode, TypeInfo, Clone)]
#[codec(crate = sails_rs::scale_codec)]
#[scale_info(crate = sails_rs::scale_info)]
pub struct IoCharacterState {
    pub owner: ActorId,
    pub strength: u128,
    pub speed: u128,
    pub health: u128,
    pub max_health: u128,
    pub actions_log: Vec<ActionRecord>,
}

impl From<CharacterState> for IoCharacterState {
    fn from(s: CharacterState) -> Self {
        IoCharacterState {
            owner: s.owner,
            strength: s.strength,
            speed: s.speed,
            health: s.health,
            max_health: s.max_health,
            actions_log: s.actions_log,
        }
    }
}

#[derive(Default)]
pub struct Service;

impl Service {
    /// Initializes the state of the character; call only once
    pub fn seed(strength: u128, speed: u128, max_health: u128) {
        if strength == 0 || speed == 0 || max_health == 0 {
            panic!("Attributes must be greater than zero");
        }
        CharacterState::init(strength, speed, max_health);
    }
}

#[sails_rs::service(events = CharacterEvent)]
impl Service {
    pub fn new() -> Self { Self }

    /// Action: run (uses speed)
    pub fn run(&mut self) -> CharacterEvent {
        let state = CharacterState::state_mut();
        let caller = msg::source();
        assert_eq!(caller, state.owner, "Only the owner can run");
        let timestamp = exec::block_timestamp();
        state.actions_log.push(ActionRecord {
            timestamp,
            kind: ActionKind::Run,
        });
        self.emit_event(CharacterEvent::Ran { speed: state.speed }).expect("Notification failed");
        CharacterEvent::Ran { speed: state.speed }
    }

    /// Action: attack (uses strength, requires specifying a target)
    pub fn attack(&mut self, target: ActorId) -> CharacterEvent {
        let state = CharacterState::state_mut();
        let caller = msg::source();
        assert_eq!(caller, state.owner, "Only the owner can attack");
        if state.strength == 0 {
            panic!("Strength is zero, can't attack");
        }
        let timestamp = exec::block_timestamp();
        state.actions_log.push(ActionRecord {
            timestamp,
            kind: ActionKind::Attack { target, amount: state.strength },
        });
        self.emit_event(CharacterEvent::Attacked { target, damage: state.strength }).expect("Notification failed");
        CharacterEvent::Attacked { target, damage: state.strength }
    }

    /// Action: restore health (restore a specified amount)
    pub fn restore_health(&mut self, amount: u128) -> CharacterEvent {
        if amount == 0 {
            panic!("Restore amount must be greater than zero");
        }
        let state = CharacterState::state_mut();
        let caller = msg::source();
        assert_eq!(caller, state.owner, "Only the owner can restore health");
        let new_health = core::cmp::min(state.health.saturating_add(amount), state.max_health);
        let restored = new_health.saturating_sub(state.health);
        state.health = new_health;
        let timestamp = exec::block_timestamp();
        state.actions_log.push(ActionRecord {
            timestamp,
            kind: ActionKind::RestoreHealth { amount: restored },
        });
        self.emit_event(CharacterEvent::RestoredHealth { amount: restored, health: state.health }).expect("Notification failed");
        CharacterEvent::RestoredHealth { amount: restored, health: state.health }
    }

    /// Query: get character's strength
    pub fn query_strength(&self) -> u128 {
        CharacterState::state_ref().strength
    }

    /// Query: get character's speed
    pub fn query_speed(&self) -> u128 {
        CharacterState::state_ref().speed
    }

    /// Query: get character's health
    pub fn query_health(&self) -> u128 {
        CharacterState::state_ref().health
    }

    /// Query: get character's full state
    pub fn query_state(&self) -> IoCharacterState {
        CharacterState::state_ref().clone().into()
    }
}
