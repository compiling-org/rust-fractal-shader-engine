//! Easing Functions for Smooth Animations
//!
//! This module provides various easing functions for creating smooth
//! and natural-looking animations.

/// Easing function trait for custom easing implementations
pub trait EasingFunction {
    fn apply(&self, t: f32) -> f32;
}

/// Built-in easing functions
#[derive(Debug, Clone)]
pub enum EasingFunction {
    Linear,
    EaseInQuad,
    EaseOutQuad,
    EaseInOutQuad,
    EaseInCubic,
    EaseOutCubic,
    EaseInOutCubic,
    EaseInQuart,
    EaseOutQuart,
    EaseInOutQuart,
    EaseInQuint,
    EaseOutQuint,
    EaseInOutQuint,
    EaseInSine,
    EaseOutSine,
    EaseInOutSine,
    EaseInCirc,
    EaseOutCirc,
    EaseInOutCirc,
    EaseInExpo,
    EaseOutExpo,
    EaseInOutExpo,
    EaseInBack,
    EaseOutBack,
    EaseInOutBack,
    EaseInElastic,
    EaseOutElastic,
    EaseInOutElastic,
    EaseInBounce,
    EaseOutBounce,
    EaseInOutBounce,
    Smooth,
    Custom(Box<dyn EasingFunction>),
}

