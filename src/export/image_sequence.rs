//! Image Sequence Export Module
//!
//! This module handles exporting sequences of images for animations
//! and video production workflows.

use super::*;
use std::path::PathBuf;

/// Image sequence export settings
#[derive(Debug, Clone)]
pub struct ImageSequenceSettings {
    pub base_path: PathBuf,
    pub file_prefix: String,
    pub file_extension: String,
    pub start_frame: u32,
    pub frame_count: u32,
    pub frame_rate: f32,
    pub width: u32,
    pub height: u32,
    pub quality: f32,
}

/// Image sequence exporter
pub struct ImageSequenceExporter {
    settings: ImageSequenceSettings,
    current_frame: u32,
}

impl ImageSequenceExporter {
    /// Create new image sequence exporter
    pub fn new(settings: ImageSequenceSettings) -> Self {
        Self {
            settings,
            current_frame: settings.start_frame,
        }
    }

    /// Export single frame
    pub fn export_frame(&mut self, pixels: &[u8]) -> Result<(), ExportError> {
        let frame_number = self.current_frame;
        let filename = format!("{}_{:04}.{}",
            self.settings.file_prefix,
            frame_number,
            self.settings.file_extension
        );

        let frame_path = self.settings.base_path.join(filename);

        // Ensure output directory exists
        if let Some(parent) = frame_path.parent() {
            std::fs::create_dir_all(parent)?;
        }

        // Export the frame
        ImageExporter::export_image(
            pixels,
            self.settings.width,
            self.settings.height,
            match self.settings.file_extension.as_str() {
                "png" => ExportFormat::PNG,
                "jpg" | "jpeg" => ExportFormat::JPEG,
                "tiff" | "tif" => ExportFormat::TIFF,
                "exr" => ExportFormat::EXR,
                _ => ExportFormat::PNG,
            },
            &frame_path,
        )?;

        self.current_frame += 1;
        Ok(())
    }

    /// Export multiple frames from animation data
    pub fn export_animation_frames(
        &mut self,
        frame_data: &[Vec<u8>],
    ) -> Result<(), ExportError> {
        for frame_pixels in frame_data {
            self.export_frame(frame_pixels)?;
        }
        Ok(())
    }

    /// Generate ffmpeg command for converting sequence to video
    pub fn generate_ffmpeg_command(&self, output_path: &Path) -> String {
        let input_pattern = format!("{}_{{04}}}.{}",
            self.settings.file_prefix,
            self.settings.file_extension
        );

        let input_path = self.settings.base_path.join(input_pattern);

        format!(
            "ffmpeg -framerate {} -i \"{}\" -c:v libx264 -pix_fmt yuv420p -crf 20 \"{}\"",
            self.settings.frame_rate,
            input_path.display(),
            output_path.display()
        )
    }

    /// Get progress (0.0 to 1.0)
    pub fn progress(&self) -> f32 {
        let total_frames = self.settings.frame_count as f32;
        let current = (self.current_frame - self.settings.start_frame) as f32;
        (current / total_frames).min(1.0)
    }

    /// Check if export is complete
    pub fn is_complete(&self) -> bool {
        self.current_frame >= self.settings.start_frame + self.settings.frame_count
    }

    /// Reset exporter to start frame
    pub fn reset(&mut self) {
        self.current_frame = self.settings.start_frame;
    }
}

/// Batch export functionality
pub struct BatchExporter {
    exporters: Vec<ImageSequenceExporter>,
}

impl BatchExporter {
    /// Create new batch exporter
    pub fn new() -> Self {
        Self {
            exporters: Vec::new(),
        }
    }

    /// Add exporter to batch
    pub fn add_exporter(&mut self, exporter: ImageSequenceExporter) {
        self.exporters.push(exporter);
    }

    /// Export frame to all exporters in batch
    pub fn export_frame_batch(&mut self, pixels: &[u8]) -> Result<(), ExportError> {
        for exporter in &mut self.exporters {
            if !exporter.is_complete() {
                exporter.export_frame(pixels)?;
            }
        }
        Ok(())
    }

    /// Get overall progress
    pub fn overall_progress(&self) -> f32 {
        if self.exporters.is_empty() {
            return 1.0;
        }

        let total_progress: f32 = self.exporters.iter().map(|e| e.progress()).sum();
        total_progress / self.exporters.len() as f32
    }

    /// Check if all exports are complete
    pub fn all_complete(&self) -> bool {
        self.exporters.iter().all(|e| e.is_complete())
    }
}

/// Export presets for common formats
pub struct ExportPresets;

