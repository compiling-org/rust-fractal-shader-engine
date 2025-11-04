//! Animation System Module
//!
//! This module provides keyframe animation, procedural animation,
//! and timeline-based control for fractal parameters and scene objects.
pub mod easing;
pub mod keyframe;
pub mod timeline;

use serde::{Deserialize, Serialize};

/// Animation system for fractal parameters and scene elements
#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct AnimationController {
    timeline: TimelineProject,
    current_time: f32,
    playing: bool,
    loop_animation: bool,
}

impl AnimationController {
    pub fn new() -> Self {
        Self {
            timeline: TimelineProject::new(),
            current_time: 0.0,
            playing: false,
            loop_animation: true,
        }
    }

    pub fn update(&mut self, delta_time: f32) {
        if self.playing {
            self.current_time += delta_time;
            if self.current_time >= self.timeline.duration() {
                if self.loop_animation {
                    self.current_time = 0.0;
                } else {
                    self.playing = false;
                    self.current_time = self.timeline.duration();
                }
            }
        }
    }

    pub fn play(&mut self) {
        self.playing = true;
    }

    pub fn pause(&mut self) {
        self.playing = false;
    }

    pub fn stop(&mut self) {
        self.playing = false;
        self.current_time = 0.0;
    }

    pub fn set_time(&mut self, time: f32) {
        self.current_time = time.clamp(0.0, self.timeline.duration());
    }

    pub fn current_time(&self) -> f32 {
        self.current_time
    }

    pub fn is_playing(&self) -> bool {
        self.playing
    }

    pub fn timeline(&self) -> &TimelineProject {
        &self.timeline
    }

    pub fn timeline_mut(&mut self) -> &mut TimelineProject {
        &mut self.timeline
    }
}

/// Timeline project containing animation tracks
#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct TimelineProject {
    tracks: Vec<AnimationTrack>,
    duration: f32,
    frame_rate: f32,
}

impl TimelineProject {
    pub fn new() -> Self {
        Self {
            tracks: Vec::new(),
            duration: 10.0, // 10 seconds default
            frame_rate: 30.0,
        }
    }

    pub fn add_track(&mut self, track: AnimationTrack) {
        self.tracks.push(track);
    }

    pub fn tracks(&self) -> &[AnimationTrack] {
        &self.tracks
    }

    pub fn duration(&self) -> f32 {
        self.duration
    }

    pub fn set_duration(&mut self, duration: f32) {
        self.duration = duration.max(0.1);
    }

    pub fn frame_rate(&self) -> f32 {
        self.frame_rate
    }

    pub fn set_frame_rate(&mut self, frame_rate: f32) {
        self.frame_rate = frame_rate.max(1.0);
    }

    pub fn current_time(&self) -> f32 {
        // This would be set by the controller
        0.0
    }

    pub fn current_frame(&self) -> u32 {
        (self.current_time() * self.frame_rate) as u32
    }
}

/// Animation track for a specific parameter
#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct AnimationTrack {
    pub name: String,
    pub parameter_path: String, // e.g., "fractal.power" or "camera.position.x"
    pub keyframes: Vec<Keyframe>,
    pub interpolation: InterpolationMode,
}

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct Keyframe {
    pub time: f32,
    pub value: f32,
    pub easing: EasingType,
}

#[derive(Debug, Clone, Serialize, Deserialize)]
pub enum InterpolationMode {
    Linear,
    Cubic,
    Step,
}

#[derive(Debug, Clone, Serialize, Deserialize)]
pub enum EasingType {
    Linear,
    EaseIn,
    EaseOut,
    EaseInOut,
    Cubic,
    Sine,
    Exponential,
}

impl AnimationTrack {
    pub fn new(name: &str, parameter_path: &str) -> Self {
        Self {
            name: name.to_string(),
            parameter_path: parameter_path.to_string(),
            keyframes: Vec::new(),
            interpolation: InterpolationMode::Cubic,
        }
    }

    pub fn add_keyframe(&mut self, time: f32, value: f32, easing: EasingType) {
        self.keyframes.push(Keyframe { time, value, easing });
        self.keyframes.sort_by(|a, b| a.time.partial_cmp(&b.time).unwrap());
    }

    pub fn evaluate(&self, time: f32) -> f32 {
        if self.keyframes.is_empty() {
            return 0.0;
        }

        if self.keyframes.len() == 1 {
            return self.keyframes[0].value;
        }

        // Find keyframes around the current time
        let mut left_idx = 0;
        let mut right_idx = self.keyframes.len() - 1;

        for (i, keyframe) in self.keyframes.iter().enumerate() {
            if keyframe.time <= time {
                left_idx = i;
            }
            if keyframe.time >= time {
                right_idx = i;
                break;
            }
        }

        if left_idx == right_idx {
            return self.keyframes[left_idx].value;
        }

        let left = &self.keyframes[left_idx];
        let right = &self.keyframes[right_idx];

        let t = (time - left.time) / (right.time - left.time);
        let eased_t = self.apply_easing(t, &right.easing);

        match self.interpolation {
            InterpolationMode::Linear => left.value + (right.value - left.value) * eased_t,
            InterpolationMode::Cubic => {
                // Simple cubic interpolation
                let t2 = eased_t * eased_t;
                let t3 = t2 * eased_t;
                left.value + (right.value - left.value) * (3.0 * t2 - 2.0 * t3)
            }
            InterpolationMode::Step => left.value,
        }
    }

    fn apply_easing(&self, t: f32, easing: &EasingType) -> f32 {
        match easing {
            EasingType::Linear => t,
            EasingType::EaseIn => t * t,
            EasingType::EaseOut => 1.0 - (1.0 - t) * (1.0 - t),
            EasingType::EaseInOut => {
                if t < 0.5 {
                    2.0 * t * t
                } else {
                    1.0 - (-2.0 * t + 2.0).powi(2) / 2.0
                }
            }
            EasingType::Cubic => t * t * t,
            EasingType::Sine => (t * std::f32::consts::PI / 2.0).sin(),
            EasingType::Exponential => {
                if t == 0.0 {
                    0.0
                } else {
                    (2.0_f32.powf(10.0 * t - 10.0))
                }
            }
        }
    }
}

impl Default for AnimationController {
    fn default() -> Self {
        Self::new()
    }
}

impl Default for TimelineProject {
    fn default() -> Self {
        Self::new()
    }
}