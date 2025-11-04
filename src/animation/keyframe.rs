//! Keyframe Animation System
//!
//! This module provides keyframe-based animation for fractal parameters,
//! scene objects, and material properties.

use std::collections::HashMap;
use nalgebra::{Vector3, Vector4, Scalar};
use serde::{Deserialize, Serialize};

/// Unique identifier for animation tracks
pub type TrackId = u64;

/// Animation keyframe with time and value
#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct Keyframe<T> {
    pub time: f32,
    pub value: T,
    pub interpolation: InterpolationMode,
}

/// Interpolation modes for keyframes
#[derive(Debug, Clone, Copy, PartialEq, Serialize, Deserialize)]
pub enum InterpolationMode {
    Linear,
    Smooth,
    Step,
    EaseIn,
    EaseOut,
    EaseInOut,
}

/// Animation track containing keyframes for a specific property
#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct AnimationTrack<T> {
    pub id: TrackId,
    pub name: String,
    pub target_path: String, // e.g., "fractal.power", "transform.position.x"
    pub keyframes: Vec<Keyframe<T>>,
    pub enabled: bool,
}

/// Animation controller managing multiple tracks
#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct AnimationController {
    pub tracks: HashMap<TrackId, AnimationTrackType>,
    pub current_time: f32,
    pub duration: f32,
    pub playing: bool,
    pub loop_animation: bool,
}

#[derive(Debug, Clone, Serialize, Deserialize)]
pub enum AnimationTrackType {
    Float(AnimationTrack<f32>),
    Vec3(AnimationTrack<Vector3<f32>>),
    Vec4(AnimationTrack<Vector4<f32>>),
    Color(AnimationTrack<Vector3<f32>>),
}

impl AnimationController {
    /// Create new animation controller
    pub fn new() -> Self {
        Self {
            tracks: HashMap::new(),
            current_time: 0.0,
            duration: 10.0,
            playing: false,
            loop_animation: true,
        }
    }

    /// Add float track
    pub fn add_float_track(&mut self, name: &str, target_path: &str) -> TrackId {
        let track_id = generate_track_id();
        let track = AnimationTrack {
            id: track_id,
            name: name.to_string(),
            target_path: target_path.to_string(),
            keyframes: Vec::new(),
            enabled: true,
        };
        self.tracks.insert(track_id, AnimationTrackType::Float(track));
        track_id
    }

    /// Add vector3 track
    pub fn add_vec3_track(&mut self, name: &str, target_path: &str) -> TrackId {
        let track_id = generate_track_id();
        let track = AnimationTrack {
            id: track_id,
            name: name.to_string(),
            target_path: target_path.to_string(),
            keyframes: Vec::new(),
            enabled: true,
        };
        self.tracks.insert(track_id, AnimationTrackType::Vec3(track));
        track_id
    }

    /// Add keyframe to track
    pub fn add_keyframe(&mut self, track_id: TrackId, time: f32, value: AnimationValue, interpolation: InterpolationMode) {
        if let Some(track_type) = self.tracks.get_mut(&track_id) {
            match (track_type, value) {
                (AnimationTrackType::Float(track), AnimationValue::Float(val)) => {
                    track.keyframes.push(Keyframe { time, value: val, interpolation });
                    track.keyframes.sort_by(|a, b| a.time.partial_cmp(&b.time).unwrap());
                }
                (AnimationTrackType::Vec3(track), AnimationValue::Vec3(val)) => {
                    track.keyframes.push(Keyframe { time, value: val, interpolation });
                    track.keyframes.sort_by(|a, b| a.time.partial_cmp(&b.time).unwrap());
                }
                (AnimationTrackType::Vec4(track), AnimationValue::Vec4(val)) => {
                    track.keyframes.push(Keyframe { time, value: val, interpolation });
                    track.keyframes.sort_by(|a, b| a.time.partial_cmp(&b.time).unwrap());
                }
                (AnimationTrackType::Color(track), AnimationValue::Color(val)) => {
                    track.keyframes.push(Keyframe { time, value: val, interpolation });
                    track.keyframes.sort_by(|a, b| a.time.partial_cmp(&b.time).unwrap());
                }
                _ => {} // Type mismatch, ignore
            }
        }
    }

    /// Evaluate track at current time
    pub fn evaluate_track(&self, track_id: TrackId) -> Option<AnimationValue> {
        self.tracks.get(&track_id).and_then(|track_type| {
            match track_type {
                AnimationTrackType::Float(track) => {
                    self.evaluate_float_track(track).map(AnimationValue::Float)
                }
                AnimationTrackType::Vec3(track) => {
                    self.evaluate_vec3_track(track).map(AnimationValue::Vec3)
                }
                AnimationTrackType::Vec4(track) => {
                    self.evaluate_vec4_track(track).map(AnimationValue::Vec4)
                }
                AnimationTrackType::Color(track) => {
                    self.evaluate_vec3_track(track).map(AnimationValue::Color)
                }
            }
        })
    }