impl EasingFunction {
    pub fn apply(&self, t: f32) -> f32 {
        // Clamp input between 0 and 1
        let t = t.clamp(0.0, 1.0);

        match self {
            EasingFunction::Linear => t,
            EasingFunction::EaseInQuad => t * t,
            EasingFunction::EaseOutQuad => 1.0 - (1.0 - t) * (1.0 - t),
            EasingFunction::EaseInOutQuad => {
                if t < 0.5 {
                    2.0 * t * t
                } else {
                    1.0 - 2.0 * (1.0 - t) * (1.0 - t)
                }
            }
            EasingFunction::EaseInCubic => t * t * t,
            EasingFunction::EaseOutCubic => 1.0 - (1.0 - t).powi(3),
            EasingFunction::EaseInOutCubic => {
                if t < 0.5 {
                    4.0 * t * t * t
                } else {
                    1.0 - 4.0 * (1.0 - t).powi(3)
                }
            }
            EasingFunction::EaseInQuart => t * t * t * t,
            EasingFunction::EaseOutQuart => 1.0 - (1.0 - t).powi(4),
            EasingFunction::EaseInOutQuart => {
                if t < 0.5 {
                    8.0 * t * t * t * t
                } else {
                    1.0 - 8.0 * (1.0 - t).powi(4)
                }
            }
            EasingFunction::EaseInQuint => t * t * t * t * t,
            EasingFunction::EaseOutQuint => 1.0 - (1.0 - t).powi(5),
            EasingFunction::EaseInOutQuint => {
                if t < 0.5 {
                    16.0 * t * t * t * t * t
                } else {
                    1.0 - 16.0 * (1.0 - t).powi(5)
                }
            }
            EasingFunction::EaseInSine => 1.0 - (t * std::f32::consts::PI / 2.0).cos(),
            EasingFunction::EaseOutSine => (t * std::f32::consts::PI / 2.0).sin(),
            EasingFunction::EaseInOutSine => -(std::f32::consts::PI * t / 2.0).cos(),
            EasingFunction::EaseInCirc => 1.0 - (1.0 - t * t).sqrt(),
            EasingFunction::EaseOutCirc => (1.0 - (1.0 - t) * (1.0 - t)).sqrt(),
            EasingFunction::EaseInOutCirc => {
                if t < 0.5 {
                    (1.0 - (1.0 - 2.0 * t * 2.0 * t).sqrt()) * 0.5
                } else {
                    (1.0 - (1.0 - (1.0 - 2.0 * t) * (1.0 - 2.0 * t)).sqrt()) * 0.5 + 0.5
                }
            }
            EasingFunction::EaseInExpo => {
                if t == 0.0 {
                    0.0
                } else {
                    2.0_f32.powf(10.0 * (t - 1.0))
                }
            }
            EasingFunction::EaseOutExpo => {
                if t == 1.0 {
                    1.0
                } else {
                    1.0 - 2.0_f32.powf(-10.0 * t)
                }
            }
            EasingFunction::EaseInOutExpo => {
                if t == 0.0 || t == 1.0 {
                    return t;
                }
                if t < 0.5 {
                    2.0_f32.powf(10.0 * (2.0 * t - 1.0)) * 0.5
                } else {
                    (2.0 - 2.0_f32.powf(-10.0 * (2.0 * t - 1.0))) * 0.5 + 0.5
                }
            }
            EasingFunction::EaseInBack => {
                let c1 = 1.70158;
                let c3 = c1 + 1.0;
                c3 * t * t * t - c1 * t * t
            }
            EasingFunction::EaseOutBack => {
                let c1 = 1.70158;
                let c3 = c1 + 1.0;
                1.0 + c3 * (t - 1.0).powi(3) + c1 * (t - 1.0).powi(2)
            }
            EasingFunction::EaseInOutBack => {
                let c1 = 1.70158;
                let c2 = c1 * 1.525;
                if t < 0.5 {
                    (2.0 * t).powi(2) * ((c2 + 1.0) * 2.0 * t - c2) * 0.5
                } else {
                    ((2.0 * t - 2.0).powi(2) * ((c2 + 1.0) * (2.0 * t - 2.0) + c2) + 2.0) * 0.5
                }
            }
            EasingFunction::EaseInElastic => {
                let c4 = (2.0 * std::f32::consts::PI) / 3.0;
                if t == 0.0 {
                    0.0
                } else if t == 1.0 {
                    1.0
                } else {
                    -2.0_f32.powf(10.0 * t - 10.0) * ((t * 10.0 - 10.75) * c4).sin()
                }
            }
            EasingFunction::EaseOutElastic => {
                let c4 = (2.0 * std::f32::consts::PI) / 3.0;
                if t == 0.0 {
                    0.0
                } else if t == 1.0 {
                    1.0
                } else {
                    2.0_f32.powf(-10.0 * t) * ((t * 10.0 - 0.75) * c4).sin() + 1.0
                }
            }
            EasingFunction::EaseInOutElastic => {
                let c5 = (2.0 * std::f32::consts::PI) / 4.5;
                if t == 0.0 || t == 1.0 {
                    t
                } else if t < 0.5 {
                    -(2.0_f32.powf(10.0 * (2.0 * t - 1.0)) * ((2.0 * t * 10.0 - 11.125) * c5).sin()) * 0.5
                } else {
                    2.0_f32.powf(-10.0 * (2.0 * t - 1.0)) * ((2.0 * t * 10.0 - 11.125) * c5).sin() * 0.5 + 1.0
                }
            }
            EasingFunction::EaseInBounce => 1.0 - Self::ease_out_bounce(1.0 - t),
            EasingFunction::EaseOutBounce => Self::ease_out_bounce(t),
            EasingFunction::EaseInOutBounce => {
                if t < 0.5 {
                    (1.0 - Self::ease_out_bounce(1.0 - 2.0 * t)) * 0.5
                } else {
                    (1.0 + Self::ease_out_bounce(2.0 * t - 1.0)) * 0.5
                }
            }
            EasingFunction::Smooth => {
                // Smooth interpolation using sigmoid-like function
                let s = 3.0;
                t.powi(s) / (t.powi(s) + (1.0 - t).powi(s))
            }
            EasingFunction::Custom(easing) => easing.apply(t),
        }
    }

    /// Helper function for bounce easing
    fn ease_out_bounce(t: f32) -> f32 {
        let n1 = 7.5625;
        let d1 = 2.75;

        if t < 1.0 / d1 {
            n1 * t * t
        } else if t < 2.0 / d1 {
            let t = t - 1.5 / d1;
            n1 * t * t + 0.75
        } else if t < 2.5 / d1 {
            let t = t - 2.25 / d1;
            n1 * t * t + 0.9375
        } else {
            let t = t - 2.625 / d1;
            n1 * t * t + 0.984375
        }
    }
}

impl Default for EasingFunction {
    fn default() -> Self {
        EasingFunction::Smooth
    }
}

/// Easing function factory for creating common easing patterns
pub struct EasingFactory;