impl ExportPresets {
    /// Create settings for high-quality PNG sequence
    pub fn png_sequence(base_path: PathBuf, prefix: &str, frame_count: u32) -> ImageSequenceSettings {
        ImageSequenceSettings {
            base_path,
            file_prefix: prefix.to_string(),
            file_extension: "png".to_string(),
            start_frame: 0,
            frame_count,
            frame_rate: 30.0,
            width: 1920,
            height: 1080,
            quality: 1.0,
        }
    }

    /// Create settings for compressed JPEG sequence
    pub fn jpeg_sequence(base_path: PathBuf, prefix: &str, frame_count: u32, quality: f32) -> ImageSequenceSettings {
        ImageSequenceSettings {
            base_path,
            file_prefix: prefix.to_string(),
            file_extension: "jpg".to_string(),
            start_frame: 0,
            frame_count,
            frame_rate: 30.0,
            width: 1920,
            height: 1080,
            quality,
        }
    }

    /// Create settings for EXR sequence (high dynamic range)
    pub fn exr_sequence(base_path: PathBuf, prefix: &str, frame_count: u32) -> ImageSequenceSettings {
        ImageSequenceSettings {
            base_path,
            file_prefix: prefix.to_string(),
            file_extension: "exr".to_string(),
            start_frame: 0,
            frame_count,
            frame_rate: 30.0,
            width: 1920,
            height: 1080,
            quality: 1.0,
        }
    }

    /// Create settings for 4K export
    pub fn four_k_sequence(base_path: PathBuf, prefix: &str, frame_count: u32) -> ImageSequenceSettings {
        ImageSequenceSettings {
            base_path,
            file_prefix: prefix.to_string(),
            file_extension: "png".to_string(),
            start_frame: 0,
            frame_count,
            frame_rate: 24.0, // Cinema standard
            width: 3840,
            height: 2160,
            quality: 1.0,
        }
    }
}

/// Export queue for managing multiple export jobs
pub struct ExportQueue {
    jobs: Vec<ExportJob>,
}

pub struct ExportJob {
    pub id: String,
    pub exporter: ImageSequenceExporter,
    pub status: ExportStatus,
    pub created_at: std::time::Instant,
}

#[derive(Debug, Clone, PartialEq)]
pub enum ExportStatus {
    Pending,
    InProgress,
    Completed,
    Failed(String),
}

impl ExportQueue {
    /// Create new export queue
    pub fn new() -> Self {
        Self {
            jobs: Vec::new(),
        }
    }

    /// Add job to queue
    pub fn add_job(&mut self, id: String, exporter: ImageSequenceExporter) {
        self.jobs.push(ExportJob {
            id,
            exporter,
            status: ExportStatus::Pending,
            created_at: std::time::Instant::now(),
        });
    }

    /// Process next frame for all active jobs
    pub fn process_frame(&mut self, pixels: &[u8]) -> Result<(), ExportError> {
        for job in &mut self.jobs {
            if matches!(job.status, ExportStatus::Pending | ExportStatus::InProgress) {
                job.status = ExportStatus::InProgress;

                match job.exporter.export_frame(pixels) {
                    Ok(_) => {
                        if job.exporter.is_complete() {
                            job.status = ExportStatus::Completed;
                        }
                    }
                    Err(e) => {
                        job.status = ExportStatus::Failed(format!("{:?}", e));
                        return Err(e);
                    }
                }
            }
        }
        Ok(())
    }

    /// Get job status
    pub fn get_job_status(&self, job_id: &str) -> Option<&ExportStatus> {
        self.jobs.iter().find(|j| j.id == job_id).map(|j| &j.status)
    }

    /// Get overall queue progress
    pub fn queue_progress(&self) -> f32 {
        if self.jobs.is_empty() {
            return 1.0;
        }

        let active_jobs: Vec<_> = self.jobs.iter()
            .filter(|j| matches!(j.status, ExportStatus::InProgress))
            .collect();

        if active_jobs.is_empty() {
            return 1.0;
        }

        let total_progress: f32 = active_jobs.iter().map(|j| j.exporter.progress()).sum();
        total_progress / active_jobs.len() as f32
    }

    /// Check if queue is complete
    pub fn is_complete(&self) -> bool {
        self.jobs.iter().all(|j| matches!(j.status, ExportStatus::Completed))
    }

    /// Cancel job
    pub fn cancel_job(&mut self, job_id: &str) {
        if let Some(job) = self.jobs.iter_mut().find(|j| j.id == job_id) {
            job.status = ExportStatus::Failed("Cancelled by user".to_string());
        }
    }
}