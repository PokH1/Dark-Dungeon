#![no_std]

use sails_rs::prelude::*;
pub mod services;

use services::service::Service;

pub struct Program;

#[program]
impl Program {
    /// Initializes the medieval character. Must be called once.
    pub fn new(strength: u128, speed: u128, max_health: u128) -> Self {
        Service::seed(strength, speed, max_health);
        Self
    }

    #[route("Service")]
    pub fn service(&self) -> Service {
        Service::new()
    }
}