impl EasingFactory {
    /// Create a custom easing function from a mathematical function
    pub fn custom<F: Fn(f32) -> f32 + 'static>(func: F) -> EasingFunction {
        EasingFunction::Custom(Box::new(CustomEasing(func)))
    }

    /// Create bounce easing with specified strength
    pub fn bounce(strength: f32) -> EasingFunction {
        let func = move |t: f32| {
            let strength = strength.max(0.1);
            EasingFunction::ease_out_bounce(t * strength) / strength
        };
        Self::custom(func)
    }

    /// Create elastic easing with specified tension
    pub fn elastic(tension: f32, damping: f32) -> EasingFunction {
        let func = move |t: f32| {
            let tension = tension.max(0.1);
            let damping = damping.max(0.1);
            if t == 0.0 || t == 1.0 {
                return t;
            }
            let p = damping.max(0.001);
            let s = tension / (2.0 * std::f32::consts::PI) * (1.0 / p).asin();
            -(2.0_f32.powf(10.0 * (t - 1.0)) * ((t - 1.0 - s / (2.0 * std::f32::consts::PI)) * tension * 2.0 / p).sin()) * 0.5 + 0.5
        };
        Self::custom(func)
    }

    /// Create ease-in-out with custom curve points
    pub fn bezier(p0: f32, p1: f32, p2: f32, p3: f32) -> EasingFunction {
        let func = move |t: f32| {
            // Cubic Bezier curve evaluation
            let u = 1.0 - t;
            let tt = t * t;
            let uu = u * u;
            let uuu = uu * u;
            let ttt = tt * t;

            (uuu * p0) + (3.0 * uu * t * p1) + (3.0 * u * tt * p2) + (ttt * p3)
        };
        Self::custom(func)
    }

    /// Create spring-like easing
    pub fn spring(tension: f32, friction: f32) -> EasingFunction {
        let func = move |t: f32| {
            let tension = tension.max(0.1);
            let friction = friction.max(0.1);
            // Simplified spring simulation
            let v = -tension * t + friction * (1.0 - (t * 2.0 - 1.0).abs());
            t + v * 0.1
        };
        Self::custom(func)
    }
}

/// Custom easing function wrapper
struct CustomEasing<F: Fn(f32) -> f32>(F);

impl<F: Fn(f32) -> f32> EasingFunction for CustomEasing<F> {
    fn apply(&self, t: f32) -> f32 {
        self.0(t)
    }
}

/// Preset easing functions for common animation patterns
pub mod presets {
    use super::*;

    /// Very smooth easing for organic motion
    pub const SMOOTH: EasingFunction = EasingFunction::Smooth;

    /// Linear easing (no acceleration/deceleration)
    pub const LINEAR: EasingFunction = EasingFunction::Linear;

    /// Quick start, gradual slowdown
    pub const EASE_OUT: EasingFunction = EasingFunction::EaseOutCubic;

    /// Gradual start, quick end
    pub const EASE_IN: EasingFunction = EasingFunction::EaseInCubic;

    /// Both start and end gradual
    pub const EASE_IN_OUT: EasingFunction = EasingFunction::EaseInOutCubic;

    /// Quick start with elastic bounce
    pub const BOUNCE: EasingFunction = EasingFunction::EaseOutBounce;

    /// Elastic motion
    pub const ELASTIC: EasingFunction = EasingFunction::EaseOutElastic;

    /// Back motion (overshoot and return)
    pub const BACK: EasingFunction = EasingFunction::EaseOutBack;

    /// Exponential growth/decay
    pub const EXPONENTIAL: EasingFunction = EasingFunction::EaseOutExpo;

    /// Sine wave motion
    pub const SINE: EasingFunction = EasingFunction::EaseInOutSine;

    /// Circ motion (ease-in for angular motion)
    pub const CIRC: EasingFunction = EasingFunction::EaseInCirc;

    /// Quartic curves for power animation
    pub const QUART_IN_OUT: EasingFunction = EasingFunction::EaseInOutQuart;

    /// Quintic curves for strong acceleration
    pub const QUINT_IN_OUT: EasingFunction = EasingFunction::EaseInOutQuint;
}