    /// Evaluate float track
    fn evaluate_float_track(&self, track: &AnimationTrack<f32>) -> Option<f32> {
        if track.keyframes.is_empty() {
            return None;
        }

        // Find keyframes around current time
        let current_time = if self.loop_animation {
            self.current_time % self.duration
        } else {
            self.current_time.min(self.duration)
        };

        // Find the two keyframes that bound current time
        let mut before_idx = None;
        let mut after_idx = None;

        for (i, keyframe) in track.keyframes.iter().enumerate() {
            if keyframe.time <= current_time {
                before_idx = Some(i);
            }
            if keyframe.time >= current_time {
                after_idx = Some(i);
                break;
            }
        }

        match (before_idx, after_idx) {
            (Some(before), Some(after)) if before == after => {
                // Exactly on a keyframe
                Some(track.keyframes[before].value)
            }
            (Some(before), Some(after)) => {
                // Interpolate between keyframes
                let before_key = &track.keyframes[before];
                let after_key = &track.keyframes[after];

                let t = (current_time - before_key.time) / (after_key.time - before_key.time);
                let interpolated_t = self.interpolate(t, before_key.interpolation);

                Some(before_key.value + (after_key.value - before_key.value) * interpolated_t)
            }
            (Some(before), None) => {
                // Past the last keyframe
                Some(track.keyframes[before].value)
            }
            (None, Some(after)) => {
                // Before the first keyframe
                Some(track.keyframes[after].value)
            }
            _ => None,
        }
    }

    /// Evaluate vec3 track
    fn evaluate_vec3_track(&self, track: &AnimationTrack<Vector3<f32>>) -> Option<Vector3<f32>> {
        if track.keyframes.is_empty() {
            return None;
        }

        let current_time = if self.loop_animation {
            self.current_time % self.duration
        } else {
            self.current_time.min(self.duration)
        };

        let mut before_idx = None;
        let mut after_idx = None;

        for (i, keyframe) in track.keyframes.iter().enumerate() {
            if keyframe.time <= current_time {
                before_idx = Some(i);
            }
            if keyframe.time >= current_time {
                after_idx = Some(i);
                break;
            }
        }

        match (before_idx, after_idx) {
            (Some(before), Some(after)) if before == after => {
                Some(track.keyframes[before].value)
            }
            (Some(before), Some(after)) => {
                let before_key = &track.keyframes[before];
                let after_key = &track.keyframes[after];

                let t = (current_time - before_key.time) / (after_key.time - before_key.time);
                let interpolated_t = self.interpolate(t, before_key.interpolation);

                Some(before_key.value.lerp(&after_key.value, interpolated_t))
            }
            (Some(before), None) => Some(track.keyframes[before].value),
            (None, Some(after)) => Some(track.keyframes[after].value),
            _ => None,
        }
    }

    /// Evaluate vec4 track
    fn evaluate_vec4_track(&self, track: &AnimationTrack<Vector4<f32>>) -> Option<Vector4<f32>> {
        if track.keyframes.is_empty() {
            return None;
        }

        let current_time = if self.loop_animation {
            self.current_time % self.duration
        } else {
            self.current_time.min(self.duration)
        };

        let mut before_idx = None;
        let mut after_idx = None;

        for (i, keyframe) in track.keyframes.iter().enumerate() {
            if keyframe.time <= current_time {
                before_idx = Some(i);
            }
            if keyframe.time >= current_time {
                after_idx = Some(i);
                break;
            }
        }

        match (before_idx, after_idx) {
            (Some(before), Some(after)) if before == after => {
                Some(track.keyframes[before].value)
            }
            (Some(before), Some(after)) => {
                let before_key = &track.keyframes[before];
                let after_key = &track.keyframes[after];

                let t = (current_time - before_key.time) / (after_key.time - before_key.time);
                let interpolated_t = self.interpolate(t, before_key.interpolation);

                Some(before_key.value.lerp(&after_key.value, interpolated_t))
            }
            (Some(before), None) => Some(track.keyframes[before].value),
            (None, Some(after)) => Some(track.keyframes[after].value),
            _ => None,
        }
    }

    /// Apply interpolation curve
    fn interpolate(&self, t: f32, mode: InterpolationMode) -> f32 {
        match mode {
            InterpolationMode::Linear => t,
            InterpolationMode::Step => if t < 1.0 { 0.0 } else { 1.0 },
            InterpolationMode::EaseIn => t * t,
            InterpolationMode::EaseOut => 1.0 - (1.0 - t) * (1.0 - t),
            InterpolationMode::EaseInOut => {
                if t < 0.5 {
                    2.0 * t * t
                } else {
                    1.0 - (-2.0 * t + 2.0).powi(2) / 2.0
                }
            }
            InterpolationMode::Smooth => {
                // Smoothstep function
                t * t * (3.0 - 2.0 * t)
            }
        }
    }

    /// Update animation time
    pub fn update(&mut self, delta_time: f32) {
        if self.playing {
            self.current_time += delta_time;

            if self.loop_animation && self.current_time >= self.duration {
                self.current_time = self.current_time % self.duration;
            } else if !self.loop_animation && self.current_time >= self.duration {
                self.current_time = self.duration;
                self.playing = false;
            }
        }
    }

    /// Play animation
    pub fn play(&mut self) {
        self.playing = true;
    }

    /// Pause animation
    pub fn pause(&mut self) {
        self.playing = false;
    }

    /// Stop animation and reset to beginning
    pub fn stop(&mut self) {
        self.playing = false;
        self.current_time = 0.0;
    }

    /// Seek to specific time
    pub fn seek(&mut self, time: f32) {
        self.current_time = time.clamp(0.0, self.duration);
    }
}

/// Animation value types
#[derive(Debug, Clone)]
pub enum AnimationValue {
    Float(f32),
    Vec3(Vector3<f32>),
    Vec4(Vector4<f32>),
    Color(Vector3<f32>),
}

/// Generate unique track ID
fn generate_track_id() -> TrackId {
    use std::sync::atomic::{AtomicU64, Ordering};
    static COUNTER: AtomicU64 = AtomicU64::new(1);
    COUNTER.fetch_add(1, Ordering::Relaxed)
}