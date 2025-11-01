//! Timeline and Animation Management
//!
//! This module provides timeline-based animation editing and playback controls.

use super::keyframe::*;

/// Timeline for managing multiple animation tracks
#[derive(Debug, Clone)]
pub struct Timeline {
    name: String,
    duration: f32,
    frame_rate: f32,
    current_frame: u32,
    tracks: Vec<TrackInfo>,
    markers: Vec<TimelineMarker>,
}

#[derive(Debug, Clone)]
struct TrackInfo {
    name: String,
    track_type: TrackType,
    muted: bool,
    locked: bool,
    visible: bool,
}

#[derive(Debug, Clone)]
pub enum TrackType {
    Float,
    FractalParameters,
    Vector3,
}

#[derive(Debug, Clone)]
pub struct TimelineMarker {
    name: String,
    time: f32,
    color: [f32; 3],
}

impl Timeline {
    pub fn new(name: &str) -> Self {
        Self {
            name: name.to_string(),
            duration: 10.0,
            frame_rate: 30.0,
            current_frame: 0,
            tracks: Vec::new(),
            markers: Vec::new(),
        }
    }

    /// Add a new track to the timeline
    pub fn add_track(&mut self, name: &str, track_type: TrackType) {
        self.tracks.push(TrackInfo {
            name: name.to_string(),
            track_type,
            muted: false,
            locked: false,
            visible: true,
        });
    }

    /// Remove a track from the timeline
    pub fn remove_track(&mut self, name: &str) {
        self.tracks.retain(|track| track.name != name);
    }

    /// Get track by name
    pub fn get_track(&self, name: &str) -> Option<&TrackInfo> {
        self.tracks.iter().find(|track| track.name == name)
    }

    /// Get mutable track by name
    pub fn get_track_mut(&mut self, name: &str) -> Option<&mut TrackInfo> {
        self.tracks.iter_mut().find(|track| track.name == name)
    }

    /// Set track visibility
    pub fn set_track_visible(&mut self, name: &str, visible: bool) {
        if let Some(track) = self.get_track_mut(name) {
            track.visible = visible;
        }
    }

    /// Set track mute state
    pub fn set_track_muted(&mut self, name: &str, muted: bool) {
        if let Some(track) = self.get_track_mut(name) {
            track.muted = muted;
        }
    }

    /// Set track lock state
    pub fn set_track_locked(&mut self, name: &str, locked: bool) {
        if let Some(track) = self.get_track_mut(name) {
            track.locked = locked;
        }
    }

    /// Add a timeline marker
    pub fn add_marker(&mut self, name: &str, time: f32, color: [f32; 3]) {
        self.markers.push(TimelineMarker {
            name: name.to_string(),
            time,
            color,
        });
    }

    /// Remove a timeline marker
    pub fn remove_marker(&mut self, time: f32) {
        self.markers.retain(|marker| marker.time != time);
    }

    /// Get all markers
    pub fn markers(&self) -> &[TimelineMarker] {
        &self.markers
    }

    /// Get timeline duration
    pub fn duration(&self) -> f32 {
        self.duration
    }

    /// Set timeline duration
    pub fn set_duration(&mut self, duration: f32) {
        self.duration = duration.max(0.1);
    }

    /// Get frame rate
    pub fn frame_rate(&self) -> f32 {
        self.frame_rate
    }

    /// Set frame rate
    pub fn set_frame_rate(&mut self, frame_rate: f32) {
        self.frame_rate = frame_rate.max(1.0);
    }

    /// Convert time to frame number
    pub fn time_to_frame(&self, time: f32) -> u32 {
        (time * self.frame_rate) as u32
    }

    /// Convert frame number to time
    pub fn frame_to_time(&self, frame: u32) -> f32 {
        frame as f32 / self.frame_rate
    }

    /// Get current frame
    pub fn current_frame(&self) -> u32 {
        self.current_frame
    }

    /// Set current frame
    pub fn set_current_frame(&mut self, frame: u32) {
        let max_frame = self.time_to_frame(self.duration);
        self.current_frame = frame.min(max_frame);
    }

    /// Get current time
    pub fn current_time(&self) -> f32 {
        self.frame_to_time(self.current_frame)
    }

    /// Set current time
    pub fn set_current_time(&mut self, time: f32) {
        let clamped_time = time.clamp(0.0, self.duration);
        self.current_frame = self.time_to_frame(clamped_time);
    }

    /// Jump to next frame
    pub fn next_frame(&mut self) {
        let max_frame = self.time_to_frame(self.duration);
        if self.current_frame < max_frame {
            self.current_frame += 1;
        }
    }

    /// Jump to previous frame
    pub fn prev_frame(&mut self) {
        if self.current_frame > 0 {
            self.current_frame -= 1;
        }
    }

    /// Jump to next marker
    pub fn jump_to_next_marker(&mut self) {
        let current_time = self.current_time();
        if let Some(marker) = self.markers.iter()
            .find(|marker| marker.time > current_time)
        {
            self.set_current_time(marker.time);
        }
    }

    /// Jump to previous marker
    pub fn jump_to_prev_marker(&mut self) {
        let current_time = self.current_time();
        if let Some(marker) = self.markers.iter()
            .rev()
            .find(|marker| marker.time < current_time)
        {
            self.set_current_time(marker.time);
        }
    }

    /// Get all tracks
    pub fn tracks(&self) -> &[TrackInfo] {
        &self.tracks
    }

    /// Get timeline name
    pub fn name(&self) -> &str {
        &self.name
    }

