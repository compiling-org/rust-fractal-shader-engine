//! Keyframe Animation System
//!
//! This module provides keyframe-based animation capabilities for smooth parameter transitions.

use crate::fractal::types::FractalParameters;
use nalgebra::Vector3;

/// Single keyframe in an animation track
#[derive(Debug, Clone)]
pub struct Keyframe<T> {
    pub time: f32,
    pub value: T,
    pub easing: EasingFunction,
}

/// Animation track that interpolates between keyframes
#[derive(Debug, Clone)]
pub struct AnimationTrack<T: Clone> {
    name: String,
    keyframes: Vec<Keyframe<T>>,
}

impl<T: Clone> AnimationTrack<T> {
    pub fn new(name: &str) -> Self {
        Self {
            name: name.to_string(),
            keyframes: Vec::new(),
        }
    }

    /// Add a keyframe to the track
    pub fn add_keyframe(&mut self, time: f32, value: T, easing: EasingFunction) {
        // Insert keyframe in order
        let position = self.keyframes.iter()
            .position(|kf| kf.time > time)
            .unwrap_or(self.keyframes.len());
        
        self.keyframes.insert(position, Keyframe { time, value, easing });
    }

    /// Get interpolated value at given time
    pub fn get_value(&self, time: f32) -> Option<&T> {
        if self.keyframes.is_empty() {
            return None;
        }

        // Find the two keyframes surrounding the given time
        let mut prev_keyframe: Option<&Keyframe<T>> = None;
        let mut next_keyframe: Option<&Keyframe<T>> = None;

        for keyframe in &self.keyframes {
            if keyframe.time <= time {
                prev_keyframe = Some(keyframe);
            } else {
                next_keyframe = Some(keyframe);
                break;
            }
        }

        match (prev_keyframe, next_keyframe) {
            (Some(prev), Some(next)) => {
                let t = (time - prev.time) / (next.time - prev.time);
                let t = prev.easing.apply(t);
                
                // Note: This is a simplified version. Real interpolation would
                // require implementing interpolation for each specific type.
                Some(&prev.value)
            }
            (Some(prev), None) => Some(&prev.value), // After last keyframe
            (None, Some(next)) => Some(&next.value), // Before first keyframe
            (None, None) => None, // No keyframes
        }
    }

    /// Get the name of this track
    pub fn name(&self) -> &str {
        &self.name
    }

    /// Get all keyframes
    pub fn keyframes(&self) -> &[Keyframe<T>] {
        &self.keyframes
    }

    /// Remove all keyframes
    pub fn clear(&mut self) {
        self.keyframes.clear();
    }
}

/// Animation controller that manages multiple tracks
pub struct AnimationController {
    tracks: Vec<AnimationTrack<f32>>,
    fractal_tracks: Vec<AnimationTrack<FractalParameters>>,
    vector_tracks: Vec<AnimationTrack<Vector3<f32>>>,
    current_time: f32,
    duration: f32,
    playing: bool,
    loop_animation: bool,
}

impl AnimationController {
    pub fn new() -> Self {
        Self {
            tracks: Vec::new(),
            fractal_tracks: Vec::new(),
            vector_tracks: Vec::new(),
            current_time: 0.0,
            duration: 0.0,
            playing: false,
            loop_animation: false,
        }
    }

    /// Add a float parameter track
    pub fn add_float_track(&mut self, track: AnimationTrack<f32>) {
        self.tracks.push(track);
        self.update_duration();
    }

    /// Add a fractal parameters track
    pub fn add_fractal_track(&mut self, track: AnimationTrack<FractalParameters>) {
        self.fractal_tracks.push(track);
        self.update_duration();
    }

    /// Add a vector parameter track
    pub fn add_vector_track(&mut self, track: AnimationTrack<Vector3<f32>>) {
        self.vector_tracks.push(track);
        self.update_duration();
    }

    /// Get value from a float track at current time
    pub fn get_float_value(&self, track_name: &str) -> Option<f32> {
        self.tracks.iter()
            .find(|track| track.name() == track_name)
            .and_then(|track| track.get_value(self.current_time))
            .cloned()
    }

