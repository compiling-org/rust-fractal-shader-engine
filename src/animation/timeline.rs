//! Timeline Animation System
//!
//! This module provides a timeline-based interface for managing keyframe animations,
//! with support for multiple tracks, curves, and professional animation tools.

use super::keyframe::*;
use std::collections::HashMap;
use serde::{Deserialize, Serialize};

/// Timeline project containing multiple animation tracks
#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct TimelineProject {
    pub name: String,
    pub animation_controller: AnimationController,
    pub frame_rate: f32,
    pub current_frame: u32,
    pub total_frames: u32,
    pub tracks: Vec<TimelineTrack>,
}

/// Timeline track representation for UI
#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct TimelineTrack {
    pub id: TrackId,
    pub name: String,
    pub track_type: TimelineTrackType,
    pub visible: bool,
    pub locked: bool,
    pub color: [f32; 3],
}

/// Types of timeline tracks
#[derive(Debug, Clone, Copy, PartialEq, Serialize, Deserialize)]
pub enum TimelineTrackType {
    Float,
    Position,
    Rotation,
    Scale,
    Color,
    Custom,
}

/// Keyframe data for timeline display
#[derive(Debug, Clone)]
pub struct TimelineKeyframe {
    pub time: f32,
    pub value: f32, // Normalized 0-1 for display
    pub interpolation: InterpolationMode,
    pub selected: bool,
}

/// Timeline selection state
#[derive(Debug, Clone)]
pub struct TimelineSelection {
    pub selected_tracks: Vec<TrackId>,
    pub selected_keyframes: Vec<(TrackId, usize)>, // (track_id, keyframe_index)
    pub time_selection: Option<(f32, f32)>, // start and end time
}

impl TimelineProject {
    /// Create new timeline project
    pub fn new(name: &str) -> Self {
        Self {
            name: name.to_string(),
            animation_controller: AnimationController::new(),
            frame_rate: 30.0,
            current_frame: 0,
            total_frames: 300, // 10 seconds at 30fps
            tracks: Vec::new(),
        }
    }

    /// Add track to timeline
    pub fn add_track(&mut self, name: &str, track_type: TimelineTrackType, color: [f32; 3]) -> TrackId {
        let track_id = match track_type {
            TimelineTrackType::Float => self.animation_controller.add_float_track(name, &format!("custom.{}", name)),
            TimelineTrackType::Position => self.animation_controller.add_vec3_track(name, "transform.position"),
            TimelineTrackType::Rotation => self.animation_controller.add_vec3_track(name, "transform.rotation"),
            TimelineTrackType::Scale => self.animation_controller.add_vec3_track(name, "transform.scale"),
            TimelineTrackType::Color => self.animation_controller.add_vec3_track(name, "material.color"),
            TimelineTrackType::Custom => self.animation_controller.add_float_track(name, &format!("custom.{}", name)),
        };

        self.tracks.push(TimelineTrack {
            id: track_id,
            name: name.to_string(),
            track_type,
            visible: true,
            locked: false,
            color,
        });

        track_id
    }

    /// Get current time in seconds
    pub fn current_time(&self) -> f32 {
        self.current_frame as f32 / self.frame_rate
    }

    /// Set current frame
    pub fn set_current_frame(&mut self, frame: u32) {
        self.current_frame = frame.min(self.total_frames);
        self.animation_controller.seek(self.current_time());
    }

    /// Convert time to frame
    pub fn time_to_frame(&self, time: f32) -> u32 {
        (time * self.frame_rate) as u32
    }

    /// Convert frame to time
    pub fn frame_to_time(&self, frame: u32) -> f32 {
        frame as f32 / self.frame_rate
    }

    /// Get keyframes for a track (for UI display)
    pub fn get_track_keyframes(&self, track_id: TrackId) -> Vec<TimelineKeyframe> {
        self.animation_controller.tracks.get(&track_id)
            .map(|track_type| match track_type {
                AnimationTrackType::Float(track) => {
                    track.keyframes.iter().map(|kf| {
                        TimelineKeyframe {
                            time: kf.time,
                            value: kf.value, // Assume normalized for display
                            interpolation: kf.interpolation,
                            selected: false, // TODO: Implement selection
                        }
                    }).collect()
                }
                _ => Vec::new(), // TODO: Handle other track types
            })
            .unwrap_or_default()
    }

    /// Add keyframe to track
    pub fn add_keyframe(&mut self, track_id: TrackId, time: f32, value: f32, interpolation: InterpolationMode) {
        match self.animation_controller.tracks.get(&track_id) {
            Some(AnimationTrackType::Float(_)) => {
                self.animation_controller.add_keyframe(track_id, time, AnimationValue::Float(value), interpolation);
            }
            _ => {} // TODO: Handle other types
        }
    }

    /// Remove keyframe from track
    pub fn remove_keyframe(&mut self, track_id: TrackId, keyframe_index: usize) {
        if let Some(track_type) = self.animation_controller.tracks.get_mut(&track_id) {
            match track_type {
                AnimationTrackType::Float(track) => {
                    if keyframe_index < track.keyframes.len() {
                        track.keyframes.remove(keyframe_index);
                    }
                }
                _ => {} // TODO: Handle other types
            }
        }
    }

