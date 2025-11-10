use std::fs::{OpenOptions};
use std::io::Write;
use std::path::PathBuf;
use std::time::{Duration, Instant};

/// Lightweight runtime performance tracker for UI and renderer sections.
/// Collects per-frame timings and periodically appends to a CSV log.
pub struct PerformanceTracker {
    frame_start: Option<Instant>,
    last_frame_end: Option<Instant>,
    pub last_frame_ms: f32,
    pub fps_smoothed: f32,
    // Section timings (most recent frame)
    pub ui_top_ms: f32,
    pub ui_left_ms: f32,
    pub ui_right_ms: f32,
    pub ui_bottom_ms: f32,
    pub viewport_ms: f32,
    pub renderer_update_ms: f32,
    // Logging control
    frames_since_log: u32,
    log_every_n_frames: u32,
    log_path: PathBuf,
}

impl Default for PerformanceTracker {
    fn default() -> Self {
        let mut path = PathBuf::from("validation_outputs");
        path.push("perf_log.csv");
        Self {
            frame_start: None,
            last_frame_end: None,
            last_frame_ms: 0.0,
            fps_smoothed: 0.0,
            ui_top_ms: 0.0,
            ui_left_ms: 0.0,
            ui_right_ms: 0.0,
            ui_bottom_ms: 0.0,
            viewport_ms: 0.0,
            renderer_update_ms: 0.0,
            frames_since_log: 0,
            log_every_n_frames: 120, // ~2s at 60 FPS
            log_path: path,
        }
    }
}

impl PerformanceTracker {
    pub fn start_frame(&mut self) {
        self.frame_start = Some(Instant::now());
    }

    pub fn end_frame(&mut self) {
        if let Some(start) = self.frame_start.take() {
            let dur = start.elapsed();
            self.last_frame_ms = duration_to_ms(dur);
            // Simple exponential smoothing for FPS
            let fps = if self.last_frame_ms > 0.0 { 1000.0 / self.last_frame_ms } else { 0.0 };
            if self.fps_smoothed == 0.0 {
                self.fps_smoothed = fps;
            } else {
                self.fps_smoothed = self.fps_smoothed * 0.9 + fps * 0.1;
            }
            self.last_frame_end = Some(Instant::now());
            self.frames_since_log += 1;
        }
    }

    pub fn mark_renderer_update(&mut self, duration: Duration) {
        self.renderer_update_ms = duration_to_ms(duration);
    }

    pub fn mark_ui_top(&mut self, duration: Duration) {
        self.ui_top_ms = duration_to_ms(duration);
    }
    pub fn mark_ui_left(&mut self, duration: Duration) {
        self.ui_left_ms = duration_to_ms(duration);
    }
    pub fn mark_ui_right(&mut self, duration: Duration) {
        self.ui_right_ms = duration_to_ms(duration);
    }
    pub fn mark_ui_bottom(&mut self, duration: Duration) {
        self.ui_bottom_ms = duration_to_ms(duration);
    }
    pub fn mark_viewport(&mut self, duration: Duration) {
        self.viewport_ms = duration_to_ms(duration);
    }

    /// Append a CSV line periodically. Creates the file with header if missing.
    pub fn maybe_log(&mut self) {
        if self.frames_since_log < self.log_every_n_frames { return; }
        self.frames_since_log = 0;

        let header = "timestamp_ms,last_frame_ms,fps,renderer_ms,ui_top_ms,ui_left_ms,ui_right_ms,ui_bottom_ms,viewport_ms\n";
        let line = format!(
            "{},{:.3},{:.2},{:.3},{:.3},{:.3},{:.3},{:.3},{:.3}\n",
            now_millis(),
            self.last_frame_ms,
            self.fps_smoothed,
            self.renderer_update_ms,
            self.ui_top_ms,
            self.ui_left_ms,
            self.ui_right_ms,
            self.ui_bottom_ms,
            self.viewport_ms,
        );

        if let Some(parent) = self.log_path.parent() {
            let _ = std::fs::create_dir_all(parent);
        }

        let file_exists = self.log_path.exists();
        if let Ok(mut file) = OpenOptions::new().create(true).append(true).open(&self.log_path) {
            if !file_exists {
                let _ = file.write_all(header.as_bytes());
            }
            let _ = file.write_all(line.as_bytes());
        }
    }
}

fn duration_to_ms(d: Duration) -> f32 {
    (d.as_secs_f64() * 1000.0) as f32
}

fn now_millis() -> u128 {
    use std::time::SystemTime;
    SystemTime::now().duration_since(SystemTime::UNIX_EPOCH).map(|d| d.as_millis()).unwrap_or(0)
}