    /// Get value from a fractal track at current time
    pub fn get_fractal_value(&self, track_name: &str) -> Option<FractalParameters> {
        self.fractal_tracks.iter()
            .find(|track| track.name() == track_name)
            .and_then(|track| track.get_value(self.current_time))
            .cloned()
    }

    /// Get value from a vector track at current time
    pub fn get_vector_value(&self, track_name: &str) -> Option<Vector3<f32>> {
        self.vector_tracks.iter()
            .find(|track| track.name() == track_name)
            .and_then(|track| track.get_value(self.current_time))
            .cloned()
    }

    /// Set animation time
    pub fn set_time(&mut self, time: f32) {
        self.current_time = time.clamp(0.0, self.duration);
    }

    /// Get current time
    pub fn time(&self) -> f32 {
        self.current_time
    }

    /// Get animation duration
    pub fn duration(&self) -> f32 {
        self.duration
    }

    /// Start animation playback
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

    /// Check if animation is playing
    pub fn is_playing(&self) -> bool {
        self.playing
    }

    /// Set loop mode
    pub fn set_loop(&mut self, loop_enabled: bool) {
        self.loop_animation = loop_enabled;
    }

    /// Check if animation is looping
    pub fn is_looping(&self) -> bool {
        self.loop_animation
    }

    /// Update animation time based on delta time
    pub fn update(&mut self, delta_time: f32) {
        if !self.playing {
            return;
        }

        self.current_time += delta_time;

        if self.current_time > self.duration {
            if self.loop_animation {
                self.current_time = 0.0;
            } else {
                self.current_time = self.duration;
                self.playing = false;
            }
        }
    }

    /// Update the total duration based on keyframes
    fn update_duration(&mut self) {
        self.duration = 0.0;

        // Check all track types for maximum keyframe time
        for track in &self.tracks {
            if let Some(last_keyframe) = track.keyframes().last() {
                self.duration = self.duration.max(last_keyframe.time);
            }
        }

        for track in &self.fractal_tracks {
            if let Some(last_keyframe) = track.keyframes().last() {
                self.duration = self.duration.max(last_keyframe.time);
            }
        }

        for track in &self.vector_tracks {
            if let Some(last_keyframe) = track.keyframes().last() {
                self.duration = self.duration.max(last_keyframe.time);
            }
        }
    }
}

impl Default for AnimationController {
    fn default() -> Self {
        Self::new()
    }
}

/// Convenience functions for creating common animation tracks
pub fn create_fractal_zoom_animation() -> AnimationTrack<FractalParameters> {
    let mut track = AnimationTrack::new("fractal_zoom");
    
    let mut params1 = FractalParameters::default();
    params1.scale = 1.0;
    
    let mut params2 = FractalParameters::default();
    params2.scale = 0.1;
    
    let mut params3 = FractalParameters::default();
    params3.scale = 10.0;
    
    track.add_keyframe(0.0, params1, EasingFunction::Smooth);
    track.add_keyframe(2.0, params2, EasingFunction::Smooth);
    track.add_keyframe(4.0, params3, EasingFunction::Smooth);
    
    track
}

pub fn create_rotation_animation(axis: Vector3<f32>) -> AnimationTrack<Vector3<f32>> {
    let mut track = AnimationTrack::new("rotation");
    
    track.add_keyframe(0.0, Vector3::zeros(), EasingFunction::Linear);
    track.add_keyframe(10.0, axis * std::f32::consts::TAU, EasingFunction::Linear);
    
    track
}

pub fn create_color_transition_animation() -> AnimationTrack<FractalParameters> {
    let mut track = AnimationTrack::new("color_transition");
    
    let mut params1 = FractalParameters::default();
    params1.color_map.palette[0] = Vector3::new(0.0, 0.0, 0.0); // Black
    
    let mut params2 = FractalParameters::default();
    params2.color_map.palette[0] = Vector3::new(1.0, 0.0, 0.0); // Red
    
    let mut params3 = FractalParameters::default();
    params3.color_map.palette[0] = Vector3::new(0.0, 1.0, 0.0); // Green
    
    track.add_keyframe(0.0, params1, EasingFunction::Smooth);
    track.add_keyframe(2.0, params2, EasingFunction::Smooth);
    track.add_keyframe(4.0, params3, EasingFunction::Smooth);
    
    track
}