    /// Update keyframe
    pub fn update_keyframe(&mut self, track_id: TrackId, keyframe_index: usize, time: f32, value: f32) {
        if let Some(track_type) = self.animation_controller.tracks.get_mut(&track_id) {
            match track_type {
                AnimationTrackType::Float(track) => {
                    if let Some(keyframe) = track.keyframes.get_mut(keyframe_index) {
                        keyframe.time = time;
                        keyframe.value = value;
                        // Re-sort keyframes after time change
                        track.keyframes.sort_by(|a, b| a.time.partial_cmp(&b.time).unwrap());
                    }
                }
                _ => {} // TODO: Handle other types
            }
        }
    }

    /// Play animation
    pub fn play(&mut self) {
        self.animation_controller.play();
    }

    /// Pause animation
    pub fn pause(&mut self) {
        self.animation_controller.pause();
    }

    /// Stop animation
    pub fn stop(&mut self) {
        self.animation_controller.stop();
        self.current_frame = 0;
    }

    /// Step to next frame
    pub fn step_forward(&mut self) {
        if self.current_frame < self.total_frames {
            self.set_current_frame(self.current_frame + 1);
        }
    }

    /// Step to previous frame
    pub fn step_backward(&mut self) {
        if self.current_frame > 0 {
            self.set_current_frame(self.current_frame - 1);
        }
    }

    /// Update timeline (called every frame)
    pub fn update(&mut self, delta_time: f32) {
        self.animation_controller.update(delta_time);

        // Update current frame based on animation time
        let current_time = self.animation_controller.current_time;
        self.current_frame = self.time_to_frame(current_time).min(self.total_frames);
    }
}

/// Timeline UI utilities
pub struct TimelineUI;

impl TimelineUI {
    /// Calculate visible time range for timeline display
    pub fn calculate_visible_range(current_time: f32, zoom: f32, viewport_width: f32) -> (f32, f32) {
        let visible_duration = viewport_width / zoom;
        let start_time = (current_time - visible_duration * 0.5).max(0.0);
        let end_time = start_time + visible_duration;
        (start_time, end_time)
    }

    /// Convert screen coordinates to time
    pub fn screen_to_time(screen_x: f32, start_time: f32, zoom: f32, timeline_x: f32) -> f32 {
        start_time + (screen_x - timeline_x) / zoom
    }

    /// Convert time to screen coordinates
    pub fn time_to_screen(time: f32, start_time: f32, zoom: f32, timeline_x: f32) -> f32 {
        timeline_x + (time - start_time) * zoom
    }

    /// Snap time to frame
    pub fn snap_to_frame(time: f32, frame_rate: f32) -> f32 {
        let frame = (time * frame_rate).round();
        frame / frame_rate
    }

    /// Get time at mouse position
    pub fn get_time_at_mouse(mouse_x: f32, timeline_rect: &egui::Rect, visible_start: f32, zoom: f32) -> f32 {
        let relative_x = mouse_x - timeline_rect.left();
        visible_start + relative_x / zoom
    }
}

/// Animation curve presets
pub struct AnimationPresets;

impl AnimationPresets {
    /// Create bounce animation
    pub fn create_bounce_animation(controller: &mut AnimationController, track_id: TrackId) {
        controller.add_keyframe(track_id, 0.0, AnimationValue::Float(0.0), InterpolationMode::EaseOut);
        controller.add_keyframe(track_id, 0.3, AnimationValue::Float(1.2), InterpolationMode::EaseOut);
        controller.add_keyframe(track_id, 0.6, AnimationValue::Float(0.9), InterpolationMode::EaseOut);
        controller.add_keyframe(track_id, 0.8, AnimationValue::Float(1.05), InterpolationMode::EaseOut);
        controller.add_keyframe(track_id, 1.0, AnimationValue::Float(1.0), InterpolationMode::EaseOut);
    }

    /// Create elastic animation
    pub fn create_elastic_animation(controller: &mut AnimationController, track_id: TrackId) {
        controller.add_keyframe(track_id, 0.0, AnimationValue::Float(0.0), InterpolationMode::EaseOut);
        controller.add_keyframe(track_id, 1.0, AnimationValue::Float(1.0), InterpolationMode::EaseOut);
    }

    /// Create smooth pulse animation
    pub fn create_pulse_animation(controller: &mut AnimationController, track_id: TrackId, duration: f32) {
        let keyframes = 8;
        for i in 0..keyframes {
            let time = (i as f32 / (keyframes - 1) as f32) * duration;
            let value = if i % 2 == 0 { 1.0 } else { 0.8 };
            let interpolation = if i == keyframes - 1 { InterpolationMode::Smooth } else { InterpolationMode::EaseInOut };
            controller.add_keyframe(track_id, time, AnimationValue::Float(value), interpolation);
        }
    }

    /// Create fractal parameter morphing animation
    pub fn create_fractal_morph(controller: &mut AnimationController, power_track: TrackId, bailout_track: TrackId) {
        // Power animation: 2 -> 8 -> 2
        controller.add_keyframe(power_track, 0.0, AnimationValue::Float(2.0), InterpolationMode::Smooth);
        controller.add_keyframe(power_track, 2.5, AnimationValue::Float(8.0), InterpolationMode::Smooth);
        controller.add_keyframe(power_track, 5.0, AnimationValue::Float(2.0), InterpolationMode::Smooth);

        // Bailout animation: 4 -> 2 -> 4
        controller.add_keyframe(bailout_track, 0.0, AnimationValue::Float(4.0), InterpolationMode::Smooth);
        controller.add_keyframe(bailout_track, 2.5, AnimationValue::Float(2.0), InterpolationMode::Smooth);
        controller.add_keyframe(bailout_track, 5.0, AnimationValue::Float(4.0), InterpolationMode::Smooth);
    }
}