    /// Set timeline name
    pub fn set_name(&mut self, name: &str) {
        self.name = name.to_string();
    }
}

/// Animation playback controller
#[derive(Debug, Clone)]
pub struct PlaybackController {
    playing: bool,
    loop_playback: bool,
    speed: f32,
    ping_pong: bool,
    direction: PlaybackDirection,
}

#[derive(Debug, Clone, Copy)]
pub enum PlaybackDirection {
    Forward,
    Backward,
}

impl PlaybackController {
    pub fn new() -> Self {
        Self {
            playing: false,
            loop_playback: false,
            speed: 1.0,
            ping_pong: false,
            direction: PlaybackDirection::Forward,
        }
    }

    /// Start playback
    pub fn start(&mut self) {
        self.playing = true;
    }

    /// Stop playback
    pub fn stop(&mut self) {
        self.playing = false;
    }

    /// Toggle playback state
    pub fn toggle(&mut self) {
        self.playing = !self.playing;
    }

    /// Check if playing
    pub fn is_playing(&self) -> bool {
        self.playing
    }

    /// Set loop playback
    pub fn set_loop_playback(&mut self, loop_playback: bool) {
        self.loop_playback = loop_playback;
    }

    /// Get loop playback state
    pub fn loop_playback(&self) -> bool {
        self.loop_playback
    }

    /// Set playback speed
    pub fn set_speed(&mut self, speed: f32) {
        self.speed = speed.max(0.01);
    }

    /// Get playback speed
    pub fn speed(&self) -> f32 {
        self.speed
    }

    /// Set ping-pong mode
    pub fn set_ping_pong(&mut self, ping_pong: bool) {
        self.ping_pong = ping_pong;
    }

    /// Get ping-pong mode
    pub fn ping_pong(&self) -> bool {
        self.ping_pong
    }

    /// Set playback direction
    pub fn set_direction(&mut self, direction: PlaybackDirection) {
        self.direction = direction;
    }

    /// Get playback direction
    pub fn direction(&self) -> PlaybackDirection {
        self.direction
    }

    /// Update playback based on delta time
    pub fn update(&mut self, delta_time: f32, timeline: &mut Timeline) {
        if !self.playing {
            return;
        }

        let effective_delta = delta_time * self.speed;
        let current_time = timeline.current_time();
        let duration = timeline.duration();

        match self.direction {
            PlaybackDirection::Forward => {
                let new_time = current_time + effective_delta;
                if new_time > duration {
                    if self.loop_playback {
                        timeline.set_current_time(new_time % duration);
                    } else if self.ping_pong {
                        timeline.set_current_time(duration);
                        self.direction = PlaybackDirection::Backward;
                        self.playing = false;
                    } else {
                        timeline.set_current_time(duration);
                        self.playing = false;
                    }
                } else {
                    timeline.set_current_time(new_time);
                }
            }
            PlaybackDirection::Backward => {
                let new_time = current_time - effective_delta;
                if new_time < 0.0 {
                    if self.loop_playback {
                        timeline.set_current_time(duration + new_time);
                    } else if self.ping_pong {
                        timeline.set_current_time(0.0);
                        self.direction = PlaybackDirection::Forward;
                        self.playing = false;
                    } else {
                        timeline.set_current_time(0.0);
                        self.playing = false;
                    }
                } else {
                    timeline.set_current_time(new_time);
                }
            }
        }
    }
}

impl Default for PlaybackController {
    fn default() -> Self {
        Self::new()
    }
}

/// Timeline project that contains all animation data
#[derive(Debug, Clone)]
pub struct TimelineProject {
    name: String,
    timeline: Timeline,
    playback_controller: PlaybackController,
    animation_controller: AnimationController,
}

impl TimelineProject {
    pub fn new(name: &str) -> Self {
        Self {
            name: name.to_string(),
            timeline: Timeline::new(name),
            playback_controller: PlaybackController::new(),
            animation_controller: AnimationController::new(),
        }
    }

    /// Get timeline reference
    pub fn timeline(&self) -> &Timeline {
        &self.timeline
    }

    /// Get mutable timeline reference
    pub fn timeline_mut(&mut self) -> &mut Timeline {
        &mut self.timeline
    }

    /// Get playback controller reference
    pub fn playback_controller(&self) -> &PlaybackController {
        &self.playback_controller
    }

    /// Get mutable playback controller reference
    pub fn playback_controller_mut(&mut self) -> &mut PlaybackController {
        &mut self.playback_controller
    }

    /// Get animation controller reference
    pub fn animation_controller(&self) -> &AnimationController {
        &self.animation_controller
    }

    /// Get mutable animation controller reference
    pub fn animation_controller_mut(&mut self) -> &mut AnimationController {
        &mut self.animation_controller
    }

    /// Update the project (advance animation)
    pub fn update(&mut self, delta_time: f32) {
        self.playback_controller.update(delta_time, &mut self.timeline);
        self.animation_controller.update(delta_time);
    }

    /// Export timeline as animation data
    pub fn export_animation(&self) -> TimelineExportData {
        TimelineExportData {
            name: self.name.clone(),
            duration: self.timeline.duration(),
            frame_rate: self.timeline.frame_rate(),
            tracks: self.timeline.tracks().to_vec(),
            markers: self.timeline.markers().to_vec(),
            fps: self.timeline.frame_rate(),
        }
    }
}

#[derive(Debug, Clone)]
pub struct TimelineExportData {
    pub name: String,
    pub duration: f32,
    pub frame_rate: f32,
    pub tracks: Vec<TrackInfo>,
    pub markers: Vec<TimelineMarker>,
    pub fps: f32,
}